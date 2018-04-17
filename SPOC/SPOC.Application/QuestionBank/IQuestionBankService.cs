using Abp.Application.Services;
using SPOC.Common.Dto;
using SPOC.Common.Pagination;
using SPOC.Exam;
using SPOC.QuestionBank.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;

namespace SPOC.QuestionBank
{
    public interface IQuestionBankService:IApplicationService
    {
        /// <summary>
        /// 根据条件进行分页查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PaginationOutputDto<QuestionItem>> GetPagination(QuestionPaginationInputDto input);

        /// <summary>
        /// 创建一个新的试题
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ExamQuestionDto> Create(QuestionInputDto input);

        /// <summary>
        /// 更新试题数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Update(QuestionInputDto input);

        /// <summary>
        /// 删除试题
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost, HttpGet]
        Task Delete(string ids);

        /// <summary>
        /// 根据id获取试题信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, HttpGet]
        Task<ExamQuestionDto> Get(Guid id);
        
        /// <summary>
        /// 根据id获取子试题信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PaginationOutputDto<QuestionItem>> GetChildPagination(QuestionPaginationInputDto input);

        /// <summary>
        /// 根据ID获取试题所有子试题
        /// </summary>
        /// <param name="parentUid"></param>
        /// <returns></returns>
        Task<List<ExamQuestion>> GetChildren(string parentUid);

        /// <summary>
        /// 获取分类下指定难度试题数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<int> GetQuestionNum(QuestionNumInputDto input);

        /// <summary>
        /// 从上传的文件内容来批量创建试题
        /// </summary>
        /// <param name="fileStream">文件流</param>
        /// <param name="folderUid">分类</param>
        /// <param name="fileType">上传文件类型</param>
        /// <returns></returns>
        ImportResultOutputDto CreateFromFile(Stream fileStream, Guid folderUid, string fileType);

        /// <summary>
        /// 获取导出到word的试题文本内容
        /// </summary>
        /// <param name="input">需要导出的试题ID</param>
        /// <returns></returns>
        Task<Guid> ExportToWord(IdListInputDto input);
    }
}