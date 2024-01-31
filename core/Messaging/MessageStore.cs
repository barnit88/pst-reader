using core.LTP.PropertyContext;
using Core.PST;
using System.IO.MemoryMappedFiles;

namespace core.Messaging
{
    public class MessageStore
    {
        public PropertyContext PropertyContext { get; set; }
        public MessageStore(MemoryMappedFile file,PST pst) {

            this.PropertyContext = new PropertyContext();
        }
    }
}