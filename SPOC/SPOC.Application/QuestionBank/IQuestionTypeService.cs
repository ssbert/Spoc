using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using SPOC.Exam;
using System.Web.Http;

namespace SPOC.QuestionBank
{
    public interface IQuestionTypeService:IApplicationService
    {
        [HttpGet]
        Task<List<ExamQuestionType>> Get();
    }
}