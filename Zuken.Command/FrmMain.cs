using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Zuken.Common.Utility;
using Zuken.Common.Net;
using Zuken.Command.Utility;
using Zuken.Command.Entity;
using Zuken.Common;
using System.Diagnostics;
using System.Web;

namespace Zuken.Command
{
    public partial class FrmMain : Form
    {
        public FrmMain(string cmd, string cadtype, string url)
        {
            InitializeComponent();
            this.UserInfo.CadOrder = Tools.ConvertToEnum<IntegrationOrder>(cmd, IntegrationOrder.USERDEFINED);
            this.UserInfo.Apptype = cadtype;
            this.UserInfo.OpUrl = url;
        }
        #region 变量
        /// <summary>
        /// 用户数据
        /// </summary>
        UserInfo UserInfo = new UserInfo();
        /// <summary>
        /// 是否登陆完成
        /// </summary>
        bool IsLoginFinish = false;
        /// <summary>
        /// 自动登陆次数
        /// </summary>
        int AutoLoginCount = 0;
        /// <summary>
        /// 页面的状态视图 __VIEWSTATE
        /// </summary>
        string __VIEWSTATE = string.Empty;
        /// <summary>
        /// 页面的事件数据 __EVENTVALIDATION
        /// </summary>
        string __EVENTVALIDATION = string.Empty;
        #endregion

