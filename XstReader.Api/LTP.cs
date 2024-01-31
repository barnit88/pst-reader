﻿// Project site: https://github.com/iluvadev/XstReader
//
// Based on the great work of Dijji. 
// Original project: https://github.com/dijji/XstReader
//
// Issues: https://github.com/iluvadev/XstReader/issues
// License (Ms-PL): https://github.com/iluvadev/XstReader/blob/master/license.md
//
// Copyright (c) 2016, Dijji, and released under Ms-PL.  This can be found in the root of this distribution. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using XstReader.Common;
using XstReader.Common.BTrees;
using XstReader.ElementProperties;

namespace XstReader
{
    /// <summary>
    /// This class implements the LTP (Lists, Tables and Properties) layer, which provides higher-level concepts 
    /// on top of the NDB, notably the Property Context (PC) and Table Context (TC)
    /// </summary>
    internal class LTP
    {
        // A heap-on-node data block
        private class HNDataBlock
        {
            public int Index;
            public byte[] Buffer;
            public UInt16[] rgibAlloc;

            // In first block only
            public EbType bClientSig;
            public HID hidUserRoot;  // HID that points to the User Root record
        }

        // Used when reading table data to normalise handling of in-line and sub node data storage
        private class RowDataBlock
        {
            public byte[] Buffer;
            public int Offset;
            public int Length;
        }

        private NDB ndb;

        #region Public methods

        public LTP(NDB ndb)
        {
            this.ndb = ndb;
        }

        public BTree<Node> ReadProperties(NID nid, XstPropertySet propertySet)
        {
            BTree<Node> subNodeTree;
            var rn = ndb.LookupNodeAndReadItsSubNodeBtree(nid, out subNodeTree);

            ReadPropertiesInternal(subNodeTree, rn.DataBid, propertySet);

            return subNodeTree;
        }


        // Second form takes a node ID for a node in the supplied sub node tree
        // An optional switch can be used to indicate that the property values are stored in the child node tree of the supplied node tree
        public BTree<Node> ReadProperties<T>(BTree<Node> subNodeTree,
                                             NID nid,
                                             T target,
                                             bool propertyValuesInChildNodeTree = false)
            where T : XstElement
        {
            BTree<Node> childSubNodeTree;
            var rn = ndb.LookupSubNodeAndReadItsSubNodeBtree(subNodeTree, nid, out childSubNodeTree);

            //ReadPropertiesInternal(propertyValuesInChildNodeTree ? childSubNodeTree : subNodeTree, rn.DataBid, target);

            return childSubNodeTree;
        }
        // Second form takes a node ID for a node in the supplied sub node tree
        // An optional switch can be used to indicate that the property values are stored in the child node tree of the supplied node tree
        public BTree<Node> ReadProperties(BTree<Node> subNodeTree, NID nid, XstPropertySet propertySet, bool propertyValuesInChildNodeTree = false)
        {
            BTree<Node> childSubNodeTree;
            var rn = ndb.LookupSubNodeAndReadItsSubNodeBtree(subNodeTree, nid, out childSubNodeTree);

            ReadPropertiesInternal(propertyValuesInChildNodeTree ? childSubNodeTree : subNodeTree, rn.DataBid, propertySet);

            return childSubNodeTree;
        }
        public bool ContainsProperty(NID nid, PropertyCanonicalName tag)
        {
            BTree<Node> subNodeTree;
            var rn = ndb.LookupNodeAndReadItsSubNodeBtree(nid, out subNodeTree);

            return ContainsPropertyInternal(subNodeTree, rn.DataBid, tag);
        }

        public XstProperty ReadProperty(NID nid, PropertyCanonicalName tag)
        {
            BTree<Node> subNodeTree;
            var rn = ndb.LookupNodeAndReadItsSubNodeBtree(nid, out subNodeTree);

            return ReadPropertyInternal(subNodeTree, rn.DataBid, tag);
        }

        // Read all the properties in a property context, apart from a supplied set of exclusions
        // Returns a series of Property objects
        //
        // First form takes a node ID for a node in the main node tree
        public IEnumerable<XstProperty> ReadAllProperties(NID nid, HashSet<PropertyCanonicalName> excluding = null)
        {
            BTree<Node> subNodeTree;
            var rn = ndb.LookupNodeAndReadItsSubNodeBtree(nid, out subNodeTree);

            return ReadAllPropertiesInternal(subNodeTree, rn.DataBid, excluding);
        }

