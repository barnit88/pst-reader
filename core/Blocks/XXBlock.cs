using System;

namespace core.Blocks
{
    /// <summary>
    /// The XXBLOCK further expands the data that is associated with a node by using an array of BIDs that 
    /// reference XBLOCKs.A BLOCKTRAILER is present at the end of an XXBLOCK, and the end of the
    /// BLOCKTRAILER MUST be aligned on a 64-byte boundary.
    /// </summary>
    public class XXBlock
    {
        /// <summary>
        /// btype (1 byte): Block type; MUST be set to 0x01 to indicate an XBLOCK or XXBLOCK.
        /// </summary>
        public byte bytpe { get; set; }
        /// <summary>
        /// cLevel (1 byte): MUST be set to 0x02 to indicate and XXBLOCK.
        /// </summary>
        public byte cLevel { get; set; }
        /// <summary>
        /// cEnt (2 bytes): The count of BID entries in the XXBLOCK.
        /// </summary>
        public UInt16 cEnt{ get; set; }
        /// <summary>
        /// lcbTotal (4 bytes): Total count of bytes of all the external data stored in XBLOCKs under this XXBLOCK.
        /// </summary>
        public UInt32 IcbTotal { get; set; }
        /// <summary>
        /// rgbid(variable) : Array of BIDs that reference XBLOCKs.The size is equal to the number of entries
        /// indicated by cEnt multiplied by the size of a BID (8 bytes for Unicode PST files, 4 bytes for ANSI
        /// PST Files).
        /// </summary>
        public byte[] rgbid { get; set; }
        /// <summary>
        /// rgbPadding (variable, optional): This field is present if the total size of all of the other fields is not 
        /// a multiple of 64. The size of this field is the smallest number of bytes required to make the size of
        /// the XXBLOCK a multiple of 64. Implementations MUST ignore this field.
        /// </summary>
        public byte[] rgbPadding { get; set; }
        /// <summary>
        /// blockTrailer (ANSI: 12 bytes; Unicode: 16 bytes): A BLOCKTRAILER structure
        /// Block Trailer holds the metadata about the block
        /// </summary>
        public BlockTrailer BlockTrailer { get; set; }
    }
}
