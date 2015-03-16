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
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using AutoShare.Engine.Network.Sharing;

namespace AutoShare
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public AutoShare.Engine.IO.FilesFolderChecker FolderWatchdog;
        public AutoShare.Engine.IO.SerializableList<UserInfo> KnownUsers;
        public AutoShare.Engine.Network.NetworkClient Client;
        public AutoShare.Engine.Network.NetworkServer Server;
        //TEMP
        public bool AskUserExit = true;
        App()
        {
            #region Initialization Block
            
            KnownUsers = new Engine.IO.SerializableList<Engine.Network.Sharing.UserInfo>();
            /** UNCOMMENT TO CREATE TEST USERINFO
             *
             * 
             UserInfo self = new UserInfo("TestBuddy", "Apple", new System.Net.IPEndPoint(new System.Net.IPAddress(new byte[4]{127,0,0,1}),4321), true);
             KnownUsers.List.Add(self);
            */
            Client = new Engine.Network.NetworkClient();
            Server = new Engine.Network.NetworkServer(4321);
            
            string path;
            if (AutoShare.Properties.Settings.Default.UseDocFolder)
                path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + System.IO.Path.DirectorySeparatorChar + AutoShare.Properties.Settings.Default.FolderPath;
            else
                path = AutoShare.Properties.Settings.Default.FolderPath;
            FolderWatchdog = new Engine.IO.FilesFolderChecker(path, true);


            
            
            #endregion
            #region Subscription to events
            
            
            #endregion
            #region Network Startup
            

            #endregion
            this.Initialize();


            //TEMP just for testing purposes

            
            
        }
        public void Initialize()
        {
            //TEMP as the Initialize() function is described in the paper
            BinaryFormatter formatter = new BinaryFormatter();
            string Path = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + System.IO.Path.DirectorySeparatorChar + 
                          AutoShare.Properties.Settings.Default.UserlistPrefix + System.Environment.UserName + '.' + AutoShare.Properties.Settings.Default.UserlistExtension;
            using (FileStream tfs = new FileStream(Path, FileMode.Open, FileAccess.Read))
                KnownUsers = (AutoShare.Engine.IO.SerializableList<UserInfo>)formatter.Deserialize(tfs);

        }
        public new void Shutdown()
        {
            //save all settings
            AutoShare.Properties.Settings.Default.Save();
            //save folder state
            
            //save user state
            //TEMP (because user can change its name, need to make login system)
            string CurrentUserName = System.Environment.UserName;
            //filepath
            string Path = AutoShare.Properties.Settings.Default.UserlistPrefix + CurrentUserName + '.' + AutoShare.Properties.Settings.Default.UserlistExtension; 
            string TempPath = AutoShare.Properties.Settings.Default.TemporaryUserlistName + '.' + AutoShare.Properties.Settings.Default.UserlistExtension;
            //get save path
            BinaryFormatter formatter = new BinaryFormatter();
            //TEMP as can be failed/access denied/program halted during I/O operation etc. Need to write first in a temp file, then rewrite, then delete temp file.
            //1. Save to temp folder
            using( FileStream tfs = new FileStream(TempPath, FileMode.Create, FileAccess.Write) )
                formatter.Serialize(tfs, KnownUsers,  
                    new System.Runtime.Remoting.Messaging.Header[1]{ new System.Runtime.Remoting.Messaging.Header("USERLISTLENGTH", KnownUsers.List.Count ) } );
            //2. Overwrite
            File.Copy( TempPath, System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + System.IO.Path.DirectorySeparatorChar + Path, true );
            
            
            //Dispose all the resources
            this.Server.Dispose();
            //Finally, shutdown itself
            base.Shutdown();
            
        }
        
        //END CLASS <<APP>>
    }





    //END APP.XAML.CS
}
