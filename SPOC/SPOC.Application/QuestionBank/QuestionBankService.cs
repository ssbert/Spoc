using Abp.AutoMapper;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using Aspose.Words;
using Castle.Core.Logging;
using Newtonsoft.Json;
using SPOC.Category;
using SPOC.Common;
using SPOC.Common.Cookie;
using SPOC.Common.Dto;
using SPOC.Common.File;
using SPOC.Common.Helper;
using SPOC.Common.Http;
using SPOC.Common.Pagination;
using SPOC.Exam;
using SPOC.ExamPaper;
using SPOC.Exercises;
using SPOC.QuestionBank.Const;
using SPOC.QuestionBank.Dto;
using SPOC.User;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Json;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Abp.Threading;
using SPOC.Core;
using SPOC.Lib;
using SPOC.Lib.Dto;

namespace SPOC.QuestionBank
{
    /// <summary>
    /// 试卷题库服务
    /// </summary>
    public class QuestionBankService:SPOCAppServiceBase,IQuestionBankService
    {
        private readonly IRepository<ExamQuestion, Guid> _iExamQuestionRep;
        private readonly IRepository<ExamQuestionType, Guid> _iExamQuestionTypeRep;
        private readonly IRepository<NvFolder, Guid> _iNvFolderRep;
        private readonly IRepository<TeacherInfo, Guid> _iTeacherInfoRep;
        private readonly IRepository<ExamPaperNodeQuestion, Guid> _iExamPaperNodeQuestionRep;
        private readonly IRepository<Exercise, Guid> _iExerciseRep;
        private readonly IRepository<UserBase, Guid> _iUserRep;
        private readonly IRepository<QuestionStandardCode, Guid> _iQuestionStandardCodeRep;
        private readonly IRepository<QuestionLabel, Guid> _iQuestionLabelRep;
        private readonly IRepository<Label, Guid> _iLabelRep;
        private readonly IUnitOfWorkManager _iUnitOfWorkManager;
        private readonly IExamPaperService _iExamPaperService;
        private readonly ILogger _iLogger;
        private string NewMoocApiUrl => L("payUrl").TrimEnd('/') + "/api/";
        /// <summary>
        /// 构造函数
        /// </summary>
        public QuestionBankService(IRepository<ExamQuestionType, Guid> iExamQuestionTypeRepository,
            IRepository<ExamQuestion, Guid> iExamQuestionRepository,
            IRepository<NvFolder, Guid> iNvFolderRepository, 
            IRepository<TeacherInfo, Guid> iTeacherInfoRep,
            IUnitOfWorkManager iUnitOfWorkManager, IRepository<ExamPaperNodeQuestion, Guid> iExamPaperNodeQuestionRep, 
            IRepository<Exercise, Guid> iExerciseRep, IRepository<UserBase, Guid> iUserRep, IExamPaperService iExamPaperService, 
            IRepository<QuestionStandardCode, Guid> iQuestionStandardCodeRep, IRepository<QuestionLabel, Guid> iQuestionLabelRep, 
            IRepository<Label, Guid> iLabelRep)
        {
            _iExamQuestionRep = iExamQuestionRepository;
            _iExamQuestionTypeRep = iExamQuestionTypeRepository;
            _iNvFolderRep = iNvFolderRepository;
            _iTeacherInfoRep = iTeacherInfoRep;
            _iUnitOfWorkManager = iUnitOfWorkManager;
            _iExamPaperNodeQuestionRep = iExamPaperNodeQuestionRep;
            _iExerciseRep = iExerciseRep;
            _iUserRep = iUserRep;
            _iExamPaperService = iExamPaperService;
            _iQuestionStandardCodeRep = iQuestionStandardCodeRep;
            _iQuestionLabelRep = iQuestionLabelRep;
            _iLabelRep = iLabelRep;
            _iLogger = new NullLogger();
        }
        /// <summary>
        /// 根据条件获取分页数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PaginationOutputDto<QuestionItem>> GetPagination(QuestionPaginationInputDto input)
        {
            input.questionText = input.questionText.Trim();
            input.questionCode = input.questionCode.Trim();
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (cookie == null || !cookie.IsLogin)
            {
                throw new UserFriendlyException("登录已过期");
            }

            List<Guid> questionIdList = null;
            if (input.labelIdList.Any())
            {
                questionIdList = await _iQuestionLabelRep.GetAll().Where(a => input.labelIdList.Contains(a.labelId) && a.labelType == 1)
                    .Select(a => a.questionId).ToListAsync();
            }

            var queryables = _iExamQuestionRep.GetAll().AsNoTracking()
                .WhereIf(questionIdList != null, a => questionIdList.Contains(a.Id))
                .WhereIf(!string.IsNullOrEmpty(input.questionText), a => a.questionPureText.Contains(input.questionText))
                .WhereIf(!string.IsNullOrEmpty(input.questionCode), a => a.questionCode.Contains(input.questionCode))
                .WhereIf(!string.IsNullOrEmpty(input.questionBaseTypeCode),a => a.questionBaseTypeCode == input.questionBaseTypeCode)
                .WhereIf(input.folderUidList.Any(), a => input.folderUidList.Contains(a.folderUid))
                .WhereIf(!string.IsNullOrEmpty(input.questionStatusCode),a => a.questionStatusCode == input.questionStatusCode)
                .WhereIf(!string.IsNullOrEmpty(input.hardGrade), a => a.hardGrade == input.hardGrade)
                .WhereIf(!string.IsNullOrEmpty(input.operateTypeCode), a => a.operateTypeCode == QuestionTypeConst.Operate && a.operateTypeCode == input.operateTypeCode)
                .WhereIf(input.hasChild == "N", a => a.parentQuestionUid == Guid.Empty)
                .WhereIf(input.hasAnalysis == "Y", a => !string.IsNullOrEmpty(a.questionAnalysis))
                .WhereIf(input.hasAnalysis == "N", a => string.IsNullOrEmpty(a.questionAnalysis))
                .WhereIf(input.subjectUidList.Count > 0, a => input.subjectUidList.Contains(a.subjectUid))
                .WhereIf(input.departmentUidList.Count > 0, a => input.departmentUidList.Contains(a.departmentUid))
                //.WhereIf(!cookie.IsAdmin, a=>a.creatorUid == cookie.Id) //暂时屏蔽资源类权限 2018-02-28
                .GroupJoin(input.exceptionIdList, q=>q.Id, id=>id, (q, ids)=>new {question=q, ids})
                .Where(a=>!a.ids.Any())
                .Select(a=>a.question)
                .Join(_iExamQuestionTypeRep.GetAll().AsNoTracking(), q => q.questionTypeUid, qt => qt.Id,
                    (q, qt) => new { question = q, questionType = qt });

            

            var tempQueryable = from a in queryables
                                join u in _iUserRep.GetAll().AsNoTracking() on a.question.creatorUid equals u.Id
                                where (string.IsNullOrEmpty(input.userFullName) || u.userFullName.Contains(input.userFullName))
                                && (string.IsNullOrEmpty(input.userLoginName) || u.userLoginName.Contains(input.userLoginName))
                                select new QuestionItem()
            {
                Id = a.question.Id,
                questionTypeUid = a.question.questionTypeUid,
                questionCode = a.question.questionCode,
                questionTypeName = a.questionType.questionTypeName,
                parentQuestionUid = a.question.parentQuestionUid,
                questionBaseTypeCode = a.question.questionBaseTypeCode,
                questionText = a.question.questionText,
                questionPureText = a.question.questionPureText,
                hardGrade = a.question.hardGrade,
                score = a.question.score,
                outdatedDate = a.question.outdatedDate,
                examTime = a.question.examTime,
                questionStatusCode = a.question.questionStatusCode,
                listOrder = a.question.listOrder,
                createTime = a.question.createTime,
                creatorUid = a.question.creatorUid,
                userLoginName = u.userLoginName,
                userFullName = u.userFullName
            };

            if (!string.IsNullOrEmpty(input.sort))
            {
                tempQueryable = tempQueryable.OrderBy(input.sort + " " + input.order);
            }
            else
            {
                tempQueryable = tempQueryable.OrderByDescending(a => a.createTime);
            }
            var rows = tempQueryable.Skip(input.skip).Take(input.pageSize).ToList();
            rows.ForEach(a=>a.questionPureText = a.questionPureText.Replace("&nbsp;", " "));
            return await Task.FromResult(new PaginationOutputDto<QuestionItem>()
            {
                rows = rows,
                total = tempQueryable.Count()
            });
        }

