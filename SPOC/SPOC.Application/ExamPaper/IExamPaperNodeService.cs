using Abp.Application.Services;
using SPOC.ExamPaper.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace SPOC.ExamPaper
{
    public interface IExamPaperNodeService : IApplicationService
    {
        [HttpPost, HttpGet]
        Task Delete(string ids);
        Task Update(ExamPaperNodeInputDto input);

        Task<ExamPaperNodeOutputDto> Create(ExamPaperNodeInputDto input);

        [HttpPost, HttpGet]
        Task<List<ExamPaperNodeOutputDto>> GetList(Guid paperUid);

        [HttpPost, HttpGet]
        Task<ExamPaperNodeOutputDto> Get(Guid id);

        [HttpPost, HttpGet]
        Task UpdateTotalScoreAndQuestionNum(Guid id, decimal score, int questionNum);
    }
}