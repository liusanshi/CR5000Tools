using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

using Zuken.Common.Utility;
using Zuken.Command.Utility;

namespace Zuken.Command
{
    public partial class FrmConfig : Form
    {
        public FrmConfig()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            string surl = IniHelper.IniReadValue(_.ServerUrl).Trim();
            string uname = IniHelper.IniReadValue(_.UserNameKey).Trim();
            string pwd = IniHelper.IniReadValue(_.Pwdkey).Trim();
            if (!string.IsNullOrEmpty(surl))
            {
                this.txtUrl.Text = surl;
            }
            if (!string.IsNullOrEmpty(uname) && !string.IsNullOrEmpty(pwd))
            {
                this.txtUser.Text = "用户 ‘" + Security.DecryptText(uname) + "’ 已登录";
            }
            else
            {
                this.txtUser.Text = "未登录";
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var url = this.txtUrl.Text.Trim();
            if (!string.IsNullOrEmpty(url))
            {
                url = CheckUrl(url);
                if (url.Length == 0)
                {
                    MessageBox.Show("服务器地址输入错误！", "提示");
                    txtUrl.Focus();
                }
                else
                {
                    IniHelper.IniWriteValue(_.ServerUrl, url);
                    MessageBox.Show("保存成功！", "提示");
                }
            }
            else
            {
                MessageBox.Show("请输入PLM系统URL", "提示");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 检查url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        string CheckUrl(string url)
        {
            if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            {
                return url.TrimEnd('/');
            }
            return string.Empty;
        }
    }
}
