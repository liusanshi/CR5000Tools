using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Proway.Framework;
using Proway.PLM.Document;
using Proway.Framework.LanguagePack;
using Proway.PLM.Material;
using Proway.PLM.Settings;

namespace Zuken.Server.Validator
{
    /// <summary>
    /// 验证位号
    /// </summary>
    public class ValidateZKReference : DefaultValidator
    {
        private bool isCheckReference = true;
		public ValidateZKReference()
		{
		}
        /// <summary>
        /// 验证位号
        /// </summary>
        /// <param name="ischeckreference"></param>
        public ValidateZKReference(bool ischeckreference)
		{
			this.isCheckReference = ischeckreference;
		}
		public override bool Validate(ValidateContext context)
		{
			BOMStruct bOMStruct = context.ValidateList as BOMStruct;
			DocStruct doc = context.ValidateObject as DocStruct;
			bool result = true;
			IList<MaterialRelation> list = BOMHelp.GetOValueFromDictionary(context.ExtendProperty, "_LIST_MaterialRelation") as IList<MaterialRelation>;
			if (list != null)
			{
				if (doc == null || bOMStruct == null)
				{
					return result;
				}
				if (string.IsNullOrEmpty(doc.FileName))
				{
					if (!this.isCheckReference)
					{
						if (!list.Any((MaterialRelation p) => p.OriginalMode == 1))
						{
							return true;
						}
					}
                    if (string.IsNullOrEmpty(doc.GetString(_.MATERIALCODE))) //如果物料编码为空不验证位号
                    {
                        return true;
                    }
					MaterialRelation materialRelation = list.FirstOrDefault((MaterialRelation p) => BOMHelp.IndexOf(p.AssemblyPlace, doc.Reference, ",", true));
					if (materialRelation != null)
					{
						doc.MatMsg = new Remark(MutiLanguageManager.L("已存在"));
						doc.AddValue("__matverid", materialRelation.ChildVerId);
						doc.SetVerId(materialRelation.ChildVerId);
					}
					else
					{
						doc.MatMsg = new Remark("red", MutiLanguageManager.L("不存在"));
						result = false;
					}
				}
				else
				{
					IEnumerable<DocStruct> children = doc.GetChildren();
					if (!BOMHelp.IsNullOrEmptyG<DocStruct>(children))
					{
						DocStruct docStruct = children.FirstOrDefault<DocStruct>();
						if (docStruct != null && BOMHelp.IsNullOrEmpty(docStruct.Child))
						{
							result = !this.CheckLostReference(list, doc);
						}
					}
					else
					{
						result = !this.CheckLostReference(list, doc);
					}
				}
			}
			return result;
		}
		private bool CheckLostReference(IList<MaterialRelation> listRelation, DocStruct root)
		{
			bool result = false;
			IEnumerable<string> first = 
				from p in listRelation.SelectMany((MaterialRelation p) => p.AssemblyPlace.Split(new char[]
				{
					','
				}))
				where !string.IsNullOrEmpty(p)
				select p;
			IEnumerable<string> second = 
				from p in root.Owner.SelectMany((DocStruct p) => p.Reference.Split(new char[]
				{
					','
				}))
				where !string.IsNullOrEmpty(p)
				select p;
			string[] array = first.Except(second, StringComparer.OrdinalIgnoreCase).ToArray<string>();
			if (array.Length > 0)
			{
				root.SetDocStateL(false, "red", string.Format("位号：{0}丢失", string.Join(",", array)));
				result = true;
			}
			return result;
		}
    }
}
