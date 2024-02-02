using core.LTP.HeapNode;
using System;

namespace core.LTP.BTreeOnHeap
{
    public class BTHIndexEntry
    {
        /// <summary>
        /// key (variable): This is the key of the first record in the next level index record array. The size of the 
        /// key is specified in the cbKey field in the corresponding BTHHEADER structure(section 2.3.2.1). 
        /// The size and contents of the key are specific to the higher level structure that implements this 
        /// BTH.
        /// </summary>
        public byte[] Key { get; set; }
        /// <summary>
        /// hidNextLevel (4 bytes): HID of the next level index record array. This contains the HID of the heap 
        /// item that contains the next level index record array.
        /// </summary>
        public HID HidNextLevel { get; set; }
        /// <summary>
        /// key (variable): This is the key of the first record in the next level index record array. The size of the 
        /// key is specified in the cbKey field in the corresponding BTHHEADER structure(section 2.3.2.1). 
        /// The size and contents of the key are specific to the higher level structure that implements this 
        /// BTH.
        /// </summary>
        private byte[] key { get; set; }
        /// <summary>
        /// hidNextLevel (4 bytes): HID of the next level index record array. This contains the HID of the heap 
        /// item that contains the next level index record array.
        /// </summary>
        private UInt32 hidNextLevel { get; set; }
        public BTHIndexEntry(byte[] data, BTHHeader header, int offset)
        {
            int keySize = (int)header.CbKey;
            Array.Copy(data, offset, this.Key, 0, header.CbKey);
            this.key = this.Key;
            int hidOffset = offset + keySize;
            byte[] tempHidData = new byte[4];
            this.hidNextLevel = BitConverter.ToUInt32(data, hidOffset);
            Array.Copy(data, hidOffset, tempHidData, 0, 4);
            this.HidNextLevel = new HID(tempHidData);
        }
    }
}