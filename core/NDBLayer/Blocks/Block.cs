using core.NDBLayer.BREF;
using core.NDBLayer.ID;
using System;
using System.IO.MemoryMappedFiles;

namespace core.NDBLayer.Blocks
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
    /// This is a Generic Block implementation.Actual Block which are of
    /// specific type will be stored inside its property.
    /// 
    /// </summary>
    public class Block
    {
        /// <summary>
        /// If Block Type is of XXBlock this will have a value otherwise holds null
        /// </summary>
        public XXBlock XXBlock { get; set; } = null;
        /// <summary>
        /// If Block Type is of XBlock this will have a value otherwise holds null
        /// </summary>
        public XBlock XBlock { get; set; } = null;
        /// <summary>
        /// If Block Type is of XXBlock this will have a value otherwise holds null
        /// </summary>
        public DataBlock DataBlock { get; set; } = null;
        /// <summary>
        /// If Block Type is of XBlock this will have a value otherwise holds null
        /// </summary>
        public SLBlock SLBlock { get; set; } = null;
        /// <summary>
        /// If Block Type is of XBlock this will have a value otherwise holds null
        /// </summary>
        public SIBlock SIBlock { get; set; } = null;
        /// <summary>
        /// Type of Block this Block will reference.
        /// </summary>
        public BlockType BlockType { get; set; }
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
        /// <summary>
        /// This are used to identify which type of intermediate block it is. 
        /// It is found in data portion of Block of type Internal.
        /// Furthe data cLevel is used to identify exact type of intermediate block
        /// </summary>
        public byte btype { get; set; }
        /// <summary>
        /// This are used to identify which type of intermediate block it is. 
        /// It is found in data portion of Block of type Internal.
        /// It is used with btype to identify the exact block.
        /// </summary>
        public byte cLevel { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="file">Memory Mapped File To Access File portions as a virtual memory</param>
        /// <param name="blockDataSize">The count of bytes of the raw data contained in the block referenced by BREF
        /// excluding the block trailer and alignment padding, if any.
        /// This value comes from the cb(2bytes) value in the BlockBTreeEntry 
        /// </param>
        public Block(MemoryMappedFile file, Bref bref, ushort blockDataSize)
        {
            decimal blockDataAndTrailerSize = blockDataSize + 16;
            decimal tempValue = blockDataAndTrailerSize / 64;
            var multipleOfSixtyFourRequiredForActualBlockSize =
                Math.Ceiling(tempValue);
            long actualDataBlockSize = (long)multipleOfSixtyFourRequiredForActualBlockSize * 64;
            int paddingSize = (int)actualDataBlockSize - 16 - blockDataSize;
            padding = new byte[paddingSize];
            using (MemoryMappedViewAccessor view = file.CreateViewAccessor((long)bref.ib, actualDataBlockSize))
            {
                byte[] blockTrailerDataBytes = new byte[16];
                long blockTrailerDataStartPosition = actualDataBlockSize - 16;
                view.ReadArray(blockTrailerDataStartPosition, blockTrailerDataBytes, 0, 16);
                BlockTrailer = new BlockTrailer(blockTrailerDataBytes);
                byte[] blockDataBytes = new byte[blockDataSize];
                view.ReadArray(0, blockDataBytes, 0, blockDataSize);
                if (bref.ExternalOrInternalBid == ExternalOrInternalBid.External)
                {
                    //External. This is a Data Block
                    BlockType = BlockType.DATABLOCK;
                    DataBlock = new DataBlock(blockDataBytes, blockTrailerDataBytes);
                }
                else if (bref.ExternalOrInternalBid == ExternalOrInternalBid.Internal)
                {
                    //Intermediate Block need further calculation for type of block
                    //Determining which type of internal block :- XBLOCK,XXBLOCK,SLBLOCK,SIBLOCK
                    btype = view.ReadByte(0);
                    cLevel = view.ReadByte(1);
                    // XBLOCK or XXBLOCK
                    if (btype == 0x01)
                    {
                        // XBLOCK
                        if (cLevel == 0x01)
                        {

                            BlockType = BlockType.XBLOCK;
                            XBlock = new XBlock(blockDataBytes, blockTrailerDataBytes);
                        }
                        // XXBLOCK
                        else if (cLevel == 0x02)
                        {
                            BlockType = BlockType.XXBLOCK;
                            XXBlock = new XXBlock(blockDataBytes, blockTrailerDataBytes);
                        }
                    }
                    // SLBLOCK OR SIBLOCK
                    else if (btype == 0x02)
                    {
                        //SLBLOCK
                        if (cLevel == 0x00)
                        {
                            BlockType = BlockType.SLBLOCK;
                            SLBlock = new SLBlock(blockDataBytes, blockTrailerDataBytes);
                        }
                        //SIBLOCK
                        if (cLevel == 0x01)
                        {
                            BlockType = BlockType.SIBLOCK;
                            SIBlock = new SIBlock(blockDataBytes, blockTrailerDataBytes);
                        }
                    }
                }
            }
        }
    }
}