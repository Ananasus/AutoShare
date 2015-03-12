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
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using System.Security.Permissions;
using System.Threading;
using System.Collections.ObjectModel;


namespace AutoShare
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : MetroWindow
    {
        ObservableCollection<AutoShare.Engine.Network.Sharing.UserInfo> UsersModel;
        public MainWindow()
        {
            InitializeComponent();
            UsersModel = new ObservableCollection<Engine.Network.Sharing.UserInfo>((App.Current as App).KnownUsers.List);
            UsersDataGrid.ItemsSource = UsersModel;
            UsersModel.CollectionChanged += UsersModel_CollectionChanged;
        }

        void UsersModel_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void ShowBinaryDialog(string message, string caption, Action onOk, Action onCancel = null, string okButton = "Yes", string cancelButton = "No")
        {
            new Thread(() =>
            {
                MahApps.Metro.Controls.Dialogs.MetroDialogSettings ms = new MahApps.Metro.Controls.Dialogs.MetroDialogSettings();
                ms.AffirmativeButtonText = okButton;
                ms.NegativeButtonText = cancelButton;
                ms.AnimateHide = ms.AnimateShow = false;
                ms.ColorScheme = MahApps.Metro.Controls.Dialogs.MetroDialogColorScheme.Theme;
                Task<MessageDialogResult> res = null;
                this.Dispatcher.Invoke(new Action(() =>
                {
                    res = this.ShowMessageAsync(caption, message, MessageDialogStyle.AffirmativeAndNegative, ms);
                    

                }), System.Windows.Threading.DispatcherPriority.Send);
                res.Wait();
                if (res.Result == MessageDialogResult.Affirmative)
                    onOk();
                else if (onCancel != null)
                    onCancel();
            }).Start();
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
                        //check if the file really exists
                        if (System.IO.File.Exists(fileLoc))
                        {
                            //check if the file with that name does not exist in the Sync folder
                            if (!(App.Current as App).FolderWatchdog.HasFileName(fileLoc))
                                (App.Current as App).FolderWatchdog.DropFile(fileLoc);
                            //check if the user is dropping a file from a sync folder (wtf he is doing?) //temp
                            else if (!(App.Current as App).FolderWatchdog.IsInSyncFolder(fileLoc))
                                this.ShowBinaryDialog("A file with that name already exists in the Sync Folder. Do you want to overwrite it?",
                                                      "Overwriting descision:", () => { (App.Current as App).FolderWatchdog.DropFile(fileLoc); });
                            
                        }
                    }
                    catch {
                    
                    }
                }
            }
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //TEMP
            if ((App.Current as App).AskUserExit)
            {
                this.ShowBinaryDialog("There are actions preventing to close the application. Do you really want to exit?", "Exit Confirmation" , () => {
                    this.Dispatcher.BeginInvoke(new Action(() => {
                        this.Closing -= OnClosing;
                        this.Close(); 
                    }));  
                });
            }
            e.Cancel = true;
            
        }
    }
                    
    
}
