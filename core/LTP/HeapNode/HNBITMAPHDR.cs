using System;
using System.Linq;

namespace core.LTP.HeapNode
{
    /// <summary>
    ///  HNBITMAPHDR
    ///  Beginning with the eighth data block, a new Fill Level Map is required.An HNBITMAPHDR fulfills this
    ///  requirement.The Fill Level Map in the HNBITMAPHDR can map 128 blocks.This means that an
    ///  HNBITMAPHDR appears at data block 8 (the first data block is data block 0) and thereafter every 128 
    ///  blocks. (that is, data block 8, data block 136, data block 264, and so on).
    /// </summary>
    public class HNBITMAPHDR
    {
        /// <summary>
        /// ibHnpm (2 bytes): The byte offset to the HNPAGEMAP record (section 2.3.1.5) relative to the 
        /// beginning of the HNPAGEHDR structure.
        /// </summary>
        public UInt16 ibHnpm { get; set; }
        /// <summary>
        /// rgbFillLevel(64 bytes): Per-block Fill Level Map.This array consists of one hundred and twentyeight (128) 4-bit values that indicate the fill level for the next 128 data blocks(including this data
        /// block). If the HN has fewer than 128 data blocks after this data block, then the values
        /// corresponding to the non-existent data blocks MUST be set to zero. See rgbFillLevel in section
        /// 2.3.1.2 for possible values.
        /// </summary>
        public byte[] rgbFillLevel { get; set; }
        public HNBITMAPHDR(byte[] dataBytes)
        {
            this.ibHnpm = BitConverter.ToUInt16(dataBytes, 0);
            this.rgbFillLevel = dataBytes.Skip(2).Take(64).ToArray();
        }
    }
}