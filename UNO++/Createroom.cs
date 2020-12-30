using GameCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using UserNamespace;
namespace UNO__
{
    public partial class Createroom : Form
    {
        public delegate void SendInfoDelegate(Communication comm);
        SendInfoDelegate sendInfoDelegate = null;
        Socket server = null;
        List<Socket> sockets = new List<Socket>();
        public List<User> users = new List<User>();
        public void InitServer(int port)
        {
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            EndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
            server.Bind(endPoint);
            server.Listen(1000);
            Debug.WriteLine("Server started");
            Thread t = new Thread(GetClient);
            t.Start(server);
        }
        public void GetClient(object s)
        {
            Socket server = s as Socket;
            Socket client = null;
            while (true)
            {
                try { client = server.Accept(); }
                catch (Exception) { return; }
                sockets.Add(client);
                Thread t2 = new Thread(ReceiveInfo);
                t2.Start(client);
            }
        }
        public void ReceiveInfo(object o)
        {
            Socket client = o as Socket;
            byte[] b = new byte[10240];
            Communication com = null;
            while (true)
            {
                try
                {
                    client.Receive(b);
                    com = DeserializeObject(b) as Communication;
                }
                catch (Exception)
                {
                    return;
                }
                switch (com.MsgType)
                {
                    case msgType.user:
                        User user = com.user;
                        renewuserbox(user);
                        users.Add(user);
                        break;
                    case msgType.card_with_id:
                        sendInfoExceptDelegate = new SendInfoExceptDelegate(SendInfoExcept);
                        this.Invoke(sendInfoExceptDelegate, com, com.userid);
                        playgame.SetLastCard(com.card);
                        Core.last_card.number = com.card.number;
                        Core.last_card.reset_card();
                        Core.Delete(Core.order, com.cardid);
                        Core.handle_card();
                        if (Core.top_color == "any_color")
                        {
                            Core.top_color = com.color;
                            playgame.cardControl17.label2.Text = Core.out_change(com.color);
                        }
                        if (Core.FindNextUser())
                        {
                            MessageBox.Show($"{users[Core.win_rand[1] - 1].Name} 赢了！", "游戏结束");
                            Application.Exit();
                        }
                        else
                        {
                            playgame.SetNameLabel();
                            playgame.RefreshUsers();
                            if (Core.order == users.Count) playgame.button1.Enabled = true;
                        }
                        playgame.RefreshCard();
                        break;
                    default:
                        break;
                }
            }
        }

        Playgame playgame = null;

        public void SendInfos(Communication comm)
        {
            byte[] b = SerializeObject(comm);
            foreach (Socket s in sockets)
                s.Send(b);
        }
        public object DeserializeObject(byte[] pBytes)
        {
            object newOjb = null;
            if (pBytes == null)
                return newOjb;
            MemoryStream memory = new MemoryStream(pBytes)
            {
                Position = 0
            };
            BinaryFormatter formatter = new BinaryFormatter();
            newOjb = formatter.Deserialize(memory);
            memory.Close();
            return newOjb;
        }
        public byte[] SerializeObject(object pObj)
        {
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
        public Createroom()
        {
            InitializeComponent();
            textBox2.Text = "您 ";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int port = Convert.ToInt32(textBox1.Text);
                if (!(port >= 1000 && port <= 10000))
                    throw (new Exception());
                InitServer(port);
                MessageBox.Show($"房间创建成功，您的IP为{GetLocalIP()}。\n 输入IP与房间号以连接至本房间。"
                    , "创建成功");
            }
            catch (Exception)
            {
                MessageBox.Show("房间号格式不正确，请重新输入。", "错误");
                return;
            }
        }
        User hostuser = null;
        public void SetUser(User user)
        {
            hostuser = user;
        }
        void renewuserbox(User user)
        {
            textBox2.Text += user.Name + " ";
        }
        public static string GetLocalIP()
        {
            string result = RunApp("route", "print", true);
            Match m = Regex.Match(result, @"0.0.0.0\s+0.0.0.0\s+(\d+.\d+.\d+.\d+)\s+(\d+.\d+.\d+.\d+)");
            if (m.Success)
            {
                return m.Groups[2].Value;
            }
            else
            {
                try
                {
                    TcpClient c = new TcpClient();
                    c.Connect("www.baidu.com", 80);
                    string ip = ((IPEndPoint)c.Client.LocalEndPoint).Address.ToString();
                    c.Close();
                    return ip;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static string RunApp(string filename, string arguments, bool recordLog)
        {
            try
            {
                if (recordLog)
                    Trace.WriteLine(filename + " " + arguments);
                Process proc = new Process();
                proc.StartInfo.FileName = filename;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.Arguments = arguments;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.UseShellExecute = false;
                proc.Start();

                using (StreamReader sr = new StreamReader
                    (proc.StandardOutput.BaseStream, Encoding.Default))
                {
                    Thread.Sleep(100);
                    if (!proc.HasExited) proc.Kill();
                    string txt = sr.ReadToEnd();
                    sr.Close();
                    if (recordLog)
                        Trace.WriteLine(txt);
                    return txt;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return ex.Message;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            users.Add(hostuser);
            sendInfoDelegate = new SendInfoDelegate(SendInfos);
            this.Invoke(sendInfoDelegate, new Communication(users));
            Playgame playgame = new Playgame
            {
                StartPosition = FormStartPosition.CenterScreen,
            };
            this.playgame = playgame;
            playgame.SetText(hostuser);
            playgame.hostuser = hostuser;
            playgame.users = users;
            Core.n = users.Count;
            Core.InitGame(users.Count);
            this.Invoke(sendInfoDelegate, new Communication(Core.get_pile));
            for (int i = 1; i <= users.Count; i++) Core.get_card(i, 7);
            playgame.RefreshCard();
            playgame.RefreshUsers();
            playgame.SetNameLabel();
            playgame.SendCardEvent += new Playgame.SendCardDelegate(SendCard);
            playgame.Show();
        }

        public delegate void SendInfoExceptDelegate(Communication comm, int userid);
        SendInfoExceptDelegate sendInfoExceptDelegate;
        public void SendInfoExcept(Communication comm, int userid)
        {
            int cnt = 0;
            byte[] b = SerializeObject(comm);
            for (int i = 0; i < sockets.Count; ++i)
                if (i != userid-1)
                {
                    sockets[i].Send(b);
                    ++cnt;
                }
        }

        void SendCard(Card card, int id, string color = "", int userid = 0)
        {
            this.Invoke(sendInfoDelegate, new Communication(card, id, color, userid));
        }

        private void Createroom_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("是否确定关闭游戏？", "提示", MessageBoxButtons.YesNo) == DialogResult.No)
                e.Cancel = true;
        }
        private void Createroom_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
