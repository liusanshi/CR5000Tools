using System;
using System.IO;
using System.Linq;
using System.Text;

using Vici.Core.Parser;
using Vici.Core;

namespace Zuken.Command.Utility
{
    /// <summary>
    /// 模板帮助类
    /// </summary>
    public static class TemplateHelper
    {
        /// <summary>
        /// 语法分析
        /// </summary>
        static readonly TemplateParser velocityParser = new TemplateParser<Vici.Core.Parser.Config.Velocity>();

        /// <summary>
        /// 呈现模板
        /// </summary>
        /// <param name="templ"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string Render(string templ, CSharpContext context)
        {
            context.AddType("String", typeof(String));
            
            return velocityParser.Render(templ, context);
        }

        /// <summary>
        /// 呈现模板文件
        /// </summary>
        /// <param name="templpath"></param>
        /// <param name="coding"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string Render(string templpath, Encoding coding, CSharpContext context)
        {
            if (!File.Exists(templpath)) return string.Empty;
            return Render(File.ReadAllText(templpath, coding), context);
        }
    }
}
