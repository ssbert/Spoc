using System;
using System.Collections.Generic;
using SPOC.Common.Pagination;

namespace SPOC.Statement.Dto.Exercise
{
    /// <summary>
    /// 学生练习情况分页查询
    /// </summary>
    public class ExerciseStudentStatementPaginationInputDto:PaginationInputDto
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerciseStudentStatementPaginationInputDto()
        {
            ClassIdList = new List<Guid>();
        }
        /// <summary>
        /// 练习Id
        /// </summary>
        public Guid ExerciseId { get; set; }
        /// <summary>
        /// 班级Id
        /// </summary>
        public List<Guid> ClassIdList { get; set; }
        /// <summary>
        /// 学生用户名
        /// </summary>
        public string UserLoginName { get; set; }
        /// <summary>
        /// 学生姓名
        /// </summary>
        public string UserFullName { get; set; }
        /// <summary>
        /// 通过情况，0：全部，1：通过，2：未通过
        /// </summary>
        public int PassState { get; set; }
        /// <summary>
        /// 参加情况，0：全部，1：已参加，2：未参加
        /// </summary>
        public int JoinState { get; set; }
    }
}