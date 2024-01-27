using System;

namespace core.LTP.TableContext
{
    /// <summary>
    /// TCINFO is the header structure for the TC. The TCINFO is accessed using the hidUserRoot field in the 
    /// HNHDR structure of the containing HN.The header contains the column definitions and other relevant
    /// data
    /// </summary>
    public class TCINFO
    {
        /// <summary>
        /// bType (1 byte): TC signature; MUST be set to bTypeTC.
        /// </summary>
        public byte bType { get; set; }
        /// <summary>
        /// cCols (1 byte): Column count. This specifies the number of columns in the TC.
        /// </summary>
        public byte cCols { get; set; }
        /// <summary>
        /// rgib (8 bytes): This is an array of 4 16-bit values that specify the offsets of various groups of data in 
        /// the actual row data.The application of this array is specified in section 2.3.4.4, which covers the
        /// data layout of the Row Matrix.The following table lists the meaning of each value: 
        /// 
        /// Index |  Friendly name  | Meaning of rgib[Index] value
        /// 0     |  TCI_4b         | Ending offset of 8- and 4-byte data value group.
        /// 1     |  TCI_2b         | Ending offset of 2-byte data value group.
        /// 2     |  TCI_1b         | Ending offset of 1-byte data value group.
        /// 3     |  TCI_bm         | Ending offset of the Cell Existence Block.
        /// 
        /// 
        /// </summary>
        public UInt64 rgib { get; set; }
        /// <summary>
        /// hidRowIndex (4 bytes): HID to the Row ID BTH. The Row ID BTH contains (RowID, RowIndex) 
        /// value pairs that correspond to each row of the TC.The RowID is a value that is associated with the
        /// row identified by the RowIndex, whose meaning depends on the higher level structure that
        /// implements this TC.The RowIndex is the zero-based index to a particular row in the Row Matrix.
        /// </summary>
        public UInt32 hidRowIndex { get; set; }
        /// <summary>
        /// hnidRows (4 bytes): HNID to the Row Matrix (that is, actual table data). This value is set to zero if 
        /// the TC contains no rows.
        /// </summary>
        public UInt32 hnidRows { get; set; }
        /// <summary>
        /// hidIndex (4 bytes): Deprecated. Implementations SHOULD ignore this value, and creators of a new 
        /// PST MUST set this value to zero
        /// </summary>
        public UInt32 hidIndex { get; set; }
        /// <summary>
        /// rgTCOLDESC (variable): Array of Column Descriptors. This array contains cCols entries of type 
        /// TCOLDESC structures that define each TC column.The entries in this array MUST be sorted by
        /// the tag field of TCOLDESC.
        /// </summary>
        public byte[] rgTCOLDESC { get; set; }
    }
}
