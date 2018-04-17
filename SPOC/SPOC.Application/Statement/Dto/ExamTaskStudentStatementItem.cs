using System;

namespace SPOC.Statement.Dto
{
    /// <summary>
    /// 学生考试情况报表项
    /// </summary>
    public class ExamTaskStudentStatementItem
    {
        /// <summary>
        /// 学生Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 学生用户名
        /// </summary>
        public string UserLoginName { get; set; }
        /// <summary>
        /// 学生姓名
        /// </summary>
        public string UserFullName { get; set; }
        /// <summary>
        /// 班级Id
        /// </summary>
        public Guid ClassId { get; set; }
        /// <summary>
        /// 班级名
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 通过情况，1：已通过，2：未通过
        /// </summary>
        public int PassState { get; set; }
        /// <summary>
        /// 参加情况，1：已参加，2：未参加
        /// </summary>
        public int JoinState { get; set; }
        /// <summary>
        /// 提交情况，0：无数据，1：已提交，2：未提交
        /// </summary>
        public int SubmitState { get; set; }
        /// <summary>
        /// 评分情况，0：无数据，1：已出成绩，2：未出成绩
        /// </summary>
        public int CompileState { get; set; }
        /// <summary>
        /// 学生成绩
        /// </summary>
        public decimal? Score { get; set; }
    }
}