using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Proway.PLM.Material;

namespace Zuken.Server
{
    /// <summary>
    /// 字典
    /// </summary>
    static class Key
    {
        /// <summary>
        /// ZKMAT
        /// </summary>
        public const string ZKMAT = "ADMAT";
        /// <summary>
        /// ZKSCH
        /// </summary>
        public const string ZKSCH = "ZKSCH";
        /// <summary>
        /// ZKPCB
        /// </summary>
        public const string ZKPCB = "ZKPCB";
        /// <summary>
        /// ZKPRJ
        /// </summary>
        public const string ZKPRJ = "ADPRJ";
        /// <summary>
        /// 文件类型
        /// </summary>
        public const string FileType = "filetype";

        /// <summary>
        /// 物料业务类型
        /// </summary>
        public const string MaterialCategoryName = "MaterialCategoryName";
        /// <summary>
        /// 物料规格
        /// </summary>
        public const string MaterialSpec = "MaterialSpec";
        /// <summary>
        /// BOM备注
        /// </summary>
        public const string BOMRemark = "BOMRemark";
        /// <summary>
        /// zuken显示层的key
        /// </summary>
        public const string Layer_zk = "Layer_zk";
        
    }

    static class Helper
    {
        /// <summary>
        /// 将 value 转换为 Layer
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Layer ConvertToLayer(string value)
        {
            Layer result = Layer.Top;
            value = value.Trim();
            if (string.Compare(value, "B_SIDE", true) == 0)
            {
                result = Layer.Bottom;
            }
            return result;
        }
    }
}
