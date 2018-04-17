using System;
using SPOC.Common.Pagination;

namespace SPOC.Statement.Dto.Exercise
{
    /// <summary>
    /// 某人练习记录分页
    /// </summary>
    public class ExerciseRecordStatementPaginationInputDto:PaginationInputDto
    {
        /// <summary>
        /// 练习Id
        /// </summary>
        public Guid ExerciseId { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserId { get; set; }
    }
}