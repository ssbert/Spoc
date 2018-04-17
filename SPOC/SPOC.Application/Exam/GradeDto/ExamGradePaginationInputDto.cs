using SPOC.Common.Pagination;
using System;
using System.Collections.Generic;

namespace SPOC.Exam.GradeDto
{
    /// <summary>
    /// 考试成绩分页查询
    /// </summary>
    public class ExamGradePaginationInputDto : PaginationInputDto
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExamGradePaginationInputDto()
        {
            ClassIds = new List<Guid>();
        }
        /// <summary>
        /// 班级ID列表
        /// </summary>
        public List<Guid> ClassIds { get; set; }
        /// <summary>
        /// 考试任务ID
        /// </summary>
        public Guid TaskId { get; set; }
        /// <summary>
        /// 用户登录名
        /// </summary>
        public string UserLoginName { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string UserFullName { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? BeginTime { get; set; }
        /// <summary>
        /// /结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 考试类型，exam_normal: 正考, exam_retest: 补考
        /// </summary>
        public string ExamTypeCode { get; set; }
        /// <summary>
        /// 答卷状态，release已发布、submitted已提交、judged已评卷、pause暂停中、examing考试中、judging评卷中
        /// </summary>
        public string GradeStatusCode { get; set; }
        /// <summary>
        /// 最低分
        /// </summary>
        public decimal? MinScore { get; set; }
        /// <summary>
        /// 最高分
        /// </summary>
        public decimal? MaxScore { get; set; }
    }
}