        /// <summary>
        /// 创建一条新的试题数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ExamQuestionDto> Create(QuestionInputDto input)
        {
            //TODO:需要添加验证
            var cookie = CookieHelper.GetLoginInUserInfo();
            #region 验证
            if (cookie == null || cookie.Id == Guid.Empty)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }
            if (!cookie.IsAdmin && !_iTeacherInfoRep.GetAll().Any(a => a.userId == cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }
            if (!_iNvFolderRep.GetAll().Any(a => a.Id == input.folderUid))
            {
                throw new UserFriendlyException("无效的试题分类");
            }
            var questionType = _iExamQuestionTypeRep.GetAll().FirstOrDefault(a => a.questionBaseTypeCode == input.questionBaseTypeCode);
            if (questionType == null)
            {
                throw new UserFriendlyException("无效的试题题型");
            }
            if (input.parentQuestionUid != Guid.Empty && !_iExamQuestionRep.GetAll().Any(a => a.Id == input.parentQuestionUid && a.questionBaseTypeCode == QuestionTypeConst.Compose))
            {
                throw new UserFriendlyException("无效的父试题");
            }
            
            if (input.questionBaseTypeCode == QuestionTypeConst.Operate &&
                !OperateTypeConst.HasConstValue(input.operateTypeCode))
            {
                throw new UserFriendlyException("无效的操作类型");
            }

            if (input.isCustomCode)
            {
                if (string.IsNullOrEmpty(input.questionCode))
                {
                    throw new UserFriendlyException("自定义试题编号不能为空");
                }

                if (_iExamQuestionRep.GetAll().Any(a => a.questionCode == input.questionCode))
                {
                    throw new UserFriendlyException("已有相同的试题编号");
                }
            }

            #endregion

            if (!input.isCustomCode)
            {
                input.questionCode = CreateNewCode();
            }

            if (input.questionBaseTypeCode == "program" || input.questionBaseTypeCode == "program_fill")
            {
                input.language = L("Language");
            }

            var entity = input.MapTo<ExamQuestion>();

            entity.createTime = entity.modifyTime = DateTime.Now;
            entity.Id = Guid.NewGuid();
            entity.modifier = cookie.Id;
            entity.creatorUid = cookie.Id;

            entity.questionTypeUid = questionType.Id;
            entity.questionPureText = QuestionUtil.RemoveHtmlTag(entity.questionText).Replace("&nbsp;", " ");

            #region 图片处理
            var host = "http://" + HttpContext.Current.Request.Url.Authority;
            string rootPath = HttpContext.Current.Server.MapPath("~/");
            Func<string, string, string, string> changeImgPath = (path, content, id) =>
            {
                var reg = new Regex("<img.*?>");
                var reg2 = new Regex("src=\".*?\"");
                var matches = reg.Matches(content);
                foreach (Match match in matches)
                {
                    var value = match.Value.Replace(host, "");
                    content = content.Replace(match.Value, value);

                    var srcMatch = reg2.Match(value);
                    var src = srcMatch.Value.Substring(6, srcMatch.Value.Length - 7);
                    src = src.Replace("/", "\\");
                    var srcPath = rootPath + src;
                    if (File.Exists(srcPath))
                    {
                        var dirPath = Path.Combine(rootPath, "fileroot", "question", id);
                        if (!Directory.Exists(dirPath))
                        {
                            Directory.CreateDirectory(dirPath);
                        }
                        var index = srcPath.LastIndexOf("\\", StringComparison.Ordinal);
                        var fileName = srcPath.Substring(index + 1);
                        var targetPath = Path.Combine(dirPath, fileName);
                        if (!File.Exists(targetPath))
                        {
                            File.Copy(srcPath, targetPath);
                        }
                        content = content.Replace(srcMatch.Value, "src=\"./" + fileName + "\"");
                    }
                }
                return content;
            };
            var idStr = entity.Id.ToString();
            var fileRootPath = FilePathUtil.GetOppositeFileWebPathRoot(idStr, "question");
            entity.questionText = changeImgPath(fileRootPath, entity.questionText, idStr);
            entity.questionAnalysis = changeImgPath(fileRootPath, entity.questionAnalysis, idStr);
            entity.standardAnswer = changeImgPath(fileRootPath, entity.standardAnswer, idStr);
            entity.selectAnswer = changeImgPath(fileRootPath, entity.selectAnswer, idStr);
            #endregion
            //TODO:缺少创建者与修改者的赋值
            //UserSession loginUserSession=(UserSession) Session[Session.SessionID]

            await _iExamQuestionRep.InsertAsync(entity);

            if (input.parentQuestionUid != Guid.Empty)
            {
                var parentScoreQuery =
                    _iExamQuestionRep.GetAll()
                        .Where(a => a.parentQuestionUid == input.parentQuestionUid)
                        .Select(a => a.score);
                var parentScore = parentScoreQuery.Any() ? parentScoreQuery.Sum() : 0m;
                parentScore += input.score;
                await _iExamQuestionRep.UpdateAsync(input.parentQuestionUid, q =>
                {
                    q.score = parentScore;
                    return Task.FromResult(q);
                });
            }
            var dto = entity.MapTo<ExamQuestionDto>();
            dto.questionText = FilePathUtil.GetContentTextWithFilePath(idStr, "question", dto.questionText, false);
            dto.questionAnalysis = FilePathUtil.GetContentTextWithFilePath(idStr, "question", dto.questionAnalysis, false);
            dto.standardAnswer = FilePathUtil.GetContentTextWithFilePath(idStr, "question", dto.standardAnswer, false);
            dto.selectAnswer = FilePathUtil.GetContentTextWithFilePath(idStr, "question", dto.selectAnswer, false);
            var questionInput = new QuestionToCloudDto
            {
                ExamQuestion= entity,
                QuestionStandardCodes=new List<QuestionStandardCode>(),
                QuestionLabels = new List<QuestionLabel>()
            };
            //处理标准答案
            if (!string.IsNullOrWhiteSpace(input.standardCode))
            {
                var standardCode = new QuestionStandardCode
                {
                    Id = Guid.NewGuid(),
                    questionId = entity.Id,
                    code = input.standardCode,
                    isDefault = 1,
                    modifyTime = DateTime.Now,
                    type = "normal"
                };
                questionInput.QuestionStandardCodes.Add(standardCode);
                _iQuestionStandardCodeRep.Insert(standardCode);
            }
            //处理主标签信息
            if (input.label != null)
            {
                foreach (var label in input.label)
                {
                    var questionLabel = new QuestionLabel
                    {
                        Id = Guid.NewGuid(),
                        labelId = label,
                        labelType = 1,
                        questionId = entity.Id,
                        questionType = "normal"
                    };
                    questionInput.QuestionLabels.Add(questionLabel);
                    _iQuestionLabelRep.Insert(questionLabel);
                }

            }
            //处理辅标签信息
            if (input.seclabel != null)
            {
                foreach (var label in input.seclabel)
                {
                    var questionLabel = new QuestionLabel
                    {
                        Id = Guid.NewGuid(),
                        labelId = label,
                        labelType = 0,
                        questionId = entity.Id,
                        questionType = "normal"
                    };
                    questionInput.QuestionLabels.Add(questionLabel);
                    _iQuestionLabelRep.Insert(questionLabel);
                }

            }
            //if (entity.questionBaseTypeCode.Equals("program")|| entity.questionBaseTypeCode.Equals("program_fill"))
            //{
                var success = await CompileQuestionPublish(questionInput);
                if (!success)
                    throw new UserFriendlyException("编程题同步云端失败");
            //}
            dto.label = input.label;
            dto.secLabel = input.seclabel;
            return await Task.FromResult(dto);
        }

