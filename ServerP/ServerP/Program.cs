using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClient
{
    class Program
    { 
        static void Main(string[] args)
        {
            string hostName = Dns.GetHostName();
            IPHostEntry ipHost =  Dns.GetHostEntry(hostName);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

           
            try
            {
                Console.WriteLine("Connecting...");

                socket.Connect(endPoint);

                //send
                byte[] sendByts = Encoding.UTF8.GetBytes("Hello World");
                int sendLen = socket.Send(sendByts);


                //recv
                byte[] recvBytes = new byte[1024];
                int recvLen = socket.Receive(recvBytes);
                string recvBuffer = Encoding.UTF8.GetString(recvBytes, 0, recvLen);
                Console.WriteLine($"[From Server] : {recvBuffer}");

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine($"{e.ToString}");
            }

        }
    }
}