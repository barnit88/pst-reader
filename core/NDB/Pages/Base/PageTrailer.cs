using System;

namespace core.NDB.Pages.Base
{
    /// <summary>
    /// A PAGETRAILER structure contains information about the page in which it is contained. 
    /// PAGETRAILER structure is present at the very end of each page in a PST file.
    /// </summary>
    public class PageTrailer
    {
        /// <summary>
        /// Type of Page
        /// 
        ///   - Block BTree Page
        ///   - Node BTree Page
        ///   - Free Map Page
        ///   - Allocation Page Map Page
        ///   - Allocation Map Page
        ///   - Free Page Map Page
        ///   - Density List Page
        /// 
        /// </summary>
        public PageType PageType { get; set; }
        /// <summary>
        /// This value indicates the type of data contained within the page. This field MUST 
        /// contain one of the following values.
        /// 
        /// Value | Friendlyname | Meaning | wSig Value
        /// 0x80 ptypeBBT Block BTree page. Block or page signature 
        /// 0x81 ptypeNBT Node BTree page.Block or page signature
        /// 0x82 ptypeFMap Free Map page. 0x0000 
        /// 0x83 ptypePMap Allocation Page Map page. 0x0000
        /// 0x84 ptypeAMap Allocation Map page. 0x0000
        /// 0x85 ptypeFPMap Free Page Map page. 0x0000
        /// 0x86 ptypeDL Density List page. Block or page si
        /// 
        /// </summary>
        protected byte ptype;
        /// <summary>
        /// MUST be set to the same value as ptype.
        /// </summary>
        protected byte ptypeRepeat;
        /// <summary>
        /// Page signature. This value depends on the value of the ptype field. This value is 
        /// zero(0x0000) for AMap, PMap, FMap, and FPMap pages.For BBT, NBT, and DList pages, 
        /// a page / block signature is computed
        /// </summary>
        protected ushort wSig;
        /// <summary>
        /// : 32-bit CRC of the page data, excluding the page trailer. See section 5.3 for the 
        /// CRC algorithm.Note the locations of the dwCRC and bid are differs between the Unicode and ANSI
        /// version of this structure
        /// </summary>
        protected int dwCRC;
        public PageTrailer(byte ptype)
        {
            PageType = (PageType)ptype;
        }
    }
}
