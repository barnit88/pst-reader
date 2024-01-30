using core.NDB.ID;
using System;
using System.Collections;
using System.Collections.Generic;

namespace core.NDB.Blocks
{
    /// <summary>
    /// XBLOCKs are used when the data associated with a node data that exceeds 8,176 bytes in size. The 
    /// XBLOCK expands the data that is associated with a node by using an array of BIDs that reference data
    /// blocks that contain the data stream associated with the node.A BLOCKTRAILER is present at the end
    /// of an XBLOCK, and the end of the BLOCKTRAILER MUST be aligned on a 64-byte boundary.
    /// </summary>
    public class XBlock
    {
        public List<Bid> DataBlockBids { get; set; } = new List<Bid>();
        /// <summary>
        /// btype (1 byte): Block type; MUST be set to 0x01 to indicate an XBLOCK or XXBLOCK.
        /// </summary>
        public byte btype { get; set; }
        /// <summary>
        /// cLevel (1 byte): MUST be set to 0x01 to indicate an XBLOCK.
        /// </summary>
        public byte cLevel { get; set; }
        /// <summary>
        /// cEnt (2 bytes): The count of BID entries in the XBLOCK.
        /// </summary>
        public Int16 cEnt { get; set; }
        /// <summary>
        /// lcbTotal (4 bytes):
        /// Total count of bytes of all the external data stored in the data blocks 
        /// referenced by XBLOCK
        /// </summary>
        public UInt32 IcbTotal { get; set; }
        /// <summary>
        /// rgbid (variable): Array of BIDs that reference data blocks. The size is equal to 
        /// the number of entries indicated by cEnt multiplied by the size of a 
        /// BID(8 bytes for Unicode PST files, 4 bytes for ANSI PST files).
        /// </summary>
        public byte[] rgbid { get; set; }
        /// <summary>
        /// rgbPadding (variable, optional): This field is present if the total size of all of the 
        /// other fields is not a multiple of 64. The size of this field is the 
        /// smallest number of bytes required to make the size of
        /// the XBLOCK a multiple of 64. Implementations MUST ignore this field.
        /// </summary>
        public byte[] rgbPadding { get; set; }
        /// <summary>
        /// blockTrailer (ANSI: 12 bytes; Unicode: 16 bytes): A BLOCKTRAILER structure
        /// Block Trailer holds the metadata about the block
        /// </summary>
        public BlockTrailer BlockTrailer { get; set; }
        public XBlock(byte[] xblockBytes, byte[] xblockTrailerBytes)
        {
            this.BlockTrailer = new BlockTrailer(xblockTrailerBytes);
            if (this.BlockTrailer.cb != xblockBytes.Length)
                throw new Exception("XBlock Data Size mismatch");
            this.btype = xblockBytes[0];
            this.cLevel = xblockBytes[1];
            this.cEnt = BitConverter.ToInt16(xblockBytes, 2);
            this.IcbTotal = BitConverter.ToUInt32(xblockBytes,4);
            var rgbidLength = cEnt * 8;
            this.rgbid = new byte[rgbidLength];
            Array.Copy(xblockBytes, 8, rgbid, 0, rgbidLength);
            for(int i = 1; i <= cEnt; i++)
            {
                int position = ((i - 1) * 8);
                var tempBid=  BitConverter.ToUInt64(this.rgbid, position);
                DataBlockBids.Add(new Bid(tempBid));
            }
            if (rgbid.Length != rgbidLength)
                throw new Exception("XBlock , Something wrong in rgbid array");
            this.BlockTrailer = new BlockTrailer(xblockTrailerBytes);
        }
    }
}
