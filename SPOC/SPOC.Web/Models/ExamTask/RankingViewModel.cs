using SPOC.Common.Pagination;
using SPOC.Exam.ViewDto;

namespace SPOC.Web.Models.ExamTask
{
    public class RankingViewModel
    {
        /// <summary>
        /// 考试基础信息
        /// </summary>
        public ExamTaskBaseViewOutputDto Base { get; set; }
        /// <summary>
        /// 自己排行数据
        /// </summary>
        public ExamRankingViewItem SelfRanking { get; set; }
        /// <summary>
        /// 排行分页数据
        /// </summary>
        public PaginationOutputDto<ExamRankingViewItem> Pagination { get; set; }
        /// <summary>
        /// 排行榜考试类型
        /// </summary>
        public string ExamTypeCode { get; set; }
    }
}