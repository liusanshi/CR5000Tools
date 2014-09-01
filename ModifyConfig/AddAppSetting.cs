using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ModifyConfig
{
    /// <summary>
    /// 添加配置类
    /// </summary>
    public class AddAppSetting : IModify
    {
        #region IModify 成员
        /// <summary>
        /// 核心方法
        /// </summary>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Modify(ModifyContext context, string key, string value)
        {
            context.BeginModify();

            context.AddAppSetting(key, value);

            context.EndModify();
        }

        #endregion
    }
    /// <summary>
    /// 修改配置类
    /// </summary>
    public class ModifyAppSetting : IModify
    {
        #region IModify 成员
        /// <summary>
        /// 核心方法
        /// </summary>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Modify(ModifyContext context, string key, string value)
        {
            context.BeginModify();

            context.GetAppSetting(key).Attributes["value"].Value = value;

            context.EndModify();
        }

        #endregion
    }

    /// <summary>
    /// 删除配置项类
    /// </summary>
    public class DeleteAppSetting : IModify
    {
        #region IModify 成员
        /// <summary>
        /// 核心方法
        /// </summary>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Modify(ModifyContext context, string key, string value)
        {
            context.BeginModify();

            context.RemoveAppSetting(key);

            context.EndModify();
        }

        #endregion
    }
    /// <summary>
    /// 替换配置项类
    /// </summary>
    public class ReplaceAppSetting : IModify
    {
        #region IModify 成员
        /// <summary>
        /// 核心方法
        /// </summary>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
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
    /// 追加配置项类
    /// </summary>
    public class AppendAppSetting : IModify
    {
        #region IModify 成员
        /// <summary>
        /// 核心方法
        /// </summary>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
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
