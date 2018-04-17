using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.Specifications;
using Abp.UI;
using Aspose.Words;
using newv.common;
using SPOC.Category;
using SPOC.Common.Cookie;
using SPOC.Common.Dto;
using SPOC.Common.File;
using SPOC.Common.Pagination;
using SPOC.Exam;
using SPOC.ExamPaper.Dto;
using SPOC.ExamPaper.Struct;
using SPOC.QuestionBank;
using SPOC.QuestionBank.Dto;
using SPOC.User;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using SPOC.Core;
using SPOC.Lib;
using ConvertUtil = SPOC.Common.Helper.ConvertUtil;
using DataTableExtensions = SPOC.Common.Extensions.DataTableExtensions;
using DateTimeUtil = SPOC.Exam.DateTimeUtil;
using ReturnValue = SPOC.Common.ReturnValue;
using StringUtil = SPOC.Common.Helper.StringUtil;

namespace SPOC.ExamPaper
{
    /// <summary>
    /// 试卷服务
    /// </summary>
    public class ExamPaperService:SPOCAppServiceBase, IExamPaperService
    {
        private readonly IRepository<Exam.ExamPaper, Guid> _iExamPaperRep;
        private readonly IRepository<ExamExam, Guid> _iExamExamRep;
        private readonly IRepository<TeacherInfo, Guid> _iTeacherInfoRep;
        private readonly IRepository<ExamPaperNode, Guid> _iExamPaperNodeRep;
        private readonly IRepository<ExamPaperNodeQuestion, Guid> _iExamPaperNodeQuestionRep;
        private readonly IRepository<ExamQuestion, Guid> _iExamQuestionRep;
        private readonly IRepository<ExamQuestionType, Guid> _iExamQuestionTypeRep;
        private readonly IRepository<ExamAnswer, Guid> _iExamAnswerRep;
        private readonly IRepository<ExamGrade, Guid> _iExamGradeRep;
        private readonly IRepository<ExamPolicy, Guid> _iExamPolicyRep;
        private readonly IRepository<ExamPolicyItem, Guid> _iExamPolicyItemRep;
        private readonly IRepository<ExamPolicyNode, Guid> _iExamPolicyNodeRep;
        private readonly IRepository<ExamUserAnswer, Guid> _iExamUserAnswerRep;
        private readonly IRepository<Exam.ExamJudge, Guid> _iExamJudgeRep;
        private readonly IRepository<UserBase, Guid> _iUserRep;
        private readonly IRepository<NvFolder, Guid> _iNvFolderRep;
        private readonly IRepository<ExamProgramResult, Guid> _iExamProgramResultRep;
        private readonly IRepository<QuestionStandardCode, Guid> _iQuestionStandardCodeRep;
        private readonly IRepository<Label, Guid> _iLabelRep;
        private readonly IRepository<QuestionLabel, Guid> _iQuestionLabelRep;
        private readonly IExamInfoChangeService _iExamInfoChangeService;
        private readonly IUnitOfWorkManager _iUnitOfWorkManager;
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExamPaperService(IRepository<Exam.ExamPaper, Guid> iExamPaperRep, IRepository<ExamExam, Guid> iExamExamRep,
            IRepository<TeacherInfo, Guid> iTeacherInfoRep, 
            IRepository<ExamPaperNode, Guid> iExamPaperNodeRep, IRepository<ExamPaperNodeQuestion, Guid> iExamPaperNodeQuestionRep, 
            IExamInfoChangeService iExamInfoChangeService, IRepository<ExamQuestion, Guid> iExamQuestionRep,
            IRepository<ExamQuestionType, Guid> iExamQuestionTypeRep, IRepository<ExamAnswer, Guid> iExamAnswerRep,
            IRepository<ExamGrade, Guid> iExamGradeRep, IRepository<ExamPolicy, Guid> iExamPolicyRep,
            IRepository<ExamPolicyItem, Guid> iExamPolicyItemRep, IRepository<ExamPolicyNode, Guid> iExamPolicyNodeRep,
            IRepository<ExamUserAnswer, Guid> iExamUserAnswerRep, IRepository<Exam.ExamJudge, Guid> iExamJudgeRep,
            IRepository<UserBase, Guid> iUserRep, IUnitOfWorkManager iUnitOfWorkManager, IRepository<NvFolder, Guid> iNvFolderRep, 
            IRepository<ExamProgramResult, Guid> iExamProgramResultRep, IRepository<QuestionStandardCode, Guid> iQuestionStandardCodeRep, IRepository<Label, Guid> iLabelRep, IRepository<QuestionLabel, Guid> iQuestionLabelRep)
        {
            _iExamPaperRep = iExamPaperRep;
            _iExamExamRep = iExamExamRep;
            _iTeacherInfoRep = iTeacherInfoRep;
            _iExamPaperNodeRep = iExamPaperNodeRep;
            _iExamPaperNodeQuestionRep = iExamPaperNodeQuestionRep;
            _iExamInfoChangeService = iExamInfoChangeService;
            _iExamQuestionRep = iExamQuestionRep;
            _iExamQuestionTypeRep = iExamQuestionTypeRep;
            _iExamAnswerRep = iExamAnswerRep;
            _iExamGradeRep = iExamGradeRep;
            _iExamPolicyRep = iExamPolicyRep;
            _iExamPolicyItemRep = iExamPolicyItemRep;
            _iExamPolicyNodeRep = iExamPolicyNodeRep;
            _iExamUserAnswerRep = iExamUserAnswerRep;
            _iExamJudgeRep = iExamJudgeRep;
            _iUserRep = iUserRep;
            _iUnitOfWorkManager = iUnitOfWorkManager;
            _iNvFolderRep = iNvFolderRep;
            _iExamProgramResultRep = iExamProgramResultRep;
            _iQuestionStandardCodeRep = iQuestionStandardCodeRep;
            _iLabelRep = iLabelRep;
            _iQuestionLabelRep = iQuestionLabelRep;
        }

        public async Task<PaginationOutputDto<ExamPaperPaginationItem>> GetPagination(ExamPaperPaginationInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (cookie == null || !cookie.IsLogin)
            {
                throw new UserFriendlyException("登录已过期");
            }

            var queryable = from p in _iExamPaperRep.GetAll()
                join u in _iUserRep.GetAll() on p.creatorUid equals u.Id
                where p.paperTypeCode != "fix_from_random"
                      && (string.IsNullOrEmpty(input.paperName) || p.paperName.Contains(input.paperName.Trim()))
                      && (string.IsNullOrEmpty(input.paperCode) || p.paperCode.Contains(input.paperCode.Trim()))
                      && (string.IsNullOrEmpty(input.paperTypeCode) || p.paperTypeCode == input.paperTypeCode)
                      && (!input.subjectUidList.Any() || input.subjectUidList.Contains(p.subjectUid))
                      && (!input.departmentUidList.Any() || input.departmentUidList.Contains(p.departmentUid))
                      && (!input.folderUidList.Any() || input.folderUidList.Contains(p.folderUid))
                      && (!input.checkOutDate || !p.outdatedDate.HasValue || p.outdatedDate.Value > DateTime.Now)
                      && (cookie.IsAdmin || p.creatorUid == cookie.Id)
                      && (string.IsNullOrEmpty(input.userFullName) || u.userFullName.Contains(input.userFullName.Trim()))
                      && (string.IsNullOrEmpty(input.userLoginName) || u.userLoginName.Contains(input.userLoginName.Trim()))
                orderby p.createTime descending
                select new
                {
                    paper = p,
                    u.userFullName,
                    u.userLoginName
                };
            var rows = new List<ExamPaperPaginationItem>();
            var list = await queryable.Skip(input.skip).Take(input.pageSize).ToListAsync();
            foreach (var obj in list)
            {
                var item = obj.paper.MapTo<ExamPaperPaginationItem>();
                item.userLoginName = obj.userLoginName;
                item.userFullName = obj.userFullName;
                rows.Add(item);
            }

            return new PaginationOutputDto<ExamPaperPaginationItem>()
            {
                rows = rows,
                total = queryable.Count()
            };
        }

        public async Task Delete(string ids)
        {
            #region 验证

            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }

            if (!cookie.IsAdmin && !_iTeacherInfoRep.GetAll().Any(a => a.userId == cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }

            var idArray = ids.Split(',').Select(a=>new Guid(a)).ToArray();
            var policyIds = new List<Guid>();
            foreach (var id in idArray)
            {
                var paperTypeCode =
                    _iExamPaperRep.GetAll()
                        .Where(a => a.Id == id)
                        .Select(a => a.paperTypeCode)
                        .FirstOrDefault();
                if (string.IsNullOrEmpty(paperTypeCode))
                {
                    throw new UserFriendlyException("无效的试卷");
                }
                
                if (paperTypeCode == "random")
                {
                    if (!_iExamPolicyRep.GetAll().Any(a => a.Id == id))
                    {
                        throw new UserFriendlyException("无效的试卷");
                    }
                    policyIds.Add(id);
                }

                if (_iExamExamRep.GetAll().Any(a => a.paperUid == id))
                {
                    throw new UserFriendlyException("试卷已被使用");
                }
            }


            #endregion

            foreach (var id in idArray)
            {
                await _iExamPaperRep.DeleteAsync(a => a.Id == id);
            }

            if (policyIds.Count > 0)
            {
                foreach (var id in policyIds)
                {
                    await _iExamPolicyRep.DeleteAsync(a => a.Id == id);
                }
            }
        }

        public async Task<ExamPaperDto> Get(Guid id)
        {
            var entity = _iExamPaperRep.Get(id);
            var dto = entity.MapTo<ExamPaperDto>();
            return await Task.FromResult(dto);
        }
        
        public async Task<ExamPaperDto> Create(ExamPaperInputDto input)
        {
            #region 验证
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }

            if (!cookie.IsAdmin && !_iTeacherInfoRep.GetAll().Any(a => a.userId == cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }

            if (input.isCustomCode)
            {
                if (string.IsNullOrEmpty(input.paperCode))
                {
                    throw new UserFriendlyException("试卷编码不可为空");
                }

                if (_iExamPaperRep.GetAll().Any(a => a.paperCode == input.paperCode))
                {
                    throw new UserFriendlyException("已有相同的试卷编码");
                }
            }
            #endregion

            if (!input.isCustomCode)
            {
                input.paperCode = CreateNewCode();
            }
            input.Id = Guid.NewGuid();
            var entity = input.MapTo<Exam.ExamPaper>();
            entity.createTime = DateTime.Now;
            entity.lastUpdateTime = entity.createTime;
            entity.creatorUid = cookie.Id;
            _iExamPaperRep.Insert(entity);
            _iUnitOfWorkManager.Current.SaveChanges();
            await BuidExamPaper(input.Id);
            return await Task.FromResult(entity.MapTo<ExamPaperDto>());
        }

        public async Task Update(ExamPaperInputDto input)
        {
            #region 验证

            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }

            if (!cookie.IsAdmin && !_iTeacherInfoRep.GetAll().Any(a => a.userId == cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }

            if (!_iExamPaperRep.GetAll().Any(a => a.Id == input.Id))
            {
                throw new UserFriendlyException("无效的试卷");
            }

            if (input.isCustomCode)
            {
                if (string.IsNullOrEmpty(input.paperCode))
                {
                    throw new UserFriendlyException("试卷编码不可为空");
                }

                if (_iExamPaperRep.GetAll().Any(a => a.paperCode == input.paperCode && a.Id != input.Id))
                {
                    throw new UserFriendlyException("已有相同的试卷编码");
                }
            }
            #endregion

            var entity = _iExamPaperRep.Get(input.Id);
            if (entity.isCustomCode && !input.isCustomCode)
            {
                input.paperCode = CreateNewCode();
            }
            input.MapTo(entity);
            entity.lastUpdateTime = DateTime.Now;

            _iExamPaperRep.Update(entity);
            _iUnitOfWorkManager.Current.SaveChanges();

            await BuidExamPaper(input.Id);
        }

        public async Task BuidExamPaper(Guid id)
        {
            var msg = "";
            try
            {
                msg = await UpdatePaperXml(id.ToString());
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                throw;
            }
            
            if (!string.IsNullOrEmpty(msg))
            {
                throw new UserFriendlyException(msg);
            }
        }

        public async Task UpdateTotalScoreAndQuestionNum(Guid id, decimal score, int questionNum)
        {
            #region 验证

            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }

            if (!cookie.IsAdmin && !_iTeacherInfoRep.GetAll().Any(a => a.userId == cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }

            var entity = _iExamPaperRep.Get(id);
            if (entity == null)
            {
                throw new UserFriendlyException("无效的试卷");
            }
            #endregion

            entity.totalScore += score;
            entity.questionNum += questionNum;
            entity.lastUpdateTime = DateTime.Now;
            await _iExamPaperRep.UpdateAsync(entity);
        }

