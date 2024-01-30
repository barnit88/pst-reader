using System;

namespace core.NDB.ID
{
    /// <summary>
    /// Blocks are the fundamental units of data storage at the NDB layer. Blocks are assigned in sizes that
    /// are multiples of 64 bytes and are aligned on 64-byte boundaries.The maximum size of any block is 8 kilobytes (8192 bytes).
    /// Similar to pages, each block stores its metadata in a block trailer placed at the very end of the block
    /// so that the end of the trailer is aligned with the end of the block.
    /// Blocks generally fall into one of two categories: data blocks and subnode blocks.Data blocks are used
    /// to store raw data, where subnode blocks are used to represent nodes contained within a node.
    /// The storage capacity of each data block is the size of the data block(from 64 to 8192 bytes) minus
    /// the size of the trailer block.
    /// </summary>
    public class Bid
    {
        public ExternalOrInternalBid ExternalOrInternalBid { get; set; }
        /// <summary>
        /// Indentifies if the block is internal or external
        /// </summary>
        public bool IsInternal { get; set; }
        /// <summary>
        /// Actual BId value after making least significant bit 0;
        /// </summary>
        public ulong BId { get; set; }//64 bit unsigned integer 0 to +value
        /// <summary>
        /// A - r (1 bit)
        /// Reserved bit. Readers MUST ignore this bit and treat it as zero before looking up the
        /// BID from the BBT.Writers MUST set this bit to zero.
        /// </summary>
        public bool Ar { get; set; }
        /// <summary>
        /// B - i (1 bit)
        /// MUST set to 1 when the block is "Internal", or zero when the block is not "Internal". An
        /// internal block is an intermediate block that, instead of containing actual data, contains metadata
        /// about how to locate other data blocks that contain the desired information.
        /// </summary>
        public bool Bi { get; set; }
        /// <summary>
        /// bidIndex (Unicode: 62 bits; ANSI: 30 bits):
        /// A monotonically increasing value that uniquely
        /// identifies the BID within the PST file.bidIndex values are assigned based on the bidNextB value in
        /// the HEADER structure. The bidIndex increments by one each time a new BID
        /// is assigned.
        /// </summary>
        public ulong bidIndex { get; set; }
        public Bid(ulong bid)
        {
            
            this.BId = ResetLeastSignificantBit(bid);
            this.Ar = false;
            var bicalc = (this.BId & 0x02);//Taking 2nd most least significant bit
            this.IsInternal = bicalc > 0;
            this.bidIndex = (this.BId >> 2) & 0x3FFFFFFFFFFFFFFF;//Shifting and taking 62 bits
            ExternalOrInternalBid = this.IsInternal ? 
                    ExternalOrInternalBid.Internal : ExternalOrInternalBid.External;
            Console.WriteLine("****************************************");
            Console.WriteLine("Is Internal:- is 1 and External:- is 0 or not internal");
            Console.WriteLine("Is Internal: " + this.ExternalOrInternalBid + " or " + this.IsInternal.ToString());
            Console.WriteLine(this.BId);
            Console.WriteLine("****************************************");
        }

        /// <summary>
        /// A - r (1 bit): Reserved bit. Readers MUST ignore this bit and 
        /// treat it as zero before looking up the BID from the BBT.
        /// Writers MUST set this bit to zero.
        /// 0xffffffffffffffff in bits is f=1111, e= 1110
        /// Bitwise &(AND) opetaion is performed to make the Lease significant bit as 0
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private ulong ResetLeastSignificantBit(ulong value)
        {
            ulong sixtyFourBitWithZeroAsLeastSignificantBit = 0xfffffffffffffffe;
            var normalizedBID = value & sixtyFourBitWithZeroAsLeastSignificantBit;
            return normalizedBID;
        }
    }
}