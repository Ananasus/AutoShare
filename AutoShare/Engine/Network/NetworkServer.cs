using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Autoshare.Engine.Network 
{
    class NewtworkServer
    {
        TcpListener Listener; // ������, ����������� TCP-��������
        
        

        // ������ �������
        public NewtworkServer(int Port)
        {
            // ������� "���������" ��� ���������� �����
            Listener = new TcpListener(IPAddress.Any, Port);
            Listener.Start(); // ��������� ���

            // � ����������� �����
            while (true)
            {
                // ��������� ����� ��������
                Listener.AcceptTcpClient();
            }
        }

        // ��������� �������
        ~NewtworkServer()
        {
            // ���� "���������" ��� ������
            if (Listener != null)
            {
                // ��������� ���
                Listener.Stop();
            }

        }
    }
}