using Proway.PLM.DAL;
using Proway.PLM.Document;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zuken.Server.DAL
{
    /// <summary>
    /// 新的文档管理类
    /// </summary>
    public class DocversionManager : BaseDataAccess<DocumentVersion>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="Appid"></param>
        /// <returns></returns>
        public DocumentVersion GetDocumentVersionByFileName(string fileName, string Appid)
        {
            string query = string.Format("From DocumentVersion a Where a.FileName= '{0}' and a.AppType='{1}' and a.DeleteFlag=0 and (a.StateId=2 or a.IsEffective=1) ", fileName, Appid);

            return base.Session.CreateQuery(query).SetMaxResults(1).UniqueResult<DocumentVersion>();
        }
    }
}