        public async Task<PaperPreviewOutputDto> GetPaperPreview(PaperPreviewInputDto input)
        {
            var output = new PaperPreviewOutputDto();
            var viewHtml = "";
            var paper = new Exam.ExamPaper();
            
            if (input.paperUid != Guid.Empty)
            {
                viewHtml = GetPaperView(input.paperUid, out paper);
            }
            else if (input.policyUid != Guid.Empty && input.policyNodeUid == Guid.Empty)
            {
                viewHtml = GetPaperViewFromPolicy(input.policyUid, "", out paper);
                output.isPolicy = true;
            }
            else if (input.examUid != Guid.Empty)
            {
                var exam = _iExamExamRep.GetAll().FirstOrDefault(a=>a.Id == input.examUid);
                if (exam == null)
                {
                    viewHtml = "";
                }
                else
                {
                    if (exam.paperTypeCode == "fix")
                    {
                        viewHtml = GetPaperView(exam.paperUid, out paper);
                    }
                    else
                    {
                        viewHtml = GetPaperViewFromPolicy(exam.paperUid, "", out paper);
                        output.isPolicy = true;
                    }
                }
            }
            else if (input.policyNodeUid != Guid.Empty)
            {
                var items = _iExamPolicyItemRep.GetAll().Where(a => a.policyNodeUid == input.policyNodeUid).ToList();
                var policyItems = string.Empty;
                items.ForEach(item =>
                {
                    policyItems += item.Id + ",";
                });

                viewHtml = GetPaperViewFromPolicy(input.policyUid, policyItems.TrimEnd(','), out paper);
                output.isPolicy = true;
            }
            output.paperId = paper.Id.ToString();
            output.viewHtml = viewHtml;
            if (paper != null)
            {
                output.title = paper.paperName;
                output.subTitle = string.Format("总共{0}题共{1}分", paper.questionNum, paper.totalScore.ToString("0.##"));
                output.code = paper.paperCode;
            }
            return await Task.FromResult(output);
        }

        public async Task<UserPaperViewOutputDto> GetUserPaperView(Guid examGradeUid, string filterType, bool judge = false)
        {
            var examGrade = _iExamGradeRep.GetAll().FirstOrDefault(a => a.Id == examGradeUid);
            var outputDto = new UserPaperViewOutputDto();
            if (examGrade == null)
            {
                outputDto.viewHtml = "找不到成绩信息!";
                return await Task.FromResult(outputDto);
            }
            var examExam = _iExamExamRep.GetAll().FirstOrDefault(a => a.Id.Equals(examGrade.examUid));
            var paperUid = examGrade.paperUid;
            var paper = _iExamPaperRep.GetAll().FirstOrDefault(a => a.Id == paperUid);
            //得到考试编号
            var user = _iUserRep.GetAll().FirstOrDefault(a => a.Id.Equals(examGrade.userUid));
            var examUserName = user != null ? user.userFullName : "";
            var examBeginTime = examGrade.beginTime;
            var examEndTime = examGrade.endTime;
            var examTime = examGrade.examTime ?? 0;

            var currentDateTime = DateTime.Now;
            string publishGradeDate = examExam.publishGradeDate ?? "";

            //允许查看成绩天数
            int allowSeeGradeDays = examExam.allowSeeGradeDays ?? 0;
            bool isAllowSeeGradeDays = true;
            if (allowSeeGradeDays > 0)
            {

                var lastUpdateTime = DateTimeUtil.ConvertToDataStr(examGrade.lastUpdateTime);
                string allowSeeGradeTime = DateTimeUtil.AddDays(lastUpdateTime, allowSeeGradeDays);
                if (DateTimeUtil.SecondsAfter(allowSeeGradeTime, DateTime.Now.ToString()) > 0) isAllowSeeGradeDays = false;
            }
            var examTotalScore = (examGrade.gradeScore ?? 0).ToString("0.##");
            //if (examExam.isAllowSeeGrade == "Y" && (publishGradeDate == "" || DateTimeUtil.ToDateTime(publishGradeDate) < currentDateTime) && isAllowSeeGradeDays == true)
            //{
            //    examTotalScore = (examGrade.gradeScore ?? 0).ToString("0.##");
            //}


            //考试设置时的评卷人与查看试卷结果的评卷人保持一致
            var examJudge = from j in _iExamJudgeRep.GetAll()
                            join u in _iUserRep.GetAll() on j.ownerUid equals u.Id
                            where j.examUid == examGrade.examUid
                            select u.userLoginName;
            string judgeUserName = string.Empty;
            if (examJudge != null && examJudge.Count() > 0)
            {
                judgeUserName = examJudge.ToList()[0];
            }

            var judgeRealName = string.IsNullOrEmpty(judgeUserName) ? ("系统自动评卷") : judgeUserName;
            //var judgeRealName = string.IsNullOrEmpty(examGrade.judge_user_name) ? ("系统自动评卷") : examGrade.judge_user_name;


            var judgeBeginTime = examGrade.judgeBeginTime ?? examGrade.lastUpdateTime;
            //var inputDto = new UserPaperViewInputDto
            //{
            //    paper = paper,
            //    exam = examExam,
            //    examGrade = examGrade,
            //    viewType = "",
            //    filterType = filterType
            //};
            outputDto.gradeUid = examGrade.Id;
            outputDto.examCode = examExam.ExamCode;
            outputDto.examUserName = examUserName;
            outputDto.examBeginTime = examBeginTime;
            if (examEndTime != null)
            {
                outputDto.examEndTime = examEndTime.Value;
            }
            outputDto.examTime = examTime;
            outputDto.judgeRealName = judgeRealName;
            outputDto.judgeBeginTime = judgeBeginTime;
            outputDto.examTotalScore = examTotalScore;
            outputDto.viewHtml = GetUserPaperView(paper, examExam, examGrade, "", filterType, judge);
            outputDto.title = paper == null ? "" : paper.paperName;
            outputDto.subTitle = paper == null
                ? ""
                : string.Format(("总共{0}题共{1}分"), paper.questionNum, paper.totalScore.ToString("0.##"));
            return outputDto;
        }

        public async Task<PaperPreviewOutputDto> GetExamPreview(Guid examId)
        {
            var inputDto = new PaperPreviewInputDto { examUid = examId };
            return await Task.FromResult(await GetPaperPreview(inputDto));
        }

        public ImportResultOutputDto CreateFromFile(Stream fileStream, Guid questionFolderUid, Guid paperFolderUid)
        {
            
            if (!_iNvFolderRep.GetAll().Any(a => a.Id == questionFolderUid && a.folderTypeCode == "question_bank") ||
                !_iNvFolderRep.GetAll().Any(a=>a.Id == paperFolderUid && a.folderTypeCode == "exam_paper"))
            {
                throw new UserFriendlyException("无效的分类");
            }

            var result = new ImportResultOutputDto();

            try
            {
                var rootPath = AppConfiguration.WebServerFileRootPath;
                var helper = new ImportExamPaperHelper(rootPath, questionFolderUid, paperFolderUid, _iNvFolderRep, 
                    _iExamQuestionTypeRep, _iUnitOfWorkManager, _iExamPaperRep, _iExamPaperNodeRep, _iExamPaperNodeQuestionRep, _iExamQuestionRep, _iQuestionStandardCodeRep, _iLabelRep, _iQuestionLabelRep, L("payUrl").TrimEnd('/') + "/api/", L("Language"));
                string errMessage;
                helper.ImportFormWord(fileStream, out errMessage);
                result.successCount = 1;
                result.errMessage = errMessage;
            }
            catch (Exception e)
            {
                var guidNum = Guid.NewGuid().GetHashCode();
                result.errMessage = "发生未知错误，请联系管理员，错误编码：[" + guidNum + "]";
                Logger.Error("[" + guidNum + "]" + e.ToString()); 
            }
            return result;
        }

        public async Task ExportToWord(Guid id)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            #region 验证
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }

            if (!cookie.IsAdmin && !_iTeacherInfoRep.GetAll().Any(a => a.userId == cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }

            var paper = await _iExamPaperRep.FirstOrDefaultAsync(id);
            if (paper == null)
            {
                throw new UserFriendlyException("无效的试卷");
            }
            #endregion

            ReturnValue reValue = null;
            
            if (paper.paperTypeCode == "fix")
            {
                var nodes = await _iExamPaperNodeRep.GetAllListAsync(a => a.paperUid == id);
                var nodeQuestions = await _iExamPaperNodeQuestionRep.GetAllListAsync(a => a.paperUid == id);
                var idList = nodeQuestions.Select(a => a.questionUid);
                var questions = (await _iExamQuestionRep.GetAllListAsync(a => idList.Contains(a.Id))).MapTo<List<ExamQuestionDto>>();
                var questionTypeDic = await _iExamQuestionTypeRep.GetAll().ToDictionaryAsync(a => a.Id, a => a);
                var folders = await _iNvFolderRep.GetAllListAsync(a => a.folderTypeCode == "question_bank");
                reValue = ExamImportAndExportUtil.ConvertPaperObjectToText(paper, nodes, nodeQuestions, questions, questionTypeDic,
                    folders, _iExamQuestionRep, _iQuestionStandardCodeRep,_iQuestionLabelRep,_iLabelRep);
            }
            else
            {
                //TODO:补充随机试卷
            }
            if (reValue == null)
            {
                throw new UserFriendlyException("导出试卷失败");
            }

            var htmlStr = reValue.ReturnObject.ToString();
            htmlStr = StringUtil.ReplaceEnter2BrWhenNoHtml(htmlStr) + "<br>";

            var doc = new Document();
            var builder = new DocumentBuilder(doc);
            builder.InsertHtml(htmlStr);
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fileroot", "CacheFile", "ExportPaper");
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

            path += "\\" + cookie.Id + "_" + id + ".doc";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            doc.Save(path, SaveFormat.Doc);
        }

        public async Task<string> ExportPaperPreviewHtml(Guid id)
        {
            var sbHtml = new StringBuilder();
            var paper = _iExamPaperRep.FirstOrDefault(p => p.Id == id);
            if (paper == null)
            {
                throw new UserFriendlyException("无效的试卷");
            }
            var paperNodeList = _iExamPaperNodeRep.GetAll().Where(p => p.paperUid == id).OrderBy(a=>a.listOrder).ToList();
            var paperQuestionDic = _iExamPaperNodeQuestionRep.GetAll()
                 .Include(nodeQuestion => nodeQuestion.Question)
                .Where(nodeQuestion => nodeQuestion.paperUid == id)
                .ToDictionary(nodeQuestion => nodeQuestion, nodeQuestion => nodeQuestion.Question);
            sbHtml.Append("<body>");
            sbHtml.Append("<h3 align='center'>" + paper.paperName + "</h3>");
            sbHtml.Append("<p align='right'>总共" + paper.questionNum + "题共" + paper.totalScore + "分</p>");
            sbHtml.Append("<br><br><br>");
            var nodeIndex = 1;
            foreach (var node in paperNodeList)
            {
                var sbReturn = new StringBuilder();
                
                #region 大题
                var strTitle = "<b><font size='5'>";
                strTitle += StringUtil.NumberToBigNumber(nodeIndex) + "、" + node.paperNodeName + " ";
                strTitle += "（共" + node.questionNum + "题";
                if (node.questionScore != 0)
                {
                    strTitle += "," + string.Format("每题{0}分", node.questionScore.ToString("0.##"));
                }
                if (node.totalScore != 0)
                {
                    strTitle += "," + string.Format("共{0}分", node.totalScore.ToString("0.##"));
                }
                strTitle += "）";
                strTitle += "</font></b><br>";

                //大题标题与标题描述
                if (string.IsNullOrEmpty(node.paperNodeDesc))
                {
                    strTitle += "<br>" + node.paperNodeDesc + "<br>";
                }

                sbHtml.Append(strTitle);
                #endregion

                #region 小题
                var thisPaperNode = "<font size=3>"+GetPaperQuestionContentWord(node, paperQuestionDic)+"</font>";
                sbReturn.Append(thisPaperNode);
                sbHtml.Append(sbReturn.ToString().Replace("$Enter$", "<br>").Replace("\r\n", "<br>"));
                #endregion

                //大题结束加两个空行
                sbHtml.Append("<br><br>");
                nodeIndex ++;
            }
            sbHtml.Append("</body>");
            return await Task.FromResult(sbHtml.ToString());
        }

