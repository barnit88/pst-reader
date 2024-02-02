//using core.NDB.BREF;
//using core.NDB.ID;
//using System;

//namespace core.NDB.Headers.Unicode
//{
//    /// <summary>
//    /// The BREF is a record that maps a BID to its absolute file offset location.
//    /// Its length is 16 bytes
//    /// (Assuming its fullform as Block Reference)
//    /// </summary>
//    public class UnicodeBREF : Bref
//    {
//        public UnicodeBREF(byte[] brefData)
//        {
//            if (brefData.Length != 16)
//                throw new Exception("BREF Byte Length error");
//            bid = BitConverter.ToUInt64(brefData);
//            ib = BitConverter.ToUInt64(brefData, 8);
//            BId = new Bid(bid);
//            _bId = BId.BId;
//            Ib = ib;
//        }
//    }
//}