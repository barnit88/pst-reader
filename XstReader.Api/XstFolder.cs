﻿// Project site: https://github.com/iluvadev/XstReader
//
// Based on the great work of Dijji. 
// Original project: https://github.com/dijji/XstReader
//
// Issues: https://github.com/iluvadev/XstReader/issues
// License (Ms-PL): https://github.com/iluvadev/XstReader/blob/master/license.md
//
// Copyright (c) 2021 iluvadev, and released under Ms-PL License.
// Copyright (c) 2016, Dijji, and released under Ms-PL.  This can be found in the root of this distribution. 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using XstReader.Common.BTrees;
using XstReader.ElementProperties;

namespace XstReader
{
    /// <summary>
    /// Class for a Folder stored inside an .ost or .pst file
    /// </summary>
    public class XstFolder : XstElement
    {
        /// <summary>
        /// The Container File
        /// </summary>
        [DisplayName("File")]
        [Category("General")]
        [Description("The Container File")]
        public override XstFile XstFile { get; }

        /// <summary>
        /// Number of messages inside the Folder
        /// </summary>
        [DisplayName("Content Count")]
        [Category(@"Folder Properties")]
        [Description(@"Specifies the number of rows under the header row.")]
        public int ContentCount => Properties[PropertyCanonicalName.PidTagContentCount]?.Value ?? (int)0;

        /// <summary>
        /// Number of unread messages inside the Folder
        /// </summary>
        [DisplayName("Content Unread Count")]
        [Category(@"Folder Properties")]
        [Description(@"Specifies the number of rows under the header row that have the PidTagRead property (section 2.878) set to FALSE.")]
        public int ContentUnreadCount => Properties[PropertyCanonicalName.PidTagContentUnreadCount]?.Value ?? (int)0;

        /// <summary>
        /// The Parent Folder of this Folder
        /// </summary>
        [DisplayName("Parent Folder")]
        [Category("General")]
        [Description(@"The Parent Folder of this Folder")]
        public XstFolder ParentFolder { get; set; }
        private IEnumerable<XstFolder> _Folders = null;

        /// <summary>
        /// The Folders contained inside this Folder
        /// </summary>
        [Browsable(false)]
        public IEnumerable<XstFolder> Folders => GetFolders();

        /// <summary>
        /// Returns if the current folder has folders inside
        /// </summary>
        [Obsolete("This property is Obsolete. Use Folders.Any() instead")]
        [Browsable(false)]
        public bool HasSubFolders => Folders.Any();

        /// <summary>
        /// Says if there are errors reading inner Folders
        /// </summary>
        [Category("Errors")]
        [Description(@"Says if there are errors reading inner Folders")]
        public bool HasErrorInFolders { get; set; } = false;

        /// <summary>
        /// The error reading inner Folders
        /// </summary>
        [Category("Errors")]
        [Description(@"The error reading inner Folders")]
        public string ErrorInFolders { get; set; } = "";


        /// <summary>
        /// The Parents of this Folder
        /// </summary>
        [Browsable(false)]
        public override XstElement Parent => (XstElement)ParentFolder ?? XstFile;

        /// <summary>
        /// The Name of the Element
        /// </summary>
        [DisplayName("Display Name")]
        [Category(@"Mapi Common")]
        [Description(@"Contains the display name of the folder.")]
        public override string DisplayName
        {
            get => string.IsNullOrEmpty(base.DisplayName) ? "Root" : base.DisplayName;
            protected set => base.DisplayName = value;
        }

        //private string _Path = null;
        ///// <summary>
        ///// The Path of this Folder
        ///// </summary>
        //[DisplayName("Path")]
        //[Category("General")]
        //[Description(@"The Path of this Folder")]
        //public override string Path => _Path ?? (_Path = string.IsNullOrEmpty(ParentFolder?.DisplayName) ? DisplayName : $"{ParentFolder.Path}\\{DisplayName}");

        private IEnumerable<XstMessage> _Messages = null;
        /// <summary>
        /// The Messages contained in the Folder
        /// </summary>
        [Browsable(false)]
        public IEnumerable<XstMessage> Messages => GetMessages();
        /// <summary>
        /// Says if there are errors reading inner Messages
        /// </summary>
        [Category("Errors")]
        [Description(@"Says if there are errors reading inner Messages")]
        public bool HasErrorInMessages { get; set; } = false;

