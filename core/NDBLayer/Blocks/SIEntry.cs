﻿using System;

namespace core.NDBLayer.Blocks
{
    /// <summary>
    /// SIENTRY (Intermediate Block Entry)
    /// SIENTRY are intermediate records that point to SLBLOCKs
    /// </summary>
    public class SIEntry
    {
        /// <summary>
        /// nid (Unicode: 8 bytes; ANSI: 4 bytes): The key NID value to the next-level child block. This NID is 
        /// only unique within the parent node.The NID is extended to 8 bytes in order for Unicode PST files
        /// to follow the general convention of 8-byte indices (see section 2.2.2.7.7.4 for details).
        /// </summary>
        public ulong nid { get; set; }
        /// <summary>
        /// bid (Unicode: 8 bytes; ANSI: 4 bytes): The BID of the SLBLOCK.
        /// </summary>
        public ulong bid { get; set; }
        public SIEntry(byte[] siEntryBytes)
        {
            if (siEntryBytes.Length != 16)
                throw new Exception("SIEntry, byte length error");
            nid = BitConverter.ToUInt64(siEntryBytes, 0);
            bid = BitConverter.ToUInt64(siEntryBytes, 8);
        }
    }
}
