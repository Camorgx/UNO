using System;
using System.Windows.Forms;
using UserNamespace;
using GameCore;
using System.Collections.Generic;

namespace UNO__ {
    public partial class mainForm : Form {
        public static User user = new User();
        public mainForm() {
            InitializeComponent();
        }

        private void mainForm_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void 关于ToolStripMenuItem1_Click(object sender, EventArgs e) {
            About about = new About {
                StartPosition = FormStartPosition.CenterScreen
            };
            about.Show();
        }


        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void 游戏规则ToolStripMenuItem_Click(object sender, EventArgs e) {
            Rules rules = new Rules {
                StartPosition = FormStartPosition.CenterScreen
            };
            rules.Show();
        }

        private void button2_Click(object sender, EventArgs e) {
            this.Hide();
            PersonnalMessage personnalMessage = new PersonnalMessage {
                StartPosition = FormStartPosition.CenterScreen,
                Owner = this
            };
            personnalMessage.SetText(user);
            personnalMessage.ReturnEvent += new PersonnalMessage.ReturnUser(getUser);
            personnalMessage.Show();
        }
        private void getUser(User usr) {
            if (!user.Equals(usr)) {
                user = usr;
                userset = true;
            }
        }


        private void 用户设置ToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Hide();
            PersonnalMessage personnalMessage = new PersonnalMessage {
                StartPosition = FormStartPosition.CenterScreen,
                Owner = this
            };
            personnalMessage.SetText(user);
            personnalMessage.ReturnEvent += new PersonnalMessage.ReturnUser(getUser);
            personnalMessage.Show();
        }

        private void button3_Click(object sender, EventArgs e) {
            if (!userset) {
                MessageBox.Show("你还没有设置用户！", "提示");
                return;
            }
            this.Hide();
            Createroom createroom = new Createroom {
                StartPosition = FormStartPosition.CenterScreen,
            };
            createroom.SetUser(user);
            createroom.Show();
        }

        private void mainForm_FormClosed(object sender, FormClosedEventArgs e) {
            Environment.Exit(0);
        }
        bool userset = false;
        private void button4_Click(object sender, EventArgs e) {
            if (!userset) {
                MessageBox.Show("你还没有设置用户！", "提示");
                return;
            }
            this.Hide();
            Joinroom joinroom = new Joinroom {
                StartPosition = FormStartPosition.CenterScreen,
                Owner = this,
                clientuser = user
            };
            joinroom.Show();
        }

        private void button1_Click(object sender, EventArgs e) {
            Playgame playgame = new Playgame {
                StartPosition = FormStartPosition.CenterScreen
            };
            playgame.SetText(user);
            playgame.hostuser = user;
            playgame.users = new List<User>();
            playgame.users.Add(user);
            Core.n = 1;
            Core.InitGame(1);
            Core.get_card(1, 7);
            Core.SinglePlayerMode = true;
            playgame.RefreshCard();
            playgame.RefreshUsers();
            playgame.SetNameLabel();
            playgame.tableLayoutPanel4.Show();
            playgame.SendCardEvent += new Playgame.SendCardDelegate((Card card, int id, string s, int t) => { });
            playgame.Show();
        }
    }
}
