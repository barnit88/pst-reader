using System;

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
        public UInt32 MyProperty { get; set; }
    }
}