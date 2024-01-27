using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Reflection;
using static System.Collections.Specialized.BitVector32;
using System.Security.Cryptography;

namespace Core.PST.Headers.Unicode
{
    public class UnicodeHeader
    {
        public bool IsDataEncoded { get; set; } = false;
        public bCryptMethodType EncodingFriendlyName { get; set; }
        public string EncodingAlgorithm { get; set; }
        public UnicodeRoot Root { get; set; }
        /// <summary>
        /// bidUnused (8 bytes Unicode only): Unused padding added when the 
        /// Unicode PST file format was created.
        /// </summary>
        Int64 bidUnused { get; set; }
        /// <summary>
        /// bidNextP (Unicode: 8 bytes; ANSI: 4 bytes): Next page BID. Pages have a special 
        /// counter allocating bidIndex values.
        /// The value of bidIndex for BIDs for pages is allocated from this counter.
        /// </summary>
        byte[] bidNextP { get; set; } = new byte[8];
        /// <summary>
        /// dwUnique (4 bytes): This is a monotonically-increasing value that is 
        /// modified every time the PST file's HEADER structure is modified.
        /// The function of this value is to provide a unique value, and to 
        /// ensure that the HEADER CRCs are different after each header modification.
        /// </summary>
        Int32 dwUnique { get; set; }//4 byte
        /// <summary>
        /// rgnid[] (128 bytes): A fixed array of 32 NIDs, each corresponding to one of the 
        /// 32 possible NID_TYPEs. Different NID_TYPEs can have different starting nidIndex values.
        /// When a blank PST file is created, these values are initialized by 
        /// NID_TYPE according to the following table.
        /// Each of these NIDs indicates the last nidIndex value that had been allocated for 
        /// the corresponding NID_TYPE.When an NID of a particular type is assigned, 
        /// the corresponding slot in rgnid is also incremented by 1.
        /// 
        ///     NID_TYPE                  |  StartingnidIndex
        ///     NID_TYPE_NORMAL_FOLDER    |  1024 (0x400)
        ///     NID_TYPE_SEARCH_FOLDER    |  16384 (0x4000)
        ///     NID_TYPE_NORMAL_MESSAGE   |  65536 (0x10000)
        ///     NID_TYPE_ASSOC_MESSAGE    |  32768 (0x8000)
        ///     Any other NID_TYPE        |  1024 (0x400
        /// </summary>
        Int32[] rgnid { get; set; } = new Int32[32];//128 byte
        /// <summary>
        /// qwUnused (8 bytes): Unused space; MUST be set to zero. Unicode PST file format only
        /// </summary>
        Int64 qwUnused { get; set; }
        /// <summary>
        /// root (Unicode: 72 bytes; ANSI: 40 bytes): A ROOT structure
        /// </summary>
        byte[] root { get; set; } = new byte[72];
        /// <summary>
        /// dwAlign (4 bytes): Unused alignment bytes; MUST be set to zero. Unicode PST file format only.
        /// </summary>
        Int32 dwAlign { get; set; }
        /// <summary>
        /// rgbFM (128 bytes): Deprecated FMap. This is no longer used and MUST be filled with 0xFF.
        /// Readers SHOULD ignore the value of these bytes.
        /// </summary>
        byte[] rgbFM { get; set; } = new byte[128];
        /// <summary>
        /// rgbFP (128 bytes): Deprecated FPMap. This is no longer used and MUST be filled with 0xFF.
        /// Readers SHOULD ignore the value of these bytes.
        /// </summary>
        byte[] rgbFP { get; set; } = new byte[128];
        /// <summary>
        /// bSentinel (1 byte): MUST be set to 0x80.
        /// </summary>
        byte bSentinel { get; set; }
        /// <summary>
        /// bCryptMethod (1 byte): Indicates how the data within the PST file is encoded.
        /// MUST be set to one of the pre-defined values described in the following table.
        /// </summary>
        byte bCryptMethod { get; set; }
        Int16 rgbReserved { get; set; } // 2 byte
        Int64 bidNextB { get; set; } // 8 bytes
        Int32 dwCRCFull { get; set; }//CRC check 4 bytes
        byte[] rgbReserved2 { get; set; } = new byte[3];//3 bytes Readers Ignore
        byte bReserved { get; set; }//1 byte Readers Ignore
        byte[] rgbReserved3 { get; set; } = new byte[32];//32 bytes Readers Ignore

