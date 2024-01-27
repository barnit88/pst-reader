using Core.PST.BREF;
using Core.PST.Pages.Base;
using System;

namespace Core.PST.Pages.Unicode
{
    public class UnicodePageTrailer : PageTrailer
    {
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageTrailerByte">16 byte page trailer data</param>
        /// <param name="bref">BREF info</param>
        public UnicodePageTrailer(byte[] pageTrailerByte, Bref bref) : base(pageTrailerByte[0])
        {
            if (pageTrailerByte.Length != 16)
                throw new Exception("Page Trailer Byte Lenth Error");
            this.ptype = pageTrailerByte[0];
            this.PageType = (PageType)this.ptype;
            this.ptypeRepeat = pageTrailerByte[1];
            this.wSig = BitConverter.ToUInt16(pageTrailerByte, 2);
            this.dwCRC = BitConverter.ToInt32(pageTrailerByte, 4);
            this.bid = BitConverter.ToUInt64(pageTrailerByte, 8);
            if (this.bid != ComputeBid(this.PageType, bref))
                throw new Exception("Unicode Page Trailer Error. Bid Mismatch");
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
                return (ulong)bref.Ib;
            else if ((byte)pageType == 0x83)//ptypePMap
                return (ulong)bref.Ib;
            else if ((byte)pageType == 0x84)//ptypeAMap
                return (ulong)bref.Ib;
            else if ((byte)pageType == 0x85)//ptypeFPMap
                return (ulong)bref.Ib;
            else
                return (ulong)bref._bId;

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
    }
}
