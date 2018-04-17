using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using Abp.Application.Services;
using Abp.Runtime.Validation;
using SPOC.Common.Dto;
using SPOC.Common.Pagination;
using SPOC.Core.Dto.Challenge;
using SPOC.Core.Dto.CodeCompile;
using SPOC.Exam;
using SPOC.QuestionBank.Dto;

namespace SPOC.Core
{
    public interface IChallengeQuestionService:IApplicationService
    {
        /// <summary>
        /// 根据条件进行分页查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DisableValidation]
        Task<PaginationOutputDto<QuestionItem>> GetPagination(QuestionPaginationInputDto input);

        /// <summary>
        /// 创建一个新的试题
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ExamQuestionDto> Create(ChallengeQuestionInputDto input);

        /// <summary>
        /// 更新试题数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Update(ChallengeQuestionInputDto input);

        /// <summary>
        /// 删除试题
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost, HttpGet]
        Task<string> Delete(string ids);

        /// <summary>
        /// 根据id获取试题信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, HttpGet]
        Task<ChallengeOutDto> Get(string id);

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
        Task<List<ChallengeQuestion>> GetChildren(string parentUid);

        /// <summary>
        /// 获取分类下指定难度试题数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<int> GetQuestionNum(QuestionNumInputDto input);

        #region 导入导出


        /// <summary>
        /// 从上传的文件内容来批量创建试题
        /// </summary>
        /// <param name="fileStream">文件流</param>
        /// <param name="folderUid">分类</param>
        /// <param name="fileType">上传文件类型</param>
        /// <returns></returns>
        ImportResultOutputDto CreateFromFile(Stream fileStream,  Guid folderUid, string fileType);

        /// <summary>
        /// 获取导出到word的试题文本内容
        /// </summary>
        /// <param name="input">需要导出的试题ID</param>
        /// <returns></returns>
        Task<Guid> ExportToWord(IdListInputDto input);


        #endregion

        #region 前台用户界面API
        /// <summary>
        /// 获取挑战导航分类
        /// </summary>
        /// <returns></returns>
        Task<List<ChallengeFolderDto>> GetChallengeFolder();
        /// <summary>
        /// 获取我的挑战得分与排名
        /// </summary>
        /// <returns></returns>
        Task<PointsRankDto> GetPointsAndRank();

        /// <summary>
        /// 挑战页展示数据
        /// </summary>
        /// <returns></returns>
        Task<ChallengeQuestionViewModel> GetChallengeList(ChallengeInputDto input);
        /// <summary>
        /// 获取挑战问题详情
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        Task<ProblemViewDto> GetProblem(Guid questionId);
        /// <summary>
        /// 挑战编译运行
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ProblemCompileResultDto> CompileRun(ProblemInputDto input);
        /// <summary>
        /// 提交挑战
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ProblemSubmitResultDto> Submit(ProblemInputDto input);
        /// <summary>
        /// 获取排行分页数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<RankListViewDto> GetRankPagination(RankInputDto input);
        /// <summary>
        /// 获取挑战提交记录列表分页数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<SubmissionListViewDto> GetSubmissionPagination(RankInputDto input);

        #endregion

        #region  挑战排行榜

        /// <summary>
        /// 挑战排行榜单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DisableValidation]
        Task<ChallengeLeaderboardView> ChallengeLeaderboard(RankInputDto input);
        /// <summary>
        /// 获取挑战用户班级
        /// </summary>
        /// <returns></returns>
        Task<List<ChallengeClassSelect>> GetChallengeClassList(string className);
        /// <summary>
        /// 获取挑战用户列表
        /// </summary>
        /// <returns></returns>
        Task<List<SelectDto>> GetChallengeUserList(string userName);
        /// <summary>
        /// 获取用户挑战答题记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DisableValidation]
        Task<PaginationOutputDto<UserAnswerRecordDto>> GetUserAnswerRecord(UserAnswerInputDto input);
        #endregion


    }
}