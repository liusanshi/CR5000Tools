using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Proway.PLM.Document;
using Proway.Framework;
using Zuken.Server.Validator;
using Proway.PLM.Material;
using Zuken.Server.DAL;

namespace Zuken.Server
{
    /// <summary>
    /// zuken pcb 处理
    /// </summary>
    public class ZukenPcbOperate : DocumentOperateADPCB
    {

        #region 变量
        /// <summary>
        /// 新的文档管理类
        /// </summary>
        DocversionManager docversionManager1 = new DocversionManager();

        #endregion

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
            server.RegisterValidator(new ValidateZKRObjDocExists(base.IntegType)); //只会跑prj文件
            server.RegisterValidator(new ValidateRelationObjectMExists(true, (DocStruct p) => BOMHelp.IsNullOrEmpty(p.ParentRelation)));
            server.RegisterValidator(new ValidateMaterialChild());
            server.RegisterValidator(new ValidateZKReference(false));
            server.RegisterValidator(new ValidateDocName(false));
            server.RegisterValidator(new ValidateRepeatReference());
            server.RegisterValidator(new ValidateDocStrcutChange());
        }
        /// <summary>
        /// 导入时 注册验证器
        /// </summary>
        /// <param name="server"></param>
        protected override void ImportValidator(ValidateServer server)
        {
            server.RegisterValidator(new ValidateZKPCBImport());
            server.RegisterValidator(new ValidateOption());
            if (base.CurOperatePosition == OperatePosition.Task || base.CurOperatePosition == OperatePosition.Project)
            {
                server.RegisterValidator(new ValidateWork(base.CurOperatePosition == OperatePosition.Project));
            }
            server.RegisterValidator(new ValidateRelationObjectMExists(true));
            server.RegisterValidator(new ValidateMaterialChild());
            server.RegisterValidator(new ValidateDocumentName());
            server.RegisterValidator(new ValidateZKReference(false));
            server.RegisterValidator(new ValidateDocName(false));
            server.RegisterValidator(new ValidateDocumentCopy(base.IsCreateCopy, CreateCopyType.Import));
            server.RegisterValidator(new ValidateRepeatReference());
        }

        #endregion

