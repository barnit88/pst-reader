using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace core.LTP.HeapNode
{
    /// <summary>
    /// 
    /// HNPAGEMAP
    /// 
    /// The HNPAGEMAP is the last item in the variable length data portion of the block immediately 
    /// following the last heap item.There can be anywhere from 0 to 63 bytes of padding between the
    /// HNPAGEMAP and the block trailer.The beginning of the HNPAGEMAP is aligned on a 2-byte
    /// boundary so there can be an additional 1 byte of padding between the last heap item and the
    /// HNPAGEMAP.
    /// The HNPAGEMAP structure contains the information about the allocations in the page. The
    /// HNPAGEMAP is located using the ibHnpm field in the HNHDR, HNPAGEHDR and HNBITMAPHDR
    /// records.
    /// </summary>
    public class HNPAGEMAP
    {
        /// <summary>
        /// cAlloc (2 bytes): Allocation count. This represents the number of items (allocations) in the HN.
        /// </summary>
        public UInt16 cAlloc { get; set; }
        /// <summary>
        /// cFree (2 bytes): Free count. This represents the number of freed items in the HN.
        /// </summary>
        public UInt16 cFree { get; set; }
        /// <summary>
        /// rgibAlloc (variable): Allocation table. This contains cAlloc + 1 entries. Each entry is a WORD value 
        /// that is the byte offset to the beginning of the allocation.An extra entry exists at the cAlloc + 1st
        /// position to mark the offset of the next available slot.Therefore, the nth allocation starts at offset
        /// rgibAlloc[n] (from the beginning of the HN header), and its size is calculated as 
        /// rgibAlloc[n +1] – rgibAlloc[n] bytes
        /// 
        /// WORD = 2 bytes (16bit)
        /// A WORD is a 16-bit unsigned integer (range: 0 through 65535 decimal). 
        /// Because a WORD is unsigned, its first bit (Most Significant Bit (MSB)) is not reserved for signing.
        ///  typedef unsigned short WORD, *PWORD, *LPWORD;
        /// </summary>
        public List<UInt16> rgibAlloc { get; set; } = new List<UInt16>();
        public HNPAGEMAP(byte[] dataBytes, int offset)
        {
            this.cAlloc = BitConverter.ToUInt16(dataBytes, offset);
            this.cFree = BitConverter.ToUInt16(dataBytes, offset + 2);
            //Allocation table. This contains cAlloc + 1 entries. 
            for (int i =0; i < this.cAlloc + 1; i++) {
                var rgibAllocStartOffset = 4 + (i * 2);
                this.rgibAlloc.Add(BitConverter.ToUInt16(dataBytes, offset + rgibAllocStartOffset));
            }
        }
    }
}
