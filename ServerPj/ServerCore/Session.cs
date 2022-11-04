using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{

    public abstract class PacketSession : Session
    {
        public readonly int HeaderSize = 2;

        // [size(2)] [packetId(2)] 
        public sealed override int OnRecv(ArraySegment<byte> buffer)
        {
            int processLen = 0;

            while (true)
            {
                if (buffer.Count < HeaderSize)
                    break;

                int dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
                
                if (buffer.Count < dataSize)
                    break;

                OnRecvPacket(new ArraySegment<byte>(buffer.Array , buffer.Offset, dataSize));
                
                processLen += dataSize;
                buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
            }
            return processLen;
        }

        public abstract void OnRecvPacket(ArraySegment<byte> buffer);


    }
    public abstract class Session
    {
        Socket _socket;

        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();

        RecvBuffer _recvBuffer = new RecvBuffer(1024);


        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();


        object _lock = new object();
        int _disconnected = 0;

        
        public abstract void OnConnect(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisconnected(EndPoint endPoint);
        

        public void Start(Socket socket)
        {
            _socket = socket;

            _recvArgs.Completed += OnRecvComplete;
            _sendArgs.Completed += OnSendComplete;

            RegisterRecv();
        }

        public void Send(ArraySegment<byte> sendBuff)
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

            OnDisconnected(_socket.RemoteEndPoint);

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();

        }
        #region 네트워크 통신

        void RegisterRecv()
        {
            _recvBuffer.Clean();

            ArraySegment<byte> buffer = _recvBuffer.WriteSegment;
            _recvArgs.SetBuffer(buffer.Array, buffer.Offset, buffer.Count);

            bool pending = _socket.ReceiveAsync(_recvArgs);
            if (pending == false)
                OnRecvComplete(null, _recvArgs);


        //    _recvBuffer.Clean();

        //    ArraySegment<byte> segment = _recvBuffer.WriteSegment;
        //    _recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

        //    bool pending = _socket.ReceiveAsync(_recvArgs);
        //    if (pending == false)
        //        OnRecvComplete(null, _recvArgs);
        }

        void OnRecvComplete(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success && args.BytesTransferred > 0)
            {
                try
                {
                    //Wrtie 커서 이동
                    if (_recvBuffer.OnWrite(args.BytesTransferred) == false)
                    {
                        Disconnect();
                        return;

                    }

                    //컨텐츠 쪽으로 데이터를 넘겨주고 얼마나 처리했는지 받는다
                    int processLen = OnRecv(_recvBuffer.ReadSegment);
                    if (processLen < 0 || processLen < _recvBuffer.DataSize)
                    {
                        Disconnect();
                        return;
                    }

                    if (_recvBuffer.OnRead(processLen) == false)
                    {
                        Disconnect();
                        return;
                    }


                    RegisterRecv();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"OnRecv Error : {e.ToString}");
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
                ArraySegment<byte> buffer = _sendQueue.Dequeue();
                _pendingList.Add(buffer);   
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

                OnSend(_sendArgs.BytesTransferred);

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
