using ServerCore;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClient
{
    class Packet
    {
        public ushort size;
        public ushort packetId;
    }

    class PlayerInfoReq : Packet
    {
        public long playerId;
    }

    class PlayerInfoOk : Packet
    {
        public int hp;
        public int attack;
    }

    public enum PacketID
    {
        PlayerInfoReq = 1,
        PlayerInfoOk = 2,
    }

    class ServerSession : Session
    {   
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            PlayerInfoReq packet = new PlayerInfoReq() { size = 4, packetId = (ushort)PacketID.PlayerInfoReq, playerId = 1001};

            //보낸다
            //for (int i = 0; i < 5; i++)
            {
                ArraySegment<byte> s = SendBufferHelper.Open(4096);


                ushort count = 0; 
                bool success = true;
                
                //success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count),packet.size);
                count += 2;

                success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count),packet.packetId);
                count += 2;

                success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count , s.Count - count),packet.playerId);
                count += 8;

                success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), count);


                ArraySegment<byte> sendBuff = SendBufferHelper.Close(count);

                if(success)
                    Send(sendBuff);
            }
        }

        public override void OnDisConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisConnected : {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, 0, buffer.Count);

            Console.WriteLine($"[From Server] {recvData}");

            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes : {numOfBytes}");
        }
    }

    class Progam
    {
        static void Main(string[] args)
        {
            string name = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(name);
            IPAddress ipAdr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAdr, 7777);

            Connector connector = new Connector();
            connector.Connect(endPoint, () => { return new ServerSession(); });

            while (true)
            {
                try
                {
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

        }
    }
}
