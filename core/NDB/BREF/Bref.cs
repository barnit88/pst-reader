using core.NDB.ID;
using System;

namespace core.NDB.BREF
{
    public class Bref
    {
        /// <summary>
        /// Structured Bid
        /// </summary>
        public Bid BId { get; set; } = null;
        /// <summary>
        /// Block Id to locate the root of the block
        /// bid (Unicode: 64 bits): A BID structure.
        /// </summary>
        public ulong _bId { get; set; }
        /// <summary>
        /// Absolute offset(location) value from the begining of the file.
        /// ib (Unicode: 64 bits): An IB structure,
        /// </summary>
        public ulong Ib { get; set; }
        /// <summary>
        /// Every block allocated in the PST file is identified using the BID structure.
        /// There are two types of BIDS
        /// 1. BIDs used in the context of Pages use all of the bits of the structure 
        /// and are incremented by 1.
        /// 2. Block BIDs reserve the two least significant bits(first two bits) for flags. As a
        /// result these increment by 4 each time a new one is assigned.
        /// </summary>
        protected ulong bid { get; set; }
        /// <summary>
        /// The IB (Byte Index) is used to represent an absolute offset within the PST file with respect to the
        /// beginning of the file.The IB is a simple unsigned integer value and is 64 bits in Unicode versions and
        /// 32 bits in ANSI versions.
        /// </summary>
        protected ulong ib { get; set; }
    }
}