        /// <summary>
        /// 更新一条试题数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task Update(QuestionInputDto input)
        {
            //TODO:需要添加验证
            var cookie = CookieHelper.GetLoginInUserInfo();

            #region 验证
            if (cookie == null || cookie.Id == Guid.Empty)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }
            if (!cookie.IsAdmin && !_iTeacherInfoRep.GetAll().Any(a => a.userId == cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }
            if (!_iExamQuestionRep.GetAll().Any(a => a.Id == input.Id))
            {
                throw new UserFriendlyException("无效的试题");
            }

            if (!_iNvFolderRep.GetAll().Any(a => a.Id == input.folderUid))
            {
                throw new UserFriendlyException("无效的试题分类");
            }

            if (!_iExamQuestionTypeRep.GetAll()
                    .Any(a => a.Id == input.questionTypeUid && a.questionBaseTypeCode == input.questionBaseTypeCode))
            {
                throw new UserFriendlyException("无效的试题题型");
            }
            if (input.parentQuestionUid != Guid.Empty && !_iExamQuestionRep.GetAll().Any(a => a.Id == input.parentQuestionUid && a.questionBaseTypeCode == QuestionTypeConst.Compose))
            {
                throw new UserFriendlyException("无效的父试题");
            }

            if (input.questionBaseTypeCode == QuestionTypeConst.Operate &&
                !OperateTypeConst.HasConstValue(input.operateTypeCode))
            {
                throw new UserFriendlyException("无效的操作类型");
            }

            if (input.isCustomCode)
            {
                if (string.IsNullOrEmpty(input.questionCode))
                {
                    throw new UserFriendlyException("自定义试题编号不能为空");
                }

                if (_iExamQuestionRep.GetAll().Any(a => a.questionCode == input.questionCode && a.Id != input.Id))
                {
                    throw new UserFriendlyException("已有相同的试题编号");
                }
            }
            #endregion

            var entity = _iExamQuestionRep.Get(input.Id);
            var scoreChanded = input.score != entity.score;
            var score = scoreChanded ? input.score - entity.score : 0m;//计算以增加分数计算基准
            if (entity.isCustomCode && !input.isCustomCode)
            {
                input.questionCode = CreateNewCode();
            }
            try
            {
                input.MapTo(entity);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                throw;
            }
            
            entity.modifyTime = DateTime.Now;
            entity.questionPureText = QuestionUtil.RemoveHtmlTag(entity.questionText);
            #region 图片处理
            var host = "http://" + HttpContext.Current.Request.Url.Authority;
            string rootPath = HttpContext.Current.Server.MapPath("~/");
            Func<string, string, string, string> changeImgPath = (path, content, id) =>
            {
                var reg = new Regex("<img.*?>");
                var reg2 = new Regex("src=\".*?\"");
                var matches = reg.Matches(content);
                foreach (Match match in matches)
                {
                    var value = match.Value.Replace(host, "");
                    content = content.Replace(match.Value, value);

                    var srcMatch = reg2.Match(value);
                    var src = srcMatch.Value.Substring(6, srcMatch.Value.Length - 7);
                    src = src.Replace("/", "\\");
                    var srcPath = rootPath + src;
                    if (File.Exists(srcPath))
                    {
                        var dirPath = Path.Combine(rootPath, "fileroot", "question", id);
                        if (!Directory.Exists(dirPath))
                        {
                            Directory.CreateDirectory(dirPath);
                        }
                        var index = srcPath.LastIndexOf("\\", StringComparison.Ordinal);
                        var fileName = srcPath.Substring(index + 1);
                        var targetPath = Path.Combine(dirPath, fileName);
                        if (!File.Exists(targetPath))
                        {
                            File.Copy(srcPath, targetPath);
                        }
                        content = content.Replace(srcMatch.Value, "src=\"./" + fileName + "\"");
                    }
                }
                return content;
            };
            var idStr = entity.Id.ToString();
            var fileRootPath = FilePathUtil.GetOppositeFileWebPathRoot(idStr, "question");
            entity.questionText = changeImgPath(fileRootPath, entity.questionText, idStr);
            entity.questionAnalysis = changeImgPath(fileRootPath, entity.questionAnalysis, idStr);
            entity.standardAnswer = changeImgPath(fileRootPath, entity.standardAnswer, idStr);
            entity.selectAnswer = changeImgPath(fileRootPath, entity.selectAnswer, idStr);
            #endregion
            //TODO:缺少创建者与修改者的赋值
            //UserSession loginUserSession=(UserSession) Session[Session.SessionID]

            await _iExamQuestionRep.UpdateAsync(entity);

            if (scoreChanded && input.parentQuestionUid != Guid.Empty)
            {
                var parentScore =
                    _iExamQuestionRep.GetAll()
                        .Where(a => a.parentQuestionUid == input.parentQuestionUid)
                        .Select(a => a.score)
                        .Sum();
                parentScore += score;
                await _iExamQuestionRep.UpdateAsync(input.parentQuestionUid, q =>
                {
                    q.score = parentScore;
                    return Task.FromResult(q);
                });
            }
            var questionInput = new QuestionToCloudDto
            {
                ExamQuestion = entity,
                QuestionStandardCodes = _iQuestionStandardCodeRep.GetAllList(a => a.isDefault == 0 && a.questionId.Equals(entity.Id)),
                QuestionLabels = new List<QuestionLabel>()
            };
            //处理标准答案 
            _iQuestionStandardCodeRep.Delete(a => a.isDefault == 1 && a.questionId.Equals(entity.Id));
            if (!string.IsNullOrWhiteSpace(input.standardCode))
            {
                var standardCode = new QuestionStandardCode
                {
                    Id = Guid.NewGuid(),
                    questionId = entity.Id,
                    code = input.standardCode,
                    isDefault = 1,
                    modifyTime = DateTime.Now,
                    type = "normal"
                };
                questionInput.QuestionStandardCodes.Add(standardCode);
                _iQuestionStandardCodeRep.Insert(standardCode);
            }
            //处理主标签信息
            _iQuestionLabelRep.Delete(a => a.questionId.Equals(entity.Id));
            if (input.label != null)
            {
                foreach (var label in input.label)
                {
                    var questionLabel = new QuestionLabel
                    {
                        Id = Guid.NewGuid(),
                        labelId = label,
                        labelType = 1,
                        questionId = entity.Id,
                        questionType = "normal"
                    };
                    questionInput.QuestionLabels.Add(questionLabel);
                    _iQuestionLabelRep.Insert(questionLabel);
                }

            }
            //处理辅标签信息
            if (input.seclabel != null)
            {
                foreach (var label in input.seclabel)
                {
                    var questionLabel = new QuestionLabel
                    {
                        Id = Guid.NewGuid(),
                        labelId = label,
                        labelType = 0,
                        questionId = entity.Id,
                        questionType = "normal"
                    };
                    questionInput.QuestionLabels.Add(questionLabel);
                    _iQuestionLabelRep.Insert(questionLabel);
                }

            }
            //if (entity.questionBaseTypeCode.Equals("program") || entity.questionBaseTypeCode.Equals("program_fill"))
            //{
               var success=await  CompileQuestionPublish(questionInput);
               if(!success)
                   throw new UserFriendlyException("编程题云端存储保存失败");
           // }
            _iUnitOfWorkManager.Current.SaveChanges();
            var paperIdList = await _iExamPaperNodeQuestionRep.GetAll().Where(a => a.questionUid == input.Id)
                .Select(a => a.paperUid).Distinct().ToListAsync();
            if (paperIdList.Any())
            {
                foreach (var paperId in paperIdList)
                {
                   await _iExamPaperService.BuidExamPaper(paperId); 
                }
            }
           
        }
        /// <summary>
        /// 编译题发布新课云
        /// </summary>
        /// <returns></returns>
        private async Task<bool> CompileQuestionPublish(QuestionToCloudDto question)
        {

            var jsonValue = JsonValue.Parse(JsonConvert.SerializeObject(question));
            var apiContent = System.Web.HttpUtility.UrlEncode(jsonValue.ToString(),
                Encoding.UTF8);
            var url = NewMoocApiUrl + "/compile/question?sign=";
            var result = await HttpHelper.PostResponseSerializeData<ApiResponseResult<dynamic>>(url, apiContent);
            return result.IsSuccess;
        }

