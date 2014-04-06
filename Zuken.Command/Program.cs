#define Test //正式的时候删除
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Windows.Forms;
using System.Linq;

using Zuken.Common;
using Zuken.Command.Utility;
using System.Collections;
using Zuken.Command.Entity;

namespace Zuken.Command
{
    class Program
    {
        /// <summary>
        /// 1.将目录添加至path环境变量
        /// 2.添加环境变量
        /// </summary>
        /// <param name="args"></param>
        //[STAThread]
        static void Main(string[] args)
        {
            if (args == null || args.Length < 3)
            {
                if (args != null || args.Length == 2)
                {
                    //解析cmd文件生成rul文件
                    ProjectBuild.BuildRulFile(args[0], args[1]);
                    return;
                }
                MessageBox.Show("参数错误！", _.Title);
                return;
            }
            else
            {
                try
                {
                    string ProjectPath = args[0];
                    OperateType ot = Tools.ConvertToEnum<OperateType>(args[1], OperateType.LOGIN);
                    //ZukenFileType zft = Tools.ConvertToEnum<ZukenFileType>(args[2], ZukenFileType.ZKSch);
                    ZukenFileType zft = ConvertToType(ProjectPath);
                    string url = string.Empty;
                    if (args.Length > 3)
                        url = args[3];

                    ProjectBuild build = null;
                    switch (ot)
                    {
                        default:
                        case OperateType.LOGIN:
                        case OperateType.LOGOUT:
                        case OperateType.OPENPLM:
                        case OperateType.CONFIG:
                            break;
                        case OperateType.CHECKIN:
                            //检入之前的操作

                            goto create;
                        case OperateType.WORKSPACE:
                        case OperateType.TASK:
                        case OperateType.PROJECT:
                        //导入之前的操作
                        create:
                            //创建项目文件
                            build = new ProjectBuild(Tools.CR5000ZLOCALROOT, ProjectPath, zft);
                        break;
                    }
                    if (build != null)
                    {
                        build.SaveProject();
                        build.SaveBOM();
                    }
                    //Form frm;
                    //if (ot == OperateType.CONFIG)
                    //{
                    //    frm = new FrmConfig();
                    //}
                    //else
                    //{
                    //    frm = new FrmMain(ot.ToString(), zft.ToString(), url);
                    //}
                    //if (frm != null)
                    //{
                    //    Application.Run(frm);
                    //}
                    string Arguments = ConvertArgs(ot, zft, url);
                    Tools.ShellExecute(_.IntegrationLoginExe, Arguments);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, _.Title);
                    Tools.Log(ex);
                }
            }
        }
        /// <summary>
        /// 根据路径获取集成类型
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static ZukenFileType ConvertToType(string path)
        {
            if (path.EndsWith(".cir", StringComparison.OrdinalIgnoreCase) && FileHelper.IsDirectory(path))
            {
                return ZukenFileType.ZKSCH;
            }
            else if (path.EndsWith(".pcb", StringComparison.OrdinalIgnoreCase) && FileHelper.IsFile(path))
            {
                return ZukenFileType.ZKPCB;
            }
            throw new Exception(@"未找到操作对象！
    对原理图操作请选择原理图所在的文件夹；
    对印制板操作请选择印制板文件");
        }


        /// <summary>
        /// 转换为参数字符串
        /// </summary>
        /// <param name="ot"></param>
        /// <param name="zft"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        static string ConvertArgs(OperateType ot, ZukenFileType zft, string url)
        {
            string apptype = zft.ToString();
#if Test
            switch (zft)
            {
                default:
                case ZukenFileType.ZKSCH:
                    //apptype = "ADSCH";
                    apptype = "ALLEGROCIS";
                    break;
                case ZukenFileType.ZKPCB:
                    //apptype = "ADPCB";
                    apptype = "ALLEGROPCB";
                    break;
            }
#endif

            if (string.IsNullOrEmpty(url))
            {
                return string.Join(",", new string[] { ot.ToString(), apptype });
            }
            else
            {
                return string.Join(",", new string[] { ot.ToString(), apptype, url });
            }
        }
    }

    /// <summary>
    /// 操作类型
    /// </summary>
    enum OperateType
    {
        LOGIN = 0,
        LOGOUT,
        OPENPLM,
        CHECKIN,
        WORKSPACE,
        PROJECT,
        TASK,
        CONFIG
    }
}
