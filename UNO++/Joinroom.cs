using GameCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;
using UserNamespace;
namespace UNO__ {
    public partial class Joinroom : Form {
        List<User> users = new List<User>();
        Socket client = null;
        void InitClient(int port, string ip = "192.168.43.245") {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            EndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            client.Connect(endPoint);
            client.Send(SerializeObject(new Communication(clientuser)));
            Thread t = new Thread(ReceiveInfo);
            t.Start(client);
        }

        public delegate void CardReceivedDelegate(Communication com);
        CardReceivedDelegate cardReceivedDelegate;

        void CardReCeived(Communication com) {
            playgame1.SetLastCard(com.card);
            Core.last_card.number = com.card.number;
            Core.last_card.reset_card();
            Core.Delete(Core.order, com.cardid);
            Core.handle_card();
            if (Core.top_color == "any_color") {
                Core.top_color = com.color;
                playgame1.cardControl17.label2.Text = Core.out_change(com.color);
            }
            if (Core.FindNextUser()) {
                MessageBox.Show($"{users[Core.win_rand[1] - 1].Name} 赢了！", "游戏结束");
                Application.Exit();
            }
            else {
                playgame1.SetNameLabel();
                playgame1.RefreshUsers();
                playgame1.RefreshCard();
                if (Core.order == playgame1.userid) {
                    playgame1.button1.Enabled = true;
                }
            }
        }

        void ReceiveInfo(object c) {
            byte[] b = new byte[10240];
            Socket client = c as Socket;
            while (true) {
                try {
                    client.Receive(b);
                }
                catch (Exception) {
                    client.Dispose();
                    return;
                }
                Communication comm = DeserializeObject(b) as Communication;
                switch (comm.MsgType) {
                    case msgType.userlist:
                        users = comm.users;
                        break;
                    case msgType.initusercards:
                        this.Hide();
                        Core.n = users.Count;
                        Core.prepare();
                        Core.get_pile = comm.usercards;
                        for (int i = 1; i <= users.Count; ++i)
                            Core.get_card(i, 7);
                        Thread thread = new Thread(new ThreadStart(ThreadBegin));
                        thread.Start();
                        break;
                    case msgType.card_with_id:
                        cardReceivedDelegate = new CardReceivedDelegate(CardReCeived);
                        this.Invoke(cardReceivedDelegate, comm);//尝试切换为Invoke
                        break;
                    default:
                        break;
                }
            }
        }

        Playgame playgame1 = null;

        void ThreadBegin() {
            MethodInvoker methodInvoker = new MethodInvoker(ShowPlaygameWindow);
            BeginInvoke(methodInvoker);
        }

        void ShowPlaygameWindow() {
            Playgame playgame = new Playgame {
                StartPosition = FormStartPosition.CenterScreen
            };
            playgame1 = playgame;
            playgame.SetText(clientuser);
            playgame.hostuser = clientuser;
            playgame.users = users;
            playgame.RefreshCard();
            playgame.RefreshUsers();
            playgame.SetNameLabel();
            playgame.SendCardEvent += new Playgame.SendCardDelegate(SendCard);
            playgame.Show();
        }

        void SendCard(Card card, int id, string color, int userid) {
            client.Send(SerializeObject(new Communication(card, id, color, userid)));
        }

        object DeserializeObject(byte[] pBytes) {
            object newOjb = null;
            if (pBytes == null)
                return newOjb;
            MemoryStream memory = new MemoryStream(pBytes) {
                Position = 0
            };
            BinaryFormatter formatter = new BinaryFormatter();
            newOjb = formatter.Deserialize(memory);
            memory.Close();
            return newOjb;
        }
        byte[] SerializeObject(object pObj) {
            if (pObj == null)
                return null;
            MemoryStream memory = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(memory, pObj);
            memory.Position = 0;
            byte[] read = new byte[memory.Length];
            memory.Read(read, 0, read.Length);
            memory.Close();
            return read;
        }
        public User clientuser = null;
        public Joinroom() {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void button1_Click(object sender, EventArgs e) {
            try {
                int port = Convert.ToInt32(textBox2.Text);
                InitClient(port, textBox1.Text);
                MessageBox.Show("房间加入成功，请等待房主开始游戏！", "成功");
            }
            catch (Exception) {
                MessageBox.Show("IP或房间号的格式不正确，请重新输入！", "错误");
                return;
            }
        }

        private void Joinroom_FormClosed(object sender, FormClosedEventArgs e) {
            Application.Exit();
        }

        private void Joinroom_FormClosing(object sender, FormClosingEventArgs e) {
            if (MessageBox.Show("是否确定关闭游戏？", "提示", MessageBoxButtons.YesNo) == DialogResult.No)
                e.Cancel = true;
        }
    }
}