        /// <summary>
        /// 根据Id字符串删除对应的试题，id用逗号“,”分隔
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task Delete(string ids)
        {
            var idArray = ids.Split(',').Select(a=>new Guid(a)).ToArray();
            var entityList = new List<ExamQuestion>();
            var childList = new List<ExamQuestion>();
            var cookie = CookieHelper.GetLoginInUserInfo();
            //TODO:需要补完验证
            #region 验证
            if (cookie == null || cookie.Id == Guid.Empty)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }
            if (!cookie.IsAdmin && !_iTeacherInfoRep.GetAll().Any(a => a.userId == cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }
            foreach (var uid in idArray)
            {
                var entity = _iExamQuestionRep.Get(uid);
                if (entity == null)
                {
                    throw new UserFriendlyException("无效的试题");
                }

                entityList.Add(entity);

                if (entity.questionBaseTypeCode == QuestionTypeConst.Compose)
                {
                    var children = _iExamQuestionRep.GetAll().Where(a => a.parentQuestionUid == entity.Id).ToList();
                    childList.AddRange(children);
                }
            }
            if (_iExamPaperNodeQuestionRep.GetAll().Any(a => idArray.Contains(a.questionUid)))
            {
                throw new UserFriendlyException("已有试卷引用试题，不可进行删除");
            }
            if (_iExerciseRep.GetAll().Any(a => idArray.Contains(a.QuestionId)))
            {
                throw new UserFriendlyException("已有练习引用试题，不可进行删除"); 
            }
            #endregion

            var dic = new Dictionary<Guid, List<ExamQuestion>>();

            foreach (var examQuestion in childList)
            {
                List<ExamQuestion> list;
                if (dic.ContainsKey(examQuestion.parentQuestionUid))
                {
                    list = dic[examQuestion.parentQuestionUid];
                }
                else
                {
                    list = new List<ExamQuestion>();
                    dic.Add(examQuestion.parentQuestionUid, list);
                }
                list.Add(examQuestion);
                await _iExamQuestionRep.DeleteAsync(examQuestion);
            }

            foreach (var examQuestion in entityList)
            {
                await _iExamQuestionRep.DeleteAsync(examQuestion);
                _iQuestionStandardCodeRep.Delete(a => a.questionId.Equals(examQuestion.Id));
                //_iQuestionLabelRep.Delete(a => a.questionId.Equals(examQuestion.Id));
            }

            foreach (var kv in dic)
            {
                if (entityList.Any(a => a.Id == kv.Key))
                {
                    continue;
                }
                var score = kv.Value.Select(q => q.score).Sum();
                await _iExamQuestionRep.UpdateAsync(kv.Key, q =>
                {
                    q.score -= score;
                    return Task.FromResult(q);
                });
            }
        }

