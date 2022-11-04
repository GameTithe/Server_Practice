using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerCore;

namespace Server
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

            this.playerId = BitConverter.ToInt64(s.Slice(count, s.Length - count));
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


    class ClientSession : PacketSession
    {
        public override void OnConnect(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected {endPoint}");

            // Packet packet = new Packet() { size = 100, packetId = 10 };
            //
            // ArraySegment<byte> openSegment = SendBufferHelper.Open(1024);
            // 
            // byte[] buffer1 = BitConverter.GetBytes(packet.size);
            // byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
            //
            // Array.Copy(buffer1, 0, openSegment.Array, openSegment.Offset, buffer1.Length);
            // Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer1.Length, buffer2.Length);
            //
            // ArraySegment<byte> sendBuffer = SendBufferHelper.Close(buffer1.Length + buffer2.Length);
            // 
            // Send(sendBuffer);
            Thread.Sleep(1000);
            Disconnect();
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort count = 0;
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            count += 2;

            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;

            switch((PacketID)id)
            {
                case PacketID.PlayerInfoReq:
                    {
                        PlayerInfoReq p = new PlayerInfoReq();
                        p.Read(buffer);

                        Console.WriteLine($"PlayearID : {p.playerId} {p.name}");
                    }
                    break;  
                case PacketID.PlayerInfoOk:
                    {

                    }
                    break;  
            }

        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"Disconnected : {endPoint}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Trasnffered: {numOfBytes}");
        }
    }


}
