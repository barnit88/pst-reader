using System;

namespace core.NDB.Pages.BTree
{
    /// <summary>
    /// NBTENTRY records contain information about nodes and are found in BTPAGES 
    /// with cLevel equal to 0, with the ptype of ptypeNBT.
    /// These are the leaf entries of the NBT.
    /// </summary>
    public class NodeBTreeEntry : IBTPageEntry
    {
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
        protected int nidParent { get; set; }

    }
}