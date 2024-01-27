using Core.PST.BREF;
using Core.PST.Headers.Unicode;
using Core.PST.Pages.Base;
using Core.PST.Pages.BTree;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;

namespace Core.PST.Pages.Unicode
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
                mmf.CreateViewAccessor((long)bref.Ib, this.PageSize))
            {
                this.InternalChildren = new List<BTreePage>();
                this.rgentries = new byte[488];
                view.ReadArray(0, this.rgentries, 0, 488);
                this.cEnt = view.ReadByte(488);//Number of BTree Entries
                this.cEntMax = view.ReadByte(489);//Maximum Number of BTree Entries that can fit inside the page
                this.cbEnt = view.ReadByte(490);//Size of each BTree Entry
                this.cLevel = view.ReadByte(491);//The depth level of this page
                this.dwPadding = view.ReadInt32(492);
                this.pageTrailer = new byte[16];
                view.ReadArray(this.PageSize - 16, this.pageTrailer, 0, 16);
                this.PageTrailer = new UnicodePageTrailer(pageTrailerByte: this.pageTrailer, bref: bref);
                ConfigureBTreeEntries(view, mmf, bTreeType);
            }
        }

        private void ConfigureBTreeEntries(MemoryMappedViewAccessor view, MemoryMappedFile file, BTreeType bTreeType)
        {
            this.BTPageEntries = new List<IBTPageEntry>();
            for (var i = 0; i < this.cEnt; i++)
            {
                byte[] curEntryBytes = new byte[this.cbEnt];
                view.ReadArray(i * this.cbEnt, curEntryBytes, 0, this.cbEnt);
                if (this.cLevel == 0)
                {
                    if (this.BTreeType == BTreeType.NBT)
                    {
                        this.BTPageEntries.Add(new UnicodeNodeBTreeEntry(curEntryBytes));
                    }
                    else
                    {
                        var curEntry = new UnicodeBlockBTreeEntry(curEntryBytes);
                        this.BTPageEntries.Add(curEntry);
                    }
                }
                else
                {
                    //btentries
                    var entry = new UnicodeBTreeEntry(curEntryBytes, this.BTreeType);
                    this.BTPageEntries.Add(entry);
                    using (var views = file.CreateViewAccessor((long)entry.Bref.Ib, 512))
                    {
                        var bytes = new byte[512];
                        view.ReadArray(0, bytes, 0, 512);
                        this.InternalChildren.Add(new UnicodeBTreePage(file, entry.Bref, bTreeType));
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
            if ((int)cLevel == 0) { }

        }
    }
}
