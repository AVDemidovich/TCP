using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TCPserver
{
    public class Program
    {
        private const int portNum = 9050;

        public static int Main(String[] args)
        {
            bool done = false;
            IPAddress ipaddress = IPAddress.Parse("192.168.0.103");
            TcpListener listener = new TcpListener(ipaddress, portNum);

            listener.Start();

            while (!done)
            {
                Console.Write("Waiting for connection...");
                TcpClient client = listener.AcceptTcpClient();

                Console.WriteLine("Connection accepted.");
                NetworkStream ns = client.GetStream();

                byte[] bytes = new byte[1024];
                int bytesRead = ns.Read(bytes, 0, bytes.Length);
                Console.WriteLine(Encoding.ASCII.GetString(bytes, 0, bytesRead));

                byte[] byteMessage = Encoding.ASCII.GetBytes("hi");

                try
                {
                    ns.Write(byteMessage, 0, byteMessage.Length);
                    ns.Close();
                    client.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            listener.Stop();

            return 0;
        }
    }
}
