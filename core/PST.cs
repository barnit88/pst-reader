using core.NDBLayer.Headers;
using core.NDBLayer.Pages.BTree;
using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;

namespace core
{
    public class PST
    {
        public Header Header { get; set; }
        public BTreePage NodeBTPage { get; set; }
        public BTreePage BlockBTPage { get; set; }
        public MemoryMappedFile MemoryMappedPSTFile { get; set; }
        public PST(MemoryMappedFile file)
        {
            this.MemoryMappedPSTFile = file;
            this.Header = new Header(file);
            if (this.Header.IsUnicode)
            {
                //Reference to this NDB Layer Pages are stored in root of the header of the PST file
                this.NodeBTPage =
                    new BTreePage(file, this.Header.UnicodeHeader.Root.NBTBREF, BTreeType.NBT,
                    this.Header.UnicodeHeader.EncodingFriendlyName);

                this.BlockBTPage =
                    new BTreePage(file, this.Header.UnicodeHeader.Root.BBTBREF, BTreeType.BBT,
                    this.Header.UnicodeHeader.EncodingFriendlyName);
            }
        }
        //public NodeBTreeEntry GetNodeBTreeEntryFromNid(ulong nid, List<IBTPageEntry> nodeBTPageEntries)
        //{
        //    for (int i = 0; i < nodeBTPageEntries.Count; i++)
        //    {
        //        var currentEntry = nodeBTPageEntries[i];
        //        var nextEntry = nodeBTPageEntries[nodeBTPageEntries.Count == i + 1 ? i : i + 1];

        //        if (currentEntry.BTreePageEntriesType == BTreePageEntriesType.BTENTRY)
        //        {
        //            var currentBTPageEntry = (BTEntry)currentEntry;
        //            var nextBTPageEntry = (BTEntry)nextEntry;
        //            if (nid >= currentBTPageEntry.btkey && nid <= nextBTPageEntry.btkey)
        //                return GetNodeBTreeEntryFromNid(nid, currentBTPageEntry.bTreePage.BTPageEntries);
        //        }
        //        else if (currentEntry.BTreePageEntriesType == BTreePageEntriesType.NBTENTRY)
        //        {
        //            var nodeBTPageEntry = (NodeBTreeEntry)currentEntry;
        //            if (nodeBTPageEntry.nid == nid)
        //                return nodeBTPageEntry;
        //        }
        //        else if (currentEntry.BTreePageEntriesType == BTreePageEntriesType.BBTENTRY)
        //            throw new Exception("GetNidFromBid | Unexpected BBTEntry ");
        //        else
        //            throw new Exception("GetNidFromBid | BTPageEntryType error");
        //    }
        //    throw new Exception("GetNidFromBid | No any NodeBTree found for provided Nid");
        //}

        //public BlockBTreeEntry GetBlockBTreeEntryFromBid(ulong bid, List<IBTPageEntry> blockBTPageEntries,
        //    bool valueFound = false)
        //{
        //    for (int i = 0; i < blockBTPageEntries.Count; i++)
        //    {
        //        var currentEntry = blockBTPageEntries[i];
        //        var nextEntry = blockBTPageEntries[blockBTPageEntries.Count == i + 1 ? i : i + 1];

        //        if (currentEntry.BTreePageEntriesType == BTreePageEntriesType.BTENTRY)
        //        {
        //            var currentBTPageEntry = (BTEntry)currentEntry;
        //            var nextBTPageEntry = (BTEntry)nextEntry;
        //            if (bid >= currentBTPageEntry.btkey && bid <= nextBTPageEntry.btkey)
        //                return GetBlockBTreeEntryFromBid(bid, currentBTPageEntry.bTreePage.BTPageEntries);
        //        }
        //        else if (currentEntry.BTreePageEntriesType == BTreePageEntriesType.BBTENTRY)
        //        {
        //            var blockBTPageEntry = (BlockBTreeEntry)currentEntry;
        //            if (blockBTPageEntry.Bref.bid == bid)
        //                return blockBTPageEntry;
        //        }
        //        else if (currentEntry.BTreePageEntriesType == BTreePageEntriesType.NBTENTRY)
        //            throw new Exception("GetNidFromBid | Unexpected BBTEntry ");
        //        else
        //            throw new Exception("GetNidFromBid | BTPageEntryType error");
        //    }
        //    throw new Exception("GetNidFromBid | No any NodeBTree found for provided Nid");
        //}
    }
}