using System;
using System.Drawing.Printing;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

using Microsoft.VisualBasic.Devices;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Diagnostics;

//using PacketType;

namespace Omok_Game
{
    public partial class Form1 : Form
    {
        public static Form1 Instance { get; private set; }

        const ushort headerLen = 2;


        Graphics g;
        COmok omok;
        int boardSize = 15;
        char playType;
        char firstPlayers;

        System.Windows.Forms.Timer dispatcherUITimer;

        // 추가
        System.Threading.Thread NetworkReadThread = null;
        System.Threading.Thread NetworkSendThread = null;


        //PacketBufferManager PacketBuffer = new PacketBufferManager();
        Queue<PacketData> RecvPacketQueue = new Queue<PacketData>();
        Queue<byte[]> SendPacketQueue = new Queue<byte[]>();

        bool IsNetworkThreadRunning = false;


        CClientTcp clientTcp = new CClientTcp();

        struct PacketData
        {
            public byte code;
            public Int16 len;
            public byte rk;
            public byte checkSum;
            public byte[] data;
        }
        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();
        public Form1()
        {
            InitializeComponent();
            Instance = this;
            AllocConsole(); // 콘솔 창 열기

            // Create Double Bufferd Panel
            DoublePanel panel = new DoublePanel();

            panel.BackColor = System.Drawing.Color.Peru;
            panel.Location = new System.Drawing.Point(499, 20);
            panel.Name = "panel";
            panel.Size = new System.Drawing.Size(460, 460);
            panel.TabIndex = 0;
            panel.Enabled = true;

            panel.Paint += new PaintEventHandler(panel_Paint);
            panel.MouseDown += new MouseEventHandler(panel_MouseDown);


            Controls.Add(panel);

            panel.BringToFront();
            //CClient client = new CClient();
        }

        //private void panel_MouseDown(object sender, MouseEventArgs e)
        private void panel_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Console.WriteLine("Mouse Down");
            if (!CClient.Instance.GetGameFlag())
                return;

            Point boardPoint = omok.GetBoardPosition(e.Location.X, e.Location.Y);

            int bx = boardPoint.X;
            int by = boardPoint.Y;
            if (bx < 0 || bx >= omok.GetBoardSize() || by < 0 || by >= omok.GetBoardSize())
                return;



            ushort type = 1108;
            ulong accountNo = CClient.Instance.GetAccountNo();
            ushort roomNo = CClient.Instance.GetCurrentRoom();

            byte position = CClient.Instance.GetPosition();
            byte x = (byte)boardPoint.X;
            byte y = (byte)boardPoint.Y;


            CBuffer packet = new CBuffer();
            packet.Clear();

            // packet 합치기.
            packet.Write(type);
            packet.Write((long)accountNo);
            packet.Write(roomNo);
            packet.Write(position);
            packet.Write(x);
            packet.Write(y);


            packet.SetHeader();

            int useSize = packet.GetUseSize();

            // sendBuffer에 넣기.
            clientTcp._sendBuffer.Enqueue(packet.ToArray(), 2 + useSize);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            g = CreateGraphics();

            IsNetworkThreadRunning = true;
            NetworkReadThread = new System.Threading.Thread(this.NetworkReadProcess);
            NetworkReadThread.Start();
            NetworkSendThread = new System.Threading.Thread(this.NetworkSendProcess);
            NetworkSendThread.Start();

            //dispatcherUITimer = new System.Windows.Forms.Timer();
            //dispatcherUITimer.Tick += new EventHandler(BackGroundProcess);
            //dispatcherUITimer.Interval = 5100;
            //dispatcherUITimer.Start();



            omok = new COmok(g, boardSize, playType, firstPlayers);
        }

