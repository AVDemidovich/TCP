using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;

namespace TCPserver
{
    class Client
    {
        public Client(TcpClient Client)
        {
            string Request = "";
            byte[] Buffer = new byte[1024];
            int Count;
            while ((Count = Client.GetStream().Read(Buffer, 0, Buffer.Length)) > 0)
            {

                Request += Encoding.ASCII.GetString(Buffer, 0, Count);
                // Запрос должен обрываться последовательностью \r\n\r\n
                // Либо обрываем прием данных сами, если длина строки Request превышает 4 килобайта
                // Нам не нужно получать данные из POST-запроса (и т. п.), а обычный запрос
                // по идее не должен быть больше 4 килобайт
                break;
                if (Request.IndexOf("\r\n\r\n") >= 0 || Request.Length > 4096)
                {
                    break;
                }
            }

            //Request = Request.Trim(new char[] { ',', '.' }); //удаление точен и запятых            while (Request.Contains("  ")) //пока есть двойные пробелы
            Request = Request.Replace(".", " ");
            Request = Request.Replace("  ", " ");


            string[] textArray = Request.Split(new char[] { ' ' });

            Console.WriteLine(Request);
            Console.WriteLine("words num: " + textArray.Length);
            
            
            byte[] HeadersBuffer = Encoding.ASCII.GetBytes(textArray.Length.ToString());
            Client.GetStream().Write(HeadersBuffer, 0, HeadersBuffer.Length);
           
            Client.Close();
        }
    }

    public class Server
    {
        TcpListener Listener; 

        public Server(int Port)
        {
            IPAddress ipaddress = IPAddress.Parse("192.168.0.103");
        //    Listener = new TcpListener(ipaddress, Port); 
            Listener = new TcpListener(IPAddress.Any, Port); 
            Listener.Start();

            while (true)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(ClientThread), Listener.AcceptTcpClient());

                TcpClient Client = Listener.AcceptTcpClient();
                Thread Thread = new Thread(new ParameterizedThreadStart(ClientThread));
                Thread.Start(Client);
                
            }
        }

        static void ClientThread(Object StateInfo)
        {
            new Client((TcpClient)StateInfo);
        }

        ~Server()
        {
            if (Listener != null)
            {
                Listener.Stop();
            }
        }

        public static void Main(string[] args)
        {
            int MaxThreadsCount = Environment.ProcessorCount * 4;
            ThreadPool.SetMaxThreads(MaxThreadsCount, MaxThreadsCount);
            ThreadPool.SetMinThreads(2, 2);
            new Server(9050);
        }
    }
}
