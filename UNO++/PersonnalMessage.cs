using System;
using System.Windows.Forms;
using UserNamespace;
namespace UNO__
{
    public partial class PersonnalMessage : Form
    {
        public void SetText(User user)
        {
            textBox1.Text = user.Name;
            textBox2.Text = user.Id;
        }
        public PersonnalMessage()
        {
            InitializeComponent();
        }
        public delegate void ReturnUser(User ret);
        public event ReturnUser ReturnEvent;
        private void button1_Click(object sender, EventArgs e)
        {
            User ret = new User(textBox1.Text, textBox2.Text);
            ReturnEvent(ret);
            Owner.Show();
            this.Close();
        }

        private void PersonnalMessage_FormClosed(object sender, FormClosedEventArgs e)
        {
            Owner.Show();
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Enter)
            {
                User ret = new User(textBox1.Text, textBox2.Text);
                ReturnEvent(ret);
                Owner.Show();
                this.Close();
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                User ret = new User(textBox1.Text, textBox2.Text);
                ReturnEvent(ret);
                Owner.Show();
                this.Close();
            }
        }
    }
}
