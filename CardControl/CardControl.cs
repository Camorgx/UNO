using System;
using System.Drawing;
using System.Windows.Forms;
using GameCore;
namespace CardControl
{
    public partial class CardControl: UserControl
    {
        public bool isClicked = false;
        public CardControl()
        {
            InitializeComponent();
        }

        public void SetCard(Card card)
        {

            string attribute = card.attribute;
            if (attribute == "figure")
            {
                label1.Text = card.num.ToString();
                label2.Text = Core.out_change(card.color);
            }
            else
            {
                label1.Text = Core.out_change(attribute);
                label2.Text = Core.out_change(card.color);
            }
        }

        private void label1_MouseClick(object sender, MouseEventArgs e)
        {
            if (isClicked)
            {
                isClicked = false;
                tableLayoutPanel1.BackColor = SystemColors.ButtonFace;
            }
            else
            {
                isClicked = true;
                tableLayoutPanel1.BackColor = SystemColors.Highlight;
            }
        }

        private void label1_MouseLeave(object sender, EventArgs e)
        {
            if (!isClicked) tableLayoutPanel1.BackColor = SystemColors.ButtonFace;
            else tableLayoutPanel1.BackColor = SystemColors.Highlight;
        }

        private void label1_MouseEnter(object sender, EventArgs e)
        { 
            if (!isClicked) tableLayoutPanel1.BackColor = Color.Aqua;
            else tableLayoutPanel1.BackColor = Color.CornflowerBlue;
        }

        private void label2_MouseEnter(object sender, EventArgs e)
        {
            if (!isClicked) tableLayoutPanel1.BackColor = Color.Aqua;
            else tableLayoutPanel1.BackColor = Color.CornflowerBlue;
        }

        private void label2_MouseLeave(object sender, EventArgs e)
        {
            if (!isClicked) tableLayoutPanel1.BackColor = SystemColors.ButtonFace;
            else tableLayoutPanel1.BackColor = SystemColors.Highlight;
        }

        private void label2_MouseClick(object sender, MouseEventArgs e)
        {
            if (isClicked)
            {
                isClicked = false;
                tableLayoutPanel1.BackColor = SystemColors.ButtonFace;
            }
            else
            {
                isClicked = true;
                tableLayoutPanel1.BackColor = SystemColors.Highlight;
            }
        }
    }
}
