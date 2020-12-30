using GameCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using UserNamespace;
namespace UNO__
{
    public partial class Playgame : Form
    {
        int operatetimes = 0;
        public Playgame()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            tableLayoutPanel4.Hide();
            namelabels.Add(label3); namelabels.Add(label12);
            namelabels.Add(label9); namelabels.Add(label8);
            namelabels.Add(label6); namelabels.Add(label5);
            namelabels.Add(label11); namelabels.Add(label4);
            cardlabels.Add(label16); cardlabels.Add(label15);
            cardlabels.Add(label18); cardlabels.Add(label17);
            cardlabels.Add(label7); cardlabels.Add(label10);
            cardlabels.Add(label14); cardlabels.Add(label13);
            cards.Add(cardControl1); cards.Add(cardControl2);
            cards.Add(cardControl3); cards.Add(cardControl4);
            cards.Add(cardControl5); cards.Add(cardControl6);
            cards.Add(cardControl7); cards.Add(cardControl8);
            cards.Add(cardControl9); cards.Add(cardControl10);
            cards.Add(cardControl11); cards.Add(cardControl12);
            cards.Add(cardControl13); cards.Add(cardControl14);
            cards.Add(cardControl15); cards.Add(cardControl16);
        }
        public User hostuser = null;
        public List<User> users = null;
        public int userid = 0;

        public void RefreshUsers()
        {
            int tmp = 0;
            for (int i = 0; i < users.Count; ++i)
            {
                if (!users[i].Equals(hostuser))
                {
                    namelabels[tmp].Text = users[i].Name;
                    cardlabels[tmp++].Text = Convert.ToString(Core.user_card_sum[i + 1]);
                }
            }
            label21.Text = "剩余" + Core.user_card_sum[userid].ToString() + "张手牌";
        }

        public void RefreshCard()
        {
            for (int i = 1; i <= users.Count; ++i)
            {
                if (users[i - 1].Equals(hostuser))
                {
                    for (int j = 0; j < Core.user_card_sum[i]; ++j)
                    {
                        cards[j].SetCard(Core.user[i, j + 1]);
                    }
                    //i!=1是初始化条件，开局时只允许一号玩家出牌
                    if (Core.last_card.num == -2 && i != 1) button1.Enabled = false;
                    userid = i;
                }
            }
            for (int i = Core.user_card_sum[userid]; i <= 15; ++i)
            {
                try
                {
                    cards[i].label1.Text = cards[i].label2.Text = "";
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        public void SetText(User user)
        {
            label20.Text = user.Name;
            label21.Text = "剩余" + user.CardNumber + "张手牌";
        }

        private void Playgame_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }
        List<Label> namelabels = new List<Label>();
        List<Label> cardlabels = new List<Label>();
        List<CardControl.CardControl> cards = new List<CardControl.CardControl>();

        private void Playgame_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("是否确定关闭游戏？", "提示", MessageBoxButtons.YesNo) == DialogResult.No)
                e.Cancel = true;
        }

        public delegate void SendCardDelegate(Card card, int id, string color = "", int userid = 0);
        public event SendCardDelegate SendCardEvent;

        private void button1_Click(object sender, EventArgs e)
        {
            Card card = new Card(-2);
            int id = -1;
            for (int i = 1; i <= Core.user_card_sum[userid]; ++i)
            {
                if (cards[i - 1].isClicked)
                {
                    if (card.num != -2)
                    {
                        MessageBox.Show("每次只能出一张牌哦。", "提示");
                        return;
                    }
                    card.number = Core.user[userid, i].number;
                    card.reset_card();
                    id = i;
                }
            }
            if (card.num == -2)
            {
                MessageBox.Show("请选择一张牌。", "提示");
                return;
            }
            else if (!Core.Judge(card))
            {
                MessageBox.Show("这张牌不能出哦。", "提示");
                return;
            }
            if (card.color == "any_color")//
            {
                ColorLIst colorLIst = new ColorLIst
                {
                    StartPosition = FormStartPosition.CenterScreen
                };
                colorLIst.ReturnColorEvent += new ColorLIst.ReturnColorDelegate(GetColor);
                colorLIst.ShowDialog();
            }
            SendCardEvent(card, id, tmpcolor, userid);
            SetLastCard(card);
            Core.last_card.number = card.number;
            Core.last_card.reset_card();
            Core.Delete(Core.order, id);
            Core.handle_card();
            RefreshCard();
            RefreshUsers();
            if (Core.top_color == "any_color")
            {
                Core.top_color = tmpcolor;
                cardControl17.label2.Text = Core.out_change(tmpcolor);
            }
            if (Core.FindNextUser())
            {
                MessageBox.Show($"{users[Core.win_rand[1] - 1].Name} 赢了！", "游戏结束");
                Environment.Exit(0);
            }
            else
            {
                for (int i = 0; i < 16; ++i)
                {
                    cards[i].BackColor = SystemColors.ButtonFace;
                    cards[i].isClicked = false;
                }
                if (Core.SinglePlayerMode)
                    label23.Text = (++operatetimes).ToString();
                if (Core.user_card_sum[1] > 16)
                {
                    MessageBox.Show($"您 输了！", "游戏结束");
                    Environment.Exit(0);
                }
                RefreshCard();
                RefreshUsers();
                SetNameLabel();
                if (Core.order == userid)
                {
                    button1.Enabled = true;
                    return;
                }
            }
            button1.Enabled = false;
        }

        public void SetNameLabel()
        {
            label20.BackColor = SystemColors.ActiveCaption;
            foreach (var i in namelabels)
                i.BackColor = SystemColors.ActiveCaption;
            if (Core.order != userid)
            {
                if (Core.order < userid) namelabels[Core.order - 1].BackColor = Color.Yellow;
                else namelabels[Core.order - 2].BackColor = Color.Yellow;
            }
            else label20.BackColor = Color.Yellow;
        }

        string tmpcolor = "";

        void GetColor(string color)
        {
            tmpcolor = color;
        }

        public void SetLastCard(Card card)
        {
            cardControl17.SetCard(card);
        }
    }
}