        /// <summary>
        /// 根据id获取试题信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ExamQuestionDto> Get(Guid id)
        {
            var entity = _iExamQuestionRep.Get(id);
            #region 验证
           
            if (entity == null)
            {
                throw new UserFriendlyException("无效的试题id");
            }
            #endregion
            var result = entity.MapTo<ExamQuestionDto>();
            var qsc = _iQuestionStandardCodeRep.FirstOrDefault(a => a.questionId.Equals(entity.Id) && a.isDefault==1);
            result.standardCode = qsc?.code;
            //标签信息
            var labels = _iQuestionLabelRep.GetAll().Where(l => l.questionId.Equals(entity.Id)).ToList();
            result.label = labels.Where(l => l.labelType == 1).Select(l => l.labelId).ToList();
            result.secLabel = labels.Where(l => l.labelType == 0).Select(l => l.labelId).ToList();
            return await Task.FromResult(result);
        }
        
        /// <summary>
        /// 根据id获取子试题
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PaginationOutputDto<QuestionItem>> GetChildPagination(QuestionPaginationInputDto input)
        {
            var entity = _iExamQuestionRep.Get(input.parentQuestionUid);
            #region 验证

            if (entity == null)
            {
                throw new UserFriendlyException("无效的试题id");
            }

            if (entity.questionBaseTypeCode != QuestionTypeConst.Compose)
            {
                throw new UserFriendlyException("不是组合题的id，不能获取子试题");
            }
            #endregion

            var rows = _iExamQuestionRep.GetAll().Where(a => a.parentQuestionUid == input.parentQuestionUid).OrderBy(a=>a.listOrder);
            return await Task.FromResult(new PaginationOutputDto<QuestionItem>()
            {
                rows = rows.Skip(input.skip).Take(input.pageSize).Select(a => new QuestionItem()
                {
                    Id = a.Id,
                    questionTypeUid = a.questionTypeUid,
                    questionCode = a.questionCode,
                    questionTypeName = a.QuestionType.questionTypeName,
                    parentQuestionUid = a.parentQuestionUid,
                    questionBaseTypeCode = a.questionBaseTypeCode,
                    questionText = a.questionText,
                    hardGrade = a.hardGrade,
                    score = a.score,
                    outdatedDate = a.outdatedDate, 
                    examTime = a.examTime,
                    questionStatusCode = a.questionStatusCode,
                    listOrder = a.listOrder
                }).ToList(),
                total = rows.Count()
            });
        }