        void OnMessage(CProtocol proto, en_PacketType pacektType, CBuffer newPacket)
        {
            switch (pacektType)
            {
                case en_PacketType.LoginResponse:               // 2
                    proto.RecvLogin(ref newPacket);
                    break;
                case en_PacketType.CreateRoomResponse:          // 202
                    proto.RecvCreateRoom(ref newPacket);
                    break;
                case en_PacketType.EnterRoomResponse:           // 302
                    proto.RecvEnterRoom(ref newPacket);
                    break;
                case en_PacketType.EnterRoomBroadCastAlarm:     // 303
                    proto.RecvEnterRoomAlarm(ref newPacket);
                    break;
                case en_PacketType.EnterRoomGetRoomUserList:    // 304
                    proto.RecvUserList(ref newPacket);
                    break;
                case en_PacketType.EnterRoomGetRoomPlayerList:  // 305
                    proto.RecvPlayerList(ref newPacket);
                    break;
                case en_PacketType.LeaveRoomResponse:           // 402
                    proto.RecvLeaveRoom(ref newPacket);
                    break;
                case en_PacketType.LeaveRoomBroadCastAlarm:     // 403
                    proto.RecvLeaveRoomAlarm(ref newPacket);
                    break;
                case en_PacketType.ChatResponse:                // 502
                    proto.RecvChatting(ref newPacket);
                    break;

                case en_PacketType.PlayerResponse:              // 1003
                    proto.RecvChangePlayerPosition(ref newPacket);
                    break;
                case en_PacketType.SpectatorResponse:           // 1004
                    proto.RecvChangeSpecPosition(ref newPacket);
                    break;

                case en_PacketType.ReadyResponse:               // 1102
                    proto.RecvReady(ref newPacket);
                    break;

                case en_PacketType.CancelResponse:              // 1104
                    proto.RecvCancelReady(ref newPacket);
                    break;

                case en_PacketType.GameStart:                   // 1105
                    Console.WriteLine("Game Start");
                    proto.RecvGameStart(ref newPacket);
                    break;
                case en_PacketType.PlaceStoneResponse:          // 1109
                    proto.RecvPutStone(ref newPacket);
                    Console.WriteLine("en_PutStoneResponse X");
                    break;

                case en_PacketType.Record:  // 더미만 사용.
                    break;
                case en_PacketType.GameOver:
                    proto.RecvGameOver(ref newPacket);
                    Console.WriteLine("en_GameOverResponse X");
                    break;
                case en_PacketType.Echo:
                    proto.RecvEcho(ref newPacket);
                    break;
            }
        }

        void NetworkReadProcess()
        {
            const Int16 PacketHeaderSize = CProtocol.PACKET_HEADER_SIZE;
            CProtocol proto = new CProtocol();
            //Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //clientSocket.Blocking = false;


            while (true)
            {
                if (clientTcp.IsConnected() == false)
                    continue;

                List<Socket> readSockets = new List<Socket> { clientTcp.serverSock };
                List<Socket> writeSockets = null;
                List<Socket> errorSockets = new List<Socket> { clientTcp.serverSock };
                Socket.Select(readSockets, writeSockets, errorSockets, -1);
                if (readSockets.Count > 0)
                {
                    int headerSize = 2;
                    byte[] buffer = new byte[1000];
                    byte[] tmp = new byte[1000];
                    int bytesReceived = clientTcp.serverSock.Receive(buffer);
                    if (bytesReceived > 0)
                    {
                        int enqueueRecv = clientTcp._recvBuffer.Enqueue(buffer, bytesReceived);
                        if (enqueueRecv != bytesReceived)
                        {
                            Debugger.Break();
                        }
                        else
                        {
                            int rrr = clientTcp._recvBuffer.Peek(tmp, bytesReceived);
                            if (enqueueRecv != rrr)
                            {
                                Debugger.Break();
                            }
                            for (int i = 0; i < rrr; i++)
                            {
                                if (buffer[i] != tmp[i])
                                {
                                    Debugger.Break();
                                }
                            }
                            while (bytesReceived > 0)
                            {
                                if (bytesReceived < PacketHeaderSize)
                                    break;
                                CBuffer newPacket = new CBuffer();
                                int peekSize = clientTcp._recvBuffer.Peek(newPacket.ToArray(), PacketHeaderSize);
                                if (peekSize != PacketHeaderSize)
                                    break;
                                int uS = clientTcp._recvBuffer.GetUseSize();
                                int fS = clientTcp._recvBuffer.GetFreeSize();
                                int eQ = clientTcp._recvBuffer.DirectEnqueueSize();
                                int dQ = clientTcp._recvBuffer.DirectDequeueSize();
                                var ss = $"bytesReceived {bytesReceived}, {uS}, {fS}, {eQ}, {dQ}";
                                if (fS < eQ)
                                {
                                    Debugger.Break();
                                }
                                //Console.WriteLine(ss);
                                short packetSize = newPacket.GetPacketLength();
                                if (packetSize > 20000)
                                {
                                    Debugger.Break();
                                }
                                var sss = $"packet {packetSize}";
                                Console.WriteLine(sss);
                                if (packetSize + headerSize > bytesReceived)
                                {
                                    Console.WriteLine("break");
                                    break;
                                }

                                clientTcp._recvBuffer.Dequeue(newPacket.ToArray(), packetSize + headerSize);

                                // Parsing(newPacket);
                                //ushort type;
                                //newPacket.Read(out type);

                                en_PacketType pacektType;
                                newPacket.Read(out pacektType);

                                OnMessage(proto, pacektType, newPacket);

                                bytesReceived -= (packetSize + headerSize);
                            }
                            if (bytesReceived < 0)
                            {
                                Debugger.Break();
                            }
                        }
                    }
                    else if (bytesReceived == 0)
                    {
                        break;
                    }
                }
            }

        }

