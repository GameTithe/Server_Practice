using System.Net;
using System.Net.Sockets;
using System.Text;

class GameSession : Session
{
    public override void OnConnected(EndPoint endPoint)
    {
        Console.WriteLine($"OnConnected : {endPoint}");

        byte[] sendBuff = Encoding.UTF8.GetBytes($"Welcome MMORPG!!");
        Send(sendBuff);

        Thread.Sleep(1000);

        Disconnect();


    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        Console.WriteLine($"OnDisconnecrted : {endPoint}");
    }

    public override void OnRecv(ArraySegment<byte> buffer)
    {
        string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
        Console.WriteLine($" [From Client] : {recvData} ");

    }

    public override void OnSend(int numOfBytes)
    {
        Console.WriteLine($"Transfferd bytes : {numOfBytes}");

    }
}

class Program
{
    
    static Listener _listener = new Listener();

    static void Main(string[] Args)
    {
        // DNS (Domain Name System)
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        // 문지기

        _listener.Init(endPoint, () => { return new GameSession(); });

        Console.WriteLine("Listening...");

        while (true)
        {
            ;
        }


    }
}