        private string GetPaperQuestionContentWord(ExamPaperNode node, Dictionary<ExamPaperNodeQuestion, ExamQuestion> dic)
        {
            var sbReturn = new StringBuilder();
            var stringBuilder = new StringBuilder();
            var nodeQuestionList = dic.Keys.Where(a => a.paperNodeUid == node.Id).OrderBy(a => a.listOrder).ToList();
            var questionIndex = 1;//试题序号
            foreach (var nodeQuestion in nodeQuestionList)
            {
                var question = dic.Values.FirstOrDefault(a => a.Id == nodeQuestion.questionUid);
                //略过子试题
                if (question == null || question.parentQuestionUid != Guid.Empty)
                {
                    continue;
                }
                stringBuilder.Append(GetPaperQuestionViewWord(nodeQuestion, question, questionIndex));
                #region 如果是组合题则还要显示子试题

                if (question.questionBaseTypeCode == EnumQuestionBaseTypeCode.Compose)
                {
                    var subExamQuestionList = dic.Values.Where(a=>a.parentQuestionUid == question.Id).OrderBy(a=>a.listOrder).ToList();
                    var childQuestionIndex = 1;
                    foreach (var childQuestion in subExamQuestionList)
                    {
                        var childNodeQuestion = dic.Keys.FirstOrDefault(a => a.paperUid == node.paperUid && a.questionUid == childQuestion.Id);
                        if (childNodeQuestion == null)
                        {
                            continue;
                        }
                        stringBuilder.Append(GetPaperQuestionViewWord(childNodeQuestion, childQuestion, childQuestionIndex));
                        if (childQuestion != subExamQuestionList.Last())
                        {
                            stringBuilder.Append("$Enter$"); //每个组合题的小题后加空行
                        }
                        childQuestionIndex++;
                    }
                }
                #endregion
                stringBuilder.Append("$Enter$");
                questionIndex++;
            }
            sbReturn.Append(stringBuilder);
            return sbReturn.ToString();
        }

        private string GetPaperQuestionViewWord(ExamPaperNodeQuestion nodeQuestion, ExamQuestion question, int questionIndex)
        {
            var stringBuilder = new StringBuilder();
            int[] arrOriginalSelectAnswerIndex = new int[0];
            //显示试题
            var strAnswer = GetQuestionViewWord(nodeQuestion, question, questionIndex, ref arrOriginalSelectAnswerIndex);
            stringBuilder.Append(strAnswer);
            //显示答案
            //strAnswer = GetQuestionAnswerWord(question, arrOriginalSelectAnswerIndex);
            //stringBuilder.Append(strAnswer);
            return stringBuilder.ToString();
        }

        private string GetQuestionViewWord(ExamPaperNodeQuestion nodeQuestion, ExamQuestion question, int questionIndex, ref int[] arrOriginalSelectAnswerIndex)
        {
            var questionUid = question.Id;
            var parentQuestionUid = question.parentQuestionUid;
            var questionBaseTypeCode = question.questionBaseTypeCode;
            var content = question.questionText;
            var score = nodeQuestion.paperQuestionScore;
            var selectAnswer = question.selectAnswer;
            var stringBuilder = new StringBuilder();
            if (question.questionBaseTypeCode == EnumQuestionBaseTypeCode.Fill)
            {
                var regex1 = new Regex("[_]{3,}", RegexOptions.IgnoreCase);
                var match = regex1.Match(content);
                var sAnswers = question.standardAnswer.Split('|');
                var answerIndex = 0;
                while (true)
                {
                    if (!match.Success)
                    {
                        break;
                    }
                    if (sAnswers.Length >= answerIndex)
                    {
                        var sAnswerLength = 5;
                        if (sAnswers[answerIndex].IndexOf('&') > 0)
                        {
                            var qSAnswer = sAnswers[answerIndex].Split('&');
                            sAnswerLength = qSAnswer[0].Length;
                            for (var q = 1; q < qSAnswer.Length; q++)
                            {
                                if (qSAnswer[q].Length > sAnswerLength)
                                    sAnswerLength = qSAnswer[q].Length;
                            }
                        }
                        else
                        {
                            sAnswerLength = sAnswers[answerIndex].Length;
                        }

                        var strSpace = StringUtil.RepeatChar("{$space}", sAnswerLength * 4);
                        content = regex1.Replace(content, strSpace, 1, match.Index);
                        match = regex1.Match(content);
                    }
                    answerIndex++;
                }
                content = content.Replace("{$space}", "_");
            }
            //现固定从题库中取图片之类路径
            content = FilePathUtil.GetContentTextWithFilePath(questionUid.ToString(), "question", content);
            //现固定从题库中取图片之类路径
            selectAnswer = FilePathUtil.GetContentTextWithFilePath(questionUid.ToString(), "question", selectAnswer);

            if (questionBaseTypeCode == EnumQuestionBaseTypeCode.Typing)
            {
                content = content.Replace("<", "");
                content = content.Replace(">", "");
                content = content.Replace("$Enter$", " ");

                selectAnswer = selectAnswer.Replace("<", "");
                selectAnswer = selectAnswer.Replace(">", "");
                selectAnswer = selectAnswer.Replace("$Enter$", "<br>");
            }
            else
            {
                content = StringUtil.ReplaceSpaces2Html(content, true);
                selectAnswer = StringUtil.ReplaceSpaces2Html(selectAnswer, true);
            }

            //开始写试题内容
            //stringBuilder.Append("\n");
            //试题号
            var sContentIndex = "" + questionIndex;
            if (parentQuestionUid == Guid.Empty)//用于处理组合题子试题
            {
                sContentIndex = sContentIndex + ".";
            }
            else
            {
                sContentIndex = "(" + questionIndex + ")";
            }
            

            if (parentQuestionUid == Guid.Empty)//用于处理组合题子试题
            {
                stringBuilder.Append(sContentIndex + " ");
            }
            else
            {
                stringBuilder.Append(" " + sContentIndex + " ");
            }

            //如果不是打字题或不显示输入框时都要显示题目
            if (questionBaseTypeCode != EnumQuestionBaseTypeCode.Typing)
                stringBuilder.Append(content);

            //Nick2007/9/4加上可以不显示分数的功能(在试卷属性中定义)
            if (score > 0) stringBuilder.Append(" （" + score.ToString("0.##")  + "分）");

            //显示可选答案
            string[] arrSelectAnswer;
            char[] spliter = {'|'};
            string upChar;

            int maxOneSelectAnswerLength;	//一个可选答案长度
            int selectNumberOnOneRow;		//一行显示可选答案个数
            int oneSelectAnswerLength;		//一个可选答案显示长度

            arrOriginalSelectAnswerIndex = null;
            switch (questionBaseTypeCode)
            {
                case EnumQuestionBaseTypeCode.Single:
                case EnumQuestionBaseTypeCode.EvaluationSingle:
                    arrSelectAnswer = selectAnswer.Split(spliter);
                    arrOriginalSelectAnswerIndex = ArrayUtil.GetOrderValueIntArray(arrSelectAnswer.Length);

                    //计算总共长度,如果长度大于某一长度则写成一行，否则写成每行2个,4个,6个
                    GetStringRealLengthWord(selectAnswer);
                    maxOneSelectAnswerLength = 0;

                    for (int j = 0; j < arrSelectAnswer.Length; j++)
                    {
                        oneSelectAnswerLength = GetStringRealLengthWord(arrSelectAnswer[j]);
                        if (maxOneSelectAnswerLength < oneSelectAnswerLength) maxOneSelectAnswerLength = oneSelectAnswerLength;
                    }
                    selectNumberOnOneRow = 1;
                    oneSelectAnswerLength = 0;
                    if (maxOneSelectAnswerLength < 11)
                    {
                        selectNumberOnOneRow = 4;
                        oneSelectAnswerLength = 14;
                    }
                    else if (maxOneSelectAnswerLength < 25)
                    {
                        selectNumberOnOneRow = 2;
                        oneSelectAnswerLength = 28;
                    }

                    for (int j = 0; j < arrSelectAnswer.Length; j++)
                    {
                        if (j / selectNumberOnOneRow == j * 1.0 / selectNumberOnOneRow)
                            stringBuilder.Append("$Enter$");

                        upChar = ((Char)(65 + j)).ToString();

                        if (oneSelectAnswerLength > 0)
                            stringBuilder.Append(upChar + "." + FormatMixLengthStringWord(arrSelectAnswer[j], oneSelectAnswerLength));
                        else
                            stringBuilder.Append(upChar + "." + arrSelectAnswer[j]);

                    }
                    stringBuilder.Append("$Enter$<span style='font-size:2.0pt'>.</span>");
                    //如果不打乱顺序，则返回的顺序列表为空，这样可以避免多余操作
                    arrOriginalSelectAnswerIndex = null;
                    break;
                case EnumQuestionBaseTypeCode.Multi:
                case EnumQuestionBaseTypeCode.EvaluationMulti:
                    //case EnumQuestionBaseTypeCode.Scoredown:
                    arrSelectAnswer = selectAnswer.Split(spliter);

                    arrOriginalSelectAnswerIndex = ArrayUtil.GetOrderValueIntArray(arrSelectAnswer.Length);

                    //计算总共长度,如果长度大于某一长度则写成一行，否则写成每行2个,4个,6个
                    GetStringRealLengthWord(selectAnswer);
                    maxOneSelectAnswerLength = 0;

                    for (int j = 0; j < arrSelectAnswer.Length; j++)
                    {
                        oneSelectAnswerLength = GetStringRealLengthWord(arrSelectAnswer[j]);
                        if (maxOneSelectAnswerLength < oneSelectAnswerLength) maxOneSelectAnswerLength = oneSelectAnswerLength;
                    }
                    selectNumberOnOneRow = 1;
                    oneSelectAnswerLength = 0;
                    if (maxOneSelectAnswerLength < 11)
                    {
                        selectNumberOnOneRow = 4;
                        oneSelectAnswerLength = 14;
                    }
                    else if (maxOneSelectAnswerLength < 25)
                    {
                        selectNumberOnOneRow = 2;
                        oneSelectAnswerLength = 28;
                    }

                    for (int j = 0; j < arrSelectAnswer.Length; j++)
                    {
                        if (j / selectNumberOnOneRow == j * 1.0 / selectNumberOnOneRow)
                            stringBuilder.Append("$Enter$");

                        upChar = ((Char)(65 + j)).ToString();

                        if (oneSelectAnswerLength > 0)
                            stringBuilder.Append(upChar + "." + FormatMixLengthStringWord(arrSelectAnswer[j], oneSelectAnswerLength));
                        else
                            stringBuilder.Append(upChar + "." + arrSelectAnswer[j]);
                    }
                    stringBuilder.Append("$Enter$<span style='font-size:2.0pt'>.</span>");
                    //如果不打乱顺序，则返回的顺序列表为空，这样可以避免多余操作
                    arrOriginalSelectAnswerIndex = null;
                    break;
                case EnumQuestionBaseTypeCode.Judge:
                    stringBuilder.Append("\t(&nbsp;&nbsp;&nbsp;&nbsp;)");
                    stringBuilder.Append("$Enter$<span style='font-size:2.0pt'>.</span>");
                    //如果不打乱顺序，则返回的顺序列表为空，这样可以避免多余操作
                    arrOriginalSelectAnswerIndex = null;
                    break;
                case EnumQuestionBaseTypeCode.JudgeCorrect:     //2008-4-18 Nick 还不确定

                    stringBuilder.Append("\t(&nbsp;&nbsp;&nbsp;&nbsp;)");
                    stringBuilder.Append("<br>" + "如果错误，请改正:");
                    stringBuilder.Append("$Enter$<span style='font-size:2.0pt'>.</span>");
                    //如果不打乱顺序，则返回的顺序列表为空，这样可以避免多余操作
                    arrOriginalSelectAnswerIndex = null;
                    break;
                case EnumQuestionBaseTypeCode.Fill:
                    stringBuilder.Append("$Enter$<span style='font-size:2.0pt'>.</span>");
                    break;
                case EnumQuestionBaseTypeCode.Compose:
                    stringBuilder.Append("$Enter$");
                    break;
                case EnumQuestionBaseTypeCode.Operate:	//新加入类型为操作题 目的是要显示office操作钮
                    break;
                case EnumQuestionBaseTypeCode.Typing:
                    break;
                case EnumQuestionBaseTypeCode.Answer:
                    //Nick2007/9/4加上可自定义空白行数所功能
                    stringBuilder.Append("$Enter$");
                    for (int k = 0; k < 1; k++)
                    {
                        stringBuilder.Append("$Enter$");
                    }
                    break;
                case EnumQuestionBaseTypeCode.Program:
                case EnumQuestionBaseTypeCode.ProgramFill:
                    if (!string.IsNullOrEmpty(question.PreinstallCode))
                    {
                        var preinstallCode = question.PreinstallCode
                            .Replace("<", "&lt;")
                            .Replace(">", "&gt;")
                            .Replace("\n", "<br>")
                            .Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
                        stringBuilder.Append("$Enter$预设代码：");
                        stringBuilder.Append("$Enter$" + preinstallCode);
                    }
                    break;
            }
            return stringBuilder.ToString();
        }

