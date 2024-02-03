using core;
using core.Messaging;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;

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
        var sw = new Stopwatch();
        sw.Start();
        string path = "C:\\Users\\Dell\\Workstation\\SoftwareDevelopment\\Dotnet\\NugetLibraries\\Personal\\PSTExtractionLibrary\\sample-pst\\source.pst";
        using (var memoryMappedFile = MemoryMappedFile.CreateFromFile(path, FileMode.Open))
        {
            var pst = new PST(memoryMappedFile);
            var message = new MessageStore(pst, SpecialInternalNId.NID_MESSAGE_STORE);
            var rootFolder = new Folder(message.RootFolderEntryId.Nid, new List<string>()
                , pst.NodeBTPage.BTPageEntries, pst.BlockBTPage.BTPageEntries);
            var namedPropertyLookup = new NamedPropertyLookup(pst.NodeBTPage.BTPageEntries, pst.BlockBTPage.BTPageEntries);



            var stack = new Stack<Folder>();
            stack.Push(rootFolder);
            var totalCount = 0;

            while (stack.Count > 0)
            {
                var curFolder = stack.Pop();
                if (curFolder.SubFolders != null && curFolder.SubFolders.Count != 0)
                    foreach (var child in curFolder.SubFolders)
                        stack.Push(child);
                var count = curFolder.ContentsTable.RowIndexBTH.Properties.Count;
                totalCount += count;
                foreach (var ipmItem in curFolder)
                {
                    if (ipmItem is MessageObject)
                    {
                        var messageObject = ipmItem as MessageObject;
                        Console.WriteLine(messageObject.Subject);
                        Console.WriteLine(messageObject.Imporance);
                        Console.WriteLine("Sender Name: " + messageObject.SenderName);
                        if (messageObject.From.Count > 0)
                            Console.WriteLine("From: {0}",
                                              String.Join("; ", messageObject.From.Select(r => r.EmailAddress)));
                        if (messageObject.To.Count > 0)
                            Console.WriteLine("To: {0}",
                                              String.Join("; ", messageObject.To.Select(r => r.EmailAddress)));
                        if (messageObject.CC.Count > 0)
                            Console.WriteLine("CC: {0}",
                                              String.Join("; ", messageObject.CC.Select(r => r.EmailAddress)));
                        if (messageObject.BCC.Count > 0)
                            Console.WriteLine("BCC: {0}",
                                              String.Join("; ", messageObject.BCC.Select(r => r.EmailAddress)));
                    }
                }
            }
        }
        sw.Stop();
        Console.WriteLine("{0} messages total"+ sw.Elapsed);
        Console.WriteLine("Elapsed Milisecod  "+ sw.ElapsedMilliseconds);
    }
}
