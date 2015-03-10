using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AutoShare.Engine.Network 
{
    class NewtworkServer
    {
        TcpListener Listener; // ������, ����������� TCP-��������

        ManualResetEvent Hevent;

        TcpListener WaitForClient()
        {
            while (true)
            {
                Listener.AcceptTcpClient();
            }
        }
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