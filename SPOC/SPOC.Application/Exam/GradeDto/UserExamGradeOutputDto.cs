using System;
using Newtonsoft.Json;

namespace SPOC.Exam.GradeDto
{
    public class UserExamGradeOutputDto
    {
        public Guid Id { get; set; }
        public Guid userUid { get; set; }
        public string userLoginName { get; set; }
        public string userFullName { get; set; }
        public decimal paperTotalScore { get; set; }
        [JsonConverter(typeof(DateFormat))]
        public DateTime beginTime { get; set; }
        [JsonConverter(typeof(DateFormat))]
        public DateTime? endTime { get; set; }
        public int? examTime { get; set; }
        public decimal? gradeScore { get; set; }
        public string isPass { get; set; }
        /// <summary>
        /// 成绩状态（examing考试中、submitted已提交、judging评卷中、judged已评分）
        /// </summary>
        public string gradeStatusCode { get; set; }
    }
}
