using Newtonsoft.Json;
using System;

namespace SPOC.Exam.GradeDto
{
    /// <summary>
    /// 考试成绩分页项
    /// </summary>
    public class ExamGradeItem
    {
        /// <summary>
        /// 考试ID
        /// </summary>
        public Guid ExamId { get; set; }
        /// <summary>
        /// 成绩ID
        /// </summary>
        public Guid GradeId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 班级ID
        /// </summary>
        public Guid ClassId { get; set; }
        /// <summary>
        /// 班级名
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 用户登陆名
        /// </summary>
        public string UserLoginName { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string UserFullName { get; set; }
        /// <summary>
        /// 考试名
        /// </summary>
        public string ExamName { get; set; }
        /// <summary>
        /// 作答开始时间
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime BeginTime { get; set; }
        /// <summary>
        /// 作答结束时间
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 成绩
        /// </summary>
        public decimal? Score { get; set; }
        /// <summary>
        /// 试卷总分
        /// </summary>
        public decimal TotalScore { get; set; }
        /// <summary>
        /// 是否通过
        /// </summary>
        public string IsPass { get; set; }
        /// <summary>
        /// 答卷状态，release已发布、submitted已提交、judged已评卷、pause暂停中、examing考试中、judging评卷中
        /// </summary>
        public string GradeStatusCode { get; set; }
    }
}
