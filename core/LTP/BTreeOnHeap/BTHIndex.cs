using core.LTP.HeapNode;
using System;
using System.Collections.Generic;

namespace core.LTP.BTreeOnHeap
{
    /// <summary>
    /// Intermediate BTH (Index) Records
    /// 
    /// Index records do not contain actual data, but point to other index records or leaf records. The format 
    /// of the intermediate index record is as follows.The number of index records can be determined based
    /// on the size of the heap allocation.
    /// </summary>
    public class BTHIndex
    {
        public int Level { get; set; }
        public BTHData BTHData { get; set; } = null;
        public List<BTHIndex> BTHIndexes { get; set; } = null;
        public List<BTHIndexEntry> BTHIndexEntries { get; set; } = null;
        public HID HidRoot { get; set; } = null;
        /// <summary>
        /// key (variable): This is the key of the first record in the next level index record array. The size of the 
        /// key is specified in the cbKey field in the corresponding BTHHEADER structure(section 2.3.2.1). 
        /// The size and contents of the key are specific to the higher level structure that implements this 
        /// BTH.
        /// </summary>
        public byte[] key { get; set; }
        /// <summary>
        /// hidNextLevel (4 bytes): HID of the next level index record array. This contains the HID of the heap 
        /// item that contains the next level index record array.
        /// </summary>
        public UInt32 hidNextLevel { get; set; }
        public BTHIndex(BTH bTreeOnHeap, int levels)
        {
            this.Level = levels;//Levels
            this.HidRoot = bTreeOnHeap.BTHHeader.HidRoot;
            if (this.HidRoot.hidIndex == 0)
                return;
            this.BTHIndexEntries = new List<BTHIndexEntry>();
            if (Level == 0)
                this.BTHData = new BTHData(HidRoot, bTreeOnHeap);
            else
            {
                var tempBytes = HeapOnNode.GetHNHIDBytes(bTreeOnHeap.HeapOnNode, HidRoot);
                for (int i = 0; i < tempBytes.Length; i += (int)bTreeOnHeap.BTHHeader.CbKey + 4)
                    this.BTHIndexEntries.Add(new BTHIndexEntry(tempBytes, bTreeOnHeap.BTHHeader, i));
                this.BTHIndexes = new List<BTHIndex>();
                foreach (var entry in this.BTHIndexEntries)
                    this.BTHIndexes.Add(new BTHIndex(bTreeOnHeap, this.Level - 1));
            }
        }
    }
}