        public async Task<List<ExamQuestion>> GetChildren(string parentUid)
        {
            var uid = new Guid(parentUid);
            var entity = _iExamQuestionRep.Get(uid);
            #region 验证

            if (entity == null)
            {
                throw new UserFriendlyException("无效的试题id");
            }

            if (entity.questionBaseTypeCode != QuestionTypeConst.Compose)
            {
                throw new UserFriendlyException("不是组合题的id，不能获取子试题");
            }
            #endregion

            var rows = _iExamQuestionRep.GetAll().Where(a => a.parentQuestionUid == uid).OrderBy(a => a.listOrder);

            return await Task.FromResult(rows.ToList());
        }

        public async Task<int> GetQuestionNum(QuestionNumInputDto input)
        {
            List<Guid> questionIdList = null;
            if (input.labelIdList.Any())
            {
                questionIdList = await _iQuestionLabelRep.GetAll().Where(a => input.labelIdList.Contains(a.labelId) && a.labelType == 1)
                    .Select(a => a.questionId).ToListAsync();
            }
            var count = _iExamQuestionRep.GetAll()
                .Where(a=>a.parentQuestionUid == Guid.Empty && a.questionStatusCode == input.questionStatusCode)
                .WhereIf(input.questionTypeUid != Guid.Empty, a=>a.questionTypeUid == input.questionTypeUid)
                .WhereIf(input.folderUids.Count > 0, a => input.folderUids.Contains(a.folderUid))
                .WhereIf(!string.IsNullOrEmpty(input.hardGrade), a => a.hardGrade == input.hardGrade)
                .WhereIf(questionIdList != null, a => questionIdList.Contains(a.Id))
                .Count();
            return await Task.FromResult(count);
        }

