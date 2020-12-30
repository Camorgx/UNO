using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UNO__
{
    public partial class ColorLIst : Form
    {
        public ColorLIst()
        {
            InitializeComponent();
        }

        public delegate void ReturnColorDelegate(string ret);
        public event ReturnColorDelegate ReturnColorEvent;

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked) ReturnColorEvent("red");
            if (radioButton2.Checked) ReturnColorEvent("yellow");
            if (radioButton3.Checked) ReturnColorEvent("blue");
            if (radioButton4.Checked) ReturnColorEvent("green");
            this.Close();
        }

        private void ColorLIst_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!radioButton1.Checked && !radioButton2.Checked
                && !radioButton3.Checked && !radioButton4.Checked)
            {
                MessageBox.Show("请选择一种颜色", "提示");
                e.Cancel = true;
            }
        }
    }
}
