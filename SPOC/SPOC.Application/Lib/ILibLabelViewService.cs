using System;
using System.Collections.Generic;
using Abp.Application.Services;
using System.Threading.Tasks;
using System.Web.Http;
using SPOC.Common.Pagination;
using SPOC.Lib.Dto;

namespace SPOC.Lib
{
    /// <summary>
    /// 知识点前台展示相关接口
    /// </summary>
    public interface ILibLabelViewService: IApplicationService
    {
        /// <summary>
        /// 获取自己的标签积分数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        Task<Dictionary<Guid, int?>> GetSelfLabelScore();

        /// <summary>
        /// 获取用户的标签积分数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        Task<Dictionary<Guid, int?>> GetUserLabelScore(Guid userId);

        /// <summary>
        /// 获取用户作答记录数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PaginationOutputDto<UserAnswerRecordsPaginationItem>> GetUserAnswerRecordsPagination(
            UserAnswerRecordsPaginationInput input);

        /// <summary>
        ///  获取用户作答详细
        /// </summary>
        /// <param name="recordsId"></param>
        /// <returns></returns>
        Task<UserAnswerRecordsQuestion> GetUserAnswerRecordsQuestion(Guid recordsId);
        /// <summary>
        /// 获取挑战题用户答题记录
        /// </summary>
        /// <param name="gradeId"></param>
        /// <returns></returns>
        Task<UserAnswerRecordsQuestion> GetRecordsQuestionByChallenge(Guid gradeId);
    }
}