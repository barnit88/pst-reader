using System;

namespace core.LTP.TableContext
{
    /// <summary>
    /// TCROWID
    /// The TCROWID structure is a manifestation of the BTH data record (section 2.3.2.3). The size of the 
    /// TCROWID structure varies depending on the version of the PST.For the Unicode PST, each record in
    /// the BTH are 8 bytes in size, where cbKey is set to 4 and cEnt is set to 4. For an ANSI PST, each
    /// record is 6 bytes in size, where cbKey is set to 4 and cEnt is set to 2. The following is the binary
    /// layout of the TCROWID structure.
    /// </summary>
    public class TCROWID
    {

        /// <summary>
        /// dwRowID (4 bytes): This is the 32-bit primary key value that uniquely identifies a row in 
        /// the Row Matrix.
        /// </summary>
        public Int32 dwRowID { get; set; }
        /// <summary>
        /// dwRowIndex (Unicode: 4 bytes; ANSI: 2 bytes): The 0-based index to the corresponding row in 
        /// the Row Matrix.Note that for ANSI PSTs, the maximum number of rows is 2^16.
        /// </summary>
        public Int32 dwRowIndex { get; set; }
        public TCROWID()
        {

        }
    }
}