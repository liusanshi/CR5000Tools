using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;

using Kingdee.PLM.Integration.Setup.Abstract;
using Kingdee.PLM.Integration.Setup.Common;

namespace Zuken.Setup
{
    /// <summary>
    /// 集成插件安装方法
    /// </summary>
    public class ZukenSetup : InstallInterface
    {
        private StringBuilder CheckString = new StringBuilder();

        /// <summary>
        /// 安装目录
        /// </summary>
        private string SetupUserDir
        {
            get { return Path.Combine(this.UserDefaultDir, this.DirName); }
        }
        /// <summary>
        /// 安装文件路径
        /// </summary>
        private string SetupSourceDir
        {
            get { return Path.Combine(this.SourceDefaultDir, this.DirName); }
        }
        /// <summary>
        /// 插件源路径
        /// </summary>
        private string CommonDllSourcePath
        {
            get { return Path.Combine(this.SourceDefaultDir, "Common\\Dll"); }
        }
        /// <summary>
        /// 插件源路径
        /// </summary>
        private string CommonSourcePath
        {
            get { return Path.Combine(this.SourceDefaultDir, "Common"); }
        }
        /// <summary>
        /// 插件路径
        /// </summary>
        private string CommonDllUserPath
        {
            get { return Path.Combine(this.UserDefaultDir, "Common\\Dll"); }
        }

        /// <summary>
        /// 环境变量
        /// </summary>
        const string ZUKEN_PLM = "ZUKEN_PLM";

        /// <summary>
        /// 资源文件
        /// </summary>
        readonly string[] ResourcesFiles = new string[] { 
            "Zuken.Common.dll", "Zuken.Command.exe", "openCADFileManage.js",
            "openCADFileManage.vbs", "proj.tpl", "PCBColumnConfig.txt", "Vici.Core.dll"};

        /// <summary>
        /// 菜单命令
        /// </summary>
        readonly string[] MenuCmd = new string[] {
            @" (""plmlogin"" ""Log On PLM"" Shell ""Zuken.Command.exe $data_file_path LOGIN ZK"" NoQuery NoWindow NoWait)" 
           ,@" (""logoutplm"" ""Logout PLM"" Shell ""Zuken.Command.exe $data_file_path LOGOUT ZK"" NoQuery NoWindow NoWait)"
           ,@" (""openplm"" ""Open PLM"" Shell ""Zuken.Command.exe $data_file_path OPENPLM ZK"" NoQuery NoWindow NoWait)"
           ,@" (""checkin"" ""Checkin File"" Shell ""Zuken.Command.exe $data_file_path CHECKIN ZK /Document/ElectronIntegration.aspx?Optype=1"" NoQuery NoWindow NoWait)"
           ,@" (""importworkarea"" ""Import To Workarea"" Shell ""Zuken.Command.exe $data_file_path WORKSPACE ZK /Document/ElectronIntegration.aspx?Optype=2"" NoQuery NoWindow NoWait)"
           ,@" (""importproject"" ""Import To Project Ouput"" Shell ""Zuken.Command.exe $data_file_path PROJECT ZK /Document/ElectronIntegration.aspx?Optype=3"" NoQuery NoWindow NoWait)"
           ,@" (""importtask"" ""Import To Task Ouput"" Shell ""Zuken.Command.exe $data_file_path TASK ZK /Document/ElectronIntegration.aspx?Optype=4"" NoQuery NoWindow NoWait)"
           ,@" (""plmconfig"" ""Config"" Shell ""Zuken.Command.exe $data_file_path CONFIG ZK"" NoQuery NoWindow NoWait)"
            };

        #region 方法
        /// <summary>
        /// 检查主方法
        /// </summary>
        /// <returns></returns>
        public string CheckMain()
        {
            this.CheckString.Remove(0, this.CheckString.Length);
            string ZukenPath = this.GetSetupPath();
            string MenuPath = this.GetMenuPath();
            if (string.IsNullOrEmpty(ZukenPath) || string.IsNullOrEmpty(MenuPath))
            {
                this.CheckString.Append("未安装CR5000软件或不支持的CR5000版本。\r\n");
            }
            this.GetSoftOpen(AddCheckProcess(), false);
            return this.CheckString.ToString();
        }

