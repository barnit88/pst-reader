using core.NDB.BREF;
using core.NDB.ID;

namespace core.NDB.Pages.BTree
{
    /// <summary>
    /// BTENTRY records contain a key value (NID or BID) and 
    /// a reference to a child BTPAGE page in the BTree.
    /// </summary>
    public class BTEntry : IBTPageEntry
    {
        /// <summary>
        /// If BTEntry is of NodeBTree then it will have a NID
        /// </summary>
        public Nid Nid { get; set; } = null;
        /// <summary>
        /// If BTEntry is of BlockBTree then it will have a BID
        /// </summary>
        public Bid Bid { get; set; } = null;
        /// <summary>
        /// Bool value to specify if Id associated with this 
        /// BTEntry is  of Nid type or Bid type
        /// </summary>
        public bool HasNid { get; set; } = false;
        /// <summary>
        /// Bool value to specify if Id associated with this 
        /// BTEntry is  of Nid type or Bid type
        /// </summary>
        public bool HasBid { get; set; } = false;

        ///// <summary>
        ///// Both NodeBTree and BlockBTree has a BTEntry,which specifies the 
        ///// current node type. 
        ///// If the Current Node is of BTreeEntry then the node is intermediate or 
        ///// root node of NodeBTree or BlockBTree
        ///// </summary>
        //public BTreeType BTreeType { get; set; }
        /// <summary>
        /// BREF(Unicode: 16 bytes; ANSI: 8 bytes): 
        /// BREF structure that points to the child BTPAGE.
        /// </summary>
        public Bref Bref { get; set; }
        public BTEntry(BTreeType bTreeType)
        {
            //this.BTreeType = bTreeType;
        }


    }
}
