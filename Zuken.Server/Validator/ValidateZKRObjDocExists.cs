using Proway.Framework;
using Proway.Framework.LanguagePack;
using Proway.PLM.Document;
using System;
using Zuken.Server.DAL;

namespace Zuken.Server.Validator
{
    /// <summary>
    /// 相关对象的验证
    /// </summary>
    public class ValidateZKRObjDocExists : DefaultValidator
	{
        private DocversionManager manager = new DocversionManager();
		private IntegrationType IntegType;
		private Func<DocStruct, string> GetFileNameByDocStruct;
		private Func<DocStruct, string> GetIntegTypeByDocStruct;
		public ValidateZKRObjDocExists(IntegrationType integType)
		{
			this.IntegType = integType;
			this.GetIntegTypeByDocStruct = ((DocStruct p) => Key.ZKPRJ);
			this.GetFileNameByDocStruct = ((DocStruct p) => p.FileName);
		}
		public override bool Validate(ValidateContext context)
		{
			DocStruct docStruct = context.ValidateObject as DocStruct;
			if (docStruct != null && BOMHelp.IsNullOrEmpty(docStruct.ParentRelation))
			{
				string text = this.GetIntegTypeByDocStruct(docStruct);
				DocumentVersion documentVersionByFileName = this.manager.GetDocumentVersionByFileName(this.GetFileNameByDocStruct(docStruct), text);
				if (documentVersionByFileName == null)
				{
					docStruct.SetUploadState(false);
					docStruct.DocMsg = new Remark("red", string.Format(MutiLanguageManager.L("相关[{0}]文档不存在！"), text));
					return false;
				}
				docStruct.AddValue("__CISVerId", documentVersionByFileName.VerId);
				docStruct.AddValue("__CISDocId", documentVersionByFileName.DocId);
				BOMHelp.Add(context.ExtendProperty, "__RO_CIS_Doc", documentVersionByFileName);
				docStruct.SetUploadState(true);
			}
			return true;
		}
		private DocumentVersion GetDocByName(string filename)
		{
			string fileName = BOMHelp.ConvertToFileFromSchToPcb(filename, this.IntegType);
			IntegrationType integrationtype = BOMHelp.ConvertToIntegrationType(this.IntegType);
			return this.manager.GetDocumentVersionByFileName(fileName, BOMHelp.ConvertToAppType(integrationtype));
		}
	}
}
