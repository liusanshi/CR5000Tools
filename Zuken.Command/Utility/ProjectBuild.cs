using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

using Zuken.Common;

namespace Zuken.Command.Utility
{
    /// <summary>
    /// 生成项目文件
    /// 1.  创建几个文件夹
    /// 2.  生成文件
    /// 3.  复制文件
    /// 4.  打开CAD程序
    /// 
    /// j1052为项目名称
    /// j1052.rfb : PCB图路径查找原理图路径(文件夹名称)内容：Relation ../sd/j1052.cir
    /// j1052.rfc : 原理图路径查找PCB图路径(文件夹名称)内容：Relation ../../bd/j1052
    /// .rul 据zuken 专业人士 说这个需要上传服务器
    /// </summary>
    public class ProjectBuild
    {
        #region 构造函数
        public ProjectBuild(string zlroot, string prjfullname, ZukenFileType filetype)
        {
            ZLOCALROOT = zlroot;
            FileType = filetype;
            prjfullname = prjfullname.TrimEnd('\\');
            ProjName = Path.GetFileNameWithoutExtension(prjfullname); //项目名称
            CurDir = Path.GetDirectoryName(prjfullname);
            switch (FileType)
            {
                case ZukenFileType.ZKSCH:
                    SchFullName = prjfullname; //***\**.cir  
                    RFCPath = Path.Combine(SchFullName, string.Format("{0}.rfc", ProjName));
                    PcbFullName = string.Format("{0}.pcb", GetTargetPath(RFCPath));
                    break;
                case ZukenFileType.ZKPCB:
                    PcbFullName = prjfullname; //***\**.pcb
                    RFBPath = Path.Combine(CurDir, string.Format("{0}.rfb", ProjName));
                    SchFullName = GetTargetPath(RFBPath);
                    LayerBomFilePath = Path.Combine(CurDir, string.Format("{0}.bom", ProjName));
                    RULPath = Path.Combine(CurDir, string.Format("{0}.rul", ProjName));
                    break;
                default:
                    break;
            }
            NetListFilePath = Path.Combine(SchFullName, string.Format("ext\\{0}.prt", ProjName));
            CheckData();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void CheckData()
        {
            if (string.IsNullOrEmpty(SchFullName))
            {
                throw new Exception("未找到对应的原理图路径");
            }
            if (!SchFullName.EndsWith(".cir", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("项目名称错误！");
            }
            
        }
        /// <summary>
        /// 获取对应的文件路径
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        string GetTargetPath(string filepath)
        {
            string dir = Path.GetDirectoryName(filepath);
            var path = Tools.GetFileContent(filepath).FirstOrDefault();
            const string prx = "Relation ";
            if (!string.IsNullOrEmpty(path) && path.StartsWith(prx, StringComparison.OrdinalIgnoreCase))
            {
                return Path.GetFullPath(Path.Combine(dir, path.Substring(prx.Length)));
            }
            else if (string.IsNullOrEmpty(path))
            {
                if (string.Compare(Path.GetFileName(dir), "ext", true) == 0)
                {
                    return Path.GetDirectoryName(dir);
                }                
            }
            return string.Empty;
        }

        static ProjectBuild()
        {
            TemplPath = Path.Combine(Tools.ZukenAppDataPath, "proj.tpl");

            string pcbcc = Path.Combine(Tools.ZukenAppDataPath, "PCBColumnConfig.txt");
            var config = Tools.GetFileContent(pcbcc).FirstOrDefault(p => !string.IsNullOrEmpty(p) && !p.StartsWith("#"));
            if (string.IsNullOrEmpty(config))
            {
                config = "reference list,Part Name,Layer";
            }
            PCBColumnConfig = config.Split(',');
        }
        #endregion

        #region 变量
        /// <summary>
        /// 固定的文件夹
        /// </summary>
        private static readonly string[] FixedFolder = { "drc", "ext", "log", "frame" };
        /// <summary>
        /// 固定文件名称
        /// </summary>
        private static readonly string FixedFileName = "rcpath";
        /// <summary>
        /// 项目文件的名称，没有扩展名
        /// </summary>
        private string ProjName = string.Empty;
        /// <summary>
        /// 当前文件类型
        /// </summary>
        private ZukenFileType FileType = ZukenFileType.ZKSCH;
        /// <summary>
        /// 当前工作目录
        /// </summary>
        private string CurDir = string.Empty;
        /// <summary>
        /// PCB图路径查找原理图路径文件
        /// </summary>
        private string RFBPath = string.Empty;
        /// <summary>
        /// 原理图路径查找PCB图路径文件
        /// </summary>
        private string RFCPath = string.Empty;
        /// <summary>
        /// rul 文件路径
        /// </summary>
        private string RULPath = string.Empty;
        /// <summary>
        /// 模板文件的路径
        /// </summary>
        readonly static string TemplPath = string.Empty;
        /// <summary>
        /// pcb 列头的配置文件
        /// </summary>
        public readonly static string[] PCBColumnConfig;

        #endregion

        #region 属性

        /// <summary>
        /// zuken本地根目录
        /// </summary>
        public string ZLOCALROOT { get; private set; }
        /// <summary>
        /// 原理图文件夹的全路径
        /// </summary>
        public string SchFullName { get; set; }
        /// <summary>
        /// Pcb 文件的 路径
        /// </summary>
        public string PcbFullName { get; set; }
        /// <summary>
        /// 固定文件 rcpath 的路径
        /// </summary>
        private string RcpathPath
        {
            get
            {
                return String.Format("{0}\\{1}", SchFullName, FixedFileName);
            }
        }
        /// <summary>
        /// 命令文件路径
        /// </summary>
        private string CmdPath
        {
            get
            {
                return String.Format("{0}\\{1}.cmd", CurDir, ProjName);
            }
        }
        /// <summary>
        /// BOM文件的路径
        /// </summary>
        private string BomPath
        {
            get
            {   //@"E:\项目备份\Zuken项目\sample\ll.cir\ll.csv"
                return string.Format("{0}\\{1}.csv", SchFullName, ProjName);
            }
        }
        /// <summary>
        /// 网表文件的路径
        /// </summary>
        public string NetListFilePath { get; private set; }
        /// <summary>
        /// 含有层信息的bom清单文件
        /// </summary>
        public string LayerBomFilePath { get; private set; }
        /// <summary>
        /// rul文件的标识
        /// </summary>
        public const string RULTARGET = ":@$@:";
        #endregion

        #region 公开方法

        /// <summary>
        /// 生成 SCH 命令
        /// </summary>
        /// <returns></returns>
        public string BuildSchCmd()
        {
            return BuildCmd(new ZukenFileType[] { ZukenFileType.ZKSCH });
        }

        /// <summary>
        /// 生成 Board 命令
        /// </summary>
        /// <returns></returns>
        public string BuildBoardCmd()
        {
            //将 rul 嵌入项目文件
            string ruldata = "";
            if (File.Exists(RULPath))
            {
                ruldata = Convert.ToBase64String(File.ReadAllBytes(RULPath));
                if (!string.IsNullOrEmpty(ruldata))
                {
                    ruldata = RULTARGET + ruldata + "\r\n";
                }
            }
            else
            {
                throw new Exception("缺少rul文件！");
            }
            return ruldata + BuildCmd(new ZukenFileType[] { ZukenFileType.ZKPCB, ZukenFileType.ZKSCH });
        }
        /// <summary>
        /// 保存项目文件
        /// 生成命令文件
        /// 可能会发生异常
        /// </summary>
        /// <param name="filetype"></param>
        public void SaveProject()
        {
            switch (FileType)
            {
                default:
                case ZukenFileType.ZKSCH:
                    FileHelper.SaveFile(CmdPath, BuildSchCmd());
                    Tools.SaveProjectPath(SchFullName);
                    break;
                case ZukenFileType.ZKPCB:
                    FileHelper.SaveFile(CmdPath, BuildBoardCmd());
                    Tools.SaveProjectPath(PcbFullName);
                    break;
            }
        }
        /// <summary>
        /// 保存BOM信息
        /// </summary>
        /// <returns></returns>
        public void SaveBOM()
        {
            switch (FileType)
            {
                default:
                case ZukenFileType.ZKSCH:
                    GetBOMData(p => p.Trim('"')).SaveXml(Tools.BOMFilePath);
                    break;
                case ZukenFileType.ZKPCB:
                    GetBOMDataWithLayer(p =>
                    {
                        if (p >= 0 && p < PCBColumnConfig.Length) {
                            return PCBColumnConfig[p];
                        }
                        return string.Empty;
                    })
                        .SaveXml(Tools.BOMFilePath);
                    break;
            }
        }

        /// <summary>
        /// 获取BOM数据
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public DocumentProperty GetBOMData(Func<string, string> func)
        {
            if (!File.Exists(NetListFilePath))
            {
                throw new Exception("请生成原理图BOM文件之后，再操作。\r\n文件路径为：" + NetListFilePath);
            }

            var docprop = FileHelper.GetDocData(NetListFilePath, func);
            int index = 0;
            var proj = FileHelper.GetPartProp(new FileInfo(CmdPath));
            foreach (var item in FileHelper.GetPartProps(SchFullName, (new ZukenFileType[] { ZukenFileType.ZKSCH }).Select(p => ConvertToExt(p))))
            {
                ModifyFileName(ProjName, item);
                docprop.Insert(index++, item);
                proj.ChildRelation.Add(new Dictionary<string, string>() { { _.VerId, item[_.VerId] } });
            }
            docprop.Insert(0, proj);

            if (proj.ChildRelation.Count > 0)
            {
                foreach (var item in docprop.Where(p => p[_.FileType] == _.ZKMAT))
                {
                    proj.ChildRelation.Add(new Dictionary<string, string>() { { _.VerId, item[_.VerId] } });
                }
            }
            
            return docprop;
        }

        public DocumentProperty GetBOMDataWithLayer(Func<int, string> func)
        {
            if (!File.Exists(LayerBomFilePath))
            {
                throw new Exception("请生成印制板BOM文件之后，再操作。\r\n文件路径为：" + LayerBomFilePath);
            }

            var docprop = FileHelper.GetDocDataWithLayer(LayerBomFilePath, func);
            int index = 0;
            var proj = FileHelper.GetPartProp(new FileInfo(CmdPath));
            foreach (var item in FileHelper.GetPartProps(SchFullName, (new ZukenFileType[] { ZukenFileType.ZKSCH }).Select(p => ConvertToExt(p))))
            {
                ModifyFileName(ProjName, item);
                docprop.Insert(index++, item);
                proj.ChildRelation.Add(new Dictionary<string, string>() { { _.VerId, item[_.VerId] } });
            }
            var pcb = FileHelper.GetPartProp(new FileInfo(PcbFullName));//pcb
            docprop.Insert(0, pcb);
            proj.ChildRelation.Add(new Dictionary<string, string>() { { _.VerId, pcb[_.VerId] } });

            docprop.Insert(0, proj);

            if (proj.ChildRelation.Count > 0)
            {
                foreach (var item in docprop.Where(p => p[_.FileType] == _.ZKMAT))
                {
                    pcb.ChildRelation.Add(new Dictionary<string, string>() { { _.VerId, item[_.VerId] } });
                }
            }
            return docprop;
        }

        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="projname"></param>
        /// <param name="partprop"></param>
        void ModifyFileName(string projname, PartProperty partprop)
        {
            string curdir = partprop["filepath"];
            string fname = partprop["filename"];
            partprop["filename"] = string.Format("{0}_{1}", projname, fname);
            try
            {
                string sourcepath = Path.Combine(curdir, fname);
                string targetpath = Path.Combine(curdir, partprop["filename"]);
                if (File.Exists(targetpath))
                {
                    FileInfo fi = new FileInfo(targetpath);
                    fi.Attributes = FileAttributes.Normal;
                }
                File.Copy(sourcepath, targetpath, true);
            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region 静态函数
        /// <summary>
        /// 生成rul文件
        /// </summary>
        /// <param name="rulfilepath"></param>
        /// <param name="datapath"></param>
        public static void BuildRulFile(string rulfilepath, string datapath)
        {
            if (File.Exists(datapath))
            {
                var data = Tools.GetFileContent(datapath).FirstOrDefault(p => p.StartsWith(RULTARGET));
                if (!string.IsNullOrEmpty(data))
                {
                    WriteRulFile(rulfilepath, data);
                }
            }
        }

        /// <summary>
        /// 生成rul文件
        /// </summary>
        /// <param name="filepath">rul文件路径</param>
        /// <param name="data">数据</param>
        private static void WriteRulFile(string filepath, string data)
        {
            if (data.StartsWith(RULTARGET))
            {
                data = data.Substring(RULTARGET.Length);
            }
            FileInfo fi = new FileInfo(filepath);
            if (fi.Exists)
            {
                fi.Attributes = FileAttributes.Normal;
            }
            var b = Convert.FromBase64String(data);
            using (var sw = fi.OpenWrite())
            {
                sw.Write(b, 0, b.Length);
                sw.Close();
            }
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 生成 项目文件
        /// </summary>
        /// <param name="rcpathcontent"></param>
        /// <param name="childfilepath"></param>
        /// <param name="filetype"></param>
        /// <returns></returns>
        private string BuildCmd(IEnumerable<ZukenFileType> filetypes)
        {
            Vici.Core.Parser.CSharpContext context = new Vici.Core.Parser.CSharpContext();
            context.Set<string>("PrjName", ProjName);
            context.Set<string>("ZukenLocal", Tools.CR5000ZLOCALROOT);
            if (filetypes.Any())
            {
                context.Set<bool>("IsSch", filetypes.First() == ZukenFileType.ZKSCH);
            }
            var rcpathcontent = Tools.GetFileContent(RcpathPath);
            context.Set<string[]>("RcpathData", Tools.GetFileContent(RcpathPath).ToArray());
            context.Set<string[]>("SchList", FileHelper.SreachFile(SchFullName, filetypes.Select(p => ConvertToExt(p)))
                .Select(p => p.Name).ToArray());

            return TemplateHelper.Render(@TemplPath, Encoding.Default, context);

#if 以前的方式
            StringBuilder cmd = new StringBuilder(500);
            cmd.AppendLine("@echo off");
            cmd.AppendLine(MkDir(SDName));
            foreach (var item in FixedFolder)
            {
                cmd.AppendLine(MkDir(SDName, item));
            }

            var rcpathcontent = Tools.GetFileContent(RcpathPath);
            cmd.AppendLine(WirteFile(SDName + "\\" + FixedFileName, rcpathcontent));

            foreach (var item in FileHelper.SreachFile(SchFullName, filetypes.Select(p => ConvertToExt(p))))
            {
                cmd.AppendLine(MoveFile(item.Name, SDName));
            }
            if (filetypes.Any())
            {
                if (filetypes.First() == ZukenFileType.ZKSch)
                {
                    cmd.AppendLine(OpenSch(ZLOCALROOT, SDName));
                }
                else
                {
                    cmd.AppendLine(OpenBoard(ZLOCALROOT, SDName));
                }
            }

            cmd.AppendLine(Exit());

            return cmd.ToString();
#endif
        }

        private string ConvertToExt(ZukenFileType filetype)
        {
            switch (filetype)
            {
                default:
                case ZukenFileType.ZKSCH:
                    return "*.sht";
                case ZukenFileType.ZKPCB:
                    return "*.pcb";
            }
        }

        #region 生成创建命令
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="dirname"></param>
        /// <returns></returns>
        private string MkDir(string dirname)
        {
            return string.Format("if not exist \"{0}\" ( mkdir \"{0}\" )", dirname);
        }
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="pdirname"></param>
        /// <param name="cdirname"></param>
        /// <returns></returns>
        private string MkDir(string pdirname, string cdirname)
        {
            return MkDir(string.Concat(pdirname, "\\", cdirname));
        }
        /// <summary>
        /// 向文件中写文本
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private string WirteFile(string filepath, string content)
        {
            return string.Format("echo {1}>> \"{0}\"", filepath, content);
        }
        /// <summary>
        /// 向文本中输入多行记录
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private string WirteFile(string filepath, IEnumerable<string> content)
        {
            StringBuilder cmd = new StringBuilder(100);
            if (content != null && content.Any())
            {
                cmd.AppendFormat("if not exist \"{0}\" (\r\n", filepath);
                foreach (var item in content)
                {
                    cmd.AppendLine(WirteFile(filepath, item));
                }
                cmd.Append(")");
            }
            return cmd.ToString();
        }
        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private string MoveFile(string source, string target)
        {
            return string.Format("if exist \"{0}\" (move /y \"{0}\" \"{1}\\{0}\")", source, target);
        }

        /// <summary>
        /// 打开sch的命令
        /// </summary>
        /// <param name="root">HKEY_LOCAL_MACHINE\SOFTWARE\ZUKEN\CR-5000 Runtime\11.0\ZLOCALROOT</param>
        /// <param name="filename">文件夹的名称</param>
        /// <returns></returns>
        private string OpenSch(string root, string filename)
        {
            return string.Format("wscript //e:javascript //nologo \"%ZUKEN_PLM%\\openCADFileManage.js\" \"sch\" \"{0}\" \"%~dp0\\{1}\"", root, filename);
        }
        /// <summary>
        /// 打开 board 的命令
        /// </summary>
        /// <param name="root">HKEY_LOCAL_MACHINE\SOFTWARE\ZUKEN\CR-5000 Runtime\11.0\ZLOCALROOT</param>
        /// <param name="filename">文件夹的名称</param>
        /// <returns></returns>
        private string OpenBoard(string root, string filename)
        {
            return string.Format("wscript //e:javascript //nologo \"%ZUKEN_PLM%\\openCADFileManage.js\" \"board\" \"{0}\" \"%~dp0\\{1}\"", root, filename);
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        private string Exit()
        {
            return "exit";
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// Zuken文件类型
    /// </summary>
    public enum ZukenFileType
    {
        /// <summary>
        /// 原理图文件
        /// </summary>
        ZKSCH = 0,
        /// <summary>
        /// 印制板文件
        /// </summary>
        ZKPCB = 1
    }
}