        void NetworkSendProcess()
        {
            while (true)
            {
                System.Threading.Thread.Sleep(1);

                if (clientTcp.IsConnected() == false)
                {
                    continue;
                }
                List<Socket> readSockets = null;
                List<Socket> writeSockets = null;
                List<Socket> errorSockets = null;

                if (clientTcp._sendBuffer.GetUseSize() > 0)
                {
                    writeSockets = new List<Socket> { clientTcp.serverSock };
                }
                if (readSockets == null && writeSockets == null && errorSockets == null)
                    continue;
                Socket.Select(readSockets, writeSockets, errorSockets, 0);

                if (writeSockets != null && writeSockets.Count() > 0)
                {
                    clientTcp.Send();
                }
            }
        }

        public void Putstones(int x, int y)
        {
            if (panel.InvokeRequired)
            {
                panel.Invoke(new MethodInvoker(delegate
                {
                    Putstones(x, y);
                }));
            }
            else
            {
                omok.PutStone(x, y);
                //omok.DrawStoneAll(g);
                Refresh();
            }
        }

        public void SetDisconnectd()
        {
            //if (btnConnect.Enabled == false)
            //{
            //    btnConnect.Enabled = true;
            //    btnDisconnect.Enabled = false;
            //}

            //SendPacketQueue.Clear();

            //listBoxRoomChatMsg.Items.Clear();
            //listBoxRoomUserList.Items.Clear();

            //labelStatus.Text = "서버 접속이 끊어짐";
        }


        internal class DoublePanel : System.Windows.Forms.Panel
        {
            internal DoublePanel()
            {
                DoubleBuffered = true;
            }
        }

        private void panel_Paint(object? sender, PaintEventArgs e)// ? 추가
        {
            omok.DrawBoard(e.Graphics);
            omok.DrawStoneAll(e.Graphics);
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            string address = serverIpBox.Text;
            ushort port = Convert.ToUInt16(serverPortBox.Text);

            if (clientTcp.ServerConnect(address, port))
            {
                labelStatus.Text = string.Format("{0}. 서버에 접속 중", DateTime.Now);
                connectButton.Enabled = false;
                closeButton.Enabled = true;
            }
            else
            {
                labelStatus.Text = string.Format("{0}. 서버에 접속 실패", DateTime.Now);
            }
        }

