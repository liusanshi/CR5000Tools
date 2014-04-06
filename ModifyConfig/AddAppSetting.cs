using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ModifyConfig
{
    public class AddAppSetting : IModify
    {
        #region IModify 成员

        public void Modify(ModifyContext context, string key, string value)
        {
            context.BeginModify();

            context.AddAppSetting(key, value);

            context.EndModify();
        }

        #endregion
    }

    public class ModifyAppSetting : IModify
    {
        #region IModify 成员

        public void Modify(ModifyContext context, string key, string value)
        {
            context.BeginModify();

            context.GetAppSetting(key).Attributes["value"].Value = value;

            context.EndModify();
        }

        #endregion
    }

    public class DeleteAppSetting : IModify
    {
        #region IModify 成员

        public void Modify(ModifyContext context, string key, string value)
        {
            context.BeginModify();

            context.RemoveAppSetting(key);

            context.EndModify();
        }

        #endregion
    }

    public class ReplaceAppSetting : IModify
    {
        #region IModify 成员

        public void Modify(ModifyContext context, string key, string value)
        {
            if (File.Exists(context.CfgPath))
            {
                context.RemoveReadonly(context.CfgPath);
                try
                {
                    var text = File.ReadAllText(context.CfgPath, Encoding.UTF8);
                    if (string.IsNullOrEmpty(context.ReplaceIf))
                    {
                        if (text.IndexOf(value) == -1)
                            File.WriteAllText(context.CfgPath, text.Replace(key, value), Encoding.UTF8);
                    }
                    else if (text.IndexOf(context.ReplaceIf) == -1)
                    {
                        File.WriteAllText(context.CfgPath, text.Replace(key, context.ReplaceIf + value), Encoding.UTF8);
                    }
                }
                catch (Exception ex) 
                {
                    Console.Write(ex.ToString());
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// 追加
    /// </summary>
    public class AppendAppSetting : IModify
    {
        #region IModify 成员

        public void Modify(ModifyContext context, string key, string value)
        {
            if (File.Exists(context.CfgPath))
            {
                context.RemoveReadonly(context.CfgPath);
                try
                {
                    var text = File.ReadAllText(context.CfgPath, Encoding.UTF8);
                    if (string.IsNullOrEmpty(key) || text.IndexOf(key) == -1)
                    {
                        File.AppendAllText(context.CfgPath, value, Encoding.UTF8);
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                }
            }
        }

        #endregion
    }
}
