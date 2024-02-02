using core.LTP.HeapNode;
using System.Collections.Generic;

namespace core.LTP.BTreeOnHeap
{
    /// <summary>
    /// Leaf BTH (Data) Records
    /// 
    /// Leaf BTH records contain the actual data associated with each key entry.The BTH records are tightly
    /// packed (that is, byte-aligned), and each record is exactly cbKey + cbEnt bytes in size.The number of
    /// data records can be determined based on the size of the heap allocation.
    /// 
    /// </summary>
    public class BTHData
    {
        public List<BTHDataEntry> BTHDataEntries { get; set; }
        /// <summary>
        /// key (variable): This is the key of the record. The size of the key is specified in the cbKey field in the 
        /// corresponding BTHHEADER structure(section 2.3.2.1). The size and contents of the key are
        /// specific to the higher level structure that implements this BTH.
        /// </summary>
        public byte[] key { get; set; }
        /// <summary>
        /// data (variable): This contains the actual data associated with the key. The size of the data is 
        /// specified in the cbEnt field in the corresponding BTHHEADER structure.The size and contents of
        /// the data are specific to the higher level structure that implements this BTH.
        /// </summary>
        public byte[] data { get; set; }
        public BTHData(HID hid, BTH tree)
        {
            this.data = HeapOnNode.GetHNHIDBytes(tree.HeapOnNode, hid);
            this.BTHDataEntries = new List<BTHDataEntry>();
            var keyValueLengthInBytes = (int)(tree.BTHHeader.CbKey + tree.BTHHeader.CbEnt);
            for (int i = 0; i < this.data.Length; i += keyValueLengthInBytes)
                this.BTHDataEntries.Add(new BTHDataEntry(this.data, i, tree));
        }
    }
}