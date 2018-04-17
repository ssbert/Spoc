using System;
using System.Collections.Generic;

namespace SPOC.Exam.Dto
{
    public class UserExamInfoOutputDto
    {
        public Guid UserId { get; set; }
        public string ExamDomain { get; set; }
        public List<UserExamInfo> UserExamInfoList { get; set; }
    }
}
