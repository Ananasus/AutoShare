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
    public class NetworkServer
    {
        #region 
        TcpListener Listener; // Объект, принимающий TCP-клиентов
        ManualResetEvent HEvent;
        List<Thread> ProcessingThreads;
        Thread ServerThread;

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
                        ns.Read(buffer, 0, buffer.Length);

                        //TEMP
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
                TcpClient cli = Listener.EndAcceptTcpClient(res);
                if(cli!=null){
                    Thread thr = new Thread(ProcessClientThreadingFunction);
                    ProcessingThreads.Add(thr);
                    thr.Start( cli );
                }
            }, null);
            this.HEvent.WaitOne();
            Listener.Stop();
            
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
            this.HEvent.Set();
            for (int i = 0, s = ProcessingThreads.Count; i < s; ++i )
                ProcessingThreads[i].Abort();
        }
        #endregion
        #region API Calls

        public void Start()
        {
            if(ServerThread.ThreadState != ThreadState.Running){
                ServerThread.Start();
            }
        }
        #endregion
    }
}