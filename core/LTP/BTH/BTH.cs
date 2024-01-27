namespace core.LTP.BTH
{
    /// <summary>
    /// A BTree-on-Heap implements a classic BTree on a heap node. A BTH consists of several parts: 
    /// header, the BTree records, and optional BTree data.The following diagram shows a high-level
    /// schematic of a BTH.
    /// 
    /// 
    /// The preceding diagram shows a BTH with two levels of indices. The top-level index (Key, HID) value 
    /// pairs actually point to heap items that contain the Level 1 Indices, which, in turn, point to heap items
    /// that contain the leaf(Key, data) value pairs.Each of the six boxes in the diagram actually represents
    /// six separate items allocated out of the same HN, as indicated by their associated HIDs.
    /// 
    /// </summary>
    public class BTH
    {

    }
}
