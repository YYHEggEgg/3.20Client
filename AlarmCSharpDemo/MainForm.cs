using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using PreviewDemo;
using System.Data.SQLite;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Timers;

namespace AlarmCSharpDemo
{
    public partial class MainForm : Form
    {
        static public string getIp { get; set; }
        static public string getPort { get; set; }
        static public string getvalue{get;set;}
        private bool m_bInitSDK = false;
        public delegate void MyDebugInfo(string str);
        private Int32 m_lUserID = -1;
        private int[] iIPDevID = new int[96];
        private int iDeviceNumber = 0; //添加设备个数
        private uint iLastErr = 0;
        private string strErr;
        private long iSelIndex = 0;
        //bool listenBool = false;
        System.Timers.Timer ListenTimer = new System.Timers.Timer();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new RegionalSelection());
        }
        public MainForm()
        {
            InitializeComponent();
            m_bInitSDK = CHCNetSDK.NET_DVR_Init();
            if (m_bInitSDK == false)
            {
                MessageBox.Show("NET_DVR_Init error!");
                return;
            }
            else
            {
                //保存SDK日志 To save the SDK log
                CHCNetSDK.NET_DVR_SetLogToFile(3, "C:\\SdkLog\\", true);
                for (int i = 0; i < 64; i++)
                {
                    iIPDevID[i] = -1;
                }
            }
            
        }
        string state = "";
        private void btnLogin_Click(object sender, EventArgs e)
        {
            int tmp;
            if (!int.TryParse(textBoxPort.Text, out tmp))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            if (textBoxIP.Text == "" || textBoxPort.Text == "" ||
                textBoxUserName.Text == "" || textBoxPassword.Text == ""||textBox1.Text =="")
            {
                MessageBox.Show("Please input IP, Port, User name and Password!");
                return;
            }
            if (iDeviceNumber >= 20)
            {
                //MessageBox.Show("本程序限制最多添加20台设备！");
                //return;
            }
            string DVRIPAddress = textBoxIP.Text; //设备IP地址或者域名 Device IP
            string DVRPortNumber = textBoxPort.Text;//设备服务端口号 Device Port
            string DVRUserName = textBoxUserName.Text;//设备登录用户名 User name to login
            string DVRPassword = textBoxPassword.Text;//设备登录密码 Password to login
            string DVRDevname = textBox1.Text;//设备登录密码 Password to login
            int login_id= deviceLogin(DVRIPAddress, DVRPortNumber, DVRUserName, DVRPassword);
            string sql = "INSERT INTO \"main\".\"DeviceList\" " +
                "(\"IP\", \"Port\", \"Name\", \"Password\", \"Text\",\"Menuinfo\")" +
                " VALUES ('"+ DVRIPAddress + "', '"+ DVRPortNumber + "', '"+ DVRUserName + "', '"+ DVRPassword + "', '" + DVRDevname + "', '" + getvalue + "')";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
            if (login_id == -1)state = "登录失败";
            else state = "登录成功"; 
            listViewDevice.Items.Add(new ListViewItem(new string[] { login_id.ToString(), DVRIPAddress, DVRPortNumber, state, DVRDevname, DVRUserName , DVRPassword }));
        }
         private void btnExit_Click(object sender, EventArgs e)
         {
             //停止监听
             //注销登录
             for (int i = 0; i < iDeviceNumber; i++)
             {
                 m_lUserID = Int32.Parse(listViewDevice.Items[i].SubItems[0].Text);
                 CHCNetSDK.NET_DVR_Logout(m_lUserID);                 
             }

             //释放SDK资源，在程序结束之前调用
             CHCNetSDK.NET_DVR_Cleanup();
             Application.Exit();
        }

         private void listViewDevice_MouseClick(object sender, MouseEventArgs e)
         {
             if (e.Button == MouseButtons.Right)
             {
                 if (listViewDevice.SelectedItems.Count > 0)
                 {
                     if (DialogResult.OK == MessageBox.Show("请确认是否删除所选择的设备！","删除提示",MessageBoxButtons.OKCancel))
                     {
                         foreach (ListViewItem item in this.listViewDevice.SelectedItems)
                         {
                             if (item.Selected)
                             {
                                try { 
                                //SQLiteConnection cn = new SQLiteConnection("data source=" + str[0]);
                                string dbPath = "Data Source =" + Environment.CurrentDirectory + "/psdbase.db";
                                SQLiteConnection cn = new SQLiteConnection(dbPath);//创建数据库实例，指定文件位置  
                                cn.Open();
                                SQLiteCommand cmd = new SQLiteCommand();
                                cmd.Connection = cn;
                                string sql = "DELETE FROM \"main\".\"DeviceList\" WHERE IP = '" + item.SubItems[1].Text + "' AND Text = '" + item.SubItems[4].Text
                                   + "' AND Port = '" + item.SubItems[2].Text + "' AND Name = '" + item.SubItems[5].Text + "' AND Password = '" + item.SubItems[6].Text + "'";
                                m_lUserID = Int32.Parse(item.SubItems[0].Text);
                                //取得str[1]表名的表的建表SQL语句 
                                cmd.CommandText = sql;
                                cmd.ExecuteNonQuery();
                                cn.Close();
                                DebugInfo("注销成功");
                                    CHCNetSDK.NET_DVR_Logout(m_lUserID);
                                
                                }
                                catch
                                {
                                    DebugInfo("删除失败");
                                }
                                //CHCNetSDK.NET_DVR_CloseAlarmChan_V30(m_lAlarmHandle[m_lUserID]);
                                
                                 item.Remove();
                                 //iDeviceNumber--;
                             }
                         }
                         this.listViewDevice.Refresh();
                     }                      
                 }
                 else
                 {
                     
                 }
             }
         }
        Socket client_socket;
        Socket server_socket;
        void Client_content()
        {

            for (int i = 0; i < listViewDevice.Items.Count; i++)
            {
                try
                {
                    int port;
                    int.TryParse(listViewDevice.Items[i].SubItems[2].Text, out port);
                    IPAddress ip = IPAddress.Parse(listViewDevice.Items[i].SubItems[1].Text);
                    IPEndPoint ipe = new IPEndPoint(ip, port);//把ip和端口转化为IPEndPoint实例
                    client_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    client_socket.Connect(ipe);//连接到服务器
                    listViewDevice.Items[i].SubItems[3].Text = "在线";
                    client_socket.Close();
                }
                catch (ArgumentNullException)
                {

                }
                catch (StackOverflowException)
                {
                    client_socket.Close();

                }
                catch (SocketException)
                {
                    listViewDevice.Items[i].SubItems[3].Text = "离线";
                    client_socket.Close();

                }
                listViewDevice.Update();
            }

        }
        private void GroupBox3_Enter(object sender, EventArgs e)
        {

        }
        public void DebugInfo(string str)
        {
            if (str.Length > 0)
            {
                str += "\n";
                TextBoxInfo.AppendText(DateTime.Now.ToString()+":"+str);
            }
        }
        private void listViewDevice_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            
            if (listViewDevice.SelectedItems.Count > 0)
            {
                iSelIndex = listViewDevice.SelectedItems[0].Index;  //当前选中的行
                int.TryParse(listViewDevice.SelectedItems[0].Text,out m_lUserID);

            }
        }
        SQLiteConnection conn = null;
        private void AlarmDemo_Load(object sender, EventArgs e)
        {
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
                AreaList.Items.Add(new ListViewItem(new string[] { dTable.Rows[i]["Area"].ToString(), dTable.Rows[i]["Ip"].ToString(), dTable.Rows[i]["Port"].ToString() }));
            }

            sql = "SELECT * FROM \"DeviceList\" WHERE \"Menuinfo\" LIKE '%"+getvalue+"%' LIMIT 0, 1000";
            cmd = new SQLiteCommand(sql, conn);
            //cmd.ExecuteReader();
            cmd.CommandType = CommandType.Text;
            dReader = cmd.ExecuteReader();
            dTable = new DataTable();
            dTable.Load(dReader);
            string ip,name,port,password,text;
            for (int i = 0; i < dTable.Rows.Count; i++)
            {
                ip = dTable.Rows[i]["IP"].ToString();
                port = dTable.Rows[i]["Port"].ToString();
                name = dTable.Rows[i]["Name"].ToString();
                password = dTable.Rows[i]["password"].ToString();
                text = dTable.Rows[i]["Text"].ToString();
                int login_id = deviceLogin(ip, port, name, password);
                if (login_id == -1) state = "登录失败";
                else state = "登录成功";
                listViewDevice.Items.Add(new ListViewItem(new string[] { login_id.ToString(), ip, port, state, text, name , password }));
            }
            conn.Close();
            AreaList.HideSelection = false;
            if (AreaList.Items[0]!=null)AreaList.Items[0].Selected = true;
        }
        int deviceLogin(string DVRIPAddress, string DVRPortNumber, string DVRUserName, string DVRPassword)
        {
            if (iDeviceNumber >= 20)
            {
                //MessageBox.Show("本程序限制最多添加20台设备！");
                //return;
            }
            CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();

            //登录设备 Login the device
            m_lUserID = CHCNetSDK.NET_DVR_Login_V30(DVRIPAddress, int.Parse(DVRPortNumber), DVRUserName, DVRPassword, ref DeviceInfo);
            if (m_lUserID < 0)
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                strErr = "NET_DVR_Login_V30 failed, error code= " + iLastErr; //登录失败，输出错误号 Failed to login and output the error code
                DebugInfo(strErr);
                return m_lUserID;
            }
            else
            {
                DebugInfo("NET_DVR_Login_V30 succ!");
                //登录成功
                iDeviceNumber++;
                string str1 = "" + m_lUserID;
                //将已注册设备添加进列表
                                                                                                       //DebugInfo("NET_DVR_Login_V30 succ!");
                                                                                                       //btnLogin.Text = "Logout";

                return m_lUserID;
            }
        }
        private void groupBox2_Enter(object sender, EventArgs e)
        {
        }
        private void button1_Click(object sender, EventArgs e)
        {

            CheckForIllegalCrossThreadCalls = false;
            ThreadStart ts = new ThreadStart(server_content);
            Thread thread = new Thread(ts);
            thread.Name = "LoadData";
            thread.Start();
        }
        private void server_content()
        {
            ListenTimer.Interval = 5000;
            ListenTimer.Elapsed += new ElapsedEventHandler(ServerT_Tick);
            ListenTimer.Enabled = true;
            this.Invoke(new MethodInvoker(delegate
            {
                ListenTimer.Start();
            }));

            //throw new NotImplementedException();
        }
        private void ServerT_Tick(object sender, EventArgs e)
        {

            if (!ifcontent)
            {
                Server_connect_Info();
            }
            else
            {

                ListenTimer.Enabled = false;
                if (server_socket.Poll(-1, SelectMode.SelectRead))
                {
                    server_socket.Close();
                    ifcontent = false;
                    DebugInfo("连接已经断开,5秒后自动重新连接\r\n");
                    ListenTimer.Enabled = true;
                }
            }
        }
        //连接服务器获取信息
        void Server_connect_Info()
        {

            try
            {
                IPAddress ip = IPAddress.Parse(getIp);
                IPEndPoint ipe = new IPEndPoint(ip, int.Parse(getPort));//把ip和端口转化为IPEndPoint实例
                server_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //创建一个Socket
                DebugInfo("连接中...");
                server_socket.Connect(ipe);//连接到服务器//string sendStr = "北京监控Socket 监控一\r\n";
                First_server_send_info();
                ifcontent = true;
            }
            catch
            {
                //server_socket.Close();
                ifcontent = false;
            }

        }
        //连接服务器获取信息
        static bool ifcontent = false;
        private int AreaListIndex;

        void First_server_send_info()
        {
            string dbPath = "Data Source =" + Environment.CurrentDirectory + "/psdbase.db";
            conn = new SQLiteConnection(dbPath);//创建数据库实例，指定文件位置  
            conn.Open();//打开数据库，若文件不存在会自动创建  
            string sql = "SELECT * FROM \"DeviceList\" WHERE \"Menuinfo\" LIKE '%" + getvalue + "%' LIMIT 0, 1000";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            //cmd.ExecuteReader();
            cmd.CommandType = CommandType.Text;
            SQLiteDataReader dReader = cmd.ExecuteReader();
            DataTable dTable = new DataTable();
            dTable.Load(dReader);
            string ip, name, port, password, text, menuinfo;
            byte[] bs;
            for (int i = 0; i < dTable.Rows.Count; i++)
            {
                ip = dTable.Rows[i]["IP"].ToString();
                port = dTable.Rows[i]["Port"].ToString();
                name = dTable.Rows[i]["Name"].ToString();
                password = dTable.Rows[i]["password"].ToString();
                text = dTable.Rows[i]["Text"].ToString();
                menuinfo = dTable.Rows[i]["Menuinfo"].ToString();
                bs = Encoding.UTF8.GetBytes("MENUINFO " + getvalue + "|" + text + "|" + ip + "|"
                    + port + "|" + name + "|" + password + "|" + menuinfo + "\r\n");
                server_socket.Send(bs, bs.Length, 0);
            }
            DebugInfo("本地信息发送成功");
        }

        private void TextBoxInfo_TextChanged(object sender, EventArgs e)
        {

        }

        private void AlarmDemo_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = 0; i < iDeviceNumber; i++)
            {
                m_lUserID = Int32.Parse(listViewDevice.Items[i].SubItems[0].Text);
                CHCNetSDK.NET_DVR_Logout(m_lUserID);
            }

            //释放SDK资源，在程序结束之前调用
            CHCNetSDK.NET_DVR_Cleanup();
            Application.Exit();
        }

        private void AreaList_MouseClick(object sender, MouseEventArgs e)
        {
            MessageBox.Show(AreaList.SelectedItems[0].Text);
        }

        private void AreaList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (AreaList.SelectedItems.Count > 0)
            {
                AreaListIndex = AreaList.SelectedItems[0].Index;  //当前选中的行
            }
        }
    }
}
