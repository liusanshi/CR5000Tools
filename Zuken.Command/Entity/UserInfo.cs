using System;
using Zuken.Common.Utility;
using Zuken.Command.Utility;

namespace Zuken.Command.Entity
{
    public class UserInfo    
	{
		private string m_ServerUrl = string.Empty;
		private string m_Apptype = string.Empty;
		private string m_UserCode = string.Empty;
		private string m_Password = string.Empty;
        private IntegrationOrder m_CadOrder;
		private string m_LanguageID = string.Empty;
		private string m_OpUrl = string.Empty;
        /// <summary>
        /// 服务器url
        /// </summary>
		public string ServerUrl
		{
			get
			{
				return this.m_ServerUrl;
			}
			set
			{
				this.m_ServerUrl = value;
			}
		}
		/// <summary>
		/// 用户编码
		/// </summary>
        public string UserCode
		{
			get
			{
				return this.m_UserCode;
			}
			set
			{
				this.m_UserCode = value;
			}
		}
        /// <summary>
        /// 密码
        /// </summary>
		public string Password
		{
			get
			{
				return this.m_Password;
			}
			set
			{
				this.m_Password = value;
			}
		}
        /// <summary>
        /// Apptype
        /// </summary>
		public string Apptype
		{
			get
			{
				return this.m_Apptype;
			}
			set
			{
				this.m_Apptype = value;
			}
		}
        /// <summary>
        /// 命令
        /// </summary>
        public IntegrationOrder CadOrder
		{
			get
			{
				return this.m_CadOrder;
			}
			set
			{
				this.m_CadOrder = value;
			}
		}
        /// <summary>
        /// 语言
        /// </summary>
		public string LanguageID
		{
			get
			{
				return this.m_LanguageID;
			}
			set
			{
				this.m_LanguageID = value;
			}
		}
        /// <summary>
        /// 操作的url
        /// </summary>
		public string OpUrl
		{
			get
			{
				return this.m_OpUrl;
			}
			set
			{
				this.m_OpUrl = value;
			}
		}
        public UserInfo()
		{
            string usercode = IniHelper.IniReadValue(_.UserNameKey);
            string pwd = IniHelper.IniReadValue(_.Pwdkey);
            this.ServerUrl = IniHelper.IniReadValue(_.ServerUrl);
			if (!string.IsNullOrEmpty(usercode))
			{
				this.UserCode = Security.DecryptText(usercode);
			}
            if (!string.IsNullOrEmpty(pwd))
			{
				this.Password = Security.DecryptText(pwd);
			}
            if (string.IsNullOrEmpty(this.ServerUrl))
			{
				this.ServerUrl = "http://Localhost/PLM";
			}
			this.m_LanguageID = "chs";
		}

        /// <summary>
        /// 保存账号密码
        /// </summary>
        /// <param name="u"></param>
        /// <param name="p"></param>
        public static void Save(string u, string p)
        {
            IniHelper.IniWriteValue(_.UserNameKey, u);
            IniHelper.IniWriteValue(_.Pwdkey, p);
        }
	}
}
