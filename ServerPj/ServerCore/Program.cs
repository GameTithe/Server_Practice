using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Program
    {
        static Listener _listener = new Listener();
        
        static void OnAcceptHandler(Socket clientSocket)
        {
            try
            {
                Session _session = new Session();

                _session.Init(clientSocket);

                //보내기
                byte[] sendBuffer = Encoding.UTF8.GetBytes($"Welcome Server!");
                _session.Send(sendBuffer);

                Thread.Sleep(1000); 
                //끝내기

                _session.Disconnect();
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