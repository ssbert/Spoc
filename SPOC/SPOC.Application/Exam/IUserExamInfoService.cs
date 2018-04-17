using SPOC.Exam.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPOC.Exam
{
    public interface IUserExamInfoService
    {
        List<ExamInfoObj> GetExams(UserExamInputDto input,ref int total);

        List<ExamDetailObj> GetExamDetails(UserExamInputDto input, ref int total);
    }
}
