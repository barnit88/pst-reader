using Core.PST.BREF;
using Core.PST.Headers.Unicode;
using Core.PST.ID;
using Core.PST.Pages.BTree;
using System;

namespace Core.PST.Pages.Unicode
{
    public class UnicodeBlockBTreeEntry : BlockBTreeEntry
    {
        /// <summary>
        /// BREF(Unicode: 16 bytes; ANSI: 8 bytes): BREF structure that contains the BID
        /// and IB of the block that the BBTENTRY references.
        /// </summary>
        public Bref Bref { get; set; }
        /// <summary>
        /// dwPadding(Unicode file format only, 4 bytes): Padding; MUST be set to zero.
        /// </summary>
        public Int32 dwPadding { get; set; }
        public UnicodeBlockBTreeEntry(byte[] blockBTreeDataBytes)
        {
            byte[] brefDataBytes = new byte[16];
            Array.Copy(blockBTreeDataBytes, 0, brefDataBytes, 0, 16);
            this.Bref = new UnicodeBREF(brefDataBytes);
            this.cb = BitConverter.ToInt16(blockBTreeDataBytes, 16);
            this.cRef = BitConverter.ToInt16(blockBTreeDataBytes, 18);
            this.dwPadding = 0;
        }
    }
}