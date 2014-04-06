using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

using Zuken.Common;

namespace Zuken.Command.Utility
{
    /// <summary>
    /// 文件读取工具类
    /// </summary>
    public static class FileHelper
    {
#if false
        private static IEnumerable<T> GetData<T>(string filepath, Func<string, string> func, Func<string[], string[], int, T> convertTo)
        {
            var data = File.ReadAllLines(filepath, Encoding.Default);
            int count = data.Length, len = count - 2;
            len = len >= 0 ? len : 0;
            if (len > 0)
            {
                var head = data[1].Split(',');
                int capacity = 0;
                for (int i = 0; i < head.Length; i++)
                {
                    head[i] = func(head[i].Trim());
                    if (!string.IsNullOrEmpty(head[i]))
                    {
                        capacity++;
                    }
                }
                for (int i = 2; i < count; i++)
                {
                    yield return convertTo(head, data[i].Split(','), capacity);
                }
            }
        }
#endif

        /// <summary>
        /// 从文件中读取文档属性 sch
        /// </summary>
        /// <param name="fpath"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static DocumentProperty GetDocData(string fpath, Func<string, string> func)
        {
            var docprop = new DocumentProperty();
            AnalyzeStrnig anastring = null;
            PartProperty prop = null;
            foreach (var item in GetSourceData(fpath, p => anastring = new AnalyzeStrnig(p, func)))
            {
                if (prop == null)
                    prop = anastring.ConvertToProp(item);
                else
                {
                    if (item.StartsWith(">"))
                    {
                        anastring.AppendData(prop, item);
                    }
                    else
                    {
                        docprop.Add(prop);
                        prop = anastring.ConvertToProp(item);
                    }
                }
            }
            docprop.Add(prop); //最后一条数据'
            return docprop;
        }

