using System;

namespace SPOC.Statement.Dto.Lib
{
    /// <summary>
    /// 学生知识点统计项
    /// </summary>
    public class StudentLabelGettingItem
    {
        /// <summary>
        /// 学生ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        public string UserLoginName { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string UserFullName { get; set; }
        /// <summary>
        /// 班级ID
        /// </summary>
        public Guid ClassId { get; set; }
        /// <summary>
        /// 班级名称
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 标签掌握数
        /// </summary>
        public int MasterNum { get; set; }
        /// <summary>
        /// 标签未掌握数
        /// </summary>
        public int FailNum { get; set; }
        /// <summary>
        /// 标签不稳定数
        /// </summary>
        public int UnskilledNum { get; set; }
        /// <summary>
        /// 标签无反馈数
        /// </summary>
        public int EmptyNum { get; set; }
        /// <summary>
        /// 知识点总数
        /// </summary>
        public int LabelNum { get; set; }
        /// <summary>
        /// 掌握比率
        /// </summary>
        public float MasterRate { get; set; }
    }
}