using SPOC.Common.Pagination;
using SPOC.Exercises.Dto;

namespace SPOC.Web.Models.Exercises
{
    /// <summary>
    /// 练习排行榜视图模型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RankingViewModel<T>
    {
        /// <summary>
        /// 练习基础信息
        /// </summary>
        public ExerciseBaseViewOutputDto Base { get; set; }
        /// <summary>
        /// 排行分页数据
        /// </summary>
        public PaginationOutputDto<T> Pagination { get; set; }
        /// <summary>
        /// 自己排行数据
        /// </summary>
        public T SelfRanking { get; set; }
    }
}