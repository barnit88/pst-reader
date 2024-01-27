using System.ComponentModel;

namespace Core.PST.ID
{
    /// <summary>
    /// Types of Bid
    /// </summary>
    //public enum BidType
    //{
    //    /// <summary>
    //    /// Points to a block
    //    /// </summary>
    //    BlockBid,
    //    /// <summary>
    //    /// Points to a Page
    //    /// </summary>
    //    PageBid
    //}
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
}