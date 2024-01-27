using Core.PST.ID;
using Core.PST.Pages.BTree;
using System;

namespace Core.PST.Pages.Unicode
{
    public class UnicodeNodeBTreeEntry : NodeBTreeEntry
    {
        /// <summary>
        /// Node Id
        /// </summary>
        public Nid Nid { get; set; }
        /// <summary>
        /// Bid of Data Block of this node
        /// </summary>
        public Bid DataBlock { get; set; }
        /// <summary>
        /// Bid of SubNode Block of this node.
        /// </summary>
        public Bid SubNodeBlock { get; set; }
        /// <summary>
        /// nid(Unicode: 8 bytes; ANSI: 4 bytes): The NID of the entry.
        /// Note that the NID is a 4-byte value for both Unicode and ANSI formats.
        /// However, to stay consistent with the size of the btkey member in BTENTRY, 
        /// the 4-byte NID is extended to its 8-byte equivalent for Unicode PST files.
        /// </summary>
        protected UInt64 nid { get; set; }
        /// <summary>
        /// bidData(Unicode: 8 bytes; ANSI: 4 bytes): The BID of the data block for this node.
        /// </summary>
        protected UInt64 bidData { get; set; }
        /// <summary>
        /// bidSub(Unicode: 8 bytes; ANSI: 4 bytes): The BID of the subnode block for this node.
        /// If this value is zero, a subnode block does not exist for this node.
        /// </summary>
        protected UInt64 bidSub { get; set; }
        /// <summary>
        /// dwPadding (Unicode file format only, 4 bytes): Padding; MUST be set to zero.
        /// </summary>
        protected Int32 dwPadding { get; set; }
        public UnicodeNodeBTreeEntry(byte[] nodeBTreeDataBytes)
        {
            this.nid = BitConverter.ToUInt64(nodeBTreeDataBytes, 0);
            this.bidData = BitConverter.ToUInt64(nodeBTreeDataBytes, 8);
            this.bidSub = BitConverter.ToUInt64(nodeBTreeDataBytes, 16);
            this.nidParent = BitConverter.ToInt32(nodeBTreeDataBytes, 24);
            this.dwPadding = 0;
            this.Nid = new Nid(nid);
            this.DataBlock = new Bid(this.bidData);
            this.SubNodeBlock = new Bid(this.bidSub);
        }
    }
}
