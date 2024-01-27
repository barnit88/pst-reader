using System;
using System.ComponentModel;

namespace core.NDB.Pages.Base
{
    /// <summary>
    /// Denoted as pType. A Page Type defines what type of page it is.
    /// 
    /// Value | Friendlyname | Meaning | wSig Value
    /// 0x80 ptypeBBT Block BTree page. Block or page signature 
    /// 0x81 ptypeNBT Node BTree page.Block or page signature
    /// 0x82 ptypeFMap Free Map page. 0x0000 
    /// 0x83 ptypePMap Allocation Page Map page. 0x0000
    /// 0x84 ptypeAMap Allocation Map page. 0x0000
    /// 0x85 ptypeFPMap Free Page Map page. 0x0000
    /// 0x86 ptypeDL Density List page. Block or page si
    /// 
    /// </summary>
    public enum PageType : byte
    {
        [Description("Block BTree Page")]
        ptypeBBT = 0x80,
        [Description("Node BTree Page")]
        ptypeNBT = 0x81,
        [Description("Free Map Page")]
        ptypeFMap = 0x82,
        [Description("Allocation Page Map Page")]
        ptypePMap = 0x83,
        [Description("Allocation Map Page")]
        ptypeAMap = 0x84,
        [Description("Free Page Map Page")]
        ptypeFPMap = 0x85,
        [Description("Density List Page")]
        ptypeDL = 0x86
    }
    public enum wSig : short
    {
        ptypeFMap = 0x0000,
        ptypePMap = 0x0000,
        ptypeAMap = 0x0000,
        ptypeFPMap = 0x0000,
    }
}
