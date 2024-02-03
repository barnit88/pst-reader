using core.NDBLayer.Blocks;
using System.Collections.Generic;

namespace core.NDBLayer
{
    /// <summary>
    /// This DTO reference data outputted from a single node 
    /// in Node BTree of NDB Layer.
    /// </summary>
    public class NodeDataDTO
    {
        /// <summary>
        /// Parent Node Id of this Node if there is any 
        /// </summary>
        public ulong ParentNodeID { get; set; }
        /// <summary>
        /// Current Nodes Node Id
        /// </summary>
        public ulong NodeID { get; set; }
        /// <summary>
        /// Block Id of the Block referenced by this Node
        /// </summary>
        public ulong NodeBlockId { get; set; }
        /// <summary>
        /// SubNode Block Id if there is any Subnode of this Node
        /// </summary>
        public ulong SubNodeBlockId { get; set; }
        /// <summary>
        /// Lsit of Data Block Referenced by this Node, which is retreived
        /// thorugh the NodeBlockId
        /// </summary>
        public List<DataBlock> NodeData { get; set; }
        /// <summary>
        /// SubNode will have a list of Nodes, if there are any subnode
        /// referenced by this node.
        /// </summary>
        public List<NodeDataDTO> SubNodeData { get; set; }
    }
    //public class SubNodeDataDTO
    //{
    //    public int SubNodeID { get; set; }
    //    public int SubNodesNodeBlockID { get; set; }
    //    public int SubNodesSubNodeBlockID { get; set; }
    //    public List<DataBlock> NodeData { get; set; }
    //    public List<SubNodeDataDTO> SubNodeData { get; set; }
    //}
}
