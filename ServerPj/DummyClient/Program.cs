using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);


            while (true)
            {
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    socket.Connect(endPoint);
                    Console.WriteLine($"Connected TOo {socket.RemoteEndPoint.ToString()}");


                    //보내기
                    byte[] sendBuff = Encoding.UTF8.GetBytes($"Hello Server");
                    socket.Send(sendBuff);

                    //받기
                    byte[] recvBuff = new byte[1024];
                    int recvLen = socket.Receive(recvBuff);
                    string recvBytes = Encoding.UTF8.GetString(recvBuff, 0, recvLen);
                    Console.WriteLine($"[From Server] : {recvBytes}");


                    //끝내기
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();

                }
                catch (Exception e)
                {
                    Console.WriteLine($"Connect Fail (Client->Server) : {e.ToString()}");
                }

                Thread.Sleep(100);
            } 

        }
    }
}