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
    /// 验证物料
    /// </summary>
    public class ValidateMaterial : DefaultValidator
    {
        private MaterialBaseManager materialManager = new MaterialBaseManager();
        private BusinessCategoryManager bcategorymanager = new BusinessCategoryManager();

        public override bool Validate(ValidateContext context)
        {
            DocStruct doc = context.ValidateObject as DocStruct;
            BOMStruct bom = context.ValidateList as BOMStruct;
            if (doc != null && BOMHelp.IsEquals(doc.GetString(Key.FileType), Key.ZKMAT)) //物料
            {
                var root = bom.FindRootDoc();
                string MaterialCode = doc.GetString("MaterialCode");
                if (string.IsNullOrEmpty(MaterialCode))
                {
                    doc.MatMsg = new Remark("blue", MutiLanguageManager.L("不存在"));
                    root.AddValue("__HasNOExists", "1");//有不存在的
                }
                else
                {
                    MaterialVersion materialVersion = this.materialManager.GetAllVersionByCode(MaterialCode)
                        .FirstOrDefault<MaterialVersion>(p => p.IsEffect);
                    if (materialVersion != null)
                    {
                        if (root != null)
                        {
                            string materialVerId = root.MaterialVerId;
                            if (materialVerId == materialVersion.MaterialVerId)
                            {
                                root.MatMsg = new Remark("red", MutiLanguageManager.L("不能循环引用物料"));
                                return false;
                            }
                        }
                        if (materialVersion.EnglishName != doc.GetString("MaterialName"))
                        {
                            doc.MatMsg = new Remark("red", MutiLanguageManager.L("不匹配"));
                            return false;
                        }                      
                        
                        doc.MaterialCategoryId = materialVersion.CategoryId;
                        doc.SetVerId(materialVersion.MaterialVerId);
                        doc.AddValue("__matverid", materialVersion.MaterialVerId);
                        doc.AddValue("MaterialName", materialVersion.Name);
                        doc.AddValue("MaterialCode", materialVersion.Code);
                        doc.AddValue(Key.MaterialSpec, materialVersion.Spec);
                        doc.AddValue(Key.BOMRemark, materialVersion.Remark);
                        BusinessCategory businessCategory = this.bcategorymanager.GetBusinessCategory(materialVersion.CategoryId);
                        if (businessCategory != null)
                        {
                            doc.MaterialCategoryName = businessCategory.CategoryName;
                        }
                        if (materialVersion.IsFrozen)
                        {
                            doc.MatMsg = new Remark("red", MutiLanguageManager.L("已冻结"));
                            return false;
                        }
                        else
                        {
                            if (materialVersion.DesignCycle == DesignCycle.BlankOut)
                            {
                                doc.MatMsg = new Remark("red", MutiLanguageManager.L("已报废"));
                                return false;
                            }
                            else
                            {
                                doc.MatMsg = new Remark("black", MutiLanguageManager.L("已存在"));
                                return true;
                            }
                        } 
                    }
                    else
                    {
                        if (!BOMHelp.IsNullOrEmpty(doc.Child))
                        {
                            doc.MatMsg = new Remark("black", MutiLanguageManager.L("新建"));
                            return false;
                        }
                        else
                        {
                            doc.MatMsg = new Remark("red", MutiLanguageManager.L("找不到"));
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}
