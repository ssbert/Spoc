using System;
using SPOC.Common.Pagination;

namespace SPOC.Exam.ViewDto
{
    /// <summary>
    /// 考试排行榜分页查询
    /// </summary>
    public class RankingPaginationInputDto: PaginationInputDto
    {
        /// <summary>
        /// 考试Id
        /// </summary>
        public Guid ExamId { get; set; }

        /// <summary>
        /// 考试类型exam_normal，exam_train
        /// </summary>
        public string ExamTypeCode { get; set; }
    }
}