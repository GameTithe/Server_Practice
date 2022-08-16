using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    static Listener _listener = new Listener();
    static void OnAcceptHandler(Socket clientSocket)
    {
        try
        {
            Session session = new Session();
            session.Start(clientSocket);

            byte[] sendBuff = Encoding.UTF8.GetBytes($"Welcome MMORPG!!");
            clientSocket.Send(sendBuff);

            Thread.Sleep(1000);

            session.Disconnect();

        }
        catch (Exception e)
        {
            Console.WriteLine($"Server Error : {e.ToString()}");
        }
    }
    static void Main(string[] Args)
    {
        // DNS (Domain Name System)
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        // 문지기

        _listener.Init(endPoint, OnAcceptHandler);
        Console.WriteLine("Listening...");

        while (true)
        {
            ;
        }


    }
}