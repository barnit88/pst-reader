using core.NDB.Headers;
using core.NDB.Pages.Base;
using core.NDB.Pages.BTree;
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
                    new BTreePage(file, this.Header.UnicodeHeader.Root.NBTBREF, BTreeType.NBT);

                this.BlockBTPage =
                    new BTreePage(file, this.Header.UnicodeHeader.Root.BBTBREF, BTreeType.BBT);
            }
        }
    }
}