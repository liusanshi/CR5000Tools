using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ModifyConfig
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Length > 3)
            {
                string cfgpath = args[0];
                string xpath = args[1];
                string replaceif = "";
                var type = ConvertToType(args[2]);
                var key = args[3];
                string val = string.Empty;
                if (type != ModifyType.Delete && args.Length > 4)
                {
                    val = args[4];
                }
                if (type == ModifyType.Replace && args.Length > 5)
                {
                    replaceif = args[5];
                }
                ModifyContext context = new ModifyContext(@cfgpath, xpath, replaceif);
                var mdf = ModifyFactory.GetModify(type);
                mdf.Modify(context, key, val);
            }
        }

        static ModifyType ConvertToType(string type)
        {
            switch (type.ToLower())
            {
                default:
                case "add":
                case "insert":
                    return ModifyType.Add;
                case "modify":
                case "update":
                    return ModifyType.Modify;
                case "delete":
                case "del":
                    return ModifyType.Delete;
                case "replace":
                    return ModifyType.Replace;
                case "append":
                    return ModifyType.Append;
            }
        }
    }
}
