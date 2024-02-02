using core.LTP.HeapNode;
using System;

namespace core.LTP.BTreeOnHeap
{
    /// <summary>
    /// BTHHEADER
    /// 
    /// The BTHHEADER contains the BTH metadata, which instructs the reader how to access the other
    /// objects of the BTH structure.
    /// </summary>
    public class BTHHeader
    {
        /// <summary>
        /// cbKey (1 byte): Size of the BTree Key value, in bytes. This value MUST be set to 2, 4, 8, or 16.
        /// </summary>
        public uint CbKey { get; set; }
        /// <summary>
        /// cbEnt (1 byte): Size of the data value, in bytes. This MUST be greater than zero and less than or 
        /// equal to 32.
        /// </summary>
        public uint CbEnt { get; set; }
        /// <summary>
        /// bIdxLevels (1 byte): Index depth. This number indicates how many levels of intermediate indices 
        /// exist in the BTH.Note that this number is zero-based, meaning that a value of zero actually
        /// means that the BTH has one level of indices. If this value is greater than zero, then its value
        /// indicates how many intermediate index levels are present
        /// </summary>
        public uint BIdxLevels { get; set; }
        /// <summary>
        /// hidRoot (4 bytes): This is the HID that points to the BTH entries for this BTHHEADER. The data 
        /// consists of an array of BTH records.This value is set to zero if the BTH is empty.
        /// </summary>
        public HID HidRoot { get; set; }
        /// <summary>
        /// bType (1 byte): MUST be bTypeBTH.
        /// </summary>
        private byte bType { get; set; }
        /// <summary>
        /// cbKey (1 byte): Size of the BTree Key value, in bytes. This value MUST be set to 2, 4, 8, or 16.
        /// </summary>
        private byte cbKey { get; set; }
        /// <summary>
        /// cbEnt (1 byte): Size of the data value, in bytes. This MUST be greater than zero and less than or 
        /// equal to 32.
        /// </summary>
        private byte cbEnt { get; set; }
        /// <summary>
        /// bIdxLevels (1 byte): Index depth. This number indicates how many levels of intermediate indices 
        /// exist in the BTH.Note that this number is zero-based, meaning that a value of zero actually
        /// means that the BTH has one level of indices. If this value is greater than zero, then its value
        /// indicates how many intermediate index levels are present
        /// </summary>
        private byte bIdxLevels { get; set; }
        /// <summary>
        /// hidRoot (4 bytes): This is the HID that points to the BTH entries for this BTHHEADER. The data 
        /// consists of an array of BTH records.This value is set to zero if the BTH is empty.
        /// </summary>
        private byte[] hidRoot { get; set; }
        public BTHHeader(byte[] dataBytes)
        {
            this.bType = dataBytes[0];
            this.cbKey = dataBytes[1];
            this.cbEnt = dataBytes[2];
            this.bIdxLevels = dataBytes[3];
            this.hidRoot = new byte[4];
            Array.Copy(dataBytes, 4, this.hidRoot, 0, 4);
            this.HidRoot = new HID(this.hidRoot);
            this.CbKey = (uint)this.cbKey;
            this.CbEnt = (uint)this.cbEnt;
            this.BIdxLevels = (uint)this.bIdxLevels;
        }
    }
}