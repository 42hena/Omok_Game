namespace Omok_Game
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        /// 

        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel = new Panel();
            groupBox1 = new GroupBox();
            closeButton = new Button();
            connectButton = new Button();
            serverPortBox = new TextBox();
            serverIpBox = new TextBox();
            labelPort = new Label();
            labelIp = new Label();
            labelStatus = new Label();
            groupBox2 = new GroupBox();
            echoButton = new Button();
            echolabel = new Label();
            echoBox = new TextBox();
            groupBox3 = new GroupBox();
            loginbutton = new Button();
            accountBox = new TextBox();
            nicknameBox = new TextBox();
            accountlabel = new Label();
            nicknamelabel = new Label();
            groupBox4 = new GroupBox();
            nextbutton = new Button();
            prevbutton = new Button();
            roomlistView = new ListView();
            createRoombutton = new Button();
            leaveRoombutton = new Button();
            enterRoombutton = new Button();
            groupBox5 = new GroupBox();
            userlistBox = new ListBox();
            leaveRoombutton2 = new Button();
            enterRoombutton2 = new Button();
            roomNoBox = new TextBox();
            chatbutton = new Button();
            chatBox = new TextBox();
            chatlistBox = new ListBox();
            blackbutton = new Button();
            whitebutton = new Button();
            blacklabel = new Label();
            whitelabel = new Label();
            readybutton = new Button();
            cancelbutton = new Button();
            groupBox6 = new GroupBox();
            specbutton = new Button();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox5.SuspendLayout();
            groupBox6.SuspendLayout();
            SuspendLayout();
            // 
            // panel
            // 
            panel.BackColor = Color.Peru;
            panel.Location = new Point(499, 20);
            panel.Name = "panel";
            panel.Size = new Size(460, 460);
            panel.TabIndex = 0;
            panel.Paint += panel_Paint;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(closeButton);
            groupBox1.Controls.Add(connectButton);
            groupBox1.Controls.Add(serverPortBox);
            groupBox1.Controls.Add(serverIpBox);
            groupBox1.Controls.Add(labelPort);
            groupBox1.Controls.Add(labelIp);
            groupBox1.Location = new Point(2, 1);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(328, 81);
            groupBox1.TabIndex = 12;
            groupBox1.TabStop = false;
            groupBox1.Text = "소켓 더미 클라이언트 설정";
            // 
            // closeButton
            // 
            closeButton.Location = new Point(237, 41);
            closeButton.Name = "closeButton";
            closeButton.Size = new Size(75, 23);
            closeButton.TabIndex = 15;
            closeButton.Text = "close";
            closeButton.UseVisualStyleBackColor = true;
            // 
            // connectButton
            // 
            connectButton.Location = new Point(237, 16);
            connectButton.Name = "connectButton";
            connectButton.Size = new Size(75, 23);
            connectButton.TabIndex = 13;
            connectButton.Text = "connect";
            connectButton.UseVisualStyleBackColor = true;
            connectButton.Click += connectButton_Click;
            // 
            // serverPortBox
            // 
            serverPortBox.Location = new Point(76, 41);
            serverPortBox.Name = "serverPortBox";
            serverPortBox.Size = new Size(150, 23);
            serverPortBox.TabIndex = 14;
            serverPortBox.Text = "12001";
            // 
            // serverIpBox
            // 
            serverIpBox.Location = new Point(76, 16);
            serverIpBox.Name = "serverIpBox";
            serverIpBox.Size = new Size(150, 23);
            serverIpBox.TabIndex = 13;
            serverIpBox.Text = "127.0.0.1";
            // 
            // labelPort
            // 
            labelPort.AutoSize = true;
            labelPort.Location = new Point(6, 44);
            labelPort.Name = "labelPort";
            labelPort.Size = new Size(60, 15);
            labelPort.TabIndex = 13;
            labelPort.Text = "서버 port:";
            // 
            // labelIp
            // 
            labelIp.AutoSize = true;
            labelIp.Location = new Point(6, 19);
            labelIp.Name = "labelIp";
            labelIp.Size = new Size(48, 15);
            labelIp.TabIndex = 13;
            labelIp.Text = "서버 ip:";
            // 
            // labelStatus
            // 
            labelStatus.AutoSize = true;
            labelStatus.Location = new Point(336, 9);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(62, 15);
            labelStatus.TabIndex = 16;
            labelStatus.Text = "서버 상태:";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(echoButton);
            groupBox2.Controls.Add(echolabel);
            groupBox2.Controls.Add(echoBox);
            groupBox2.Location = new Point(2, 88);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(328, 48);
            groupBox2.TabIndex = 17;
            groupBox2.TabStop = false;
            groupBox2.Text = "에코 테스트(숫자 입력)";
            // 
            // echoButton
            // 
            echoButton.Location = new Point(237, 22);
            echoButton.Name = "echoButton";
            echoButton.Size = new Size(75, 23);
            echoButton.TabIndex = 18;
            echoButton.Text = "전송";
            echoButton.UseVisualStyleBackColor = true;
            echoButton.Click += echoButton_Click;
            // 
            // echolabel
            // 
            echolabel.AutoSize = true;
            echolabel.Location = new Point(6, 25);
            echolabel.Name = "echolabel";
            echolabel.Size = new Size(50, 15);
            echolabel.TabIndex = 17;
            echolabel.Text = "에코 값:";
            // 
            // echoBox
            // 
            echoBox.Location = new Point(76, 22);
            echoBox.Name = "echoBox";
            echoBox.Size = new Size(150, 23);
            echoBox.TabIndex = 16;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(loginbutton);
            groupBox3.Controls.Add(accountBox);
            groupBox3.Controls.Add(nicknameBox);
            groupBox3.Controls.Add(accountlabel);
            groupBox3.Controls.Add(nicknamelabel);
            groupBox3.Location = new Point(2, 139);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(328, 70);
            groupBox3.TabIndex = 18;
            groupBox3.TabStop = false;
            groupBox3.Text = "로그인";
            // 
            // loginbutton
            // 
            loginbutton.Location = new Point(237, 18);
            loginbutton.Name = "loginbutton";
            loginbutton.Size = new Size(75, 43);
            loginbutton.TabIndex = 23;
            loginbutton.Text = "로그인";
            loginbutton.UseVisualStyleBackColor = true;
            loginbutton.Click += loginbutton_Click;
            // 
            // accountBox
            // 
            accountBox.Location = new Point(76, 43);
            accountBox.Name = "accountBox";
            accountBox.Size = new Size(150, 23);
            accountBox.TabIndex = 22;
            // 
            // nicknameBox
            // 
            nicknameBox.Location = new Point(76, 17);
            nicknameBox.Name = "nicknameBox";
            nicknameBox.Size = new Size(150, 23);
            nicknameBox.TabIndex = 21;
            // 
            // accountlabel
            // 
            accountlabel.AutoSize = true;
            accountlabel.Location = new Point(4, 46);
            accountlabel.Name = "accountlabel";
            accountlabel.Size = new Size(66, 15);
            accountlabel.TabIndex = 20;
            accountlabel.Text = "accountNo";
            // 
            // nicknamelabel
            // 
            nicknamelabel.AutoSize = true;
            nicknamelabel.Location = new Point(6, 25);
            nicknamelabel.Name = "nicknamelabel";
            nicknamelabel.Size = new Size(43, 15);
            nicknamelabel.TabIndex = 19;
            nicknamelabel.Text = "닉네임";
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(nextbutton);
            groupBox4.Controls.Add(prevbutton);
            groupBox4.Controls.Add(roomlistView);
            groupBox4.Controls.Add(createRoombutton);
            groupBox4.Controls.Add(leaveRoombutton);
            groupBox4.Controls.Add(enterRoombutton);
            groupBox4.Location = new Point(2, 211);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(328, 141);
            groupBox4.TabIndex = 19;
            groupBox4.TabStop = false;
            groupBox4.Text = "로비";
            // 
            // nextbutton
            // 
            nextbutton.Location = new Point(259, 114);
            nextbutton.Name = "nextbutton";
            nextbutton.Size = new Size(41, 23);
            nextbutton.TabIndex = 7;
            nextbutton.Text = ">";
            nextbutton.UseVisualStyleBackColor = true;
            // 
            // prevbutton
            // 
            prevbutton.Location = new Point(217, 114);
            prevbutton.Name = "prevbutton";
            prevbutton.Size = new Size(36, 23);
            prevbutton.TabIndex = 6;
            prevbutton.Text = "<";
            prevbutton.UseVisualStyleBackColor = true;
            // 
            // roomlistView
            // 
            roomlistView.FullRowSelect = true;
            roomlistView.Location = new Point(3, 19);
            roomlistView.Name = "roomlistView";
            roomlistView.Size = new Size(208, 118);
            roomlistView.TabIndex = 5;
            roomlistView.UseCompatibleStateImageBehavior = false;
            roomlistView.View = View.Details;
            // 
            // createRoombutton
            // 
            createRoombutton.Location = new Point(217, 23);
            createRoombutton.Name = "createRoombutton";
            createRoombutton.Size = new Size(75, 23);
            createRoombutton.TabIndex = 4;
            createRoombutton.Text = "방 만들기";
            createRoombutton.UseVisualStyleBackColor = true;
            createRoombutton.Click += createRoombutton_Click;
            // 
            // leaveRoombutton
            // 
            leaveRoombutton.Location = new Point(217, 81);
            leaveRoombutton.Name = "leaveRoombutton";
            leaveRoombutton.Size = new Size(75, 23);
            leaveRoombutton.TabIndex = 3;
            leaveRoombutton.Text = "방 나가기";
            leaveRoombutton.UseVisualStyleBackColor = true;
            leaveRoombutton.Click += leaveRoombutton_Click;
            // 
            // enterRoombutton
            // 
            enterRoombutton.Location = new Point(217, 52);
            enterRoombutton.Name = "enterRoombutton";
            enterRoombutton.Size = new Size(75, 23);
            enterRoombutton.TabIndex = 2;
            enterRoombutton.Text = "방 입장";
            enterRoombutton.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(userlistBox);
            groupBox5.Controls.Add(leaveRoombutton2);
            groupBox5.Controls.Add(enterRoombutton2);
            groupBox5.Controls.Add(roomNoBox);
            groupBox5.Controls.Add(chatbutton);
            groupBox5.Controls.Add(chatBox);
            groupBox5.Controls.Add(chatlistBox);
            groupBox5.Location = new Point(2, 358);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(470, 298);
            groupBox5.TabIndex = 20;
            groupBox5.TabStop = false;
            groupBox5.Text = "groupBox5";
            // 
            // userlistBox
            // 
            userlistBox.Font = new Font("맑은 고딕", 8F, FontStyle.Regular, GraphicsUnit.Point);
            userlistBox.FormattingEnabled = true;
            userlistBox.Location = new Point(6, 48);
            userlistBox.Name = "userlistBox";
            userlistBox.Size = new Size(100, 199);
            userlistBox.TabIndex = 12;
            // 
            // leaveRoombutton2
            // 
            leaveRoombutton2.Location = new Point(259, 19);
            leaveRoombutton2.Name = "leaveRoombutton2";
            leaveRoombutton2.Size = new Size(75, 23);
            leaveRoombutton2.TabIndex = 11;
            leaveRoombutton2.Text = "방 나가기";
            leaveRoombutton2.UseVisualStyleBackColor = true;
            leaveRoombutton2.Click += leaveRoombutton2_Click;
            // 
            // enterRoombutton2
            // 
            enterRoombutton2.Location = new Point(178, 19);
            enterRoombutton2.Name = "enterRoombutton2";
            enterRoombutton2.Size = new Size(75, 23);
            enterRoombutton2.TabIndex = 10;
            enterRoombutton2.Text = "방 입장";
            enterRoombutton2.UseVisualStyleBackColor = true;
            enterRoombutton2.Click += enterRoombutton2_Click;
            // 
            // roomNoBox
            // 
            roomNoBox.Font = new Font("맑은 고딕", 7F, FontStyle.Regular, GraphicsUnit.Point);
            roomNoBox.Location = new Point(6, 19);
            roomNoBox.Name = "roomNoBox";
            roomNoBox.Size = new Size(110, 20);
            roomNoBox.TabIndex = 9;
            roomNoBox.Text = "12345678901234567890";
            // 
            // chatbutton
            // 
            chatbutton.Location = new Point(389, 262);
            chatbutton.Name = "chatbutton";
            chatbutton.Size = new Size(75, 23);
            chatbutton.TabIndex = 8;
            chatbutton.Text = "전송";
            chatbutton.UseVisualStyleBackColor = true;
            chatbutton.Click += chatbutton_Click;
            // 
            // chatBox
            // 
            chatBox.Location = new Point(17, 262);
            chatBox.Name = "chatBox";
            chatBox.Size = new Size(362, 23);
            chatBox.TabIndex = 7;
            // 
            // chatlistBox
            // 
            chatlistBox.FormattingEnabled = true;
            chatlistBox.ItemHeight = 15;
            chatlistBox.Location = new Point(146, 48);
            chatlistBox.Name = "chatlistBox";
            chatlistBox.Size = new Size(286, 199);
            chatlistBox.TabIndex = 6;
            // 
            // blackbutton
            // 
            blackbutton.Location = new Point(0, 37);
            blackbutton.Name = "blackbutton";
            blackbutton.Size = new Size(75, 23);
            blackbutton.TabIndex = 21;
            blackbutton.Text = "Black";
            blackbutton.UseVisualStyleBackColor = true;
            blackbutton.Click += blackbutton_Click;
            // 
            // whitebutton
            // 
            whitebutton.Location = new Point(81, 37);
            whitebutton.Name = "whitebutton";
            whitebutton.Size = new Size(75, 23);
            whitebutton.TabIndex = 22;
            whitebutton.Text = "White";
            whitebutton.UseVisualStyleBackColor = true;
            whitebutton.Click += whitebutton_Click;
            // 
            // blacklabel
            // 
            blacklabel.AutoSize = true;
            blacklabel.Location = new Point(6, 19);
            blacklabel.Name = "blacklabel";
            blacklabel.Size = new Size(19, 15);
            blacklabel.TabIndex = 23;
            blacklabel.Text = "흑";
            // 
            // whitelabel
            // 
            whitelabel.AutoSize = true;
            whitelabel.Location = new Point(81, 19);
            whitelabel.Name = "whitelabel";
            whitelabel.Size = new Size(19, 15);
            whitelabel.TabIndex = 24;
            whitelabel.Text = "백";
            // 
            // readybutton
            // 
            readybutton.Location = new Point(0, 67);
            readybutton.Name = "readybutton";
            readybutton.Size = new Size(75, 23);
            readybutton.TabIndex = 25;
            readybutton.Text = "Ready";
            readybutton.UseVisualStyleBackColor = true;
            readybutton.Click += readybutton_Click;
            // 
            // cancelbutton
            // 
            cancelbutton.Location = new Point(81, 66);
            cancelbutton.Name = "cancelbutton";
            cancelbutton.Size = new Size(75, 23);
            cancelbutton.TabIndex = 26;
            cancelbutton.Text = "Cancel";
            cancelbutton.UseVisualStyleBackColor = true;
            cancelbutton.Click += cancelbutton_Click;
            // 
            // groupBox6
            // 
            groupBox6.Controls.Add(specbutton);
            groupBox6.Controls.Add(blacklabel);
            groupBox6.Controls.Add(cancelbutton);
            groupBox6.Controls.Add(blackbutton);
            groupBox6.Controls.Add(readybutton);
            groupBox6.Controls.Add(whitelabel);
            groupBox6.Controls.Add(whitebutton);
            groupBox6.Location = new Point(336, 46);
            groupBox6.Name = "groupBox6";
            groupBox6.Size = new Size(157, 119);
            groupBox6.TabIndex = 27;
            groupBox6.TabStop = false;
            groupBox6.Text = "groupBox6";
            // 
            // specbutton
            // 
            specbutton.Location = new Point(0, 93);
            specbutton.Name = "specbutton";
            specbutton.Size = new Size(75, 23);
            specbutton.TabIndex = 27;
            specbutton.Text = "관전자";
            specbutton.UseVisualStyleBackColor = true;
            specbutton.Click += specbutton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(966, 655);
            Controls.Add(groupBox6);
            Controls.Add(groupBox2);
            Controls.Add(groupBox5);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(labelStatus);
            Controls.Add(groupBox1);
            Controls.Add(panel);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            groupBox6.ResumeLayout(false);
            groupBox6.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel;
        private GroupBox groupBox1;
        private Label labelIp;
        private TextBox serverPortBox;
        private TextBox serverIpBox;
        private Label labelPort;
        private Button closeButton;
        private Button connectButton;
        private Label labelStatus;
        private GroupBox groupBox2;
        private Button echoButton;
        private Label echolabel;
        private TextBox echoBox;
        private GroupBox groupBox3;
        private Button loginbutton;
        private TextBox accountBox;
        private TextBox nicknameBox;
        private Label accountlabel;
        private Label nicknamelabel;
        private GroupBox groupBox4;
        private Button leaveRoombutton;
        private Button enterRoombutton;
        private Button createRoombutton;
        private GroupBox groupBox5;
        private Button chatbutton;
        private TextBox chatBox;
        private ListBox chatlistBox;
        private Button leaveRoombutton2;
        private Button enterRoombutton2;
        private TextBox roomNoBox;
        private ListBox userlistBox;
        private ListView roomlistView;
        private Button nextbutton;
        private Button prevbutton;
        private Button blackbutton;
        private Button whitebutton;
        private Label blacklabel;
        private Label whitelabel;
        private Button readybutton;
        private Button cancelbutton;
        private GroupBox groupBox6;
        private Button specbutton;
    }
}