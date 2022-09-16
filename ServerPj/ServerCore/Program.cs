using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Program
    {
        static Listener _listener = new Listener();
        static Session _session = new Session();

        static void OnAcceptHandler(Socket clientSocket)
        {
            try
            {
                _session.Init(clientSocket);

                //보내기
                _session.Send(Encoding.UTF8.GetBytes($"Welcome Server"));

                //끝내기
                _session.Disconnect();

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

            _listener.Init(endPoint, OnAcceptHandler);
            
            while(true)
            {
                ;
            }
        }
    }
}