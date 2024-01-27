using Core.PST.Headers.Unicode;
using Core.PST.ID;
using System;
using System.IO.MemoryMappedFiles;

namespace Core.PST.Headers
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
        public UnicodeBREF NBTBREF { get; set; }
        /// <summary>
        /// A BREF structure that references 
        /// the root page of the Block BTree(NBT).
        /// Pointing to BTPage for Block
        /// </summary>
        public UnicodeBREF BBTBREF { get; set; }
        /// <summary>
        /// dwReserved (4 bytes): Implementations SHOULD ignore this value and 
        /// SHOULD NOT modify it. 
        /// Creators of a new PST file MUST initialize this value to zero.
        /// </summary>
        Int32 dwReserved;
        /// <summary>
        /// bFileEof (Unicode: 8 bytes; ANSI 4 bytes): The size of the PST file, in bytes. Length of End Byte from First 
        /// </summary>
        Int64 ibFileEof;
        /// <summary>
        /// ibAMapLast (Unicode: 8 bytes; ANSI 4 bytes): An IB structure 
        /// that contains the absolute file offset to the last AMap page of the PST file
        /// </summary>
        Int64 ibAMapLast;
        /// <summary>
        /// cbAMapFree (Unicode: 8 bytes; ANSI 4 bytes): 
        /// The total free space in all AMaps, combined.
        /// </summary>
        Int64 cbAMapFree;
        /// <summary>
        /// cbPMapFree (Unicode: 8 bytes; ANSI 4 bytes): The total free space in all PMaps, combined. 
        /// Because the PMap is deprecated, this value SHOULD be zero.Creators of new PST files MUST
        /// initialize this value to zero.
        /// </summary>
        Int64 cbPMapFree;//(Unicode: 8 bytes)
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
        Int16 wReserved;//(2 bytes) 
        public UnicodeRoot(MemoryMappedViewAccessor view, int offset)
        {
            int currentOffset = offset;
            this.dwReserved = view.ReadInt32(currentOffset);
            currentOffset += 4;
            this.ibFileEof = view.ReadInt64(currentOffset);
            currentOffset += 8;
            this.ibAMapLast = view.ReadInt64(currentOffset);
            currentOffset += 8;
            this.cbAMapFree = view.ReadInt64(currentOffset);
            currentOffset += 8;
            this.cbPMapFree = view.ReadInt64(currentOffset);
            currentOffset += 8;
            view.ReadArray(currentOffset, this.BREFNBT, 0, 16);
            this.NBTBREF = new UnicodeBREF(this.BREFNBT);
            currentOffset += 16;
            view.ReadArray(currentOffset, this.BREFBBT, 0, 16);
            this.BBTBREF = new UnicodeBREF(this.BREFBBT);
            currentOffset += 16;
            CheckfAMapValid(view, currentOffset);
            currentOffset += 1;
            this.bReserved = view.ReadByte(currentOffset);
            currentOffset += 1;
            this.wReserved = view.ReadInt16(currentOffset);
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
            this.fAMapValid = view.ReadByte(offset);
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
