using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace TCPserver
{
    class Client
    {
        public Client(TcpClient Client)
        {
            string Request = "";
            byte[] Buffer = new byte[1024];
            int Count;
            for (int i = 0; i <= 100; i++)
            {
                Count = Client.GetStream().Read(Buffer, 0, Buffer.Length);

                Request = Encoding.ASCII.GetString(Buffer, 0, Count);
                Request = Request.Replace(",", " ");
                Request = Request.Replace("  ", " ");

                string[] textArray = Request.Split(new char[] { ' ' });

                Console.WriteLine(Request);
                Console.WriteLine("words num: " + textArray.Length);

                byte[] HeadersBuffer = Encoding.ASCII.GetBytes(textArray.Length.ToString());
                Client.GetStream().Write(HeadersBuffer, 0, HeadersBuffer.Length);
                if (Request == "exit")
                {
                    break;
                }
            }
            Client.Close();
        }
    }

    public class Server
    {
        TcpListener Listener; 

        public Server(int Port)
        {
            Listener = new TcpListener(IPAddress.Any, Port); 
            Listener.Start();
            int i = 0;
            while (i <= 8)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(ClientThread), Listener.AcceptTcpClient());
                TcpClient Client = Listener.AcceptTcpClient();
                Thread Thread = new Thread(new ParameterizedThreadStart(ClientThread));
                Thread.Start(Client);
                i++;
                Thread.Sleep(1000);
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
