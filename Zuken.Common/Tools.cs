using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

using Zuken.Common.Template;

namespace Zuken.Common
{
    /// <summary>
    /// 工具
    /// </summary>
    public static class Tools
    {
        static Tools()
        {
            CR5000ZLOCALROOT = WRegisterTool.GetRegistryValue(WRegisterRootKeyType.HKEY_LOCAL_MACHINE,
                           @"SOFTWARE\ZUKEN\CR-5000 Runtime\11.0", "ZLOCALROOT");
            ZukenAppDataPath = Environment.GetEnvironmentVariable("ZUKEN_PLM", EnvironmentVariableTarget.Machine);
            FileNamePath = Path.Combine(ZukenAppDataPath, "zk_filename.data");
            BOMFilePath = Path.Combine(ZukenAppDataPath, "zk_bomfile.data");
        }
        /// <summary>
        /// 返回 注册表值
        /// HKEY_LOCAL_MACHINE\SOFTWARE\ZUKEN\CR-5000 Runtime\11.0\ZLOCALROOT
        /// </summary>
        public static readonly string CR5000ZLOCALROOT;
        /// <summary>
        /// 获取Zuken 的 AppData路径
        /// </summary>
        public static readonly string ZukenAppDataPath;
        /// <summary>
        /// 文件名称路径
        /// </summary>
        public static readonly string FileNamePath;
        /// <summary>
        /// Bom文件的路径
        /// </summary>
        public static readonly string BOMFilePath;
        /// <summary>
        /// 日志文件
        /// </summary>
        private static string LogFilePath
        {
            get
            {
                return Path.Combine(ZukenAppDataPath, DateTime.Now.ToString("yyyy-MM-dd") + ".log");
            }
        }

        /// <summary>
        /// 获取文件内容
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetFileContent(string path)
        {
            if (File.Exists(path))
                using (StreamReader reader = new StreamReader(path, Encoding.Default))
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        yield return line;
                    }
                }
        }

        /// <summary>
        /// 转换为枚举类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="defaultvalue"></param>
        /// <returns></returns>
        public static T ConvertToEnum<T>(string name, T defaultvalue) where T : struct
        {
            if (string.IsNullOrEmpty(name)) return defaultvalue;
            try
            {
                return (T)Enum.Parse(typeof(T), name, true);
            }
            catch
            {
                return defaultvalue;
            }
        }

        /// <summary>
        /// 保存项目路径
        /// </summary>
        /// <param name="projpath"></param>
        public static void SaveProjectPath(string projpath)
        {
            File.WriteAllText(FileNamePath, projpath, Encoding.Default);
        }
        /// <summary>
        /// 获取项目路径
        /// </summary>
        /// <returns></returns>
        public static string GetProjectPath()
        {
            foreach (var item in GetFileContent(FileNamePath))
            {
                return item;
            }
            return string.Empty;
        }
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="ex"></param>
        public static void Log(Exception ex)
        {
            StringBuilder content = new StringBuilder(300);
            content.AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            TotelMesg(ex, content);
            content.AppendLine("===================================================");
            File.AppendAllText(LogFilePath, content.ToString());
        }

        /// <summary>
        /// 汇总异常信息
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="str"></param>
        private static void TotelMesg(Exception ex, StringBuilder str)
        {
            if (ex.InnerException != null)
                TotelMesg(ex.InnerException, str);
            str.AppendLine(ex.Message);
            str.AppendLine(ex.Source);
            str.AppendLine(ex.StackTrace);
            str.AppendLine();
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="messgae"></param>
        public static void Log(string messgae)
        {
            StringBuilder content = new StringBuilder(100);
            content.AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            content.AppendLine(messgae);
            content.AppendLine("===================================================");
            File.AppendAllText(LogFilePath, content.ToString());
        }

        /// <summary>
        /// 将Url编码 防止中文乱码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UrlEncode(string str)
        {
            StringBuilder stringBuilder = new StringBuilder();
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            for (int i = 0; i < bytes.Length; i++)
            {
                stringBuilder.Append("%");
                stringBuilder.Append(Convert.ToString(bytes[i], 16));
            }
            return stringBuilder.ToString();
        }

        #region 系统部分
        /// <summary>
        /// 执行应用程序
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="args"></param>
        public static void ShellExecute(string filepath, string args)
        {
            Process.Start(filepath, args);
        }

        /// <summary>
        /// 指定的应用程序是否在运行
        /// </summary>
        /// <param name="processname"></param>
        /// <returns></returns>
        public static bool ApplicationIsRun(string processname)
        {
            try
            {
                var procs = Process.GetProcessesByName(processname);
                return procs != null && procs.Length > 0;
            }
            catch
            {
                return true;
            }
        }
        #endregion

        #region Path
        /// <summary>
        /// 获取上几级的文档路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public static string GetDirectoryName(string path, int depth)
        {
            var dir = path;
            for (int i = 0; i < depth; i++)
            {
                dir = Path.GetDirectoryName(dir);
                if (string.IsNullOrEmpty(dir))
                    return string.Empty;
            }
            return dir;
        }

        #endregion
    }
}