        public ImportResultOutputDto CreateFromFile(Stream fileStream, Guid folderUid, string fileType)
        {
            
            if (!_iNvFolderRep.GetAll().Any(a => a.Id == folderUid && a.folderTypeCode == "question_bank"))
            {
                throw new UserFriendlyException("无效的分类");
            }
            if (fileType != "word" && fileType != "excel")
            {
                throw new UserFriendlyException("无效的文件格式");
            }
            var result = new ImportResultOutputDto();
            try
            {
                var rootPath = AppConfiguration.WebServerFileRootPath;
                var helper = new ImportQuestionHelper(rootPath, folderUid, _iNvFolderRep, _iExamQuestionRep, _iExamQuestionTypeRep, _iUnitOfWorkManager, _iQuestionStandardCodeRep, _iLabelRep,_iQuestionLabelRep,NewMoocApiUrl, L("Language"));
                int successCount;
                string errMessage;
                if (fileType == "word")
                {
                    helper.ImportFromWord(fileStream, out successCount, out errMessage);
                }
                else
                {
                    helper.ImportFromExcel(fileStream, out successCount, out errMessage);
                }
                result = new ImportResultOutputDto
                {
                    successCount = successCount,
                    errMessage = errMessage
                };
                return result;
            }
            catch (Exception e)
            {
                var guidNum = Guid.NewGuid().GetHashCode();
                result.errMessage = "发生未知错误，请联系管理员，错误编码：["+guidNum +"]";
                _iLogger.Error("[" + guidNum + "]" + e);
            }

            return result;
        }

