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
        /// 获取T版或有效版
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="Appid"></param>
        /// <returns></returns>
        public DocumentVersion GetDocumentVersionByFileName(string fileName, string Appid)
        {
            string query = string.Format("From DocumentVersion a Where a.FileName= '{0}' and a.StateId in (1,2,3,4) and a.AppType='{1}' and a.DeleteFlag=0 order by a.CreateDate desc", fileName, Appid);

            var VerList = base.Session.CreateQuery(query).List<DocumentVersion>();
            if (VerList == null || VerList.Count == 0)
            {
                return null;
            }
            DocumentVersion documentVersion = (DocumentVersion)VerList[0];
            if (documentVersion.StateId == 2)
            {
                return documentVersion;
            }
            for (int i = 1; i < VerList.Count; i++)
            {
                DocumentVersion documentVersion2 = (DocumentVersion)VerList[i];
                if (documentVersion2.StateId == 2)
                {
                    return documentVersion2;
                }
                if (documentVersion2.IsShow == 1)
                {
                    documentVersion = documentVersion2;
                }
            }
            return documentVersion;
        }
    }
}