        private void echoButton_Click(object sender, EventArgs e)
        {
            ulong echoData = Convert.ToUInt64(echoBox.Text);
            ushort type = 60000;
            CBuffer packet = new CBuffer();
            packet.Clear();

            // packet 합치기.
            packet.Write(type);
            packet.Write((long)echoData);
            packet.SetHeader();

            // sendBuffer에 넣기.
            clientTcp._sendBuffer.Enqueue(packet.ToArray(), headerLen + packet.GetUseSize());
        }

        private void loginbutton_Click(object sender, EventArgs e)
        {
            en_PacketType packetType = en_PacketType.LoginRequest;
            ushort type = 1;
            byte nickLen;
            string nickName = nicknameBox.Text;
            nickLen = (byte)nickName.Length;
            ulong accountNo = Convert.ToUInt64(accountBox.Text);

            CBuffer packet = new CBuffer();
            packet.Clear();

            // packet 합치기.
            //packet.Write(type);
            packet.Write(packetType);
            packet.Write((long)accountNo);
            packet.Write(nickLen);
            //byte[] nickArray = Encoding.UTF8.GetBytes(nickName);
            byte[] nickArray = Encoding.Unicode.GetBytes(nickName);
            packet.Write(nickArray, nickArray.Length);

            packet.SetHeader();

            int useSize = packet.GetUseSize();

            // sendBuffer에 넣기.
            clientTcp._sendBuffer.Enqueue(packet.ToArray(), headerLen + useSize);
        }

        private void createRoombutton_Click(object sender, EventArgs e)
        {
            ushort type = 201;
            ulong accountNo = CClient.Instance.GetAccountNo();
            //byte roomLen;
            //string roomName = "testRoom";
            //roomLen = (byte)roomName.Length;

            CBuffer packet = new CBuffer();
            packet.Clear();

            // packet 합치기.
            packet.Write(type);
            packet.Write((long)accountNo);
            //packet.Write(roomLen);
            //byte[] roomByte = Encoding.UTF8.GetBytes(roomName);
            //packet.Write(roomByte, roomLen);

            packet.SetHeader();

            int useSize = packet.GetUseSize();

            // sendBuffer에 넣기.
            clientTcp._sendBuffer.Enqueue(packet.ToArray(), headerLen + useSize);
        }



        // =================================================

        public void LoginDisable()
        {
            if (loginbutton.InvokeRequired)
            {
                loginbutton.Invoke(new MethodInvoker(delegate
                {
                    LoginDisable();
                }));
            }
            else
            {
                loginbutton.Enabled = false;
            }
        }

        public void EnterRoomEnable1()
        {
            if (enterRoombutton.InvokeRequired)
            {
                enterRoombutton.Invoke(new MethodInvoker(delegate
                {
                    EnterRoomEnable1();
                }));
            }
            else
            {
                enterRoombutton.Enabled = true;
            }
        }
        public void EnterRoomEnable2()
        {
            if (enterRoombutton2.InvokeRequired)
            {
                enterRoombutton2.Invoke(new MethodInvoker(delegate
                {
                    EnterRoomEnable2();
                }));
            }
            else
            {
                enterRoombutton2.Enabled = true;
            }
        }

        public void EnterRoomDisable1()
        {
            if (enterRoombutton.InvokeRequired)
            {
                enterRoombutton.Invoke(new MethodInvoker(delegate
                {
                    EnterRoomDisable1();
                }));
            }
            else
            {
                enterRoombutton.Enabled = false;
            }
        }
        public void EnterRoomDisable2()
        {
            if (enterRoombutton2.InvokeRequired)
            {
                enterRoombutton2.Invoke(new MethodInvoker(delegate
                {
                    EnterRoomDisable2();
                }));
            }
            else
            {
                enterRoombutton2.Enabled = false;
            }
        }

        public void LeaveRoomEnable1()
        {
            if (leaveRoombutton.InvokeRequired)
            {
                leaveRoombutton.Invoke(new MethodInvoker(delegate
                {
                    LeaveRoomEnable1();
                }));
            }
            else
            {
                leaveRoombutton.Enabled = true;
            }
        }

