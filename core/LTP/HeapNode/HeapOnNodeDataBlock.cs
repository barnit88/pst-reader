using core.NDB.Blocks;
using System;

namespace core.LTP.HeapNode
{
    /// <summary>
    /// Heaps Data Block are of different type. 
    /// And they can be differentiated  from its Header and Footer.
    /// 
    /// HNPAGEMAP( HNPageMap) Identical to footer in Heap, it can be found in all 
    /// type of the Heap On Node.
    /// 
    /// HNHDR (HNHeader) This can be assumed as Header and is found in first data block 
    /// 
    /// HNPAGEHDR( HNPageHeader) This can also be assumed as Header and it is found on
    /// subsequent data block after block containing HNHDR and in certain condition where 
    /// HNBITMAPHDR is required.
    /// 
    /// HNBITMAPHDR(HNBITMAPHeader) This can also be assumed as Header and it is found on 
    /// certain blocks with certain index only HNBITMAPHDR appears at data block 8 
    /// (the first data block is data block 0) and thereafter every 128 
    /// blocks. (that is, data block 8, data block 136, data block 264, and so on).
    /// 
    /// </summary>
    public class HeapOnNodeDataBlock
    {
        public DataBlock DataBlock { get; set; }
        public UInt16 PageOffset { get; set; }

        /// <summary>
        /// This is the header record used in subsequent data blocks of the HN that do not require a 
        /// new Fill Level Map(see next section). This is only used when multiple data blocks are present.
        /// </summary>
        public HNHDR HNHeader { get; set; } = null;
        /// <summary>
        /// HNPAGEHDR 
        /// This is the header record used in subsequent data blocks of the HN that do not require a new Fill Level
        /// Map(see next section). This is only used when multiple data blocks are present.
        /// </summary>
        public HNPAGEHDR HNPageHeader { get; set; } = null;
        /// <summary>
        /// Beginning with the eighth data block, a new Fill Level Map is required. An HNBITMAPHDR fulfills this 
        /// requirement.The Fill Level Map in the HNBITMAPHDR can map 128 blocks.This means that an
        /// HNBITMAPHDR appears at data block 8 (the first data block is data block 0) and thereafter every 128 
        /// blocks. (that is, data block 8, data block 136, data block 264, and so on).
        /// </summary>
        public HNBITMAPHDR HNBITMAPHeader { get; set; } = null;
        /// <summary>
        /// The HNPAGEMAP is the last item in the variable length data portion of the block immediately 
        /// following the last heap item.There can be anywhere from 0 to 63 bytes of padding between the
        /// HNPAGEMAP and the block trailer.The beginning of the HNPAGEMAP is aligned on a 2-byte
        /// boundary so there can be an additional 1 byte of padding between the last heap item and the
        /// HNPAGEMAP.
        /// The HNPAGEMAP structure contains the information about the allocations in the page. The
        /// HNPAGEMAP is located using the ibHnpm field in the HNHDR, HNPAGEHDR and HNBITMAPHDR records
        /// </summary>
        public HNPAGEMAP HNPageMap { get; set; }

        public HeapOnNodeDataBlock(int blockIndex, DataBlock dataBlock)
        {
            this.DataBlock = dataBlock;
            var dataBytes = dataBlock.data;
            this.PageOffset = BitConverter.ToUInt16(dataBytes, 0);
            this.HNPageMap = new HNPAGEMAP(dataBytes, this.PageOffset);
            // First block contains a HNHDR
            if (blockIndex == 0)
                this.HNHeader = new HNHDR(dataBytes);
            // Blocks 8, 136, then every 128th contains a HNBITMAPHDR
            else if (blockIndex % 128 == 8)
                this.HNBITMAPHeader = new HNBITMAPHDR(dataBytes);
            // All other blocks contain a HNPAGEHDR
            else
                this.HNPageHeader = new HNPAGEHDR(dataBytes);
        }
        public byte[] GetAllocation(HID hidUserRoot)
        {
            var offsetBegining = this.HNPageMap.rgibAlloc[(int)hidUserRoot.hidIndex - 1];
            var offsetEnd = this.HNPageMap.rgibAlloc[(int)hidUserRoot.hidIndex];
            byte[] allocatiionData = new byte[offsetEnd - offsetBegining];
            Array.Copy(this.DataBlock.data, offsetBegining, allocatiionData, 0, offsetEnd - offsetBegining);
            return allocatiionData;
        }
    }
}

//            private class HNDataBlock
//{
//    public int Index;
//    public byte[] Buffer;
//    public UInt16[] rgibAlloc;

//    // In first block only
//    public EbType bClientSig;
//    public HID hidUserRoot;  // HID that points to the User Root record
//}

//foreach (var buf in ndb.ReadDataBlocks(dataBid))
//{
//    
//    if (index == 0)
//    {
//        var h = Map.MapType<HNHDR>(buf, 0);
//        var pm = Map.MapType<HNPAGEMAP>(buf, h.ibHnpm);
//        var b = new HNDataBlock
//        {
//            Index = index,
//            Buffer = buf,
//            bClientSig = h.bClientSig,
//            hidUserRoot = h.hidUserRoot,
//            rgibAlloc = Map.MapArray<UInt16>(buf, h.ibHnpm + Marshal.SizeOf(pm), pm.cAlloc + 1),  //+1 to get the dummy entry that gives us the size of the last one
//        };
//        blocks.Add(b);
//    }
//    // Blocks 8, 136, then every 128th contains a HNBITMAPHDR
//    else if (index == 8 || (index >= 136 && (index - 8) % 128 == 0))
//    {
//        var h = Map.MapType<HNBITMAPHDR>(buf, 0);
//        var pm = Map.MapType<HNPAGEMAP>(buf, h.ibHnpm);
//        var b = new HNDataBlock
//        {
//            Index = index,
//            Buffer = buf,
//            rgibAlloc = Map.MapArray<UInt16>(buf, h.ibHnpm + Marshal.SizeOf(pm), pm.cAlloc + 1),  //+1 to get the dummy entry that gives us the size of the last one
//        };
//        blocks.Add(b);
//    }
//    // All other blocks contain a HNPAGEHDR
//    else
//    {
//        var h = Map.MapType<HNPAGEHDR>(buf, 0);
//        var pm = Map.MapType<HNPAGEMAP>(buf, h.ibHnpm);
//        var b = new HNDataBlock
//        {
//            Index = index,
//            Buffer = buf,
//            rgibAlloc = Map.MapArray<UInt16>(buf, h.ibHnpm + Marshal.SizeOf(pm), pm.cAlloc + 1),  //+1 to get the dummy entry that gives us the size of the last one
//        };
//        blocks.Add(b);
//    }
//    index++;
//}