        /// <summary>
        /// 获取CR5000安装路径
        /// </summary>
        /// <returns></returns>
        private string GetSetupPath()
        {
            string text = string.Empty;
            string oSBit = CommonBase.GetOSBit();
            string subitem = @"SOFTWARE\ZUKEN\CR-5000 Runtime\";
            if (oSBit == "64")
            {
                throw new Exception("暂时不支持64位操作系统！");
                //subitem = "SOFTWARE\\Wow6432Node\\ZUKEN\\CR-5000 Runtime\\";
            }
            text = CommonBase.GetRegistMachineKeyValue(subitem, "CurrentRoot", false, true);
            if (string.IsNullOrEmpty(text))
            {
                text = CommonBase.GetRegistMachineKeyValue(subitem + "11.0\\", "CR5ROOT", false, true);
            }
            return text;
        }

        /// <summary>
        /// 菜单文件
        /// C:\cr5000\home\cr5000\sys\zfmcustm.rsc
        /// </summary>
        /// <returns></returns>
        private string GetMenuPath()
        {
            string text = string.Empty;
            //string oSBit = CommonBase.GetOSBit();
            string subitem = @"SOFTWARE\ZUKEN\CR-5000 Runtime\11.0\";
            //if (oSBit == "64")
            //{
            //    throw new Exception("暂时不支持64位操作系统！");
            //    subitem = "SOFTWARE\\Wow6432Node\\ZUKEN\\CR-5000 Runtime\\11.0\\";
            //}
            text = CommonBase.GetRegistMachineKeyValue(subitem, "HOME", false, true);
            if (!string.IsNullOrEmpty(text))
            {
                text = string.Format("{0}\\cr5000\\sys\\zfmcustm.rsc", text);
            }
            return text;
        }

        /// <summary>
        /// 菜单文件
        /// C:\cr5000\local\zsys\info\eng\zfmcustm.rsc
        /// </summary>
        /// <returns></returns>
        private string GetMenuPath2()
        {
            string text = string.Empty;
            //string oSBit = CommonBase.GetOSBit();
            string subitem = @"SOFTWARE\ZUKEN\CR-5000 Runtime\11.0\";
            //if (oSBit == "64")
            //{
            //    throw new Exception("暂时不支持64位操作系统！");
            //    subitem = "SOFTWARE\\Wow6432Node\\ZUKEN\\CR-5000 Runtime\\11.0\\";
            //}
            text = CommonBase.GetRegistMachineKeyValue(subitem, "ZLOCALROOT", false, true);
            if (!string.IsNullOrEmpty(text))
            {
                text = string.Format("{0}\\zsys\\info\\eng\\zfmcustm.rsc", text);
            }
            return text;
        }

        /// <summary>
        /// 检查软件是否打开
        /// </summary>
        /// <param name="ExeName"></param>
        /// <param name="IsRemove"></param>
        private void GetSoftOpen(IEnumerable<string> ExeName, bool IsRemove)
        {
            foreach (var item in ExeName)
            {
                bool flag = CommonBase.CheckProcessStart(item);
                if (flag)
                {
                    if (IsRemove)
                    {
                        this.CheckString.Append(item + "正在运行,请先关闭再进行卸载。\r\n");
                    }
                    else
                    {
                        this.CheckString.Append(item + "正在运行,请先关闭再进行安装。\r\n");
                    }
                }
            }
        }
        /// <summary>
        /// 需要检查的进程
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> AddCheckProcess()
        {
            return new string[]
	        {
		        "DXP",
		        "iexplore",
		        "IntegrationLogin"
	        };
        }

        /// <summary>
        /// 删除文档
        /// </summary>
        private void DeleteFile()
        {
            string setupuserdir = SetupUserDir + "\\";
            string commondlluserdir = CommonDllUserPath + "\\";

            CommonBase.DeleteFileNoException(commondlluserdir + "Kingdee.PLM.Integration.Client.Zuken.dll");
            CommonBase.DeleteFileNoException(commondlluserdir + "Zuken.Common.dll");

            foreach (var item in ResourcesFiles)
            {
                CommonBase.DeleteFileNoException(setupuserdir + item);
            }
            CommonBase.DeleteFileNoException(setupuserdir + "IntegrationLogin.exe");
            CommonBase.DeleteFileNoException(setupuserdir + "IntegrationLogin.exe.config");
            this.RemoveMenuFile();
            UnInstallEnvironment();
        }

