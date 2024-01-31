using XstReader;

namespace PSTExtractor;
//position : byte from where the reading will start
//arrray : where the readed byte[] will be stored
//offset : from which position in the output array the data should be written
//length : length of the data to be read starting from position
//view.ReadArray<byte>(4, dwCRCPartial, 0, 4);
//vMagicClient
//view.ReadArray<byte>(position, array, offset, length);

class Program
{
    public static void Main(string[] args)
    {
        string path = "C:\\Users\\Dell\\Workstation\\SoftwareDevelopment\\Dotnet\\NugetLibraries\\Personal\\PSTExtractionLibrary\\sample-pst\\source.pst";
        //using (var memoryMappedFile = MemoryMappedFile.CreateFromFile(path, FileMode.Open))
        //{
        //    var pst = new PST(memoryMappedFile);
        //    var message = new MessageStore(pst,SpecialInternalNId.NID_MESSAGE_STORE);
        //}
        OpenOstOrPstFile(path);
    }
    
    public static void OpenOstOrPstFile(string fileName)
    {
        // Note the "using": XstFile implements IDisposable
        // The file remains opened until dispose of XstFile
        using (var xstFile = new XstFile(fileName))
        {
            ProcessFolder(xstFile.RootFolder);
        }
    }

    //#### Processing a Folder
    public static void ProcessFolder(XstFolder folder)
    {
        // We can process the properties of the Folder
        var properties = folder.Properties;

        // Messages in the folder
        foreach (var message in folder.Messages)
        {
            ProcessMessage(message);
        }

        // Folders inside the folder
        foreach (var childFolder in folder.Folders)
        {
            ProcessFolder(childFolder);
        }
    }

    //#### Processing a Message
    public static void ProcessMessage(XstMessage message)
    {
        // We can process the properties of the Message
        var properties = message.Properties;

        // Recipients of the Message
        ProcessRecipients(message.Recipients);

        // Body of the Message
        ProcessBody(message.Body);

        // Attachments in the message
        foreach (var attachment in message.Attachments)
        {
            ProcessAttachment(attachment);
        }
    }

    //#### Processing Recipients

    public static void ProcessRecipients(XstRecipientSet recipients)
    {
        // We have info about recipients involved in a Message:
        XstRecipient originator = recipients.Originator;
        XstRecipient originalSentRepresenting = recipients.OriginalSentRepresenting;
        XstRecipient sentRepresenting = recipients.SentRepresenting;
        XstRecipient sender = recipients.Sender;
        IEnumerable<XstRecipient> to = recipients.To;
        IEnumerable<XstRecipient> cc = recipients.Cc;
        IEnumerable<XstRecipient> bcc = recipients.Bcc;
        XstRecipient receivedBy = recipients.ReceivedBy;
        XstRecipient receivedRepresenting = recipients.ReceivedRepresenting;

        // All Recipients with its own properties:
        var senderProperties = sender.Properties;
    }

    //#### Processing Body
    public static void ProcessBody(XstMessageBody body)
    {
        switch (body.Format)
        {
            case XstMessageBodyFormat.Html:
                Console.Write("body in html"); break;
            case XstMessageBodyFormat.Rtf:
                Console.Write("body in rtf"); break;
            case XstMessageBodyFormat.PlainText:
                Console.Write("body in txt"); break;
        }

        // The Body in the format can be accessed by text or bytearray
        var text = body.Text;
        var bytes = body.Bytes;
    }
    //#### Processing Attachment
    public static void ProcessAttachment(XstAttachment attachment)
    {
        string fileName = "myAttachment";

        // We can process the properties of the Attachment
        var properties = attachment.Properties;

        // We can open attached Messages
        if (attachment.IsEmail)
            ProcessMessage(attachment.AttachedEmailMessage);
        // We can save attachments
        else if (attachment.IsFile)
            attachment.SaveToFile(fileName, attachment.LastModificationTime);
    }
}

//There are more NID Types
public enum rgnidType : int //(NID_TYPE)
{
    //NID_TYPE = Starting nidIndex
    NID_TYPE_NORMAL_FOLDER = 1024,//Hex 0x400
    NID_TYPE_SEARCH_FOLDER = 16384,//Hex 0x4000
    NID_TYPE_NORMAL_MESSAGE = 65536,//Hex 0x10000
    NIDE_TYPE_ASSOC_MESSAGE = 32768//Hex 0x8000
}
