using System;

namespace core.LTP.BTH
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
        /// bType (1 byte): MUST be bTypeBTH.
        /// </summary>
        public byte bType { get; set; }
        /// <summary>
        /// cbKey (1 byte): Size of the BTree Key value, in bytes. This value MUST be set to 2, 4, 8, or 16.
        /// </summary>
        public byte cbKey { get; set; }
        /// <summary>
        /// cbEnt (1 byte): Size of the data value, in bytes. This MUST be greater than zero and less than or 
        /// equal to 32.
        /// </summary>
        public byte cbEnt { get; set; }
        /// <summary>
        /// bIdxLevels (1 byte): Index depth. This number indicates how many levels of intermediate indices 
        /// exist in the BTH.Note that this number is zero-based, meaning that a value of zero actually
        /// means that the BTH has one level of indices. If this value is greater than zero, then its value
        /// indicates how many intermediate index levels are present
        /// </summary>
        public byte bIdxLevels { get; set; }
        /// <summary>
        /// hidRoot (4 bytes): This is the HID that points to the BTH entries for this BTHHEADER. The data 
        /// consists of an array of BTH records.This value is set to zero if the BTH is empty.
        /// </summary>
        public UInt32 MyProperty { get; set; }
    }
}
