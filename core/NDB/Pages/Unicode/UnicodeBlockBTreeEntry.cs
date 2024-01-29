//using core.NDB.BREF;
//using core.NDB.Headers.Unicode;
//using core.NDB.Pages.BTree;
//using System;

//namespace core.NDB.Pages.Unicode
//{
//    public class UnicodeBlockBTreeEntry : BlockBTreeEntry
//    {
//        /// <summary>
//        /// BREF(Unicode: 16 bytes; ANSI: 8 bytes): BREF structure that contains the BID
//        /// and IB of the block that the BBTENTRY references.
//        /// </summary>
//        public Bref Bref { get; set; }
//        /// <summary>
//        /// dwPadding(Unicode file format only, 4 bytes): Padding; MUST be set to zero.
//        /// </summary>
//        public int dwPadding { get; set; }
//        public UnicodeBlockBTreeEntry(byte[] blockBTreeDataBytes)
//        {
//            byte[] brefDataBytes = new byte[16];
//            Array.Copy(blockBTreeDataBytes, 0, brefDataBytes, 0, 16);
//            Bref = new UnicodeBREF(brefDataBytes);
//            //cb = BitConverter.ToInt16(blockBTreeDataBytes, 16);
//            //cRef = BitConverter.ToInt16(blockBTreeDataBytes, 18);
//            dwPadding = 0;
//        }
//    }
//}