        public bool ContainsProperty(BTree<Node> subNodeTree, NID nid, PropertyCanonicalName tag, bool propertyValuesInChildNodeTree = false)
        {
            BTree<Node> childSubNodeTree;
            var rn = ndb.LookupSubNodeAndReadItsSubNodeBtree(subNodeTree, nid, out childSubNodeTree);

            return ContainsPropertyInternal(propertyValuesInChildNodeTree ? childSubNodeTree : subNodeTree, rn.DataBid, tag);
        }

        public XstProperty ReadProperty(BTree<Node> subNodeTree, NID nid, PropertyCanonicalName tag, bool propertyValuesInChildNodeTree = false)
        {
            BTree<Node> childSubNodeTree;
            var rn = ndb.LookupSubNodeAndReadItsSubNodeBtree(subNodeTree, nid, out childSubNodeTree);

            return ReadPropertyInternal(propertyValuesInChildNodeTree ? childSubNodeTree : subNodeTree, rn.DataBid, tag);
        }

        // Second form takes a node ID for a node in the supplied sub node tree
        // An optional switch can be used to indicate that the property values are stored in the child node tree of the supplied node tree
        public IEnumerable<XstProperty> ReadAllProperties(BTree<Node> subNodeTree, NID nid, HashSet<PropertyCanonicalName> excluding = null, bool propertyValuesInChildNodeTree = false)
        {
            BTree<Node> childSubNodeTree;
            var rn = ndb.LookupSubNodeAndReadItsSubNodeBtree(subNodeTree, nid, out childSubNodeTree);

            return ReadAllPropertiesInternal(propertyValuesInChildNodeTree ? childSubNodeTree : subNodeTree, rn.DataBid, excluding);
        }

        // This is a cutdown version of the table reader to fetch subfolder NIDs from the hierarchy table of a folder,
        // avoiding the overhead of reading the data rows when scanning the folder tree
        public IEnumerable<NID> ReadTableRowIds(NID nid)
        {
            var blocks = ReadHeapOnNode(nid);
            var h = blocks.First();
            if (h.bClientSig != EbType.bTypeTC)
                throw new XstException("Was expecting a table");

            // Read the table information
            var t = MapType<TCINFO>(blocks, h.hidUserRoot);

            // Read the row index and return the NIDs from it
            if (ndb.IsUnicode)
                return ReadBTHIndex<TCROWIDUnicode>(blocks, t.hidRowIndex).Select(r => new NID(r.dwRowID));
            else
                return ReadBTHIndex<TCROWIDANSI>(blocks, t.hidRowIndex).Select(r => new NID(r.dwRowID));
        }

        // This is the full-scale table reader, that reads all of the data in a table
        // T is the type of the row object to be populated
        //
        // First form takes a node ID for a node in the main node tree
        public IEnumerable<T> ReadTable<T>(NID nid,
                                           Action<T, UInt32> idGetter,
                                           Action<T> postProcessAction)
            where T : XstElement, new()
        {
            BTree<Node> subNodeTree;
            var rn = ndb.LookupNodeAndReadItsSubNodeBtree(nid, out subNodeTree);
            if (rn == null)
                throw new XstException("Node block does not exist");

            return ReadTableInternal(subNodeTree, rn.DataBid, idGetter, postProcessAction);
        }

        // Second form takes a node ID for a node in the supplied sub node tree
        public IEnumerable<T> ReadTable<T>(BTree<Node> subNodeTree,
                                           NID nid,
                                           Action<T, UInt32> idGetter,
                                           Action<T> postProcessAction)
            where T : XstElement, new()
        {
            BTree<Node> childSubNodeTree;
            var rn = ndb.LookupSubNodeAndReadItsSubNodeBtree(subNodeTree, nid, out childSubNodeTree);
            if (rn == null)
                throw new XstException("Node block does not exist");

            return ReadTableInternal(childSubNodeTree, rn.DataBid, idGetter, postProcessAction);
        }

