using System;
using System.Security.Authentication;

namespace core.NDBLayer.ID
{
    public class Nid
    {
        public ulong _Nid { get; set; }
        /// <summary>
        /// Special Internal NIDs
        /// 
        /// This section focuses on a special NID_TYPE: NID_TYPE_INTERNAL (0x01). As specified in section 
        /// 2.2.2.1, the nidType of an NID is ignored by the NDB Layer, and is left for the interpretation by
        /// higher level implementations. 
        /// In the Messaging layer, nodes with various nidType values are also used to build related structures
        /// that collectively represent complex structures (for example, a Folder object is a composite object 
        /// that consists of a PC and three TCs of various nidType values). In addition, the Messaging layer also
        /// uses NID_TYPE_INTERNAL to define special NIDs that have special functions.
        /// Because top-level NIDs are globally-unique within a PST, it follows that each instance of a special NID
        /// can only appear once in a PST. The following table lists all predefined internal NIDs.
        ///     
        ///     Special Internal NID's table
        ///     Value  |  Friendly name                     |  Meaning
        ///     0x21   |  NID_MESSAGE_STORE                 |  Message store node(section 2.4.3).
        ///     0x61   |  NID_NAME_TO_ID_MAP                |  Named Properties Map(section 2.4.7).
        ///     0xA1   |  NID_NORMAL_FOLDER_TEMPLATE        |  Special template node for an empty Folder object.
        ///     0xC1   |  NID_SEARCH_FOLDER_TEMPLATE        |  Special template node for an empty search Folder object.
        ///     0x122  |  NID_ROOT_FOLDER                   |  Root Mailbox Folder object of PST.
        ///     0x1E1  |  NID_SEARCH_MANAGEMENT_QUEUE       |  Queue of Pending Search-related updates.
        ///     0x201  |  NID_SEARCH_ACTIVITY_LIST          |  Folder object NIDs with active Search activity.
        ///     0x241  |  NID_RESERVED1                     |  Reserved.
        ///     0x261  |  NID_SEARCH_DOMAIN_OBJECT          |  Global list of all Folder objects that are referenced by any Folder object's Search Criteria.
        ///     0x281  |  NID_SEARCH_GATHERER_QUEUE         |  Search Gatherer Queue (section 2.4.8.5.1).
        ///     0x2A1  |  NID_SEARCH_GATHERER_DESCRIPTOR    |  Search Gatherer Descriptor(section 2.4.8.5.2).
        ///     0x2E1  |  NID_RESERVED2                     |  Reserved.
        ///     0x301  |  NID_RESERVED3                     |  Reserved.
        ///     0x321  |  NID_SEARCH_GATHERER_FOLDER_QUEUE  |  Search Gatherer Folder Queue (section 2.4.8.5.3)
        ///     
        /// 
        /// </summary>
        public SpecialInternalNID SpecialInternalNID { get; set; }
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
        ulong nidType;
        /// <summary>
        /// nidIndex (27 bits): The identification portion of the NID.
        /// </summary>
        ulong nidIndex;
        public Nid(ulong nid)
        {
            this._Nid = nid;
            nidType = nid & 0x1f;//Lowest five bits
            nidIndex = nid >> 5;
            NidType = (NidType)(nid & 0x1f);//Lowest five bits
            SpecialInternalNID = (SpecialInternalNID)nid;
        }
    }
}
