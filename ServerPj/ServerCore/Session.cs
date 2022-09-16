using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    class Session
    {
        Socket _socket;

        public void Init(Socket socket)
        {
            _socket = socket;

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += OnCompleteRecv;

            args.SetBuffer(new byte[1024], 0, 1024);

            RegisterRecv(args);
        }

        public void Send(byte[] sendBuff)
        {
            _socket.Send(sendBuff);
        }

        public void Disconnect()
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();

        }
        #region 네트워크 통신
        void RegisterRecv(SocketAsyncEventArgs args)
        {
            bool pending = _socket.ReceiveAsync(args);

            if (pending == false)
                OnCompleteRecv(null, args);
        }

        void OnCompleteRecv(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success && args.BytesTransferred > 0)
            {
                string recvBytes = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                Console.WriteLine($"[From Client] : {recvBytes}");

            }
            else
            {
                Console.WriteLine($"Recv error : {args.SocketError.ToString()}");
            }
        }
        #endregion
    }
}
