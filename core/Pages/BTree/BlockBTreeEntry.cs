using Core.PST.Headers.Unicode;
using System;

namespace Core.PST.Pages.BTree
{
    /// <summary>
    /// BBTENTRY records contain information about blocks and are found in BTPAGES 
    /// with cLevel equal to 0, with the ptype of "ptypeBBT". 
    /// These are the leaf entries of the BBT, these structures might not be 
    /// tightly packed and the cbEnt field of the BTPAGE SHOULD
    /// be used to iterate over the entries.
    /// </summary>
    public class BlockBTreeEntry : IBTPageEntry
    {

        /// <summary>
        /// cb(2 bytes): The count of bytes of the raw data contained in the block referenced by BREF
        /// excluding the block trailer and alignment padding, if any.
        /// </summary>
        public Int16 cb { get; set; }
        /// <summary>
        /// cRef(2 bytes): Reference count indicating the count of references to this block.
        /// </summary>
        public Int16 cRef { get; set; }

    }
}