using System;
using System.Collections.Generic;
using System.Linq;

using Proway.PLM.Document;
using Proway.Framework;
using Zuken.Server.Validator;
using Proway.PLM.Settings;

namespace Zuken.Server
{
    /// <summary>
    /// Zuken sch 处理 
    /// </summary>
    public class ZukenSchOperate : DocumentOperateADSCH
    {
        #region 验证
        /// <summary>
        /// 检入时 注册验证器
        /// </summary>
        /// <param name="server"></param>
        protected override void CheckInValidator(ValidateServer server)
        {
            server.RegisterValidator(new ValidateRootExists());
            server.RegisterValidator(new ValidateDocumentExists());
            server.RegisterValidator(new ValidateDocumentOperateType());
            server.RegisterValidator(new ValidateDocumentCopy(base.IsCreateCopy, CreateCopyType.CheckIn));
            server.RegisterValidator(new ValidateDocumentIsCheckOut(base.UserId));
            server.RegisterValidator(new ValidateMaterial());
            server.RegisterValidator(new ValidateDocName(true, (DocStruct p) => !BOMHelp.IsEquals(p.GetString(Key.FileType), Key.ZKMAT)));
            server.RegisterValidator(new ValidateRelationObjectMExists(false, (DocStruct p) => BOMHelp.IsNullOrEmpty(p.ParentRelation)));
            server.RegisterValidator(new ValidateParentDocumentIsCheckOut());
            server.RegisterValidator(new ValidateDocStrcutChange());
            //?
            server.RegisterValidator(new ValidateDeleteDoc(this.dicDeleteDoc, (DocumentVersion docver, DocStruct doc) => BOMHelp.IsEquals(BOMHelp.GetFileNameNoVersion(docver.FileName), BOMHelp.GetFileNameNoVersion(doc.FileName)), false));
            server.RegisterValidator(new ValidateRepeatReference());
        }
        /// <summary>
        /// 导入时 注册验证器
        /// </summary>
        /// <param name="server"></param>
        protected override void ImportValidator(ValidateServer server)
        {
            server.RegisterValidator(new ValidateOption());
            server.RegisterValidator(new ValidateDocumentName());
            server.RegisterValidator(new ValidateBOMExists());
            server.RegisterValidator(new ValidateMaterial());
            server.RegisterValidator(new ValidateDocName(true, (DocStruct p) => !BOMHelp.IsEquals(p.GetString("filetype"), "ADMAT")));
            server.RegisterValidator(new ValidateDocumentCopy(base.IsCreateCopy, CreateCopyType.Import));
            server.RegisterValidator(new ValidateDocStrcutChange());
            server.RegisterValidator(new ValidateRepeatReference());
        }

        #endregion

        #region 初始化

