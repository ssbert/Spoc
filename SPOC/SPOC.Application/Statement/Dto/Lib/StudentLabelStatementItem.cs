using System;

namespace SPOC.Statement.Dto.Lib
{
    /// <summary>
    /// 学生知识点掌握情况项
    /// </summary>
    public class StudentLabelStatementItem
    {
        /// <summary>
        /// 标签ID
        /// </summary>
        public Guid LabelId { get; set; }
        /// <summary>
        /// 知识点名称
        /// </summary>
        public string LabelTitle { get; set; }
        /// <summary>
        /// 标签分数
        /// </summary>
        public int? Score { get; set; }
    }
}