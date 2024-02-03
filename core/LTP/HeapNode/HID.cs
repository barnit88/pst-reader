using System;

namespace core.LTP.HeapNode
{
    /// <summary>
    /// An HID is a 4-byte value that identifies an item allocated from the heap. The value is unique only 
    /// within the heap itself.The following is the structure of an HID.
    /// </summary>
    public class HID
    {
        public UInt32 hid { get; set; }
        /// <summary>
        /// hidType (5 bits): HID Type; MUST be set to 0 (NID_TYPE_HID) to indicate a valid HID.
        /// </summary>
        public ulong hidType { get; set; }
        /// <summary>
        /// hidIndex (11 bits): HID index. This is the 1-based index value that identifies an item allocated from 
        /// the heap node. This value MUST NOT be zero.
        /// </summary>
        public ulong hidIndex { get; set; }
        /// <summary>
        /// hidBlockIndex (16 bits): This is the zero-based data block index. 
        /// This number indicates the zero based index of the data block in which this heap item resides.
        /// </summary>
        public ushort hidBlockIndex { get; set; }
        public HID(byte[] dataBytes)
        {
            int offset = 0;
            var temp = BitConverter.ToUInt32(dataBytes, offset);
            this.hid = temp;
            this.hidType = temp & 0x1F;
            this.hidIndex = (temp >> 5) & 0x7FF;
            this.hidBlockIndex = BitConverter.ToUInt16(dataBytes, offset + 2);
        }
    }
}
