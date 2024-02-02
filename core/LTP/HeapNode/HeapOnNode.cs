using core.NDB.Pages.BTree;
using System;
using System.Collections.Generic;

namespace core.LTP.HeapNode
{
    public class HeapOnNode
    {
        public List<HeapOnNodeDataBlock> HeapOnNodeDataBlocks { get; set; }
        public HeapOnNode(List<BlockBTreeEntry> blocks)
        {
            if (blocks == null || blocks.Count == 0)
                throw new Exception("Heap Node | Block Data null");
            this.HeapOnNodeDataBlocks = new List<HeapOnNodeDataBlock>();
            for (int i = 0; i < blocks.Count; i++)
                HeapOnNodeDataBlocks.Add(new HeapOnNodeDataBlock(i, blocks[i].Block.DataBlock));
        }
        public static byte[] GetHNHIDBytes(HeapOnNode heapOnNode, HID hidUserRoot)
        {
            HeapOnNodeDataBlock hnblock = heapOnNode.HeapOnNodeDataBlocks[(int)hidUserRoot.hidBlockIndex];
            return hnblock.GetAllocation(hidUserRoot);
        }
    }
}