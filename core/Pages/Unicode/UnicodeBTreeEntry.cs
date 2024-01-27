using Core.PST.Headers.Unicode;
using Core.PST.ID;
using Core.PST.Pages.BTree;
using System;

namespace Core.PST.Pages.Unicode
{
    public class UnicodeBTreeEntry : BTEntry
    {
        /// <summary>
        /// btkey (Unicode: 8 bytes; ANSI: 4 bytes): The key value associated with this BTENTRY. 
        /// All the entries in the child BTPAGE referenced by BREF have key values 
        /// greater than or equal to this key value.
        /// The btkey is either an NID (zero extended to 8 bytes for Unicode PSTs) or a BID, 
        /// depending on the ptype of the page.
        /// </summary>
        public UInt64 btKey { get; set; }
        public UnicodeBTreeEntry(byte[] btentryBytes, BTreeType bTreeType) : base(bTreeType)
        {
            this.btKey = BitConverter.ToUInt64(btentryBytes, 0);
            byte[] brefData = new byte[16];
            Array.Copy(btentryBytes, 8, brefData, 0, 16);
            this.Bref = new UnicodeBREF(brefData);
            if (BTreeType.NBT == bTreeType)
            {
                this.Nid = new Nid(this.btKey);
                this.HasNid = true;
            }
            if (BTreeType.BBT == bTreeType)
            {
                this.Bid = new Bid(this.btKey);
                this.HasBid = true;
            }
        }

    }
}
