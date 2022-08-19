using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerCore;
class Knight
{
    public int hp;
    public int attack;
}
class GameSession : Session
{
    public override void OnConnected(EndPoint endPoint)
    {
        Console.WriteLine($"OnConnected : {endPoint}");

        Knight knight = new Knight() { hp = 100, attack = 10 };


        ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);

        byte[] buffer = BitConverter.GetBytes(knight.hp);
        byte[] buffer2 = BitConverter.GetBytes(knight.attack);
        Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
        Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);

        ArraySegment<byte> sendBuffer = SendBufferHelper.Close(buffer.Length + buffer2.Length);



        Send(sendBuffer);
        Thread.Sleep(1000);
        Disconnect();


    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        Console.WriteLine($"OnDisconnecrted : {endPoint}");
    }

    // 이동 패킷 ( (3,2)좌표로 이동하고 싶다! )
    // 15 3 2   
    public override int OnRecv(ArraySegment<byte> buffer)
    {
        string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
        Console.WriteLine($" [From Client] : {recvData} ");

        return buffer.Count;
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