using core.NDB.Blocks;
using core.NDB.BREF;
using core.NDB.Headers.Unicode;
using Core.PST;
using System;
using System.Drawing;
using System.IO.MemoryMappedFiles;
using System.Net.Http.Headers;
using System.Reflection.Emit;
using System.Xml.Linq;

namespace core.NDB.Pages.BTree
{
    /// <summary>
    /// BBTENTRY records contain information about blocks and are found in BTPAGES 
    /// with cLevel equal to 0, with the ptype of "ptypeBBT". 
    /// These are the leaf entries of the BBT, these structures might not be 
    /// tightly packed and the cbEnt field of the BTPAGE SHOULD
    /// be used to iterate over the entries.
    /// 
    /// 
    /// 
    /// 
    /// 1 Reference Counts
    /// To improve storage efficiency, the NDB supports single-instancing by allowing multiple entities to
    /// reference the same data block.This is supported at the BBT level by having reference counts for 
    /// blocks.
    /// For example, when a node is copied, a new node is created with a new NID, but instead of making a
    /// separate copy of the entire contents of the node, the new node simply references the existing
    /// immediate data and subnode blocks by incrementing the reference count of each block.
    /// The single-instance is only broken when the data referenced needs to be changed by a referencing
    /// node. This requires creation of a new block into which the new data is written and the reference count
    /// to the original block is decremented.When the reference count of a block reaches one, then the block 
    /// is no longer use in use and is marked as "Free" in the corresponding AMap.Finally, the corresponding
    /// leaf BBT entry is removed from the BBT.
    /// In addition to the BBTENTRY, other types of structures can also hold references to a block.The
    /// following is a list of structures that can hold reference counts to a block:
    /// ▪ Leaf BBTENTRY: Any leaf BBT entry that points to a BID holds a reference count to it.
    /// ▪ NBTENTRY: A reference count is held if a block is referenced in the bidData or bidSub fields of a
    /// NBTENTRY.
    /// ▪ SLBLOCK: a reference count is held if a block is referenced in the bidData or bidSub fields of an
    ///     SLENTRY.
    /// ▪ Data tree: A reference count is held if a block is referenced in an rgbid slot of an XBLOCK.
    /// For example, consider a node called "Node1". The data block of Node1 has a reference count of 2 
    /// (BBTENTRY and Node1's NBTENTRY.bidData). If a copy of Node1 is made (Node2), then the block's
    /// reference count becomes 3 (Node2's NBTENTRY.bidData). If a change is made to Node2's data, then
    /// a new data block is created for the modified copy with a reference count of 2 (BBTENTRY, Node2's 
    /// NBTENTRY.bidData), and the reference count of Node1's data block returns to 2 (BBTENTRY, Node1's
    /// NBTENTRY.bidData).
    /// 
    /// 
    /// 
    /// </summary>
    public class BlockBTreeEntry : IBTPageEntry
    {
        /// <summary>
        /// BREF(Unicode: 16 bytes; ANSI: 8 bytes): 
        /// BREF structure that points to the child BTPAGE.
        /// </summary>
        public Bref Bref { get; set; }
        /// <summary>
        /// Blocks
        /// Blocks are the fundamental units of data storage at the NDB layer.
        /// Blocks are assigned in sizes are multiples of 64 bytes and are aligned on 64-byte boundaries. 
        /// The maximum size of any block is kilobytes (8192 bytes).
        /// Similar to pages, each block stores its metadata in a block trailer placed at the 
        /// very end of the block so that the end of the trailer is aligned with the end of the block.
        /// 
        /// Blocks generally fall into one of two categories: 
        /// Data blocks -
        ///     Data blocks are used to store raw data
        /// Subnode blocks -
        ///     Subnode Blocks are used to represent nodes contained within a node.
        /// 
        /// The storage capacity of each data block is the size of the 
        /// data block(from 64 to 8192 bytes) minus the size of the trailer block.
        /// </summary>
        public Block Block { get; set; }

        #region Flags
        /// <summary>
        /// BREF (Unicode: 16 bytes; ANSI: 8 bytes): BREF structure (section 2.2.2.4) that contains the BID 
        /// and IB of the block that the BBTENTRY references
        /// </summary>
        public byte[] BREF = new byte[16];
        /// <summary>
        /// cb (2 bytes): The count of bytes of the raw data contained in the block referenced by BREF
        /// excluding the block trailer and alignment padding, if any.
        /// </summary>
        public UInt16 cb;
        /// <summary>
        /// cRef (2 bytes): Reference count indicating the count of references to this block. See section 
        /// 2.2.2.7.7.3.1 regarding how reference counts work.
        /// Reference Count is just an incremental value that represents how many times this block is 
        /// referenced throughout the PST, this is done to remove dublication 
        /// </summary>
        public UInt16 cRef;
        /// <summary>
        /// dwPadding (Unicode file format only, 4 bytes): Padding; MUST be set to zero.
        /// </summary>
        public UInt32 dwPadding;
        #endregion
        public BlockBTreeEntry(byte[] blockBTreeDataBytes, MemoryMappedFile file)
        {
            byte[] brefDataBytes = new byte[16];
            Array.Copy(blockBTreeDataBytes, 0, brefDataBytes, 0, 16);
            this.BREF = brefDataBytes;
            this.Bref = new Bref(brefDataBytes);
            this.cb = BitConverter.ToUInt16(blockBTreeDataBytes, 16);
            this.cRef = BitConverter.ToUInt16(blockBTreeDataBytes, 18);
            this.dwPadding = 0;
            this.Block = new Block(file, Bref, this.cb);
            //Console.WriteLine("Block Node BTree Entry Information");
            //Console.WriteLine("Raw Data Count contained by this block: " + this.cb);
            //Console.WriteLine("Raw BREF: " + this.BREF);
            //Console.WriteLine("BREF Block Id: " + this.Bref._bId);
            //Console.WriteLine("Bref Block ib(offset): " + this.Bref.Ib);
            //Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++");
        }
    }
}