using core.NDBLayer.ID;
using System;

namespace core.NDBLayer.Blocks
{
    /// <summary>
    /// A block trailer are bytes of data that contains metadata about the current block
    /// </summary>
    public class BlockTrailer
    {
        public Bid Bid { get; set; }
        /// <summary>
        /// cb (2 bytes): The amount of data, in bytes, contained within the data section of the block. This value 
        /// does not include the block trailer or any unused bytes that can exist after the end of the data and
        /// before the start of the block trailer.
        /// </summary>
        public short cb { get; set; }
        /// <summary>
        /// wSig (2 bytes): Block signature. See section 5.5 for the algorithm to calculate the block signature.
        /// </summary>
        public short wSig { get; set; }
        /// <summary>
        /// dwCRC (4 bytes): 32-bit CRC of the cb bytes of raw data, see section 5.3 for the algorithm to 
        /// calculate the CRC.Note the locations of the dwCRC and bid are differs between the Unicode 
        /// ANSI version of this structure.
        /// </summary>
        public int dwCRC { get; set; }
        /// <summary>
        /// bid (Unicode: 8 bytes; ANSI 4 bytes): The BID (section 2.2.2.2) of the data block.
        /// </summary>
        public long bid { get; set; }
        public BlockTrailer(byte[] blockTrailerDataBytes)
        {
            if (blockTrailerDataBytes.Length != 16)
                throw new Exception("Block Trailer Data Bytes Lenght Error");
            cb = BitConverter.ToInt16(blockTrailerDataBytes, 0);
            wSig = BitConverter.ToInt16(blockTrailerDataBytes, 2);
            dwCRC = BitConverter.ToInt32(blockTrailerDataBytes, 4);
            bid = BitConverter.ToInt64(blockTrailerDataBytes, 8);
            Bid = new Bid((ulong)bid);
        }
    }
}
