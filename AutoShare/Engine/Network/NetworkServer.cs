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
        TcpListener Listener; // Объект, принимающий TCP-клиентов
        
        

        // Запуск сервера
        public NewtworkServer(int Port)
        {
            // Создаем "слушателя" для указанного порта
            Listener = new TcpListener(IPAddress.Any, Port);
            Listener.Start(); // Запускаем его

            // В бесконечном цикле
            while (true)
            {
                // Принимаем новых клиентов
                Listener.AcceptTcpClient();
            }
        }

        // Остановка сервера
        ~NewtworkServer()
        {
            // Если "слушатель" был создан
            if (Listener != null)
            {
                // Остановим его
                Listener.Stop();
            }

        }
    }
}