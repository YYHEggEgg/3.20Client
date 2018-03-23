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
    public partial class RegionalSelection : Form
    {
        static public string GetAreaIP { get; set; }
        static public string GetAreaPort { get; set; }
        static public string GetAreaText { get; set; }
        public RegionalSelection()
        {
            InitializeComponent();
        }
        private void RegionalSelection_Load(object sender, EventArgs e)
        {
            AreaList.Items.Clear();
            SQLiteConnection conn = null;
            string dbPath = "Data Source =" + Environment.CurrentDirectory + "/psdbase.db";
            conn = new SQLiteConnection(dbPath);//创建数据库实例，指定文件位置  
            conn.Open();//打开数据库，若文件不存在会自动创建  
            string sql = "SELECT * FROM \"AreaList\" LIMIT 0, 1000";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.CommandType = CommandType.Text;
            SQLiteDataReader dReader = cmd.ExecuteReader();
            DataTable dTable = new DataTable();
            dTable.Load(dReader);
            for (int i = 0; i < dTable.Rows.Count; i++)
            {
                AreaList.Items.Add(dTable.Rows[i]["Area"].ToString() + "  " + dTable.Rows[i]["Ip"].ToString() + ":" + dTable.Rows[i]["Port"].ToString());
            }
            conn.Close();
        }
        private void AddAreaBtn_Click(object sender, EventArgs e)
        {
            StarForm starForm = new StarForm();
            starForm.ShowDialog();
            RegionalSelection_Load(null, null);
        }
        private void ExitBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void EnterBtn_Click(object sender, EventArgs e)
        {
            MainForm mainForm = new MainForm();
            mainForm.ShowDialog();
        }
    }
}
