using core.LTP.PropertyContext;
using core.NDBLayer;
using System.Text;

namespace core.Messaging
{
    public class IPMItem
    {
        public NodeDataDTO node { get; set; }
        public string MessageClass { get; set; }
        public PropertyContext PC { get; set; }
        public IPMItem(NodeDataDTO node)
        {
            this.node = node;
            this.PC = new PropertyContext(node);
            this.MessageClass = Encoding.Unicode.GetString(this.PC.Properties[0x1a].Data);
        }
    }
}
