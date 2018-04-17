using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using SPOC.Exam;

namespace SPOC.QuestionBank
{
    public class QuestionTypeService:ApplicationService, IQuestionTypeService
    {
        private readonly IRepository<ExamQuestionType, Guid> _iExamQuestionTypeRep;
        public QuestionTypeService(IRepository<ExamQuestionType, Guid> iExamQuestionRepository)
        {
            _iExamQuestionTypeRep = iExamQuestionRepository;
        }
        public async Task<List<ExamQuestionType>> Get()
        {
            return await Task.FromResult(_iExamQuestionTypeRep.GetAll().ToList());
        }
    }
}