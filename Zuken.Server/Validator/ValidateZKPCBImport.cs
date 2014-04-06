using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Proway.Framework;
using Proway.PLM.Document;
using Proway.Framework.LanguagePack;


namespace Zuken.Server.Validator
{
    class ValidateZKPCBImport : DefaultValidator
    {
        private FolderManager folderManager = new FolderManager();
        private FolderRelationManager folderRelationManager = new FolderRelationManager();
        private DocumentVersionManager manager = new DocumentVersionManager();

        /// <summary>
        /// 验证主体
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool Validate(ValidateContext context)
        {
            DocStruct doc = context.ValidateObject as DocStruct;
            if ((doc == null) || !BOMHelp.IsNullOrEmpty(doc.ParentRelation))
            {
                return true;
            }
            string fileName = doc.FileName;
            DocumentVersion docver = this.manager.GetDocumentVersionByFileName(fileName, doc.GetString(Key.FileType));
            if (docver != null)
            {
                string FolderId = BOMHelp.GetValueFromDictionary(context.ExtendProperty, _.FOLDERID);
                if (string.IsNullOrEmpty(FolderId))
                {
                    FolderRelation documentFolderRelation = this.folderRelationManager.GetDocumentFolderRelation(docver.DocId);
                    if (documentFolderRelation != null)
                    {
                        FolderId = documentFolderRelation.ParentObject;
                    }
                    BOMHelp.Write(context.ExtendProperty, _.FOLDERID, FolderId);
                }
                doc.WriteValue(_.FOLDERID, FolderId);
                doc.WriteValue("__FolderName", this.folderManager.GetFolderName(FolderId));
                BOMHelp.Add(context.CurScopeProperty, _.CURVALIDATEDOCUMENT, docver);
                doc.AddValue("__CISVerId", docver.VerId);
                doc.AddValue("__CISDocId", docver.DocId);
                BOMHelp.Add(context.ExtendProperty, "__RO_CIS_Doc", docver);
                doc.SetUploadState(true);
                return true;
            }
            doc.SetDocState(false, "red", string.Format(MutiLanguageManager.L("没有对应的[{0}]文件！"), doc.GetString(Key.FileType)));
            return false;
        }
    }
}
