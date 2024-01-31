﻿// Project site: https://github.com/iluvadev/XstReader
//
// Based on the great work of Dijji. 
// Original project: https://github.com/dijji/XstReader
//
// Issues: https://github.com/iluvadev/XstReader/issues
// License (Ms-PL): https://github.com/iluvadev/XstReader/blob/master/license.md
//
// Copyright (c) 2021, iluvadev, and released under Ms-PL License.

using System.IO;

namespace XstReader
{
    /// <summary>
    /// Class representing the Body of a Message
    /// </summary>
    public class XstMessageBody
    {
        /// <summary>
        /// The Content Message
        /// </summary>
        private XstMessage Message { get; set; }

        /// <summary>
        /// The Format of the Body
        /// </summary>
        public XstMessageBodyFormat Format { get; private set; }

        /// <summary>
        /// The Text of the Body, in the Format defined in the instance
        /// </summary>
        public string Text { get; internal set; }

        private byte[] _Bytes = null;
        /// <summary>
        /// Array of Bytes of the Body, in the Format defined in the instance
        /// </summary>
        public byte[] Bytes => _Bytes ?? (_Bytes = Message?.Encoding?.GetBytes(Text));

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="text"></param>
        /// <param name="format"></param>
        public XstMessageBody(XstMessage message, string text, XstMessageBodyFormat format)
        {
            Message = message;
            Text = text;
            Format = format;
        }
    }
}
