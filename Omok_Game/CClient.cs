using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omok_Game
{
    internal class CClient
    {
        private static CClient _instance;
        
        UInt64 _accountNo = 0;
        string? _nickName = "";
        ushort _roomNo = 0;
        string? _roomName = "";
        byte _position = 0;
        //bool _loginFlag = false;
        bool _gameFlag = false;
        bool _readyFlag = false;

        public static CClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CClient();
                }
                return _instance;
            }
        }

        private CClient()
        {
            _accountNo = 0;
            _nickName = "";
            _roomNo = 0;
            _roomName = "";
            _position = 0;
        }

        public UInt64 GetAccountNo()
        {
            return _accountNo;
        }

        public string GetMyNickName()
        {
            return _nickName;
        }

        public ushort GetCurrentRoom()
        {
            return _roomNo;
        }

        public string GetCurrentRoomName()
        {
            return _roomName;
        }

        public byte GetPosition()
        {
            return _position;
        }

        public void GameStart()
        {
            _gameFlag = true;
        }
        public void GameOver()
        {
            _gameFlag = false;
        }
        public bool GetGameFlag()
        {
            return _gameFlag;
        }
        public void Login(UInt64 accountNo, byte[] nickname)
        {
            _accountNo = accountNo;
            _nickName = Encoding.Unicode.GetString(nickname);
        }

        public void CreateRoom(ushort roomNo)
        {
            _roomNo = roomNo;
            //_roomName = Encoding.UTF8.GetString(roomname);
            _position = 3;
        }

        public bool IsReady()
        {
            return _readyFlag;
        }

        public void ChangeModeReady()
        {
            _readyFlag = true; ;
        }
        public void ChangeModeCancelReady()
        {
            _readyFlag = false; ;
        }

        public ushort GetRoomNo()
        {
            return _roomNo;
        }

        public void EnterRoom(ushort roomNo, byte[] roomname)
        {
            _roomNo = roomNo;
            _roomName = Encoding.UTF8.GetString(roomname);
            _position = 3;
        }

        public void LeaveRoom()
        {
            _roomNo = 0;
            _roomName = "";
            _position = 0;
        }


        public void InfoClear()
        {
            _accountNo = 0;
            _nickName = "";
            _roomNo = 0;
            _roomName = "";
            _position = 0;
        }

        public void ChangePosition(byte to)
        {
            _position = to;
        }
    }


}
