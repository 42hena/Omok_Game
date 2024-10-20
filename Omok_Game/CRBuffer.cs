using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Omok_Game
{
    internal class CRBuffer
    {
        private readonly object _lock = new object();

        int _bufferSize;
        int _writePos;
        int _readPos;
        byte[] _buffer;

        public CRBuffer() { 
            
            _bufferSize = 2048;
            _writePos = 0;
            _readPos = 0;
            _buffer = new byte[_bufferSize];
        }

        public int DirectEnqueueSize()
        {
            lock (_lock)
            {
                if (_writePos < _readPos)
                    return _readPos - _writePos - 1;
                //return _readPos - _writePos;
                else
                {
                    if (_readPos == 0)
                        return (_bufferSize - _writePos) - 1;
                    else
                        return (_bufferSize - _writePos);
                }
            }
        }

        // 수정함.
        public int DirectDequeueSize()
        {
            lock (_lock)
            {
                if (_writePos >= _readPos)   //  _writePos > _readPos -> _writePos >= _readPos
                    return _writePos - _readPos;
                else
                    return (_bufferSize - _readPos);
            }
        }

        public int MoveWritePos(int writeSize)
        {
            lock (_lock)
            {
                //if (_writePos + writeSize > _bufferSize)
                //    _writePos = (_writePos + writeSize) % _bufferSize;
                //else
                //    _writePos += writeSize;
                //return writeSize;
                //int freeSize = GetFreeSize();
                //writeSize = int.Min(writeSize, freeSize);   // 작은 놈 선택.
                //if (_writePos + writeSize >= _bufferSize) // > -> >= 변경.
                //    _writePos = (_writePos + writeSize) % _bufferSize;
                //else
                //    _writePos += writeSize;
                //return writeSize;
                int freeSize = GetFreeSize(); 
                if (writeSize <= freeSize)
                {
                    if (_writePos + writeSize < _bufferSize)
                    {
                        _writePos += writeSize;
                    }
                    else
                    {
                        int remain = _writePos + writeSize - _bufferSize;
                        _writePos = remain;
                    }
                    return writeSize;
                }
                return 0;
            }
        }
        public int MoveReadPos(int readSize)
        {
            lock (_lock)
            {
                //if (_readPos + readSize > _bufferSize)
                //    _readPos = (_readPos + readSize) % _bufferSize;
                //else
                //    _readPos += readSize;
                //return readSize;
                //int useSize = GetUseSize();
                //readSize = int.Min(readSize, useSize);

                //if (_readPos + readSize >= _bufferSize) // > -> >=
                //    _readPos = (_readPos + readSize) % _bufferSize;
                //else
                //    _readPos += readSize;
                //return readSize;

                int useSize = GetUseSize();
                if (readSize <=useSize)
                {
                    if (_readPos + readSize < _bufferSize)
                    {
                        _readPos += readSize;
                    }
                    else
                    {
                        int remain = _readPos + readSize - _bufferSize;
                        _readPos = remain;
                    }
                    return readSize;
                }
                else
                {
                    return 0;
                }

                if (_readPos + readSize >= _bufferSize) // > -> >=
                    _readPos = (_readPos + readSize) % _bufferSize;
                else
                    _readPos += readSize;
                return readSize;
            }
        }
        public int Enqueue(byte[] src, int writeSize)
        {
            lock (_lock)
            {
                // 말 안되는 케이스
                if (writeSize <= 0 || writeSize > _bufferSize)
                    return -1;

                if (((_writePos + 1) % _bufferSize) == _readPos)
                    return 0;

                // maxSize
                //if (_writePos >= _readPos)
                int freeSize = GetFreeSize();
                if (freeSize >= writeSize)
                {
                    int eqSize = DirectEnqueueSize();
                    if (eqSize >= writeSize)
                    {
                        Buffer.BlockCopy(src, 0, _buffer, _writePos, writeSize);
                    }
                    else
                    {
                        Buffer.BlockCopy(src, 0, _buffer, _writePos, eqSize);
                        Buffer.BlockCopy(src, eqSize, _buffer, 0, writeSize - eqSize);
                    }
                    return MoveWritePos(writeSize);
                    //if (_readPos <= _writePos)
                    //{
                    //    remainWriteSize = _readPos + _bufferSize - _writePos - 1;
                    //    {
                    //        int a = 0;
                    //    }
                    //    remainWriteSize = int.Min(remainWriteSize, writeSize);
                        
                    //    if (_writePos + remainWriteSize >= _bufferSize)
                    //    {
                            
                    //        MoveWritePos(remainWriteSize);
                    //    }
                    //    else
                    //    {
                    //        Buffer.BlockCopy(src, 0, _buffer, _writePos, remainWriteSize);
                    //        MoveWritePos(remainWriteSize);
                    //    }
                    //    return remainWriteSize;

                        //if (_writePos + remainWriteSize >= _bufferSize)
                        //{
                        //    //writeSize - (_bufferSize - _writePos);
                        //    Buffer.BlockCopy(src, 0, _buffer, _writePos, (_bufferSize - _writePos));
                        //    Buffer.BlockCopy(src, 0, _buffer, 0, remainWriteSize - (_bufferSize - _writePos));

                        //    _writePos = (_writePos + remainWriteSize) % _bufferSize;
                        //    //MoveWritePos(remainWriteSize);
                        //}
                        //else
                        //{
                        //    Buffer.BlockCopy(src, 0, _buffer, _writePos, remainWriteSize);
                        //    _writePos += remainWriteSize;
                        //    //MoveWritePos(remainWriteSize);
                        //}
                    //}
                    //else
                    //{
                    //    int directEnSize = DirectEnqueueSize();
                    //    directEnSize = int.Min(directEnSize, writeSize);
                    //    Buffer.BlockCopy(src, 0, _buffer, _writePos, directEnSize);
                    //    MoveWritePos(directEnSize);
                    //    return directEnSize;
                    //    //remainWriteSize = _readPos - _writePos - 1;

                    //    //remainWriteSize = int.Min(remainWriteSize, writeSize);

                    //    //Buffer.BlockCopy(src, 0, _buffer, _writePos, remainWriteSize);
                    //    //_writePos += remainWriteSize;
                    //    //MoveWritePos(remainWriteSize);
                    //}
                }
                else
                {
                    return -1;
                }


                //return remainWriteSize;
            }
        }

        public int Peek(byte[] dst, int readSize)   // Dequeue에서 Move만 안한거.
        {
            lock (_lock)
            {
                // 말 안되는 케이스
                if (readSize <= 0 || readSize > _bufferSize)
                    return -1;

                int useSize = GetUseSize();
                if (useSize >= readSize)
                {
                    int dqSize = DirectDequeueSize();
                    if (dqSize < readSize)
                    {
                        Buffer.BlockCopy(_buffer, _readPos, dst, 0, dqSize);
                        Buffer.BlockCopy(_buffer, 0, dst, dqSize, readSize - dqSize);
                    }
                    else
                    {
                        Buffer.BlockCopy(_buffer, _readPos, dst, 0, readSize);
                    }
                    return readSize;
                }
                else
                {
                    return -1;
                }
                //int remainReadSize = 0;
                //if (_writePos >= _readPos)
                //{
                //    //remainReadSize = int.Min(_writePos - _readPos, readSize);
                //    int dqSize = DirectDequeueSize();
                //    remainReadSize = int.Min(dqSize, readSize);
                //    Buffer.BlockCopy(_buffer, _readPos, dst, 0, remainReadSize);
                //}
                //else
                //{
                //    int totalSize = GetUseSize();

                //    if (totalSize != (_bufferSize - _readPos + _writePos))
                //    {
                //        int a = 0;
                //    }
                //    remainReadSize = int.Min(totalSize, readSize);
                //    int dqSize = DirectDequeueSize();
                //    int front = int.Min(remainReadSize, dqSize);
                //    Buffer.BlockCopy(_buffer, _readPos, dst, 0, front);

                //    Buffer.BlockCopy(_buffer, 0, dst, remainReadSize, remainReadSize - front);

                //    //Buffer.BlockCopy(_buffer, _readPos, dst, 0, (_bufferSize - _readPos));
                //    //Buffer.BlockCopy(_buffer, _readPos, dst, (_bufferSize - _readPos), remainReadSize - (_bufferSize - _readPos));
                //}

                //return remainReadSize;
            }
        }
        public int Dequeue(byte[] dst, int readSize)
        {
            lock (_lock)
            {
                int peekSize = Peek(dst, readSize);
                MoveReadPos(readSize);
                return peekSize;
            }
        }


        // 확정.
        public int GetUseSize()
        {
            lock (_lock)
            {
                // front <= rear
                if (_readPos <= _writePos)
                {
                    // rear - front
                    return _writePos - _readPos;
                }
                else
                {
                    return (_bufferSize - _readPos) + _writePos;
                }
            }
        }
        public int GetFreeSize()
        {
            lock (_lock)
            {
                return _bufferSize - 1 - GetUseSize();
            }
        }
    }
}
