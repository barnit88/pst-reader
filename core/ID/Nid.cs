using System;
using System.Collections;

namespace Core.PST.ID
{
    public class Nid
    {
        /// <summary>
        /// Nid Type specifies what type of node it is.
        /// Identifies the type of the node represented by the NID.
        /// 
        ///     Value |  Friendly Name                   | Description
        ///     0x00  |  NID_TYPE_HID                    | Heap node
        ///     0x01  |  NID_TYPE_INTERNAL               | Internal node
        ///     0x02  |  NID_TYPE_NORMAL_FOLDER          | Normal Folder object (PC)
        ///     0x03  |  NID_TYPE_SEARCH_FOLDER          | Search Folder object (PC)
        ///     0x04  |  NID_TYPE_NORMAL_MESSAGE         | Normal Message object (PC)
        ///     0x05  |  NID_TYPE_ATTACHMENT             | Attachment object (PC)
        ///     0x06  |  NID_TYPE_SEARCH_UPDATE_QUEUE    | Queue of changed objects for search Folder objects
        ///     0x07  |  NID_TYPE_SEARCH_CRITERIA_OBJECT | Defines the search criteria for a search Folder object
        ///     0x08  |  NID_TYPE_ASSOC_MESSAGE          | Folder associated information (FAI) Message object (PC)
        ///     0x0A  |  NID_TYPE_CONTENTS_TABLE_INDEX   | Internal, persisted view-related
        ///     0X0B  |  NID_TYPE_RECEIVE_FOLDER_TABLE   | Receive Folder object (Inbox)
        ///     0x0C  |  NID_TYPE_OUTGOING_QUEUE_TABLE   | Outbound queue (Outbox)
        ///     0x0D  |  NID_TYPE_HIERARCHY_TABLE        | Hierarchy table (TC)
        ///     0x0E  |  NID_TYPE_CONTENTS_TABLE         | Contents table (TC)
        ///     0x0F  |  NID_TYPE_ASSOC_CONTENTS_TABLE   | FAI contents table (TC)
        ///     0x10  |  NID_TYPE_SEARCH_CONTENTS_TABLE  | Contents table (TC) of a search Folder object
        ///     0x11  |  NID_TYPE_ATTACHMENT_TABLE       | Attachment table (TC)
        ///     0x12  |  NID_TYPE_RECIPIENT_TABLE        | Recipient table (TC)
        ///     0x13  |  NID_TYPE_SEARCH_TABLE_INDEX     | Internal, persisted view-related
        ///     0x1F  |  NID_TYPE_LTP                    | LTP 
        ///  
        /// 
        /// </summary>
        public NidType NidType { get; set; }
        /// <summary>
        /// nidType (5 bits): Identifies the type of the node represented by the NID. 
        /// The following table specifies a list of values for nidType.However, 
        /// it is worth noting that nidType has no meaning to the 
        /// structures defined in the NDB Layer.
        /// </summary>
        BitArray nidType = new BitArray(5);
        /// <summary>
        /// nidIndex (27 bits): The identification portion of the NID.
        /// </summary>
        BitArray nidIndex = new BitArray(27);
        public Nid(ulong nid)
        {
            UInt32 calculatedNid = CalculateNID(nid);
            var arr = new BitArray(BitConverter.GetBytes(calculatedNid));
            for (int i = 0; i < arr.Length; i++)
            {
                if (i < 5)
                    nidType[i] = arr[i];
                else
                    nidIndex[i - 5] = arr[i];
            }

        }
        public UInt32 CalculateNID(ulong btkey)
        {
            // Assuming Unicode PST and little-endian system
            UInt32 nid = (uint)(btkey & 0xFFFFFFFF); // Extract lower 32 bits

            //// Extract nidType and check validity
            //byte nidType = (byte)((nid >> 27) & 0x1F);
            return nid;
        }
        public NidType GetNidTypey(BitArray bits)
        {
            byte result = 0;

            for (int i = 0; i < bits.Length; i++)
            {
                if (bits[i])
                {
                    result += (byte)(1 << i); // Multiply bit by 2^i and add to result
                }
            }
            return (NidType)result;
        }
    }
}
