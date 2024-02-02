using System.ComponentModel;

namespace core.NDBLayer.Pages.BTree
{
    /// <summary>
    /// Defines which type of the Page is current Page(NBT or BBT)
    /// </summary>
    public enum BTreeType
    {
        [Description("Node BTree")]
        NBT,
        [Description("Block BTree")]
        BBT
    }

    /// <summary>
    /// Defines which type of the BTEntry is current BTEntry
    /// </summary>
    public enum BTreeEntryType
    {
        [Description("Node BTree")]
        NBTreeEntry,
        [Description("Block BTree")]
        BBTreeEntry
    }



    /// <summary>
    /// cbEnt (1 byte)
    /// The size of each BTree entry, in bytes. Note that in some cases, cbEnt can be 
    /// greater than the corresponding size of the corresponding rgentries structure because of
    /// alignment or other considerations.Implementations MUST use the size specified in cbEnt to
    /// advance to the next entry.
    /// </summary>
    public enum BTreePageEntriesType : byte
    {
        /// <summary>
        /// BTENTRY (Intermediate Entries)
        /// 
        /// BTENTRY records contain a key value (NID or BID) and 
        /// a reference to a child BTPAGE page in the BTree.
        /// ANSI: 16, Unicode: 32
        /// </summary>
        [Description("Node BTree Entry")]
        NBTENTRY,
        /// <summary>
        /// BBTENTRY (Leaf BBT Entry)
        /// 
        /// BBTENTRY records contain information about blocks and are found in 
        /// BTPAGES with cLevel equal to 0, with the ptype of "ptypeBBT". 
        /// These are the leaf entries of the BBT. These structures might not 
        /// be tightly packed and the cbEnt field of the BTPAGE SHOULD
        /// be used to iterate over the entries.
        /// ANSI: 12, Unicode: 24
        /// </summary>
        [Description("Block BTree Entry")]
        BBTENTRY,
        /// <summary>
        /// NBTENTRY (Leaf NBT Entry)
        /// 
        /// NBTENTRY records contain information about nodes and are found in 
        /// BTPAGES with cLevel equal to 0, with the ptype of ptypeNBT. 
        /// These are the leaf entries of the NBT
        /// ANSI: 12, Unicode: 24
        /// </summary>
        [Description("BTree Entry")]
        BTENTRY
    }
    /// <summary>
    /// cbEnt (1 byte)
    /// The size of each BTree entry, in bytes. Note that in some cases, cbEnt can be 
    /// greater than the corresponding size of the corresponding rgentries structure because of
    /// alignment or other considerations.Implementations MUST use the size specified in cbEnt to
    /// advance to the next entry.
    /// </summary>
    public enum UnicodergentriesType : byte
    {
        //ANSI: 16, Unicode: 32
        [Description("Node BTree Entry")]
        NBTENTRY = 32,
        //ANSI: 12, Unicode: 24
        [Description("Block BTree Entry")]
        BBTENTRY = 24,
        //ANSI: 12, Unicode: 24
        [Description("BTree Entry")]
        BTENTRY = 24
    }
    /// <summary>
    /// cbEnt (1 byte)
    /// The size of each BTree entry, in bytes. Note that in some cases, cbEnt can be 
    /// greater than the corresponding size of the corresponding rgentries structure because of
    /// alignment or other considerations.Implementations MUST use the size specified in cbEnt to
    /// advance to the next entry.
    /// </summary>
    public enum AnsirgentriesType : byte
    {
        //ANSI: 16, Unicode: 32
        [Description("Node BTree Entry")]
        NBTENTRY = 16,
        //ANSI: 12, Unicode: 24
        [Description("Block BTree Entry")]
        BBTENTRY = 12,
        //ANSI: 12, Unicode: 24
        [Description("BTree Entry")]
        BTENTRY = 12
    }
}
