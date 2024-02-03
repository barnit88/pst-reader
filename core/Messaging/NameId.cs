using System;

namespace core.Messaging
{
    /// <summary>
    /// Each NAMEID record corresponds to a named property. The contents of the NAMEID record can be 
    /// interpreted in two ways, depending on the value of the N bit.
    /// </summary>
    public class NameId
    {
        public Guid Guid { get; set; }
        /// <summary>
        /// dwPropertyID (4 bytes): If the N field is 1, this value is the byte offset into the String stream in 
        /// which the string name of the property is stored.If the N field is 0, this value contains the value of
        /// numerical name.
        /// </summary>
        public UInt32 dwPropertyID { get; set; }
        /// <summary>
        /// N (1 bit): Named property identifier type. If this value is 1, the named property identifier is a string. 
        /// If this value is 0, the named property identifier is a 16-bit numerical value.
        /// </summary>
        public bool N { get; set; }
        /// <summary>
        /// wGuid (15 bits): GUID index. If this value is 1 or 2, the named property's GUID is one of 2 wellknown GUIDs. 
        /// If this value is greater than 2, this value is the index plus 3 into the GUID Stream 
        /// where the GUID associated with this named property is located.The following table explains how
        /// the wGuid value works.
        /// 
        /// wGuid   Friendly name                   Description
        /// 0x0000  NAMEID_GUID_NONE                No GUID(N= 1).
        /// 0x0001  NAMEID_GUID_MAPI                The GUID is PS_MAPI([MS-OXPROPS] section 1.3.2).
        /// 0x0002  NAMEID_GUID_PUBLIC_STRINGS      The GUID is PS_PUBLIC_STRINGS([MSOXPROPS] section 1.3.2).
        /// 0x0003  N/A                             GUID is found at the(N-3) * 16 byte offset in the GUID Stream.
        /// </summary>
        public byte wGuid { get; set; }
        /// <summary>
        /// wPropIdx (2 bytes): Property index. This is the ordinal number of the named property, which is 
        /// used to calculate the NPID of this named property.The NPID of this named property is calculated by
        /// adding 0x8000 to wPropIdx.
        /// </summary>
        public UInt16 wPropIdx { get; set; }
        public NameId(byte[] dataBytes, int offset, NamedPropertyLookup lookup)
        {

            this.dwPropertyID = BitConverter.ToUInt32(dataBytes, offset);
            this.N = (dataBytes[offset + 4] & 0x1) == 1;
            var guidType = BitConverter.ToUInt16(dataBytes, offset + 4) >> 1;
            if (guidType == 1)
            {
                this.Guid = new Guid("00020328-0000-0000-C000-000000000046");//PS-MAPI
            }
            else if (guidType == 2)
            {
                this.Guid = new Guid("00020329-0000-0000-C000-000000000046");//PS_PUBLIC_STRINGS
            }
            else
            {
                this.Guid = new Guid(lookup._GUIDs.RangeSubset((guidType - 3) * 16, 16));
            }

            this.wPropIdx = (UInt16)(0x8000 + BitConverter.ToUInt16(dataBytes, offset + 6));

            //this.PropertyID = BitConverter.ToUInt32(bytes, offset);
            //this.PropertyIDStringOffset = (bytes[offset + 4] & 0x1) == 1;
            //var guidType = BitConverter.ToUInt16(bytes, offset + 4) >> 1;
            //if (guidType == 1)
            //{
            //    this.Guid = new Guid("00020328-0000-0000-C000-000000000046");//PS-MAPI
            //}
            //else if (guidType == 2)
            //{
            //    this.Guid = new Guid("00020329-0000-0000-C000-000000000046");//PS_PUBLIC_STRINGS
            //}
            //else
            //{
            //    this.Guid = new Guid(lookup._GUIDs.RangeSubset((guidType - 3) * 16, 16));
            //}

            //this.PropIndex = (UInt16)(0x8000 + BitConverter.ToUInt16(bytes, offset + 6));

        }

    }
}
