using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClient
{
    class Progam
    {
        static void Main(string[] args)
        {
            string name = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(name);
            IPAddress ipAdr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAdr, 7777);

            while (true)
            {
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    socket.Connect(endPoint);

                    //보낸다
                    for (int i = 0; i < 5; i++)
                    {
                        byte[] sendBuff = Encoding.UTF8.GetBytes($"Hello World! {i} ");
                        int sendBytes = socket.Send(sendBuff);
                    }
                    //받는다
                    byte[] recvBuff = new byte[1024];
                    int recvLen = socket.Receive(recvBuff);

                    string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvLen);
                    Console.WriteLine($"[From Server] : {recvData}");


                    //끊는다
                    //socket.Shutdown(SocketShutdown.Both);
                    //socket.Close();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

        }
    }
}