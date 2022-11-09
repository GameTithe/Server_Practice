using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerCore;

namespace Server
{
    class Program
    {
        static Listener _listener = new Listener();

        static void Main(string[] args)
        {
            PacketManager.Instacne.Register();

            //DNS (Domain Name System)
            string name = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(name);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);


            Console.WriteLine("Listening....");

            _listener.Init(endPoint, () => { return new ClientSession(); });

            while (true)
            {
                ;
            }
        }
    }
}