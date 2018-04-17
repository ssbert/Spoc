using System;
using SPOC.Common.Pagination;

namespace SPOC.Exercises.Dto
{
    /// <summary>
    /// 练习排行分页查询
    /// </summary>
    public class ExerciseRankingPaginationInputDto:PaginationInputDto
    {
        /// <summary>
        /// 练习ID
        /// </summary>
        public Guid ExerciseId { get; set; }
    }
}