        /// <summary>
        /// 复制文档
        /// </summary>
        void CopyFile()
        {
            string setupuserdir = SetupUserDir + "\\";
            string setupsourdir = SetupSourceDir + "\\";
            if (!Directory.Exists(setupuserdir))
            {
                Directory.CreateDirectory(setupuserdir);
            }
            InstallEnvironment();

            //复制文件
            CommonBase.CopyFile(CommonDllSourcePath + "\\Kingdee.PLM.Integration.Client.Zuken.dll", CommonDllUserPath + "\\Kingdee.PLM.Integration.Client.Zuken.dll");
            CommonBase.CopyFile(setupsourdir + "Zuken.Common.dll", CommonDllUserPath + "\\Zuken.Common.dll");

            foreach (var item in ResourcesFiles)//必要的资源文件
            {
                CommonBase.CopyFile(setupsourdir + item, setupuserdir + item);
            }

            CommonBase.CopyFile(CommonSourcePath + "\\IntegrationLogin.exe", setupuserdir + "IntegrationLogin.exe");
            CommonBase.CopyFile(CommonSourcePath + "\\IntegrationLogin.exe.config", setupuserdir + "IntegrationLogin.exe.config");
        }

        #region 菜单
        /// <summary>
        /// 安装菜单
        /// </summary>
        void SetUpMenuFile()
        {
            AddMenu(GetMenuPath());
            AddMenu(GetMenuPath2());
        }
        /// <summary>
        /// 卸载菜单文件
        /// </summary>
        void RemoveMenuFile()
        {
            DelMenu(GetMenuPath());
            DelMenu(GetMenuPath2());
        }
        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <param name="filepath"></param>
        void AddMenu(string filepath)
        {
            DelMenu(filepath);
            FileInfo fi = new FileInfo(filepath);
            if (!fi.Exists) return;
            fi.IsReadOnly = false;
            string[] text = File.ReadAllLines(filepath, Encoding.Default);
            List<string> data = new List<string>(text.Length + MenuCmd.Length);
            bool isadd = false;
            foreach (var item in text)
            {
                data.Add(item);
                if (!isadd && IsBegin(item))
                {
                    isadd = true;
                    data.AddRange(MenuCmd);
                }
            }
            File.WriteAllLines(filepath, data.ToArray());
        }
        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="filepath"></param>
        void DelMenu(string filepath)
        {
            FileInfo fi = new FileInfo(filepath);
            if (!fi.Exists) return;
            fi.IsReadOnly = false;
            string[] text = File.ReadAllLines(filepath, Encoding.Default);
            List<string> data = new List<string>(text.Length);
            bool isbegin = false, isend = false;
            foreach (var item in text)
            {
                if (!isbegin && IsBegin(item))
                {
                    isbegin = true;
                }
                if (!isend && IsEnd(item))
                {
                    isend = true;
                }
                if (isbegin && !isend && IsMenu(item))
                {
                    continue;
                }
                data.Add(item);
            }
            File.WriteAllLines(filepath, data.ToArray());
        }

        /// <summary>
        /// 是否开始
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        bool IsBegin(string text)
        {
            text = text.Trim();
            if (string.Compare("Tool {", text, true) == 0)
                return true;
            else if (text.StartsWith("Tool", StringComparison.OrdinalIgnoreCase) && text.EndsWith("{"))
            {
                return string.IsNullOrEmpty(text.Substring(4, text.Length - 5).Trim());
            }
            return false;
        }
        /// <summary>
        /// 是否结束
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        bool IsEnd(string text)
        {
            return string.Compare(text.Trim(), "}") == 0;
        }
        /// <summary>
        /// 是否菜单
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        bool IsMenu(string text)
        {
            foreach (var item in MenuCmd)
            {
                if (string.Compare(item, text, true) == 0)
                    return true;
            }
            return false;
        }
        #endregion


