using core.LTP.PropertyContext;
using core.NDBLayer;
using core.NDBLayer.Pages.BTree;

namespace core.Messaging
{
    public class MessageStore
    {
        public EntryId RootFolderEntryId { get; set; }
        public PropertyContext PropertyContext { get; set; }
        public MessageStore(PST pst, SpecialInternalNId specialInternalNID)
        {
            ulong nid = (ulong)specialInternalNID;
            NodeBTreeEntry nodeBTreeEntry = NDB.GetNodeBTreeEntryFromNid(nid, pst.NodeBTPage.BTPageEntries);
            BlockBTreeEntry mainNodeBlockBTreeEntry = NDB.GetBlockBTreeEntryFromBid
                                                        (nodeBTreeEntry.bidData, pst.BlockBTPage.BTPageEntries);
            NodeDataDTO node = NDB.GetNodeDataFromNodeBTreeEntry
                                        (nodeBTreeEntry, pst.BlockBTPage.BTPageEntries);
            BlockBTreeEntry subNodeBlockBTreeEntry = null;
            //if (nodeBTreeEntry.bidSub != 0)
            //{
            //    subNodeBlockBTreeEntry = NDB.GetBlockBTreeEntryFromBid(nodeBTreeEntry.bidSub, pst.BlockBTPage.BTPageEntries);
            //    node.SubNodeData = NDB.GetSubNodesDataBlockFromSubNodeBlockBTreeEntry(subNodeBlockBTreeEntry, pst.BlockBTPage.BTPageEntries);
            //}

            this.PropertyContext = new PropertyContext(node);
            this.RootFolderEntryId = new EntryId(
                this.PropertyContext.Properties[(ushort)RequireProperties.PidTagIpmSubTreeEntryId].Data);
        }
    }
}