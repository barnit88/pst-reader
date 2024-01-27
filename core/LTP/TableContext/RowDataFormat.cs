using System;

namespace core.LTP.TableContext
{
    /// <summary>
    /// 
    /// Row Data Format
    /// The following is the organization of a single row of data in the Row Matrix.Rows of data are tightlypacked in the Row Matrix, and the size of each data row is TCINFO.rgib[TCI_bm] bytes.The
    /// following constraints exist for the columns within the structure.
    /// Columns MUST be sorted
    /// 1. PidTagLtpRowId MUST be assigned iBit == 0
    /// 2. PidTagLtpRowId MUST be assigned ibData == 0
    /// 3. PidTagLtpRowVer MUST be assigned iBit == 1
    /// 4. PidTagLtpRowVer MUST be assigned ibData == 4
    /// 5. For any other columns, iBit can change/be any valid value (other than 0 and 1)
    /// 6. For any other columns, ibData can be any valid value(other than 0 and 4)
    /// 
    /// Space is reserved for a column in the Row Matrix, regardless of the corresponding CEB bit value for 
    /// that column.Specifically, an fCEB bit value of TRUE indicates that the corresponding column value in
    /// the Row matrix is valid and SHOULD be returned if requested.However, an fCEB bit value of false
    /// indicates that the corresponding column value in the Row matrix is "not set" or "invalid". In this case,
    /// the property MUST be "not found" if requested.
    /// The size of rgCEB is CEIL(TCINFO.cCols / 8) bytes.Extra lower-order bits SHOULD be ignored.
    /// Creators of a new PST MUST set the extra lower-order bits to zero
    /// 
    /// </summary>
    public class RowDataFormat
    {

        /// <summary>
        /// dwRowID (4 bytes): The 32-bit value that corresponds to the dwRowID value in this row's 
        /// corresponding TCROWID record.Note that this value corresponds to the PidTagLtpRowId
        /// property.
        /// </summary>
        public Int32 dwRowID { get; set; }
        /// <summary>
        /// rgdwData (variable): 4-byte-aligned Column data. This region contains data with a size that is a 
        /// multiple of 4 bytes.The types of data stored in this region are 4-byte and 8-byte values.
        /// </summary>
        public byte[] rgdwData { get; set; }
        /// <summary>
        /// rgwData (variable): 2-byte-aligned Column data. This region contains data that are 2 bytes in size.
        /// </summary>
        public byte[] rgwData { get; set; }
        /// <summary>
        /// rgbData (variable): Byte-aligned Column data. This region contains data that are byte-sized.
        /// </summary>
        public byte[] rgbData { get; set; }
        /// <summary>
        /// rgbCEB (variable): Cell Existence Block. This array of bits comprises the CEB, in which each bit 
        /// corresponds to a particular Column in the current row.The mapping between CEB bits and actual
        /// Columns is based on the iBit member of each TCOLDESC(section 2.3.4.2), where an iBit value
        /// of zero maps to the Most Significant Bit(MSB) of the 0th byte of the CEB array(rgCEB[0]). 
        /// Subsequent iBit values map to the next less-significant bit until the Least Significant Bit(LSB) is 
        /// reached, where the subsequent iBit can be found in the MSB of the next byte in the CEB array
        /// and the process repeats itself.Programmatically, the Cell Existence Bit that corresponds to iBit can
        /// be extracted as follows:
        ///     BOOL fCEB = !!(rgCEB[iBit / 8] & (1 << (7 - (iBit % 8))));
        /// Space is reserved for a column in the Row Matrix, regardless of the corresponding CEB bit value for 
        /// that column.Specifically, an fCEB bit value of TRUE indicates that the corresponding column value in
        /// the Row matrix is valid and SHOULD be returned if requested.However, an fCEB bit value of false
        /// indicates that the corresponding column value in the Row matrix is "not set" or "invalid". In this case,
        /// the property MUST be "not found" if requested.
        /// The size of rgCEB is CEIL(TCINFO.cCols / 8) bytes.Extra lower-order bits SHOULD be ignored.
        /// Creators of a new PST MUST set the extra lower-order bits to zero.
        /// </summary>
        public byte[] rgbCEB { get; set; }
    }
}
