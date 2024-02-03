using core.LTP.BTreeOnHeap;
using core.LTP.HeapNode;
using core.NDBLayer;
using System;
using System.Collections.Generic;

namespace core.LTP.TableContext
{
    /// <summary>
    /// 
    /// Table Context (TC)
    /// A Table Context represents a table with rows of columns.From an implementation perspective, a TC is 
    /// a complex, composite structure that is built on top of an HN. The presence of a TC is indicated at both
    /// the NDB and LTP Layers. At the NDB Layer, a TC is indicated through one of the special NID_TYPEs,
    /// and at the LTP Layer, a value of bTypeTC for bClientSig in the HNHDR structure is reserved for TCs.
    /// The underlying TC data is separated into 3 entries: a header with Column descriptors, a RowIndex (a
    /// nested BTH), and the actual table data(known as the Row Matrix).
    /// The Row Matrix contains the actual row data for the TC.New rows are appended to the end of the Row
    /// Matrix, which means that the rows are not sorted in any meaningful manner.To provide a way to
    /// efficiently search the Row Matrix for a particular data row, each TC also contains an embedded BTH,
    /// known as the RowIndex, to provide a 32-bit "primary index" for the Row Matrix.Each 32-bit value is a
    /// key that uniquely identifies a row within the Row Matrix.
    /// In practice, the Row Matrix is stored in a subnode because of its typical size, but in rare cases, a TC
    /// can fit into a single data block if it is small enough.To facilitate navigation between rows, each row of
    /// data is of the same size, and the size is stored in the TCINFO header structure (section 2.3.4.1). To
    /// further help with data packing and alignment, the data values are grouped according to its
    /// corresponding data size.DWORD and ULONGLONG values are grouped first, followed by WORD-sized
    /// data, and then byte-sized data. The TCINFO structure contains an array of offsets that points to the
    /// starting offset of each group of data.
    /// The TC also includes a construct known as a Cell Existence Bitmap (CEB), which is used to denote
    /// whether a particular column in a particular row actually "exists". A CEB is present at the end of each
    /// row of data in the Row Matrix that indicates which columns in that row exists and which columns don't 
    /// exist.
    /// 
    /// 
    /// Data organization of a Table Context
    /// The preceding example illustrates a typical TC arrangement, where the metadata is stored in the main
    /// data block(a data tree can be used if the TC is large), and the Row Matrix is stored in the
    /// corresponding subnode.Note that the numerical values used in the example are for reference
    /// purposes only.
    /// The hidUserRoot of the HNHDR points to the TC header, which is allocated from the heap with
    /// HID = 0x20.The TC header contains a TCINFO structure, followed by an array of column descriptors.
    /// The TCINFO structure contains pointers that point to the RowIndex (hidRowIndex) and The Row
    /// Matrix (hnidRowData). The RowIndex is allocated off the heap, whereas the Row Matrix is stored in
    /// the subnode (in rare cases where the TC is very small, the Row Matrix can be stored in a heap
    /// allocation instead. Note that the subnode structure in the diagram is significantly simplified for 
    /// illustrative purposes.
    /// 
    /// 
    /// TCINFO
    /// TCINFO is the header structure for the TC.The TCINFO is accessed using the hidUserRoot field in the
    /// HNHDR structure of the containing HN.The header contains the column definitions and other relevant
    /// data.
    /// 
    /// 
    /// TCOLDESC
    /// The TCOLDESC structure describes a single column in the TC, which includes metadata about the size
    /// of the data associated with this column, as well as whether a column exists, and how to locate the
    /// column data from the Row Matrix.
    /// 
    /// 
    /// The RowIndex
    /// The hidRowIndex member in TCINFO points to an embedded BTH that contains an array of 
    /// (dwRowID, dwRowIndex) value pairs, which provides a 32-bit primary index for searching the Row
    /// Matrix.Simply put, the RowIndex maps dwRowID, a unique identifier, to the index of a particular
    /// row in the Row Matrix.
    /// The RowIndex itself is a generic mechanism to provide a 32-bit primary key and therefore it is up to
    /// the implementation to decide what value to use for the primary key.However, an NID value is used as 
    /// the primary key because of its uniqueness within a PST.
    /// The following is the layout of the BTH data record used in the RowIndex.
    /// 
    /// 
    /// 
    /// Row Matrix
    /// The Row Matrix contains the actual data for the rows and columns of the TC.The data is physically
    /// arranged in rows; each row contains the data for each of its columns.Each row of column data in the
    /// Row Matrix is of the same size and is arranged in the same layout, and the size of each row is 
    /// specified in the rgib[TCI_bm] value in the TCINFO header structure. 
    /// However, in many cases, the Row Matrix is larger than 8 kilobytes and therefore cannot fit in a single
    /// data block, which means that a data tree is used to store the Row Matrix in separate data blocks.This
    /// means that the row data is partitioned across two or more data blocks and needs special handling
    /// considerations.
    /// The design of a TC dictates that each data block MUST store an integral number of rows, which means
    /// that rows cannot span across two blocks, and that each block MUST start with a fresh row. This also
    /// means that in order for a client to access a particular row in the Row Matrix, it first calculates how
    /// many rows fit in a block, and calculates the row index within that block at which the row data is 
    /// located.The general formulas to calculate the block index and row index for the Nth row are as 
    /// follows:
    /// Rows per block = Floor((sizeof(block) – sizeof(BLOCKTRAILER)) / TCINFO.rgib[TCI_bm])
    /// Block index = N / (rows per block)
    /// Row index = N % (rows per block)
    /// Each block except the last one MUST have a size of 8192 bytes.If not, the file is considered corrupted.
    /// The size of a block is specified in the formula by sizeof(block).
    /// The following diagram illustrates how the data in the Row Matrix is organized.
    /// 
    /// Data organization of the Row Matrix
    /// In addition to showing the data organization of the Row Matrix, this diagram also illustrates how the
    /// rows in the RowIndex relate to the row data in the Row Matrix.As illustrated by the crossing of dotted
    /// lines between the two structures, the Row Matrix data is unsorted, which makes searching inefficient. 
    /// The RowIndex, which is implemented using an embedded BTH indexed by dwRowID, provides the
    /// primary search key to lookup specific rows in the Row Matrix.
    /// It is also worth noting that because of the fact that partial rows are not allowed, there might be
    /// unused space at the end of the data block(shaded in gray in the diagram). Readers MUST ignore any
    /// such "dead space" and MUST NOT interpret its contents.
    /// 
    /// Row Data Format
    /// The following is the organization of a single row of data in the Row Matrix.Rows of data are tightlypacked in the Row Matrix, and the size of each data row is TCINFO.rgib[TCI_bm] bytes.The
    /// following constraints exist for the columns within the structure.
    /// Columns MUST be sorted
    /// 1. PidTagLtpRowId MUST be assigned iBit == 0
    /// 2. PidTagLtpRowId MUST be assigned ibData == 0
    /// 3. PidTagLtpRowVer MUST be assigned iBit == 1
    /// 4. PidTagLtpRowVer MUST be assigned ibData == 4
    /// 5. For any other columns, iBit can change/be any valid value (other than 0 and 1)
    /// 6. For any other columns, ibData can be any valid value(other than 0 and 4)
    /// 
    ///     BOOL fCEB = !!(rgCEB[iBit / 8] & (1 << (7 - (iBit % 8))));
    /// 
    /// Space is reserved for a column in the Row Matrix, regardless of the corresponding CEB bit value for 
    /// that column.Specifically, an fCEB bit value of TRUE indicates that the corresponding column value in
    /// the Row matrix is valid and SHOULD be returned if requested.However, an fCEB bit value of false
    /// indicates that the corresponding column value in the Row matrix is "not set" or "invalid". In this case,
    /// the property MUST be "not found" if requested.
    /// The size of rgCEB is CEIL(TCINFO.cCols / 8) bytes.Extra lower-order bits SHOULD be ignored.
    /// Creators of a new PST MUST set the extra lower-order bits to zero
    /// 
    /// 
    /// Variable-sized Data
    /// With respect to the TC, variable-sized data is defined as any data type that allows a variable size
    /// (such as strings), or any fixed-size data type that exceeds 8 bytes(for example, a GUID).In the case 
    /// of variable - sized data, the actual data is stored elsewhere in the heap or in a subnode, and the HNID
    /// that references the data is stored the corresponding rgdwData slot instead.The following is a list of
    /// the property types that are stored using an HNID.A complete list of property types is specified in 
    /// [MS-OXCDATA] section 2.11.1.
    /// ▪ PtypString
    /// ▪ PtypString8
    /// ▪ PtypBinary
    /// ▪ PtypObject
    /// ▪ PtypGuid
    /// ▪ All multi-valued types
    /// The following table illustrates the handling of fixed-and variable - sized data in the TC(see section
    /// 2.3.3.2 for determining if an HNID is an HID or an NID).
    ///     
    ///     Variable size ?   |  Fixed data size   |  NID_TYPE(dwValueHnid) == NID_TYPE_HID ?  |   rgdwData value
    ///     N                 |  <= 8 bytes *      |              -                            |   Data value 
    ///                       |  > 8 bytes *       |              Y                            |   HID
    ///     Y                 |  -                 |              Y                            |   HID(<= 3580 bytes)
    ///                       |                    |              N                            |   NID(subnode, > 3580 bytes)
    ///                                                         
    /// This contrasts with the PC in that the TC stores 8 - byte values inline(in rgdwData), whereas a PC
    /// would use an HNID for any data that exceeds 4 - bytes in size.
    /// All property value not stored inline in the Row Matrix are processed as described in section 2.3.3
    /// Property Context(PC).
    /// 
    /// 
    /// Cell Existence Test
    /// Despite the existence of the CEB, the size of each row of column data is still the same for every row.
    /// This means that a data slot exists for a column, whether or not the column exists for that row. 
    /// Because the data slot of a non-existent column contains random values, third-party implementations
    /// MUST first check the CEB to determine if a column exists, and only process the column data if the
    /// column exists.This prevents any confusion resulting from interpreting invalid data from non-existent
    /// columns. Implementations MUST set the value of a non-existent column to zero.
    /// 
    /// </summary>
    public class TableContext
    {
        public TCINFO TCHeader { get; set; }
        public List<TCOLDESC> TCDescriptor { get; set; } = new List<TCOLDESC>();
        public HeapOnNode HeapOnNode { get; set; }
        public BTH RowIndexBTH { get; set; }
        public NodeDataDTO nodeData { get; set; }
        public TCRowMatrix TCRowMatrix { get; set; }
        public Dictionary<uint, uint> ReverseRowIndex;
        public TableContext(NodeDataDTO nodeData)
        {
            this.nodeData = nodeData;
            this.HeapOnNode = new HeapOnNode(nodeData);
            var tcinfoHID = this.HeapOnNode.HeapOnNodeDataBlocks[0].HNHeader.RootHId;
            var tcinfoHIDbytes = HeapOnNode.GetHNHIDBytes(this.HeapOnNode, tcinfoHID);
            this.TCHeader = new TCINFO(tcinfoHIDbytes);
            this.RowIndexBTH = new BTH(this.HeapOnNode, this.TCHeader.HIDRowIndexLocation);
            this.ReverseRowIndex = new Dictionary<uint, uint>();
            foreach (var prop in this.RowIndexBTH.Properties)
            {
                var temp = BitConverter.ToUInt32(prop.Value.Data, 0);
                this.ReverseRowIndex.Add(temp, BitConverter.ToUInt32(prop.Key, 0));
            }
            this.TCRowMatrix = new TCRowMatrix(this, this.RowIndexBTH);
        }
    }
}
