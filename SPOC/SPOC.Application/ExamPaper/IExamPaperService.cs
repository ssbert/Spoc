using Abp.Application.Services;
using Abp.Runtime.Validation;
using SPOC.Common;
using SPOC.Common.Pagination;
using SPOC.ExamPaper.Dto;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using SPOC.Common.Dto;

namespace SPOC.ExamPaper
{
    /// <summary>
    /// 试卷服务接口
    /// </summary>
    public interface IExamPaperService:IApplicationService
    {
        /// <summary>
        /// 根据分页获取数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PaginationOutputDto<ExamPaperPaginationItem>> GetPagination(ExamPaperPaginationInputDto input);

        /// <summary>
        /// 根据ID串删除试卷
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost, HttpGet]
        Task Delete(string ids);
        
        /// <summary>
        /// 获取一个试卷
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, HttpGet]
        Task<ExamPaperDto> Get(Guid id);

        /// <summary>
        /// 创建一个试卷
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ExamPaperDto> Create(ExamPaperInputDto input);
        
        /// <summary>
        /// 更新一个试卷
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Update(ExamPaperInputDto input);

        /// <summary>
        /// 构建试卷xml
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task BuidExamPaper(Guid id);

        /// <summary>
        /// 更新试卷总分与试题数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="score"></param>
        /// <param name="questionNum"></param>
        /// <returns></returns>
        [HttpPost, HttpGet]
        Task UpdateTotalScoreAndQuestionNum(Guid id, decimal score, int questionNum);

        /// <summary>
        /// 获取试卷预览数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PaperPreviewOutputDto> GetPaperPreview(PaperPreviewInputDto input);

        /// <summary>
        /// 显示答卷信息 DisableValidation 属性标记不进行参数值校验
        /// </summary>
        /// <param name="examGradeUid"></param>
        /// <param name="filterType"></param>
        /// <param name="judge"></param>
        /// <returns></returns>
        [DisableValidation]
        Task<UserPaperViewOutputDto> GetUserPaperView(Guid examGradeUid, string filterType, bool judge = false);

        /// <summary>
        /// 根据exmaId获取试卷预览数据
        /// </summary>
        /// <param name="examId"></param>
        /// <returns></returns>
        Task<PaperPreviewOutputDto> GetExamPreview(Guid examId);

        /// <summary>
        /// 根据导入的文件生成试卷
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="questionFolderUid"></param>
        /// <param name="paperFolderUid"></param>
        /// <returns></returns>
        ImportResultOutputDto CreateFromFile(Stream fileStream, Guid questionFolderUid, Guid paperFolderUid);

        /// <summary>
        /// 导出试卷到word
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task ExportToWord(Guid id);

            /// <summary>
        /// 根据试卷Id导出Html
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<string> ExportPaperPreviewHtml(Guid id);

        /// <summary>
        /// 更新试卷的XML字段值
        /// </summary>
        /// <param name="retValue"></param>
        /// <returns></returns>
        void UpdatePaperXml(ReturnValue retValue);
    }
}