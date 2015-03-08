using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using System.Windows.Shapes;
using System.Security.Permissions;

namespace AutoShare
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void FileDropped(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])(e.Data.GetData(DataFormats.FileDrop));
                foreach (string fileLoc in filePaths)
                {
                    try
                    {
                        if (File.Exists(fileLoc))
                        {
                        
                        }
                    }
                    catch { }
                }
            }
        }
    }
                    
    public class File
    {
        string FilePath;
        bool IsShared;
        bool IsChanged;
        bool IsDeleted;
        bool IsRenamed;

        File(string FilePath, bool IsShared, bool IsChanged, bool IsRenamed, bool IsDeleted) 
        { 
            this.FilePath = FilePath;
            this.IsShared = IsShared;
            this.IsShared = IsRenamed;
            this.IsDeleted = IsDeleted;
        }
                }

    public class FileList
    {
        List<File> FList;

        void Add() { }
        void Remove() { }
        void SyncFileList(){ }
            }


    public class FilesFolderChecker
    {
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        static void CheckFolder()
        {
            
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = Properties.Settings.Default.FolderPath;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            //watcher.Filter = "*.*";
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnCreated);
            watcher.Deleted += new FileSystemEventHandler(OnDeleted);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);

            watcher.EnableRaisingEvents = true;
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {

        }
        private static void OnDeleted(object source, FileSystemEventArgs e)
        {
            
        }
        private static void OnCreated(object source, FileSystemEventArgs e) 
        {

        }
        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            
        }
    }
}
