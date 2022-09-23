using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class GameSession : Session
    {
        public override void OnConnect(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected {endPoint}");

            //보내기
            byte[] sendBuffer = Encoding.UTF8.GetBytes($"Welcome Server!");
            Send(sendBuffer);

            Thread.Sleep(1000);

            //끝내기
            Disconnect();
            Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"Disconnected : {endPoint}");
        }

        public override void OnRecv(ArraySegment<byte> recvBuff)
        {
            string recvBytes = Encoding.UTF8.GetString(recvBuff.Array, recvBuff.Offset, recvBuff.Count);
            Console.WriteLine($"[From Client] : {recvBytes}");

        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Trasnffered: {numOfBytes}");
        }

    }
    class Program
    {
        static Listener _listener = new Listener();

        static void Main(string[] args)
        {
            //DNS (Domain Name System)
            string name = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(name);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            
            Console.WriteLine("Listening....");

            _listener.Init(endPoint, () => { return new GameSession(); });
            
            while(true)
            {
                ;
            }
        }
    }
}