        #region 方法
        /// <summary>
        /// 登陆
        /// </summary>
        private void onLogin()
        {
            string urlString = string.Format("{1}/login.aspx?{0}IsExternal=1&ReturnUrl=LoginCAD.aspx&language={2}",
                (this.UserInfo.CadOrder == IntegrationOrder.LOGIN) ? "" : "LoginMode=1&",
                this.UserInfo.ServerUrl,
                this.UserInfo.LanguageID);
            this.web.Navigate(urlString);
        }
        /// <summary>
        /// 注销 ??
        /// </summary>
        private void onLogout()
        {
            string str = "?AppId=" + UserInfo.Apptype + "&Operate=NewLogout&UserCode=" + HttpUtility.UrlEncode(UserInfo.UserCode);
            string urlString = this.UserInfo.ServerUrl + "/Common/WebServices.aspx" + str;
            this.web.Navigate(urlString);
        }
        /// <summary>
        /// 注销 后写数据 ??
        /// </summary>
        private void SetLogOut()
        {
            string url = this.web.Url.ToString();
            if (url.IndexOf("WEBSERVICES.ASPX", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                if (this.UserInfo.CadOrder == IntegrationOrder.LOGOUT)
                {
                    int count = this.web.Document.All.Count;
                    for (int i = 0; i < count; i++)
                    {
                        HtmlElement htmlElement = this.web.Document.All[i];
                        string outerText = htmlElement.OuterText;
                        if (!string.IsNullOrEmpty(outerText))
                        {
                            if (outerText.IndexOf("INTEGRATIONLOGOUTSUCCESS") >= 0)
                            {
                                UserInfo.Save(string.Empty, string.Empty);
                                base.Close();
                            }
                            if (outerText.IndexOf("INTEGRATIONLOGOUTFAILD") >= 0)
                            {
                                base.Close();
                            }
                        }
                    }
                }
                this.web.Visible = true;
                this.picLoad.Visible = false;
            }
        }
        /// <summary>
        /// 打开PLM
        /// </summary>
        private void onOpenPlm()
        {
            string path = Path.Combine(Tools.ZukenAppDataPath, "Login.Html");
            WriteDateFile(path);
            Process.Start("iexplore", path);
            base.Close();
        }
        /// <summary>
        /// 查找文档
        /// </summary>
        private void onDocFind()
        {
            string urlString = this.UserInfo.ServerUrl + "/Common/DocumentQuery.aspx?AppId=" + this.UserInfo.Apptype;
            this.web.Navigate(urlString);
        }
        /// <summary>
        /// 查找物料
        /// </summary>
        private void onMatFind()
        {
            string urlString = this.UserInfo.ServerUrl + "/Common/MaterialQuery.aspx?AppId=" + this.UserInfo.Apptype;
            this.web.Navigate(urlString);
        }
        /// <summary>
        /// 用户自定义操作
        /// </summary>
        private void onUserDefined()
        {
            string url = this.UserInfo.OpUrl;
            if (!string.IsNullOrEmpty(url))
            {
                if (url.IndexOf("?") < 0)
                {
                    url = url + "?AppId=" + this.UserInfo.Apptype;
                }
                else
                {
                    url = url + "&AppId=" + this.UserInfo.Apptype;
                }
            }
            else
            {
                url = "/ExceptionPage.aspx";
            }
            string urlString = this.UserInfo.ServerUrl + url;
            this.web.Navigate(urlString);
        }
        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="path"></param>
        private void WriteDateFile(string path)
        {
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(path, false, Encoding.Default))
                {
                    streamWriter.WriteLine("<!-- saved from url=(0009)http://a/ -->");
                    streamWriter.WriteLine("<html>");
                    streamWriter.WriteLine("<script type=\"text/javascript\">");
                    streamWriter.WriteLine("window.onload = function(){");
                    streamWriter.WriteLine("document.getElementById('btnLogin').click();");
                    streamWriter.WriteLine("}");
                    streamWriter.WriteLine("</script>");
                    streamWriter.WriteLine("<form id=\"form1\" method=\"post\" action=\"" + this.UserInfo.ServerUrl + "/Login.aspx\">");
                    streamWriter.WriteLine("<input type=\"hidden\" name=\"__VIEWSTATE\" id=\"__VIEWSTATE\" value=\"" + this.__VIEWSTATE + "\" />");
                    streamWriter.WriteLine("<input type=\"hidden\" name=\"__EVENTVALIDATION\" id=\"__EVENTVALIDATION\" value=\"" + this.__EVENTVALIDATION + "\" />");
                    streamWriter.WriteLine("<input name='__EVENTTARGET' type='hidden' value='' />");
                    streamWriter.WriteLine("<input name='__EVENTARGUMENT' type='hidden' value='' />");
                    streamWriter.WriteLine("<input name='hidControls' type='hidden' value='' />");
                    streamWriter.WriteLine("<input name='hidControlVers' type='hidden' value='' />");
                    streamWriter.WriteLine("<input name='hidSetups' type='hidden' value='' />");
                    streamWriter.WriteLine("<input name='hidSetupVers' type='hidden' value='' />");
                    streamWriter.WriteLine("<input name='hdDisplayType' type='hidden' value='0' />");
                    streamWriter.WriteLine("<input name='hdLanguageType' type='hidden' value='" + this.UserInfo.LanguageID + "' />");
                    streamWriter.WriteLine("<input name='hdClientSysLanType' type='hidden' value='" + this.UserInfo.LanguageID + "' />");
                    streamWriter.WriteLine("<input name='LoginSuccess' type='hidden' value='' />");
                    streamWriter.WriteLine("<input name='LoginResponse' type='hidden' value='' />");
                    streamWriter.WriteLine("<input name='hidClientDate' type='hidden' value='9/F4/F2012+10%3A46%3A52' />");
                    streamWriter.WriteLine("<input name='isExcucte' type='hidden' value='' />");
                    streamWriter.WriteLine("<input name='isPostBack' type='hidden' value='0' />");
                    streamWriter.WriteLine("<input name='txtUserName' type='hidden' type='text' value='" + HttpUtility.UrlEncode(this.UserInfo.UserCode) + "' />");
                    streamWriter.WriteLine("<input name='txtPassword'type='hidden' type='text' value='" + this.UserInfo.Password + "' />");
                    streamWriter.WriteLine("<input name='ddlDomain'type='hidden' type='text' value='local' />");
                    streamWriter.WriteLine("<input name='btnLogin' style='display:none;' type='submit' value='%E7%99%BB+%E5%BD%95' />");
                    streamWriter.WriteLine("</form>");
                    streamWriter.WriteLine("</html>");
                    streamWriter.Close();
                }
            }
            catch
            {
            }
        }
        /// <summary>
        /// 获取登陆信息
        /// </summary>
        private void GetLoginString()
        {
            string url = this.web.Url.ToString();
            if (url.IndexOf("LOGIN.ASPX", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                FindControl("__VIEWSTATE", el => __VIEWSTATE = el.GetAttribute("Value"));
                FindControl("__EVENTVALIDATION", el => __EVENTVALIDATION = el.GetAttribute("Value"));
            }
        }

        /// <summary>
        /// 完成登陆后的操作
        /// </summary>
        private void OnLoginFinish()
        {
            switch (UserInfo.CadOrder)
            {
                case IntegrationOrder.LOGIN:
                    this.onLogin();
                    break;
                case IntegrationOrder.LOGOUT:
                    this.onLogout();
                    break;
                case IntegrationOrder.OPENPLM:
                case IntegrationOrder.CHECKOUT:
                case IntegrationOrder.GM:
                case IntegrationOrder.SETRIGHT:
                    this.onOpenPlm();
                    break;
                case IntegrationOrder.DOCFIND:
                    this.onDocFind();
                    break;
                case IntegrationOrder.MATFIND:
                    this.onMatFind();
                    break;
                case IntegrationOrder.CHECKIN:
                case IntegrationOrder.WORKSPACE:
                case IntegrationOrder.PROJECT:
                case IntegrationOrder.TASK:
                case IntegrationOrder.USERDEFINED:
                    this.onUserDefined();
                    break;
            }
        }
        /// <summary>
        /// 自动登陆
        /// </summary>
        private void AutoLogin()
        {
            if (!this.IsLoginFinish)
            {
                if (this.UserInfo.CadOrder == IntegrationOrder.LOGIN)
                {
                    if (this.HasElement("DesktopTop"))
                    {
                        this.IsLoginFinish = true;
                        base.Hide();
                        this.SaveUser();
                        MessageBox.Show("登录成功!", _.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        base.Close();
                    }
                    string loginReturn = this.GetLoginReturn();
                    if (!string.IsNullOrEmpty(loginReturn))
                    {
                        MessageBox.Show(loginReturn, _.Title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    if (this.AutoLoginCount > 3)
                    {
                        MessageBox.Show("用户验证失败,请检查后重新登录!", _.Title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        base.Close();
                    }
                    if (this.HasElement("btnLogin"))
                    {
                        this.AutoLoginCount++;
                        this.LoginToPlm();
                    }
                    else
                    {
                        if (this.HasElement("DesktopTop"))
                        {
                            this.IsLoginFinish = true;
                            this.OnLoginFinish();
                        }
                        else
                        {
                            this.web.Visible = true;
                            this.picLoad.Visible = false;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 登陆plm
        /// </summary>
        private void LoginToPlm()
        {
            FindControl("txtUserName", el => el.SetAttribute("Value", UserInfo.UserCode));
            FindControl("txtPassword", el => el.SetAttribute("Value", UserInfo.Password));
            FindControl("btnLogin", el => el.InvokeMember("click"));
        }
        /// <summary>
        /// 获取返回信息
        /// </summary>
        /// <returns></returns>
        private string GetLoginReturn()
        {
            string result = string.Empty;
            FindControl("LoginResponse", el => result = el.GetAttribute("Value"));
            return result;
        }
        /// <summary>
        /// 是否有元素
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        private bool HasElement(string elementName)
        {
            return null != this.web.Document.GetElementById(elementName);
        }
        /// <summary>
        /// 保存用户信息
        /// </summary>
        private void SaveUser()
        {
            string usercode = UserInfo.UserCode;
            string pwd = UserInfo.Password;
            //FindControl("hidPassWord", el => pwd = el.GetAttribute("Value"));
            //FindControl("hidUserCode", el => usercode = el.GetAttribute("Value"));
            if (!string.IsNullOrEmpty(usercode) && !string.IsNullOrEmpty(pwd))
            {
                UserInfo.Save(Security.EncryptText(usercode), Security.EncryptText(pwd));
            }
        }
        /// <summary>
        /// 改变事件的执行
        /// </summary>
        private void ChangeCloseEvent()
        {
            FindControls(new string[] { "btnExit", "btnClose", "btnCancel" },
                el => { el.Click += new HtmlElementEventHandler(htmlElement_Click); return false; });
        }

        void htmlElement_Click(object sender, HtmlElementEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        ///　查找控件
        /// </summary>
        void FindControl(string id, Action<HtmlElement> func)
        {
            var htmlElement = web.Document.GetElementById(id);
            if (htmlElement != null)
            {
                func(htmlElement);
            }
        }
        /// <summary>
        /// 查找所有控件
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="func"></param>
        void FindControls(IEnumerable<string> ids, Func<HtmlElement, bool> func)
        {
            foreach (var item in ids)
            {
                var htmlElement = web.Document.GetElementById(item);
                if (htmlElement != null)
                {
                    if (!func(htmlElement)) break;
                }
            }
        }
        #endregion

        #region 事件
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void web_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.ToString().IndexOf("Login.aspx", StringComparison.OrdinalIgnoreCase) > -1)
            {
                FindControl("txtUserName", el => UserInfo.UserCode = el.GetAttribute("Value"));
                FindControl("txtPassword", el => UserInfo.Password = el.GetAttribute("Value"));
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (this.UserInfo.CadOrder == IntegrationOrder.LOGOUT)
            {
                this.onLogout();
            }
            else
            {
                this.onLogin();
            }
            this.web.Visible = false;
            this.picLoad.Visible = true;
        }

        private void web_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (this.web.Url.ToString().Trim() == "about:blank")
            {
                this.web.Document.Body.InnerHtml = "<div style='width:100%;height:100%;color:#00f;text-align:center;padding-top:200px'>loading...</div>";
            }
            else
            {
                if (this.web.ReadyState == WebBrowserReadyState.Complete)
                {
                    if (this.UserInfo.CadOrder == IntegrationOrder.OPENPLM)
                    {
                        this.GetLoginString();
                    }
                    if (this.UserInfo.CadOrder == IntegrationOrder.LOGOUT)
                    {
                        this.SetLogOut();
                    }
                    else
                    {
                        this.AutoLogin();
                        this.ChangeCloseEvent();
                        string url = this.web.Url.ToString();
                        if (this.UserInfo.CadOrder == IntegrationOrder.LOGIN)
                        {
                            this.web.Visible = true;
                            this.picLoad.Visible = false;
                        }
                        if (url.IndexOf("LOGINCAD.ASPX", StringComparison.OrdinalIgnoreCase) < 0)
                        {
                            this.web.Visible = true;
                            this.picLoad.Visible = false;
                        }
                    }
                }
            }
        }
        #endregion

    }
}

#if false
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            string uname = IniHelper.IniReadValue(_.UserNameKey);
            if (!string.IsNullOrEmpty(uname))
            {
                txt_UserName.Text = Security.DecryptText(uname);
            }
        }

        private void btn_Login_Click(object sender, EventArgs e)
        {
            string username = txt_UserName.Text;
            string pwd = txt_Pwd.Text;
            string serverurl = IniHelper.IniReadValue(_.ServerUrl);

            string loginurl = string.Format("{0}/Common/WebServices.aspx?Operate=login&txtUserName={1}&txtPassword={2}",
                serverurl, HttpUtility.UrlEncode(username), HttpUtility.UrlEncode(pwd));

            string data = WebPageHelper.GetData(loginurl, null, HttpHeader.DefaultHeader);
            if (data.Trim().StartsWith("ok;", StringComparison.OrdinalIgnoreCase))
            {
                IniHelper.IniWriteValue(_.UserNameKey, Security.EncryptText(username));
                IniHelper.IniWriteValue(_.Pwdkey, Security.EncryptText(pwd));
                MessageBox.Show("登录成功！", "提示");
                this.Close();
            }
            else
            {
                MessageBox.Show("登录失败！", "提示");
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
#endif
