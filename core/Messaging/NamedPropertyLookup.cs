using core.LTP.PropertyContext;
using core.NDBLayer;
using core.NDBLayer.Pages.BTree;
using System.Collections.Generic;

namespace core.Messaging
{
    /// <summary>
    /// Named Property Lookup Map
    /// The mapping between NPIDs and property names is done using a special Name-to-ID-Map in the PST,
    /// with a special NID of NID_NAME_TO_ID_MAP(0x61). There is one Name-to-ID-Map per PST.From an
    /// implementation point of view, the Name-to-ID-Map is a standard PC with some special properties.
    /// Specifically, the properties in the PC do not refer to real property identifiers, but instead point to
    /// specific data sections of the Name-to-ID-Map.
    /// A named property is identified by a(GUID, identifier) value pair, otherwise known as the property
    /// name.The identifier can be a string or a 16-bit numerical value.The GUID value identifies the
    /// property set to which the property name is associated.Well-known property names and a list of
    /// property set GUIDs are specified in [MS-OXPROPS].
    /// The Name-to-ID-Map(NPMAP) consists of several components: an Entry Stream, a GUID Stream, a
    /// String Stream, and a hash table to expedite searching.The following are the data structures used for 
    /// the NPMAP
    /// </summary>
    public class NamedPropertyLookup
    {
        public static ulong NODE_ID = 0x61;

        public PropertyContext PC;
        public Dictionary<ushort, NameId> Lookup;

        internal byte[] _GUIDs;
        internal byte[] _entries;
        internal byte[] _string;
        public NamedPropertyLookup(List<IBTPageEntry> nodeBTPageEntries, List<IBTPageEntry> blockBTPageEntries)
        {
            NodeBTreeEntry nodeBTreeEntry = NDB.GetNodeBTreeEntryFromNid(NamedPropertyLookup.NODE_ID, nodeBTPageEntries);
            NodeDataDTO node = NDB.GetNodeDataFromNodeBTreeEntry(nodeBTreeEntry, blockBTPageEntries);
            this.PC = new PropertyContext(node);
            this._GUIDs = this.PC.Properties[0x0002].Data;
            this._entries = this.PC.Properties[0x0003].Data;
            this._string = this.PC.Properties[0x0004].Data;
            this.Lookup = new Dictionary<ushort, NameId>();
            for (int i = 0; i < this._entries.Length; i += 8)
            {
                var cur = new NameId(this._entries, i, this);
                this.Lookup.Add(cur.wPropIdx, cur);
            }
        }
    }
}
