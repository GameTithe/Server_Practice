using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerCore;

namespace Server
{
	using System;
	using ServerCore;
	using System.Net;
	using System.Net.Sockets;
	using System.Text;

	public enum PacketID
	{
		PlayerInfoReq = 1,
		Test = 2,

	}


	class PlayerInfoReq
	{
		public byte testByte;
		public long playerId;
		public string name;

		public class Skill
		{
			public int id;
			public short level;
			public float duration;

			public class Attribute
			{
				public int att;

				public void Read(ReadOnlySpan<byte> s, ref ushort count)
				{

					this.att = BitConverter.ToInt32(s.Slice(count, s.Length - count));
					count += sizeof(int);

				}

				public bool Write(Span<byte> s, ref ushort count)
				{
					bool success = true;


					success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.att);
					count += sizeof(int);


					return success;
				}


			}
			public List<Attribute> attributes = new List<Attribute>();



			public void Read(ReadOnlySpan<byte> s, ref ushort count)
			{

				this.id = BitConverter.ToInt32(s.Slice(count, s.Length - count));
				count += sizeof(int);


				this.level = BitConverter.ToInt16(s.Slice(count, s.Length - count));
				count += sizeof(short);


				this.duration = BitConverter.ToSingle(s.Slice(count, s.Length - count));
				count += sizeof(float);


				attributes.Clear();

				ushort attributeLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
				count += sizeof(ushort);

				for (int i = 0; i < attributeLen; i++)
				{
					Attribute attribute = new Attribute();
					attribute.Read(s, ref count);
					attributes.Add(attribute);

				}

			}

			public bool Write(Span<byte> s, ref ushort count)
			{
				bool success = true;


				success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.id);
				count += sizeof(int);


				success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.level);
				count += sizeof(short);


				success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.duration);
				count += sizeof(float);


				success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)attributes.Count);
				count += sizeof(ushort);

				foreach (Attribute attribute in attributes)
					success &= attribute.Write(s, ref count);



				return success;
			}


		}
		public List<Skill> skills = new List<Skill>();




		public void Read(ArraySegment<byte> segment)
		{
			ushort count = 0;

			ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

			count += sizeof(short);
			count += sizeof(short);

			this.testByte = segment.Array[segment.Offset + count];
			count += sizeof(byte);


			this.playerId = BitConverter.ToInt64(s.Slice(count, s.Length - count));
			count += sizeof(long);


			ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
			count += sizeof(ushort);
			this.name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
			count += nameLen;


			skills.Clear();

			ushort skillLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
			count += sizeof(ushort);

			for (int i = 0; i < skillLen; i++)
			{
				Skill skill = new Skill();
				skill.Read(s, ref count);
				skills.Add(skill);

			}

		}

		public ArraySegment<byte> Write()
		{
			ArraySegment<byte> segment = SendBufferHelper.Open(4096);

			bool success = true;
			ushort count = 0;

			Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

			count += sizeof(ushort);

			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.PlayerInfoReq);
			count += sizeof(ushort);

			segment.Array[segment.Offset + count] = this.testByte;
			count += sizeof(byte);


			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
			count += sizeof(long);


			ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);

			count += sizeof(ushort);
			count += nameLen;


			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)skills.Count);
			count += sizeof(ushort);

			foreach (Skill skill in skills)
				success &= skill.Write(s, ref count);



			success &= BitConverter.TryWriteBytes(s, count);

			if (success == false)
				return null;

			return SendBufferHelper.Close(count);

		}
	}

	class Test
	{
		public int textInt;


		public void Read(ArraySegment<byte> segment)
		{
			ushort count = 0;

			ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

			count += sizeof(short);
			count += sizeof(short);


			this.textInt = BitConverter.ToInt32(s.Slice(count, s.Length - count));
			count += sizeof(int);

		}

		public ArraySegment<byte> Write()
		{
			ArraySegment<byte> segment = SendBufferHelper.Open(4096);

			bool success = true;
			ushort count = 0;

			Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

			count += sizeof(ushort);

			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.Test);
			count += sizeof(ushort);


			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.textInt);
			count += sizeof(int);


			success &= BitConverter.TryWriteBytes(s, count);

			if (success == false)
				return null;

			return SendBufferHelper.Close(count);

		}
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
                        
                        foreach (PlayerInfoReq.Skill skill in p.skills)
                        {
                            Console.WriteLine($"Skill ({skill.id}) ({skill.level}) ({skill.duration})");   
                        }
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
