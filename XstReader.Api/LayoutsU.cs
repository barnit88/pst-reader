﻿// Project site: https://github.com/iluvadev/XstReader
//
// Based on the great work of Dijji. 
// Original project: https://github.com/dijji/XstReader
//
// Issues: https://github.com/iluvadev/XstReader/issues
// License (Ms-PL): https://github.com/iluvadev/XstReader/blob/master/license.md
//
// Copyright (c) 2016, Dijji, and released under Ms-PL.  This can be found in the root of this distribution. 

using System;
using System.Runtime.InteropServices;

namespace XstReader
{
    // xst layouts that are Unicode specific
    // All layouts and names are taken from <MS-PST>

    // Constants
    internal class LayoutsU
    {
        public const int BTPAGEEntryBytes = 488;
    }

    // NDB layer

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct FileHeader2Unicode
    {
        public UInt64 bidUnused;
        public UInt64 bidNextP;
        public UInt32 dwUnique;
        public fixed UInt32 rgnid[32];
        public UInt64 qwUnused;
        public RootUnicode root;
        public UInt32 dwAlign;
        public fixed Byte rgbFM[128];
        public fixed Byte rgbFP[128];
        public Byte bSentinel;
        public EbCryptMethod bCryptMethod;
        public UInt16 rgbReserved;
        public UInt64 bidNextB;
        public UInt32 dwCRCFull;
        public UInt32 rgbReserved2;
        public fixed Byte rgbReserved3[32];
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct RootUnicode
    {
        public UInt32 dwReserved;
        public UInt64 ibFileEof;
        public UInt64 ibAMapLast;
        public UInt64 cbAMapFree;
        public UInt64 cbPMapFree;
        public BREFUnicode BREFNBT;
        public BREFUnicode BREFBBT;
        public Byte fAMapValid;
        public Byte bReserved;
        public UInt16 wReserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BREFUnicode
    {
        public UInt64 bid;
        public UInt64 ib;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct BTPAGEUnicode
    {
        public fixed Byte rgentries[LayoutsU.BTPAGEEntryBytes];
        public Byte cEnt;
        public Byte cEntMax;
        public Byte cbEnt;
        public Byte cLevel;
        public UInt32 dwPadding;
        public PAGETRAILERUnicode pageTrailer;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct PAGETRAILERUnicode
    {
        public Eptype ptype;
        public Eptype ptypeRepeat;
        public UInt16 wSig;
        public UInt32 dwCRC; 
        public UInt64 bid;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BTENTRYUnicode
    {
        public UInt64 btkey;
        public BREFUnicode BREF;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BBTENTRYUnicode
    {
        public BREFUnicode BREF;
        public UInt16 cb;
        public UInt16 cRef;
        public UInt32 dwPadding;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct NBTENTRYUnicode
    {
        public NID nid;
        public UInt32 dwPad;
        public UInt64 bidData;
        public UInt64 bidSub;
        public UInt32 nidParent;
        public UInt32 dwPadding;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BLOCKTRAILERUnicode
    {
        public UInt16 cb;
        public UInt16 wSig;
        public UInt32 dwCRC;
        public UInt64 bid;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct XBLOCKUnicode
    {
        public EbType btype;
        public Byte cLevel;
        public UInt16 cEnt;
        public UInt32 lcbTotal;
        // Marshal the following array manually
        //public UInt64[] rgbid;
    }

    // We just use the XBLOCKUnicode structure for XXBLOCKUnicode, as it is the same
    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    //internal struct XXBLOCKUnicode
    //{
    //    public EbType btype;
    //    public Byte cLevel;
    //    public UInt16 cEnt;
    //    public UInt32 lcbTotal;
    //    //public UInt64[] rgbid;
    //}

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct SLENTRYUnicode
    {
        public UInt64 nid;
        public UInt64 bidData;
        public UInt64 bidSub;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct SLBLOCKUnicode
    {
        public EbType btype;
        public Byte cLevel;
        public UInt16 cEnt;
        public UInt32 dwPadding;
        // Marshal the following array manually
        //public SLENTRYUnicode[] rgentries;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct SIENTRYUnicode
    {
        public UInt64 nid;
        public UInt64 bid;
    }

    // We just use the SLBLOCKUnicode structure for SIBLOCKUnicode, as it is the same apart from the array type
    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    //internal struct SIBLOCKUnicode
    //{
    //    public EbType btype;
    //    public Byte cLevel;
    //    public UInt16 cEnt;
    //    public UInt32 dwPadding;
    //    public SIENTRYUnicode[] rgentries;
    //}

    // LTP layer

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TCROWIDUnicode
    {
        public UInt32 dwRowID;
        public UInt32 dwRowIndex;
    }
}
