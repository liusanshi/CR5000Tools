using System;
using System.Collections.Generic;
using System.Web.UI;

namespace Zuken.Common.Template
{
    /// <summary>
    /// 模板内容
    /// </summary>
    public class TemplateBody : UserControl
    {
        /// <summary>
        /// 内容容器
        /// </summary>
        private IDictionary<string, object> _context = new Dictionary<string, object>();
        
        public IDictionary<string, object> ViewData
        {
            get { return _context; }
        }
        //VirtualPathProvide
        
    }
}
