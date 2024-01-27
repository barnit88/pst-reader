using System.ComponentModel;
using System.Text;

namespace core.Blocks
{
    /// <summary>
    /// Blocks are the fundamental units of data storage at the NDB layer. Blocks are assigned in sizes that 
    /// are multiples of 64 bytes and are aligned on 64-byte boundaries.The maximum size of any block is 8 
    /// kilobytes (8192 bytes). Similar to pages, each block stores its metadata in a block trailer 
    /// placed at the very end of the block so that the end of the trailer is aligned with the end of the block.
    /// Blocks generally fall into one of two categories: data blocks and subnode blocks.Data blocks are used
    /// to store raw data, where subnode blocks are used to represent nodes contained within a node.
    /// The storage capacity of each data block is the size of the data block(from 64 to 8192 bytes) minus
    /// the size of the trailer block.
    /// 
    /// Several types of blocks are defined at the NDB Layer. The following table defines the block type mapping
    /// 
    /// 
    /// 
    ///         Block type          | Data structure   |   Internal BID? |  Header level |  Array content
    ///         Data Tree           | Data block       |   No            |  N/A          |  Bytes
    ///                             | XBLOCK           |   Yes           |  1            |  Data block reference
    ///                             | XXBLOCK          |   Yes           |  2            |  XBLOCK reference
    ///         Subnode BTree data  | SLBLOCK          |   Yes           |  0            |  SLENTRY
    ///                             | SIBLOCK          |   Yes           |  1            |  SIENTRY
    /// 
    /// 
    /// Data Block Encoding/Obfuscation
    /// 
    /// A special case exists when a PST file is configured to encode its contents. In that case, the NDB Layer 
    /// encodes the data field of data blocks to obfuscate the data using one of two 
    /// keyless ciphers.Section 5.1 and section 5.2 contain further information 
    /// about the two cipher algorithms used to encode the
    /// data.Only the data field is encoded.The padding and blockTrailer are not encoded.
    /// 
    /// 
    /// Data Tree
    /// A data tree collectively refers to all the elements that are used to store data. In the simplest case, a 
    /// data tree consists of a single data block, which can hold up to 8,176 bytes.If the data is more than
    /// 8,176 bytes, a construct using XBLOCKs and XXBLOCKs is used to store the data in a series of data
    /// blocks arranged in a tree format.
    /// 
    /// 
    /// </summary>
    public class Block
    {
        /// <summary>
        /// Raw Data. This is  a variable and its size is determined by its corresponding 
        /// Block from the BBTreeEntry
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