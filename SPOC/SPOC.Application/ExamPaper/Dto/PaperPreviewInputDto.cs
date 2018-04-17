using System;

namespace SPOC.ExamPaper.Dto
{
    public class PaperPreviewInputDto
    {
        public Guid paperUid { get; set; }
        public Guid policyUid { get; set; }
        public Guid examUid { get; set; }
        public Guid policyNodeUid { get; set; }
    }
}