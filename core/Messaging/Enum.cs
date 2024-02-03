using System;
using System.ComponentModel;
using System.Threading;
using System.Xml.Linq;

namespace core.Messaging
{
    /// <summary>
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
    /// 
    ///     Special Internal NId's Table
    ///        Value  |  Friendly name                     |  Meaning
    ///        0x21   |  NID_MESSAGE_STORE                 |  Message store node (section 2.4.3).
    ///        0x61   |  NID_NAME_TO_ID_MAP                |  Named Properties Map (section 2.4.7).
    ///        0xA1   |  NID_NORMAL_FOLDER_TEMPLATE        |  Special template node for an empty Folder object.
    ///        0xC1   |  NID_SEARCH_FOLDER_TEMPLATE        |  Special template node for an empty search Folder object.
    ///        0x122  |  NID_ROOT_FOLDER                   |  Root Mailbox Folder object of PST.
    ///        0x1E1  |  NID_SEARCH_MANAGEMENT_QUEUE       |  Queue of Pending Search-related updates.
    ///        0x201  |  NID_SEARCH_ACTIVITY_LIST          |  Folder object NIDs with active Search activity.
    ///        0x241  |  NID_RESERVED1                     |  Reserved.
    ///        0x261  |  NID_SEARCH_DOMAIN_OBJECT          |  Global list of all Folder objects that are referenced by any Folder object's Search Criteria.
    ///        0x281  |  NID_SEARCH_GATHERER_QUEUE         |  Search Gatherer Queue (section 2.4.8.5.1).
    ///        0x2A1  |  NID_SEARCH_GATHERER_DESCRIPTOR    |  Search Gatherer Descriptor (section 2.4.8.5.2).
    ///        0x2E1  |  NID_RESERVED2                     |  Reserved.
    ///        0x301  |  NID_RESERVED3                     |  Reserved.
    ///        0x321  |  NID_SEARCH_GATHERER_FOLDER_QUEUE  |  Search Gatherer Folder Queue (section 2.4.8.5.3)
    /// 
    /// </summary>
    public enum SpecialInternalNId : UInt32
    {
        [Description("Message store node")]
        NID_MESSAGE_STORE = 0x21,
        [Description("Named Properties Map")]
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
        [Description("Search Gatherer Queue")]
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
    /// <summary>
    /// 
    ///     Property   | Property       |                           |
    ///     identifier | type           | Friendly name             |  Description
    ///     0x3001     | PtypString     | PidTagDisplayName         |  Display name of the Folder object.
    ///     0x3602     | PtypInteger32  | PidTagContentCount        |  Total number of items in the Folder object.
    ///     0x3603     | PtypInteger32  | PidTagContentUnreadCount  |  Number of unread items in the Folder object.
    ///     0x360A     | PtypBoolean    | PidTagSubfolders          |  Whether the Folder object has any sub-Folder objects.
    ///     
    /// </summary>
    public enum FolderProperty : ushort
    {
        [Description("Display name of the Folder object. PtypString")]
        PidTagDisplayName = 0x3001,
        [Description("Total number of items in the Folder object. PtypInteger32")]
        PidTagContentCount = 0x3602,
        [Description("Number of unread items in the Folder object. PtypInteger32")]
        PidTagContentUnreadCount = 0x3603,
        [Description("Whether the Folder object has any sub-Folder objects. PtypBoolean")]
        PidTagSubfolders = 0x360A,
    }
}
