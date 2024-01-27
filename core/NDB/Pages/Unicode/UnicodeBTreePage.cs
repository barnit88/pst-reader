using core.NDB.BREF;
using core.NDB.Pages.BTree;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;

namespace core.NDB.Pages.Unicode
{
    /// <summary>
    /// BTrees are widely used throughout the PST file format. 
    /// In the NDB Layer, BTrees are the building blocks for the NBT and BBT, 
    /// which are used to quickly navigate and search nodes and blocks.The PST
    /// file format uses a general BTree implementation that supports up to 8 
    /// intermediate levels.
    /// </summary>
    public class UnicodeBTreePage : BTreePage
    {
        /// <summary>
        /// NDB Layer Contains two BTrees Node BTree and Block BTree
        /// This flag determines we are accessing the page for 
        /// Node BTree.
        /// </summary>
        public bool IsNodePage { get; set; }
        /// <summary>
        /// NDB Layer Contains two BTrees Node BTree and Block BTree
        /// This flag determines we are accessing the page for 
        /// Block BTree
        /// </summary>
        public bool IsBlockPage { get; set; }
        public List<BTreePage> InternalChildren { get; set; }
        public UnicodeBTreePage(MemoryMappedFile mmf, Bref bref, BTreeType bTreeType) : base(bTreeType)
        {
            using (MemoryMappedViewAccessor view =
                mmf.CreateViewAccessor((long)bref.Ib, PageSize))
            {
                InternalChildren = new List<BTreePage>();
                rgentries = new byte[488];
                view.ReadArray(0, rgentries, 0, 488);
                cEnt = view.ReadByte(488);//Number of BTree Entries
                cEntMax = view.ReadByte(489);//Maximum Number of BTree Entries that can fit inside the page
                cbEnt = view.ReadByte(490);//Size of each BTree Entry
                cLevel = view.ReadByte(491);//The depth level of this page
                dwPadding = view.ReadInt32(492);
                pageTrailer = new byte[16];
                view.ReadArray(PageSize - 16, pageTrailer, 0, 16);
                PageTrailer = new UnicodePageTrailer(pageTrailerByte: pageTrailer, bref: bref);
                ConfigureBTreeEntries(view, mmf, bTreeType);
            }
        }

        private void ConfigureBTreeEntries(MemoryMappedViewAccessor view, MemoryMappedFile file, BTreeType bTreeType)
        {
            BTPageEntries = new List<IBTPageEntry>();
            for (var i = 0; i < cEnt; i++)
            {
                byte[] curEntryBytes = new byte[cbEnt];
                view.ReadArray(i * cbEnt, curEntryBytes, 0, cbEnt);
                if (cLevel == 0)
                {
                    if (BTreeType == BTreeType.NBT)
                    {
                        BTPageEntries.Add(new UnicodeNodeBTreeEntry(curEntryBytes));
                    }
                    else
                    {
                        var curEntry = new UnicodeBlockBTreeEntry(curEntryBytes);
                        BTPageEntries.Add(curEntry);
                    }
                }
                else
                {
                    //btentries
                    var entry = new UnicodeBTreeEntry(curEntryBytes, BTreeType);
                    BTPageEntries.Add(entry);
                    using (var views = file.CreateViewAccessor((long)entry.Bref.Ib, 512))
                    {
                        var bytes = new byte[512];
                        view.ReadArray(0, bytes, 0, 512);
                        InternalChildren.Add(new UnicodeBTreePage(file, entry.Bref, bTreeType));
                    }
                    //using (var view = pst.PSTMMF.CreateViewAccessor((long)entry.BREF.IB, 512))
                    //{
                    //    var bytes = new byte[512];
                    //    view.ReadArray(0, bytes, 0, 512);
                    //    this.InternalChildren.Add(new BTPage(bytes, entry.BREF, pst));
                    //}
                }
            }
        }
        private void ConfigurecbEnt(byte cLevel)
        {
            if (cLevel == 0) { }

        }
    }
}
