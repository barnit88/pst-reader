using System;

namespace core.LTP.PropertyContext
{
    /// <summary>
    /// 
    /// Multi-Valued Properties
    /// Multi-valued(MV) properties are properties that contain an array of values.A Multi-Valued property
    /// can be derived from any basic property type, for example: PtypInteger32, PtypGuid, PtypString, 
    /// PtypBinary([MS-OXCDATA] section 2.11.1). The value of an MV property is stored using an HNID, 
    /// and is encoded in a packed binary format.The following explains the data format for Multi-valued
    /// properties
    /// 
    /// MV Properties with Fixed-size Base Type
    /// When an MV property contains elements of fixed size, such as PtypInteger32 or PtypGuid, the data
    /// layout is very straightforward. The number of elements present is determined by dividing the size of
    /// the heap or node data size by the size of the data type.Each data element is aligned with respect to
    /// its own data type, which results in a tightly - packed array of elements.
    /// For example, if the HID points to an allocation of 64 bytes, and the Fixed - size type is a
    /// PtypInteger64 (8 bytes), then the number of items in the MV property is 64 / 8 = 8 items.The size
    /// of the heap or node data MUST be an integer multiple of the data type size
    /// 
    /// 
    /// MV Properties with Variable-size Base Type
    /// When the MV property contains variable-size elements, such as PtypBinary, PtypString, or PtypString8), 
    /// the data layout is more complex.The following is the data format of a multi-valued
    /// property with variable-size base type.
    /// 
    /// 
    /// </summary>
    public class MVPropertyContext
    {
        /// <summary>
        /// ulCount (4 bytes): Number of data items in the array
        /// </summary>
        public UInt32 ulCount { get; set; }
        /// <summary>
        /// rgulDataOffsets (variable): An array of ULONG values that represent offsets to the start of each 
        /// data item for the MV array.Offsets are relative to the beginning of the MV property data record.
        /// The length of the Nth data item is calculated as: rgulDataOffsets[N + 1] – rgulDataOffsets[N],
        /// with the exception of the last item, in which the total size of the MV property data record is used
        /// instead of rgulDataOffsets[N+1]. 
        /// </summary>
        public byte[] rgulDataOffsets { get; set; }
        /// <summary>
        /// rgDataItems (variable): A byte-aligned array of data items. Individual items are delineated using 
        /// the rgulDataOffsets values.
        /// </summary>
        public byte[] rgDataItems { get; set; }
    }
}