        private string GetQuestionAnswerWord(ExamQuestion question, int[] arrOriginalSelectAnswerIndex)
        {
            var questionUid = question.Id;
            var questionBaseTypeCode = question.questionBaseTypeCode;
            var standardAnswer = question.standardAnswer;

            //处理回车换行
            standardAnswer = standardAnswer.Replace("\r\n", "<br>");

            var stringBuilder = new StringBuilder();
            var sAnswerText = GetQuestionAnswerTextWord(questionBaseTypeCode, standardAnswer, arrOriginalSelectAnswerIndex);
            sAnswerText = FilePathUtil.GetContentTextWithFilePath(questionUid.ToString(), "question", sAnswerText);//也要图片
            var strAnswerDesc  = "<font color='blue'>&nbsp;★" + "标准答案：";

            if (sAnswerText != "none")
            {
                stringBuilder.Append(strAnswerDesc + sAnswerText + "</font>$Enter$");
            }
            else
            {
                stringBuilder.Append("</font>$Enter$");
            }

            stringBuilder.Append("$Enter$");
            
            #region '显示得分评语信息'
            var auestionAnalysis = question.questionAnalysis;
            auestionAnalysis = FilePathUtil.GetContentTextWithFilePath(questionUid.ToString(), "question", auestionAnalysis, true);
            if (questionBaseTypeCode != EnumQuestionBaseTypeCode.Compose)
            {
                if (auestionAnalysis != "")
                {
                    stringBuilder.Append("<font color='blue'>&nbsp;★答题分析：" + auestionAnalysis + "</font>$Enter$");
                }
            }
            #endregion
            return stringBuilder.ToString();

        }
        
        //计算选项内容真实长度
        private static int GetStringRealLengthWord(string s)
        {
            var length = 0;
            var arr = s.ToCharArray();
            foreach (var t in arr)
            {
                int asciiNumber = (int)t;
                if (asciiNumber < -256 || asciiNumber > 256)
                    length = length + 2;
                else
                    length = length + 1;
            }
            return length;
        }

        //填充空格
        private static string FormatMixLengthStringWord(string s, int length)
        {
            var realLength = GetStringRealLengthWord(s);
            var spaceStr = "";
            for (var i = realLength; i < length; i++)
            {
                spaceStr = spaceStr + "&nbsp;";
            }
            return s + spaceStr;
        }

        //取得试题答案
        private static string GetQuestionAnswerTextWord(string questionBaseTypeCode, string answer, int[] arrOriginalSelectAnswerIndex)
        {
            char[] spliter = {'|'};

            string sAnswerText = "";
            switch (questionBaseTypeCode)
            {
                case EnumQuestionBaseTypeCode.Single:
                    sAnswerText = QuestionUtil.GetSelectAnswerView(answer, arrOriginalSelectAnswerIndex);
                    break;
                case EnumQuestionBaseTypeCode.Multi:
                    sAnswerText = QuestionUtil.GetSelectAnswerView(answer, arrOriginalSelectAnswerIndex);
                    break;
                case EnumQuestionBaseTypeCode.Judge:
                    if (answer == "Y")
                        sAnswerText ="正确";
                    else if (answer == "N")
                        sAnswerText = "错误";
                    else
                        sAnswerText = "";
                    break;
                case EnumQuestionBaseTypeCode.JudgeCorrect:
                    if (answer == "Y")
                        sAnswerText = "正确";
                    else
                        sAnswerText = "错误;改正" + ":" + answer;
                    break;
                case EnumQuestionBaseTypeCode.Fill:
                    sAnswerText = "";
                    string[] arrAnswer = answer.Split(spliter);
                    for (int j = 0; j < arrAnswer.Length; j++)
                    {
                        sAnswerText = sAnswerText + (j + 1) + ". " + arrAnswer[j].Replace("&Vertical;", "|") + ";";
                    }
                    break;
                case EnumQuestionBaseTypeCode.Compose:
                    sAnswerText = "none";
                    break;
                default:
                    sAnswerText = answer;
                    break;
            }
            return sAnswerText;
        }
        private string CreateNewCode()
        {
            var code = "P000001";
            var entity = _iExamPaperRep.GetAll().Where(a => !a.isCustomCode).OrderByDescending(a => a.createTime).FirstOrDefault();
            if (entity != null)
            {
                code = entity.paperCode;
                do
                {
                    code = StringUtil.GetNextCodeByAuto(code);
                } while (_iExamPaperRep.GetAll().Any(a => a.paperCode == code));
            }
            return code;
        }

        #region 更新试卷的XML字段值

        /// <summary>
        /// 更新试卷的XML字段值
        /// </summary>
        /// <param name="paperId"></param>
        /// <returns></returns>
        public async Task<string> UpdatePaperXml(string paperId)
        {
            var paperUid = Guid.Parse(paperId);
            var paper = _iExamPaperRep.FirstOrDefault(a => a.Id == paperUid);
            var paperNodes = _iExamPaperNodeRep.GetAll().Where(a => a.paperUid == paperUid).OrderBy(a => a.listOrder);
            var paperNodeQuestionQueryable = _iExamPaperNodeQuestionRep.GetAll().Where(a => a.paperUid == paperUid).OrderBy(a => a.listOrder);
            var paperQuestions = await paperNodeQuestionQueryable.Join(_iExamQuestionRep.GetAll(), nq=>nq.questionUid, q=>q.Id, (nq, q)=>q).ToListAsync();
            var questions = paperQuestions.MapTo<List<ExamQuestionDto>>();
            var paperNodeQuestions = paperNodeQuestionQueryable.ToList();
            var sb = new StringBuilder();
            sb.AppendLine("<?xml version = \"1.0\" encoding=\"gb2312\" standalone = \"no\"?>");

            sb.AppendLine("<exam_paper_object>");

            //写版本号
            sb.AppendLine("    <paper_xml_version>2.0</paper_xml_version>");

            //试卷基本信息
            sb.AppendLine("    <" + "exam_paper" + ">");
            sb.AppendLine("        <" + "paper_uid" + ">" + paper.Id + "</" + "paper_uid" + ">");
            sb.AppendLine("        <" + "paper_name" + "><![CDATA[" + paper.paperName + "]]></" + "paper_name" + ">");
            sb.AppendLine("        <" + "question_num" + ">" + paper.questionNum + "</" + "question_num" + ">");
            sb.AppendLine("        <" + "total_score" + ">" + paper.totalScore.ToString("0.##") + "</" + "total_score" + ">");
            sb.AppendLine("        <" + "each_option_score" + ">" + paper.eachOptionScore + "</" + "each_option_score" + ">");
            sb.AppendLine("        <" + "is_show_score" + ">" + paper.isShowScore + "</" + "is_show_score" + ">");
            sb.AppendLine("        <" + "is_single_as_multi" + ">" + paper.isSingleAsMulti + "</" + "is_single_as_multi" + ">");
            sb.AppendLine("        <" + "paper_class_code" + ">" + paper.paperClassCode + "</" + "paper_class_code" + ">");
            sb.AppendLine("        <" + "paper_code" + ">" + paper.paperCode + "</" + "paper_code" + ">");
            sb.AppendLine("        <" + "paper_type_code" + ">" + paper.paperTypeCode + "</" + "paper_type_code" + ">");
            sb.AppendLine("        <" + "policy_uid" + ">" + paper.policyUid + "</" + "policy_uid" + ">");
            sb.AppendLine("        <" + "remarks" + "><![CDATA[" + paper.remarks + "]]></" + "remarks" + ">");
            sb.AppendLine("        <" + "folder_uid" + ">" + paper.folderUid + "</" + "folder_uid" + ">");
            sb.AppendLine("    </" + "exam_paper" + ">");

            //试卷大题根节点
            sb.AppendLine("    <" + "exam_paper_node" + "s>");
            Guid paperNodeUid;
            int errorCount = 0;
            var paperUtil = new PaperUtil(_iExamPaperNodeQuestionRep);
            foreach (ExamPaperNode examPaperNodeRow in paperNodes)
            {
                paperNodeUid = examPaperNodeRow.Id;

                //一个大题
                //大题基本信信
                sb.AppendLine("        <" + "exam_paper_node" + ">");
                sb.AppendLine("            <" + "paper_node_uid" + ">" + examPaperNodeRow.Id + "</" + "paper_node_uid" + ">");
                sb.AppendLine("            <" + "paper_node_name" + "><![CDATA[" + examPaperNodeRow.paperNodeName + "]]></" + "paper_node_name" + ">");
                sb.AppendLine("            <" + "paper_node_desc" + "><![CDATA[" + examPaperNodeRow.paperNodeDesc + "]]></" + "paper_node_desc" + ">");
                sb.AppendLine("            <" + "question_num" + ">" + examPaperNodeRow.questionNum + "</" + "question_num" + ">");
                sb.AppendLine("            <" + "question_score" + ">" + examPaperNodeRow.questionScore.ToString("0.##") + "</" + "question_score" + ">");
                sb.AppendLine("            <" + "total_score" + ">" + examPaperNodeRow.totalScore.ToString("0.##") + "</" + "total_score" + ">");
                sb.AppendLine("            <" + "list_order" + ">" + examPaperNodeRow.listOrder + "</" + "list_order" + ">");

                //大题下的试题
                sb.AppendLine("            <" + "exam_paper_node_question" + "s>");

                foreach (var examPaperNodeQuestionRow in paperNodeQuestions)
                {
                    //本大题的才加进来，非本大题不加进来
                    if (examPaperNodeQuestionRow.paperNodeUid != paperNodeUid) continue;

                    Guid questionUid = examPaperNodeQuestionRow.questionUid;

                    var examQuestionRow = questions.FirstOrDefault(a => a.Id == questionUid);
                    if (examQuestionRow == null)
                    {
                        //Logger.GetLogger("UpdatePaperXml").Write(LogLevel.warn, "生成试卷" + paper.paper_name + "(" + paper.paper_uid + ")时出现错误，试题在题库中找不到．");
                        continue;
                    }
                    /* 0分试题不再计入错误里
                    if (examPaperNodeQuestionRow.paperQuestionScore == 0)
                    {
                        errorCount++;
                    }
                    */
                    if (examQuestionRow.parentQuestionUid == Guid.Empty)    //子试题不要另外输出来,因为在组合题中会输出来
                    {
                        sb.AppendLine("                <" + "exam_paper_node_question" + ">");

                        sb.AppendLine(paperUtil.TranslatePaperQuestionObjectToXml(examPaperNodeQuestionRow, examQuestionRow));

                        //如果是组合题则把子试题加进来
                        if (examQuestionRow.questionBaseTypeCode == "compose")
                        {
                            sb.AppendLine("                    <sub_exam_paper_node_questions>");
                            //获取子试题列表
                            var subExamQuestionRowCollection = questions.Where(q => q.parentQuestionUid == questionUid).ToList();
                            
                            foreach (var subExamQuestionRow in subExamQuestionRowCollection)
                            {
                                var subExamPaperNodeQuestionRow = paperNodeQuestions.FirstOrDefault(a => a.paperNodeUid == paperNodeUid && a.questionUid == subExamQuestionRow.Id);
                                if (subExamPaperNodeQuestionRow != null)
                                {
                                    sb.AppendLine("                <" + "exam_paper_node_question" + ">");
                                    sb.AppendLine(paperUtil.TranslatePaperQuestionObjectToXml(subExamPaperNodeQuestionRow, subExamQuestionRow));
                                    sb.AppendLine("                </" + "exam_paper_node_question" + ">");
                                }
                            }
                            sb.AppendLine("                    </sub_exam_paper_node_questions>");
                        }
                        sb.AppendLine("            </" + "exam_paper_node_question" + ">");
                    }

                }
                sb.AppendLine("            </" + "exam_paper_node_question" + "s>");
                sb.AppendLine("        </" + "exam_paper_node" + ">");
            }
            sb.AppendLine("    </" + "exam_paper_node" + "s>");
            sb.AppendLine("</exam_paper_object>");
            paper.paperXml = sb.ToString();
            await _iExamPaperRep.UpdateAsync(paper);
            //var outMessage = errorCount > 0 ? string.Format("试卷的试题存在0分的题目数为{0}个", errorCount) : "";//0分试题不再计入错误里
            //发送事件通知
            ExamEventArg eventArg = new ExamEventArg
            {
                ChangeType = EnumExamEventChangeType.Edit,
                ObjectUid = paperUid,
                OperatorUid = CookieHelper.GetLoginInUserInfo().Id
            };
            _iExamInfoChangeService.ExamPaperInfoChanged(eventArg);
            return "";
        }
        
        public void UpdatePaperXml(ReturnValue retValue)
        {
            var paperUtil = new PaperUtil(_iExamPaperNodeQuestionRep);
            //试卷信息
            var paper = (Exam.ExamPaper)retValue.GetValue("examPaperRow");
            var paperNodes = (List<ExamPaperNode>)retValue.GetValue("examPaperNodeRowCollection");
            var paperNodeQuestions = (List<ExamPaperNodeQuestion>)retValue.GetValue("examPaperNodeQuestionRowCollection");
            var questions = (List<ExamQuestionDto>)retValue.GetValue("examQuestionRowCollection");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version = \"1.0\" encoding=\"gb2312\" standalone = \"no\"?>");

