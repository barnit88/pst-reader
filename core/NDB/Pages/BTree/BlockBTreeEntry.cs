using core.NDB.Blocks;
using core.NDB.BREF;
using core.NDB.ID;
using Core.PST;
using System;
using System.Drawing;
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

        #region Flags
        /// <summary>
        /// BREF (Unicode: 16 bytes; ANSI: 8 bytes): BREF structure (section 2.2.2.4) that contains the BID 
        /// and IB of the block that the BBTENTRY references
        /// </summary>
        public UInt64 BREF;
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
    }
}