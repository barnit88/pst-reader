using core.LTP.HeapNode;
using core.LTP.PropertyContext;
using System;
using System.Collections.Generic;

namespace core.LTP.BTreeOnHeap
{
    /// <summary>
    /// A BTree-on-Heap implements a classic BTree on a heap node. A BTH consists of several parts: 
    /// header, the BTree records, and optional BTree data.The following diagram shows a high-level
    /// schematic of a BTH.
    /// 
    /// 
    /// The preceding diagram shows a BTH with two levels of indices. The top-level index (Key, HID) value 
    /// pairs actually point to heap items that contain the Level 1 Indices, which, in turn, point to heap items
    /// that contain the leaf(Key, data) value pairs.Each of the six boxes in the diagram actually represents
    /// six separate items allocated out of the same HN, as indicated by their associated HIDs.
    /// 
    /// </summary>
    public class BTH
    {
        public BTHHeader BTHHeader { get; set; }
        public BTHIndex BTHRoot { get; set; }
        public BTHData BTHDataRecord { get; set; }
        public HeapOnNode HeapOnNode { get; set; }
        public Dictionary<byte[], BTHDataEntry> Properties;
        public BTH(HeapOnNode heapOnNode, HID userRoot = null)
        {
            this.HeapOnNode = heapOnNode;
            var bthHeaderHID = userRoot ?? heapOnNode.HeapOnNodeDataBlocks[0].HNHeader.HId;
            this.BTHHeader = new BTHHeader(HeapOnNode.GetHNHIDBytes(heapOnNode, bthHeaderHID));
            this.BTHRoot = new BTHIndex(this, (int)this.BTHHeader.BIdxLevels);
            this.Properties = new Dictionary<byte[], BTHDataEntry>(new ByteArrayComparer());
            var stack = new Stack<BTHIndex>();
            stack.Push(this.BTHRoot);
            while (stack.Count > 0)
            {
                var cur = stack.Pop();
                if (cur.BTHData != null)
                    foreach (var entry in cur.BTHData.BTHDataEntries)
                        this.Properties.Add(entry.Key, entry);
                if (cur.BTHIndexes != null)
                    foreach (var child in cur.BTHIndexes)
                        stack.Push(child);
            }
        }
        //copied
        public Dictionary<ushort, ExchangeProperty> GetExchangeProperties()
        {
            var ret = new Dictionary<ushort, ExchangeProperty>();
            var stack = new Stack<BTHIndex>();
            stack.Push(this.BTHRoot);
            while (stack.Count > 0)
            {
                var cur = stack.Pop();

                if (cur.BTHData != null)
                    foreach (var entry in cur.BTHData.BTHDataEntries)
                    {
                        var curKey = BitConverter.ToUInt16(entry.Key, 0);
                        int i = 0;
                        if (curKey == 0x02)
                            i++;
                        ret.Add(curKey, new ExchangeProperty(entry, this));
                    }
                if (cur.BTHIndexes != null)
                    foreach (var child in cur.BTHIndexes)
                        stack.Push(child);
            }
            return ret;
        }
    }
}