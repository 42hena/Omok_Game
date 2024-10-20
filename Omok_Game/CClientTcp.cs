using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Omok_Game
{
    internal class CClientTcp
    {
        public Socket serverSock = null;
        public string errMsg;

        public CRBuffer _recvBuffer = new CRBuffer();
        public CRBuffer _sendBuffer = new CRBuffer();


        public bool ServerConnect(string ip, ushort  port)
        {
            try
            {
                IPAddress serverIP = IPAddress.Parse(ip);
                int serverPort = port;

                serverSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSock.Blocking = false;
                serverSock.Connect(new IPEndPoint(serverIP, serverPort));

                if (serverSock == null || serverSock.Connected == false)
                {
                    return false;
                }

                return true;
            }
            catch (SocketException ex)
            {
                // 소켓 에러가 InProgress이면 연결이 진행 중인 상태일 수 있음
                if (ex.SocketErrorCode != SocketError.WouldBlock && ex.SocketErrorCode != SocketError.InProgress)
                {
                    // 다른 에러가 발생한 경우는 연결 실패로 간주
                    return false;
                }
            }

            // 연결이 진행 중인 경우, 소켓의 상태를 Select를 통해 확인
            int timeoutMs = 5000;
            int elapsedMs = 0;
            int sleepIntervalMs = 100;

            // 연결이 완료될 때까지 반복적으로 확인
            while (!serverSock.Poll(0, SelectMode.SelectWrite))
            {
                // 일정 시간 동안 대기
                System.Threading.Thread.Sleep(sleepIntervalMs);
                elapsedMs += sleepIntervalMs;

                // 타임아웃 초과 시 실패로 간주
                if (elapsedMs >= timeoutMs)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsConnected()
        {
            if (serverSock != null)
                return true;
            return false;
        }

        public void Close()
        {
            if (serverSock != null && serverSock.Connected)
            {
                serverSock.Close();
                serverSock = null;
            }
        }

        public bool Receive()
        {

            try
            {
                byte[] ReadBuffer = new byte[2048];
                var nRecv = serverSock.Receive(ReadBuffer, 0, ReadBuffer.Length, SocketFlags.None);
                if (nRecv == 0)
                {
                    return false;
                }

                _recvBuffer.Enqueue(ReadBuffer, nRecv);

                return true;
            }
            catch (SocketException se)
            {
                errMsg = se.Message;
            }

            return false;
        }

        //스트림에 쓰기
        public void Send()
        {
            try
            {
                if (serverSock != null && serverSock.Connected) //연결상태 유무 확인
                {
                    CBuffer buffer = new CBuffer();


                    int sendUseSize = _sendBuffer.GetUseSize();
                    _sendBuffer.Dequeue(buffer.ToArray(), sendUseSize);


                    buffer.MoveWritePos(sendUseSize - 5);

                    int Test = buffer.GetUseSize();
                    serverSock.Send(buffer.ToArray(), 0, Test + 5, SocketFlags.None);
                }
                else
                {
                    errMsg = "먼저 채팅서버에 접속하세요!";
                }
            }
            catch (SocketException se)
            {
                errMsg = se.Message;
            }
        }
    }
}
