using Abp.Application.Services;
using SPOC.ExamPaper.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace SPOC.ExamPaper
{
    public interface IExamPaperNodeQuestionService:IApplicationService
    {
        Task Create(ExamPaperNodeQuestionCreateInputDto input);

        [HttpPost, HttpGet]
        Task<List<ExamPaperNodeQuestionOutputDto>> GetList(Guid paperNodeUid);

        [HttpPost, HttpGet]
        Task<List<Guid>> GetIdList(Guid paperNodeUid);
        
        [HttpPost, HttpGet]
        Task<ExamPaperNodeQuestionOutputDto> Get(Guid id);
        Task Update(ExamPaperNodeQuestionInputDto input);

        [HttpPost, HttpGet]
        Task Delete(Guid nodeUid, string ids);
    }
}