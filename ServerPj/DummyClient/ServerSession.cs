using System;
using ServerCore;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClient
{ 
	class ServerSession : Session
    {
        public override void OnConnect(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected {endPoint}");

            PlayerInfoReq packet = new PlayerInfoReq() { playerId = 1001, name = "ABCD" };
			var skill = new PlayerInfoReq.Skill() { id = 101, level = 1, duration = 3.0f };
			skill.attributes.Add(new PlayerInfoReq.Skill.Attribute() { att = 7 });
			packet.skills.Add(skill);
            packet.skills.Add(new PlayerInfoReq.Skill() { id = 201, level = 2, duration = 3.0f});
            packet.skills.Add(new PlayerInfoReq.Skill() { id = 301, level = 3, duration = 3.0f});
            packet.skills.Add(new PlayerInfoReq.Skill() { id = 401, level = 4, duration = 3.0f});

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
