using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Program
    {
        static Listener _socket = new Listener();

        static void OnAcceptHandler(Socket socket)
        {
            try
            {
                //받기
                byte[] recvBuff = new byte[1024];
                int recvLen = socket.Receive(recvBuff);
                string recvBytes = Encoding.UTF8.GetString(recvBuff, 0, recvLen);
                Console.WriteLine($"[From Client] : {recvBytes}");

                //보내기
                byte[] sendBuff = Encoding.UTF8.GetBytes($"Welcome Server");
                socket.Send(sendBuff);

                //끝내기
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine($"Handler Error : {e.ToString()}");
            }
        }

        static void Main(string[] args)
        {
            //DNS (Domain Name System)
            string name = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(name);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            
            Console.WriteLine("Listening....");

            _socket.Init(endPoint, OnAcceptHandler);
         
            while(true)
            {
                ;
            }
        }
    }
}