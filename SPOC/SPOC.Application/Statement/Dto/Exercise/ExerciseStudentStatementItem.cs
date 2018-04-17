using System;

namespace SPOC.Statement.Dto.Exercise
{
    /// <summary>
    /// 学生练习情况分页项
    /// </summary>
    public class ExerciseStudentStatementItem
    {
        /// <summary>
        /// 学生ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 学生登录名
        /// </summary>
        public string UserLoginName { get; set; }
        /// <summary>
        /// 学生姓名
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
        /// 通过情况，1：已通过，2：未通过
        /// </summary>
        public int PassState { get; set; }
        /// <summary>
        /// 参加情况，1：已参加，2：未参加
        /// </summary>
        public int JoinState { get; set; }
    }
}