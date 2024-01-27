using core.NDB.Headers.Unicode;
using core.NDB.ID;
using core.NDB.Pages.BTree;
using System;

namespace core.NDB.Pages.Unicode
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
        public ulong btKey { get; set; }
        public UnicodeBTreeEntry(byte[] btentryBytes, BTreeType bTreeType) : base(bTreeType)
        {
            btKey = BitConverter.ToUInt64(btentryBytes, 0);
            byte[] brefData = new byte[16];
            Array.Copy(btentryBytes, 8, brefData, 0, 16);
            Bref = new UnicodeBREF(brefData);
            if (BTreeType.NBT == bTreeType)
            {
                Nid = new Nid(btKey);
                HasNid = true;
            }
            if (BTreeType.BBT == bTreeType)
            {
                Bid = new Bid(btKey);
                HasBid = true;
            }
        }

    }
}
