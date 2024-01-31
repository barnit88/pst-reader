using core.NDB.Pages.BTree;
using System;
using System.Collections.Generic;

namespace core.LTP.HeapNode
{
    public class HeapOnNode
    {
        public List<HeapOnNodeDataBlock> HeapsOnNode { get; set; }
        public HeapOnNode(List<BlockBTreeEntry> blocks)
        {
            this.HeapsOnNode = new List<HeapOnNodeDataBlock>();
            if (blocks == null || blocks.Count == 0)
                throw new Exception("Heap Node | Block Data null");
            //this.HeapsOnNode.Add()
        }
    }
}