        public async Task<Guid> ExportToWord(IdListInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录或登录已过期");
            }

            var questions = await _iExamQuestionRep.GetAll().Where(a => input.idList.Contains(a.Id)).ToListAsync();
            
            var folders = await _iNvFolderRep.GetAll().Where(a => a.folderTypeCode == "question_bank").ToListAsync();
            var questionTypeDic = await _iExamQuestionTypeRep.GetAll().ToDictionaryAsync(a => a.Id, a => a);
            var reValue = ExamImportAndExportUtil.ConvertQuestionToText(questions.MapTo<List<ExamQuestionDto>>(), EnumExportTextType.Word,
                questionTypeDic, folders, _iExamQuestionRep,_iQuestionStandardCodeRep, _iQuestionLabelRep,_iLabelRep);
            var htmlStr = reValue.ReturnObject.ToString();
            htmlStr = StringUtil.ReplaceEnter2BrWhenNoHtml(htmlStr) + "<br>";

            var doc = new Document();
            var builder = new DocumentBuilder(doc);
            builder.InsertHtml(htmlStr);
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fileroot", "CacheFile", "ExportQuestion");
            FilePathUtil.CreateDirectoryIfNotExists(path);
            var dirInfo = new DirectoryInfo(path);
            var files = dirInfo.GetFiles();
            //删除超过1天的缓存文件
            foreach (var fileInfo in files)
            {
                if ((DateTime.Now - fileInfo.CreationTime).TotalHours > 24)
                {
                    fileInfo.Delete();
                }
            }

            var guid = Guid.NewGuid();
            path += "\\" + cookie.Id + "_" + guid + ".doc";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            doc.Save(path, SaveFormat.Doc);
            return guid;
        }

        private string CreateNewCode()
        {
            var code = "Q000001";
            var entity = _iExamQuestionRep.GetAll().Where(a => !a.isCustomCode).OrderByDescending(a => a.createTime).FirstOrDefault();
            if (entity != null)
            {
                code = entity.questionCode;
                do
                {
                    code = StringUtil.GetNextCodeByAuto(code);
                } while (_iExamQuestionRep.GetAll().Any(a=>a.questionCode == code));
            }
            return code;
        }

        
    }

}