        // Test for the  presence of an optional table in the supplied sub node tree
        public bool IsTablePresent(BTree<Node> subNodeTree, NID nid)
            => (subNodeTree != null && NDB.LookupSubNode(subNodeTree, nid) != null);

        #endregion

        #region Private methods

         private void ReadPropertiesInternal(BTree<Node> subNodeTree, UInt64 dataBid, XstPropertySet propertySet)
        {
            var blocks = ReadHeapOnNode(dataBid);
            var h = blocks.First();
            if (h.bClientSig != EbType.bTypePC)
                throw new XstException("Was expecting a PC");

            // Read the index of properties
            var props = ReadBTHIndex<PCBTH>(blocks, h.hidUserRoot).ToArray();

            foreach (var prop in props)
            {
                XstProperty p = CreatePropertyObject(prop, () => ReadPropertyValue(subNodeTree, blocks, prop));
                propertySet.Add(p);
            }
        }


        // Common implementation of property reading takes a data ID for a block in the main block tree
        private IEnumerable<XstProperty> ReadAllPropertiesInternal(BTree<Node> subNodeTree, UInt64 dataBid, HashSet<PropertyCanonicalName> excluding)
        {
            var blocks = ReadHeapOnNode(dataBid);
            var h = blocks.First();
            if (h.bClientSig != EbType.bTypePC)
                throw new XstException("Was expecting a PC");

            // Read the index of properties
            var props = ReadBTHIndex<PCBTH>(blocks, h.hidUserRoot).ToArray();

            foreach (var prop in props)
            {
                if (excluding != null && excluding.Contains(prop.wPropId))
                    continue;

                XstProperty p = CreatePropertyObject(prop, () => ReadPropertyValue(subNodeTree, blocks, prop));

                yield return p;
            }

            yield break;
        }

        private bool ContainsPropertyInternal(BTree<Node> subNodeTree, UInt64 dataBid, PropertyCanonicalName tag)
        {
            var blocks = ReadHeapOnNode(dataBid);
            var h = blocks.First();
            if (h.bClientSig != EbType.bTypePC)
                throw new XstException("Was expecting a PC");

            // Read the index of properties
            var props = ReadBTHIndex<PCBTH>(blocks, h.hidUserRoot);
            return props.Any(p => p.wPropId == tag);
        }

        // Common implementation of property reading takes a data ID for a block in the main block tree
        private XstProperty ReadPropertyInternal(BTree<Node> subNodeTree, UInt64 dataBid, PropertyCanonicalName tag)
        {
            var blocks = ReadHeapOnNode(dataBid);
            var h = blocks.First();
            if (h.bClientSig != EbType.bTypePC)
                throw new XstException("Was expecting a PC");

            // Read the index of properties
            var props = ReadBTHIndex<PCBTH>(blocks, h.hidUserRoot);
            XstProperty p = null;
            foreach (var prop in props)
            {
                if (tag == prop.wPropId)
                {
                    p = CreatePropertyObject(prop, () => ReadPropertyValue(subNodeTree, blocks, prop));
                    break;
                }
            }
            return p;
        }


