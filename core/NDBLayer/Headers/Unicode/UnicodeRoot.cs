using core.NDBLayer.BREF;
using System;
using System.IO.MemoryMappedFiles;

namespace core.NDBLayer.Headers.Unicode
{
    /// <summary>
    /// The ROOT structure contains current file state.
    /// </summary>
    public class UnicodeRoot
    {
        /// <summary>
        /// A BREF structure that references 
        /// the root page of the Node BTree(NBT).
        /// Pointing to BTPage for Node
        /// </summary>
        public Bref NBTBREF { get; set; }
        /// <summary>
        /// A BREF structure that references 
        /// the root page of the Block BTree(NBT).
        /// Pointing to BTPage for Block
        /// </summary>
        public Bref BBTBREF { get; set; }
        /// <summary>
        /// dwReserved (4 bytes): Implementations SHOULD ignore this value and 
        /// SHOULD NOT modify it. 
        /// Creators of a new PST file MUST initialize this value to zero.
        /// </summary>
        int dwReserved;
        /// <summary>
        /// bFileEof (Unicode: 8 bytes; ANSI 4 bytes): The size of the PST file, in bytes. Length of End Byte from First 
        /// </summary>
        long ibFileEof;
        /// <summary>
        /// ibAMapLast (Unicode: 8 bytes; ANSI 4 bytes): An IB structure 
        /// that contains the absolute file offset to the last AMap page of the PST file
        /// </summary>
        long ibAMapLast;
        /// <summary>
        /// cbAMapFree (Unicode: 8 bytes; ANSI 4 bytes): 
        /// The total free space in all AMaps, combined.
        /// </summary>
        long cbAMapFree;
        /// <summary>
        /// cbPMapFree (Unicode: 8 bytes; ANSI 4 bytes): The total free space in all PMaps, combined. 
        /// Because the PMap is deprecated, this value SHOULD be zero.Creators of new PST files MUST
        /// initialize this value to zero.
        /// </summary>
        long cbPMapFree;//(Unicode: 8 bytes)
        /// <summary>
        /// BREFNBT (Unicode: 16 bytes; ANSI: 8 bytes): A BREF structure that references 
        /// the root page of the Node BTree(NBT).
        /// </summary>
        byte[] BREFNBT = new byte[16];//(Unicode: 16 bytes)
        /// <summary>
        /// BREFNBT (Unicode: 16 bytes; ANSI: 8 bytes): A BREF structure that references 
        /// the root page of the Block BTree(NBT).
        /// </summary>
        byte[] BREFBBT = new byte[16];
        /// <summary>
        /// fAMapValid (1 byte): Indicates whether all of the AMaps in this PST file are valid.
        /// This value MUST be set to one of the pre-defined values specified in the following table.
        /// 
        /// 
        /// Value   | Friendly name   | Meaning
        /// 0x00    |INVALID_AMAP     |  One or more AMaps in the PST are INVALID
        /// 0x01    |VALID_AMAP1      |  Deprecated.Implementations SHOULD NOT use this value.The AMaps are VALID
        /// 0x02    |VALID_AMAP2      |  The AMaps are VALID.
        /// </summary>
        byte fAMapValid;
        /// <summary>
        /// bReserved (1 byte): Implementations SHOULD ignore this value and SHOULD NOT modify it. 
        /// Creators of a new PST file MUST initialize this value to zero
        /// </summary>
        byte bReserved;
        /// <summary>
        /// wReserved (2 bytes): Implementations SHOULD ignore this value and SHOULD NOT modify it. 
        /// Creators of a new PST file MUST initialize this value to zero
        /// </summary>
        short wReserved;//(2 bytes) 
        public UnicodeRoot(MemoryMappedViewAccessor view, int offset)
        {
            int currentOffset = offset;
            dwReserved = view.ReadInt32(currentOffset);
            currentOffset += 4;
            ibFileEof = view.ReadInt64(currentOffset);
            currentOffset += 8;
            ibAMapLast = view.ReadInt64(currentOffset);
            currentOffset += 8;
            cbAMapFree = view.ReadInt64(currentOffset);
            currentOffset += 8;
            cbPMapFree = view.ReadInt64(currentOffset);
            currentOffset += 8;
            view.ReadArray(currentOffset, BREFNBT, 0, 16);
            NBTBREF = new Bref(BREFNBT);
            currentOffset += 16;
            view.ReadArray(currentOffset, BREFBBT, 0, 16);
            BBTBREF = new Bref(BREFBBT);
            currentOffset += 16;
            CheckfAMapValid(view, currentOffset);
            currentOffset += 1;
            bReserved = view.ReadByte(currentOffset);
            currentOffset += 1;
            wReserved = view.ReadInt16(currentOffset);
            currentOffset += 2;

            if (currentOffset != offset + 72)
                throw new Exception("Unknow. Byte Length mismatch");
        }
        /// <summary>
        /// Check fAMapValid
        /// </summary>
        /// <param name="view"></param>
        /// <param name="offset"></param>
        /// <exception cref="Exception"></exception>
        private void CheckfAMapValid(MemoryMappedViewAccessor view, int offset)
        {
            fAMapValid = view.ReadByte(offset);
            if (fAMapValid == (byte)fAMapValidType.INVALID_AMAP)
                throw new Exception("Invalid faMapVAlid");
            if (fAMapValid == (byte)fAMapValidType.VALID_AMAP1 || fAMapValid == (byte)fAMapValidType.VALID_AMAP2)
                return;
            throw new Exception("Invalid faMapVAlid.No match found");
        }
    }
}
public enum fAMapValidType : byte
{
    INVALID_AMAP = 0x00,// One or more AMaps in the PST are INVALID
    VALID_AMAP1 = 0x01,// Deprecated. Implementations SHOULD NOT use this value.The AMaps are VALID.<6>
    VALID_AMAP2 = 0x02 //The AMaps are VALID.
}
//0x00 INVALID_AMAP One or more AMaps in the PST are INVALID
//0x01 VALID_AMAP1 Deprecated. Implementations SHOULD NOT use this value.
//The AMaps are VALID.<6>
//0x02 VALID_AMAP2 The AMaps are VALID.
