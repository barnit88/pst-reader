namespace core.NDB.Blocks
{
    /// <summary>
    /// Several types of blocks are defined at the NDB Layer. The following table defines the block type mapping
    /// 
    /// 
    /// 
    ///         Block type          | Data structure   |   Internal BID? |  Header level |  Array content
    ///         Data Tree           | Data block       |   No            |  N/A          |  Bytes
    ///                             | XBLOCK           |   Yes           |  1            |  Data block reference
    ///                             | XXBLOCK          |   Yes           |  2            |  XBLOCK reference
    ///         Subnode BTree data  | SLBLOCK          |   Yes           |  0            |  SLENTRY
    ///                             | SIBLOCK          |   Yes           |  1            |  SIENTRY
    /// 
    /// 
    /// </summary>
    public enum BlockType
    {
        DATABLOCK,
        XBLOCK,
        XXBLOCK,
        SLBLOCK,
        SIBLOCK
    }
}
