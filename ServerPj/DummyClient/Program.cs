using ServerCore;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClient
{
    class GameSession : Session
    {
        public override void OnConnect(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected {endPoint}");

            //보내기
            for (int i = 0; i < 5; i++)
            {
                byte[] sendBuff = Encoding.UTF8.GetBytes($"Hello Server {i}");
                Send(sendBuff);
            }

            Thread.Sleep(1000);
           
            //끝내기
            Disconnect();
            Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"Disconnected : {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> recvBuff)
        {
            string recvBytes = Encoding.UTF8.GetString(recvBuff.Array, recvBuff.Offset, recvBuff.Count);
            Console.WriteLine($"[From Server] : {recvBytes}");

            return recvBuff.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Trasnffered: {numOfBytes}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Connector connector = new Connector();

            connector.Connect(endPoint, () => { return new GameSession(); });
           
            
            while (true)
            {
                try
                {

                }
                catch (Exception e)
                {
                    Console.WriteLine($"Connect Fail (Client->Server) : {e.ToString()}");
                }
            }
        }
    }
}