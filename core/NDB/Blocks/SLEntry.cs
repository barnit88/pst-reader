using System;

namespace core.NDB.Blocks
{
    /// <summary>
    /// SLENTRY (Leaf Block Entry)
    /// SLENTRY are records that refer to internal subnodes of a node.
    /// </summary>
    public class SLEntry
    {
        /// <summary>
        /// nid(Unicode: 8 bytes; ANSI: 4 bytes): Local NID of the subnode.This NID is guaranteed to be
        /// unique only within the parent node.
        /// </summary>
        public UInt64 nid { get; set; }
        /// <summary>
        /// bidData(Unicode: 8 bytes; ANSI: 4 bytes): The BID of the data block associated with the
        /// subnode.
        /// </summary>
        public UInt64 bidData { get; set; }
        /// <summary>
        /// bidSub(Unicode: 8 bytes; ANSI: 4 bytes): If nonzero, the BID of the subnode of this subnode.
        /// </summary>
        public UInt64 bidSub { get; set; }
        public SLEntry(byte[] slEntryBytes)
        {
            if (slEntryBytes.Length != 24)
                throw new Exception("SlEntry, byte length error");
            this.nid = BitConverter.ToUInt64(slEntryBytes,0);
            this.bidData = BitConverter.ToUInt64(slEntryBytes, 8);
            this.bidSub = BitConverter.ToUInt64(slEntryBytes, 16);
        }
    }
}