        private dynamic ReadPropertyValue(BTree<Node> subNodeTree, List<HNDataBlock> blocks, PCBTH prop)
        {
            dynamic val = null;
            byte[] buf = null;

            switch (prop.wPropType)
            {
                case PropertyType.PT_SHORT:
                    val = (Int16)prop.dwValueHnid.dwValue;
                    break;

                case PropertyType.PT_LONG:
                    val = (Int32)prop.dwValueHnid.dwValue;
                    break;

                case PropertyType.PT_LONGLONG:
                    buf = GetBytesForHNID(blocks, subNodeTree, prop.dwValueHnid);

                    if (buf == null)
                        val = "<Could not read Integer64 value>";
                    else
                        val = Map.MapType<Int64>(buf);
                    break;

                case PropertyType.PT_DOUBLE:
                    buf = GetBytesForHNID(blocks, subNodeTree, prop.dwValueHnid);

                    if (buf == null)
                        val = "<Could not read Floating64 value>";
                    else
                        val = Map.MapType<Double>(buf);
                    break;

                case PropertyType.PT_MV_LONG:
                    buf = GetBytesForHNID(blocks, subNodeTree, prop.dwValueHnid);

                    if (buf == null)
                        val = "<Could not read MultipleInteger32 value>";
                    else
                        val = Map.MapArray<Int32>(buf, 0, buf.Length / sizeof(Int32));
                    break;

                case PropertyType.PT_BOOLEAN:
                    val = (prop.dwValueHnid.dwValue == 0x01);
                    break;

                case PropertyType.PT_BINARY:
                    if (prop.dwValueHnid.HasValue && prop.dwValueHnid.hidType != EnidType.HID && prop.wPropId == PropertyCanonicalName.PidTagAttachDataBinary)
                    {
                        // Special case for out of line attachment contents: don't dereference to binary yet
                        val = prop.dwValueHnid.NID;
                    }
                    else
                    {
                        buf = GetBytesForHNID(blocks, subNodeTree, prop.dwValueHnid);

                        if (buf == null)
                            val = null;
                        else
                            val = buf;
                    }
                    break;

                case PropertyType.PT_UNICODE: // Unicode string
                    if (!prop.dwValueHnid.HasValue)
                        val = "";
                    else
                    {
                        buf = GetBytesForHNID(blocks, subNodeTree, prop.dwValueHnid);

                        if (buf == null)
                            val = "<Could not read string value>";
                        else
                        {
                            val = Encoding.Unicode.GetString(buf, 0, buf.Length);
                        }
                    }
                    break;

                case PropertyType.PT_STRING8:  // Multipoint string in variable encoding
                    if (!prop.dwValueHnid.HasValue)
                        val = "";
                    else
                    {
                        buf = GetBytesForHNID(blocks, subNodeTree, prop.dwValueHnid);

                        if (buf == null)
                            val = "<Could not read string value>";
                        else
                            val = Encoding.UTF8.GetString(buf, 0, buf.Length);
                    }
                    break;

                case PropertyType.PT_MV_TSTRING: // Unicode strings
                    if (!prop.dwValueHnid.HasValue)
                        val = "";
                    else
                    {
                        buf = GetBytesForHNID(blocks, subNodeTree, prop.dwValueHnid);

                        if (buf == null)
                            val = "<Could not read MultipleString value>";
                        else
                        {
                            var count = Map.MapType<UInt32>(buf);
                            var offsets = Map.MapArray<UInt32>(buf, sizeof(UInt32), (int)count);
                            var ss = new string[count];

                            // Offsets are relative to the start of the buffer
                            for (int i = 0; i < count; i++)
                            {
                                int len;
                                if (i < count - 1)
                                    len = (int)(offsets[i + 1] - offsets[i]);
                                else
                                    len = buf.Length - (int)offsets[i];

                                ss[i] = Encoding.Unicode.GetString(buf, (int)offsets[i], len);
                            }
                            val = ss;
                        }
                    }
                    break;

                case PropertyType.PT_MV_BINARY:
                    if (!prop.dwValueHnid.HasValue)
                        val = null;
                    else
                    {
                        buf = GetBytesForHNID(blocks, subNodeTree, prop.dwValueHnid);

                        if (buf == null)
                            val = "<Could not read MultipleBinary value>";
                        else
                        {
                            var count = Map.MapType<UInt32>(buf);
                            var offsets = Map.MapArray<UInt32>(buf, sizeof(UInt32), (int)count);
                            var bs = new List<byte[]>();

                            // Offsets are relative to the start of the buffer
                            for (int i = 0; i < count; i++)
                            {
                                int len;
                                if (i < count - 1)
                                    len = (int)(offsets[i + 1] - offsets[i]);
                                else
                                    len = buf.Length - (int)offsets[i];

                                var b = new byte[len];
                                Array.Copy(buf, offsets[i], b, 0, len);
                                bs.Add(b);
                            }
                            val = bs;
                        }
                    }
                    break;

                case PropertyType.PT_SYSTIME:
                    // In a Property Context, time values are references to data
                    buf = GetBytesForHNID(blocks, subNodeTree, prop.dwValueHnid);

                    if (buf != null)
                    {
                        var fileTime = Map.MapType<Int64>(buf);
                        val = DateTime.FromFileTimeUtc(fileTime);
                    }
                    break;

                case PropertyType.PT_CLSID:
                    buf = GetBytesForHNID(blocks, subNodeTree, prop.dwValueHnid);

                    if (buf == null)
                        val = "<Could not read Guid value>";
                    else
                        val = new Guid(buf);
                    break;

                case PropertyType.PT_OBJECT:
                    buf = GetBytesForHNID(blocks, subNodeTree, prop.dwValueHnid);

                    if (buf == null)
                        val = "<Could not read Object value>";
                    else
                        val = Map.MapType<PtypObjectValue>(buf);
                    break;

                default:
                    val = String.Format("Unsupported property type {0}", prop.wPropType);
                    break;
            }

            return val;
        }