            sb.AppendLine("<exam_paper_object>");

            //写版本号
            sb.AppendLine("    <paper_xml_version>2.0</paper_xml_version>");

            //试卷基本信息
            sb.AppendLine("    <" + "exam_paper" + ">");
            sb.AppendLine("        <" + "paper_uid" + ">" + paper.Id + "</" + "paper_uid" + ">");
            sb.AppendLine("        <" + "paper_name" + "><![CDATA[" + paper.paperName + "]]></" + "paper_name" + ">");
            sb.AppendLine("        <" + "question_num" + ">" + paper.questionNum + "</" + "question_num" + ">");
            sb.AppendLine("        <" + "total_score" + ">" + paper.totalScore.ToString("0.##") + "</" + "total_score" + ">");
            sb.AppendLine("        <" + "each_option_score" + ">" + paper.eachOptionScore + "</" + "each_option_score" + ">");
            sb.AppendLine("        <" + "is_show_score" + ">" + paper.isShowScore + "</" + "is_show_score" + ">");
            sb.AppendLine("        <" + "is_single_as_multi" + ">" + paper.isSingleAsMulti + "</" + "is_single_as_multi" + ">");
            sb.AppendLine("        <" + "paper_class_code" + ">" + paper.paperClassCode + "</" + "paper_class_code" + ">");
            sb.AppendLine("        <" + "paper_code" + ">" + paper.paperCode + "</" + "paper_code" + ">");
            sb.AppendLine("        <" + "paper_type_code" + ">" + paper.paperTypeCode + "</" + "paper_type_code" + ">");
            sb.AppendLine("        <" + "policy_uid" + ">" + paper.policyUid + "</" + "policy_uid" + ">");
            sb.AppendLine("        <" + "remarks" + "><![CDATA[" + paper.remarks + "]]></" + "remarks" + ">");
            sb.AppendLine("        <" + "folder_uid" + ">" + paper.folderUid + "</" + "folder_uid" + ">");
            sb.AppendLine("    </" + "exam_paper" + ">");

            //试卷大题根节点
            sb.AppendLine("    <" + "exam_paper_node" + "s>");
            Guid paperNodeUid;
            foreach (var examPaperNodeRow in paperNodes)
            {
                paperNodeUid = examPaperNodeRow.Id;

                //一个大题
                //大题基本信信
                sb.AppendLine("        <" + "exam_paper_node" + ">");
                sb.AppendLine("            <" + "paper_node_uid" + ">" + examPaperNodeRow.Id + "</" + "paper_node_uid" + ">");
                sb.AppendLine("            <" + "paper_node_name" + "><![CDATA[" + examPaperNodeRow.paperNodeName + "]]></" + "paper_node_name" + ">");
                sb.AppendLine("            <" + "paper_node_desc" + "><![CDATA[" + examPaperNodeRow.paperNodeDesc + "]]></" + "paper_node_desc" + ">");
                sb.AppendLine("            <" + "question_num" + ">" + examPaperNodeRow.questionNum + "</" + "question_num" + ">");
                sb.AppendLine("            <" + "question_score" + ">" + examPaperNodeRow.questionScore.ToString("0.##") + "</" + "question_score" + ">");
                sb.AppendLine("            <" + "total_score" + ">" + examPaperNodeRow.totalScore.ToString("0.##") + "</" + "total_score" + ">");
                sb.AppendLine("            <" + "list_order" + ">" + examPaperNodeRow.listOrder + "</" + "list_order" + ">");

                //大题下的试题
                sb.AppendLine("            <" + "exam_paper_node_question" + "s>");

                //本大题的才加进来，非本大题不加进来
                var nodeQuestions = paperNodeQuestions.Where(a => a.paperNodeUid == paperNodeUid).ToList();

                foreach (ExamPaperNodeQuestion examPaperNodeQuestionRow in nodeQuestions)
                {
                    //以下判断用上面的Linq筛选出来了
                    //if (examPaperNodeQuestionRow.paperNodeUid != paperNodeUid) continue;

                    var questionUid = examPaperNodeQuestionRow.questionUid;

                    var examQuestionRow = questions.Find(a => a.Id == questionUid);
                    if (examQuestionRow == null)
                    {
                        //Logger.GetLogger("UpdatePaperXml").Write(LogLevel.warn, "生成试卷" + paper.paper_name + "(" + paper.paper_uid + ")时出现错误，试题在题库中找不到．");
                        continue;
                    }

                    if (examQuestionRow.parentQuestionUid == Guid.Empty)    //子试题不要另外输出来,因为在组合题中会输出来
                    {
                        sb.AppendLine("                <" + "exam_paper_node_question" + ">");

                        sb.AppendLine(paperUtil.TranslatePaperQuestionObjectToXml(examPaperNodeQuestionRow, examQuestionRow));

                        //如果是组合题则把子试题加进来
                        if (examQuestionRow.questionBaseTypeCode == "compose")
                        {
                            sb.AppendLine("                    <sub_exam_paper_node_questions>");
                            //获取子试题列表
                            var subExamQuestionRowCollection = questions.Where(a=>a.parentQuestionUid == questionUid).ToList();
                            //foreach (ExamQuestion row in questions)
                            //{
                            //    if (row.parentQuestionUid == questionUid.ToString()) subExamQuestionRowCollection.Add(row);
                            //}

                            foreach (var subExamQuestionRow in subExamQuestionRowCollection)
                            {
                                ExamPaperNodeQuestion subExamPaperNodeQuestionRow = paperNodeQuestions.Find(a => a.paperNodeUid == paperNodeUid && a.questionUid == subExamQuestionRow.Id);
                                if (subExamPaperNodeQuestionRow != null)
                                {
                                    sb.AppendLine("                <" + "exam_paper_node_question" + ">");
                                    sb.AppendLine(paperUtil.TranslatePaperQuestionObjectToXml(subExamPaperNodeQuestionRow, subExamQuestionRow));
                                    sb.AppendLine("                </" + "exam_paper_node_question" + ">");
                                }
                            }
                            sb.AppendLine("                    </sub_exam_paper_node_questions>");
                        }
                        sb.AppendLine("            </" + "exam_paper_node_question" + ">");
                    }

                }
                sb.AppendLine("            </" + "exam_paper_node_question" + "s>");
                sb.AppendLine("        </" + "exam_paper_node" + ">");
            }
            sb.AppendLine("    </" + "exam_paper_node" + "s>");
            sb.AppendLine("</exam_paper_object>");
            paper.paperXml = sb.ToString();

            _iExamPaperRep.UpdateAsync(paper);

