using core.NDB.BREF;
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

        #region Flags
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

        /// <summary>
        /// BId
        /// </summary>
        public ulong Bid { get; set; }
        /// <summary>
        /// The BID of the page's block. AMap, PMap, FMap, and FPMap pages 
        /// have a special convention where their BID is assigned the same value 
        /// as their IB(that is,the absolute file offset of the page). 
        /// The bidIndex for other page types are allocated from the
        /// special bidNextP counter in the HEADER structure.
        /// </summary>
        ulong bid;
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageTrailerByte">16 byte page trailer data</param>
        /// <param name="bref">BREF info</param>
        public PageTrailer(byte[] pageTrailerByte, Bref bref)
        {
            PageType = (PageType)pageTrailerByte[0];
            if (pageTrailerByte.Length != 16)
                throw new Exception("Page Trailer Byte Lenth Error");
            this.ptype = pageTrailerByte[0];
            this.PageType = (PageType)ptype;
            this.ptypeRepeat = pageTrailerByte[1];
            this.wSig = BitConverter.ToUInt16(pageTrailerByte, 2);
            this.dwCRC = BitConverter.ToInt32(pageTrailerByte, 4);
            this.bid = BitConverter.ToUInt64(pageTrailerByte, 8);
            if (bid != ComputeBid(PageType, bref))
                throw new Exception("Unicode Page Trailer Error. Bid Mismatch");
        }


        /// <summary>
        /// The BID of the page's block. AMap, PMap, FMap, and FPMap 
        /// pages have a special convention where their BID is assigned the same value 
        /// as their IB(that is,the absolute file offset of the page).
        /// The bidIndex for other page types are allocated from the
        /// special bidNextP counter in the HEADER structure.
        /// </summary>
        /// <param name="pageType"></param>
        /// <param name="bref"></param>
        private ulong ComputeBid(PageType pageType, Bref bref)
        {
            //if ((byte)pageType == 0x82)//ptypeFMap
            //    bid = (ulong)bref.Ib;
            //else if ((byte)pageType == 0x83)//ptypePMap
            //    bid = (ulong)bref.Ib;
            //else if ((byte)pageType == 0x84)//ptypeAMap
            //    bid = (ulong)bref.Ib;
            //else if ((byte)pageType == 0x85)//ptypeFPMap
            //    bid = (ulong)bref.Ib;
            //else
            //    bid = (ulong)bref.BId;
            if ((byte)pageType == 0x82)//ptypeFMap
                return bref.ib;
            else if ((byte)pageType == 0x83)//ptypePMap
                return bref.ib;
            else if ((byte)pageType == 0x84)//ptypeAMap
                return bref.ib;
            else if ((byte)pageType == 0x85)//ptypeFPMap
                return bref.ib;
            else
                return bref.bid;
        }


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
        /// <param name="ptype"></param>
        /// <exception cref="Exception"></exception>
        private void ComputePageType(byte ptype)
        {
            if (ptype == 0x80)
                PageType = PageType.ptypeBBT;
            else if (ptype == 0x81)
                PageType = PageType.ptypeNBT;
            else if (ptype == 0x82)
                PageType = PageType.ptypeFMap;
            else if (ptype == 0x83)
                PageType = PageType.ptypePMap;
            else if (ptype == 0x84)
                PageType = PageType.ptypeAMap;
            else if (ptype == 0x84)
                PageType = PageType.ptypeFPMap;
            else if (ptype == 0x86)
                PageType = PageType.ptypeDL;
            throw new Exception("Invalid Page Type Exception");
        }
        private void CheckPTypeRepeat()
        {
            if (ptypeRepeat != ptype)
            {
                throw new Exception("Page Trailer ptype not match with ptypeRepeat");
            }
        }
        /// <summary>
        /// Page signature. This value depends on the value of the ptype field. 
        /// This value is zero(0x0000) for AMap, PMap, FMap, and FPMap pages.For BBT, NBT, 
        /// and DList pages, a page / block signature is computed(see section 5.5).
        /// 
        ///    Formula:-
        ///    ib ^= bid;
        ///    return (WORD(WORD(ib >> 16) ^ WORD(ib)));
        /// 
        /// A WORD is a 16-bit unsigned integer (range: 0 through 65535 decimal). 
        /// Because a WORD is unsigned, its first bit (Most Significant Bit (MSB)) 
        /// is not reserved for signing.
        /// 
        /// </summary>
        /// <param name="bid"></param>
        /// <param name="ib"></param>
        /// <returns></returns>
        private void ComputeSig(ulong bid, ulong ib)
        {
            ib ^= bid;
            var wsig = (ushort)(ib >> 16 ^ (ushort)ib);
        }
    }
}