        /// <summary>
        /// 安装安静变量
        /// </summary>
        void InstallEnvironment()
        {
            string zkplm = Environment.GetEnvironmentVariable(ZUKEN_PLM, EnvironmentVariableTarget.Machine);
            if (string.Compare(zkplm, SetupUserDir, true) != 0)
                Environment.SetEnvironmentVariable(ZUKEN_PLM, SetupUserDir, EnvironmentVariableTarget.Machine);
            string path = Environment.GetEnvironmentVariable("path", EnvironmentVariableTarget.Machine);
            path = path.TrimEnd(';') + ";";
            if (path.IndexOf(SetupUserDir, StringComparison.OrdinalIgnoreCase) == -1)
            {
                Environment.SetEnvironmentVariable("path", path + SetupUserDir, EnvironmentVariableTarget.Machine);
            }
        }
        /// <summary>
        /// 卸载环境变量
        /// </summary>
        void UnInstallEnvironment()
        {
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(ZUKEN_PLM, EnvironmentVariableTarget.Machine)))
                Environment.SetEnvironmentVariable(ZUKEN_PLM, string.Empty, EnvironmentVariableTarget.Machine);
            string path = Environment.GetEnvironmentVariable("path", EnvironmentVariableTarget.Machine);
            path = path.TrimEnd(';') + ";";
            if (path.IndexOf(SetupUserDir, StringComparison.OrdinalIgnoreCase) > -1)
            {
                Environment.SetEnvironmentVariable("path", path.Replace(SetupUserDir, "").TrimEnd(';'), EnvironmentVariableTarget.Machine);
            }
        }
        #endregion

        #region InstallInterface 成员
        /// <summary>
        /// 卸载方法
        /// </summary>
        /// <returns></returns>
        public bool Uninstall()
        {
            DeleteFile();
            return true;
        }
        /// <summary>
        /// 回滚方法
        /// </summary>
        /// <returns></returns>
        public bool Rollback()
        {
            DeleteFile();
            return true;
        }
        /// <summary>
        /// 安装方法
        /// </summary>
        /// <returns></returns>
        public bool Install()
        {
            if (this.OnProgressChanged != null)
            {
                this.OnProgressChanged(10);
            }
            this.CopyFile();
            if (this.OnProgressChanged != null)
            {
                this.OnProgressChanged(50);
            }
            this.SetUpMenuFile();
            if (this.OnProgressChanged != null)
            {
                this.OnProgressChanged(99);
            }
            return true;
        }
        /// <summary>
        /// 安装时检查环境
        /// </summary>
        /// <returns></returns>
        public CheckEnviromentStateMessage CheckEnviroment()
        {
            string text = this.CheckMain();
            CheckEnviromentStateMessage result;
            if (string.IsNullOrEmpty(text))
            {
                result = new CheckEnviromentStateMessage(CheckEnviromentState.OK, "");
            }
            else
            {
                result = new CheckEnviromentStateMessage(CheckEnviromentState.Wrong, text);
            }
            return result;
        }
        /// <summary>
        /// 卸载时检查环境
        /// </summary>
        /// <returns></returns>
        public CheckEnviromentStateMessage CheckRemoveEnviroment()
        {
            this.CheckString.Length = 0;
            this.GetSoftOpen(AddCheckProcess(), true);
            var text = this.CheckString.ToString();
            CheckEnviromentStateMessage result;
            if (string.IsNullOrEmpty(text))
            {
                result = new CheckEnviromentStateMessage(CheckEnviromentState.OK, "");
            }
            else
            {
                result = new CheckEnviromentStateMessage(CheckEnviromentState.Wrong, text);
            }
            return result;
        }
        /// <summary>
        /// 显示的描述信息
        /// </summary>
        public string Description
        {
            get { return "\u3000\u3000CR 5000集成插件的安装将使您从CR 5000软件中直接执行金蝶PLM系统操作，例如：检入、检出等。"; }
        }
        /// <summary>
        /// 文件夹名称
        /// </summary>
        public string DirName
        {
            get { return "Zuken"; }
        }
        /// <summary>
        /// 集成哦的标识
        /// </summary>
        public string ID
        {
            get { return "6390DE36-D969-4B78-9E28-E5B14E7B15B8"; }
        }
        /// <summary>
        /// 是否中断
        /// </summary>
        public bool Interrupt
        {
            private get;
            set;
        }
        /// <summary>
        /// 是否必须
        /// </summary>
        public bool IsNecessary
        {
            get { return false; }
        }
        /// <summary>
        /// 语言
        /// </summary>
        public string LanguageMark
        {
            get;
            set;
        }
        /// <summary>
        /// 日志路径
        /// </summary>
        public string LogFilePath
        {
            private get;
            set;
        }
        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName
        {
            get { return "CR 5000集成插件"; }
        }
        /// <summary>
        /// 需要时间长度
        /// </summary>
        public int NeedTime
        {
            get { return 15; }
        }
        /// <summary>
        /// 消息事件
        /// </summary>
        public event MessageChanged OnMessageChanged;
        /// <summary>
        /// 进度事件
        /// </summary>
        public event ProgressChanged OnProgressChanged;
        /// <summary>
        /// 父id
        /// </summary>
        public string ParentId
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// 顺序号
        /// </summary>
        public float SortIndex
        {
            get { return 20F; }
        }
        /// <summary>
        /// 源文件路径
        /// </summary>
        public string SourceDefaultDir
        {
            get;
            set;
        }
        /// <summary>
        /// 是否暂停
        /// </summary>
        public bool Suspend
        {
            get;
            set;
        }
        /// <summary>
        /// 总共大小
        /// </summary>
        public long TotalSize
        {
            get { return 9550000L; }
        }
        /// <summary>
        /// 安装路径
        /// </summary>
        public string UserDefaultDir
        {
            get;
            set;
        }
        /// <summary>
        /// 版本
        /// </summary>
        public float Version
        {
            get { return 1F; }
        }

        #endregion
    }
}
