using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AlarmCSharpDemo
{
    public partial class StarForm : Form
    {
        public StarForm()
        {
            InitializeComponent();
        }

        private void StarForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int tmp;
            if (!int.TryParse(textBox2.Text, out tmp))
            {
                MessageBox.Show("端口格式错误");
                return;
            }
            if (comboBox1.Text == ""||textBox1.Text==""||textBox2.Text=="")
            {
                MessageBox.Show("请填写IP,端口和地区");
                return;
            }
            AlarmDemo.getvalue = comboBox1.Text;
            AlarmDemo alarmDemo = new AlarmDemo();
            this.Hide();
            alarmDemo.ShowDialog();
            Application.Exit();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
