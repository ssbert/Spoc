using System;
using System.Collections.Generic;

namespace SPOC.Common.Dto
{
    public class ImportFieldModel
    {
        public ImportFieldModel()
        {
            FieldEnumValue = new List<string>();
        }
        public ImportFieldModel(string[] fieldEnumValueArray)
        {
            FieldEnumValue = new List<string>();
            FieldEnumValue.AddRange(fieldEnumValueArray);
        }
        /// <summary>
        /// 对应数据库字段名
        /// </summary>
        public string FieldCode { get; set; }
        /// <summary>
        /// 模板字段名称
        /// </summary>
        public string FieldName { get; set; }

        public bool isUfoProperty { get; set; }
        /// <summary>
        /// 是否必填
        /// </summary>
        public Boolean MustFlag { get; set; }
        /// <summary>
        /// 字段的最大长度
        /// </summary>
        public int MaxLength { get; set; }
        /// <summary>
        /// 字段的排序号
        /// </summary>
        public int OrderNum { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 模板的字段显示长度
        /// </summary>
        public int FieldLength { get; set; }

        public bool isDateTimeType { get; set; }

        public List<string> FieldEnumValue { get; set; }
    }
}