        public void LeaveRoomEnable2()
        {
            if (leaveRoombutton2.InvokeRequired)
            {
                leaveRoombutton2.Invoke(new MethodInvoker(delegate
                {
                    LeaveRoomEnable2();
                }));
            }
            else
            {
                leaveRoombutton2.Enabled = true;
            }
        }

        public void LeaveRoomDisable1()
        {
            if (leaveRoombutton.InvokeRequired)
            {
                leaveRoombutton.Invoke(new MethodInvoker(delegate
                {
                    LeaveRoomDisable1();
                }));
            }
            else
            {
                leaveRoombutton.Enabled = false;
            }
        }

        public void LeaveRoomDisable2()
        {
            if (leaveRoombutton2.InvokeRequired)
            {
                leaveRoombutton2.Invoke(new MethodInvoker(delegate
                {
                    LeaveRoomDisable2();
                }));
            }
            else
            {
                leaveRoombutton2.Enabled = false;
            }
        }

        public void AddRoomUserList(string nickname)
        {
            var msg = $"{nickname}";

            if (userlistBox.InvokeRequired)
            {
                // 현재 스레드가 UI 스레드가 아니라면 Invoke를 사용하여 UI 스레드에서 이 메서드를 호출
                userlistBox.Invoke(new MethodInvoker(delegate
                {
                    AddRoomUserList(nickname);
                }));
            }
            else
            {
                userlistBox.Items.Add(nickname);
            }
        }

        public void AddRoomChatMessageList(string nickname, string message)
        {
            var msg = $"{nickname}:  {message}";

            if (chatlistBox.InvokeRequired)
            {
                // 현재 스레드가 UI 스레드가 아니라면 Invoke를 사용하여 UI 스레드에서 이 메서드를 호출
                chatlistBox.Invoke(new MethodInvoker(delegate
                {
                    AddRoomChatMessageList(nickname, message);
                }));
            }
            else
            {
                chatlistBox.Items.Add(msg);
                //chatlistBox.SelectedIndex = chatlistBox.Items.Count - 1;
            }
        }


        public void ClearUserList()
        {
            if (userlistBox.InvokeRequired)
            {
                // 현재 스레드가 UI 스레드가 아니라면 Invoke를 사용하여 UI 스레드에서 이 메서드를 호출
                userlistBox.Invoke(new MethodInvoker(delegate
                {
                    ClearUserList();
                }));
            }
            else
            {
                userlistBox.Items.Clear();
            }
        }

        public void ClearChatList()
        {
            if (chatlistBox.InvokeRequired)
            {
                // 현재 스레드가 UI 스레드가 아니라면 Invoke를 사용하여 UI 스레드에서 이 메서드를 호출
                chatlistBox.Invoke(new MethodInvoker(delegate
                {
                    ClearChatList();
                }));
            }
            else
            {
                chatlistBox.Items.Clear();
            }
        }


        // 다른 유저 나갔을 때
        public void EraseUserList(string nickname)
        {
            if (userlistBox.InvokeRequired)
            {
                userlistBox.Invoke(new MethodInvoker(delegate
                {
                    EraseUserList(nickname);
                }));
            }
            else
            {
                for (int i = userlistBox.Items.Count - 1; i >= 0; i--)
                {
                    // ListViewItem의 텍스트가 검색 문자열과 일치하면
                    if (userlistBox.Items[i].ToString() == nickname)
                    {
                        // 해당 항목 삭제
                        userlistBox.Items.RemoveAt(i);
                    }
                }
            }
        }

        public void EnterUserAlaram(string nickname)
        {
            if (chatlistBox.InvokeRequired)
            {
                chatlistBox.Invoke(new MethodInvoker(delegate
                {
                    EnterUserAlaram(nickname);
                }));
            }
            else
            {
                string message = " is Enter";
                var msg = $"{nickname} {message}";
                chatlistBox.Items.Add(msg);
            }
        }
        public void LeaveUserAlaram(string nickname)
        {
            if (chatlistBox.InvokeRequired)
            {
                chatlistBox.Invoke(new MethodInvoker(delegate
                {
                    LeaveUserAlaram(nickname);
                }));
            }
            else
            {
                string message = " is quit";
                var msg = $"{nickname} {message}";
                chatlistBox.Items.Add(msg);
            }
        }

