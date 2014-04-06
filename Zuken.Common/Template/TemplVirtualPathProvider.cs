using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Hosting;

using Zuken.Common.Utility;

namespace Zuken.Common.Template
{
    public class TemplVirtualPathProvider : VirtualPathProvider
    {
        static TemplVirtualPathProvider()
        {
            System.Web.Hosting.HostingEnvironment.RegisterVirtualPathProvider(new Zuken.Common.Template.TemplVirtualPathProvider());
            BasePath = Environment.CurrentDirectory;
        }
        static readonly string BasePath = string.Empty;
        static readonly string[] Extensions = new string[] { ".view", ".templ" };
        /// <summary>
        /// 模板引擎
        /// </summary>
        public TemplVirtualPathProvider() { }

        /// <summary>
        /// 是否虚拟路径
        /// </summary>
        public static bool IsVirtualPath(string virtualPath)
        {
            string ext = Path.GetExtension(virtualPath);
            foreach (var item in Extensions)
            {
                if (string.Compare(ext, item, true) == 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 获取真实的文件路径
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public static String GetRealPath(String virtualPath)
        {
            return Path.Combine(BasePath, virtualPath);
        }

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="virtualPath">相对路径</param>
        /// <returns></returns>
        public override bool FileExists(string virtualPath)
        {
            if (IsVirtualPath(virtualPath))
            {
                string filepath = GetRealPath(virtualPath);
                return File.Exists(filepath);
            }
            else
            {
                return Previous.FileExists(virtualPath);
            }
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            if (IsVirtualPath(virtualPath))
            {
                return new TemplVirtualFile(virtualPath);
            }
            else
            {
                return Previous.GetFile(virtualPath);
            }
        }

        public override string GetFileHash(string virtualPath, System.Collections.IEnumerable virtualPathDependencies)
        {
            if (IsVirtualPath(virtualPath))
            {
                string filepath = GetRealPath(virtualPath);
                return FileHashHelper.ComputeMD5(filepath);
            }
            else
            {
                return Previous.GetFileHash(virtualPath, virtualPathDependencies);
            }
        }

        public override System.Web.Caching.CacheDependency GetCacheDependency(string virtualPath, System.Collections.IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            if (IsVirtualPath(virtualPath))
            {
                string filepath = GetRealPath(virtualPath);
                if (File.Exists(filepath))
                    return new System.Web.Caching.CacheDependency(filepath, utcStart);
                return null;
            }
            else
            {
                return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
            }
        }

        /// <summary>
        /// 模板的虚拟文件
        /// </summary>
        public class TemplVirtualFile : VirtualFile
        {
            private string TempPath;

            public TemplVirtualFile(string virtualPath)
                : base(virtualPath)
            {
                TempPath = virtualPath;
            }

            public override Stream Open()
            {
                string filepath = GetRealPath(TempPath);
                if (File.Exists(filepath))
                {
                    using (StreamReader sr = new StreamReader(filepath, Encoding.UTF8))
                    {
                        return new MemoryStream(Encoding.Default.GetBytes(sr.ReadToEnd()));
                    }
                }
                return new MemoryStream(0);
            }
        }
    }
}
