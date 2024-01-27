using System;

namespace Core.PST.Pages.Base
{
    /// <summary>
    /// A page is a fixed-size structure of 512 bytes that is used in the NDB Layer 
    /// to represent allocation metadata and BTree data structures.
    /// A page trailer is placed at the very end of every page such that
    /// the end of the page trailer is aligned with the end of the page.
    /// 
    /// A PAGE has size of 512-bytes. 
    /// </summary>
    public class Page
    {
        /// <summary>
        /// Length of a BTreePage in bytes(512bytes) 
        /// A Page has 512-byte of data.
        /// </summary>
        public virtual Int16 PageSize { get; set; } = 512;//512 bytes of data
        /// <summary>
        /// A PAGETRAILER structure contains information about the page in which it is contained. 
        /// PAGETRAILER structure is present at the very end of each page in a PST file.
        /// </summary>
        public PageTrailer PageTrailer { get; set; }
    }
}
