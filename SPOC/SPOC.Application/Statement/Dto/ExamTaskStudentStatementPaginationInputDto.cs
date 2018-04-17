using System;
using System.Collections.Generic;
using SPOC.Common.Pagination;

namespace SPOC.Statement.Dto
{
    /// <summary>
    /// 学生考试情况报表查询
    /// </summary>
    public class ExamTaskStudentStatementPaginationInputDto:PaginationInputDto
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExamTaskStudentStatementPaginationInputDto()
        {
            ClassIdList = new List<Guid>();
        }
        /// <summary>
        /// 考试任务ID
        /// </summary>
        public Guid ExamTaskId { get; set; }
        /// <summary>
        /// 班级Id列表
        /// </summary>
        public List<Guid> ClassIdList { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserLoginName { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string UserFullName { get; set; }
        /// <summary>
        /// 通过情况，0：全部，1：已通过，2：未通过
        /// </summary>
        public int PassState { get; set; }
        /// <summary>
        /// 参加情况，0：全部，1：已参加，2：未参加
        /// </summary>
        public int JoinState { get; set; }
        /// <summary>
        /// 提交情况，0：全部，1：已提交，2：未提交
        /// </summary>
        public int SubmitState { get; set; }
        /// <summary>
        /// 评分情况，0：全部，1：已出成绩，2：等待评分
        /// </summary>
        public int CompileState { get; set; }
    }
}