        /// <summary>
        /// The error reading inner Messages
        /// </summary>
        [Category("Errors")]
        [Description(@"The error reading inner Messages")]
        public string ErrorInMessages { get; set; } = "";
        /// <summary>
        /// The unread Messages contained in the Folder
        /// </summary>
        [Obsolete("This property is Obsolete. Use Messages.Unread() instead")]
        [Browsable(false)]
        public IEnumerable<XstMessage> UnreadMessages => Messages.Where(m => !m.IsRead);
        private BTree<Node> _SubnodeTreeProperties = null;

        #region Ctor
        internal XstFolder(XstFile xstFile, NID nid, XstFolder parentFolder = null) : base(XstElementType.Folder)
        {
            XstFile = xstFile;
            Nid = nid;
            ParentFolder = parentFolder;
            _SubnodeTreeProperties = Ltp.ReadProperties(nid, Properties);
            //_SubnodeTreeProperties = Ltp.ReadProperties<XstFolder>(nid, PropertyGetters.FolderProperties, this);
        }
        #endregion Ctor

        #region Properties
        private protected override bool CheckProperty(PropertyCanonicalName tag)
            => Ltp.ContainsProperty(Nid, tag);
        private protected override XstProperty LoadProperty(PropertyCanonicalName tag)
            => Ltp.ReadProperty(Nid, tag);

        private protected override IEnumerable<XstProperty> LoadProperties()
            => Ltp.ReadAllProperties(Nid);

        #endregion Properties

        #region Folders
        /// <summary>
        /// Returns all the Folders contained inside this Folder
        /// </summary>
        /// <returns></returns>
        public IEnumerable<XstFolder> GetFolders()
        {
            if (_Folders == null)
                try
                {
                    _Folders = Ltp.ReadTableRowIds(NID.TypedNID(EnidType.HIERARCHY_TABLE, Nid))
                                  .Where(id => id.nidType == EnidType.NORMAL_FOLDER)
                                  .Select(id => new XstFolder(XstFile, id, this))
                                  .OrderBy(sf => sf.DisplayName);
                }
                catch(Exception ex) 
                {
                    _Folders = Enumerable.Empty<XstFolder>();
                    HasErrorInFolders = true;
                    ErrorInFolders = ex.Message;
                }
            return _Folders;
        }
        #endregion Folders

        #region Messages
        /// <summary>
        /// Returns all the Messages contained inside this Folder
        /// </summary>
        /// <returns></returns>
        public IEnumerable<XstMessage> GetMessages()
        {
            if (_Messages == null)
            {
                if (ContentCount > 0)
                    try
                    {
                        // Get the Contents table for the folder
                        // For 4K, not all the properties we want are available in the Contents table, so supplement them from the Message itself
                        _Messages = Ltp.ReadTable<XstMessage>(NID.TypedNID(EnidType.CONTENTS_TABLE, Nid),
                                                              (m, id) => m.Initialize(new NID(id), this),
                                                              m => m.ProcessSignedOrEncrypted());
                    }
                    catch(Exception ex)
                    {
                        _Messages = Enumerable.Empty<XstMessage>();
                        HasErrorInMessages = true;
                        ErrorInMessages = ex.Message;
                    }
                else
                    _Messages = Enumerable.Empty<XstMessage>();
            }
            return _Messages;
        }

        #endregion Messages

        private void ClearFolders()
        {
            if (_Folders != null)
                foreach (var folder in _Folders)
                    folder.ClearContentsInternal();
            _Folders = null;
            HasErrorInFolders = false;
            ErrorInFolders = "";
        }
        private void ClearMessages()
        {
            if (_Messages != null)
                foreach (var message in _Messages)
                    message.ClearContentsInternal();
            _Messages = null;
            HasErrorInMessages = false;
            ErrorInMessages = "";
        }
        internal override void ClearContentsInternal()
        {
            base.ClearContentsInternal();
            ClearFolders();
            ClearMessages();
        }

        /// <summary>
        /// Gets the String representation of the object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(DisplayName))
                return DisplayName.Trim();
            if (ParentFolder == null)
                return $"[Root] {XstFile}";
            return string.Empty;
        }
    }
}
