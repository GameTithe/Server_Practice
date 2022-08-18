﻿using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];

            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);



            while (true)
            {
                //휴대폰 설정
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                //문지기한테 입장 문의
                socket.Connect(endPoint);
                Console.WriteLine($"Connected To {socket.RemoteEndPoint.ToString()}");

                //보낸다
                for (int i = 0; i < 5; i++)
                {
                    byte[] sendBuff = Encoding.UTF8.GetBytes($"HelloWorld {i}");
                    int sendBytes = socket.Send(sendBuff);
                }

                //받는다
                byte[] recvBuff = new byte[1024];
                int recvBytes = socket.Receive(recvBuff);
                string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                Console.WriteLine($" [From Server] : {recvBuff}");

                //나간다
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();

                Thread.Sleep(100);

            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Client Error : {e.ToString()}");
        }
    }
}