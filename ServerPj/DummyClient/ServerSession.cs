using System;
using ServerCore;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClient
{
    public abstract class Packet
    {
        public ushort size;
        public ushort packetId;

        public abstract ArraySegment<byte> Write();
        public abstract void Read(ArraySegment<byte> s);
    }
    
    class PlayerInfoReq : Packet
    {
        public long playerId;
        public string name;

        public List<int> skills = new List<int>();

        public PlayerInfoReq()
        {

            this.packetId = (ushort)PacketID.PlayerInfoReq;
        }
          
        public override void Read(ArraySegment<byte> segment)
        {
            ushort count = 0;

            ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

            
            //ushort size = BitConverter.ToUInt16(s.Array, s.Offset);
            count += sizeof(short);

            //ushort id = BitConverter.ToUInt16(s.Array, s.Offset + count);
            count += sizeof(short);

            this.playerId = BitConverter.ToInt64(s.Slice(count , s.Length - count));
            count += sizeof(long);

            //string
            ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);

            this.name = Encoding.Unicode.GetString(s.Slice(count, nameLen));

        }

        public override ArraySegment<byte> Write()
        { 
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);

            bool success = true;
            ushort count = 0;

            Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

            count += sizeof(ushort);

            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.packetId);
            count += sizeof(ushort);

            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
            count += sizeof(long);

            //string
            ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);

            count += sizeof(ushort);
            count += nameLen;

            this.size = count;
            success &= BitConverter.TryWriteBytes(s, count);


            if (success == false)
                return null;

            return SendBufferHelper.Close(count);
            
        }
    }

    public enum PacketID
    {
        PlayerInfoReq = 1,
        PlayerInfoOk = 2,

    }

    class ServerSession : Session
    {
        public override void OnConnect(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected {endPoint}");

            PlayerInfoReq packet = new PlayerInfoReq() { playerId = 1001 , name = "ABCD"};

            //보내기
            //for (int i = 0; i < 5; i++)
            {
                ArraySegment<byte> s = packet.Write();

                if(s != null )
                    Send(s);
            }

            Thread.Sleep(1000);

            //끝내기
            Disconnect();
            Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"Disconnected : {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> recvBuff)
        {
            string recvData = Encoding.UTF8.GetString(recvBuff.Array, recvBuff.Offset, recvBuff.Count);


            Console.WriteLine($"[From Server] : {recvData}");

            return recvBuff.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Trasnffered: {numOfBytes}");
        }
    }
}
