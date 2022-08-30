using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Program
    {
        static Listener _listener = new Listener();
    
        static void onAcceptHandler(Socket clientSocket)
        {
            try
            {
                Session session = new Session();
                session.Start(clientSocket);

                byte[] sendBuff = Encoding.UTF8.GetBytes($"Welecom To MMO Server");

                session.Send(sendBuff);

                Thread.Sleep(1000);

                session.Disconnect();
                session.Disconnect();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAdr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAdr, 7777);

            _listener.Init(endPoint, onAcceptHandler);
            Console.WriteLine("Listening...");

            while (true)
            {
                ;
            }
        }
    }
}