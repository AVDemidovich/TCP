using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TCPclient
{
    public class Program
    {
        private const int portNum = 9050;
        private const string hostName = "WOOFWOOF";

        public static int Main(String[] args)
        {
            try
            {
                TcpClient client = new TcpClient(hostName, portNum);

                NetworkStream ns = client.GetStream();

                byte[] bytes = new byte[1024];
                for (int i = 0; i <= 100; i++)
                {
                    Console.WriteLine("Input:");
                    string Request = "";
                    Request = Console.ReadLine();
                    byte[] byteMessage = Encoding.ASCII.GetBytes(Request);
                    ns.Write(byteMessage, 0, byteMessage.Length);

                    int bytesRead = ns.Read(bytes, 0, bytes.Length);
                    Console.WriteLine(Encoding.ASCII.GetString(bytes, 0, bytesRead));
                    if (Request == "exit")
                    {
                        break;
                    }
                }
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Thread.Sleep(10000);
            return 0;
        }
    }
}
