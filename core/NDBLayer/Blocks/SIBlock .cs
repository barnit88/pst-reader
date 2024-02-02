using System;
using System.Collections.Generic;

namespace core.NDBLayer.Blocks
{
    /// <summary>
    /// 
    /// Subnode BTree
    /// 
    /// The subnode BTree collectively refers to all the elements that make up a subnode.The subnode BTree 
    /// is a BTree that is made up of SIBLOCK and SLBLOCK structures, which contain SIENTRY and SLENTRY
    /// structures, respectively.These structures are defined in the following sections.
    /// 
    /// 
    /// SIBLOCKs
    /// An SIBLOCK is a block that contains an array of SIENTRYs.It is used to extend the number of
    /// subnodes that a node can reference by chaining SLBLOCKS.
    /// </summary>
    public class SIBlock
    {
        public List<SIEntry> SIEntries { get; set; } = new List<SIEntry>();
        /// <summary>
        /// btype (1 byte): Block type; MUST be set to 0x02.
        /// </summary>
        public byte btype { get; set; }
        /// <summary>
        /// cLevel (1 byte): MUST be set to 0x01.
        /// </summary>
        public byte cLevel { get; set; }
        /// <summary>
        /// cEnt (2 bytes): The number of SIENTRYs in the SIBLOCK.
        /// </summary>
        public ushort cEnt { get; set; }
        /// <summary>
        /// dwPadding (4 bytes, Unicode only): Padding; MUST be set to zero.
        /// </summary>
        public uint dwPadding { get; set; }
        /// <summary>
        /// rgentries (variable size): Array of SIENTRY structures. The size is equal to the number of entries 
        /// indicated by cEnt multiplied by the size of an SIENTRY (16 bytes for Unicode PST files, 8 bytes for 
        /// ANSI PST Files).
        /// </summary>
        public byte[] rgentries { get; set; }
        /// <summary>
        /// rgbPadding (optional, variable): This field is present if the total size of all of the other fields is not 
        /// a multiple of 64. The size of this field is the smallest number of bytes required to make the size of
        /// the SIBLOCK a multiple of 64. Implementations MUST ignore this field.
        /// </summary>
        public byte[] rgbPadding { get; set; }
        /// <summary>
        /// blockTrailer (ANSI: 12 bytes; Unicode: 16 bytes): A BLOCKTRAILER structure (section 
        /// </summary>
        public BlockTrailer blockTrailer { get; set; }
        public SIBlock(byte[] siBlockBytes, byte[] siBlockTrailerDataBytes)
        {
            btype = siBlockBytes[0];
            cLevel = siBlockBytes[1];
            if (!(btype == 0x02 && cLevel == 0x01))
                throw new Exception("SIBlock, btype and clevel match error");
            cEnt = BitConverter.ToUInt16(siBlockBytes, 2);
            dwPadding = 0;
            var rgEntriesSize = cEnt * 16;
            rgentries = new byte[rgEntriesSize];
            Array.Copy(siBlockBytes, 8, rgentries, 0, rgEntriesSize);
            for (int i = 1; i <= cEnt; i++)
            {
                byte[] temp = new byte[16];
                int position = (i - 1) * 16;
                Array.Copy(rgentries, 0, temp, 0, 16);
                SIEntries.Add(new SIEntry(temp));
            }
            blockTrailer = new BlockTrailer(siBlockTrailerDataBytes);
        }

    }
}
