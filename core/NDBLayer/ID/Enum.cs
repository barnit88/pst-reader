using System.ComponentModel;

namespace core.NDBLayer.ID
{
    /// <summary>
    /// MUST set to 1 when the block is "Internal", or zero when the block is not "Internal". An 
    /// internal block is an intermediate block that, instead of containing actual data, contains metadata
    /// about how to locate other data blocks that contain the desired information.For more details about
    /// technical details regarding blocks, see section 2.2.2.8.
    /// </summary>
    public enum ExternalOrInternalBid
    {
        /// <summary>
        /// 2nd least significant bit of Bid(indicated by Bi) must be 1 when the block is internal. An 
        /// internal block is an intermediate block that, instead of containing actual data, 
        /// contains metadata about how to locate other data blocks 
        /// that contain the desired information.
        /// </summary>
        Internal,
        /// <summary>
        /// 2nd least significant bit of Bid(indicated by Bi) must be 0 when the block is External.
        /// Contains the data(Data Block)
        /// </summary>
        External
    }
    /// <summary>
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
    /// </summary>
    public enum NidType : byte
    {
        [Description("Heap node")]
        NID_TYPE_HID = 0x00,
        [Description("Internal node")]
        NID_TYPE_INTERNAL = 0x01,
        [Description("Normal Folder object (PC)")]
        NID_TYPE_NORMAL_FOLDER = 0x02,
        [Description("Search Folder object (PC)")]
        NID_TYPE_SEARCH_FOLDER = 0x03,
        [Description("Normal Message object (PC)")]
        NID_TYPE_NORMAL_MESSAGE = 0x04,
        [Description("Attachment object (PC)")]
        NID_TYPE_ATTACHMENT = 0x05,
        [Description("Queue of changed objects for search Folder objects")]
        NID_TYPE_SEARCH_UPDATE_QUEUE = 0x06,
        [Description("Defines the search criteria for a search Folder object")]
        NID_TYPE_SEARCH_CRITERIA_OBJECT = 0x07,
        [Description("Folder associated information (FAI) Message object (PC)")]
        NID_TYPE_ASSOC_MESSAGE = 0x08,
        [Description("Internal, persisted view-related")]
        NID_TYPE_CONTENTS_TABLE_INDEX = 0x0A,
        [Description("Receive Folder object (Inbox)")]
        NID_TYPE_RECEIVE_FOLDER_TABLE = 0X0B,
        [Description("Outbound queue (Outbox)")]
        NID_TYPE_OUTGOING_QUEUE_TABLE = 0x0C,
        [Description("Hierarchy table (TC)")]
        NID_TYPE_HIERARCHY_TABLE = 0x0D,
        [Description("Contents table (TC)")]
        NID_TYPE_CONTENTS_TABLE = 0x0E,
        [Description("FAI contents table (TC)")]
        NID_TYPE_ASSOC_CONTENTS_TABLE = 0x0F,
        [Description("Contents table (TC) of a search Folder object")]
        NID_TYPE_SEARCH_CONTENTS_TABLE = 0x10,
        [Description("Attachment table (TC)")]
        NID_TYPE_ATTACHMENT_TABLE = 0x11,
        [Description("Recipient table (TC)")]
        NID_TYPE_RECIPIENT_TABLE = 0x12,
        [Description("Internal, persisted view-related")]
        NID_TYPE_SEARCH_TABLE_INDEX = 0x13,
        [Description("LTP")]
        NID_TYPE_LTP = 0x1F
    }
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
    public enum SpecialInternalNID
    {
        [Description("Message store node(section 2.4.3)")]
        NID_MESSAGE_STORE = 0x21,
        [Description("Named Properties Map(section 2.4.7)")]
        NID_NAME_TO_ID_MAP = 0x61,
        [Description("Special template node for an empty Folder object")]
        NID_NORMAL_FOLDER_TEMPLATE = 0xA1,
        [Description("Special template node for an empty search Folder object")]
        NID_SEARCH_FOLDER_TEMPLATE = 0xC1,
        [Description("Root Mailbox Folder object of PST")]
        NID_ROOT_FOLDER = 0x122,
        [Description("Queue of Pending Search-related updates")]
        NID_SEARCH_MANAGEMENT_QUEUE = 0x1E1,
        [Description("Folder object NIDs with active Search activity")]
        NID_SEARCH_ACTIVITY_LIST = 0x201,
        [Description("Reserved")]
        NID_RESERVED1 = 0x241,
        [Description("Global list of all Folder objects that are referenced by any Folder object's Search Criteria")]
        NID_SEARCH_DOMAIN_OBJECT = 0x261,
        [Description("Search Gatherer Queue ")]
        NID_SEARCH_GATHERER_QUEUE = 0x281,
        [Description("Search Gatherer Descriptor")]
        NID_SEARCH_GATHERER_DESCRIPTOR = 0x2A1,
        [Description("Reserved")]
        NID_RESERVED2 = 0x2E1,
        [Description("Reserved")]
        NID_RESERVED3 = 0x301,
        [Description("Search Gatherer Folder Queue")]
        NID_SEARCH_GATHERER_FOLDER_QUEUE = 0x321
    }
}