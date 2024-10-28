using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Omok_Game
{
    internal class CBuffer
    {
        int _bufferSize = 0;
        int _readPos = 0;
        int _writePos = 0;
        int _headerSize = 0;

        byte[] _buffer;

        public CBuffer()
        {
            _bufferSize = 1024;
            _headerSize = 2;
            _writePos = _headerSize;
            _readPos = _headerSize;
            _buffer = new byte[_bufferSize];
        }

        public byte[] ToArray()
        {
            return _buffer;
        }

        public int GetUseSize()
        {
            return _writePos - _headerSize;
        }
        public int GetPacketCode()
        {
            return _buffer[0];
        }
        public Int16 GetPacketLength()
        {
            return BitConverter.ToInt16(_buffer, 0);
        }
        public Int16 GetRK()
        {
            return _buffer[3];
        }
        public Int16 GetCheckSum()
        {
            return _buffer[4];
        }

        // 직렬화 버퍼에 쓰는 거임.
        public bool Write(byte[] data, int writeSize)
        {
            // pass
            if (data == null)
            {
                return false;
            }

            if (_writePos + writeSize >= _bufferSize)
            {
                return false;
            }

            // src, srcoffset, dst, dstOffset, count
            Buffer.BlockCopy(data, 0, _buffer, _writePos, writeSize);
            _writePos += writeSize;

            return true;
        }

        public bool Write(byte data)
        {
            if (_writePos + 1 >= _bufferSize)
            {
                return false;
            }

            _buffer[_writePos] = data;
            _writePos += 1;

            return true;
        }

        // 2바이트(short)를 사용하는 Write 메서드

        public bool Write(en_PacketType type)
        {
            if (_writePos + sizeof(ushort) >= _bufferSize)
            {
                return false;
            }

            byte[] temp = BitConverter.GetBytes((ushort)type);
            return Write(temp, temp.Length);
        }

        public bool Write(ushort data)
        {
            if (_writePos + sizeof(ushort) >= _bufferSize)
            {
                return false;
            }

            byte[] temp = BitConverter.GetBytes(data);
            return Write(temp, temp.Length);
        }
        public bool Write(short data)
        {
            if (_writePos + sizeof(short) >= _bufferSize)
            {
                return false;
            }

            byte[] temp = BitConverter.GetBytes(data);
            return Write(temp, temp.Length);
        }

        public bool Write(int data)
        {
            if (_writePos + sizeof(int) >= _bufferSize)
            {
                return false;
            }

            byte[] temp = BitConverter.GetBytes(data);
            return Write(temp, temp.Length);
        }

        public bool Write(long data)
        {
            if (_writePos + sizeof(long) >= _bufferSize)
            {
                return false;
            }

            byte[] temp = BitConverter.GetBytes(data);
            return Write(temp, temp.Length);
        }

        public bool Read(byte[] buffer, int count)
        {
            if (count <= 0)
            {
                return false;
            }

            if (_readPos + count >= _bufferSize)
            {
                return false; // Not enough data to read
            }

            Array.Copy(_buffer, _readPos, buffer, 0, count);
            _readPos += count;
            return true;
        }
        public bool Read(out byte data)
        {
            if (_readPos + sizeof(byte) >= _bufferSize)
            {
                data = 0;
                return false;
            }

            data = _buffer[_readPos];
            _readPos += sizeof(byte);
            return true;
        }

        public bool Read(out short data)
        {
            if (_readPos + sizeof(short) >= _bufferSize)
            {
                data = 0;
                return false;
            }

            data = BitConverter.ToInt16(_buffer, _readPos);
            _readPos += sizeof(short);
            return true;
        }

        public bool Read(out en_PacketType data)
        {
            if (_readPos + sizeof(ushort) >= _bufferSize)
            {
                data = 0;
                return false;
            }

            //data = BitConverter.ToUInt16(_buffer, _readPos);
            ushort value = BitConverter.ToUInt16(_buffer, _readPos);
            data = (en_PacketType)value; // enum으로 캐스팅
            _readPos += sizeof(ushort);
            return true;
        }

        public bool Read(out ushort data)
        {
            if (_readPos + sizeof(ushort) >= _bufferSize)
            {
                data = 0;
                return false;
            }

            data = BitConverter.ToUInt16(_buffer, _readPos);
            _readPos += sizeof(ushort);
            return true;
        }

        public bool Read(out uint data)
        {
            if (_readPos + sizeof(uint) >= _bufferSize)
            {
                data = 0;
                return false;
            }

            data = BitConverter.ToUInt32(_buffer, _readPos);
            _readPos += sizeof(uint);
            return true;
        }

        public bool Read(out long data)
        {
            if (_readPos + sizeof(long) >= _bufferSize)
            {
                data = 0;
                return false;
            }

            data = BitConverter.ToInt64(_buffer, _readPos);
            _readPos += sizeof(long);
            return true;
        }

        public bool Read(out ulong data)
        {
            if (_readPos + sizeof(ulong) >= _bufferSize)
            {
                data = 0;
                return false;
            }

            data = BitConverter.ToUInt64(_buffer, _readPos);
            _readPos += sizeof(ulong);
            return true;
        }

        public ArraySegment<byte> Read()
        {
            var enableReadSize = _readPos - _headerSize;

            // 읽을 게 0임
            if (enableReadSize <= 0)
            {
                return new ArraySegment<byte>();
            }


            var packetDataSize = BitConverter.ToInt16(_buffer, _readPos);
            if (enableReadSize < packetDataSize)
            {
                return new ArraySegment<byte>();
            }

            var completePacketData = new ArraySegment<byte>(_buffer, _readPos, packetDataSize);
            _readPos += packetDataSize;
            return completePacketData;
        }


        public void Clear()
        {
            _writePos = _headerSize;
            _readPos = _headerSize;
        }

        public void SetHeader()
        {
            byte[] bytes = BitConverter.GetBytes(_writePos - _headerSize);
            Buffer.BlockCopy(bytes, 0, _buffer, 0, 2);
        }

        public int MoveReadPos(int moveSize)
        {
            if (_readPos + moveSize >= _bufferSize)
                return 0;
            _readPos += moveSize;
            return moveSize;
        }

        public int MoveWritePos(int moveSize)
        {
            if (_writePos + moveSize >= _bufferSize)
                return 0;
            _writePos += moveSize;
            return moveSize;
        }

    }
}