        #region 初始化
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
                foreach (DocStruct current in bom.Where(p => 
                    BOMHelp.IsEquals(p.GetString(Key.FileType), Key.ZKPCB) &&
                    !BOMHelp.IsNullOrEmpty(p.Child)))
                {
                    foreach (ChildStruct current2 in current.Child.Where((ChildStruct c) => c.ChildNode != null))
                    {
                        if (!current2.ChildNode.ContainsKey(Key.Layer_zk))
                        {
                            string lv = current2.ChildNode.GetString(this.LAYERKEY);
                            current2.ChildNode.WriteValue(Key.Layer_zk, lv);
                            current2.ChildNode.WriteValue(this.LAYERKEY, Helper.ConvertToLayer(lv).ToString());
                            current2.PartReference = Helper.ConvertToLayer(lv);
                        }
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
            string[] condition = new string[] { "RealityVerId", this.LAYERKEY };
            List<ChildStruct> child = doc.Child.Where(p => p.ChildNode != null && BOMHelp.IsEquals(p.ChildNode.GetString(Key.FileType), Key.ZKMAT)
                    && !string.IsNullOrEmpty(p.ChildNode.GetString("MaterialCode"))).ToList();
            Dictionary<string, ChildStruct> dictionary = new Dictionary<string, ChildStruct>();            
            if (!BOMHelp.IsNullOrEmpty(child))
            {
                foreach (ChildStruct current in child)
                {
                    string valueFromDictionary = BOMHelp.GetValueFromDictionary(current.ChildNode, condition, "_");
                    if (dictionary.ContainsKey(valueFromDictionary))
                    {
                        ChildStruct expr_73 = dictionary[valueFromDictionary];
                        expr_73.Count += 1f;                     
                        expr_73.Position = expr_73.Position + "," + current.Position;
                    }
                    else
                    {
                        dictionary.Add(valueFromDictionary, current);
                    }
                }
            }
            return (from p in dictionary select p.Value).ToList<ChildStruct>();
        }
        #endregion

        #region 装配数据
        /// <summary>
        /// 根据 DocStruct 获取 DocumentVersion 
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        protected override DocumentVersion GetDocumentVersionByDocStruct(DocStruct doc)
        {
            DocumentVersion result;
            if (string.IsNullOrEmpty(doc.FileName))
            {
                result = null;
            }
            else
            {
                result = docversionManager1.GetDocumentVersionByFileName(doc.FileName, doc.GetString(Key.FileType));
            }
            return result;
        }

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
                var docver = docentities.CurDocumentVersion.Entity;
                if (docver != null)
                {
                    docver.AppType = doc.GetString(Key.FileType);
                    docver.FileSize = doc.FileSize;
                }
            }
        }
        /// <summary>
        /// 物料关系
        /// </summary>
        /// <param name="docentities"></param>
        /// <param name="doc"></param>
        protected override void GetParentMaterialRelation(DocumentVersionEntity docentities, DocStruct doc)
        {
            if (BOMHelp.IsEquals(doc.GetString(Key.FileType), Key.ZKPCB))
            {
                doc.WriteValue(_.FileType, "ADPCB");
                base.GetParentMaterialRelation(docentities, doc);
                doc.WriteValue(_.FileType, Key.ZKPCB);
            }
        }
        #endregion

        #region 页面显示配置
        /// <summary>
        /// 层对应的字段名称
        /// </summary>
        protected override string LAYERKEY
        {
            get
            {
                return "Layer";
            }
        }

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
		        new Column{Header = base.L("文件名称"),Id = "FileName",DataIndex = "FileName",Width = 160,Align = AlignEnum.left,HeaderAlign = AlignEnum.center,Renderer = "fileNameRender"},
		        new Column{Header = base.L("文件大小"),Id = "FileSize",DataIndex = "FileSizeUnit",Width = 75,Align = AlignEnum.left,HeaderAlign = AlignEnum.center},
                new Column{Header = base.L("文件路径"),Id = "FilePath",DataIndex = "FilePath",Width = 167,Align = AlignEnum.left,HeaderAlign = AlignEnum.center},
		        new Column{Header = base.L("数量"),Id = "Count",DataIndex = "Count",Width = 50,Align = AlignEnum.left,HeaderAlign = AlignEnum.center},
		        new Column{Header = base.L("上传状态"),Id = "UploadState",DataIndex = "UploadState",Width = 75,Align = AlignEnum.left,HeaderAlign = AlignEnum.center},
		        new Column{Header = base.L("Bom"),Id = "MaterialCode",DataIndex = "MaterialCode",Width = 100,Align = AlignEnum.left,HeaderAlign = AlignEnum.center,Renderer = "selectProductRender"},
		        new Column{Header = base.L("情况备注"),Id = "DocMsg",DataIndex = "DocMsg",Width = 160,Align = AlignEnum.left,HeaderAlign = AlignEnum.center}
	        });
            dictionary.Add("BOM", new List<Column>
	        {
		        new Column{Header = base.L("序号"),Id = "OrderId",Width = 50,DataIndex = "OrderId",Align = AlignEnum.left,HeaderAlign = AlignEnum.center},
		        new Column{Header = base.L("位号"),Id = "Reference",Width = 50,DataIndex = "Reference",Align = AlignEnum.left,HeaderAlign = AlignEnum.center},
		        new Column{Header = base.L("值"),Id = "Value",DataIndex = "Value",Width = 100,Align = AlignEnum.left,HeaderAlign = AlignEnum.center},
		        new Column{Header = base.L("层"),Id = Key.Layer_zk,DataIndex = Key.Layer_zk,Width = 50,Align = AlignEnum.left,HeaderAlign = AlignEnum.center},
		        new Column{Header = base.L("状态"),Id = "MatMsg",DataIndex = "MatMsg",Width = 133,Align = AlignEnum.left,HeaderAlign = AlignEnum.center},
		        new Column{Header = base.L("封装"),Id = "Footprint",DataIndex = "Footprint",Width = 100,Align = AlignEnum.left,HeaderAlign = AlignEnum.center},
		        new Column{Header = base.L("封装描述信息"),Id = "FootPrintDescription",DataIndex = "FootPrintDescription",Width = 188,Align = AlignEnum.left,HeaderAlign = AlignEnum.center}

	        });
            dictionary["BOM"][0].SetColumnTypeAndDefaultValue(ColumnType.Int, 0);
            return dictionary;
        }
        #endregion
    }
}
