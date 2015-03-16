using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using AutoShare.Engine.Network.Sharing;

namespace AutoShare.Engine.Network
{
    //TODO: MOVE LIST<USERStatus> outside of class. To sync values from the server and from the client

    public class NetworkClient
    {
        #region Helper Classes
        public struct UserStatus
        {
            public UserInfo Info;
            public DateTime LastPinged;
            public bool IsOnline;

            
            public UserStatus(UserInfo Info, DateTime LastPinged, bool IsOnline)
            {
                this.Info = Info;
                this.LastPinged = LastPinged;
                this.IsOnline = IsOnline;
            }

        }

        #endregion
        #region Private Members
        List<Task> task;
        ManualResetEvent mres;
        List<Thread> socket_threads;
        List<UserStatus> nodes; //ping nodes

        #endregion
        #region Thread Functions

        void SocketThread(object index)
        {
            TcpClient cli = null;
            UserStatus us;
            try {
                
                lock (nodes)
                    us = nodes[(int)index];
                cli = new TcpClient();
                //BEFORE FIRST
                cli.NoDelay = false;
                cli.ReceiveTimeout = 10;
                cli.SendTimeout = 5;
                //FIRST - connect to the last known
                cli.Connect(us.Info.LastKnownAddress);
                //SECOND - cycle through the known ip addresses
                if (!cli.Connected)
                {
                    for (int i = 0, s = us.Info.AddressHistory.Count; i < s; ++i)
                    {
                        cli.Connect(us.Info.AddressHistory[i].EndPoint);
                        if (cli.Connected)
                            break;
                    }
                }
                //THIRD - EITHER DIE WITH NO STATE OR CONNECT AND SYNC LIST
                us.LastPinged = DateTime.Now;
                //TEMP just a workaround
                us.IsOnline = cli.Connected;
                if (us.IsOnline)
                {
                    //TEMP: Send something
                    NetworkStream ns = cli.GetStream();
                    byte[] bytes = System.Text.UTF8Encoding.UTF8.GetBytes("PING");
                    ns.Write( bytes, 0, bytes.Length );
                    ns.Close();
                }
            }
            catch (ThreadAbortException) {

            }
            finally {
                //set thread running to false
                if(cli!=null)
                    cli.Close();

            }

            


            //END

        }

        #endregion
        #region Constructor and Destructors
        public NetworkClient(IEnumerable<UserStatus> UserStatuses = null, bool StartImmediately = true)
        {
            if (UserStatuses != null && UserStatuses.Count() > 0)
                nodes = new List<UserStatus>(UserStatuses);
            else
                nodes = new List<UserStatus>();
            socket_threads = new List<Thread>();
            mres = new ManualResetEvent(false);
            if (nodes.Count > 0)
            {
                for (int i = 0, s = nodes.Count; i < s; ++i )
                    AddThread(true);
            }
        }
        
        ~NetworkClient()
        {
            mres.Set();
            //Delay for dispose
            mres.WaitOne();
            //Dispose object
            mres.Dispose();
        }
        
        #endregion
        #region Protected Methods
        protected void AddThread(bool StartImmediately)
        {
            Thread thr = new Thread(SocketThread);
            lock (this.socket_threads)
            {
                this.socket_threads.Add(thr);
                if (StartImmediately)
                    thr.Start(this.socket_threads.Count - 1);
            }
            
        }
        #endregion
        
        #region API Calls and Public Accessors
        public void AddNode(UserStatus UserStatus, bool StartImmediately = true)
        {
            lock (this.nodes)
            {
                this.nodes.Add(UserStatus);
                this.AddThread(StartImmediately);
            }
        }
        public void RemoveNode(int index)
        {
            this.nodes.RemoveAt(index);
            this.socket_threads[index].Abort();
            this.socket_threads.RemoveAt(index);
        }
        public UserStatus this[int i] { get { return nodes[i]; } set { nodes[i] = value; } }

        public ThreadState NodeThreadState(int i)
        {
            return this.socket_threads[i].ThreadState;
        }

        public void StartNodeThread(int i)
        {
            this.socket_threads[i].Start();
        }

        public void StopNodeThread(int i)
        {
            //TEMP and workaround. Recode this thing
            this.socket_threads[i].Abort();
        }
        #endregion
    }
}