        private XstProperty CreatePropertyObject(PCBTH prop, Func<dynamic> valGetter)
            => new XstProperty(prop.wPropId, prop.wPropType, valGetter);

        private XstProperty CreatePropertyObject(TCOLDESC col, Func<dynamic> valGetter)
            => new XstProperty(col.wPropId, col.wPropType, valGetter);


        // Common implementation of table reading takes a data ID for a block in the main block tree
        private IEnumerable<T> ReadTableInternal<T>(BTree<Node> subNodeTree,
                                                    UInt64 dataBid,
                                                    Action<T, UInt32> idGetter,
                                                    Action<T> postProcessAction)
            where T : XstElement, new()
        {
            var blocks = ReadHeapOnNode(dataBid);
            var h = blocks.First();
            if (h.bClientSig != EbType.bTypeTC)
                throw new XstException("Was expecting a table");

            // Read the table information
            var t = MapType<TCINFO>(blocks, h.hidUserRoot);

            // Read the column descriptions
            var cols = MapArray<TCOLDESC>(blocks, h.hidUserRoot, t.cCols, Marshal.SizeOf(typeof(TCINFO)));

            // Read the row index
            TCROWIDUnicode[] indexes;
            if (ndb.IsUnicode)
                indexes = ReadBTHIndex<TCROWIDUnicode>(blocks, t.hidRowIndex).ToArray();
            else
                // For ANSI, convert the index entries to the slightly more capacious Unicode equivalents
                indexes = ReadBTHIndex<TCROWIDANSI>(blocks, t.hidRowIndex).Select(e => new TCROWIDUnicode { dwRowID = e.dwRowID, dwRowIndex = e.dwRowIndex }).ToArray();

            // The data rows may be held in line, or in a sub node
            if (t.hnidRows.IsHID)
            {
                // Data is in line
                var buf = GetBytesForHNID(blocks, subNodeTree, t.hnidRows);
                var dataBlocks = new List<RowDataBlock>
                {
                    new RowDataBlock
                    {
                        Buffer = buf,
                        Offset = 0,
                        Length = buf.Length,
                    }
                };
                return ReadTableData<T>(t, blocks, dataBlocks, cols, subNodeTree, indexes, idGetter, postProcessAction);
            }
            else if (t.hnidRows.NID.HasValue)
            {
                // Don't use GetBytesForHNID in this case, as we need to handle multiple blocks
                var dataBlocks = ReadSubNodeRowDataBlocks(subNodeTree, t.hnidRows.NID);
                return ReadTableData<T>(t, blocks, dataBlocks, cols, subNodeTree, indexes, idGetter, postProcessAction);
            }
            else
                return Enumerable.Empty<T>();
        }


