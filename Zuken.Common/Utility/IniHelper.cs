using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Zuken.Common.Utility
{
    /// <summary>
    /// ini操作类
    /// </summary>
    public static class IniHelper
    {
        static string _iniPath = string.Empty;
        /// <summary>
        /// ini文档的名称
        /// </summary>
        const string IniFileName = "LoginSetting.ini";
        /// <summary>
        /// ini文档的默认节点
        /// </summary>
        const string IniDefaulrSection = "PLM Setting";
        /// <summary>
        /// ini文件的路径
        /// </summary>
        public static string IniPath
        {
            get
            {
                if (string.IsNullOrEmpty(_iniPath))
                {
                    return System.IO.Path.Combine(Tools.ZukenAppDataPath, IniFileName);
                }
                return _iniPath;
            }
        }

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        /// <summary>
        /// 在PLM Setting 节点下写值
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        public static void IniWriteValue(string Key, string Value)
        {
            IniWriteValue(IniDefaulrSection, Key, Value);
        }
        /// <summary>
        /// 在指定的节点下写值
        /// </summary>
        /// <param name="section"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        public static void IniWriteValue(string section, string Key, string Value)
        {
            WritePrivateProfileString(section, Key, Value, IniPath);
        }
        /// <summary>
        /// 获取指定的节点下指定的值
        /// </summary>
        /// <param name="section"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static string IniReadValue(string section, string Key)
        {
            string result = string.Empty;
            try
            {
                StringBuilder stringBuilder = new StringBuilder(255);
                int privateProfileString = GetPrivateProfileString(section, Key, "", stringBuilder, 255, IniPath);
                result = stringBuilder.ToString().Trim();
            }
            catch
            {
                result = string.Empty;
            }
            return result;
        }
        /// <summary>
        /// 获取指定的节点下指定的值
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static string IniReadValue(string Key)
        {
            return IniReadValue(IniDefaulrSection, Key);
        }
    }
}
