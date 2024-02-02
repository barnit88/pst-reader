using System;
using System.Collections.Generic;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace core.LTP.HeapNode
{
    /// <summary>
    /// HNPAGEHDR
    /// This is the header record used in subsequent data blocks of the HN that do not require a new Fill Level
    /// Map(see next section). This is only used when multiple data blocks are present.
    /// </summary>
    public class HNPAGEHDR
    {
        /// <summary>
        /// ibHnpm (2 bytes): The bytes offset to the HNPAGEMAP record (section 2.3.1.5), with respect to 
        /// the beginning of the HNPAGEHDR structure.
        /// </summary>
        public ushort ibHnpm { get; set; }
        public HNPAGEHDR(byte[] dataBytes)
        {
            this.ibHnpm = BitConverter.ToUInt16(dataBytes, 0);
        }
    }
}
