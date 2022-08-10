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
            // 받는다.
            byte[] recvBuff = new byte[1024];
            int recvBytes = clientSocket.Receive(recvBuff);
            string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
            Console.WriteLine($"[From Client] : {recvData}");

            //보낸다.
            byte[] sendBuff = Encoding.UTF8.GetBytes($"Welecom to MMORPG Server");
            clientSocket.Send(sendBuff);

            //쫓아낸다
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
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
        
        }
    }
}
