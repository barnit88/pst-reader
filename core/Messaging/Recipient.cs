using core.LTP.TableContext;
using System;
using System.Text;

namespace core.Messaging
{
    public class Recipient
    {

        public RecipientType Type;
        public ObjectType ObjType;
        public bool Responsibility;
        public byte[] Tag;
        public EntryId EntryID;
        public string DisplayName;
        public string EmailAddress;
        public string EmailAddressType;

        public Recipient(TCRowData row)
        {
            foreach (var exProp in row)
            {
                switch (exProp.ID)
                {
                    case 0x0c15:
                        this.Type = (RecipientType)BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    case 0x0e0f:
                        this.Responsibility = exProp.Data[0] == 0x01;
                        break;
                    case 0x0ff9:
                        this.Tag = exProp.Data;
                        break;
                    case 0x0ffe:
                        this.ObjType = (ObjectType)BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    case 0x0fff:
                        this.EntryID = new EntryId(exProp.Data);
                        break;
                    case 0x3001:
                        this.DisplayName = Encoding.Unicode.GetString(exProp.Data);
                        break;
                    case 0x3002:
                        this.EmailAddressType = Encoding.Unicode.GetString(exProp.Data);
                        break;
                    case 0x3003:
                        this.EmailAddress = Encoding.Unicode.GetString(exProp.Data);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}