        public void ChangePositionBlack(string nick)
        {
            if (blacklabel.InvokeRequired)
            {
                blacklabel.Invoke(new MethodInvoker(delegate
                {
                    ChangePositionBlack(nick);
                }));
            }
            else
            {
                blacklabel.Text = nick;
            }
        }

        public void ChangePositionWhite(string nick)
        {
            if (whitelabel.InvokeRequired)
            {
                whitelabel.Invoke(new MethodInvoker(delegate
                {
                    ChangePositionWhite(nick);
                }));
            }
            else
            {
                whitelabel.Text = nick;
            }
        }

        public string GetBlackName()
        {
            return blacklabel.Text;
        }
        public string GetWhiteName()
        {
            return whitelabel.Text;
        }
        // 진영 선택
        public void ClearBlackLabel()
        {
            if (blacklabel.InvokeRequired)
            {
                blacklabel.Invoke(new MethodInvoker(delegate
                {
                    ClearBlackLabel();
                }));
            }
            else
            {
                blacklabel.Text = "";
            }
        }
        public void ClearWhiteLabel()
        {
            if (whitelabel.InvokeRequired)
            {
                whitelabel.Invoke(new MethodInvoker(delegate
                {
                    ClearWhiteLabel();
                }));
            }
            else
            {
                whitelabel.Text = "";
            }
        }

        public void ReadyBlackLabel()
        {
            if (blacklabel.InvokeRequired)
            {
                blacklabel.Invoke(new MethodInvoker(delegate
                {
                    ReadyBlackLabel();
                }));
            }
            else
            {
                blacklabel.ForeColor = Color.Red;
            }
        }

        public void CancelBlackLabel()
        {
            if (blacklabel.InvokeRequired)
            {
                blacklabel.Invoke(new MethodInvoker(delegate
                {
                    CancelBlackLabel();
                }));
            }
            else
            {
                blacklabel.ForeColor = Color.Black;
            }
        }
        public void ReadyWhiteLabel()
        {
            if (whitelabel.InvokeRequired)
            {
                whitelabel.Invoke(new MethodInvoker(delegate
                {
                    ReadyWhiteLabel();
                }));
            }
            else
            {
                whitelabel.ForeColor = Color.Red;
            }
        }
        public void CancelWhiteLabel()
        {
            if (whitelabel.InvokeRequired)
            {
                whitelabel.Invoke(new MethodInvoker(delegate
                {
                    CancelWhiteLabel();
                }));
            }
            else
            {
                whitelabel.ForeColor = Color.Black;
            }
        }

        // 클릭했을 때

        private void chatbutton_Click(object sender, EventArgs e)
        {
            string message = chatBox.Text;

            //AddRoomChatMessageList(CClient.Instance.GetMyNickName(), message);

            // 메시지 전송 만들기.
            ushort type = 501;
            ulong accountNo = CClient.Instance.GetAccountNo();
            ushort roomNo = CClient.Instance.GetCurrentRoom();
            byte chatLen;
            chatLen = (byte)message.Length;
            byte[] chat = Encoding.Unicode.GetBytes(message);

            CBuffer packet = new CBuffer();
            packet.Clear();

            // packet 합치기.
            packet.Write(type);
            packet.Write((long)accountNo);
            packet.Write(roomNo);
            packet.Write(chatLen);
            packet.Write(chat, chatLen * 2);

            packet.SetHeader();

            int useSize = packet.GetUseSize();

            // sendBuffer에 넣기.
            clientTcp._sendBuffer.Enqueue(packet.ToArray(), headerLen + useSize);
        }