        /// <summary>
        /// 获取原始数据
        /// </summary>
        /// <param name="fpath"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        static IEnumerable<string> GetSourceData(string fpath, Action<string> func)
        {
            bool canread = false, isfind = false;
            foreach (var item in Tools.GetFileContent(fpath).Where(p => p.Trim().Length > 0))
            {
                if (!isfind && item.StartsWith("********"))
                {
                    isfind = true;
                    continue;
                }
                if (!canread && isfind)
                {
                    func(item);
                    canread = true;
                    continue;
                }
                if (canread && item.StartsWith("--------"))
                {
                    canread = false;
                    yield break;
                }
                if (canread)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// 获取BOM数据，含有层信息
        /// </summary>
        /// <param name="fpath"></param>
        /// <param name="func">获取表头</param>
        /// <returns></returns>
        public static DocumentProperty GetDocDataWithLayer(string fpath, Func<int, string> func)
        {
            var docprop = new DocumentProperty();
            PartProperty prop = null;
            foreach (string item in Tools.GetFileContent(fpath).Where(p => p.Trim().Length > 0))
            {
                string pval = item;
                prop = new PartProperty(5);
                int index = 0;
                prop[_.VerId] = Guid.NewGuid().ToString();
                prop[_.FileType] = _.ZKMAT;
                foreach (var i in pval.Split(new char[] { ' ' }, StringSplitOptions.None))
                {
                    string key = func(index++);
                    if (!string.IsNullOrEmpty(key))
                        prop[key] = i == "-" ? string.Empty : i;
                }
                docprop.Add(prop);
            }
            return docprop;
        }

        /// <summary>
        /// 获取文件的 属性
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static PartProperty GetPartProp(FileInfo fi)
        {
            if (fi == null || !fi.Exists)
                throw new Exception("参数：fi，错误！");

            var prop = new PartProperty(7);
            prop["filename"] = fi.Name;
            prop["filepath"] = fi.DirectoryName;
            prop["filesize"] = fi.Length.ToString();
            prop[_.VerId] = Guid.NewGuid().ToString();
            prop[_.FileType] = _.ZKSCH;
            if (string.Compare(fi.Extension, ".sht", true) == 0)
            {
                prop[_.FileType] = _.ZKSCH;
            }
            else if (string.Compare(fi.Extension, ".cmd", true) == 0)
            {
                prop[_.FileType] = _.ZKPRJ;
            }
            else if (string.Compare(fi.Extension, ".pcb", true) == 0)
            {
                prop[_.FileType] = _.ZKPCB;
            }
            return prop;
        }
        /// <summary>
        /// 查找所有指定后缀名的文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> SreachFile(string path, string searchPattern)
        {
            return SreachFile(path, new string[] { searchPattern });
        }
        /// <summary>
        /// 查找所有指定后缀名的文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> SreachFile(string path, IEnumerable<string> searchPattern)
        {
            if (searchPattern.Any())
            {
                DirectoryInfo dirinfo = new DirectoryInfo(path);
                if (dirinfo.Exists && (dirinfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    var sfs = searchPattern.SelectMany(p => dirinfo.GetFiles(p));
                    //if (sfs.Any(p => p.Name.StartsWith("#") || p.Name.StartsWith("_")))
                    //{
                    //    throw new Exception("请先关闭原理图文件之后，再操作！");
                    //}
                    //p.Name.StartsWith("_") || 
                    return sfs.Where(p => !(p.Name.StartsWith("#") || p.Name.IndexOf("_") >= 0 ));
                    //return searchPattern
                    //    .SelectMany(p => dirinfo.GetFiles(p))
                    //    .Where(p => !(p.Name.StartsWith("#") || p.Name.StartsWith("_")));
                }
            }
            return Enumerable.Empty<FileInfo>();
        }
        /// <summary>
        /// 获取所有的文件属性
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public static IEnumerable<PartProperty> GetPartProps(string path, IEnumerable<string> searchPattern)
        {
            return SreachFile(path, searchPattern).Select(p => GetPartProp(p));
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        public static void SaveFile(string path, string content)
        {
            File.WriteAllText(path, content, Encoding.Default);
        }

        /// <summary>
        /// 查找文件夹 递归查找
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public static IEnumerable<DirectoryInfo> SreachDir(string path, string searchPattern)
        {
            DirectoryInfo dirinfo = new DirectoryInfo(path);
            if (dirinfo.Exists && (dirinfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                return dirinfo.GetDirectories(searchPattern, SearchOption.AllDirectories);
            }
            return Enumerable.Empty<DirectoryInfo>();
        }
        /// <summary>
        /// 递归查找 所有子文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> SreachFiles(string path, string searchPattern)
        {
            DirectoryInfo dirinfo = new DirectoryInfo(path);
            if (dirinfo.Exists && (dirinfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                return dirinfo.GetFiles(searchPattern, SearchOption.AllDirectories);
            }
            return Enumerable.Empty<FileInfo>();
        }

        /// <summary>
        /// 是否文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsDirectory(string path)
        {
            DirectoryInfo dirinfo = new DirectoryInfo(path);
            return (dirinfo.Exists && (dirinfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory);
        }
        /// <summary>
        /// 是否文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsFile(string path)
        {
            FileInfo fi = new FileInfo(path);
            return (fi.Exists && (fi.Attributes & FileAttributes.Directory) != FileAttributes.Directory);
        }

    }

    class AnalyzeStrnig
    {
        List<ColumnInfo> ColumnInfos = null;
        int Capacity = 0;
        static readonly string split = "  ";
        int colindex = -1;

        public AnalyzeStrnig(string head, Func<string, string> func)
        {
            head = head.Trim() + split;
            int index = -1, len = 0;
            var names = head.Split(new string[] { split }, StringSplitOptions.RemoveEmptyEntries);
            ColumnInfos = new List<ColumnInfo>(names.Length);
            foreach (var item in names)
            {
                var h = item.Trim();
                index = head.IndexOf(h + split, index + 1);
                len = FindNextColumn(head, index, split) - index;
                h = func(h);
                if (!string.IsNullOrEmpty(h))
                {
                    //ColumnInfos.Add(new KeyValuePair<string, int>(h, index));
                    ColumnInfos.Add(new ColumnInfo(h, index, len));
                    Capacity++;
                }
            }
        }

        /// <summary>
        /// 查找下一列
        /// </summary>
        /// <param name="head"></param>
        /// <param name="bindex"></param>
        /// <returns></returns>
        static int FindNextColumn(string head, int bindex, string split)
        {
            if (bindex > head.Length) return head.Length;
            var index = head.IndexOf(split, bindex);
            if (index == -1) index = head.Length;
            for (int i = index; i < head.Length; i++)
            {
                if (!char.IsWhiteSpace(head, i))
                    return i;
            }
            return head.Length;
        }

        /// <summary>
        /// 将数据转换为 属性
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public PartProperty ConvertToProp(string data)
        {
            colindex = -1;
            ClearMark();
            var dic = new PartProperty(Capacity + 2);
            dic[_.FileType] = _.ZKMAT;
            dic[_.VerId] = Guid.NewGuid().ToString();
            int len = 0;
            foreach (var item in ColumnInfos)
            {
                if (data.Length < item.StartIndex) break;
                colindex++;
                len = FindNextColumn(data, item.StartIndex, " ") - item.StartIndex;
                dic[item.ColumnName] = data.Substring(item.StartIndex, len).TrimEnd();
                if (colindex == ColumnInfos.Count - 1 && dic[item.ColumnName].Length > item.Length)
                {
                    item.DealState = DealState.Dealing;
                    break;
                }
                else
                {
                    item.DealState = DealState.Dealed;
                }
                if (dic[item.ColumnName].Length > item.Length)//表示要结束
                {
                    break;
                }
            }
            return dic;
        }
        /// <summary>
        /// 追加数据
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="data"></param>
        public void AppendData(PartProperty prop, string data)
        {
            if(data.Length <= 1) return;
            int len = 0;
            if (colindex >= 0 && colindex < ColumnInfos.Count)
            {
                var curcol = ColumnInfos[colindex];
                if (curcol.DealState == DealState.Dealing)
                {
                    len = FindNextColumn(data, curcol.StartIndex, " ") - curcol.StartIndex;
                    prop[curcol.ColumnName] += data.Substring(curcol.StartIndex, len).TrimEnd();
                }
                else if (curcol.DealState == DealState.Dealed)
                {
                    var item = ColumnInfos[++colindex];
                    if (!char.IsWhiteSpace(data[1]))
                    {
                        len = FindNextColumn(data, 1, " ") - 1;
                        prop[item.ColumnName] = data.Substring(1, len).TrimEnd();
                        ++colindex;
                    }
                    for (int i = colindex; i < ColumnInfos.Count; i++)
                    {
                        item = ColumnInfos[i];
                        colindex++;
                        len = FindNextColumn(data, item.StartIndex, " ") - item.StartIndex;
                        prop[item.ColumnName] = data.Substring(item.StartIndex, len).TrimEnd();
                        if (colindex == ColumnInfos.Count - 1 && prop[item.ColumnName].Length > item.Length)
                        {
                            item.DealState = DealState.Dealing;
                            break;
                        }
                        else
                        {
                            item.DealState = DealState.Dealed;
                        }
                        if (prop[item.ColumnName].Length > item.Length)//表示要结束
                        {
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 清楚标记
        /// </summary>
        void ClearMark()
        {
            foreach (var item in ColumnInfos)
            {
                item.DealState = DealState.NonDeal;
            }
        }

        class ColumnInfo
        {
            public ColumnInfo(string name, int bindex, int len)
            {
                ColumnName = name;
                StartIndex = bindex;
                Length = len;
            }
            /// <summary>
            /// 列名称
            /// </summary>
            public string ColumnName { get; set; }
            /// <summary>
            /// 起始序号
            /// </summary>
            public int StartIndex { get; set; }
            /// <summary>
            /// 长度
            /// </summary>
            public int Length { get; set; }
            /// <summary>
            /// 处理状态
            /// </summary>
            public DealState DealState { get; set; }

            /// <summary>
            /// Equals
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                var temp = obj as ColumnInfo;
                if (temp == null) return false;
                return string.Compare(ColumnName, temp.ColumnName) == 0 && StartIndex == temp.StartIndex;
            }
            /// <summary>
            /// GetHashCode
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return ColumnName.GetHashCode() * StartIndex;
            }
            /// <summary>
            /// ToString
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return string.Format("colName:{0};start:{1};len:{2};", ColumnName, StartIndex.ToString(), Length.ToString());
            }
        }
        /// <summary>
        /// 处理状态
        /// </summary>
        enum DealState
        {
            /// <summary>
            /// 未处理
            /// </summary>
            NonDeal = 0,
            /// <summary>
            /// 处理中
            /// </summary>
            Dealing,
            /// <summary>
            /// 处理结束
            /// </summary>
            Dealed,
        }
    }
}
