using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using E.StringEx;
using ECode.Commons;
using static ECode.Commons.TimeEx;

namespace ToNative
{
    public partial class FormToNative : Form
    {
        public FormToNative()
        {
            InitializeComponent();

            EventBind();
        }

        void EventBind()
        {
            this.btnSelectOutPutPath.Click += BtnSelectOutPutPath_Click;
            this.btnSelectSourcePath.Click += BtnSelectSourcePath_Click;
            this.btnAbout.Click += BtnAbout_Click;
            this.btnExec.Click += BtnExec_Click;
            this.btnSelectIcoPath.Click += BtnSelectIcoPath_Click;
            this.btnGetDll.Click += BtnGetDll_Click;

            this.txtSourcePath.TextChanged += TxtSourcePath_TextChanged;
            this.txtOutPutAppName.TextChanged += TxtOutPutAppName_TextChanged;

            txtLog.TextChanged += TxtLog_TextChanged;

            cmbMode.SelectedIndex = 0;
        }

        private void BtnGetDll_Click(object sender, EventArgs e)
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe");
            psi.Arguments = $"/e,{Path.Combine(Application.StartupPath, @"AppData\tonative\tools\备用DLL")}";
            System.Diagnostics.Process.Start(psi);
        }

        private void BtnSelectIcoPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "图标文件|*.ico";
            ofd.Multiselect = false;
            ofd.Title = "选择输出程序图标";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtAppIcoPath.Text = ofd.FileName;
            }
        }

        bool isBtnSelectInPath = false;
        private void BtnSelectSourcePath_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = ".net应用程序|*.exe";
            ofd.Multiselect = false;
            ofd.Title = "选择源程序";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                isBtnSelectInPath = true;
                txtSourcePath.Text = ofd.FileName;
            }
        }

        private void TxtSourcePath_TextChanged(object sender, EventArgs e)
        {
            if (!isBtnSelectInPath)
            {
                // 如果是目录则检测目录下是否存在 .exe 文件
                if (!txtSourcePath.Text.EndsWith(".exe") && Directory.Exists(txtSourcePath.Text))
                {
                    var files = Directory.GetFiles(txtSourcePath.Text);
                    // 遍历检测
                    bool flag = false;
                    foreach (var item in files)
                    {
                        if (item.EndsWith(".exe"))
                        {
                            flag = true;
                            txtOutPutPath.Focus();

                            if (txtSourcePath.Text.EndsWith("\\"))
                                txtSourcePath.Text = item;
                            else
                                txtSourcePath.Text = item;
                            break;
                        }
                    }

                    // 是否检测到项目文件
                    if (!flag)
                    {
                        MessageBox.Show("未找到exe文件!请检查是否存在.exe文件!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                isBtnSelectInPath = false;
            }
        }

        private void BtnSelectOutPutPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txtOutPutPath.Text = fbd.SelectedPath;
            }
        }

        bool isSelfChange = false;
        private void TxtOutPutAppName_TextChanged(object sender, EventArgs e)
        {
            // 如果是自己改变自己的值则不执行过滤
            if (!isSelfChange)
            {
                var tempStr = string.Empty;
                foreach (var item in Path.GetInvalidFileNameChars())
                {
                    tempStr = txtOutPutAppName.Text.Replace(item.ToString(), "");
                }
                isSelfChange = true;
                txtOutPutAppName.Text = tempStr;
                txtOutPutAppName.Select(txtOutPutAppName.Text.Length, 0);
            }
            else
            {
                isSelfChange = false;
            }

        }


        private void BtnAbout_Click(object sender, EventArgs e)
        {
            var txt = File.ReadAllText(@".\AppData\tonative\使用说明.txt", Encoding.Default);
            MessageBox.Show(txt, "关于ToNative");
        }

        private void BtnExec_Click(object sender, EventArgs e)
        {
            txtSourcePath.Text = txtSourcePath.Text.Trim();
            txtOutPutPath.Text = txtOutPutPath.Text.Trim();
            txtOutPutAppName.Text = txtOutPutAppName.Text.Trim();

            if (txtSourcePath.Text.IsNull()
                || txtOutPutPath.Text.IsNull()
                || txtOutPutAppName.Text.IsNull())
            {
                MessageBox.Show("请确认信息都已填入!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Directory.Exists(txtOutPutPath.Text))
            {
                Directory.CreateDirectory(txtOutPutPath.Text);
            }

            var sb = new StringBuilder();

            sb.Append(".\\AppData\\tonative\\ToNative.exe  ");
            switch (cmbMode.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    sb.Append(" -w ");
                    break;
                case 2:
                    sb.Append(" --x32 ");
                    break;
                case 3:
                    sb.Append(" --xp ");
                    break;
                case 4:
                    sb.Append(" --lin ");
                    break;
            }
            var icoPath = txtAppIcoPath.Text.Trim();
            if (!icoPath.IsNull())
            {
                sb.Append($" --ico:\"{txtAppIcoPath.Text.Trim()}\" ");
            }


            sb.Append($"\"{txtSourcePath.Text}\" ");

            var outPath = $"{txtOutPutPath.Text}\\{txtOutPutAppName.Text}".Replace("\\\\", "\\");
            if (!outPath.EndsWith(".exe"))
                sb.Append($" \"{outPath}.exe\"");
            else
                sb.Append($" \"{outPath}\"");

            var cmd = new CMD() { CmdStr = sb.ToString() };
            this.AppendText($"{TimeNow()}  正在执行:\r\n{cmd.CmdStr}\r\n");
            Task.Factory.StartNew(() =>
            {
                if (cmd.Exec(cmd, str => this.AppendText(str)))
                {
                    this.AppendText("==================== 执行完成! ====================\r\n\r\n");
                }
                else
                {
                    this.AppendText($"{TimeNow()}  执行命令出错！错误信息:{cmd.Exception.Message}\r\n{cmd.Exception.ToString()}\r\n");
                    this.AppendText("=================== 执行命令出错 ===================\r\n\r\n");
                }
            });
        }


        private void AppendText(string str)
        {
            this.txtLog.Invoke(new Action<string>((par) =>
            {
                this.txtLog.AppendText(par);
            }), str);
        }


        /// <summary>
        /// 日志文件内容修改时滑动到底部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtLog_TextChanged(object sender, EventArgs e)
        {
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }
    }
}
