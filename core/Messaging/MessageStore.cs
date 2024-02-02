using core.LTP.PropertyContext;
using core.NDBLayer.Pages.BTree;

namespace core.Messaging
{
    public class MessageStore
    {
        public PropertyContext PropertyContext { get; set; }
        public MessageStore(PST pst,SpecialInternalNId specialInternalNID)
        {
            ulong nid = (ulong)specialInternalNID;
            NodeBTreeEntry nodeBTreeEntry = pst.GetNodeBTreeEntryFromNid(nid, pst.NodeBTPage.BTPageEntries);
            BlockBTreeEntry dataBlock = pst.GetBlockBTreeEntryFromBid(nodeBTreeEntry.bidData,pst.BlockBTPage.BTPageEntries);
            BlockBTreeEntry subNodeDataBlock = null;
            if(nodeBTreeEntry.bidSub != 0)
                subNodeDataBlock = pst.GetBlockBTreeEntryFromBid(nodeBTreeEntry.bidSub, pst.BlockBTPage.BTPageEntries);
            //this.PropertyContext = new PropertyContext();
        }
    }
}