using core.NDBLayer.ID;
using System;

namespace core.Messaging
{
    /// <summary>
    /// Objects in the message store are accessed externally using EntryIDs 
    /// ([MS-OXCDATA] section 2.2), where within the PST, objects are accessed 
    /// using their respective NIDs.The following explains the layout of the 
    /// ENTRYID structure, which is used to map between an NID and its EntryID:
    /// 
    /// The corresponding NID of an EntryID can be directly extracted from the EntryID structure. 
    /// In addition, the NID_TYPE of the NID can be further verified to ensure that the 
    /// type of node(for example,NID_TYPE_NORMAL_MESSAGE) actually matches the type of 
    /// object being referenced.Also, as a further verification mechanism, implementations 
    /// can compare the uid field against the PidTagRecordKey property in the message store 
    /// to ensure the EntryID actually refers to an item in the current PST.
    /// This is particularly useful if the implementation supports opening more than one PST 
    /// at a time.Conversely, the procedure for converting an NID to an 
    /// EntryID simply involves constructing the ENTRYID structure from the     /// NID and the PST Provider UID(PidTagRecordKey).
    /// </summary>
    public class EntryId
    {
        public Nid Nid { get; set; }
        /// <summary>
        /// rgbFlags (4 bytes): Flags; each of these bytes MUST be initialized to zero.
        /// </summary>
        public UInt32 rgbFlags { get; set; }
        /// <summary>
        /// uid (16 bytes): The provider UID of this PST, which is the value of the 
        /// PidTagRecordKey property in the message store.If this property does not exist, 
        /// the PST client MAY generate a new unique ID, or reject the PST as invalid.
        /// </summary>
        public byte[] uid { get; set; }
        /// <summary>
        /// nid (4 bytes): This is the corresponding NID of the underlying node 
        /// that represents the object.
        /// </summary>
        public UInt32 nid { get; set; }
        public EntryId(byte[] dataBytes, int offset = 0)
        {
            if (dataBytes.Length != 24)
                throw new Exception("Data Bytes Lenght error in EntryID");
            this.rgbFlags = BitConverter.ToUInt32(dataBytes, offset);
            this.uid = new byte[16];
            Array.Copy(dataBytes, offset + 4, this.uid, 0, 16);
            this.nid = BitConverter.ToUInt32(dataBytes, 20);
            this.Nid = new Nid(this.nid);
        }
    }
}
