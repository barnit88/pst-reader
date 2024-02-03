using core.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace core.LTP.PropertyContext
{
    /// <summary>
    /// The following properties MUST be present in any valid message store PC.
    /// 
    ///     Property identifier  |   Property type | Friendly name                 |  Description
    ///     0x0FF9               |   PtypBinary    | PidTagRecordKey               |  Record key.This is the Provider UID of this PST.
    ///     0x3001               |   PtypString    | PidTagDisplayName             |  Display name of PST
    ///     0x35E0               |   PtypBinary    | PidTagIpmSubTreeEntryId       |  EntryID of the Root Mailbox Folder object
    ///     0x35E3               |   PtypBinary    | PidTagIpmWastebasketEntryId   |  EntryID of the Deleted Items Folder object
    ///     0x35E7               |   PtypBinary    | PidTagFinderEntryId           |  EntryID of the search Folder object
    /// 
    /// </summary>
    public enum RequireProperties : ushort
    {
        [Description("Record key.This is the Provider UID of this PST.")]
        PidTagRecordKey = 0x0FF9,
        [Description("Display name of PST")]
        PidTagDisplayName = 0x3001,
        [Description("EntryID of the Root Mailbox Folder object")]
        PidTagIpmSubTreeEntryId = 0x35E0,
        [Description("EntryID of the Deleted Items Folder object")]
        PidTagIpmWastebasketEntryId = 0x35E3,
        [Description("EntryID of the search Folder object")]
        PidTagFinderEntryId = 0x35E7
    }
}
