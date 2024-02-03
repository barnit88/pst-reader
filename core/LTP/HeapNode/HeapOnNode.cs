using core.NDBLayer;
using System;
using System.Collections.Generic;

namespace core.LTP.HeapNode
{
    public class HeapOnNode
    {
        public List<HeapOnNodeDataBlock> HeapOnNodeDataBlocks { get; set; }
        public NodeDataDTO Node { get; set; }
        public HeapOnNode(NodeDataDTO node)
        {
            if (node.NodeData == null || node.NodeData.Count == 0)
                throw new Exception("Heap Node | Block Data null");
            this.HeapOnNodeDataBlocks = new List<HeapOnNodeDataBlock>();
            for (int index = 0; index < node.NodeData.Count; index++)
                HeapOnNodeDataBlocks.Add(new HeapOnNodeDataBlock(index, node.NodeData[index]));
            this.Node = node;
        }
        public static byte[] GetHNHIDBytes(HeapOnNode heapOnNode, HID hidUserRoot)
        {
            HeapOnNodeDataBlock hnblock = heapOnNode.HeapOnNodeDataBlocks[(int)hidUserRoot.hidBlockIndex];
            return hnblock.GetAllocation(hidUserRoot);
        }
    }
}