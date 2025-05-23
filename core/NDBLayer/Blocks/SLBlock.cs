﻿using System;
using System.Collections.Generic;

namespace core.NDBLayer.Blocks
{
    /// <summary>
    /// 
    /// Subnode BTree
    /// The subnode BTree collectively refers to all the elements that make up a subnode.The subnode BTree 
    /// is a BTree that is made up of SIBLOCK and SLBLOCK structures, which contain SIENTRY and SLENTRY    /// structures, respectively.These structures are defined in the following sections.
    /// 
    ///  SLBLOCKs
    ///  An SLBLOCK is a block that contains an array of SLENTRYs.It is used to reference the subnodes of a
    ///  node.
    /// </summary>
    public class SLBlock
    {
        public List<SLEntry> SLEntries { get; set; } = new List<SLEntry>();
        /// <summary>
        /// btype(1 byte) : Block type; MUST be set to 0x02.
        /// </summary>
        public byte btype { get; set; }
        /// <summary>
        /// cLevel(1 byte) : MUST be set to 0x00.
        /// </summary>
        public byte cLevel { get; set; }
        /// <summary>
        /// cEnt(2 bytes) : The number of SLENTRYs in the SLBLOCK.This value and the number of elements in
        /// the rgentries array MUST be non-zero.When this value transitions to zero, it is required for the
        /// block to be deleted.
        /// </summary>
        public ushort cEnt { get; set; }
        /// <summary>
        /// dwPadding (4 bytes, Unicode only): Padding; MUST be set to zero.
        /// </summary>
        public uint dwPadding { get; set; }
        /// <summary>
        /// rgentries(variable size) : Array of SLENTRY structures.The size is equal to the number of entries
        /// indicated by cEnt multiplied by the size of an SLENTRY (24 bytes for Unicode PST files, 12 bytes
        /// for ANSI PST Files).
        /// </summary>
        public byte[] rgentries { get; set; }
        /// <summary>
        /// rgbPadding(optional, variable) : This field is present if the total size of all of the other fields is not
        /// a multiple of 64. The size of this field is the smallest number of bytes required to make the size of
        /// the SLBLOCK a multiple of 64. Implementations MUST ignore this field.
        /// </summary>
        public byte[] rgbPadding { get; set; }
        /// <summary>
        /// blockTrailer (ANSI: 12 bytes; Unicode: 16 bytes): A BLOCKTRAILER structure 
        /// </summary>
        public BlockTrailer blockTrailer { get; set; }
        public SLBlock(byte[] slBlockBytes, byte[] slBlockTrailerDataBytes)
        {
            btype = slBlockBytes[0];
            cLevel = slBlockBytes[1];
            if (!(btype == 0x02 && cLevel == 0x00))
                throw new Exception("SLBlock, btype and clevel match error");
            cEnt = BitConverter.ToUInt16(slBlockBytes, 2);
            dwPadding = 0;
            var rgEntriesSize = cEnt * 24;
            rgentries = new byte[rgEntriesSize];
            Array.Copy(slBlockBytes, 8, rgentries, 0, rgEntriesSize);
            for (int i = 1; i <= cEnt; i++)
            {
                byte[] temp = new byte[24];
                int position = (i - 1) * 24;
                Array.Copy(rgentries, position, temp, 0, 24);
                SLEntries.Add(new SLEntry(temp));
            }
            blockTrailer = new BlockTrailer(slBlockTrailerDataBytes);
        }
    }
}
