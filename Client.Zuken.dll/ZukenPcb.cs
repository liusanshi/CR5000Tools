using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Kingdee.PLM.Integration.Client.Common.Abstract;
using Zuken.Common;

namespace Zuken.Client
{
    /// <summary>
    /// pcb客户端集成核心类
    /// 继承至plm系统提供的接口
    /// </summary>
    public class ZukenPcb : IIntegration
    {
        #region IIntegration 成员
        /// <summary>
        /// 图纸类型
        /// </summary>
        public string CadType
        {
            get
            {
#if test
                //return "ADPCB";
                return "ALLEGROPCB";
#else
                return "ZKPCB";
#endif

            }
        }
        /// <summary>
        /// 检查应用程序是否运行
        /// </summary>
        /// <returns></returns>
        public bool CheckApplicationIsRun()
        {
            return Tools.ApplicationIsRun("zfmscheme.exe");
        }
        /// <summary>
        /// 读取当前图纸的bom数据
        /// </summary>
        /// <returns></returns>
        public System.Collections.ArrayList GetBom()
        {
            return DocumentProperty.LoadXml(Tools.BOMFilePath).ConvertToBOM();
        }
        /// <summary>
        /// 根据文件名称读取bom数据
        /// </summary>
        /// <param name="Filename"></param>
        /// <returns></returns>
        public System.Collections.ArrayList GetBomByFileName(string Filename)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 返回当前图纸的文件名称
        /// </summary>
        /// <returns></returns>
        public string GetCurrentFileName()
        {
            return Path.GetFileName(GetCurrentFullPath());
        }
        /// <summary>
        /// 返回当前图纸的路径
        /// </summary>
        /// <returns></returns>
        public string GetCurrentFilePath()
        {
            return Path.GetDirectoryName(GetCurrentFullPath());
        }
        /// <summary>
        /// 获取当前图纸的属性
        /// 机械类cad使用
        /// </summary>
        /// <returns></returns>
        public string GetCurrentFileProperty()
        {
            return "";
        }
        /// <summary>
        /// 获取当前图纸的全路径
        /// </summary>
        /// <returns></returns>
        public string GetCurrentFullPath()
        {
            return Tools.GetProjectPath();
        }
        /// <summary>
        /// 根据文件名称获取文件属性
        /// 机械类cad使用
        /// </summary>
        /// <param name="FullPath"></param>
        /// <returns></returns>
        public string GetFileDesCription(string FullPath)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 打开指定的文件
        /// </summary>
        /// <param name="FullPath"></param>
        /// <returns></returns>
        public bool OpenFile(string FullPath)
        {
            Tools.ShellExecute(FullPath, "");
            return true;
        }
        /// <summary>
        /// 设置文件的属性
        /// 机械类cad使用
        /// </summary>
        /// <param name="FullPath"></param>
        /// <param name="PropString"></param>
        /// <returns></returns>
        public bool SetPropertyInfo(string FullPath, string PropString)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 写入文件描述信息
        /// 机械cad使用
        /// </summary>
        /// <param name="FullPath"></param>
        /// <param name="PropString"></param>
        /// <returns></returns>
        public bool WriteFileDesCription(string FullPath, string PropString)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
