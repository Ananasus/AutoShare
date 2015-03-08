using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace AutoShare.Engine.Networks
{
    public class Node {
        System.Net.EndPoint EndPoint;
        string Key;
        

    }

    public class Helper
    {
        static const int DefaultServerPort = 4321;
    }

    public class Client
    {
        TcpClient tcp_client;
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
