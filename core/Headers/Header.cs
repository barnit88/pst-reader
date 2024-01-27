using Core.PST.Headers.Ansi;
using Core.PST.Headers.Unicode;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Xml.Linq;

namespace Core.PST.Headers
{
    /// <summary>
    /// The HEADER structure is located at the beginning of the PST file (absolute file offset 0), and contains 
    /// metadata about the PST file, as well as the ROOT information to access the NDB Layer data
    /// structures.Note that the layout of the HEADER structure, including the location and relative ordering
    /// of some fields, differs between the Unicode and ANSI versions.
    /// </summary>
    public class Header
    {
        private int offset = 0;//First byte 0 byte
        private int length = 580;//580th byte . Total bytes occupied by header
        public UnicodeHeader UnicodeHeader { get; set; } = null;
        public AnsiHeader AnsiHeader { get; set; } = null;
        public string FileFormat { get; set; } = null;
        public bool IsUnicode { get; set; } = false;
        public bool IsAnsi { get; set; } = false;
        /// <summary>
        /// dwMagic (4 bytes): MUST be "{ 0x21, 0x42, 0x44, 0x4E } ("!BDN")".
        /// If this value is not available the file is not a valid PST File
        /// </summary>
        Int32 dwMagic { get; set; }
        /// <summary>
        /// dwCRCPartial (4 bytes): The 32-bit cyclic redundancy check (CRC) value of the 471 bytes of 
        /// data starting from wMagicClient(0ffset 0x0008)
        /// </summary>
        Int32 dwCRCPartial { get; set; }
        /// <summary>
        /// wMagicClient (2 bytes): MUST be "{ 0x53, 0x4D }"
        /// This value defines different types
        /// { 0x53, 0x4d };//PST FIle
        /// { 0x41, 0x42 };//PAB FIle
        /// { 0x53, 0x4f };//OST FIle
        /// </summary>
        Int16 wMagicClient { get; set; }
        /// <summary>
        /// wVer (2 bytes): File format version. This value MUST be 14 or 15 if the file is an ANSI PST file, and 
        /// MUST be greater than 23 if the file is a Unicode PST file.If the value is 37, it indicates that the file
        /// is written by an Outlook of version that supports Windows Information Protection (WIP). The data
        /// MAY have been protected by WIP
        /// 
        /// Helps determines the file type is ANSI or UNICODE.
        /// </summary>
        Int16 wVer { get; set; }
        /// <summary>
        /// wVerClient (2 bytes): Client file format version. The version that corresponds to the format 
        /// described in this document is 19. Creators of a new PST file based on this document 
        /// initialize this value to 19.
        /// </summary>
        Int16 wVerClient { get; set; }
        /// <summary>
        /// bPlatformCreate (1 byte): This value MUST be set to 0x01.
        /// </summary>
        byte bPlatformCreate { get; set; }
        /// <summary>
        /// bPlatformAccess (1 byte): This value MUST be set to 0x01.
        /// </summary>
        byte bPlatformAccess { get; set; }
        /// <summary>
        /// dwReserved1 (4 bytes): Implementations SHOULD ignore this value and SHOULD NOT modify it. 
        /// Creators of a new PST file MUST initialize this value to zero.
        /// </summary>
        Int32 dwReserved1 { get; set; }
        /// <summary>
        /// dwReserved2 (4 bytes): Implementations SHOULD ignore this value and SHOULD NOT modify it. 
        /// Creators of a new PST file MUST initialize this value to zero.
        /// </summary>
        Int32 dwReserved2 { get; set; }
        public Header(MemoryMappedFile memoryMappedFile)
        {
            using (var view = memoryMappedFile.CreateViewAccessor(offset, length))
            {
                CheckdwMagic(view);
                this.dwCRCPartial = view.ReadInt32(4);
                CheckwMagicClient(view);
                CheckwVer(view);
                CheckwVerClient(view);
                this.bPlatformCreate = view.ReadByte(14);
                this.bPlatformAccess = view.ReadByte(15);
                this.dwReserved1 = view.ReadInt32(16);
                this.dwReserved2 = view.ReadInt32(20);
                if (this.IsUnicode)
                    this.UnicodeHeader = new UnicodeHeader(view, 24);
                if (this.IsAnsi)
                    this.AnsiHeader = new AnsiHeader();
            }
        }
        /// <summary>
        /// A valid pst first 4 byte should always be { 0x21, 0x42, 0x44, 0x4E }
        /// which is !BDN
        /// </summary>
        /// <param name="dwMagicValue"></param>
        /// <returns></returns>
        private void CheckdwMagic(MemoryMappedViewAccessor view)
        {
            byte[] dwMagic = { 0x21, 0x42, 0x44, 0x4E };//!BDN
            int offset = 0;
            this.dwMagic = view.ReadInt32(offset);
            if (BitConverter.ToInt32(dwMagic, 0) == this.dwMagic)
                return;
            throw new System.Exception("Not a valid file");
        }
        /// <summary>
        /// Gives Value Based on the File Type (PST, PAB, OST)
        /// </summary>
        /// <param name="wMagicClientValue">vMagicClientValue bytes with length 2</param>
        /// <returns>File Type</returns>
        /// <exception cref="Exception"></exception>
        private void CheckwMagicClient(MemoryMappedViewAccessor view)
        {
            int offset = 8;
            byte[] pst = { 0x53, 0x4d };//PST FIle
            byte[] pab = { 0x41, 0x42 };//PAB FIle
            byte[] ost = { 0x53, 0x4f };//OST FIle
            Int16 pstFile = BitConverter.ToInt16(pst);//SM
            Int16 pabFile = BitConverter.ToInt16(pab);//AB
            Int16 ostFile = BitConverter.ToInt16(ost);//SO
            this.wMagicClient = view.ReadInt16(offset);
            if (pstFile == this.wMagicClient)
                return;
            //if (pabFile == this.wMagicClient)
            //    throw new Exception("PST File only supported");
            //if (ostFile == this.wMagicClient)
            //    throw new Exception("PST File only supported");

            throw new Exception("PST File only supported");
        }
        /// <summary>
        /// File format version. This value MUST be 14 or 15 if the file is an ANSI PST file, and
        /// MUST be greater than 23 if the file is a Unicode PST file.If the value is 37, it indicates that the file
        /// is written by an Outlook of version that supports Windows Information Protection (WIP). The data
        /// MAY have been protected by WIP.
        /// </summary>
        /// <param name="view"></param>
        /// <exception cref="System.Exception"></exception>
        private void CheckwVer(MemoryMappedViewAccessor view)
        {
            int offset = 10;
            this.wVer = view.ReadInt16(offset);
            if (this.wVer == 14 || this.wVer == 15)
            {
                this.IsAnsi = true;
                this.FileFormat = "ANSI";
                return;
            }
            if (this.wVer >= 23 && this.wVer <= 36)
            {
                this.IsUnicode = true;
                this.FileFormat = "UNICODE";
                return;
            }
            throw new System.Exception("Invaid Encoding Type");
        }
        /// <summary>
        /// Value should always be 19
        /// </summary>
        /// <param name="view"></param>
        /// <exception cref="Exception"></exception>
        private void CheckwVerClient(MemoryMappedViewAccessor view)
        {
            int offset = 12;
            int wVerClientValue = 19;
            this.wVerClient = view.ReadInt16(offset);
            if (wVerClientValue == this.wVerClient)
                return;
            throw new Exception("Invalid wVerClient.");
        }
    }
}