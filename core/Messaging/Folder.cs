using core.LTP.PropertyContext;
using core.LTP.TableContext;
using core.NDBLayer;
using core.NDBLayer.ID;
using core.NDBLayer.Pages.BTree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace core.Messaging
{
    /// <summary>
    /// Folders
    /// 
    /// Folder objects are hierarchical containers that are used to create a storage hierarchy for the
    /// message store.In the PST architecture, a single root Folder object exists at the top of the message
    /// store, from which an arbitrarily complex hierarchy of Folder objects descends to provide structured
    /// storage for all the Messaging objects.
    /// 
    /// At the LTP level, 
    /// a Folder object is a composite entity that is represented using four LTP constructs.
    /// Specifically, each Folder object consists of :
    /// one PC, which contains the properties directly associated with the Folder object, 
    /// and three TCs :
    /// for information about the contents, hierarchy and other associated information of the Folder object.
    /// 
    /// Some Folder objects MAY have additional nodes that pertain to Search, which is discussed 
    /// in section 2.4.8.6. 
    /// 
    /// At the NDB level, the 4 LTP constructs are persisted as 4 separate top-level nodes (that is, 4 different
    /// NIDs). For identification purposes, the nidIndex portion for each of the NIDs is the same to indicate
    /// that these nodes collectively make up a Folder object. However, each of the 4 NIDs has a different
    /// nidType value to differentiate their respective function.The following diagram indicates the
    /// relationships among these elements.
    /// 
    /// </summary>
    public class Folder : IEnumerable<IPMItem>
    {
        public PropertyContext PropertyContext { get; set; }
        public TableContext HierarchyTable { get; set; }
        public TableContext ContentsTable { get; set; }
        public TableContext AssociatedContentsTable { get; set; }
        public List<Folder> SubFolders { get; set; }
        public string DisplayName { get; set; }
        public List<string> Path { get; set; }
        private List<IBTPageEntry> NodeBTPageEntries;
        private List<IBTPageEntry> BlockBTPageEntries;
        public Folder(Nid nid, List<string> path, List<IBTPageEntry> nodeBTPageEntries, List<IBTPageEntry> blockBTPageEntries)
        {
            this.NodeBTPageEntries = nodeBTPageEntries;
            this.BlockBTPageEntries = blockBTPageEntries;
            var pcNid = ((nid._Nid >> 5) << 5) | 0x02;
            var hirearchyTableNid = ((nid._Nid >> 5) << 5) | 0x0D;
            var contentsTableNid = ((nid._Nid >> 5) << 5) | 0x0E;
            var associatedContentsTableNid = ((nid._Nid >> 5) << 5) | 0x0F;

            this.PropertyContext = new PropertyContext(GetNodeDataFromNid(pcNid, nodeBTPageEntries, blockBTPageEntries));
            this.DisplayName = Encoding.Unicode.GetString(this.PropertyContext.Properties[(ushort)FolderProperty.PidTagDisplayName].Data);
            this.Path = new List<string>(path);
            this.Path.Add(DisplayName);
            this.HierarchyTable = new TableContext(GetNodeDataFromNid(hirearchyTableNid, nodeBTPageEntries, blockBTPageEntries));
            var hasSubFolder = BitConverter.ToBoolean(this.PropertyContext.Properties[(ushort)FolderProperty.PidTagSubfolders].Data);
            if (hasSubFolder && this.HierarchyTable.ReverseRowIndex.Count > 0)
            {
                this.SubFolders = new List<Folder>();
                foreach (var row in this.HierarchyTable.ReverseRowIndex)
                {
                    this.SubFolders.Add(new Folder(new Nid(row.Value), this.Path, nodeBTPageEntries, blockBTPageEntries));
                }
            }
            this.ContentsTable = new TableContext(GetNodeDataFromNid(contentsTableNid, nodeBTPageEntries, blockBTPageEntries));
            this.AssociatedContentsTable = new TableContext(GetNodeDataFromNid(associatedContentsTableNid, nodeBTPageEntries, blockBTPageEntries));
        }
        public NodeDataDTO GetNodeDataFromNid(ulong nid, List<IBTPageEntry> nodeBTPageEntries, List<IBTPageEntry> blockBTPageEntries)
        {
            NodeBTreeEntry nodeBTreeEntry = NDB.GetNodeBTreeEntryFromNid(nid, nodeBTPageEntries);
            NodeDataDTO nodeData = NDB.GetNodeDataFromNodeBTreeEntry(nodeBTreeEntry, blockBTPageEntries);
            return nodeData;
        }

        public IEnumerator<IPMItem> GetEnumerator()
        {
            foreach (var row in this.ContentsTable.ReverseRowIndex)
            {
                NodeDataDTO node = GetNodeDataFromNid(row.Value, this.NodeBTPageEntries, this.BlockBTPageEntries);
                var curItem = new IPMItem(node);
                yield return new MessageObject(curItem, node);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
