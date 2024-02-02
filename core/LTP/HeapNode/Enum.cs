using System.ComponentModel;

namespace core.LTP.HeapNode
{
    /// <summary>
    /// bClientSig (1 byte): Client signature. This value describes the higher-level structure that is 
    /// implemented on top of the HN.This value is intended as a hint for a higher-level structure and has
    /// no meaning for structures defined at the HN level. The following values are pre-defined for 
    /// bClientSig.All other values not described in the following table are reserved and MUST NOT be
    /// assigned or used.
    /// 
    ///         Value |  Friendly name    |   Meaning
    ///         0x6C  |  bTypeReserved1   |   Reserved
    ///         0x7C  |  bTypeTC          |   Table Context(TC/HN)
    ///         0x8C  |  bTypeReserved2   |   Reserved
    ///         0x9C  |  bTypeReserved3   |   Reserved
    ///         0xA5  |  bTypeReserved4   |   Reserved
    ///         0xAC  |  bTypeReserved5   |   Reserved
    ///         0xB5  |  bTypeBTH         |   BTree-on-Heap(BTH)
    ///         0xBC  |  bTypePC          |   Property Context(PC/BTH)
    ///         0xCC  |  bTypeReserved6   |   Reserved
    /// 
    /// </summary>
    public enum HNClientSig
    {
        [Description("Reserved")]
        bTypeReserved1 = 0x6C,
        [Description("Table Context(TC/HN)")]
        bTypeTC = 0x7C,
        [Description("Reserved")]
        bTypeReserved2 = 0x8C,
        [Description("Reserved")]
        bTypeReserved3 = 0x9C,
        [Description("Reserved")]
        bTypeReserved4 = 0xA5,
        [Description("Reserved")]
        bTypeReserved5 = 0xAC,
        [Description("BTree-on-Heap(BTH)")]
        bTypeBTH = 0xB5,
        [Description("Property Context(PC/BTH)")]
        bTypePC = 0xBC,
        [Description("Reserved")]
        bTypeReserved6 = 0xCC
    }
}