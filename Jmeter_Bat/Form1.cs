using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jmeter_Bat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.Text = "Jmeter 批量脚本生成器";
            Init();
        }

        //jmx命令
        //String jmx_cmd = @"call jmeter -Jroom_index=%index%10000 -Jid_index=%index%1000 -Jteacher_id_index=%index%2000 -n -t %JMX_NAME%.jmx -l ./%d%/Jtl/%JMX_NAME%.csv -j %d%/jmeter.log -e -o ./%d%/Report";
        String jmx_cmd = "";

        private void Init() {
            textBox1.Text = GetConfigValue("jmx_name", "1V1_WebSocket");
            textBox2.Text = GetConfigValue("index", "1");
            textBox3.Text = GetConfigValue("bat_number", "20");
            jmx_cmd = GetConfigValue("jmeter_cmd", jmx_cmd);
            textBox4.Text = GetConfigValue("jmeter_cmd", jmx_cmd);
            label5.Text = "内置参数: %JMX_NAME% 脚本名称 %d% 启动时间 %index% 标识值";
            label5.Enabled = false;

            //关闭窗口事件
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {


        }


        private string GetCMD(int index)
        {
            String cmd = "";
            cmd += @"@echo off" + "\r\n\r\n";

            /*
            DateTime now = DateTime.Now;
            Console.WriteLine(now.ToString("yyyyMMddHHmmss"));
            */



            cmd += @"::---------配置文件-------------" + "\r\n";
            cmd += @"::jmx文件名" + "\r\n";
            cmd += @"set JMX_NAME=" + textBox1.Text +  "\r\n";
            cmd += @"::起始标识值" + "\r\n";
            cmd += @"set index=" + index + "\r\n";
            cmd += @"::------------------------------" + "\r\n\r\n";


            cmd += @"::生成当前日期" + "\r\n";
            cmd += @"set dateTmp=%date:~0,4%%date:~5,2%%date:~8,2%" + "\r\n";
            cmd += "if \"%time:~0,2%\" lss \"10\" (set hour=0%time:~1,1%) else (set hour=%time:~0,2%)" + "\r\n";
            cmd += @"set timeTmp=%hour%%time:~3,2%%time:~6,2%%time:~9,2%" + "\r\n";
            cmd += @"set d=%dateTmp%%timeTmp%" + "\r\n";
            cmd += @"echo 当前时间: %d%" + "\r\n";
            cmd += "\r\n";
            cmd += "mkdir \"%d%\"" + "\r\n";
            cmd += "\r\n";

            cmd += @"::执行Jmx脚本" + "\r\n";
            cmd += textBox4.Text + "\r\n";
            return cmd;
        }


        private int IfInt(String num, Boolean showMessageBox)
        {
            if (int.TryParse(num, out int tmp1) && int.Parse(num) >= 0)
            {
                return int.Parse(num);
            }
            else
            {
                if (showMessageBox) { MessageBox.Show(num + "不是正整数,已做为0处理！"); }
                return 0;

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void GenerateVBS()
        {

            CreateDir(@".\Bat");
            RunBat(@".\", "清除.bat文件.bat");
            String arr = "";
            for (int i = 0; i < IfInt(textBox3.Text, false); i++)
            {
                String cmd = GetCMD(IfInt(textBox2.Text, false) + i);
                WriteFile(@".\Bat\Jmeter_Bat_" + (IfInt(textBox2.Text, false) + i) + ".bat", cmd);
                //生成字符串
                arr += "\"Jmeter_Bat_" + (IfInt(textBox2.Text, false) + i) + "\"";
                if (i < IfInt(textBox3.Text, false) - 1)
                {
                    arr += ",";
                }
            }

            String vbs = "";
            vbs += "arr = array(" + arr + ")" + "\r\n";
            vbs += "For i=0 To UBound(arr)-LBound(arr)" + "\r\n";
            vbs += "    bat=\".\\Bat\\\"+arr(i)+\".bat\"" + "\r\n";
            vbs += "    wscript.createobject(\"wscript.shell\").run \"cmd /c \"\"\"+bat+\"\"\"\"" + "\r\n";
            vbs += "    wscript.sleep " + GetConfigValue("bat_delay", "1000") + "\r\n";
            vbs += "Next";

            String vbs_name = textBox2.Text + "_to_" + (IfInt(textBox2.Text, false) + IfInt(textBox3.Text, false) - 1);
            if (IfInt(textBox3.Text, false) > 0)
            {
                WriteFile(@".\RUN_TEST_" + vbs_name + ".vbs", vbs);
            }
        }

            private void button1_Click(object sender, EventArgs e)
        {

            String clearBat = "";
            clearBat += "if exist .\\Bat\\*.bat (del /a /f \".\\Bat\\*.bat\")" + "\r\n";
            clearBat += "if exist .\\*.vbs (del /a /f \".\\*.vbs\")";
            WriteFile(@".\" + "清除.bat文件.bat", clearBat);

            if (IfInt(textBox3.Text, false) > 0 && IfInt(textBox2.Text, false) > 0)
            {
                GenerateVBS();
            }
            else {
                MessageBox.Show("标识值和生成数量必须是正整数！");
            }


        }


        private void button2_Click(object sender, EventArgs e)
        {
            String clearBat = "";
            clearBat += "if exist .\\Bat\\*.bat (del /a /f \".\\Bat\\*.bat\")" + "\r\n";
            clearBat += "if exist .\\*.vbs (del /a /f \".\\*.vbs\")";
            WriteFile(@".\" + "清除.bat文件.bat", clearBat);

            if (IfInt(textBox3.Text, false) > 0 && IfInt(textBox2.Text, false) > 0)
            {
                GenerateVBS();

                //执行vbs脚本
                String vbs_name = "RUN_TEST_" + textBox2.Text + "_to_" + (IfInt(textBox2.Text, false) + IfInt(textBox3.Text, false) - 1) + ".vbs";
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = "wscript";
                proc.StartInfo.Arguments = @".\" + vbs_name;
                proc.StartInfo.UseShellExecute = false;
                proc.Start();
            }
            else
            {
                MessageBox.Show("标识值和生成数量必须是正整数！");
            }

        }
        private void runThread() {

        }

        private void WriteFile(string path, string batStr)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.GetEncoding("GB2312"));
            //开始写入
            sw.Write(batStr);
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
        }


        private void RunBat(String fileDir, String fileName)
        {
            Process proc = null;
            try
            {
                string targetDir = string.Format(fileDir);//this is where mybatch.bat lies
                proc = new Process();
                proc.StartInfo.WorkingDirectory = targetDir;
                proc.StartInfo.FileName = fileName;
                proc.StartInfo.Arguments = string.Format("10");//this is argument
                proc.StartInfo.CreateNoWindow = false;
                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString());
            }
        }

        //根据文件夹全路径创建文件夹
        public static void CreateDir(string path)
        {

            //string path = @".\Error";
            if (Directory.Exists(path))
            {
                Console.WriteLine("此文件夹已经存在，无需创建！");
            }
            else
            {
                Directory.CreateDirectory(path);
                Console.WriteLine(path + " 创建成功!");
            }
        }

        private String RunCmd(string strInput)
        {
            Process p = new Process();
            //设置要启动的应用程序
            p.StartInfo.FileName = "cmd.exe";
            //是否使用操作系统shell启动
            p.StartInfo.UseShellExecute = false;
            // 接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardInput = true;
            //输出信息
            p.StartInfo.RedirectStandardOutput = true;
            // 输出错误
            p.StartInfo.RedirectStandardError = true;
            //不显示程序窗口
            p.StartInfo.CreateNoWindow = true;
            //启动程序
            p.Start();

            //向cmd窗口发送输入信息
            p.StandardInput.WriteLine(strInput + " && exit");

            p.StandardInput.AutoFlush = true;

            //获取输出信息
            string strOuput = p.StandardOutput.ReadToEnd();
            //等待程序执行完退出进程
            p.WaitForExit();
            p.Close();

            Console.WriteLine(strOuput);
            //MessageBox.Show(strOuput);
            //Console.ReadKey();
            return strOuput;
        }


        // 读取指定key的值 //ConfigurationSettings.AppSettings[“conn”]
        public static string GetConfigValue(string key, string default_value)
        {
            if (System.Configuration.ConfigurationManager.AppSettings[key] == null)
                return default_value;
            else
                return System.Configuration.ConfigurationManager.AppSettings[key].ToString();
        }

        //写入指定key值
        private void SetConfigValue(string key, string new_value)
        {
            //获取Configuration对象
            Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //根据Key读取<add>元素的Value
            if (config.AppSettings.Settings[key] == null)
            {
                //增加<add>元素
                config.AppSettings.Settings.Add(key, new_value);
            }
            else
            {
                //写入<add>元素的Value
                config.AppSettings.Settings[key].Value = new_value;
            }
            //一定要记得保存，写不带参数的config.Save()也可以
            config.Save(ConfigurationSaveMode.Modified);
            //刷新，否则程序读取的还是之前的值（可能已装入内存）
            System.Configuration.ConfigurationManager.RefreshSection("appSettings");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {



            if (textBox4.Text == jmx_cmd)
            {

                SetConfigValue("jmx_name", textBox1.Text);
                SetConfigValue("index", textBox2.Text);
                SetConfigValue("bat_number", textBox3.Text);
                SetConfigValue("jmeter_cmd", textBox4.Text);
            }
            else {

                if (MessageBox.Show("Jmeter启动命令发生变化，是否保存？", "保存", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    SetConfigValue("jmx_name", textBox1.Text);
                    SetConfigValue("index", textBox2.Text);
                    SetConfigValue("bat_number", textBox3.Text);
                    SetConfigValue("jmeter_cmd", textBox4.Text);
                    //Application.Exit();
                }
                else
                {
                    //e.Cancel = true;
                    //Application.Exit();
                }
            }


        }

    }
}
