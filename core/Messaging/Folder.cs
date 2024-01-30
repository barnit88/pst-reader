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
    internal class Folder
    {
    }
}
