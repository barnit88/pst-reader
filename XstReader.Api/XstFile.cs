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
using System.ComponentModel;
using System.IO;
using XstReader.ElementProperties;

namespace XstReader
{
    // See: https://docs.microsoft.com/en-us/openspecs/exchange_server_protocols/ms-oxcmsg/7fd7ec40-deec-4c06-9493-1bc06b349682


    // The code here implements the messaging layer, which depends on and invokes the NDP and LTP layers

    /// <summary>
    /// Main handling for xst (.ost and .pst) files 
    /// </summary>
    public class XstFile : XstElement, IDisposable
    {
        private NDB _Ndb;
        internal new NDB Ndb => _Ndb ?? (_Ndb = new NDB(this));

        private LTP _Ltp;
        internal new LTP Ltp => _Ltp ?? (_Ltp = new LTP(Ndb));

        private string _FileName = null;
        /// <summary>
        /// FileName of the .pst or .ost file to read
        /// </summary>
        public string FileName { get => _FileName; set => SetFileName(value); }
        private void SetFileName(string fileName)
        {
            _FileName = fileName;
            ClearContents();
        }

        private FileStream _ReadStream = null;
        internal FileStream ReadStream
        {
            get => _ReadStream ?? (_ReadStream = new FileStream(FileName, FileMode.Open, FileAccess.Read));
        }
        internal object StreamLock { get; } = new object();

        private XstFolder _RootFolder = null;
        /// <summary>
        /// The Root Folder of the XstFile. (Loaded when needed)
        /// </summary>
        public XstFolder RootFolder => _RootFolder ?? (_RootFolder = new XstFolder(this, new NID(EnidSpecial.NID_ROOT_FOLDER)));

        /// <summary>
        /// The Path of this Element
        /// </summary>
        [DisplayName("Path")]
        [Category("General")]
        [Description(@"The Path of this Element")]
        public override string Path => System.IO.Path.GetFileName(this.FileName);

        /// <summary>
        /// The Parents of this Element
        /// </summary>
        [Browsable(false)]
        public override XstElement Parent => null;


        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="fileName">The .pst or .ost file to open</param>
        public XstFile(string fileName) : base(XstElementType.File)
        {
            FileName = fileName;
        }
        #endregion Ctor

        private void ClearStream()
        {
            if (_ReadStream != null)
            {
                _ReadStream.Close();
                _ReadStream.Dispose();
                _ReadStream = null;
            }
        }

        /// <summary>
        /// Clears information and memory used in RootFolder
        /// </summary>
        private void ClearRootFolder()
        {
            if (_RootFolder != null)
            {
                _RootFolder.ClearContents();
                _RootFolder = null;
            }
        }

        /// <summary>
        /// Clears all information and memory used by the object
        /// </summary>
        public override void ClearContents()
        {
            ClearStream();
            ClearRootFolder();

            _Ndb = null;
            _Ltp = null;
        }

        /// <summary>
        /// Disposes memory used by the object
        /// </summary>
        public void Dispose()
        {
            ClearContents();
        }

        /// <summary>
        /// Gets the String representation of the object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return System.IO.Path.GetFileName(FileName ?? "");
        }

        private protected override IEnumerable<XstProperty> LoadProperties()
        {
            return new XstProperty[0];
        }

        private protected override XstProperty LoadProperty(PropertyCanonicalName tag)
        {
            return null;
        }

        private protected override bool CheckProperty(PropertyCanonicalName tag)
        {
            return false;
        }
    }
}
