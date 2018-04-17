using SPOC.Exam;

namespace SPOC.ExamPaper.Dto
{
    public class UserPaperViewInputDto
    {
        public Exam.ExamPaper paper { get; set; }
        public ExamExam exam { get; set; }
        public ExamGrade examGrade { get; set; }
        public string viewType { get; set; }
        public string filterType { get; set; }
    }
}
