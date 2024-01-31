﻿// Project site: https://github.com/iluvadev/XstReader
//
// Based on the great work of Dijji. 
// Original project: https://github.com/dijji/XstReader
//
// Issues: https://github.com/iluvadev/XstReader/issues
// License (Ms-PL): https://github.com/iluvadev/XstReader/blob/master/license.md
//
// Copyright (c) 2021, iluvadev, and released under Ms-PL License.
// Copyright (c) 2016, Dijji, and released under Ms-PL.  This can be found in the root of this distribution. 

using System;
using System.Collections.Generic;
using System.Text;

namespace XstReader
{
    /// <summary>
    /// Available formats for the Body of a Message
    /// </summary>
    public enum XstMessageBodyFormat
    {
        /// <summary>
        /// Unknown format
        /// </summary>
        Unknown,

        /// <summary>
        /// Plain Text, txt
        /// </summary>
        PlainText,

        /// <summary>
        /// Html
        /// </summary>
        Html,

        /// <summary>
        /// Rich Text, Rtf
        /// </summary>
        Rtf,
    }
}
