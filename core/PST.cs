using core.NDB.Headers;
using core.NDB.Pages.BTree;
using System;
using System.IO.MemoryMappedFiles;

namespace Core.PST
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
                    new BTreePage(file, this.Header.UnicodeHeader.Root.NBTBREF, BTreeType.NBT);

                this.BlockBTPage =
                    new BTreePage(file, this.Header.UnicodeHeader.Root.BBTBREF, BTreeType.BBT);
            }
        }
        public void GetBidFromNid(int nid)
        {
            foreach(var item in this.NodeBTPage.BTPageEntries)
            {
                //var 
            }
            var bTEntry = this.BlockBTPage.BTPageEntries[0] as BTEntry;
            if (bTEntry.BTreeEntryType != BTreeEntryType.NBTreeEntry)
                throw new Exception("Invalid Data from GetBidFromNid");
        }
    }
}