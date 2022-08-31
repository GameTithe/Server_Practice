using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class RecvBuffer
    {
        ArraySegment<byte> _buffer;
        int _readPos;
        int _writePos;

        public RecvBuffer(int buffersSize)
        {
            _buffer = new ArraySegment<byte>(new byte[buffersSize], 0, buffersSize);

        }

        public int DataSize { get { return _writePos - _readPos; } }
        public int FreeSize { get { return _buffer.Count - _writePos; } }

        public ArraySegment<byte> ReadSegment
        {   
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize); } 
        }
        public ArraySegment<byte> WriteSegment
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize); }
        }

        public void Clean()
        {
            int dataSize = DataSize;
            if(dataSize == 0)
            {
                //남은 데이터가 없으면 복사하지 않고 커서 위치만 리셋
                _readPos = _writePos = 0;
            }
            else
            {
                ArraySegment<byte> clone = new ArraySegment<byte>(_buffer.Array, _readPos, dataSize);
                _readPos = 0;

                //Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, dataSize);
                Array.Copy(clone.Array, clone.Offset, _buffer.Array, _buffer.Offset, clone.Count);

                _writePos = dataSize;
            }
        }

        public bool OnRead(int numOfBytes)
        {
            if (numOfBytes > DataSize)
                return false;

            //write보다 큰 것도 처리?
            _readPos += numOfBytes;
            return true;
        }
        
        public bool OnWrite(int numOfBytes)
        {
            if (numOfBytes > FreeSize)
                return false;
            
            // write + numofBytes > FreeSize 일 때는?
            _writePos += numOfBytes;
            return true;
        }
    }
}
