using ServerCore;
using System.Net;
using System.Net.Sockets;
using System.Text;

class GameSession : Session
{
    public override void OnConnected(EndPoint endPoint)
    {
        Console.WriteLine($"On Connected : {endPoint}");

        byte[] sendBuff = Encoding.UTF8.GetBytes($"Welecom to MMORPG Server");
        Send(sendBuff);

        Thread.Sleep(1000);

        Disconnect();
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        Console.WriteLine($"On Disconnected : {endPoint}");
    }

    public override void OnRecv(ArraySegment<byte> buffer)
    {
        string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
        Console.WriteLine($"[From Client] : {recvData}");
    }

    public override void OnSend(int numOfBytes)
    {
        Console.WriteLine($"Transferred bytes {numOfBytes}");
    }
}
class Progam
{
    static Listener _listener = new Listener();
    

    static void Main(string[] args)
    {
        // DNS (Domain Name System)
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAdr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAdr, 7777);

        // www.rookiss.com -> 123.123.123.12

        _listener.Init(endPoint, () => { return new GameSession();  });
        Console.WriteLine("Listening...");

        
        while (true)
        {
            ;
        }
    }
}
