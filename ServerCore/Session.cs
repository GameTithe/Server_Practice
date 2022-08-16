using System.Net.Sockets;
using System.Text;

class Session
{
    Socket _socket;

    public void Start(Socket socket)
    {
        _socket = socket;

        SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
        recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvComplete);
        recvArgs.SetBuffer(new byte[1024] , 0, 1024);

        RegisterRecv(recvArgs);
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
            OnRecvComplete(null, args);
    }

    void OnRecvComplete(object sender, SocketAsyncEventArgs args)
    {
        if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
        {
            //TODO
            try
            {
                string recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                Console.WriteLine($" [From Client] : {recvData} ");
                RegisterRecv(args);
            }
            catch(Exception e)
            {
                Console.WriteLine($"OnRecvCompleted Failed {e}");
            }
        }

        else
        {
            //DISCONNCET
        }
    
    }
    #endregion

}