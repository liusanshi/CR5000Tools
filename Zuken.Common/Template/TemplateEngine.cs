using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web.UI;

namespace Zuken.Common.Template
{
    public class TemplateEngine : IDisposable
    {
        #region 变量
        UserControl uc = null;
        TemplateBody _tpl = null;
        #endregion

        #region 构造函数
        /// <summary>
        /// 
        /// </summary>
        public TemplateEngine()
        {
            uc = new UserControl();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="entity"></param>
        public TemplateEngine(string path, IDictionary<string, object> entity)
            : this()
        {
            Load(path);
            if (entity != null)
            {
                foreach (var item in entity)
                {
                    AddContext(item.Key, item.Value);
                }
            }
        }
        #endregion

        #region 公用方法

        /// <summary>   
        /// 加载一个模板   
        /// </summary>   
        /// <param name="path">这个路径为相对路径</param>   
        public void Load(string path)
        {
            _tpl = uc.LoadControl(path) as TemplateBody;
            if (_tpl == null)
            {
                throw (new ArgumentException(path));
            }
        }
        /// <summary>   
        /// 控制展示   
        /// </summary>   
        /// <returns>返回生成的字符串</returns>   
        public string Render()
        {
            TextWriter tw = new StringWriter();
            Render(tw);
            return tw.ToString();
        }
        /// <summary>   
        /// 展示模板   
        /// </summary>   
        /// <param name="writer">TextWriter对象,可以传Response.Output</param>   
        private void Render(TextWriter writer)
        {
            HtmlTextWriter htw = new HtmlTextWriter(writer);
            _tpl.RenderControl(htw);
        }
        /// <summary>   
        /// 增加一个显示数据的上下文   
        /// </summary>   
        /// <param name="key"></param>   
        /// <param name="obj"></param>   
        public void AddContext(string key, object obj)
        {
            _tpl.ViewData.Add(key, obj);
        }

        /// <summary>
        /// 模板数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            get
            {
                object ret;
                _tpl.ViewData.TryGetValue(key, out ret);
                return ret;
            }
            set { AddContext(key, value); }
        }

        #endregion

        #region IDisposable 成员

        private bool disposing = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool dispose)
        {
            if (dispose)
            {//释放托管资源
                if (!disposing)
                {
                    if (uc != null)
                        uc.Dispose();
                }
                disposing = true;
            }
            //释放非托管资源
        }

        ~TemplateEngine()
        {
            Dispose(true);
        }
        #endregion
    }
}
