using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;


namespace AutoShare.Engine.Network
{
    public class Node {
        System.Net.EndPoint EndPoint;
        string EncryptionKey;
    }

    public class Helper
    {
        static int DefaultServerPort = 4321;
    }

    public class Client
    {
        TcpClient tcp_client;
        List<Task> task;
        List<Node> nodes;
        Client()
        {
            
        }
        void AddNode(Node node){

        }
        Node this[int i]
        {
            get
            {
                return nodes[i];
            }
        }
        ~Client()
        {
            
        }
    }
}
