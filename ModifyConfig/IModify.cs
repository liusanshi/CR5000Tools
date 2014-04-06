using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace ModifyConfig
{
    /// <summary>
    /// 修改接口
    /// </summary>
    public interface IModify
    {
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Modify(ModifyContext context, string key, string value);
    }

    /// <summary>
    /// 修改上下文
    /// </summary>
    public class ModifyContext
    {
        public ModifyContext(string cfgpath, string xpath, string replaceif)
        {
            CfgPath = cfgpath;
            XPath = xpath;
            _replaceIf = replaceif;
        }
        /// <summary>
        /// 需要修改的xml文档
        /// </summary>
        XmlDocument xmldoc = null;
        public readonly string CfgPath;
        string XPath = "configuration/appSettings";
        string _replaceIf = string.Empty;
        /// <summary>
        /// 需要修改的xml文档
        /// </summary>
        public XmlDocument XmlDoc
        {
            get { return xmldoc; }
        }

        public string ReplaceIf { get { return _replaceIf; } }

        /// <summary>
        /// 开始编辑
        /// </summary>
        public void BeginModify()
        {
            if (File.Exists(CfgPath))
            {
                if (xmldoc == null)
                {
                    xmldoc = new XmlDocument();
                    xmldoc.Load(CfgPath);
                }
            }
            else
            {
                throw new Exception(CfgPath + "配置文件不存在！");
            }
        }
        /// <summary>
        /// 结束编辑
        /// </summary>
        public void EndModify()
        {
            if (xmldoc != null)
            {
                RemoveReadonly(CfgPath);
                xmldoc.Save(CfgPath);
            }
        }
        /// <summary>
        /// 获取 AppSettings
        /// </summary>
        /// <returns></returns>
        public XmlNode GetAppSetting()
        {
            if (xmldoc == null) return null;
            return xmldoc.SelectSingleNode(XPath);
        }
        /// <summary>
        /// 节点下是否存在
        /// </summary>
        /// <param name="node"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool Exists(XmlNode node, string key)
        {
            return node.SelectNodes(string.Format("add[@key='{0}']", key)).Count > 0;
        }

        /// <summary>
        /// 获取 AppSetting
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public XmlNode GetAppSetting(string key)
        {
            if (xmldoc == null) return null;
            string xpath = string.Format("{1}/add[@key='{0}']", key, XPath);
            return xmldoc.SelectSingleNode(xpath);
        }
        /// <summary>
        /// 添加 AppSetting
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddAppSetting(string key, string value)
        {
            if (xmldoc == null) return;
            var appsetnode = GetAppSetting();
            if (appsetnode == null) return;
            if (Exists(appsetnode, key))
            {
                RemoveAppSetting(key);
            }
            var node = xmldoc.CreateNode(XmlNodeType.Element, "add", "");
            var attr = xmldoc.CreateAttribute(null, "key", "");
            attr.Value = key;
            node.Attributes.Append(attr);
            attr = xmldoc.CreateAttribute(null, "value", "");
            attr.Value = value;
            node.Attributes.Append(attr);
            appsetnode.AppendChild(node);
        }
        /// <summary>
        /// 移除 AppSetting
        /// </summary>
        /// <param name="key"></param>
        public void RemoveAppSetting(string key)
        {
            GetAppSetting().RemoveChild(GetAppSetting(key));
        }

        /// <summary>
        /// 去除文件的制度属性
        /// </summary>
        /// <param name="filepath"></param>
        public void RemoveReadonly(string filepath)
        {
            try
            {
                FileInfo fi = new FileInfo(filepath);
                if (fi.Exists)
                {
                    fi.IsReadOnly = false;
                }
            }
            catch
            {

            }
        }
    }

    /// <summary>
    /// 修改类型
    /// </summary>
    public enum ModifyType
    {
        /// <summary>
        /// 添加
        /// </summary>
        Add = 0,
        /// <summary>
        /// 修改
        /// </summary>
        Modify,
        /// <summary>
        /// 删除
        /// </summary>
        Delete,
        /// <summary>
        /// 替换
        /// </summary>
        Replace,
        /// <summary>
        /// 追加
        /// </summary>
        Append
    }

    public static class ModifyFactory
    {
        public static IModify GetModify(ModifyType type)
        {
            switch (type)
            {
                default:
                case ModifyType.Add:
                    return new AddAppSetting();
                case ModifyType.Modify:
                    return new ModifyAppSetting();
                case ModifyType.Delete:
                    return new DeleteAppSetting();
                case ModifyType.Replace:
                    return new ReplaceAppSetting();
                case ModifyType.Append:
                    return new AppendAppSetting();
            }
        }
    }
}
