using core.NDB.BREF;
using core.NDB.Headers.Unicode;
using System;
using System.IO.MemoryMappedFiles;

namespace core.NDB.Pages.BTree
{
    /// <summary>
    /// BTENTRY records contain a key value (NID or BID) and 
    /// a reference to a child BTPAGE page in the BTree.
    /// </summary>
    public class BTEntry : IBTPageEntry
    {
        public BTreePageEntriesType BTreePageEntriesType { get; set; } = BTreePageEntriesType.BTENTRY;
        /// <summary>
        /// A BTree entry record is an intermediate node that contains a child BTPage
        /// </summary>
        public BTreePage bTreePage { get; set; }

        /// <summary>
        /// 
        ///     BTree Type | cLevel           |   rgentries structure   |  cbEnt(bytes)
        ///     NBT        | 0                |   NBTENTRY              |  ANSI: 16, Unicode: 32 
        ///                | Greater than 0   |   BTENTRY               |  ANSI: 12, Unicode: 24
        ///     BBT        | 0                |   BBTENTRY              |  ANSI: 12, Unicode: 24
        ///                | Less than 0      |   BTENTRY               |  ANSI: 12, Unicode: 24
        /// 
        /// </summary>
        public BTreeEntryType BTreeEntryType { get; set; }
        /// <summary>
        /// BREF(Unicode: 16 bytes; ANSI: 8 bytes): 
        /// BREF structure that points to the child BTPAGE.
        /// </summary>
        public Bref Bref { get; set; }

        #region Flags
        /// <summary>
        /// btkey (Unicode: 8 bytes; ANSI: 4 bytes): The key value associated with this BTENTRY. All the 
        /// entries in the child BTPAGE referenced by BREF have key values greater than or equal to this key
        /// value.The btkey is either an NID (zero extended to 8 bytes for Unicode PSTs) or a BID, 
        /// depending on the ptype of the page.
        /// </summary>
        public UInt64 btkey;
        /// <summary>
        /// BREF (Unicode: 16 bytes; ANSI: 8 bytes): BREF structure (section 2.2.2.4) that points to the child 
        /// BTPAGE.
        /// </summary>
        public byte[] BREF = new byte[16];
        #endregion 

        public BTEntry(MemoryMappedFile mmf,byte[] btentryBytes, BTreeEntryType bTreeEntryType)
        {
            Console.WriteLine("+_+_+_+_+_+_+_+_+_+_+_+_+BTree Entry+_+_+_+_+_+_+_+_+_+_+_+_+_+");
            this.BTreeEntryType = bTreeEntryType;
            this.btkey = BitConverter.ToUInt64(btentryBytes, 0);
            byte[] brefData = new byte[16];
            Array.Copy(btentryBytes, 8, brefData, 0, 16);
            this.Bref = new Bref(brefData);
            if(bTreeEntryType == BTreeEntryType.NBTreeEntry)
                this.bTreePage = new BTreePage(mmf, Bref,BTreeType.NBT);
            else if (bTreeEntryType == BTreeEntryType.BBTreeEntry)
                this.bTreePage = new BTreePage(mmf, Bref, BTreeType.BBT);
            Console.WriteLine("+_+_+_+_+_+_+_+_+_+_+_+_+BTree Entry+_+_+_+_+_+_+_+_+_+_+_+_+_+");
        }
    }
}
