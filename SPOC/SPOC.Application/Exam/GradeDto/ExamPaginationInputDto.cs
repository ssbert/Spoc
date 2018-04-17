using SPOC.Common.Pagination;
using System;

namespace SPOC.Exam.GradeDto
{
    public class ExamPaginationInputDto:PaginationInputDto
    {
        public Guid courseUid { get; set; }
        public string examName { get; set; }
        public string examCode { get; set; }
    }
}
