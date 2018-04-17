using Abp.Application.Services;
using Abp.Runtime.Validation;
using SPOC.Exam.Dto;
using SPOC.ExamPaper.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace SPOC.Exam
{
    /// <summary>
    /// 考试服务接口
    /// </summary>
    public interface IExamExamService:IApplicationService
    {
        /// <summary>
        /// 获取单一考试
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, HttpGet]
        Task<ExamExamOutputDto> Get(Guid id);

        /// <summary>
        /// 添加考试
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ExamExamOutputDto> Create(ExamExamInputDto input);

        /// <summary>
        /// 更新考试
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Update(ExamExamInputDto input);

        /// <summary>
        /// 设置一个考试为正考
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, HttpGet]
        Task SetExamTypeCodeNormal(Guid id);

        /// <summary>
        /// 删除考试
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost, HttpGet]
        Task Delete(string ids);

        #region 前台考试
        /// <summary>
        /// 在线获取考试信息(当缓存文件不存在或者缓存文件异常时触发)
        /// </summary>
        /// <param name="examUid">考试系统编号</param>
        /// <returns></returns>
         [HttpPost, HttpGet]
        Task<ExamExam> GetExamInfo(Guid examUid);
         
        /// <summary>
        /// 获取用户考试配置信息
        /// </summary>
        /// <param name="examUid">考试ID</param>
        /// <param name="userUid">用户ID</param>
        /// <returns></returns>
        [DisableValidation]
        [HttpPost, HttpGet]
        Task<string> GetUserExamData(string examUid, string userUid);

        /// <summary>
        /// 初始化考试信息
        /// </summary>
        /// <param name="examUid">考试ID</param>
        /// <param name="userUid">用户ID</param>
        /// <returns></returns>
        [DisableValidation]
        [HttpPost, HttpGet]
        Task<string> InitAttendExam(string examUid, string userUid);

        #endregion

        #region 提交考试
        /// <summary>
        /// 保存考试到服务器(考试暂停)
        /// </summary>
        /// <param name="examGradeInputDto">考试答卷信息</param>
        /// <returns></returns>
        [HttpPost]
        Task<string> SaveAnswerToServer(ExamGradeInputDto examGradeInputDto);
        /// <summary>
        /// 提交考试
        /// </summary>
        /// <param name="examGradeInputDto"></param>
        /// <returns></returns>
        Task<string> SubmitPaper(ExamGradeInputDto examGradeInputDto);

        /// <summary>
        /// 考试查看答卷信息
        /// </summary>
        /// <param name="examGradeUid">答卷ID</param>
        /// <param name="filterType">过滤类型</param>
        /// <returns></returns>
        [DisableValidation]
        [HttpPost, HttpGet]
        Task<ExamPaperViewOutputDto> GetUserPaperView(string examGradeUid, string filterType);
        #endregion

        #region 新课云编译服务

        /// <summary>
        /// 拉取编译成绩
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="examId"></param>
        /// <returns></returns>
        Task CheckCompiled(Guid userId,Guid examId);

        /// <summary>
        /// 刷新有未更新考试成绩的考试
        /// </summary>
        /// <param name="examId"></param>
        /// <returns></returns>
        [HttpGet]
        Task RefreshExamGradeCompiled(Guid examId);

        /// <summary>
        /// 答卷是否可以查看
        /// </summary>
        /// <param name="examGradeId"></param>
        /// <returns></returns>
        Task<bool> CheckShowUserExamPreview(Guid examGradeId);

        #endregion
    }
}
