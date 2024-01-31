using core.LTP.BTreeOnHeap;
using core.NDB.Pages.BTree;
using System;

namespace core.LTP.PropertyContext
{
    /// <summary>
    /// 
    /// Property Context (PC)
    /// 
    /// The Property Context is built directly on top of a BTH. The existence of a PC is indicated at the HN 
    /// level, where bClientSig is set to bTypePC.Implementation-wise, the PC is simply a BTH with cbKey
    /// set to 2 and cbEnt set to 6 (see section 2.3.3.3). The following section explains the layout of a PC
    /// BTH record.
    /// Each property is stored as an entry in the BTH.Accessing a specific property is just a matter of
    /// searching the BTH for a key that matches the property identifier of the desired property, as the
    /// following data structure illustrates
    /// 
    /// Accessing the PC BTHHEADER
    /// The BTHHEADER structure of a PC is accessed through the hidUserRoot of the HNHDR structure of
    /// the containing HN.
    /// 
    /// HNID
    /// An HNID is a 32-bit hybrid value that represents either an HID or an NID.The determination is made
    /// by examining the hidType(or equivalently, nidType) value.The HNID refers to an HID if the
    /// hidType is NID_TYPE_HID.Otherwise, the HNID refers to an NID.
    /// An HNID that refers to an HID indicates that the item is stored in the data block.An HNID that refers
    /// to an NID indicates that the item is stored in the subnode block, and the NID is the local NID under
    /// the subnode where the raw data is located.
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
    /// 
    /// 
    /// 
    /// 
    /// This example shows a PC that is represented using a single data block and the subnode. For a small 
    /// BTH, a subnode is not used.The data block points to an HN, which in turn contains a BTH that is built
    /// on top of an HN as shown.For a PC, the hidUserRoot of the HN points to the BTHHEADER
    /// (allocated form the heap with HID set to 0x20). The hidRoot of the BTHHEADER points to the array
    /// of PC BTH records, which is also allocated from the heap(with HID set to 0x40).
    /// The property-value pairs in the PC BTH records are stored using the rules described in the previous
    /// sections.For a 32-bit PtypInteger32([MS-OXCDATA] section 2.11.1) property, the value is stored
    /// inline.For variable-size properties such as strings and binary BLOBs, an HNID is used to reference the
    /// data location. For the PtypString ([MS - OXCDATA] section 2.11.1) case, the data fits into the
    /// available space in the heap, and therefore is stored in the heap(HNID= 0x60). 
    /// In the PtypBinary([MS-OXCDATA] section 2.11.1) case, because the BLOB is too large to fit within
    /// the heap(larger than 3580 bytes), the subnode is used to store the data.In this case, the value of
    /// HNID is set to the subnode NID that contains the binary data. Note that the subnode structure in the
    /// diagram is significantly simplified for illustrative purposes.
    /// 
    /// 
    /// 
    /// </summary>
    public class PropertyContext
    {
        public BTH BTH { get; set; }
        /// <summary>
        /// wPropId (2 bytes): Property ID, as specified in [MS-OXCDATA] section 2.9. This is the upper 16 
        /// bits of the property tag value.This is a manifestation of the BTH record (section 2.3.2.3) and
        /// constitutes the key of this record.
        /// </summary>
        public UInt16 wPropId { get; set; }
        /// <summary>
        /// wPropType (2 bytes): Property type. This is the lower 16 bits of the property tag value, which 
        /// identifies the type of data that is associated with the property.The complete list of property type
        /// values and their data sizes are specified in [MS - OXCDATA] section 2.11.1.
        /// </summary>
        public UInt16 wPropType { get; set; }
        /// <summary>
        /// dwValueHnid (4 bytes): Depending on the data size of the property type indicated by wPropType
        /// and a few other factors, this field represents different values.The following table explains the
        /// value contained in dwValueHnid based on the different scenarios. In the event where the
        /// dwValueHnid value contains an HID or NID(section 2.3.3.2), the actual data is stored in the
        /// corresponding heap or subnode entry, respectively.
        /// 
        /// 
        ///     Variable size? | Fixed data size   |  NID_TYPE(dwValueHnid) == NID_TYPE_HID?    |  dwValueHnid 
        ///     N              | <= 4 bytes        |  -                                         |  Data Value 
        ///                    | > 4 bytes         |  Y                                         |  HID
        ///     Y              | -                 |  Y                                         |  HID(<= 3580 bytes) 
        ///                    |                   |  N                                         |  NID(subnode, > 3580 bytes)
        ///                
        /// 
        /// 
        /// </summary>
        public UInt32 dwValueHnid { get; set; }
        public PropertyContext(BlockBTreeEntry dataBlockBTreeEntry,BlockBTreeEntry subNodeDataBlockBTreeEntry)
        {
              
        }
    }
}
