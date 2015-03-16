using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Permissions;
using System.Threading;

namespace AutoShare.Engine.IO
{
    public class FileCheckEntry
    {
        string FilePath;

        bool IsShared;
        bool IsChanged;
        bool IsDeleted;
        bool IsRenamed;

        FileCheckEntry(string FilePath, bool IsShared, bool IsChanged, bool IsRenamed, bool IsDeleted)
        {
            this.FilePath = FilePath;
            this.IsShared = IsShared;
            this.IsShared = IsRenamed;
            this.IsDeleted = IsDeleted;
        }
    }

    public class FileList
    {
        List<FileCheckEntry> FList;

        void Add() { }
        void Remove() { }
        void SyncFileList() { }
    }


    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class FilesFolderChecker
    {
        #region Helper Classes
        public enum StatusInfo {
            SI_STAT_OK              = 0x00,
            //ACTIONS DONE IMPLICITLY
            SI_ACTN_DIR_CREATED     = 0x01,
            SI_ACTN_FILE_REM_DUP    = 0x02,
            //STATUSES DURING WORK
            SI_STAT_DIR_NOTFOUND    = 0x10,
            SI_STAT_DIR_DENIED      = 0x20,
        }
        public enum LoadType
        {
            FLT_UPDATE,
            FLT_STUB,
        }
        #endregion 
        #region Private Entities

        FileSystemWatcher watcher;
        ManualResetEventSlim halt_event;
        DirectoryInfo dir;
        bool is_ok = true;
        bool allow_imp_actions;
        StatusInfo status = StatusInfo.SI_STAT_OK;
        #endregion
        #region Protected Functions
        /// <summary>
        /// Every API Call will start with this function call. It will throw an error, if state is invalid
        /// </summary>
        void PassOrThrow()
        {
            if (!this.is_ok)
                throw new InvalidOperationException("FilesFolderChecker class instance is in invalid state");
        }

        protected void OnOccure(object source, FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                //notify about that
                //rescan folders
            }
        }
        #endregion
        #region Constructors and Destructor
        public FilesFolderChecker(string FolderPath, bool AllowImplicitActions = true/*, FileSharedList StubList*/)
        {
            this.allow_imp_actions = AllowImplicitActions;
            
            try
            {
                dir = new DirectoryInfo(FolderPath);
                if (!dir.Exists)
                {
                    this.status |= StatusInfo.SI_STAT_DIR_NOTFOUND;
                    if (AllowImplicitActions)
                    {
                        this.status |= StatusInfo.SI_ACTN_DIR_CREATED;
                        dir.Create();
                    }
                    else
                        is_ok = false;
                }
                    
            }
            catch (DirectoryNotFoundException)
            {
                status |= StatusInfo.SI_STAT_DIR_NOTFOUND;
                //create folder
                if(AllowImplicitActions)
                    try
                    {
                        dir = Directory.CreateDirectory(FolderPath, new System.Security.AccessControl.DirectorySecurity(FolderPath, System.Security.AccessControl.AccessControlSections.All));    
                    }
                    catch (UnauthorizedAccessException)
                    {
                        is_ok = false;
                        status |= StatusInfo.SI_STAT_DIR_DENIED;
                    }
                else
                    is_ok = false;

            }
            catch (UnauthorizedAccessException) {
                is_ok = false;
                status |= StatusInfo.SI_STAT_DIR_DENIED;
            }


            watcher = new FileSystemWatcher(FolderPath);
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            //watcher.Filter = "*.*";
            watcher.Changed += OnOccure;
            watcher.Created += OnOccure;
            watcher.Deleted += OnOccure;
            watcher.Renamed += OnOccure;

            watcher.EnableRaisingEvents = true;
            
        }
        ~FilesFolderChecker()
        {
            //do something here
            watcher.Changed -= OnOccure;
            watcher.Created -= OnOccure;
            watcher.Deleted -= OnOccure;
            watcher.Renamed -= OnOccure;
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }
        #endregion
        #region API CALLS
        //DRAG AND DROP OPERATIONS
        public bool IsInSyncFolder(string FilePath)
        {
            string root = Path.GetDirectoryName(FilePath);
            return (root == this.dir.FullName);
        }
        public bool HasFileName(string FilePath)
        {
            FileInfo fi = new FileInfo(this.dir.FullName + Path.DirectorySeparatorChar + Path.GetFileName(FilePath));
            return fi.Exists;
        }
        public bool DropFile(string FilePath){
            PassOrThrow();
            //so, open stream for read
            try
            {
                System.IO.File.Copy(FilePath, this.dir.FullName + Path.DirectorySeparatorChar + Path.GetFileName(FilePath), true);
            }
            catch
            {
                return false;
            }

            return true;
        }
        public int DropFiles(string[] FileNames)
        {
            int i = 0;
            PassOrThrow();

            throw new NotImplementedException();
            return i;
        }
        void BeginDropFiles(string[] FileNames)
        {
            PassOrThrow();
            throw new NotImplementedException();
        }
        void EndDropFiles(string[] FileNames)
        {
            PassOrThrow();
            throw new NotImplementedException();
        }
        
        
        //CHECK FOLDER OPERATIONS
        void LoadFileList(  )
        {
            PassOrThrow();
            throw new NotImplementedException();
        }
        #endregion
        
    }
}
