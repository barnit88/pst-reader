using System;

namespace core.LTP.BTreeOnHeap
{
    public class BTHDataEntry
    {
        /// <summary>
        /// Entry Key
        /// </summary>
        public byte[] Key { get; set; }
        /// <summary>
        /// Entry Data
        /// </summary>
        public byte[] Data { get; set; }
        public BTHDataEntry(byte[] data, int offset, BTH tree)
        {
            int keyOffset = offset;
            int keySize = (int)tree.BTHHeader.CbKey;
            Array.Copy(data, keyOffset, this.Key, 0, keySize);
            int dataOffset = keyOffset + keySize;
            int dataSize = (int)(dataOffset + tree.BTHHeader.CbEnt);
            Array.Copy(data, dataOffset, this.Data, 0, dataSize);
        }
    }
}
