using System;

namespace core.LTP.TableContext
{
    /// <summary>
    /// TCOLDESC
    /// The TCOLDESC structure describes a single column in the TC, which includes metadata about the size 
    /// of the data associated with this column, as well as whether a column exists, and how to locate the
    /// column data from the Row Matrix
    /// </summary>
    public class TCOLDESC
    {
        /// <summary>
        /// tag (4 bytes): This field specifies that 32-bit tag that is associated with the column.
        /// </summary>
        public Int32 tag { get; set; }
        /// <summary>
        /// ibData (2 bytes): Data Offset. This field indicates the offset from the beginning of the row data (in 
        /// the Row Matrix) where the data for this column can be retrieved.Because each data row is laid
        /// out the same way in the Row Matrix, the Column data for each row can be found at the same
        /// offset.
        /// </summary>
        public Int16 ibData { get; set; }
        /// <summary>
        /// cbData (1 byte): Data size. This field specifies the size of the data associated with this column (that 
        /// is, "width" of the column), in bytes per row.However, in the case of variable-sized data, this value 
        /// is set to the size of an HNID instead.This is explained further in section 2.3.4.4.
        /// </summary>
        public byte cbData { get; set; }
        /// <summary>
        /// iBit (1 byte): Cell Existence Bitmap Index.This value is the 0-based index into the CEB bit that
        /// corresponds to this Column.A detailed explanation of the mapping mechanism will be discussed in
        /// section 2.3.4.4.1.
        /// </summary>
        public byte iBit{ get; set; }
    }
}
