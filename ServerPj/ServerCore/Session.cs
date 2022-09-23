﻿using System;
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

        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();

        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        Queue<byte[]> _sendQueue = new Queue<byte[]>();


        object _lock = new object();
        int _disconnected = 0;

        public void Start(Socket socket)
        {
            _socket = socket;


            _recvArgs.Completed += OnRecvComplete;
            _sendArgs.Completed += OnSendComplete;

            _recvArgs.SetBuffer(new byte[1024], 0, 1024);
             
            RegisterRecv();
        }

        public void Send(byte[] sendBuff)
        {
            lock(_lock)
            {
                _sendQueue.Enqueue(sendBuff);

                if(_pendingList.Count == 0)
                    RegisterSend();
            }
        }

       
        public void Disconnect()
        {
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)
                return; 

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();

        }
        #region 네트워크 통신

        void RegisterRecv()
        {
            bool pending = _socket.ReceiveAsync(_recvArgs);
            if (pending == false)
                OnRecvComplete(null, _recvArgs);
        }

        void OnRecvComplete(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success && args.BytesTransferred > 0)
            {
                try
                {
                    string recvBytes = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                    Console.WriteLine($"[From Client] : {recvBytes}");
                    //RegisterRecv(args);
                }
                catch(Exception e)
                {
                    Console.WriteLine($"Onecv Error : {e.ToString}");
                }
            }
            else
            {
                Disconnect();

            }
        }


        void RegisterSend()
        {
            _sendArgs.BufferList = null;

            while (_sendQueue.Count > 0)
            {
                byte[] buffer = _sendQueue.Dequeue();
                _pendingList.Add(new ArraySegment<byte>(buffer, 0 , buffer.Length));   
            }

            _sendArgs.BufferList = _pendingList;
            
            bool pending = _socket.SendAsync(_sendArgs);
            if (pending == false)
                OnSendComplete(null, _sendArgs);

        }
        
        void OnSendComplete(object send , SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success && args.BytesTransferred > 0)
            {
                _sendArgs.BufferList = null;
                _pendingList.Clear();

                Console.WriteLine($"Trasnffered: {_sendArgs.BytesTransferred}");

                if (_sendQueue.Count > 0)
                    RegisterSend();
            }
            else
            {
                Disconnect();
            }
        }


        #endregion
    }
}
