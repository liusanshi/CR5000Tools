using System;
using System.Collections.Generic;
using System.Web.UI;

namespace Zuken.Common.Template
{
    public class TemplateBody : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        private IDictionary<string, object> _context = new Dictionary<string, object>();
        
        public IDictionary<string, object> ViewData
        {
            get { return _context; }
        }
        //VirtualPathProvide
        
    }
}