            //发送事件通知
            ExamEventArg eventArg = new ExamEventArg
            {
                ChangeType = EnumExamEventChangeType.Edit,
                ObjectUid = paper.Id,
                OperatorUid = paper.creatorUid,
            };
            _iExamInfoChangeService.ExamPaperInfoChanged(eventArg);
        }
        
        #endregion 
    
        #region 生成预览试卷

        private string GetPaperView(Guid paperUid, out Exam.ExamPaper examPaper)
        {
            var paper = _iExamPaperRep.Get(paperUid);
            var paperNodes = _iExamPaperNodeRep.GetAll().Where(a => a.paperUid == paperUid).OrderBy(a => a.listOrder).ToList();
            var paperNodeQuestions =
                _iExamPaperNodeQuestionRep.GetAll().Where(a => a.paperUid == paperUid)
                    .Join(_iExamPaperNodeRep.GetAll(), question => question.paperNodeUid, node => node.Id,
                        (question, node) => question).OrderBy(a => a.listOrder).ToList();
            //var questions =
            //    _iExamQuestionRep.GetAll()
            //        .Join(_iExamPaperNodeQuestionRep.GetAll(), eq => eq.Id, pnq => pnq.questionUid,
            //            (eq, pnq) => new {eq, pnq}).Where(a=>a.pnq.paperUid == paperUid).Select(a=>a.eq).ToList();
            var examQuestionRowCollection = new List<ExamQuestion>();
            //存储标准答案
            var examQuestionDtoCollection = new List<ExamQuestionDto>();
            //查询试题与预设标准答案
            var queryQuestion = (from eq in _iExamQuestionRep.GetAll()
                join epnq in _iExamPaperNodeQuestionRep.GetAll().Where(a=>a.paperUid.Equals(paperUid)) on eq.Id equals epnq.questionUid
                join qc in _iQuestionStandardCodeRep.GetAll().Where(q => q.isDefault == 1) on eq.Id equals qc.questionId                   
                into tempTable 
                from temp in tempTable.DefaultIfEmpty()
                select new
                {
                    eq,
                    code = temp == null ? "" : temp.code

                }).ToList();


            queryQuestion.ForEach(q =>
            {
                examQuestionRowCollection.Add(q.eq);
                var examQuestionDto = q.eq.MapTo<ExamQuestionDto>();
                examQuestionDto.standardCode = q.code;
                examQuestionDtoCollection.Add(examQuestionDto);
            });


            //生成试卷的序号
            Hashtable questionIndex = null;
            var goableIndex = 0;
            questionIndex = new Hashtable();
            foreach (var paperNodeRow in paperNodes)
            {
                var paperNodeUid = paperNodeRow.Id;
                foreach (var dataRow in paperNodeQuestions.Where(a => a.paperNodeUid == paperNodeUid).ToList())
                {
                    var questionUid = dataRow.questionUid;
                    var question = examQuestionRowCollection.FirstOrDefault(a => a.Id == questionUid);
                    if (question != null)
                    {
                        if (question.parentQuestionUid != Guid.Empty)
                            continue;
                        if (question.questionBaseTypeCode != EnumQuestionBaseTypeCode.Compose)
                        {
                            goableIndex = goableIndex + 1;
                            if (!questionIndex.ContainsKey(questionUid))
                                questionIndex.Add(questionUid, goableIndex);
                        }
                        else
                        {
                            var selectRows = examQuestionRowCollection.Where(a => a.parentQuestionUid != Guid.Empty  && a.parentQuestionUid == questionUid).OrderBy(a=>a.listOrder).ToList();
                            foreach (var subquestion in selectRows)
                            {
                                goableIndex = goableIndex + 1;
                                var chileQuestionUid = subquestion.Id;
                                if (!questionIndex.ContainsKey(chileQuestionUid))
                                    questionIndex.Add(chileQuestionUid, goableIndex);
                            }
                        }
                    }
                }
            }
            examPaper = paper;
            // 清除在线编辑器读取world表格的时候在td内容里会产生</br>标签。
            var info = new PaperViewBuildInfo()
            {
                ViewType = "preview",
                IsMixOrder = false,
                ExamGradeUid = Guid.Empty,
                ExamPaperRow = paper,
                ExamPaperNodeRowCollection = paperNodes,
                ExamPaperNodeQuestionRowCollection = paperNodeQuestions,
                ExamQuestionRowCollection = examQuestionDtoCollection,
                UserAnswerDataTable = null,
                QuesionIndex = questionIndex
            };
            var paperUtil = new PaperUtil(_iExamQuestionTypeRep, _iExamExamRep, _iExamGradeRep, _iExamAnswerRep, _iExamProgramResultRep);
            return ReplaceHtml(paperUtil.GetPaperQuestionViewForPaper(info));
        }

        private string GetPaperViewFromPolicy(Guid policyUid, string policyItemUids, out Exam.ExamPaper examPaper)
        {
            ReturnValue retValue = CreatePaperByPolicy(policyUid, policyItemUids);
            if (retValue.HasError == true)
            {
                examPaper = null;
                return retValue.Message;
            }


            //试卷信息
            var examPaperRow = (Exam.ExamPaper)retValue.GetValue("examPaperRow");
            var examPaperNodeRowCollection = (List<ExamPaperNode>)retValue.GetValue("examPaperNodeRowCollection");
            var examPaperNodeQuestionRowCollection = (List<ExamPaperNodeQuestion>)retValue.GetValue("examPaperNodeQuestionRowCollection");
            var examQuestionRowCollection = (List<ExamQuestionDto>)retValue.GetValue("examQuestionRowCollection");

            //生成试卷的序号
            Hashtable questionIndex = null;

            int goableIndex = 1;
            questionIndex = new Hashtable();

            foreach (ExamPaperNode examPaperNodeRow in examPaperNodeRowCollection)
            {
                foreach (ExamPaperNodeQuestion examPaperNodeQuestionRow in examPaperNodeQuestionRowCollection)
                {
                    //如果试题不包含在此大题
                    if (examPaperNodeQuestionRow.paperNodeUid != examPaperNodeRow.Id)
                        continue;
                    var questionUid = examPaperNodeQuestionRow.questionUid;
                    //如果没有找到此试题
                    var examQuestion = examQuestionRowCollection.FirstOrDefault(a => a.Id == questionUid);
                    if (examQuestion == null)
                        continue;
                    //组合题的子试题不在此处理
                    if (examQuestion.parentQuestionUid != Guid.Empty)
                        continue;
                    //处理组合题的子试题
                    if (examQuestion.questionBaseTypeCode == EnumQuestionBaseTypeCode.Compose)
                    {
                        foreach (var examQuestionRow in examQuestionRowCollection)
                        {
                            if (examQuestionRow.parentQuestionUid == questionUid)
                            {
                                questionIndex.Add(examQuestionRow.Id, goableIndex);
                                goableIndex += 1;
                            }
                        }
                    }
                    else
                    {
                        questionIndex.Add(questionUid, goableIndex);
                        goableIndex += 1;
                    }
                }
            }
            var questionIdList = examQuestionRowCollection.Select(a => a.Id).ToList();
            //查询试题与预设标准答案
            var queryQuestion = (from eq in _iExamQuestionRep.GetAll()
                                 join qc in _iQuestionStandardCodeRep.GetAll().Where(q => q.isDefault == 1) on eq.Id equals qc.questionId
                                 into tempTable
                                 from temp in tempTable.DefaultIfEmpty()
                                 where questionIdList.Contains(eq.Id)
                                 select new
                                 {
                                     eq,
                                     code = temp == null ? "" : temp.code

                                 }).ToList();


            queryQuestion.ForEach(q =>
            {
                var examQuestionDto = examQuestionRowCollection.Find(a=>a.Id == q.eq.Id);
                examQuestionDto.standardCode = q.code;
            });

            examPaper = examPaperRow;
            var info = new PaperViewBuildInfo()
            {
                ViewType = "preview",
                IsMixOrder = false,
                ExamGradeUid = Guid.Empty,
                ExamPaperRow = examPaperRow,
                ExamPaperNodeRowCollection = examPaperNodeRowCollection,
                ExamPaperNodeQuestionRowCollection = examPaperNodeQuestionRowCollection,
                ExamQuestionRowCollection = examQuestionRowCollection,
                UserAnswerDataTable = null,
                QuesionIndex = questionIndex
            };

            var paperUtil = new PaperUtil(_iExamQuestionTypeRep, _iExamExamRep, _iExamGradeRep, _iExamAnswerRep, _iExamProgramResultRep);
            return paperUtil.GetPaperQuestionViewForPaper(info);
        }

        /// <summary>
        /// 生成用户试卷视图
        /// </summary>
        private string GetUserPaperView(Exam.ExamPaper paper, ExamExam examExamRow, ExamGrade examGradeRow, string viewType, string filterType, bool isAllowModifyObjectAnswer)
        {
            var examPaperNodeQuestyable = _iExamPaperNodeRep.GetAll().Where(a => a.paperUid == paper.Id);
            var examPaperNodeRowCollection = examPaperNodeQuestyable.OrderBy(a => a.listOrder).ToList();
            var examPaperNodeQuestionRowCollection =
                _iExamPaperNodeQuestionRep.GetAll().Where(a => a.paperUid == paper.Id)
                    .Join(_iExamPaperNodeRep.GetAll(), question => question.paperNodeUid, node => node.Id,
                        (question, node) => question)
                    .OrderBy(a => a.listOrder).ToList();
            //var examQuestionRowCollection =
            //    _iExamQuestionRep.GetAll()
            //        .Join(_iExamPaperNodeQuestionRep.GetAll(), eq => eq.Id, pnq => pnq.questionUid,
            //            (eq, pnq) => new {eq, pnq})
            //        .Join(examPaperNodeQuestyable, t => t.pnq.paperNodeUid, pn => pn.Id, (t, pn) => t.eq).ToList();
            var examQuestionRowCollection = new List<ExamQuestion>();
            //存储标准答案
            var examQuestionDtoCollection = new List<ExamQuestionDto>();
            //查询试题与预设标准答案
            var queryQuestion = (from eq in _iExamQuestionRep.GetAll()
                join
                epnq in _iExamPaperNodeQuestionRep.GetAll() on eq.Id equals epnq.questionUid
                join epn in examPaperNodeQuestyable on epnq.paperNodeUid equals epn.Id
                join qc in _iQuestionStandardCodeRep.GetAll().Where(q => q.isDefault == 1) on eq.Id equals qc.questionId
                into tempTable
                from temp in tempTable.DefaultIfEmpty()
                select new
                {
                    eq,
                    code = temp == null ? "" : temp.code

                }).ToList();

           
            queryQuestion.ForEach(q =>
            {
                examQuestionRowCollection.Add(q.eq);
                var examQuestionDto =q.eq.MapTo<ExamQuestionDto>();
                examQuestionDto.standardCode = q.code;
                examQuestionDtoCollection.Add(examQuestionDto);
            });



            if (paper == null)
            {
                return ("找不到试卷信息！");
            }
            //试卷信息
            //生成试卷的序号
            Hashtable questionIndex = null;
            int goableIndex = 0;
            questionIndex = new Hashtable();


            foreach (var paperNode in examPaperNodeRowCollection)
            {

                var nodeQuestions =
                    examPaperNodeQuestionRowCollection.Where(a => a.paperNodeUid == paperNode.Id).OrderBy(a => a.listOrder);

                var questionUid = Guid.Empty;
                foreach (var nodeQuestion in nodeQuestions)
                {
                    questionUid = nodeQuestion.questionUid;

                    var question = examQuestionRowCollection.FirstOrDefault(a => a.Id == questionUid);
                    if (question != null)
                    {
                        if (question.parentQuestionUid != Guid.Empty)
                            continue;
                        if (question.questionBaseTypeCode != EnumQuestionBaseTypeCode.Compose)
                        {
                            goableIndex = goableIndex + 1;
                            if (!questionIndex.ContainsKey(questionUid))
                                questionIndex.Add(questionUid, goableIndex);
                        }
                        else
                        {
                            var subQuestions = examQuestionRowCollection.Where(a => a.parentQuestionUid != Guid.Empty && a.parentQuestionUid.Equals(questionUid)).ToList();

                            foreach (var subquestion in subQuestions)
                            {
                                goableIndex = goableIndex + 1;
                                var chileQuestionUid = subquestion.Id;
                                if (!questionIndex.ContainsKey(chileQuestionUid))
                                    questionIndex.Add(chileQuestionUid, goableIndex);
                            }
                        }
                    }
                }
            }

            //考生答案信息
            List<ExamAnswer> dtUserAnswer;
            string userAnswerXML = string.Empty;
            if (examExamRow.isRealtimeSaveAnswerToDb == "Y")       //实时保存则从答案表中取
            {

                dtUserAnswer = _iExamAnswerRep.GetAll().Where(a => a.examGradeUid.Equals(examGradeRow.Id)).ToList();

                if (dtUserAnswer.Count == 0)
                {
                    userAnswerXML = string.Empty;
                    if (examGradeRow != null)
                    {
                        userAnswerXML = _iExamUserAnswerRep.Get(examGradeRow.userAnswerUid).userAnswer;
                    }
                    dtUserAnswer = GetExamAnswerTableByXML(userAnswerXML);
                }
            }
            else
            {
                userAnswerXML = string.Empty;
                if (examGradeRow != null)
                {
                    userAnswerXML = _iExamUserAnswerRep.Get(examGradeRow.userAnswerUid).userAnswer;
                }
                if (string.IsNullOrEmpty(userAnswerXML))
                {
                    dtUserAnswer = _iExamAnswerRep.GetAll().Where(a => a.examGradeUid.Equals(examGradeRow.Id)).ToList();
                }
                else
                {
                    dtUserAnswer = GetExamAnswerTableByXML(userAnswerXML);
                }
            }

            DataTable tabUserAnswer = DataTableExtensions.ToDataTable(dtUserAnswer);
            if (filterType == EnumJudgeResultCode.Error)
                FilterQuestion(ref examPaperNodeQuestionRowCollection, dtUserAnswer, examQuestionRowCollection, EnumJudgeResultCode.Error);
            else if (filterType == EnumJudgeResultCode.Right)
                FilterQuestion(ref examPaperNodeQuestionRowCollection, dtUserAnswer, examQuestionRowCollection, EnumJudgeResultCode.Right);
            else if (filterType == "Empty")
                FilterQuestion(ref examPaperNodeQuestionRowCollection, dtUserAnswer, examQuestionRowCollection, "Empty");
            else if (filterType == "NoAnswer")
                FilterQuestion(ref examPaperNodeQuestionRowCollection, dtUserAnswer, examQuestionRowCollection, "NoAnswer");

            DateTime currentDateTime = DateTime.Now;
            string publishGradeDate = examExamRow.publishGradeDate ?? "";
            //允许查看成绩天数

            int allowSeeGradeDays = examExamRow.allowSeeGradeDays ?? 0;
            bool isAllowSeeGradeDays = true;
            if (allowSeeGradeDays > 0)
            {

                var lastUpdateTime = DateTimeUtil.ConvertToDataStr(examGradeRow.lastUpdateTime);
                string allowSeeGradeTime = DateTimeUtil.AddDays(lastUpdateTime, allowSeeGradeDays);
                if (DateTimeUtil.SecondsAfter(allowSeeGradeTime, DateTime.Now.ToString()) > 0) isAllowSeeGradeDays = false;
            }

            bool isShowGrade = false;
            if (examExamRow.isAllowSeeGrade == "Y" && (publishGradeDate == "" || DateTimeUtil.ToDateTime(publishGradeDate) < currentDateTime) && isAllowSeeGradeDays == true)
            {
                isShowGrade = true;
            }

            var paperUtil = new PaperUtil(_iExamQuestionTypeRep, _iExamExamRep, _iExamGradeRep, _iExamAnswerRep, _iExamProgramResultRep);
                var info = new PaperViewBuildInfo
                {
                    ViewType = EnumPaperViewType.ViewAnswer,
                    IsMixOrder = false,
                    ExamGradeUid = examGradeRow.Id,
                    ExamPaperRow = paper,
                    ExamPaperNodeRowCollection = examPaperNodeRowCollection,
                    ExamPaperNodeQuestionRowCollection = examPaperNodeQuestionRowCollection,
                    ExamQuestionRowCollection = examQuestionDtoCollection,
                    UserAnswerDataTable = tabUserAnswer,
                    QuesionIndex = questionIndex,
                    IsAllowModifyObjectAnswer = isAllowModifyObjectAnswer
                };
            if (viewType == "manage")
            {
                return paperUtil.GetPaperQuestionViewForPaper(info);
            }
            else
            {
                if (isAllowModifyObjectAnswer)
                {
                    info.ViewType = EnumPaperViewType.Judge;
                }
                else
                {
                    //要允许查看成绩和答案才出这个,否则只看试题和考生答案(不允许看成绩时也不允许看标准答案,因为标准答案显出来会对比出分数)
                    if (examExamRow.isAllowSeeAnswer == "Y")
                    {
                        //info.ViewType = isShowGrade ?  EnumPaperViewType.ViewAnswer : EnumPaperViewType.ViewUserAnswer;
                        info.ViewType = EnumPaperViewType.ViewUserAnswerWithAnswer;//业务需要，考完不限显示考卷成绩，成绩出来后才能查看，所以改为一直都能查看对错

                    }
                    else
                    {
                        //info.ViewType = isShowGrade ? EnumPaperViewType.ViewUserAnswerWithAnswer : EnumPaperViewType.ViewUserAnswer;
                        info.ViewType = EnumPaperViewType.ViewUserAnswerWithAnswer;//业务需要，考完不限显示考卷成绩，成绩出来后才能查看，所以改为一直都能查看对错
                    }
                }
                return paperUtil.GetPaperQuestionViewForPaper(info);
            }
        }

        /// <summary>
        /// 跟据出卷策略编号生成试卷
        /// </summary>
        /// <param name="policyUid"></param>
        /// <param name="createPolicyItemUids"></param>
        /// <returns></returns>
        private ReturnValue CreatePaperByPolicy(Guid policyUid, string createPolicyItemUids)
        {
            try
            {


                ReturnValue retValue = new ReturnValue(false, "");

                var createPolicyNodeUid = Guid.Empty; //指定卷策所在PolicyNodeUid
                Hashtable htPolicyItem = new Hashtable();

                decimal convertedTotalScore = 0;        //折算后的分数
                decimal convertScoreRate = 1;                //折算分数系数

                ////试题的原始分数（题库中），目的是折算组合题中每个小题的分数
                decimal sourceQuestionScore = 0;

                var paperName = string.Empty;
                var folerUid = Guid.Empty;
                string allCreatedQuestionUids = string.Empty;

                var examPolicyRow = _iExamPolicyRep.Get(policyUid);

                if (examPolicyRow == null)
                {
                    retValue.HasError = true;
                    retValue.Message = "没有找到随机试卷信息";
                    return retValue;
                }

                if (!string.IsNullOrEmpty(createPolicyItemUids))
                {
                    string[] policyItems = createPolicyItemUids.Split(',');
                    foreach (string policyItem in policyItems)
                    {
                        htPolicyItem.Add(policyItem, policyItem);
                    }
                    var itemUid = policyItems[0];
                    var policyItemRow = _iExamPolicyItemRep.Get(new Guid(itemUid));
                    if (policyItemRow != null)
                    {
                        createPolicyNodeUid = policyItemRow.policyNodeUid;
                    }
                    else
                    {
                        createPolicyItemUids = null;
                    }
                }

                paperName = examPolicyRow.policyName;
                folerUid = examPolicyRow.folderUid;

                var examPaperRow = new Exam.ExamPaper();
                var examPaperNodeRowCollection = new List<ExamPaperNode>();
                var examPaperNodeQuestionRowCollection = new List<ExamPaperNodeQuestion>();
                var examQuestionRowCollection = new List<ExamQuestionDto>();
                System.Random random = new System.Random(unchecked((int)DateTime.Now.Ticks));

                //创建试卷的基本信息
                examPaperRow.Id = Guid.NewGuid();
                examPaperRow.paperCode = examPolicyRow.policyCode;
                examPaperRow.policyUid = examPolicyRow.Id;
                examPaperRow.paperName = paperName;
                examPaperRow.isSingleAsMulti = examPolicyRow.isSingleAsMulti;
                examPaperRow.eachOptionScore = examPolicyRow.eachOptionScore;
                examPaperRow.paperTypeCode = "fix_from_random";
                examPaperRow.paperClassCode = examPolicyRow.paperClassCode;
                examPaperRow.isShowScore = examPolicyRow.isShowScore;
                examPaperRow.paperHardGrade = examPolicyRow.paperHardGrade;
                examPaperRow.outdatedDate = examPolicyRow.outdatedDate;
                examPaperRow.courseUid = examPolicyRow.courseUid;
                examPaperRow.createTime = DateTime.Now;
                examPaperRow.questionNum = 0;
                examPaperRow.totalScore = 0;
                var dtPolicyNode = _iExamPolicyNodeRep.GetAll().Where(a => a.policyUid == policyUid).OrderBy(a => a.listOrder).ToList();
                int thisNodeQuestionCount = 0;
                int currentContentIndex = 0;
                foreach (ExamPolicyNode examPolicyNodeRow in dtPolicyNode)
                {
                    #region 开始一大题

                    if (!string.IsNullOrEmpty(createPolicyItemUids))
                    {
                        if (examPolicyNodeRow.Id != createPolicyNodeUid)
                        {
                            continue;
                        }
                    }

                    //本大题试题计数清0
                    thisNodeQuestionCount = 0;

                    //创建试题的大题
                    var examPaperNodeRow = new ExamPaperNode();
                    examPaperNodeRow.Id = Guid.NewGuid();
                    examPaperNodeRow.paperUid = examPaperRow.Id;
                    examPaperNodeRow.questionTypeUid = examPolicyNodeRow.questionTypeUid;
                    examPaperNodeRow.paperNodeName = examPolicyNodeRow.policyNodeName;
                    examPaperNodeRow.paperNodeDesc = examPolicyNodeRow.policyNodeDesc;
                    examPaperNodeRow.listOrder = examPolicyNodeRow.listOrder;
                    examPaperNodeRowCollection.Add(examPaperNodeRow);
                    var dtPolicyItem = _iExamPolicyItemRep.GetAll().Where(a => a.policyNodeUid == examPolicyNodeRow.Id).OrderBy(a => a.listOrder).ToList();


                    foreach (ExamPolicyItem examPolicyItemRow in dtPolicyItem)
                    {

                        if (!string.IsNullOrEmpty(createPolicyItemUids))
                        {
                            if (!htPolicyItem.ContainsKey(examPolicyItemRow.Id))
                            {
                                continue;
                            }
                        }

                        decimal policyItemQuestionScore = examPolicyItemRow.questionScore;
                        var dtQuestion = GetQuestionViewByExamPolicyItem(examPolicyItemRow, examPolicyRow.courseUid);
                        int totalQuestionNum = dtQuestion.Count; //（大题下）待抽取试题集中试题的总数
                        int createdQuestionNum = 0;
                        string createdQuestionUids = "";

                        bool isMakeRandom = false;  //是否需要再打乱顺序（当抽题总题数跟所要的数量一样多时）

                        #region 开始取题号
                        int questionNum = examPolicyItemRow.questionNum;
                        ExamQuestion drQuestion;
                        var questionUid = Guid.Empty;
                        if (totalQuestionNum < questionNum)
                        {
                            retValue.HasError = true;
                            retValue.Message = "抽题失败，引起的原因是题库中试题不足．";
                            return retValue;
                        }
                        else if (questionNum == -1 || totalQuestionNum == questionNum)  //题目数为-1表示全部题目,总题数跟所要的数量一样多,则取全部
                        {
                            if (totalQuestionNum == questionNum)
                            {
                                //总题数跟所要的数量一样多时，再随机打乱试题，否则会每次都取一样的顺序
                                isMakeRandom = true;
                            }
                            for (int k = 0; k < totalQuestionNum; k++)
                            {
                                currentContentIndex = k;
                                drQuestion = dtQuestion[currentContentIndex];
                                questionUid = drQuestion.Id;
                                if (("," + allCreatedQuestionUids + ",").IndexOf("," + questionUid + ",") == -1)	//所有试题编号中都没有，则加上来
                                {
                                    allCreatedQuestionUids = allCreatedQuestionUids + questionUid + ",";
                                    createdQuestionUids = createdQuestionUids + questionUid + ",";
                                    createdQuestionNum = createdQuestionNum + 1;
                                }
                            }
                        }
                        else
                        {
                            int thisCreateTimes = 0;    //本次抽题次数,超过1000000则强行退出
                            while (createdQuestionNum < questionNum)
                            {
                                currentContentIndex = random.Next(0, totalQuestionNum);
                                drQuestion = dtQuestion[currentContentIndex];
                                questionUid = drQuestion.Id;
                                if (("," + allCreatedQuestionUids + ",").IndexOf("," + questionUid + ",") == -1)	//所有试题编号中都没有，则加上来
                                {
                                    allCreatedQuestionUids = allCreatedQuestionUids + questionUid + ",";
                                    createdQuestionUids = createdQuestionUids + questionUid + ",";
                                    createdQuestionNum = createdQuestionNum + 1;
                                }
                                thisCreateTimes += 1;
                                if (thisCreateTimes > 1000000)
                                {
                                    retValue.HasError = true;
                                    retValue.Message = ("抽题失败，引起的原因是抽取的试题过多或题库中试题不足．");
                                    return retValue;
                                };
                            } //结束while
                        }
                        #endregion

                        #region 把本策略项的题目加进来
                        string[] arrCreatedQuestionUids = createdQuestionUids.Split(',');

                        //=============把试题ID的字符串数组随机打乱=================
                        if (isMakeRandom == true)
                            arrCreatedQuestionUids = RandomSort(arrCreatedQuestionUids);

                        foreach (string createdQuestionUid in arrCreatedQuestionUids)
                        {
                            questionUid = string.IsNullOrEmpty(createdQuestionUid) ? Guid.Empty : Guid.Parse(createdQuestionUid);
                            string thisQuestionBaseTypeCode = string.Empty;
                            if (questionUid == Guid.Empty) continue;
                            var examQuestion = dtQuestion.FirstOrDefault(a => a.Id == questionUid);
                            if (examQuestion == null)
                                continue;
                            var examQuestionRow = examQuestion.MapTo<ExamQuestionDto>();
                            examQuestionRowCollection.Add(examQuestionRow);

                            //获取该试题的默认分数，目的是折算组合题中每个小题的分数
                            sourceQuestionScore = examQuestionRow.score;
                            thisQuestionBaseTypeCode = examQuestionRow.questionBaseTypeCode;


                            if (policyItemQuestionScore > 0)
                                examQuestionRow.score = policyItemQuestionScore;   //如果设定了新分数
                            if (thisQuestionBaseTypeCode != EnumQuestionBaseTypeCode.Compose) //判断是否是组合题,不是组合题才加题数
                            {
                                examPaperNodeRow.totalScore += examQuestionRow.score;       //大题分数累加
                                examPaperNodeRow.questionNum += 1;
                            }

                            ////如果是分题考试或打字题都要时间
                            //if (examDoMode == EnumExamDoModeCode.Question || thisQuestionBaseTypeCode == EnumQuestionBaseTypeCode.Typing)
                            //{
                            //    //如果定义了时限，则用该时限,否则用原来的
                            //    if (examPolicyItemRow.ExamTime > 0) examQuestionRow.ExamTime = examPolicyItemRow.ExamTime;
                            //}
                            //else
                            //{
                            //    //不是一题一题考且不是打字题则清除时限
                            //    examQuestionRow.ExamTime = 0;
                            //}
                            /*
                             * 如果有时间,都把它们显示了出来.
                             * */
                            if (examPolicyItemRow.examTime > 0) examQuestionRow.examTime = examPolicyItemRow.examTime;

                            //添加大题与题目的联系
                            var examPaperNodeQuestionRow = new ExamPaperNodeQuestion();
                            examPaperNodeQuestionRow.paperNodeUid = examPaperNodeRow.Id;
                            examPaperNodeQuestionRow.questionUid = examQuestionRow.Id;
                            examPaperNodeQuestionRow.paperUid = examPaperNodeRow.paperUid;
                            examPaperNodeQuestionRow.paperQuestionScore = examQuestionRow.score;
                            examPaperNodeQuestionRow.paperQuestionExamTime = examQuestionRow.examTime;
                            thisNodeQuestionCount += 1;
                            examPaperNodeQuestionRow.listOrder = thisNodeQuestionCount;
                            examPaperNodeQuestionRowCollection.Add(examPaperNodeQuestionRow);

                            //======处理组合题的子试题=========
                            #region 处理组合题的子试题
                            if (thisQuestionBaseTypeCode == EnumQuestionBaseTypeCode.Compose) //判断是否是组合题
                            {
                                //Step 1:查找当前组合题下的子试题列表.条件：组合题ID：questionUid,

                                var dtSubQuestion =
                                    _iExamQuestionRep.GetAll().Where(a => a.parentQuestionUid == examQuestionRow.Id).ToList();
                                //Step 2:将子试题内容逐个加入到试卷中。
                                int subQuestionCount = dtSubQuestion.Count();
                                for (int nSub = 0; nSub < subQuestionCount; nSub++)
                                {
                                    var subExamQuestionRow = dtSubQuestion[nSub].MapTo<ExamQuestionDto>();
                                    examQuestionRowCollection.Add(subExamQuestionRow);

                                    if (policyItemQuestionScore > 0)
                                    {
                                        decimal subOldQuestionScore = subExamQuestionRow.score;
                                        if (sourceQuestionScore > 0)
                                        {
                                            subExamQuestionRow.score = policyItemQuestionScore * subOldQuestionScore / sourceQuestionScore;
                                        }
                                        else
                                        {
                                            subExamQuestionRow.score = policyItemQuestionScore / subQuestionCount;
                                        }
                                    }
                                    examPaperNodeRow.totalScore += subExamQuestionRow.score;       //大题分数累加
                                    examPaperNodeRow.questionNum += 1;

                                    //组合题的子试题不要每题时限
                                    subExamQuestionRow.examTime = 0;

                                    examPaperNodeQuestionRow = new ExamPaperNodeQuestion();
                                    examPaperNodeQuestionRow.paperUid = examPaperNodeRow.paperUid;
                                    examPaperNodeQuestionRow.paperNodeUid = examPaperNodeRow.Id;
                                    examPaperNodeQuestionRow.questionUid = subExamQuestionRow.Id;
                                    examPaperNodeQuestionRow.paperQuestionScore = subExamQuestionRow.score;
                                    thisNodeQuestionCount += 1;
                                    examPaperNodeQuestionRow.listOrder = thisNodeQuestionCount;
                                    examPaperNodeQuestionRowCollection.Add(examPaperNodeQuestionRow);

                                }
                                //遗留问题：总题数（影响答卷检查），组合题的分数分配
                            }
                            #endregion
                            //======//处理组合题的子试题=========
                        }
                        #endregion //结束把本策略项的题目加进来
                    }
                    examPaperRow.totalScore += examPaperNodeRow.totalScore;       //试卷分数累加
                    examPaperRow.questionNum += examPaperNodeRow.questionNum;
                    #endregion      //结束一大题

                }

                //折算分数
                decimal paperTotalScore = examPaperRow.totalScore;
                if (examPolicyRow.isConvertScore == "Y" && paperTotalScore > 0 && examPolicyRow.totalScore > 0)
                {
                    convertedTotalScore = examPolicyRow.totalScore;
                    convertScoreRate = convertedTotalScore / paperTotalScore;

                    for (int i = 0; i < examPaperNodeQuestionRowCollection.Count; i++)
                    {
                        examPaperNodeQuestionRowCollection[i].paperQuestionScore = examPaperNodeQuestionRowCollection[i].paperQuestionScore * convertScoreRate;
                    }

                    //每大题分数
                    for (int i = 0; i < examPaperNodeRowCollection.Count; i++)
                    {
                        examPaperNodeRowCollection[i].totalScore = examPaperNodeRowCollection[i].totalScore * convertScoreRate;
                    }
                    //试卷总分
                    examPaperRow.totalScore = convertedTotalScore;
                }


                retValue.PutValue("examPaperRow", examPaperRow);
                retValue.PutValue("examPaperNodeRowCollection", examPaperNodeRowCollection);
                retValue.PutValue("examPaperNodeQuestionRowCollection", examPaperNodeQuestionRowCollection);
                retValue.PutValue("examQuestionRowCollection", examQuestionRowCollection);
                return retValue;
            }
            catch (Exception)
            {

                return null;
            }
        }

        /// <summary>
        /// 通过抽题策略取得试题列表
        /// </summary>
        /// <param name="examPolicyItemRow"></param>
        /// <param name="courseUid"></param>
        /// <returns></returns>
        private List<ExamQuestion> GetQuestionViewByExamPolicyItem(ExamPolicyItem examPolicyItemRow, Guid courseUid)
        {
            Expression<Func<ExamQuestion, bool>> expression = a => true;
            if (courseUid != Guid.Empty)
                expression = expression.And(a => a.courseUid == courseUid);
            expression = expression.And(a => a.questionStatusCode == "normal");
            expression = expression.And(a => a.parentQuestionUid == Guid.Empty);

            // 添加新试题时去掉了过期时间，试题的过期全部手动控制状态太判断，不再根据过期时间修改试题状态
            //var dateTime = DateTimeUtil.Now;
            //expression = expression.And(a => (dateTime <= a.outdated_date || a.outdated_date == 0));

            if (examPolicyItemRow.questionTypeUid != Guid.Empty)
                expression = expression.And(a => a.questionTypeUid == examPolicyItemRow.questionTypeUid);
            if (!string.IsNullOrEmpty(examPolicyItemRow.folderUid)) //有分类的根据分类过滤
            {
                expression = expression.And(a => examPolicyItemRow.folderUid.Contains(a.folderUid.ToString()));
            }
            //排除屏蔽的子分类(等加)
            if (examPolicyItemRow.hardGrade != "0" && !string.IsNullOrEmpty(examPolicyItemRow.hardGrade)) expression = expression.And(a => a.hardGrade.Equals(examPolicyItemRow.hardGrade));
            var questions = _iExamQuestionRep.GetAll().Where(expression).Join(_iExamQuestionTypeRep.GetAll(), q => q.questionTypeUid, t => t.Id, (q, t) => q).ToList();

            return questions;
        }

        /// <summary>
        /// 通过考生答案XML组装成DataTable
        /// </summary>
        /// <param name="userAnswerXML"></param>
        /// <returns></returns>
        private List<ExamAnswer> GetExamAnswerTableByXML(string userAnswerXML)
        {
            List<ExamAnswer> dtUserAnswer = new List<ExamAnswer>();


            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                //如果答案是空的直接返回,这里不检查是否是合法标准的考试答案xml，
                //如果不是应该在前端进行处理，这里处理不经济
                if (string.IsNullOrEmpty(userAnswerXML)) return dtUserAnswer;
                xmlDocument.LoadXml(userAnswerXML);
            }
            catch (Exception e)
            {
                return dtUserAnswer;
            }


            XmlNodeList xmlNodeList = xmlDocument.DocumentElement.SelectNodes("exam_answers/exam_answer");

            string examGradeUid = "";
            XmlNode tempXmlNode = null;

            tempXmlNode = xmlDocument.DocumentElement.SelectSingleNode("exam_grade_uid");
            if (tempXmlNode != null) examGradeUid = tempXmlNode.InnerText;

            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                XmlNode xmlNode = xmlNodeList[i];

                string questionUid = "";
                tempXmlNode = xmlNode.SelectSingleNode("question_uid");
                if (tempXmlNode != null) questionUid = tempXmlNode.InnerText;

                string answerText = "";
                tempXmlNode = xmlNode.SelectSingleNode("answer_text");
                if (tempXmlNode != null) answerText = tempXmlNode.InnerXml;

                int answerTime = 0;
                tempXmlNode = xmlNode.SelectSingleNode("answer_time");
                if (tempXmlNode != null) answerTime = ConvertUtil.ToInt(tempXmlNode.InnerText);

                decimal judgeScore = 0;
                tempXmlNode = xmlNode.SelectSingleNode("judge_score");
                if (tempXmlNode != null) judgeScore = ConvertUtil.ToDecimal(tempXmlNode.InnerText);

                string judgeResultCode = "";
                tempXmlNode = xmlNode.SelectSingleNode("judge_result_code");
                if (tempXmlNode != null) judgeResultCode = tempXmlNode.InnerText;

                string judgeRemarks = "";
                tempXmlNode = xmlNode.SelectSingleNode("judge_remarks");
                if (tempXmlNode != null) judgeRemarks = tempXmlNode.InnerText;

                string judgeScoreText = "";
                tempXmlNode = xmlNode.SelectSingleNode("judge_score_text");
                if (tempXmlNode != null) judgeScoreText = tempXmlNode.InnerText;

                string judgeUserUid = "";
                tempXmlNode = xmlNode.SelectSingleNode("judge_user_uid");
                if (tempXmlNode != null) judgeUserUid = tempXmlNode.InnerText;
                string judgeUserName = "";
                tempXmlNode = xmlNode.SelectSingleNode("judge_user_name");
                if (tempXmlNode != null) judgeUserName = tempXmlNode.InnerText;

                ExamAnswer answer = new ExamAnswer();
                answer.examGradeUid = new Guid(examGradeUid.Trim());
                answer.questionUid = new Guid(questionUid.Trim());
                answer.answerText = answerText;
                answer.answerTime = answerTime;
                answer.judgeScore = judgeScore;
                answer.judgeResultCode = judgeResultCode.Trim();
                answer.judgeRemarks = judgeRemarks;
                answer.judgeScoreText = judgeScoreText;
                if (!string.IsNullOrEmpty(judgeUserUid))
                answer.judgeUserUid = new Guid(judgeUserUid);
                else
                {
                    answer.judgeUserUid = Guid.Empty;
                }
                answer.judgeUserName = judgeUserName;
                dtUserAnswer.Add(answer);
            }
            return dtUserAnswer;
        }

        /// <summary>
        /// 把试题ID随机打乱
        /// </summary>
        /// <param name="questionList"></param>
        /// <returns></returns>
        private string[] RandomSort(string[] questionList)
        {
            int len = questionList.Length;
            List<int> list = new List<int>();
            string[] ret = new string[len];
            Random rand = new Random();
            int i = 0;
            while (list.Count < len)
            {
                int iter = rand.Next(0, len);
                if (!list.Contains(iter))
                {
                    list.Add(iter);
                    ret[i] = questionList[iter];
                    i++;
                }

            }
            return ret;
        }

        private string ReplaceHtml(string html)
        {
            string pattern = @"(?isx)

                      <({0})\b[^>]*>                  #开始标记“<tag...>”

                          (?>                         #分组构造，用来限定量词“*”修饰范围

                              <\1[^>]*>  (?<Open>)    #命名捕获组，遇到开始标记，入栈，Open计数加1

                          |                           #分支结构

                              </\1>  (?<-Open>)       #狭义平衡组，遇到结束标记，出栈，Open计数减1

                          |                           #分支结构

                              (?:(?!</?\1\b).)*       #右侧不为开始或结束标记的任意字符

                          )*                          #以上子串出现0次或任意多次

                          (?(Open)(?!))               #判断是否还有'OPEN'，有则说明不配对，什么都不匹配

                      </\1>                           #结束标记“</tag>”

                     ";

            return Regex.Replace(html, string.Format(pattern, Regex.Escape("td")), m => m.Value.Replace("<br/>", ""));
        }

        /// <summary>
        /// 过滤成符合条件的
        /// </summary>
        /// <param name="examPaperNodeQuestionRowCollection"></param>
        /// <param name="dtAnswer"></param>
        /// <param name="dtQuestion"></param>
        /// <param name="filterJudgeResultCode"></param>
        private void FilterQuestion(ref List<ExamPaperNodeQuestion> examPaperNodeQuestionRowCollection, List<ExamAnswer> dtAnswer, List<ExamQuestion> dtQuestion, string filterJudgeResultCode)
        {
            var dtCopyQuestion = dtQuestion; //用于判断组合体子试题是否全部被清空  
            for (int i = 0; i < examPaperNodeQuestionRowCollection.Count; i++)
            {
                var questionUid = examPaperNodeQuestionRowCollection[i].questionUid;
                var userAnswers = dtAnswer.FirstOrDefault(a => a.questionUid == questionUid);
                var question = dtQuestion.FirstOrDefault(a => a.Id == questionUid);
                if (question==null)
                    continue;
                if (question.questionBaseTypeCode == EnumQuestionBaseTypeCode.Compose)
                    continue;
                if (userAnswers != null)
                {

                    if (filterJudgeResultCode == EnumJudgeResultCode.Error)
                    {
                        if (string.IsNullOrEmpty(userAnswers.answerText) || userAnswers.judgeResultCode == EnumJudgeResultCode.Right)
                        {
                            examPaperNodeQuestionRowCollection.RemoveAt(i);
                            if (dtCopyQuestion.Contains(question))
                                dtCopyQuestion.Remove(question);
                            i = i - 1;
                        }
                    }
                    else if (filterJudgeResultCode == EnumJudgeResultCode.Right)
                    {
                        if (string.IsNullOrEmpty(userAnswers.answerText) || userAnswers.judgeResultCode == EnumJudgeResultCode.Error)
                        {
                            examPaperNodeQuestionRowCollection.RemoveAt(i);
                            if (dtCopyQuestion.Contains(question))
                                dtCopyQuestion.Remove(question);
                            i = i - 1;
                        }
                    }
                    else if (filterJudgeResultCode == "Empty")
                    {
                        //if (hidIsLoadNoAnswerQuestion.Value != "Y")
                        //{
                        if (!string.IsNullOrEmpty(userAnswers.answerText))
                        {
                            examPaperNodeQuestionRowCollection.RemoveAt(i);
                            if (dtCopyQuestion.Contains(question))
                                dtCopyQuestion.Remove(question);
                            i = i - 1;
                        }
                        //}
                        //else
                        //{
                        //    if (!string.IsNullOrEmpty(userAnswers.answer_text) || userAnswers.judge_score < 0)
                        //    {
                        //        examPaperNodeQuestionRowCollection.RemoveAt(i);

                        //        i = i - 1;
                        //    }
                        //}
                    }
                    else if (filterJudgeResultCode == "NoAnswer")
                    {
                        if (!string.IsNullOrEmpty(userAnswers.answerText) || userAnswers.judgeScore >= 0)
                        {
                            examPaperNodeQuestionRowCollection.RemoveAt(i);
                            if (dtCopyQuestion.Contains(question))
                            dtCopyQuestion.Remove(question);
                            i = i - 1;
                        }
                    }
                }
                else
                {
                    if (filterJudgeResultCode == EnumJudgeResultCode.Right || filterJudgeResultCode == EnumJudgeResultCode.Error || filterJudgeResultCode == "NoAnswer")		//如果选答对的或错误的，则没有答的也要去掉
                    {
                        examPaperNodeQuestionRowCollection.RemoveAt(i);
                        if (dtCopyQuestion.Contains(question))
                            dtCopyQuestion.Remove(question);
                        i = i - 1;
                    }
                }
            }

            //过滤组合题,如果组合题的子试题都被过滤了,则大题也不显示
            for (int i = 0; i < examPaperNodeQuestionRowCollection.Count; i++)
            {
                var questionUid = examPaperNodeQuestionRowCollection[i].questionUid;

                var question = dtQuestion.FirstOrDefault(a => a.Id.Equals(questionUid));
                if (question.questionBaseTypeCode == EnumQuestionBaseTypeCode.Compose)
                {
                    var questions = dtCopyQuestion.Count(a => a.parentQuestionUid.Equals(questionUid));
                   
                    if (questions==0)
                    {
                        examPaperNodeQuestionRowCollection.RemoveAt(i);
                        i = i - 1;
                    }
                }
            }
        }
        #endregion
    }

}