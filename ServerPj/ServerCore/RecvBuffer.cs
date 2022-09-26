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

        public RecvBuffer(int size)
        {
            _buffer = new ArraySegment<byte>(new byte[size], 0, size);
        }

        public int DataSize { get { return _writePos - _readPos; } }
        public int FreeSIze { get { return _buffer.Count - _writePos; } }


        public ArraySegment<byte> ReadSegment
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset, DataSize); }
        }

        public ArraySegment<byte> WriteSegment
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset, FreeSIze); }
        }
        // 읽기
        public bool OnRead(int num)
        {
            if (num < 0 || num > DataSize)
                return false;

            _readPos += num;

            return true;
        }
        //쓰기
        public bool OnWrite(int num)
        {
         
            if (num < 0 || num > FreeSIze)
                return false;

            _writePos += num;

            return true;
        }

        public void Clean()
        {
            if (DataSize == 0)
            {
                _readPos = _writePos = 0;
            }

            else
            {
                Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, DataSize);
                _readPos = 0;
                _writePos = DataSize;
            }
        }



        //    ArraySegment<byte> _buffer;
        //    int _readPos;
        //    int _writePos;

        //    public RecvBuffer(int bufferSize)
        //    {
        //        _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        //    }

        //    public int DataSize { get { return _writePos - _readPos; } }
        //    public int FreeSize { get { return _buffer.Count - _writePos; } }

        //    public ArraySegment<byte> ReadSegment
        //    {
        //        get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize); }
        //    }
        //    public ArraySegment<byte> WriteSegment
        //    {
        //        get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize); }
        //    }

        //    public void Clean()
        //    {
        //        int dataSize = DataSize;

        //        if (dataSize == 0)
        //        {
        //            _readPos = 0;
        //            _writePos = 0;
        //        }

        //        else
        //        {
        //            Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, dataSize);

        //            _readPos = 0;
        //            _writePos = dataSize;
        //        }
        //    }

        //    public bool OnRead(int numOfBytes)
        //    {
        //        if (numOfBytes > DataSize)
        //            return false;

        //        _readPos += numOfBytes;
        //        return true;
        //    }

        //    public bool OnWrite(int numOfBytes)
        //    {
        //        if (numOfBytes > FreeSize)
        //            return false;

        //        _writePos += numOfBytes;
        //        return true;
        //    }

        }
    }
