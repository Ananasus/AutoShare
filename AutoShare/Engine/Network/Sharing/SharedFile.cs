using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoShare.Engine.Network.Sharing
{
    class SharedFile
    {
        #region Private members
        string _owner_name, _filename, _checksum, _cert;
        int _filesize;

        string _local_filename;
        System.IO.FileInfo _associated_file;
        bool _omit_download, _omit_upload,  _linked;
        #endregion

        /*2. REGION - PROPERTIES */
        #region Public Properties

        /*2.1 REGION - SHARED */
        #region Shared
        public string OwnerName { get { return this._owner_name; } }
        public string OwnerCertificate { get { return this._cert; } }
        public string FileName { get { return this._filename; } }
        public string CheckSum { get { return this._checksum; } }
        int Size { get { return this._filesize; } }        
        #endregion
        /*2.2 REGION - LOCAL */
        public string LocalFileName { get { return this._local_filename; } }
        public System.IO.FileInfo AssociatedFile { get { return this._associated_file; } }
        /*
         * Indicates that the user decided to keep his file not updated with the copies of other users.
         * Note: Other users won't be able to automatically keep up to   
         */
        public bool IsOmittedForDownload { get { return this._omit_download; } }
        //Indicates that the user decided to not share his local copy of the file.
        public bool IsOmittedForUpload { get { return this._omit_upload; } }
        /*
         * Indicates that the local copy was explicitly deleted and won't be restored automatically.
         * Note: It is essentially a wrapper for if(this.AssociatedFile!=null&&this.AssociatedFile.Exists)
         */
        public bool IsDeleted { get { return this._associated_file!=null&&this._associated_file.Exists; } }
        /*
         * Indicates that the user decided to link his local copy to another user's local copy of the file.
         * Note: Any edits to file attempted during the App's worktime will be reverted by a link source's copy in auto-mode. It also means,
         *       that at the startup, application will update the file with the link source so the local changes will be lost.
        */
        public bool IsLinked { get { return this._linked; } }
        #endregion

        #region Public constructors
        public SharedFile()
        {

        }
        #endregion
    }
}
