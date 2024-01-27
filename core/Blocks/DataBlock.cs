namespace core.Blocks
{
    /// <summary>
    /// A Data Block can be called as who follows a Generic Block structure.
    /// This Block is the main block that holds the actual data.
    /// A data block is a block that is "External" (that is, not marked "Internal") and contains data streamed 
    /// from higher layer structures.The data contained in data blocks have no meaning to the structures
    /// defined at the NDB Layer.
    /// </summary>
    public class DataBlock
    {
        /// <summary>
        /// Size of the data is contained in the BBTreeEntry information
        /// BBTreeEntry also contains the block BREF.
        /// </summary>
        public byte[] data { get; set; }
        /// <summary>
        /// Blocks can be of any size in between 64bytes to 8192 bytes. And block should always be 
        /// multiple of 64 bytes. Since data can be of any size and block trailer is of fixed size 
        /// which is 16 byte in UNICODE format. This padding is an extra byte of useless data or space
        /// created to make the block size multiple of 64 bytes
        /// </summary>
        public byte[] padding { get; set; }
        /// <summary>
        /// Block Trailer holds the metadata about the block
        /// </summary>
        public BlockTrailer BlockTrailer { get; set; }
    }
}