using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    class Listener
    {
        Socket _listenSocket;
        Action<Socket> _OnAcceptHandler;

        public void Init(EndPoint endPoint, Action<Socket> OnAcceptHandler)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
           
            _OnAcceptHandler = OnAcceptHandler;

            _listenSocket.Bind(endPoint);
            _listenSocket.Listen(10);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += OnAcceptComplete;
            
            RegisterAccept(args);

        }

        public void RegisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;
            
            bool pending = _listenSocket.AcceptAsync(args);
            if (pending == false)
                OnAcceptComplete(null, args);

        }

        public void OnAcceptComplete(object sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                _OnAcceptHandler.Invoke(args.AcceptSocket);
            }
            else
            {
                Console.WriteLine($"Failed Accept : {args.SocketError.ToString()}");
            }

            RegisterAccept(args);
        Ss}
        public Socket Accept()
        {
            return _listenSocket.Accept();
        }
    }
}
