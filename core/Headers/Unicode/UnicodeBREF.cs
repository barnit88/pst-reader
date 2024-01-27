using Core.PST.BREF;
using Core.PST.ID;
using System;

namespace Core.PST.Headers.Unicode
{
    /// <summary>
    /// The BREF is a record that maps a BID to its absolute file offset location.
    /// Its length is 16 bytes
    /// (Assuming its fullform as Block Reference)
    /// </summary>
    public class UnicodeBREF : Bref
    {
        public UnicodeBREF(byte[] brefData)
        {
            if (brefData.Length != 16)
                throw new Exception("BREF Byte Length error");
            this.bid = BitConverter.ToUInt64(brefData);
            this.ib = BitConverter.ToUInt64(brefData, 8);
            this.BId = new Bid(this.bid);
            this._bId = this.BId.BId;
            this.Ib = this.ib;
        }
    }
}