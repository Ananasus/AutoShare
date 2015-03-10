using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using System.IO;
using System.Windows.Shapes;
using System.Security.Permissions;


namespace AutoShare
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public AutoShare.Engine.IO.FilesFolderChecker FolderWatchdog;
        App()
        {
            #region Initialization Block
            string path;
            if (AutoShare.Properties.Settings.Default.UseDocFolder)
                path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + System.IO.Path.DirectorySeparatorChar + AutoShare.Properties.Settings.Default.FolderPath;
            else
                path = AutoShare.Properties.Settings.Default.FolderPath;
            FolderWatchdog = new Engine.IO.FilesFolderChecker(path, true);   
            #endregion

        }

    }
}