        public UnicodeHeader(MemoryMappedViewAccessor view, int offset)//offset = 24
        {
            int currentOffset = offset;
            this.bidUnused = view.ReadInt64(currentOffset);
            currentOffset += 8;
            view.ReadArray(currentOffset, this.bidNextP, 0, 8);
            currentOffset += 8;
            this.dwUnique = view.ReadInt32(currentOffset);
            currentOffset += 4;
            Readrgnid(view);
            currentOffset += 128;
            this.qwUnused = view.ReadInt64(currentOffset);
            currentOffset += 8;
            view.ReadArray(currentOffset, this.root, 0, 72);
            this.Root = new UnicodeRoot(view, currentOffset);
            currentOffset += 72;
            this.dwAlign = view.ReadInt32(currentOffset);
            currentOffset += 4;
            view.ReadArray(currentOffset, this.rgbFM, 0, 128);
            currentOffset += 128;
            view.ReadArray(currentOffset, this.rgbFP, 0, 128);
            currentOffset += 128;
            CheckbSentinel(view);
            currentOffset += 1;
            CheckbCryptMethod(view);
            currentOffset += 1;
            this.rgbReserved = view.ReadInt16(currentOffset);
            currentOffset += 2;
            this.bidNextB = view.ReadInt64(currentOffset);
            currentOffset += 8;
            this.dwCRCFull = view.ReadInt32(currentOffset);
            currentOffset += 4;
            view.ReadArray(currentOffset, this.rgbReserved2, 0, 3);
            currentOffset += 3;
            this.bReserved = view.ReadByte(currentOffset);
            currentOffset += 1;
            view.ReadArray(currentOffset, this.rgbReserved3, 0, 32);
            currentOffset += 32;
            if (currentOffset != 564)
                throw new Exception("Byte Length mismatch");



        }
        /// <summary>
        /// bSentinel Value must be 0x80
        /// </summary>
        /// <param name="view"></param>
        private void CheckbSentinel(MemoryMappedViewAccessor view)
        {
            byte bSentinelValue = 0x80;
            int offset = 512;
            this.bSentinel = view.ReadByte(offset);
            if (bSentinelValue == this.bSentinel)
                return;
            throw new Exception("Invalid bSentinel Value");
        }
        /// <summary>
        /// Check for encoding
        /// </summary>
        /// <param name="view"></param>
        /// <exception cref="Exception"></exception>
        private void CheckbCryptMethod(MemoryMappedViewAccessor view)
        {
            int offset = 513;
            this.bCryptMethod = view.ReadByte(offset);
            if (this.bCryptMethod == (byte)bCryptMethodType.NDB_CRYPT_NONE)
            {
                this.EncodingFriendlyName = bCryptMethodType.NDB_CRYPT_NONE;
                this.EncodingAlgorithm = "Data Blocks are not encoded";
                return;
            }
            if (this.bCryptMethod == (byte)bCryptMethodType.NDB_CRYPT_PERMUTE)
            {
                this.IsDataEncoded = true;
                this.EncodingFriendlyName = bCryptMethodType.NDB_CRYPT_PERMUTE;
                this.EncodingAlgorithm = "Encoded with Permutation algorithm";
                return;
            }
            if (this.bCryptMethod == (byte)bCryptMethodType.NDB_CRYPT_CYCLIC)
            {
                this.IsDataEncoded = true;
                this.EncodingFriendlyName = bCryptMethodType.NDB_CRYPT_CYCLIC;
                this.EncodingAlgorithm = "Encoded with Cyclic algorithm ";
                return;
            }
            if (this.bCryptMethod == (byte)bCryptMethodType.NDB_CRYPT_EDPCRYPTED)
            {
                this.IsDataEncoded = true;
                this.EncodingFriendlyName = bCryptMethodType.NDB_CRYPT_EDPCRYPTED;
                this.EncodingAlgorithm = "Encrypted with Windows Information Protection";
                return;
            }
            throw new Exception("Invalid bCryptMethod");
        }
        ///// <summary>
        ///// Read rgnid 
        ///// </summary>
        ///// <param name="view"></param>
        //private void Readrgnid(MemoryMappedViewAccessor view)
        //{
        //    int position = 44;
        //    byte[] temprgnid = new byte[4];
        //    for (int i = 0; i < this.rgnid.GetLength(0); i++)
        //    {
        //        view.ReadArray<byte>(position, temprgnid, 0, 4);
        //        for (int j = 0; j < 4; j++)
        //        {
        //            this.rgnid[i, j] = temprgnid[j];
        //        }
        //        position = position + 4;
        //    }

        //}
        /// <summary>
        /// Read rgnid 
        /// </summary>
        /// <param name="view"></param>
        private void Readrgnid(MemoryMappedViewAccessor view)
        {
            int position = 44;
            for (int i = 0; i < this.rgnid.Length; i++)
            {
                this.rgnid[i] = view.ReadInt32(position);
                position += 4;
            }

        }
    }
}
public enum bCryptMethodType : byte
{
    NDB_CRYPT_NONE = 0x00, //Data Blocks are not encoded
    NDB_CRYPT_PERMUTE = 0x01, //Encoded with Permutation algorithm
    NDB_CRYPT_CYCLIC = 0x02, //Encoded with Cyclic algorithm 
    NDB_CRYPT_EDPCRYPTED = 0x10 //Encrypted with Windows Information Protection
}