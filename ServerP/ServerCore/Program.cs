using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Program
    {
        static void Main(string[] args)
        {
            //DNS (Domain Name System)
            string hostName = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(hostName);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Socket socket = new Socket(endPoint.AddressFamily , SocketType.Stream , ProtocolType.Tcp);

            try
            {
                socket.Bind(endPoint);

                socket.Listen(10);

                while (true)
                {
                    Console.WriteLine("Listening...");

                    Socket clientSocket = socket.Accept();

                    //receive
                    byte[] recvBytes = new byte[1024];
                    int recvLen = clientSocket.Receive(recvBytes);
                    string recvBuffer = Encoding.UTF8.GetString(recvBytes, 0, recvLen);

                    Console.WriteLine($"[From Client] : {recvBuffer}");

                    //send
                    byte[] sendBytes = Encoding.UTF8.GetBytes($"Welecome MMORPG SERVER!");
                    int sendLen = clientSocket.Send(sendBytes);

                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
              
        }
    }
}