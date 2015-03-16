using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using AutoShare.Engine.Network;

namespace AutoShare.Engine.Network 
{
    public class NetworkServer:IDisposable
    {
        #region 
        TcpListener Listener; // Объект, принимающий TCP-клиентов
        ManualResetEvent HEvent;
        List<Thread> ProcessingThreads;
        Thread ServerThread;
        bool Disposed; //Показывает, рабочий ли объект класса на данный момент
        #endregion


        #region Protected Functions
        void DisposeImplemention()
        {
            if (!Disposed)
            {
                this.HEvent.Set();
                for (int i = 0, s = ProcessingThreads.Count; i < s; ++i)
                    ProcessingThreads[i].Abort(); //TODO either remove this madness or make another approach as the processing thread could be running long and is NOT a critical (means important) thread

                Disposed = true;
            }

            
        }

        void Resurrect()
        {
            if (Disposed)
            {
                //TODO - ressurect
            }

        }
        #endregion
        #region Threading Functions
        void ProcessClientThreadingFunction(object client)
        {
            TcpClient cli = client as TcpClient;
            try
            {
                if (cli != null)
                {
                    if (HEvent.WaitOne(0)) //check if the class is being destructed //Probably unnecessary
                        cli.Close();
                    else
                    {
                        NetworkStream ns = cli.GetStream();
                        byte[] bytes_to_send, buffer = new byte[NetPacket.PacketSignatureRule.MaxSignatureSize];
                        //Accept the signature and some data (probably)
                        int nbytes = cli.Available;
                        ns.Read(buffer, 0, nbytes);
                        //TEMP
                        if (System.Text.UTF8Encoding.UTF8.GetString(buffer, 0, nbytes) == "PING")
                            this.DispatchEvent(this.PingReceived);
                            
                        bytes_to_send = System.Text.UTF8Encoding.UTF8.GetBytes("PONG");
                        //Final Function
                        ns.Write(bytes_to_send, 0, bytes_to_send.Length);
                    }
                }
            }
            catch (ThreadAbortException e)
            {
                //Close connection as the thread will be removed soon
                if (cli != null)
                    cli.Close();
            }
            
        }
        void ServerThreadFunction()
        {
            Listener.Start();
            Listener.BeginAcceptTcpClient( (IAsyncResult res)=>{
                try
                {
                    TcpClient cli = Listener.EndAcceptTcpClient(res);
                    if (cli != null)
                    {
                        Thread thr = new Thread(ProcessClientThreadingFunction);
                        ProcessingThreads.Add(thr);
                        thr.Start(cli);
                    }
                }
                catch (ObjectDisposedException){
                    //ok, socket was disposed outside the code
                }
                
            }, null);
            this.HEvent.WaitOne();
            Listener.Stop();
            
        }
        #endregion 
        #region Public Events and Event Dispatchers
        public event EventHandler<AutoShare.Engine.Network.NetPacket> AcceptedClient;
        //TEMP temporary event just for testing purposes (maybe add a META HEAD "TEST"?)
        //TEST Yeah, maybe
        public event EventHandler PingReceived;
        protected void DispatchEvent(Delegate Event, object value = null)
        {
            if (Event != null)
                Event.DynamicInvoke(this, value??new EventArgs());
        }
        #endregion 
        #region Constructors and Destructors
        //Запуск Сервера
        public NetworkServer(int Port, bool StartImmediately = true)
        {
            // Создаем "слушателя" для указанного порта

            this.Listener = new TcpListener(IPAddress.Any, Port);
            this.HEvent = new ManualResetEvent(false);
            this.ServerThread = new Thread( ServerThreadFunction );
            this.ProcessingThreads = new List<Thread>();
            if (StartImmediately)
                this.ServerThread.Start();
            
        }

        
        // Остановка сервера
        ~NetworkServer()
        {
            this.DisposeImplemention();
        }
        #endregion
        #region API Calls

        public void Start()
        {
            if(ServerThread.ThreadState != ThreadState.Running){
                ServerThread.Start();
            }
        }

        public void Dispose()
        {
            this.DisposeImplemention();
        }
        #endregion
    }
}