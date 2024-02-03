using System;
using System.Linq;

namespace core.LTP.HeapNode
{
    /// <summary>
    /// 
    /// HNHDR
    /// 
    /// The HNHDR record resides at the beginning of the first data block in the HN (an HN can span several blocks), 
    /// which contains root information about the HN.
    /// 
    /// 
    ///     bClientSig Table
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
    /// 
    ///     rgbFillLevel Table
    /// 
    ///         Value |  Friendly name       |    Meaning
    ///         0x0   |  FILL_LEVEL_EMPTY    |    At least 3584 bytes free / data block does not exist
    ///         0x1   |  FILL_LEVEL_1        |    2560-3584 bytes free
    ///         0x2   |  FILL_LEVEL_2        |    2048-2560 bytes free
    ///         0x3   |  FILL_LEVEL_3        |    1792-2048 bytes free
    ///         0x4   |  FILL_LEVEL_4        |    1536-1792 bytes free
    ///         0x5   |  FILL_LEVEL_5        |    1280-1536 bytes free
    ///         0x6   |  FILL_LEVEL_6        |    1024-1280 bytes free
    ///         0x7   |  FILL_LEVEL_7        |    768-1024 bytes free
    ///         0x8   |  FILL_LEVEL_8        |    512-768 bytes free
    ///         0x9   |  FILL_LEVEL_9        |    256-512 bytes free
    ///         0xA   |  FILL_LEVEL_10       |    128-256 bytes free
    ///         0xB   |  FILL_LEVEL_11       |    64-128 bytes free
    ///         0xC   |  FILL_LEVEL_12       |    32-64 bytes free
    ///         0xD   |  FILL_LEVEL_13       |    16-32 bytes free
    ///         0xE   |  FILL_LEVEL_14       |    8-16 bytes free
    ///         0xF   |  FILL_LEVEL_FULL     |    Data block has less than 8 bytes free
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// </summary>
    public class HNHDR
    {
        public HID RootHId { get; set; }
        public HNClientSig ClientSig { get; set; }

        /// <summary>
        /// ibHnpm (2 bytes): The byte offset to the HN page Map record (section 2.3.1.5), with respect to the 
        /// beginning of the HNHDR structure.
        /// </summary>
        public ushort ibHnpm { get; set; }
        /// <summary>
        /// bSig (1 byte): Block signature; MUST be set to 0xEC to indicate an HN.
        /// </summary>
        public byte bSig { get; set; }
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
        /// 
        /// 
        /// 
        /// </summary>
        public byte bClientSig { get; set; }
        /// <summary>
        /// hidUserRoot (4 bytes): HID that points to the User Root record. The User Root record contains data 
        /// that is specific to the higher level.
        /// </summary>
        public UInt32 hidUserRoot { get; set; }
        /// <summary>
        /// rgbFillLevel(4 bytes) : Per-block Fill Level Map.This array consists of eight 4-bit values that indicate
        /// the fill level for each of the first 8 data blocks (including this header block). If the HN has fewer
        /// than 8 data blocks, then the values corresponding to the non-existent data blocks MUST be set to
        /// zero.The following table explains the values indicated by each 4-bit value
        /// 
        /// 
        ///     Value |  Friendly name       |    Meaning
        ///     0x0   |  FILL_LEVEL_EMPTY    |    At least 3584 bytes free / data block does not exist
        ///     0x1   |  FILL_LEVEL_1        |    2560-3584 bytes free
        ///     0x2   |  FILL_LEVEL_2        |    2048-2560 bytes free
        ///     0x3   |  FILL_LEVEL_3        |    1792-2048 bytes free
        ///     0x4   |  FILL_LEVEL_4        |    1536-1792 bytes free
        ///     0x5   |  FILL_LEVEL_5        |    1280-1536 bytes free
        ///     0x6   |  FILL_LEVEL_6        |    1024-1280 bytes free
        ///     0x7   |  FILL_LEVEL_7        |    768-1024 bytes free
        ///     0x8   |  FILL_LEVEL_8        |    512-768 bytes free
        ///     0x9   |  FILL_LEVEL_9        |    256-512 bytes free
        ///     0xA   |  FILL_LEVEL_10       |    128-256 bytes free
        ///     0xB   |  FILL_LEVEL_11       |    64-128 bytes free
        ///     0xC   |  FILL_LEVEL_12       |    32-64 bytes free
        ///     0xD   |  FILL_LEVEL_13       |    16-32 bytes free
        ///     0xE   |  FILL_LEVEL_14       |    8-16 bytes free
        ///     0xF   |  FILL_LEVEL_FULL     |    Data block has less than 8 bytes free
        /// 
        /// 
        /// </summary>
        public ulong rgbFillLevel { get; set; }//storing raw value for now
        public HNHDR(byte[] dataBytes)
        {
            this.hidUserRoot = BitConverter.ToUInt16(dataBytes, 0);
            this.bSig = dataBytes[2];
            this.bClientSig = dataBytes[3];
            this.hidUserRoot = BitConverter.ToUInt32(dataBytes, 4);
            this.ClientSig = (HNClientSig)this.bClientSig;
            this.RootHId = new HID(dataBytes.Skip(4).Take(4).ToArray());
        }
    }
}