        // Read the data rows of a table, populating the members of target type T as specified by the supplied property getters, and optionally getting all columns as properties
        private IEnumerable<T> ReadTableData<T>(TCINFO t,
                                                List<HNDataBlock> blocks,
                                                List<RowDataBlock> dataBlocks,
                                                TCOLDESC[] cols,
                                                BTree<Node> subNodeTree,
                                                TCROWIDUnicode[] indexes,
                                                Action<T, UInt32> idGetter,
                                                Action<T> postProcessAction)
            where T : XstElement, new()
        {
            int rgCEBSize = (int)Math.Ceiling((decimal)t.cCols / 8);
            int rowsPerBlock;
            if (ndb.IsUnicode4K)
                rowsPerBlock = (ndb.BlockSize4K - Marshal.SizeOf(typeof(BLOCKTRAILERUnicode4K))) / t.rgibTCI_bm;
            else if (ndb.IsUnicode)
                rowsPerBlock = (ndb.BlockSize - Marshal.SizeOf(typeof(BLOCKTRAILERUnicode))) / t.rgibTCI_bm;
            else
                rowsPerBlock = (ndb.BlockSize - Marshal.SizeOf(typeof(BLOCKTRAILERANSI))) / t.rgibTCI_bm;

            foreach (var index in indexes)
            {
                T row = new T();

                // Retrieve the node ID that accesses the message
                idGetter?.Invoke(row, index.dwRowID);

                // If we were asked for all column values as properties, read them and store them
                int blockNum = (int)(index.dwRowIndex / rowsPerBlock);
                if (blockNum >= dataBlocks.Count)
                    throw new XstException("Data block number out of bounds");

                var db = dataBlocks[blockNum];
                long rowOffset = db.Offset + (index.dwRowIndex % rowsPerBlock) * t.rgibTCI_bm;
                if (rowOffset + t.rgibTCI_bm > db.Offset + db.Length)
                    throw new XstException("Out of bounds reading table data");

                // Read the column existence data
                var rgCEB = Map.MapArray<Byte>(db.Buffer, (int)(rowOffset + t.rgibTCI_1b), rgCEBSize);

                // Check if the column exists
                foreach (var col in cols.Where(c => (rgCEB[c.iBit / 8] & (0x01 << (7 - (c.iBit % 8)))) != 0))
                    row.AddProperty(CreatePropertyObject(col, () => ReadTableColumnValue(subNodeTree, blocks, db, rowOffset, col)));

                postProcessAction?.Invoke(row);

                yield return row;
            }
            yield break; // No more entries
        }

        private dynamic ReadTableColumnValue(BTree<Node> subNodeTree, List<HNDataBlock> blocks, RowDataBlock db, long rowOffset, TCOLDESC col)
        {
            dynamic val = null;
            HNID hnid;

            switch (col.wPropType)
            {
                case PropertyType.PT_LONG:
                    if (col.cbData != 4)
                        throw new XstException("Unexpected property length");
                    val = Map.MapType<Int32>(db.Buffer, (int)rowOffset + col.ibData);
                    break;

                case PropertyType.PT_BOOLEAN:
                    val = (db.Buffer[rowOffset + col.ibData] == 0x01);
                    break;

                case PropertyType.PT_BINARY:
                    if (col.cbData != 4)
                        throw new XstException("Unexpected property length");
                    hnid = Map.MapType<HNID>(db.Buffer, (int)rowOffset + col.ibData);

                    if (!hnid.HasValue)
                        val = "";
                    else
                    {
                        var buf = GetBytesForHNID(blocks, subNodeTree, hnid);

                        if (buf == null)
                            val = null;
                        else
                            val = buf;
                    }
                    break;

                case PropertyType.PT_UNICODE:  // Unicode string
                    if (col.cbData != 4)
                        throw new XstException("Unexpected property length");
                    hnid = Map.MapType<HNID>(db.Buffer, (int)rowOffset + col.ibData);

                    if (!hnid.HasValue)
                        val = "";
                    else
                    {
                        var buf = GetBytesForHNID(blocks, subNodeTree, hnid);

                        if (buf == null)
                            val = "<Could not read string value>";
                        else
                        {
                            int skip = 0;
                            if (col.wPropId == PropertyCanonicalName.PidTagSubject)
                                if (buf[0] == 0x01 && buf[1] == 0x00)  // Unicode 0x01
                                    skip = 4;
                            val = Encoding.Unicode.GetString(buf, skip, buf.Length - skip);
                        }
                    }
                    if (val == "" && col.wPropId == PropertyCanonicalName.PidTagSubject)
                        val = "<No subject>";
                    break;

                case PropertyType.PT_STRING8: // Multibyte string in variable encoding
                    if (col.cbData != 4)
                        throw new XstException("Unexpected property length");
                    hnid = Map.MapType<HNID>(db.Buffer, (int)rowOffset + col.ibData);

                    if (!hnid.HasValue)
                        val = "";
                    else
                    {
                        var buf = GetBytesForHNID(blocks, subNodeTree, hnid);

                        if (buf == null)
                            val = "<Could not read string value>";
                        else
                        {
                            int skip = 0;

                            if (col.wPropId == PropertyCanonicalName.PidTagSubject)
                                if (buf[0] == 0x01)  // ANSI 0x01
                                    skip = 2;
                            val = Encoding.UTF8.GetString(buf, skip, buf.Length - skip);
                        }
                    }
                    if (val == "" && col.wPropId == PropertyCanonicalName.PidTagSubject)
                        val = "<No subject>";
                    break;

                case PropertyType.PT_SYSTIME:
                    // In a Table Context, time values are held in line
                    if (col.cbData != 8)
                        throw new XstException("Unexpected property length");
                    var fileTime = Map.MapType<Int64>(db.Buffer, (int)rowOffset + col.ibData);
                    try
                    {
                        val = DateTime.FromFileTimeUtc(fileTime);
                    }
                    catch (System.ArgumentOutOfRangeException)
                    {
                        val = null;
                    }
                    break;

                default:
                    val = String.Format("Unsupported property type {0}", col.wPropType);
                    break;
            }

            return val;
        }

