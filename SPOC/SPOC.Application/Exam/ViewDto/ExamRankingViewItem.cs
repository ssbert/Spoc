using System;

namespace SPOC.Exam.ViewDto
{
    /// <summary>
    /// 考试排行榜项
    /// </summary>
    public class ExamRankingViewItem
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExamRankingViewItem()
        {
            GradeId = Guid.Empty;
            IsPass = false;
        }
        /// <summary>
        /// 成绩ID
        /// </summary>
        public Guid GradeId { get; set; }
        /// <summary>
        /// 学生ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 用户姓名/用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 成绩
        /// </summary>
        public decimal? GradeScore { get; set; }
        /// <summary>
        /// 是否通过
        /// </summary>
        public bool IsPass { get; set; }
        /// <summary>
        /// 排名
        /// </summary>
        public int Ranking { get; set; }
        /// <summary>
        /// 考试模式 exam_normal, exam_retest
        /// </summary>
        public string ExamTypeCode { get; set; }
    }
}