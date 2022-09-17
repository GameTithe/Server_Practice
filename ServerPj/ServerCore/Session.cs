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

        SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
        Queue<byte[]> sendQueue = new Queue<byte[]>();


        bool _pending = false;
        object _lock = new object();
        int _disconnected = 0;

        public void Init(Socket socket)
        {
            _socket = socket;

            SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
            recvArgs.Completed += OnRecvComplete;

            sendArgs.Completed += OnSendComplete;

            recvArgs.SetBuffer(new byte[1024], 0, 1024);
            
            RegisterRecv(recvArgs);
        }

        public void Send(byte[] sendBuff)
        {
            lock(_lock)
            {
                sendQueue.Enqueue(sendBuff);

                if(_pending == false)
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

        void RegisterRecv(SocketAsyncEventArgs args)
        {
            bool pending = _socket.ReceiveAsync(args);
            if (pending == false)
                OnRecvComplete(null, args);
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
                //여기에 들어와 질 때가 있다>?? => Disconnet할 때
                //Disconnect();
            }
        }


        void RegisterSend()
        {
            _pending = true;

            byte[] sendBuffer = sendQueue.Dequeue();
            sendArgs.SetBuffer(sendBuffer, 0, sendBuffer.Length);

            bool pending = _socket.SendAsync(sendArgs);
            if (pending == false)
                OnSendComplete(null, sendArgs);

        }
        
        void OnSendComplete(object send , SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success && args.BytesTransferred > 0)
            {
                if (sendQueue.Count > 0)
                    RegisterSend();
                else
                    _pending = false;
            }
            else
            {
                Disconnect();
            }
        }


        #endregion
    }
}
