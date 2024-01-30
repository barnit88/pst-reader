using Core.PST;
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
        string path = "C:\\Users\\Dell\\Workstation\\SoftwareDevelopment\\Dotnet\\NugetLibraries\\Personal\\PSTExtractionLibrary\\sample-pst\\source.pst";
        using (var memoryMappedFile = MemoryMappedFile.CreateFromFile(path, FileMode.Open))
        {
            var pst = new PST(memoryMappedFile);
        }

        
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
