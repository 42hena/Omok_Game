using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Omok_Game
{
    internal class CProtocol
    {
        public const Int16 PACKET_HEADER_SIZE = 5;
        public const int MAX_USER_ID_BYTE_LENGTH = 16;
        public const int MAX_USER_PW_BYTE_LENGTH = 16;

        long globalData;
        long prevData = 0;

        
        // Echo Test 목적일 뿐
        public void RecvEcho(ref CBuffer recvPacket)
        {
            long data;
            recvPacket.Read(out data);

            
            if (prevData == data)
            {
                Console.WriteLine("data:", data);
            }
            else
            {
                DebugBreak();
            }
        }

        public void RecvLogin(ref CBuffer recvPacket)
        {
            ulong accountNo;
            byte status;
            byte nickLen;

            recvPacket.Read(out accountNo);
            recvPacket.Read(out nickLen);
            byte[] nickname = new byte[nickLen * 2];
            recvPacket.Read(nickname, nickLen * 2);
            recvPacket.Read(out status);

            if (status > 0)
            {
                CClient.Instance.Login(accountNo, nickname);
                Console.WriteLine("Login !!!!!");
            }
            else
            {
                Console.WriteLine("Login FFFFF");
            }
        }

        public void RecvCreateRoom(ref CBuffer recvPacket)  // 202
        {
            ulong accountNo;
            ushort roomNo;
            //byte roomLen;
            //byte[] roomName;
            //byte status;

            bool flag = true;

            recvPacket.Read(out accountNo);
            recvPacket.Read(out roomNo);
            //recvPacket.Read(out roomLen);

            //roomName = new byte[roomLen];
            //recvPacket.Read(roomName, roomLen);
            //recvPacket.Read(out status);

            
            if (accountNo != CClient.Instance.GetAccountNo())
                flag = false;
            if (roomNo == 0)
                flag = false;

            if (flag)
            {
                Console.WriteLine("Create Room Success");
                CClient.Instance.CreateRoom(roomNo);
                Form1.Instance.AddRoomUserList(CClient.Instance.GetMyNickName());
                Form1.Instance.EnterUserAlaram(CClient.Instance.GetMyNickName());
            }
            else
            {
                Console.WriteLine("Create Room Fail");
            }
        }


        public void RecvEnterRoom(ref CBuffer recvPacket)   // 302
        {
            // *pResponsePacket << type << accountNo << roomNum << roomLen << roomName<< flag;

            Console.WriteLine("In RecvEnterRoom");

            ulong accountNo;
            ushort roomNo;
            //byte roomLen;
            //byte[] roomName;
            byte status;
            
            bool flag = true;

            recvPacket.Read(out accountNo);
            recvPacket.Read(out roomNo);
            //recvPacket.Read(out roomLen);
            //roomName = new byte[roomLen];
            //recvPacket.Read(roomName, roomLen);

            recvPacket.Read(out status);

            var str1 = $"{accountNo} {roomNo} {status}";
            //var str1 = $"{accountNo} {roomNo} {roomLen} {status}";
            Console.WriteLine(str1);

            if (accountNo != CClient.Instance.GetAccountNo())
            {
                Console.WriteLine("RecvEnterRoom Fail 1");
                flag = false;
            }
            if (roomNo == 0)
            {
                Console.WriteLine("RecvEnterRoom Fail 2");
                flag = false;
            }
            

            if (status > 0 && flag)
            {
                Console.WriteLine("RecvEnterRoom");
                CClient.Instance.CreateRoom(roomNo);
                Form1.Instance.AddRoomUserList(CClient.Instance.GetMyNickName());
                //Form1.Instance.EnterUserAlaram(CClient.Instance.GetMyNickName());
            }
        }


        public void RecvEnterRoomAlarm(ref CBuffer recvPacket)  // 303
        {
            ushort roomNo;
            byte nickLen;
            byte[] nickName;

            bool flag = true;

            recvPacket.Read(out roomNo);
            recvPacket.Read(out nickLen);

            nickName = new byte[nickLen * 2];
            recvPacket.Read(nickName, nickLen * 2);


            if (roomNo != CClient.Instance.GetCurrentRoom())
            {
                var str = $"{roomNo} {CClient.Instance.GetCurrentRoom()}";
                Console.WriteLine("Fail" +str);
                flag = false;
            }

            if (flag)
            {
                string nick = Encoding.Unicode.GetString(nickName);

                // UI 변경
                Form1.Instance.AddRoomUserList(nick);   // userList
                Form1.Instance.EnterUserAlaram(nick);   // chatting
                
                Console.WriteLine("Enter Room Alarm");
            }
            else
            {
                Console.WriteLine("Enter Room Alarm FFFFFFFFFF");
            }
        }

        public void RecvUserList(ref CBuffer recvPacket)    // 304
        {
            ulong accountNo;
            ushort roomNo;
            byte numOfPeople;
            byte nickLen;
            byte[] nickName;

            bool flag = true;

            recvPacket.Read(out accountNo);

            if (accountNo != CClient.Instance.GetAccountNo())
                flag = false;

            recvPacket.Read(out roomNo);
            recvPacket.Read(out numOfPeople);
            
            for (int i = 0; i < numOfPeople; ++i)
            {
                recvPacket.Read(out nickLen);
                nickName = new byte[nickLen * 2];
                recvPacket.Read(nickName, nickLen * 2);
                string nick = Encoding.Unicode.GetString(nickName);
                Form1.Instance.AddRoomUserList(nick);
            }
        }

        public void RecvPlayerList(ref CBuffer recvPacket)    // 305
        {
            ushort roomNo;
            byte numOfPeople;
            byte nickLen;
            byte[] nickName;

            //recvPacket.Read(out accountNo);

            //if (accountNo != CClient.Instance.GetAccountNo())
            //    flag = false;

            recvPacket.Read(out roomNo);
            recvPacket.Read(out numOfPeople);

            byte pos;
            byte readyFlag;
            // peopleCount, [nicklen nick]
            for (int i = 0; i < numOfPeople; ++i)
            {
                recvPacket.Read(out pos);
                recvPacket.Read(out readyFlag);
                recvPacket.Read(out nickLen);
                nickName = new byte[nickLen * 2];
                recvPacket.Read(nickName, nickLen * 2);
                string nick = Encoding.Unicode.GetString(nickName);

                if (pos == 1)
                {
                    Form1.Instance.ChangePositionBlack(nick);
                    if (readyFlag > 0)
                        Form1.Instance.ReadyBlackLabel();
                }
                else if (pos == 2)
                {
                    Form1.Instance.ChangePositionWhite(nick);
                    if (readyFlag > 0)
                        Form1.Instance.ReadyWhiteLabel();
                }
                else
                {
                    int a = 0;
                }
                
            }
        }

        public void RecvLeaveRoom(ref CBuffer recvPacket)   // 402
        {
            ulong accountNo;
            ushort roomNo;
            byte status;

            bool flag = true;

            recvPacket.Read(out accountNo);
            recvPacket.Read(out roomNo);
            recvPacket.Read(out status);

            if (accountNo != CClient.Instance.GetAccountNo())
            {
                flag = false;
                Console.WriteLine("Leave Room Account Error");
            }
            if (roomNo != CClient.Instance.GetCurrentRoom())
            {
                flag = false;
                Console.WriteLine("Leave Room NO Error");
            }
            //if (roomLen >= 20)
            //    flag = false;
            //if (Encoding.UTF8.GetString(roomName) != CClient.Instance.GetCurrentRoomName())
            //    flag = false;

            if (status > 0 && flag)
            {
                Form1.Instance.ClearBlackLabel();
                Form1.Instance.CancelBlackLabel();
                Form1.Instance.ClearWhiteLabel();
                Form1.Instance.CancelWhiteLabel();

                CClient.Instance.LeaveRoom();
                Console.WriteLine("Leave Room Success");

                Form1.Instance.ClearUserList();
                Form1.Instance.ClearChatList();
            }
            else
            {
                Console.WriteLine("Leave Room Fail");
            }
        }

        public void RecvLeaveRoomAlarm(ref CBuffer recvPacket)  // 403
        {
            ushort roomNo;
            byte nickLen;
            byte[] nickName;

            bool flag = true;

            recvPacket.Read(out roomNo);
            recvPacket.Read(out nickLen);

            nickName = new byte[nickLen * 2];
            recvPacket.Read(nickName, nickLen * 2);


            if (roomNo != CClient.Instance.GetCurrentRoom())
            {
                Console.WriteLine("RecvLeaveRoomAlarm NO Error");
                flag = false;
            }
            // UI 바꾸기 TODO
            if (flag)
            {
                string nick = Encoding.Unicode.GetString(nickName);// UTF8 -> Unicode
                Form1.Instance.EraseUserList(nick);
                Form1.Instance.LeaveUserAlaram(nick);

                string tmpNick = Form1.Instance.GetBlackName();
                if (nick == tmpNick)
                {
                    Form1.Instance.ClearBlackLabel();
                }
                tmpNick = Form1.Instance.GetWhiteName();
                if (nick == Form1.Instance.GetWhiteName())
                {
                    Form1.Instance.ClearWhiteLabel();
                }

            }
        }

        public void RecvChatting(ref CBuffer recvPacket)    // 502
        {
            ulong accountNo;    // 8
            ushort roomNo;      // 2
            byte nickLen;       // 1
            byte[] nickName;    // max 20
            byte chatLen;       // 1
            byte[] chating;     // 255

            recvPacket.Read(out accountNo);
            recvPacket.Read(out roomNo);

            recvPacket.Read(out nickLen);
            
            nickName = new byte[nickLen * 2];
            recvPacket.Read(nickName, nickLen*2);

            recvPacket.Read(out chatLen);
            
            chating = new byte[chatLen * 2];
            recvPacket.Read(chating, chatLen * 2);
            string nick = Encoding.Unicode.GetString(nickName);
            
            string chat = Encoding.Unicode.GetString(chating);

            // UI 바꾸기
            Form1.Instance.AddRoomChatMessageList(nick, chat);
        }

        public void RecvChangePlayerPosition(ref CBuffer recvPacket)
        {
            ulong accountNo;    // 8
            ushort roomNo;      // 2
            byte from;
            byte to;
            byte nickLen;       // 1
            byte[] nickName;    // max 20
            byte status;

            recvPacket.Read(out accountNo);
            recvPacket.Read(out roomNo);
            recvPacket.Read(out from); 
            recvPacket.Read(out to);
            
            recvPacket.Read(out nickLen);
            nickName = new byte[nickLen * 2];
            recvPacket.Read(nickName, nickLen * 2);
            recvPacket.Read(out status);

            if (status > 0)
            {
                string nick = Encoding.Unicode.GetString(nickName);
                // UI 변경.
                if (from == 1)
                {
                    Form1.Instance.ClearBlackLabel();
                    Form1.Instance.CancelBlackLabel();
                }
                else if (from == 2)
                {
                    Form1.Instance.ClearWhiteLabel();
                    Form1.Instance.CancelWhiteLabel();
                }

                if (to == 1)
                {
                    if (accountNo == CClient.Instance.GetAccountNo())
                        CClient.Instance.ChangePosition(to);
                    Form1.Instance.ChangePositionBlack(nick);
                }
                else if (to == 2)
                {
                    if (accountNo == CClient.Instance.GetAccountNo())
                        CClient.Instance.ChangePosition(to);
                    Form1.Instance.ChangePositionWhite(nick);
                }
            }
        }

        public void RecvChangeSpecPosition(ref CBuffer recvPacket)
        {
            ulong accountNo;    // 8
            ushort roomNo;      // 2
            byte from;
            byte to;
            byte nickLen;       // 1
            byte[] nickName;    // max 20
            byte status;

            recvPacket.Read(out accountNo);
            recvPacket.Read(out roomNo);
            recvPacket.Read(out from);
            recvPacket.Read(out to);

            recvPacket.Read(out nickLen);
            nickName = new byte[nickLen*2];
            recvPacket.Read(nickName, nickLen*2);
            recvPacket.Read(out status);
            if (status > 0)
            {
                // UI 변경
                if (from == 1)
                {
                    Form1.Instance.ClearBlackLabel();
                    Form1.Instance.CancelBlackLabel();

                }
                else if (from == 2)
                {
                    Form1.Instance.ClearWhiteLabel();
                    Form1.Instance.CancelWhiteLabel();
                }
                if (accountNo == CClient.Instance.GetAccountNo())
                    CClient.Instance.ChangePosition(3);
            }
        }

        
        public void RecvReady(ref CBuffer recvPacket)
        {
            // | type(2) | _accountNo(8) roomNo(2) position(1) flag(1)
            ulong accountNo;    // 8
            ushort roomNo;      // 2
            byte position;
            byte status;

            recvPacket.Read(out accountNo);
            recvPacket.Read(out roomNo);
            recvPacket.Read(out position);
            recvPacket.Read(out status);

            if (status > 0)
            {
                Console.WriteLine("RecvReady !!!");
                if (position == 1)
                {
                    Form1.Instance.ReadyBlackLabel();
                }
                else if (position == 2)
                {
                    Form1.Instance.ReadyWhiteLabel();
                }
                else
                    Console.WriteLine("RecvReady Fail !!!");
            }
            else
                Console.WriteLine("RecvReady Fail !!!");
        }

        public void RecvCancelReady(ref CBuffer recvPacket)
        {
            // | type(2) | _accountNo(8) roomNo(2) position(1) flag(1)
            ulong accountNo;    // 8
            ushort roomNo;      // 2
            byte position;
            byte status;

            recvPacket.Read(out accountNo);
            recvPacket.Read(out roomNo);
            recvPacket.Read(out position);
            recvPacket.Read(out status);

            if (status > 0)
            {
                Console.WriteLine("RecvCancelReady !!!");
                if (position == 1)
                {
                    Form1.Instance.CancelBlackLabel();
                }
                else if (position == 2)
                {
                    Form1.Instance.CancelWhiteLabel();
                }
                else
                    Console.WriteLine("RecvCancelReady Fail !!!");
            }
            else
                Console.WriteLine("RecvCancelReady Fail !!!");
        }

        public void RecvGameStart(ref CBuffer recvPacket)
        {
            // CClient 의 값만 바꾸자.
            //CClient.Instance.gameflag 변경
            ushort roomNo;      // 2

            recvPacket.Read(out roomNo);
            if (roomNo != CClient.Instance.GetRoomNo())
            {
                int a = 0;
            }
            if (CClient.Instance.GetGameFlag() == false)
            {
                CClient.Instance.GameStart();
                string nick = "공지사항";
                string chat = "게임 시작";
                Form1.Instance.AddRoomChatMessageList(nick, chat);
            }
            else
            {
                int a = 0;
            }

        }

        public void RecvPutStone(ref CBuffer recvPacket)
        {
            // | type(2) | accountNo(8) roomNo(2) position(1) x(1) y(1) flag(1)
            ulong accountNo;    // 8
            ushort roomNo;      // 2
            byte position;
            byte x;
            byte y;
            byte status;
            
            recvPacket.Read(out accountNo);
            recvPacket.Read(out roomNo);
            recvPacket.Read(out status);
            recvPacket.Read(out x);
            recvPacket.Read(out y);
            recvPacket.Read(out position);

            if (status > 0)
            {
                var str = $"position {position} : {x}, {y}";
                Console.WriteLine("RecvPutStone Success !!!" + str);
                Form1.Instance.Putstones(x, y);
                
            }
            else
                Console.WriteLine("RecvPutStone Fail !!!");

        }

        public void RecvGameOver(ref CBuffer recvPacket)
        {
            // | type(2) | accountNo(8) roomNo(2) position(1) x(1) y(1) flag(1)
            ushort roomNo;      // 2
            byte endflag;
            byte nickLen;
            
            recvPacket.Read(out roomNo);
            recvPacket.Read(out endflag);
            byte[] nick1;
            byte[] nick2;
            recvPacket.Read(out nickLen);
            nick1 = new byte[nickLen * 2];
            recvPacket.Read(nick1, nickLen * 2);

            recvPacket.Read(out nickLen);
            nick2 = new byte[nickLen * 2];
            recvPacket.Read(nick2, nickLen * 2);

            if (endflag >= 1 && endflag <= 3)
            {
                string announcement = "공지";
                if (endflag == 1)
                {
                    string nick = Encoding.Unicode.GetString(nick1);
                    Form1.Instance.AddRoomChatMessageList(announcement, nick + " Win");
                }
                else if (endflag == 2)
                {
                    string nick = Encoding.Unicode.GetString(nick1);
                    Form1.Instance.AddRoomChatMessageList(announcement, nick + " Win");
                }
                else if (endflag == 3)
                {
                    Form1.Instance.AddRoomChatMessageList(announcement, "Draw");
                }
                Form1.Instance.CancelBlackLabel();
                Form1.Instance.CancelWhiteLabel();
                CClient.Instance.GameOver();
                CClient.Instance.ChangeModeCancelReady();
            }
            else
            {
                int a = 0;
            }

            CClient.Instance.GameOver();
            byte curPosition = CClient.Instance.GetPosition();
            if (curPosition == 1 || curPosition == 2)
            {
                CClient.Instance.ChangeModeCancelReady();
            }
        }

        private void DebugBreak()
        {
            throw new NotImplementedException();
        }
    }
    
    public enum PACKET_ID : ushort
    {

    }
}