        private void leaveRoombutton_Click(object sender, EventArgs e)
        {
            // 보내야할 것.
            // len | type ||| flag roomid roomLen roomName

            // 메시지 전송 만들기.
            ushort type = 401;
            ulong accountNo = CClient.Instance.GetAccountNo();
            ushort roomNo = CClient.Instance.GetCurrentRoom();
            //byte roomLen;
            //string roomName = CClient.Instance.GetCurrentRoomName();
            //byte[] roomNameArray = Encoding.UTF8.GetBytes(roomName);
            //roomLen = (byte)roomName.Length;

            CBuffer packet = new CBuffer();
            packet.Clear();

            // packet 합치기.
            packet.Write(type);
            packet.Write((long)accountNo);
            packet.Write(roomNo);
            //packet.Write(roomLen);
            //packet.Write(roomNameArray, roomLen);

            packet.SetHeader();

            int useSize = packet.GetUseSize();

            // sendBuffer에 넣기.
            clientTcp._sendBuffer.Enqueue(packet.ToArray(), headerLen + useSize);
        }

        private void enterRoombutton2_Click(object sender, EventArgs e)
        {
            string roomNoText = roomNoBox.Text;
            ulong accountNo;
            ushort type;
            ushort roomNo;
            if (roomNoText == "")
            {
                return;
            }
            else
            {
                roomNo = Convert.ToUInt16(roomNoText);
                if (roomNo == 0)
                {
                    return;
                }
                else
                {
                    CBuffer packet = new CBuffer();
                    packet.Clear();

                    type = 301;
                    accountNo = CClient.Instance.GetAccountNo();

                    packet.Write(type);
                    packet.Write((long)accountNo);
                    packet.Write(roomNo);

                    packet.SetHeader();

                    int useSize = packet.GetUseSize();

                    // sendBuffer에 넣기.
                    clientTcp._sendBuffer.Enqueue(packet.ToArray(), headerLen + useSize);
                }
            }
        }


        private void blackbutton_Click(object sender, EventArgs e)
        {
            ushort type = 1001;
            ulong accountNo = CClient.Instance.GetAccountNo();
            ushort roomNo = CClient.Instance.GetCurrentRoom();

            byte from = CClient.Instance.GetPosition();
            byte to = 1;
            byte nickLen;
            string nickName = CClient.Instance.GetMyNickName();
            byte[] nickArray = Encoding.Unicode.GetBytes(nickName);
            nickLen = (byte)nickName.Length;

            bool flag = true;
            if (roomNo == 0)
            {
                flag = false;
            }

            if (flag)
            {
                CBuffer packet = new CBuffer();
                packet.Clear();

                // packet 합치기.
                packet.Write(type);
                packet.Write((long)accountNo);
                packet.Write(roomNo);
                packet.Write(from);
                packet.Write(to);
                packet.Write(nickLen);
                packet.Write(nickArray, nickLen * 2);

                packet.SetHeader();

                int useSize = packet.GetUseSize();

                // sendBuffer에 넣기.
                clientTcp._sendBuffer.Enqueue(packet.ToArray(), headerLen + useSize);
            }
            else
            {
                Console.Write("blackbutton_Click Fail");
            }
        }

        private void whitebutton_Click(object sender, EventArgs e)
        {
            ushort type = 1001;
            ulong accountNo = CClient.Instance.GetAccountNo();
            ushort roomNo = CClient.Instance.GetCurrentRoom();

            byte from = CClient.Instance.GetPosition();
            byte to = 2;
            byte nickLen;
            string nickName = CClient.Instance.GetMyNickName();
            byte[] nickArray = Encoding.Unicode.GetBytes(nickName);
            nickLen = (byte)nickName.Length;

            bool flag = true;
            if (roomNo == 0)
            {
                flag = false;
            }

            if (flag)
            {
                CBuffer packet = new CBuffer();
                packet.Clear();

                // packet 합치기.
                packet.Write(type);
                packet.Write((long)accountNo);
                packet.Write(roomNo);
                packet.Write(from);
                packet.Write(to);
                packet.Write(nickLen);
                packet.Write(nickArray, nickLen * 2);

                packet.SetHeader();

                int useSize = packet.GetUseSize();

                // sendBuffer에 넣기.
                clientTcp._sendBuffer.Enqueue(packet.ToArray(), headerLen + useSize);
            }
            else
            {
                Console.Write("whitebutton_Click Fail");
            }
        }