        // Walk a b-tree implemented on a heap, and return all the type T entries
        // This is used when reading the index of properties in a PC (property context), or rows in a TC (table context)
        private IEnumerable<T> ReadBTHIndex<T>(List<HNDataBlock> blocks, HID hid)
        {
            var b = MapType<BTHHEADER>(blocks, hid);
            foreach (var row in ReadBTHIndexHelper<T>(blocks, b.hidRoot, b.bIdxLevels))
                yield return row;

            yield break; // No more entries
        }

        private IEnumerable<T> ReadBTHIndexHelper<T>(List<HNDataBlock> blocks, HID hid, int level)
        {
            if (level == 0)
            {
                int recCount = HidSize(blocks, hid) / Marshal.SizeOf(typeof(T));
                if (hid.GetIndex(ndb.IsUnicode4K) != 0)
                {
                    // The T record also forms the key of the BTH entry
                    foreach (var row in MapArray<T>(blocks, hid, recCount))
                        yield return row;
                }
            }
            else
            {
                int recCount = HidSize(blocks, hid) / Marshal.SizeOf(typeof(IntermediateBTH4));
                var inters = MapArray<IntermediateBTH4>(blocks, hid, recCount);

                foreach (var inter in inters)
                {
                    foreach (var row in ReadBTHIndexHelper<T>(blocks, inter.hidNextLevel, level - 1))
                        yield return row;
                }
            }
            yield break; // No more entries
        }


        // Read all of the data blocks for a table, in the case where the rows are to be accessed via a sub node
        // The variation here is that for reading rows, we need to retain the block structure, so we return a set of blocks
        private List<RowDataBlock> ReadSubNodeRowDataBlocks(BTree<Node> subNodeTree, NID nid)
        {
            var blocks = new List<RowDataBlock>();
            var n = NDB.LookupSubNode(subNodeTree, nid);
            if (n == null)
                throw new XstException("Sub node NID not found");
            if (n.SubDataBid != 0)
                throw new XstException("Sub-nodes of sub-nodes not yet implemented");

            foreach (var buf in ndb.ReadDataBlocks(n.DataBid))
            {
                blocks.Add(new RowDataBlock
                {
                    Buffer = buf,
                    Offset = 0,
                    Length = buf.Length,
                });
            }

            return blocks;
        }

        // Read a heap on node data structure referenced by another node
        private List<HNDataBlock> ReadHeapOnNode(NID nid)
        {
            var rn = ndb.LookupNode(nid);
            if (rn == null)
                throw new XstException("Node block does not exist");
            return ReadHeapOnNode(rn.DataBid);
        }