        public override IDictionary<string, bool> ControlVisible
        {
            get
            {
                var cv = base.ControlVisible;
                cv[CB_ISCREATEBOM] = false;

                return cv;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string OperateJsPath
        {
            get
            {
                return "../Javascript/Integration/ZSKIntegration.js";
            }
        }
        /// <summary>
        /// 合并属性
        /// </summary>
        /// <param name="bom"></param>
        protected override void InitBOMProperty(BOMStruct bom)
        {
            if (bom != null && base.CurActiveType == ActiveType.Validate)
            {
                foreach (DocStruct current in bom.Where(p => !BOMHelp.IsNullOrEmpty(p.Child)))
                {
                    foreach (ChildStruct current2 in current.Child.Where((ChildStruct c) => c.ChildNode != null))
                    {
                        current2.PartReference = Helper.ConvertToLayer(current2.ChildNode.GetString(this.LAYERKEY));
                        current2.Position = current2.ChildNode.Reference;
                        current2.Count = current2.ChildNode.Count;
                        current2.OrderId = current2.ChildNode.GetString(_.ORDERID);
                        current2.BomRemark = current2.ChildNode.GetString(_.BOMREMARK);
                    }
                }
            }
        }
        
        /// <summary>
        /// 合并关系时剔除没有物料编码的
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        protected override IList<ChildStruct> MergeRelation(DocStruct doc)
        {
            List<DocStruct> list = doc.Owner.Where(p => BOMHelp.IsEquals(p.GetString(Key.FileType), Key.ZKMAT)
                    && !string.IsNullOrEmpty(p.GetString("MaterialCode"))).ToList();

            List<ChildStruct> list2 = new List<ChildStruct>(list.Count);
            
            foreach (DocStruct current in list)
            {
                ChildStruct childStruct = current.ParentRelation.FirstOrDefault<ChildStruct>();
                if (childStruct != null)
                {
                    childStruct.Position = current.Reference;
                    childStruct.Count = current.Count;
                    childStruct.BomRemark = current.GetString("BOMRemark");
                    childStruct.ParentNode = doc;
                    list2.Add(childStruct);
                }
            }
            return list2;
        }
        #endregion

        #region 数据组装

        /// <summary>
        /// 组装 GetDocumentVersion
        /// </summary>
        /// <param name="docentities"></param>
        /// <param name="doc"></param>
        /// <param name="IndexFields"></param>
        protected override void GetDocumentVersion(DocumentVersionEntity docentities, DocStruct doc, List<Associatefield> IndexFields)
        {
            base.GetDocumentVersion(docentities, doc, IndexFields);
            if (!BOMHelp.IsEquals(doc.GetString(Key.FileType), Key.ZKMAT))
            {
                docentities.CurDocumentVersion.Entity.AppType = doc.GetString(Key.FileType);
            }
        }
        /// <summary>
        /// 根据 DocStruct 查找 DocumentVersion
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        protected override DocumentVersion GetDocumentVersionByDocStruct(DocStruct doc)
        {
            if (string.IsNullOrEmpty(doc.FileName))
            {
                return null;
            }
            else
            {
                return base.docversionManager.GetDocumentVersionByFileName(doc.FileName, doc.GetString(Key.FileType));
            }
        }
        #endregion

        #region 页面显示配置
        /// <summary>
        /// 数据对应关系
        /// </summary>
        /// <returns></returns>
        protected override IList<Associatefield> GetDocStructAssociateField()
        {
            return new List<Associatefield>
			{
				new Associatefield("MaterialCode", "Stock No."),
				new Associatefield("MaterialName", "Part Name"),
				//new Associatefield("SchematicPart", "Description"),
				//new Associatefield("MaterialCategoryName", "Part Type"),
                new Associatefield("FootPrint", "footprint name"),
				new Associatefield("Reference", "reference list")
			};
        }
        /// <summary>
        /// 表头显示的列
        /// </summary>
        /// <returns></returns>
        protected override Dictionary<string, IList<Column>> GetColumn()
        {
            Dictionary<string, IList<Column>> dictionary = new Dictionary<string, IList<Column>>();
            dictionary.Add("Doc", new List<Column>
			{
				new Column{Header = base.L("文件名称"),Id = "FileName",DataIndex = "FileName",Width = 155,Align = AlignEnum.left,HeaderAlign = AlignEnum.center,Renderer = "fileNameRender"},
				new Column{Header = base.L("文件大小"),Id = "FileSize",DataIndex = "FileSizeUnit",Width = 100,Align = AlignEnum.left,HeaderAlign = AlignEnum.center},
                new Column{Header = base.L("文件路径"),Id = "FilePath",DataIndex = "FilePath",Width = 167,Align = AlignEnum.left,HeaderAlign = AlignEnum.center},
				new Column{Header = base.L("数量"),Id = "Count",DataIndex = "Count",Width = 100,Align = AlignEnum.left,HeaderAlign = AlignEnum.center},
				new Column{Header = base.L("上传状态"),Id = "UploadState",DataIndex = "UploadState",Width = 100,Align = AlignEnum.left,HeaderAlign = AlignEnum.center},
				new Column{Header = base.L("情况备注"),Id = "DocMsg",DataIndex = "DocMsg",Width = 160,Align = AlignEnum.left,HeaderAlign = AlignEnum.center}
			});

            dictionary.Add("BOM", new List<Column>
			{
				new Column{Header = base.L("序号"),Id = "OrderId",Width = 50,DataIndex = "OrderId",Align = AlignEnum.left,HeaderAlign = AlignEnum.center},
				new Column{Header = base.L("物料编码"),Id = "MaterialCode",Width = 123,DataIndex = "MaterialCode",Align = AlignEnum.left,HeaderAlign = AlignEnum.center,Renderer = "orderIdRender"},
				new Column{Header = base.L("采购名称"),Id = "MaterialName",Width = 140,DataIndex = "MaterialName",Align = AlignEnum.left,HeaderAlign = AlignEnum.center},
                new Column{Header = base.L("封装描述"),Id = "FootPrint",DataIndex = "FootPrint",Width = 120,Align = AlignEnum.left,HeaderAlign = AlignEnum.center},
				new Column{Header = base.L("值"),Id = "Value",DataIndex = "value",Width = 80,Align = AlignEnum.left,HeaderAlign = AlignEnum.center},
                new Column{Header = base.L("数量"),Id = "Count",DataIndex = "Count",Width = 50,Align = AlignEnum.left,HeaderAlign = AlignEnum.center},
				new Column{Header = base.L("位号"),Id = "Reference",DataIndex = "Reference",Width = 120,Align = AlignEnum.left,HeaderAlign = AlignEnum.center},
				new Column{Header = base.L("状态"),Id = "MatMsg",DataIndex = "MatMsg",Width = 100,Align = AlignEnum.left,HeaderAlign = AlignEnum.center}
                
                ,new Column{Header = base.L("业务类型"),Id = Key.MaterialCategoryName,DataIndex = Key.MaterialCategoryName,Width = 98,Align = AlignEnum.left,HeaderAlign = AlignEnum.center}
                ,new Column{Header = base.L("规格"),Id = Key.MaterialSpec,DataIndex = Key.MaterialSpec,Width = 50,Align = AlignEnum.left,HeaderAlign = AlignEnum.center}
                ,new Column{Header = base.L("BOM备注"),Id = "BOMRemark",DataIndex = "BOMRemark",Width = 80,Align = AlignEnum.left,HeaderAlign = AlignEnum.center}
			});

            dictionary["BOM"][0].SetColumnTypeAndDefaultValue(ColumnType.Int, 0);
            return dictionary;
        }

        #endregion
    }
}
