using System;

namespace core.LTP.PropertyContext
{
    /// <summary>
    /// 
    /// PtypObject Properties
    /// When a property of type PtypObject is stored in a PC, the dwValueHnid value described in section 
    /// 2.3.3.3 points to a heap allocation that contains a structure that defines the size and location of the
    /// object data.
    /// </summary>
    public class PTypObjectProperties
    {
        /// <summary>
        /// Nid (4 bytes): The subnode identifier that contains the object data.
        /// </summary>
        public UInt32 Nid { get; set; }
        /// <summary>
        /// ulSize (4 bytes): The total size of the object.
        /// </summary>
        public UInt32 ulSize { get; set; }
    }
}
