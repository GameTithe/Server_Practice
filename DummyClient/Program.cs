using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            // DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAdr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAdr, 7777);

            //휴대폰 설정
            Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // 문지기한테 입장 문의
            socket.Connect(endPoint);
            Console.WriteLine($"Conneted To {socket.RemoteEndPoint.ToString()}");

            //보낸다
            byte[] sendBuff = Encoding.UTF8.GetBytes("Hello World!");
            int sendBytes = socket.Send(sendBuff);

            //받는다
            byte[] recvBuff = new byte[1024];
            int recvBytes = socket.Receive(recvBuff);
            string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
            Console.WriteLine($"[From Server] : {recvData}");

            // 나간다
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
        catch(Exception e)
        {
            Console.WriteLine(e.ToString());
        }
       
    }
}