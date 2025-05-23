﻿using core.NDBLayer.ID;
using System;

namespace core.NDBLayer.Pages.BTree
{
    /// <summary>
    /// NBTENTRY records contain information about nodes and are found in BTPAGES 
    /// with cLevel equal to 0, with the ptype of ptypeNBT.
    /// These are the leaf entries of the NBT.
    /// </summary>
    public class NodeBTreeEntry : IBTPageEntry
    {
        public BTreePageEntriesType BTreePageEntriesType { get; set; } = BTreePageEntriesType.NBTENTRY;
        public Nid Nid { get; set; }
        public Bid Bid { get; set; }
        public Bid BidSubNode { get; set; }
        public SpecialInternalNID SpecialInternalNID { get; set; }
        public NidType NidType { get; set; }
        #region Flags
        /// <summary>
        /// nid (Unicode: 8 bytes; ANSI: 4 bytes): The NID (section 2.2.2.1) of the entry. Note that the NID is a 4-byte value for both Unicode and ANSI formats.However, to stay consistent with the size of
        /// the btkey member in BTENTRY, the 4-byte NID is extended to its 8-byte equivalent for Unicode
        /// PST files.
        /// </summary>
        public ulong nid;
        /// <summary>
        /// bidData (Unicode: 8 bytes; ANSI: 4 bytes): The BID of the data block for this node
        /// </summary>
        public ulong bidData;
        /// <summary>
        /// bidSub (Unicode: 8 bytes; ANSI: 4 bytes): The BID of the subnode block for this node. If this 
        /// value is zero, a subnode block does not exist for this node.
        /// </summary>
        public ulong bidSub;
        /// <summary>
        /// nidParent (4 bytes): If this node represents a child of a Folder object defined 
        /// in the Messaging Layer, then this value is nonzero and contains 
        /// the NID of the parent Folder object's node. Otherwise, this value is zero.
        /// This field is not interpreted by any structure defined at the NDB Layer.
        /// 
        /// 
        /// PARENT NID :-
        /// A specific challenge exists when a simple node database is used to represent hierarchical concepts 
        /// such as a tree of Folder objects where top-level nodes are disjoint items that do not contain
        /// hierarchical semantics.While subnodes have a hierarchical structure, the fact that internal subnodes
        /// are not addressable outside of the NDB Layer makes them unsuitable for this purpose.
        /// The concept of a parent NID(nidParent) is introduced to address this challenge, providing a simple
        /// and efficient way for each Folder object node to point back to its parent Folder object node in the
        /// hierarchy. This link enables traversing up the Folder object tree to find its parent Folder objects, which
        /// is necessary and common for many Folder object-related operations, without having to read the raw
        /// data associated with each node.
        /// The parent NID concept described here is separate from the node/subnode relationship. The parent
        /// NID, as described here has no meaning to the NDB layer and is merely maintained as an optimization 
        /// for the Messaging layer.
        /// 
        /// </summary>
        public uint nidParent;
        /// <summary>
        /// dwPadding (Unicode file format only, 4 bytes): Padding; MUST be set to zero.
        /// </summary>
        public uint dwPadding;
        #endregion
        public NodeBTreeEntry(byte[] nodeBTreeDataBytes)
        {
            nid = BitConverter.ToUInt64(nodeBTreeDataBytes, 0);
            SpecialInternalNID = (SpecialInternalNID)nid;
            NidType = (NidType)(nid & 0x1f);//Lowest five bits
            bidData = BitConverter.ToUInt64(nodeBTreeDataBytes, 8);
            bidSub = BitConverter.ToUInt64(nodeBTreeDataBytes, 16);
            nidParent = BitConverter.ToUInt32(nodeBTreeDataBytes, 24);
            dwPadding = 0;
            Nid = new Nid(nid);
            Bid = new Bid(bidData);
            BidSubNode = new Bid(bidSub);
            //Console.WriteLine("------------------------------------------------------");
            //Console.WriteLine("Node BTree Entry Information");
            //Console.WriteLine("NID Value is : " + nid.ToString());
            //Console.WriteLine("Parent Nid Value is : " + NidType.ToString());
            //Console.WriteLine("NID Type is: " + NidType.ToString());
            //Console.WriteLine("Special Internal NID is: " + SpecialInternalNID.ToString());
            //Console.WriteLine("Bid Data is: " + bidData.ToString());
            //Console.WriteLine("Sub Bid is: " + bidSub.ToString());
            //Console.WriteLine("------------------------------------------------------");
        }
    }
}