        // Read a heap on node data structure. The division of data into blocks is preserved,
        // because references into it have two parts: block index, and offset within block
        private List<HNDataBlock> ReadHeapOnNode(UInt64 dataBid)
        {
            var blocks = new List<HNDataBlock>();

            int index = 0;
            foreach (var buf in ndb.ReadDataBlocks(dataBid))
            {
                // First block contains a HNHDR
                if (index == 0)
                {
                    var h = Map.MapType<HNHDR>(buf, 0);
                    var pm = Map.MapType<HNPAGEMAP>(buf, h.ibHnpm);
                    var b = new HNDataBlock
                    {
                        Index = index,
                        Buffer = buf,
                        bClientSig = h.bClientSig,
                        hidUserRoot = h.hidUserRoot,
                        rgibAlloc = Map.MapArray<UInt16>(buf, h.ibHnpm + Marshal.SizeOf(pm), pm.cAlloc + 1),  //+1 to get the dummy entry that gives us the size of the last one
                    };
                    blocks.Add(b);
                }
                // Blocks 8, 136, then every 128th contains a HNBITMAPHDR
                else if (index == 8 || (index >= 136 && (index - 8) % 128 == 0))
                {
                    var h = Map.MapType<HNBITMAPHDR>(buf, 0);
                    var pm = Map.MapType<HNPAGEMAP>(buf, h.ibHnpm);
                    var b = new HNDataBlock
                    {
                        Index = index,
                        Buffer = buf,
                        rgibAlloc = Map.MapArray<UInt16>(buf, h.ibHnpm + Marshal.SizeOf(pm), pm.cAlloc + 1),  //+1 to get the dummy entry that gives us the size of the last one
                    };
                    blocks.Add(b);
                }
                // All other blocks contain a HNPAGEHDR
                else
                {
                    var h = Map.MapType<HNPAGEHDR>(buf, 0);
                    var pm = Map.MapType<HNPAGEMAP>(buf, h.ibHnpm);
                    var b = new HNDataBlock
                    {
                        Index = index,
                        Buffer = buf,
                        rgibAlloc = Map.MapArray<UInt16>(buf, h.ibHnpm + Marshal.SizeOf(pm), pm.cAlloc + 1),  //+1 to get the dummy entry that gives us the size of the last one
                    };
                    blocks.Add(b);
                }
                index++;
            }
            return blocks;
        }

        // Used in reading property contexts and table contexts to get a data value which might be held either on the local heap, or in a sub node
        // The value is returned as a byte array: the caller should convert it to a specific type if required
        private byte[] GetBytesForHNID(List<HNDataBlock> blocks, BTree<Node> subNodeTree, HNID hnid)
        {
            byte[] buf = null;

            if (hnid.hidType == EnidType.HID)
            {
                if (hnid.GetIndex(ndb.IsUnicode4K) != 0)
                {
                    buf = MapArray<byte>(blocks, hnid.HID, HidSize(blocks, hnid.HID));
                }
            }
            else if (hnid.nidType == EnidType.LTP)
            {
                buf = ndb.ReadSubNodeDataBlock(subNodeTree, hnid.NID);
            }
            else
                throw new XstException("Data storage style not implemented");

            return buf;
        }

        // Dereference the supplied HID in the supplied heap-on-node blocks,
        // and return the size of the resulting data buffer
        private int HidSize(List<HNDataBlock> blocks, HID hid)
        {
            var index = hid.GetIndex(ndb.IsUnicode4K);
            if (index == 0) // Check for empty
                return 0;
            var b = blocks[hid.GetBlockIndex(ndb.IsUnicode4K)];
            return b.rgibAlloc[index] - b.rgibAlloc[index - 1];
        }

        // Dereference the supplied HID in the supplied heap-on-node blocks,
        // and map the resulting data buffer onto the specified type T
        private T MapType<T>(List<HNDataBlock> blocks, HID hid, int offset = 0)
        {
            var b = blocks[hid.GetBlockIndex(ndb.IsUnicode4K)];
            return Map.MapType<T>(b.Buffer, b.rgibAlloc[hid.GetIndex(ndb.IsUnicode4K) - 1] + offset);
        }

        // Dereference the supplied HID in the supplied heap-on-node blocks,
        // and map the resulting data buffer onto an array of count occurrences of the specified type T
        private T[] MapArray<T>(List<HNDataBlock> blocks, HID hid, int count, int offset = 0)
        {
            var b = blocks[hid.GetBlockIndex(ndb.IsUnicode4K)];
            return Map.MapArray<T>(b.Buffer, b.rgibAlloc[hid.GetIndex(ndb.IsUnicode4K) - 1] + offset, count);
        }
        #endregion
    }
}