        private void specbutton_Click(object sender, EventArgs e)
        {
            ushort type = 1002;
            ulong accountNo = CClient.Instance.GetAccountNo();
            ushort roomNo = CClient.Instance.GetCurrentRoom();

            byte from = CClient.Instance.GetPosition();
            byte to = 3;
            byte nickLen;
            string nickName = CClient.Instance.GetMyNickName();
            byte[] nickArray = Encoding.Unicode.GetBytes(nickName);
            nickLen = (byte)nickName.Length;

            bool flag = true;
            if (roomNo == 0)
            {
                flag = false;
            }

            if (flag)
            {
                CBuffer packet = new CBuffer();
                packet.Clear();

                // packet 합치기.
                packet.Write(type);
                packet.Write((long)accountNo);
                packet.Write(roomNo);
                packet.Write(from);
                packet.Write(to);
                packet.Write(nickLen);
                packet.Write(nickArray, nickLen * 2);

                packet.SetHeader();

                int useSize = packet.GetUseSize();

                // sendBuffer에 넣기.
                clientTcp._sendBuffer.Enqueue(packet.ToArray(), headerLen + useSize);
            }
            else
            {
                Console.Write("Spec button_Click Fail");
            }
        }

        private void readybutton_Click(object sender, EventArgs e)
        {
            ushort type = 1101;
            ulong accountNo = CClient.Instance.GetAccountNo();
            ushort roomNo = CClient.Instance.GetCurrentRoom();

            byte position = CClient.Instance.GetPosition();

            bool flag = true;
            if (roomNo == 0)
            {
                flag = false;
            }
            if (!(position == 1 || position == 2))
                flag = false;
            if (flag)
            {
                CBuffer packet = new CBuffer();
                packet.Clear();

                // packet 합치기.
                packet.Write(type);
                packet.Write((long)accountNo);
                packet.Write(roomNo);
                packet.Write(position);

                packet.SetHeader();

                int useSize = packet.GetUseSize();

                // sendBuffer에 넣기.
                clientTcp._sendBuffer.Enqueue(packet.ToArray(), headerLen + useSize);
            }
            else
            {
                Console.Write("readybutton_Click Fail");
            }
        }

        private void cancelbutton_Click(object sender, EventArgs e)
        {
            ushort type = 1103;
            ulong accountNo = CClient.Instance.GetAccountNo();
            ushort roomNo = CClient.Instance.GetCurrentRoom();

            byte position = CClient.Instance.GetPosition();

            bool flag = true;
            if (roomNo == 0)
            {
                flag = false;
            }
            if (!(position == 1 || position == 2))
                flag = false;

            if (flag)
            {
                CBuffer packet = new CBuffer();
                packet.Clear();

                // packet 합치기.
                packet.Write(type);
                packet.Write((long)accountNo);
                packet.Write(roomNo);
                packet.Write(position);

                packet.SetHeader();

                int useSize = packet.GetUseSize();

                // sendBuffer에 넣기.
                clientTcp._sendBuffer.Enqueue(packet.ToArray(), headerLen + useSize);
            }
            else
            {
                Console.Write("cancelbutton_Click Fail");
            }
        }

        private void leaveRoombutton2_Click(object sender, EventArgs e)
        {
            en_PacketType packetType = en_PacketType.LeaveRoomRequest;
            ulong accountNo = CClient.Instance.GetAccountNo();
            ushort roomNo = CClient.Instance.GetCurrentRoom();

            CBuffer packet = new CBuffer();

            packet.Write(packetType);

            packet.Write((long)accountNo);
            packet.Write(roomNo);

            packet.SetHeader();

            int useSize = packet.GetUseSize();
            clientTcp._sendBuffer.Enqueue(packet.ToArray(), headerLen + useSize);
        }
    }
}