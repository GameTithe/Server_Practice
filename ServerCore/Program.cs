using ServerCore;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Progam
{
    static Listener _listener = new Listener();

    static void OnAccptHandler(Socket clientSocket)
    {
        try
        {
            Session session = new Session();
            session.Start(clientSocket);

            byte[] sendBuff = Encoding.UTF8.GetBytes($"Welecom to MMORPG Server");
            session.Send(sendBuff);

            Thread.Sleep(1000);

            session.Disconnect();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    static void Main(string[] args)
    {
        // DNS (Domain Name System)
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAdr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAdr, 7777);

        // www.rookiss.com -> 123.123.123.12

        _listener.Init(endPoint, OnAccptHandler);
        Console.WriteLine("Listening...");

        
        while (true)
        {
            ;
        }
    }
}
