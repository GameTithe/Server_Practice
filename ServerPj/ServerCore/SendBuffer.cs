using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class SendBufferHelper
    {
        public static ThreadLocal<SendBuffer> CurrentBuffer = new ThreadLocal<SendBuffer>(() => { return null; });

        public static int chunkSize { get; set; } = 4096 * 100;

        public static ArraySegment<byte> Open(int reserveSize)
        {
            if (CurrentBuffer.Value == null)
                CurrentBuffer.Value = new SendBuffer(chunkSize);

            if (CurrentBuffer.Value.FreeSize < reserveSize)
                CurrentBuffer.Value = new SendBuffer(chunkSize);

            return CurrentBuffer.Value.Open(reserveSize);
        }
        public static ArraySegment<byte> Close(int usedSize)
        {
            return CurrentBuffer.Value.Close(usedSize);
        }

    }

    public class SendBuffer
    {
        ArraySegment<byte> _sendBuffer;
        int _usedSize;

        public int FreeSize { get { return _sendBuffer.Count - _usedSize; } }

        public SendBuffer(int chunkSize)
        {
            _sendBuffer = new ArraySegment<byte>(new byte[chunkSize], 0, chunkSize);
        }

        public ArraySegment<byte> Open(int reserveSize)
        {
            if (reserveSize > FreeSize)
                return null;

            return new ArraySegment<byte>(_sendBuffer.Array, _sendBuffer.Offset + _usedSize, reserveSize);
        }

        public ArraySegment<byte> Close(int usedSize)
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(_sendBuffer.Array, _sendBuffer.Offset + _usedSize, usedSize);

            _usedSize += usedSize;

            return segment;
        }
    }
}
