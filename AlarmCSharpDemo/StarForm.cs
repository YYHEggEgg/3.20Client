using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
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
            SQLiteConnection conn = null;
            string dbPath = "Data Source =" + Environment.CurrentDirectory + "/psdbase.db";
            conn = new SQLiteConnection(dbPath);//创建数据库实例，指定文件位置  
            conn.Open();//打开数据库，若文件不存在会自动创建  
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
            string sql = "SELECT * FROM \"AreaList\" LIMIT 0, 1000";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.CommandType = CommandType.Text;
            SQLiteDataReader dReader = cmd.ExecuteReader();
            DataTable dTable = new DataTable();
            dTable.Load(dReader);
            for (int i = 0; i < dTable.Rows.Count; i++)
            {
                if(dTable.Rows[i]["Area"].ToString()== comboBox1.Text)
                {
                    MessageBox.Show("已经添加过该地区");
                    conn.Close();
                    return;
                }
            }
            sql = "INSERT INTO \"main\".\"AreaList\" (\"Area\", \"Ip\", \"Port\") VALUES ('"+comboBox1.Text+"', '"+ textBox1.Text + "', '"+ textBox2.Text + "')";
            cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
            this.Close();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
