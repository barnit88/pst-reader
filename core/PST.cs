using core.NDB.Headers;
using core.NDB.Pages.Base;
using core.NDB.Pages.BTree;
using core.NDB.Pages.Unicode;
using System.IO.MemoryMappedFiles;

namespace Core.PST
{
    public class PST
    {
        public Header Header { get; set; }
        public Page NodeBTPage { get; set; }
        public Page BlockBTPage { get; set; }

        public PST(MemoryMappedFile file)
        {
            this.Header = new Header(file);
            if (this.Header.IsUnicode)
            {
                this.NodeBTPage =
                    new UnicodeBTreePage(file, this.Header.UnicodeHeader.Root.NBTBREF, BTreeType.NBT);

                this.BlockBTPage =
                    new UnicodeBTreePage(file, this.Header.UnicodeHeader.Root.BBTBREF, BTreeType.BBT);
            }
        }
    }
}