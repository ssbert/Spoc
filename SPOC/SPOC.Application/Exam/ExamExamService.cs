using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Specifications;
using Abp.UI;
using Castle.Core.Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using newv.common;
using SPOC.Common;
using SPOC.Common.Cookie;
using SPOC.Common.Exam;
using SPOC.Common.Extensions;
using SPOC.Common.File;
using SPOC.Common.Helper;
using SPOC.Common.Http;
using SPOC.Exam.CloudDto;
using SPOC.Exam.Dto;
using SPOC.ExamPaper;
using SPOC.ExamPaper.Dto;
using SPOC.QuestionBank.Dto;
using SPOC.SqlExecuter;
using SPOC.SysSetting;
using SPOC.User;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Json;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;
using Abp.Collections.Extensions;
using SPOC.Lib;
using SPOC.SystemSet;
using ConvertUtil = SPOC.Common.Helper.ConvertUtil;
using ReturnValue = SPOC.Common.ReturnValue;
using StringUtil = SPOC.Common.Helper.StringUtil;

namespace SPOC.Exam
{
    public class ExamExamService : SPOCAppServiceBase, IExamExamService
    {
        private readonly IRepository<ExamExam, Guid> _iExamExamRep;
        private readonly IRepository<TeacherInfo, Guid> _iTeacherInfoRep;
        private readonly IRepository<ExamGrade, Guid> _iExamGradeRep;
        private readonly IRepository<UserBase, Guid> _iUserBaseRepository;
        private readonly IRepository<ExamJudgePolicy, Guid> _iExamJudgePolicyRepository;
        private readonly IRepository<ExamJudge, Guid> _iExamJudgeRepository;
        private readonly ISqlExecuter _sqlExecuter;
        private readonly IRepository<Cloud, Guid> _iCloudRep;
        private readonly IRepository<ExamPaper, Guid> _iExamPaperRep;
        private readonly IRepository<ExamPaperNode, Guid> _iExamPaperNodeRep;
        private readonly IRepository<ExamPaperNodeQuestion, Guid> _iExamPaperNodeQuestionRep;
        private readonly IRepository<ExamPolicy, Guid> _iExamPolicyRepository;
        private readonly IRepository<ExamPolicyItem, Guid> _iExamPolicyItemRep;
        private readonly IRepository<ExamPolicyNode, Guid> _iExamPolicyNodeRep;
        private readonly IRepository<ExamQuestion, Guid> _iExamQuestionRep;
        private readonly IRepository<ExamExamPaper, Guid> _iExamExamPaperRep;
        private readonly IRepository<ExamUserAnswer, Guid> _iExamUserAnswerRep;
        private readonly IRepository<ExamAnswer, Guid> _iExamAnswerRep;
        private readonly IRepository<ExamProgramResult, Guid> _iExamProgramRep;
        private readonly IRepository<QuestionLabel, Guid> _iQuestionLabelRep;
        private readonly IRepository<ExamPolicyItemLabel, Guid> _iExamPolicyItemLabelRep;

        private readonly IExamPaperService _iExamPaperService;
        private readonly ISiteSetService _iSiteSetService;
        private readonly ILibLabelService _iLibLabelService;
        private readonly IUnitOfWorkManager _iUnitOfWorkManager;

        
        public ExamExamService(IRepository<ExamExam, Guid> iExamExamRep, IRepository<TeacherInfo, Guid> iTeacherInfoRep,
            IRepository<ExamGrade, Guid> iExamGradeRep, IRepository<UserBase, Guid> iUserBaseRepository,
            IRepository<ExamJudgePolicy, Guid> iExamJudgePolicyRepository, ISqlExecuter sqlExecuter,
            IRepository<ExamPaper, Guid> iExamPaperRep, IRepository<ExamPolicyItem, Guid> iExamPolicyItemRep,
            IRepository<ExamPolicy, Guid> iExamPolicyRepository, IRepository<ExamPolicyNode, Guid> iExamPolicyNodeRep,
            IRepository<ExamQuestion, Guid> iExamQuestionRep, IRepository<ExamPaperNode, Guid> iExamPaperNodeRep,
            IRepository<ExamPaperNodeQuestion, Guid> iExamPaperNodeQuestionRep, IUnitOfWorkManager iUnitOfWorkManager,
            IRepository<ExamExamPaper, Guid> iExamExamPaperRep, IExamPaperService iExamPaperService,
            IRepository<ExamUserAnswer, Guid> iExamUserAnswerRep, IRepository<ExamAnswer, Guid> iExamAnswerRep,
            IRepository<ExamJudge, Guid> iExamJudgeRepository, IRepository<ExamProgramResult, Guid> iExamProgramRep, 
            ISiteSetService iSiteSetService, ILibLabelService iLibLabelService, IRepository<Cloud, Guid> iCloudRep, 
            IRepository<QuestionLabel, Guid> iQuestionLabelRep, IRepository<ExamPolicyItemLabel, Guid> iExamPolicyItemLabelRep)
        {
            _iExamExamRep = iExamExamRep;
            _iTeacherInfoRep = iTeacherInfoRep;
            _iExamGradeRep = iExamGradeRep;
            _iUserBaseRepository = iUserBaseRepository;
            _iExamJudgePolicyRepository = iExamJudgePolicyRepository;
            _sqlExecuter = sqlExecuter;
            _iExamPaperRep = iExamPaperRep;
            _iExamPolicyItemRep = iExamPolicyItemRep;
            _iExamPolicyRepository = iExamPolicyRepository;
            _iExamPolicyNodeRep = iExamPolicyNodeRep;
            _iExamQuestionRep = iExamQuestionRep;
            _iExamPaperNodeRep = iExamPaperNodeRep;
            _iExamPaperNodeQuestionRep = iExamPaperNodeQuestionRep;
            _iUnitOfWorkManager = iUnitOfWorkManager;
            _iExamExamPaperRep = iExamExamPaperRep;
            _iExamPaperService = iExamPaperService;
            _iExamUserAnswerRep = iExamUserAnswerRep;
            _iExamAnswerRep = iExamAnswerRep;
            _iExamJudgeRepository = iExamJudgeRepository;
            _iExamProgramRep = iExamProgramRep;
            _iSiteSetService = iSiteSetService;
            _iLibLabelService = iLibLabelService;
            _iCloudRep = iCloudRep;
            _iQuestionLabelRep = iQuestionLabelRep;
            _iExamPolicyItemLabelRep = iExamPolicyItemLabelRep;
        }

        #region 增删改查

        public async Task<ExamExamOutputDto> Get(Guid id)
        {
            var queryable = _iExamExamRep.GetAll().Where(a => a.Id == id)
                .Select(a => new { exam = a, a.Paper.paperName }).FirstOrDefault();
            var output = queryable.exam.MapTo<ExamExamOutputDto>();
            output.paperName = queryable.paperName;
            output.BeginTime = queryable.exam.BeginTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
            output.EndTime = queryable.exam.EndTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
            return await Task.FromResult(output);
        }

        public async Task<ExamExamOutputDto> Create(ExamExamInputDto input)
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
                if (string.IsNullOrEmpty(input.ExamCode))
                {
                    throw new UserFriendlyException("自定义考试编号不能为空");
                }

                if (_iExamExamRep.GetAll().Any(a => a.ExamCode == input.ExamCode))
                {
                    throw new UserFriendlyException("已有相同的考试编号");
                }
            }
            #endregion

            input.Id = Guid.NewGuid();
            input.createTime = input.modifyTime = DateTime.Now;
            input.creatorUid = CookieHelper.GetLoginInUserInfo().Id;
            if (!input.isCustomCode)
            {
                input.ExamCode = CreateNewCode();
            }

            if (_iExamExamRep.GetAll().Any(a => a.TaskId == input.TaskId && a.examTypeCode == "exam_normal"))
            {
                input.examTypeCode = "exam_retest";
            }

            var exam = input.MapTo<ExamExam>();
            await _iExamExamRep.InsertAsync(exam);

            var paperName =
                _iExamPaperRep.GetAll()
                    .Where(a => a.Id == exam.paperUid)
                    .Select(a => a.paperName)
                    .FirstOrDefault();

            var paperTypeCode =
                _iExamPaperRep.GetAll()
                    .Where(a => a.Id == input.paperUid)
                    .Select(a => a.paperTypeCode)
                    .FirstOrDefault();

            if (paperTypeCode == "random")
            {
                var bufferPaperNum = exam.bufferPaperNum ?? 0;
                var existPaperNum = _iExamExamPaperRep.GetAll().Count(a => a.examUid == exam.Id);
                //如果小于缓存份数则生成
                if (existPaperNum < bufferPaperNum)
                {
                    CreateFixPaper(bufferPaperNum - existPaperNum, exam.paperUid, exam.Id);
                }
            }
            else
            {
                await _iExamPaperService.BuidExamPaper(exam.paperUid);
            }

            var output = exam.MapTo<ExamExamOutputDto>();
            output.paperName = paperName;
            return await Task.FromResult(output);
        }

        public async Task Update(ExamExamInputDto input)
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

            var entity = await _iExamExamRep.FirstOrDefaultAsync(input.Id);
            if (entity == null)
            {
                throw new UserFriendlyException("无效试题信息");
            }

            if (input.isCustomCode)
            {
                if (string.IsNullOrEmpty(input.ExamCode))
                {
                    throw new UserFriendlyException("自定义考试编号不能为空");
                }

                if (_iExamExamRep.GetAll().Any(a => a.ExamCode == input.ExamCode && a.Id != input.Id))
                {
                    throw new UserFriendlyException("已有相同的考试编号");
                }
            }

            if (entity.isCustomCode && !input.isCustomCode)
            {
                input.ExamCode = CreateNewCode();
            }
            #endregion

            var paperUid = entity.paperUid;
            input.MapTo(entity);

            await _iExamExamRep.UpdateAsync(entity);

            var paperTypeCode =
                _iExamPaperRep.GetAll()
                    .Where(a => a.Id == input.paperUid)
                    .Select(a => a.paperTypeCode)
                    .FirstOrDefault();

            if (paperTypeCode == "random")
            {
                if (entity.paperTypeCode == "random" && entity.paperUid != paperUid)
                {
                    DeleteExamExamPaperByExamUid(entity.Id);
                }

                var bufferPaperNum = entity.bufferPaperNum ?? 0;
                var existPaperNum = _iExamExamPaperRep.GetAll().Count(a => a.examUid == entity.Id);
                //如果小于缓存份数则生成
                if (existPaperNum < bufferPaperNum)
                {
                    CreateFixPaper(bufferPaperNum - existPaperNum, entity.paperUid, entity.Id);
                }
            }
            else
            {
                await _iExamPaperService.BuidExamPaper(entity.paperUid);
            }
        }

        public async Task Delete(string ids)
        {
            var idArray = ids.Split(',');
            var entityList = new List<ExamExam>();
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

            foreach (var idStr in idArray)
            {
                var id = new Guid(idStr);
                var entity = _iExamExamRep.Get(id);
                if (entity == null)
                {
                    throw new UserFriendlyException("无效的考试信息");
                }
                entityList.Add(entity);
            }
            #endregion

            foreach (var entity in entityList)
            {
                await _iExamExamRep.DeleteAsync(entity);
            }
        }

        public async Task SetExamTypeCodeNormal(Guid id)
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
            var entity = _iExamExamRep.FirstOrDefault(id);
            if (entity == null)
            {
                throw new UserFriendlyException("无效试题信息");
            }
            #endregion


            var list = _iExamExamRep.GetAll().Where(a => a.TaskId == entity.TaskId && a.examTypeCode == "exam_normal").ToList();
            list.ForEach(a =>
            {
                a.examTypeCode = "exam_retest";
                a.IsExamination = "Y";
                _iExamExamRep.UpdateAsync(a);
            });

            entity.examTypeCode = "exam_normal";
            entity.IsExamination = "N";
            await _iExamExamRep.UpdateAsync(entity);
        }
       
        #endregion

        #region 前台考试
        public async Task<ExamExam> GetExamInfo(Guid examUid)
        {
            var exam = await _iExamExamRep.FirstOrDefaultAsync(a => a.Id.Equals(examUid));
            return exam;
        }

        public async Task<string> GetUserExamData(string examUid, string userUid)
        {
            var examId = Guid.Parse(examUid);
            var userId = Guid.Parse(userUid);
            var jsonBuilder = new StringBuilder();
            try
            {
                string sysSetting = GetSysSetting();
                string userExamTimes = GetUserExamTimes(examUid, userUid);
                var nosubmitGrade =
                    await
                        _iExamGradeRep.FirstOrDefaultAsync(
                            a => a.examUid == examId && a.userUid == userId && (a.gradeStatusCode == "examing" || a.gradeStatusCode == "pause"));
                string userNosubmitGradeUid = nosubmitGrade == null ? "" : nosubmitGrade.Id.ToString();
                if (string.IsNullOrEmpty(userNosubmitGradeUid)) userNosubmitGradeUid = "";
                jsonBuilder.Append("{");
                jsonBuilder.Append("\"sysSetting\":" + sysSetting + ",");
                jsonBuilder.Append("\"userExamTimes\":" + userExamTimes + ",");
                jsonBuilder.Append("\"nosubmitGradeUid\":\"" + userNosubmitGradeUid + "\"");
                jsonBuilder.Append("}");

            }
            catch (Exception e)
            {
                Logger.Error(e.ToString);
                return "";
            }
            //解决北京诺姆四达人力资源测评咨询服务有限公司提示解析数据错误
            return jsonBuilder.ToString().Replace("\r\n", "<br/>");
        }

        private string GetSysSetting()
        {

            var jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{");
            jsonBuilder.Append("\"IsCheckExamTimesBeforeExam\":" + "true" + ","); //
            jsonBuilder.Append("\"IsLockKeybordWhenCloseExam\":\"" + "N" + "\","); //启用增强型防作弊机制
            jsonBuilder.Append("\"IsShowPhotoWhenExamine\":" + "false,"); //显示照片
            jsonBuilder.Append("\"IsOnlyUploadFileWhenExamining\":" + "false,"); //启用增强型防作弊机制
            jsonBuilder.Append("\"AlertLeftTimeWhenExaming\":" + (58 * 60).ToString("0") + ","); //考试结束提醒时间
            jsonBuilder.Append("\"ControllerRefreshSecond\":" + 30.ToString("0") + ","); //考试监控台刷新时间 30
            jsonBuilder.Append("\"ExamAnswerSavePath\":\"" + "" + "\",");
            jsonBuilder.Append("\"IsForceJoinQuestionnaireAfterSubmitPaper\":" + "false" + ","); //提交答卷后强制参加问卷调查
            jsonBuilder.Append("\"QuestionProtectionLevel\":\"" + "low" + "\","); //题库保护等级
            jsonBuilder.Append("\"FileServerWebRootPath\":\"" + AppConfiguration.FileServerWebRootPath + "/\",");
            jsonBuilder.Append("\"EnableClientIndividuation\":" + "false" + ","); //启用客户端个性化设置
            jsonBuilder.Append("\"ComputeExamTimeTypeCode\":\"" + "usagetime" + "\","); //考试计时方式
            jsonBuilder.Append("\"IsLoginOut\":\"" + "N" + "\",");
            jsonBuilder.Append("\"AlterMessageWhenCreatingPaper\":\"" + "" + "\"");

            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }

        /// <summary>
        /// 考试次数
        /// </summary>
        /// <param name="examUid"></param>
        /// <param name="examUserUid"></param>
        /// <returns></returns>
        private string GetUserExamTimes(string examUid, string examUserUid)
        {
            try
            {
                var examId = Guid.Parse(examUid);
                var examUserId = Guid.Parse(examUserUid);
                var userAttendTimes = _iExamGradeRep.Count(b => b.examUid == examId && b.userUid == examUserId && !string.IsNullOrEmpty(b.gradeStatusCode));
                //获取
                var examTimes =
                    _iExamGradeRep.GetAll().Where(a => a.examUid == examId && a.userUid == examUserId).Select(a => new
                    {
                        a.examUid,
                        a.isExamination,
                        attend_times = userAttendTimes

                    }).ToList();
                int[] userExamTimes = { 0, 0 };
                foreach (var examTime in examTimes)
                {
                    string isExamination = examTime.isExamination;
                    var attendTimes = examTime.attend_times;

                    if (isExamination != "Y")
                        userExamTimes[0] = attendTimes;
                    else
                        userExamTimes[1] = attendTimes;
                }

                var jsonBuilder = new StringBuilder();
                jsonBuilder.Append("{");
                int[] userAttendExamTimes = userExamTimes;
                jsonBuilder.Append("\"exam\":" + userAttendExamTimes[0] + ",\"reexam\":" + userAttendExamTimes[1]);
                jsonBuilder.Append("}");

                return jsonBuilder.ToString();

            }
            catch (Exception ex)
            {
                return "\"\"";
            }
        }

        public async Task<string> InitAttendExam(string examUid, string userUid)
        {
            try
            {
                var retValue = GetAttenExam(examUid, userUid);
                if (retValue.HasError == true)
                {
                    return BuilderJson(true, retValue.Message, "-1");
                }
                //试卷模板
                #region 试卷模板
                string paperTemplatePath = string.Empty;
                string customizeScript = string.Empty;
                paperTemplatePath = "fileroot/paperTemplate/ExamPaper/ExaminingPaper.xsl";
                paperTemplatePath = "~/" + paperTemplatePath;
                if (!string.IsNullOrEmpty(customizeScript))
                    customizeScript = "~/" + customizeScript;

                #endregion
                //返回数据
                //返回数据
                var examExamRow = (ExamExam)retValue.GetValue("ExamExamRow");
                var examGradeRow = (ExamGrade)retValue.GetValue("ExamGradeRow");
                var examPaperRow = (ExamPaper)retValue.GetValue("ExamPaperRow");
                var exampaper = examPaperRow.MapTo<ExamPaperOutputDto>();
                var examGrade = examGradeRow.MapTo<ExamGradeOutputDto>();
                if (examGradeRow.userAnswerUid != Guid.Empty)
                {
                    //取考试答案写入examGrade对象中
                    var examUserAnswer = _iExamUserAnswerRep.FirstOrDefault(examGradeRow.userAnswerUid);
                    examGrade.userAnswer = examUserAnswer != null ? examUserAnswer.userAnswer : "";
                }
                //var paperTemplateText = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath(paperTemplatePath), Encoding.UTF8); ;
                //把路径前缀加进去
                exampaper.paperXml = FilePathUtil.GetContentTextWithFilePath(examPaperRow.paperXml, AppConfiguration.FileServerFileWebPathRoot + "/");
                var doMainUrl = HttpContext.Current.Request.Url.Host;
                //exampaper.paperExtend01 = paperTemplateText;
                var jsonExamGrade = JsonUtil.JsonSerializer(examGrade);
                var jsonExamPaper = JsonUtil.JsonSerializer(exampaper);
                var jsonExamExam = new JavaScriptSerializer().Serialize(examExamRow.MapTo<ExamExamOutputDto>());
                var site = _iSiteSetService.GetAllSiteSet();
                StringBuilder jsonBuilder = new StringBuilder();
                jsonBuilder.Append("{");
                jsonBuilder.Append("\"ExamGrade\":" + jsonExamGrade + ",");
                jsonBuilder.Append("\"ExamPaper\":" + jsonExamPaper + ",");
                jsonBuilder.Append("\"ExamExam\":" + jsonExamExam + ",");
                jsonBuilder.Append("\"AllowPasteCode\":" + site.allowPasteCode + ",");
                //这里合成html 合成的html必须用javascript编码再整合到json中
                jsonBuilder.Append("\"PaperHtml\":\"" + HttpUtility.JavaScriptStringEncode(XlstHepler.Convert2XHtml(examUid, examPaperRow.Id.ToString(), HttpContext.Current.Server.MapPath(paperTemplatePath), examPaperRow.paperXml, examExamRow, examGradeRow)) + "\",");

                //用户信息

                var user = await _iUserBaseRepository.FirstOrDefaultAsync(new Guid(userUid));
                jsonBuilder.Append("\"UserInfo\":{\"UserName\":\"" + (user != null ? (string.IsNullOrEmpty(user.userFullName) ? user.userLoginName : user.userFullName) : "") + "\",");
                jsonBuilder.Append("\"LoginName\":\"" + "" + "\",");
                jsonBuilder.Append("\"OfficeDTFileWebPath\":\"" + string.Format("ExamAnswer/Exam_{0}/ExamGrade_{1}", examUid, examGradeRow.Id) + "\",");
                jsonBuilder.Append("\"WebPath\":\"" + string.Format("{0}/fileservice/FileUpload.aspx?FilePath=", doMainUrl) + "\",");
                jsonBuilder.Append("\"WebServerUrl\":\"" + string.Format("{0}/Exam", doMainUrl) + "\",");
                //if (ExamAppSetting.IsShowPhotoWhenExamine == true)
                //{
                //    string sPhotoFile = GetUserPhoto();
                //    if (FileManager.IsFileExist(sPhotoFile))
                //    {
                //        sPhotoFile = AppConfiguration.FileServerFileWebPathRoot + "/" + Server.UrlEncode(sPhotoFile);
                //    }
                //    else
                //    {
                //        sPhotoFile = "../../fileroot/no_user_photo.jpg";
                //    }
                //    jsonBuilder.Append("\"PhotoFile\":\"" + sPhotoFile + "\",");
                //}
                jsonBuilder.Append("\"UserCode\":\"" + user.userIdcard + "\"");
                jsonBuilder.Append("},");


                jsonBuilder.Append("\"PaperTemplate\":\"" + "../../fileroot/paperTemplate/ExamPaper/ExaminingPaper.xsl".Replace("/", @"\/") + "\",");
                jsonBuilder.Append("\"CustomizeScript\":\"" + customizeScript.Replace("/", @"\/") + "\",");

                //评卷策略
                var dtInfo = _iExamJudgePolicyRepository.GetAll().Where(a => a.examUid.Equals(examExamRow.Id)).ToList();
                jsonBuilder.Append("\"QuestionJudgePolicy\":{");
                if (dtInfo.Count == 0)
                {
                    jsonBuilder.AppendLine("\"norecord\":\"\"");
                }
                else
                {
                    foreach (var examJudgePolicy in dtInfo)
                    {
                        if (dtInfo.IndexOf(examJudgePolicy) == dtInfo.Count - 1)
                            jsonBuilder.AppendLine("\"" + examJudgePolicy.questionTypeUid + "\":\"" + examJudgePolicy.judgePolicyCode + "|" + examJudgePolicy.parameter + "\",");
                        else
                        {
                            jsonBuilder.AppendLine("\"" + examJudgePolicy.questionTypeUid + "\":\"" + examJudgePolicy.judgePolicyCode + "|" + examJudgePolicy.parameter + "\"");
                        }
                    }

                }
                jsonBuilder.AppendLine("}");
                jsonBuilder.Append("}");
                return jsonBuilder.ToString();

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message,ex);
                return "";
            }
        }

        /// <summary>
        /// 获取参加考试的考试信息
        /// </summary>
        /// <param name="examUid"></param>
        /// <param name="examUserUid"></param>
        /// <returns></returns>
        private ReturnValue GetAttenExam(string examUid, string examUserUid)
        {
            try
            {
                var reValue = new ReturnValue(false, "");
                var exam = _iExamExamRep.FirstOrDefault(new Guid(examUid));
                if (exam == null)
                {
                    reValue.HasError = true;
                    reValue.Message = ("找不到考试信息！");
                    return reValue;
                }
               
                //验证并初始化考试
                reValue = CheckUserAttendExam(examUserUid, exam);
                if (reValue.HasError)
                    return reValue;
                var isNewExamGrade = true;
                ExamPaper examPaperRow = null;
                var examGradeRow = reValue.GetValue("ExamGradeRow") as ExamGrade;
                if (examGradeRow != null)
                    isNewExamGrade = false;

                #region MySqlParameter参数
                var ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                //==获取分期==//
                var dateTime = DateTime.Now;


                var paras = new MySqlParameter[4];
                paras[0] = new MySqlParameter("?userUid", MySqlDbType.VarChar) { Value = examUserUid };
                paras[1] = new MySqlParameter("?examUid", MySqlDbType.VarChar) { Value = examUid };
                paras[2] = new MySqlParameter("?examGradeUid", MySqlDbType.VarChar) { Value = examGradeRow == null ? "" : examGradeRow.Id.ToString() };
                paras[3] = new MySqlParameter("?attendIP", MySqlDbType.VarChar) { Value = ipAddress };
                #endregion

                var gradePaperUid = Guid.Empty;
                var result =
                       _sqlExecuter.SqlQuery<ExamGrade>(
                            "call proInitUserExam(?userUid,?examUid,?examGradeUid,?attendIP,@gradePaperUid)",
                            paras).ToList().FirstOrDefault();
                // gradePaperUid = db.Database.SqlQuery<string>("select @gradePaperUid").FirstOrDefault();

                if (result == null)
                {
                    reValue.HasError = true;
                    reValue.Message = ("提示：初始化考生成绩失败！");
                    return reValue;
                }
                gradePaperUid = Guid.Empty.Equals(result.paperUid) ? Guid.Empty : result.paperUid;
                examGradeRow = result.MapTo<ExamGrade>();

                if (gradePaperUid == Guid.Empty
                    || (exam.paperTypeCode == "random" && exam.bufferPaperNum <= 0))
                {
                    var retValue = CreatePaperByPolicy(exam.paperUid);
                    if (retValue.HasError == true)
                    {
                        return retValue;
                    }

                    //插入试卷对象
                    if (!InsertPaperObject(retValue))
                    {
                        reValue.HasError = true;
                        reValue.Message = ("提示：插入试卷对象到数据库失败！");
                        return retValue;

                    }
                    examPaperRow = (ExamPaper)retValue.GetValue("examPaperRow");

                    //插入考试与试卷的关联信息
                    var examExamPaperRow = new ExamExamPaper();
                    examExamPaperRow.Id = Guid.NewGuid();
                    examExamPaperRow.examUid = new Guid(examUid);
                    examExamPaperRow.paperUid = examPaperRow.Id;
                    examExamPaperRow.createTime = DateTime.Now;
                    examExamPaperRow.isActive = "Y";
                    _iExamExamPaperRep.Insert(examExamPaperRow);

                    examGradeRow.paperUid = examPaperRow.Id;
                    examGradeRow.paperTotalScore = examPaperRow.totalScore;
                    _iExamGradeRep.Update(examGradeRow);

                    _iUnitOfWorkManager.Current.SaveChanges();
                }
                else
                {
                    examPaperRow = _iExamPaperRep.FirstOrDefault(a => a.Id.Equals(gradePaperUid));
                    if (examPaperRow == null)
                    {
                        reValue.HasError = true;
                        reValue.Message = ("提示：您当前不能参加该考试，因为考试设置中没有指定试卷！");
                        return reValue;
                    }
                }

                //2008/1/11Nick 过度型代码,如果没有生成XML则要生成一下.
                if (examPaperRow.paperXml == "")
                {

                    _iExamPaperService.BuidExamPaper(examPaperRow.Id);
                    examPaperRow = _iExamPaperRep.FirstOrDefault(a => a.Id.Equals(examPaperRow.Id));

                }
                else  //防止xml文件损坏 重新生成xml
                {
                    try
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(examPaperRow.paperXml);

                    }
                    catch
                    {
                        _iExamPaperService.BuidExamPaper(examPaperRow.Id);
                        examPaperRow = _iExamPaperRep.FirstOrDefault(a => a.Id.Equals(examPaperRow.Id));
                    }
                }

                //检查考试时间策略，如果策略是允许参考时间。那么 考试时长=考试结束时间-当前时间
                var allowExamTime = exam.examTime;
                var examTime = 0;
                if (exam.examTimeModule == "end_exam")
                {
                    var now = DateTime.Now;
                    if (exam.EndTime > now)
                    {
                        TimeSpan ts = now.Subtract(exam.EndTime ?? now).Duration();
                        examTime = (int)ts.TotalSeconds;
                        if (examTime < allowExamTime)
                        {
                            examGradeRow.allowExamTime = examTime;
                        }
                    }
                }

                reValue.PutValue("ExamExamRow", exam);
                reValue.PutValue("ExamGradeRow", examGradeRow);
                reValue.PutValue("ExamPaperRow", examPaperRow);
                return reValue;




            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                return new ReturnValue(true, ex.Message);
            }
        }

        /// <summary>
        /// 检查用户是否允许参加指定的考试
        /// </summary>
        /// <param name="userUid"></param>
        /// <param name="examExamRow"></param>
        /// <returns></returns>
        private ReturnValue CheckUserAttendExam(string userUid, ExamExam examExamRow)
        {
            var reValue = new ReturnValue(false, "");
            dynamic examGradeRow = null;
            try
            {
                if (examExamRow.BeginTime != null && examExamRow.EndTime != null)
                {
                    //检查考试时间
                    if (examExamRow.BeginTime > DateTime.Now)
                    {
                        reValue.HasError = true;
                        reValue.Message = "提示：您参加的考试未到开始时间，请稍候！";
                        return reValue;
                    }
                    if (examExamRow.EndTime < DateTime.Now)
                    {
                        reValue.HasError = true;
                        reValue.Message = "提示：考试时间已过，不能参加考试！";
                        return reValue;
                    }
                    //检查禁止时间
                    if (examExamRow.forbidTime != 0)
                    {
                        if (DateTimeUtil.Now > DateTimeUtil.ConvertToUnixTime((DateTime)examExamRow.BeginTime) + examExamRow.forbidTime)
                        {
                            //if (DateTime.Now > (examArrangeRow.beginTime ?? DateTime.Now).AddSeconds(examExamRow.forbidTime??0))
                            //{
                            //添加是否异常关闭考试系统造成的交卷，导致再次进入时候提示超时。
                            var noSubmitGrade =
                                _iExamGradeRep.FirstOrDefault(
                                    a =>
                                        a.examUid.Equals(examExamRow.Id) && a.userUid.Equals(new Guid(userUid)) &&
                                        a.gradeStatusCode.Equals("examing"));

                            if (noSubmitGrade == null)
                            {
                                reValue.HasError = true;
                                reValue.Message = "提示：您未在规定的时间进入考场，目前已不能参加考试！";
                                return reValue;
                            }
                        }
                    }
                }
                //存储过程查询是否允许进入考试
                //定义参数
                #region MySqlParameter参数
                var paras = new MySqlParameter[3];
                paras[0] = new MySqlParameter("?pUserUid", MySqlDbType.VarChar) { Value = userUid };
                paras[1] = new MySqlParameter("?pExamUid", MySqlDbType.VarChar) { Value = examExamRow.Id.ToString() };
                paras[2] = new MySqlParameter("?isForbitExamWhenPass", MySqlDbType.VarChar) { Value = "N" };
                #endregion
                var result = _sqlExecuter.SqlQuery<CheckUserExtendExam>(
                    "call proCheckUserExtendExam(?pUserUid,?pExamUid,?isForbitExamWhenPass,@examGradeUid,@returnCode)",
                    paras).ToList();
                var examGradeUid = Guid.Empty;
                var returnCode = "-9";

                if (result.Count > 0)
                {
                    var examGradeId = result[0].ExamGradeUid;
                    examGradeUid = string.IsNullOrEmpty(examGradeId) ? Guid.Empty : Guid.Parse(examGradeId);
                    returnCode = result[0].ReturnCode;
                }
                switch (returnCode)
                {
                    case "0":
                        break;
                    case "-1":
                        reValue.HasError = true;
                        reValue.Message = "提示：您参加的考试未到开始时间，请稍候！";
                        break;
                    case "-2":
                        reValue.HasError = true;
                        reValue.Message = "提示：考试时间已过，不能参加考试！";
                        break;
                    case "-3":
                        reValue.HasError = true;
                        reValue.Message = "提示：您未在规定的时间进入考场，目前已不能参加考试！";
                        break;
                    case "-4":
                        reValue.HasError = true;
                        reValue.Message = "提示：您未被安排参加本次考试！";
                        break;
                    case "-5":
                        reValue.HasError = true;
                        reValue.Message = "提示：您上次未成功交卷，请在未提交答卷列表中找到该份答卷，补交后再重新参加！";
                        break;
                    case "-6":
                        reValue.HasError = true;
                        reValue.Message = "提示：您当前不能参加该考试，因为已参加的次数已经达到最大允许次数！";
                        break;
                    case "-7":
                        reValue.HasError = true;
                        reValue.Message = "您已通过该考试,不能再参加该考试,请退出该系统!";
                        break;
                    case "-8":
                        reValue.HasError = true;
                        reValue.Message = "提示：您当前不能参加该考试，因为考试设置中没有指定试卷！";
                        break;
                    case "-9":
                        reValue.HasError = true;
                        reValue.Message = "提示：初始化考试失败！请稍后重试";
                        break;
                }

                if (reValue.HasError)
                    return reValue;


                if (examGradeUid != Guid.Empty)
                    examGradeRow = _iExamGradeRep.FirstOrDefault(a => a.Id == examGradeUid);
                reValue.HasError = false;
                reValue.PutValue("ExamGradeRow", examGradeRow);
            }
            catch (Exception ex)
            {
                Logger.Error("初始化考试出错：" + ex.ToString());
                reValue.HasError = true;
                reValue.Message = ex.Message;
            }

            return reValue;
        }

        public string BuilderJson(bool hasError, string message, string errorCode = "0", Dictionary<string, string> list = null)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{");
            jsonBuilder.Append("\"hasError\":" + hasError.ToString().ToLower() + ",");
            jsonBuilder.Append("\"errorCode\":" + errorCode + ",");
            jsonBuilder.Append("\"message\":\"" + message + "\"");
            if (list != null && list.Count > 0)
            {
                jsonBuilder.Append(",");
                foreach (string key in list.Keys)
                {
                    jsonBuilder.Append("\"" + key + "\":\"" + list[key] + "\",");
                }
                jsonBuilder.Length = jsonBuilder.Length - 1;
            }
            jsonBuilder.Append("}");

            return jsonBuilder.ToString();
        }

        /// <summary>
        /// 根据策略产生试卷
        /// </summary>
        /// <param name="policyUid"></param>
        /// <returns>ReturnValue的ReturnObject中返回PaperObject</returns>

        #region CreatePaperByPolicy
        public ReturnValue CreatePaperByPolicy(Guid policyUid)
        {
            return CreatePaperByPolicy(policyUid, null);
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

                string paperName = string.Empty;
                var folerUid = Guid.Empty;
                string allCreatedQuestionUids = string.Empty;

                var examPolicyRow = _iExamPolicyRepository.FirstOrDefault(a => a.Id == policyUid);

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
                    var policyItemRow = _iExamPolicyItemRep.FirstOrDefault(a => a.Id.Equals(new Guid(itemUid)));
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

                var examPaperRow = new ExamPaper();
                var examPaperNodeRowCollection = new List<ExamPaperNode>();
                var examPaperNodeQuestionRowCollection = new List<ExamPaperNodeQuestion>();
                var examQuestionRowCollection = new List<ExamQuestionDto>();
                System.Random random = new System.Random(unchecked((int)DateTime.Now.Ticks));

                //创建试卷的基本信息
                examPaperRow.Id = Guid.NewGuid();
                examPaperRow.folderUid = examPolicyRow.folderUid;
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
                examPaperRow.subjectUid = examPolicyRow.subjectUid;
                examPaperRow.departmentUid = examPolicyRow.departmentUid;
                examPaperRow.creatorUid = examPolicyRow.creatorUid;
                examPaperRow.lastUpdateTime = examPaperRow.createTime;
                var dtPolicyNode = _iExamPolicyNodeRep.GetAll().Where(a => a.policyUid.Equals(policyUid)).OrderBy(a => a.listOrder).ToList();
                int thisNodeQuestionCount = 0;
                int currentContentIndex = 0;
                foreach (ExamPolicyNode examPolicyNodeRow in dtPolicyNode)
                {
                    #region 开始一大题

                    if (!string.IsNullOrEmpty(createPolicyItemUids))
                    {
                        if (!examPolicyNodeRow.Id.Equals(createPolicyNodeUid))
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
                    var dtPolicyItem = _iExamPolicyItemRep.GetAll().Where(a => a.policyNodeUid.Equals(examPolicyNodeRow.Id)).OrderBy(a => a.listOrder).ToList();


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
                        string questionUid = string.Empty;
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
                                questionUid = drQuestion.Id.ToString();
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
                                questionUid = drQuestion.Id.ToString();
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
                            questionUid = createdQuestionUid;
                            string thisQuestionBaseTypeCode = string.Empty;
                            if (questionUid == "") continue;
                            var examQuestion = dtQuestion.FirstOrDefault(a => a.Id.Equals(new Guid(questionUid)));
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
                                examPaperNodeRow.totalScore += policyItemQuestionScore;       //大题分数累加
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
                            examPaperNodeQuestionRow.Id = Guid.NewGuid();
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
                                    _iExamQuestionRep.GetAllList(a => a.parentQuestionUid == examQuestionRow.Id);
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
                                    examPaperNodeQuestionRow.Id = Guid.NewGuid();
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
        /// 把试题ID随机打乱
        /// </summary>
        /// <param name="questionList"></param>
        /// <returns></returns>
        public static string[] RandomSort(string[] questionList)
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

        /// <summary>
        /// 通过抽题策略取得试题列表
        /// </summary>
        /// <param name="examPolicyItemRow"></param>
        /// <param name="courseUid"></param>
        /// <returns></returns>
        public List<ExamQuestion> GetQuestionViewByExamPolicyItem(ExamPolicyItem examPolicyItemRow, Guid courseUid)
        {
            var questionIdList = (from itemLabel in _iExamPolicyItemLabelRep.GetAll()
                join questionLabel in _iQuestionLabelRep.GetAll() on itemLabel.LabelId equals questionLabel.labelId
                where itemLabel.ItemId == examPolicyItemRow.Id && questionLabel.labelType == 1
                select questionLabel.questionId).ToList();
            Expression<Func<ExamQuestion, bool>> expression = a => true;
            
            if (courseUid != Guid.Empty)
            {
                expression = expression.And(a => a.courseUid.Equals(courseUid));
            }
            expression = expression.And(a => a.questionStatusCode.Equals("normal"));
            expression = expression.And(a => a.parentQuestionUid == Guid.Empty);

            //根据知识点筛选
            if (_iExamPolicyItemLabelRep.GetAll().Any(a=>a.ItemId == examPolicyItemRow.Id))
            {
                expression = expression.And(a => questionIdList.Contains(a.Id));
            }

            // 添加新试题时去掉了过期时间，试题的过期全部手动控制状态太判断，不再根据过期时间修改试题状态
            //var dateTime = DateTimeUtil.Now;
            //expression = expression.And(a => (dateTime <= a.outdated_date || a.outdated_date == 0));

            if (examPolicyItemRow.questionTypeUid != Guid.Empty)
            {
                expression = expression.And(a => a.questionTypeUid == examPolicyItemRow.questionTypeUid);
            }
            if (!string.IsNullOrEmpty(examPolicyItemRow.folderUid)) //有分类的根据分类过滤
            {
                expression = expression.And(a => examPolicyItemRow.folderUid.Contains(a.folderUid.ToString()));
            }
            //排除屏蔽的子分类(等加)
            if (examPolicyItemRow.hardGrade != "0" && !string.IsNullOrEmpty(examPolicyItemRow.hardGrade))
            {
                expression = expression.And(a => a.hardGrade.Equals(examPolicyItemRow.hardGrade));
            }
            //因有外键questionTypeUid，后面的join语句感觉没什么用，先注释掉了
            var questions = _iExamQuestionRep.GetAll().Where(expression).ToList();//Join(_iExamQuestionTypeRep.GetAll(), q => q.questionTypeUid, t => t.id, (q, t) => q).ToList();
            return questions;
        }

        #endregion

        /// <summary>
        /// 插入试卷对象到数据库中
        /// </summary>
        /// <param name="retValue"></param>
        /// <returns></returns>

        private bool InsertPaperObject(ReturnValue retValue)
        {
            try
            {
                //试卷信息
                var examPaperRow = (ExamPaper)retValue.GetValue("examPaperRow");
                var examPaperNodeRowCollection = (List<ExamPaperNode>)retValue.GetValue("examPaperNodeRowCollection");
                var examPaperNodeQuestionRowCollection = (List<ExamPaperNodeQuestion>)retValue.GetValue("examPaperNodeQuestionRowCollection");

                //如果是随机生成的固定试卷则不要保存分类
                //if (examPaperRow.paperTypeCode == "fix_from_random") examPaperRow.folderUid = Guid.Empty;
                examPaperRow.createTime = DateTime.Now;
                _iExamPaperRep.Insert(examPaperRow);
                foreach (ExamPaperNode row in examPaperNodeRowCollection)
                {
                    _iExamPaperNodeRep.Insert(row);
                }
                foreach (ExamPaperNodeQuestion row in examPaperNodeQuestionRowCollection)
                {
                    _iExamPaperNodeQuestionRep.Insert(row);
                }
                _iUnitOfWorkManager.Current.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
        
        #region  提交考试到服务器
        public async Task<string> SaveAnswerToServer(ExamGradeInputDto examGradeInputDto)
        {
            try
            {
                examGradeInputDto.userAnswer = HttpUtility.UrlDecode(examGradeInputDto.userAnswer);
                var examGradeRow = await _iExamGradeRep.FirstOrDefaultAsync(a => a.Id == examGradeInputDto.examGradeUid);
                if (examGradeRow == null)
                {
                    return BuilderJson(true, ("找不到成绩记录，无法保存。"), "1");
                }
                var examExamRow = await _iExamExamRep.FirstOrDefaultAsync(a => a.Id.Equals(examGradeRow.examUid));
                if (examExamRow == null)
                {
                    return BuilderJson(true, ("找不到考试记录，无法保存。"), "1");
                }

                if (examGradeRow.gradeStatusCode == "pause")
                {
                    return BuilderJson(true, ("考试处理暂停中，不能保存答卷。"), "1");
                }
                if (examGradeRow.gradeStatusCode != "examing")
                {
                    return BuilderJson(true, string.Format(("您的答卷已不是处于考试中状态，您不能再保存答卷。")), "-1");
                }
                if (examGradeInputDto.userAnswer != "")
                {
                    var examTime = (examGradeRow.examTime ?? 0) + DateTimeUtil.Now -
                                   newv.common.DateTimeUtil.ConvertToUnixTime(examGradeRow.lastUpdateTime);
                    examGradeRow.examTime = (int)examTime;
                    examGradeRow.lastUpdateTime = DateTime.Now;
                    var userAnswerModel = await _iExamUserAnswerRep.FirstOrDefaultAsync(examGradeRow.userAnswerUid);
                    if (userAnswerModel == null) //创建一条记录
                    {
                        userAnswerModel = new ExamUserAnswer
                        {
                            Id = Guid.NewGuid(),
                            userAnswer = examGradeInputDto.userAnswer
                        };
                        await _iExamUserAnswerRep.InsertAsync(userAnswerModel);
                    }
                    else
                    {
                        userAnswerModel.userAnswer = examGradeInputDto.userAnswer;
                    }
                    if ((examExamRow.gateNum ?? 0) > 0)
                    {

                        string userAnswerXML = userAnswerModel.userAnswer;
                        if (!string.IsNullOrEmpty(userAnswerXML))
                        {
                            UpdateUserAnswerXml(userAnswerXML, examGradeInputDto.userAnswer);
                        }
                    }
                    examGradeRow.userAnswerUid = userAnswerModel.Id;

                }
                _iUnitOfWorkManager.Current.SaveChanges();
                return BuilderJson(false, "答卷已保存。");

            }
            catch (Exception ex)
            {
                return BuilderJson(true, "服务器保存答卷失败！请保存答卷稍后重试或联系管理员" + ex.Message);
            }
        }

        private string UpdateUserAnswerXml(string oldXml, string newXml)
        {
            XmlDocument oldXmlDocument = new XmlDocument();
            XmlDocument newXmlDodument = new XmlDocument();
            try
            {
                oldXmlDocument.LoadXml(oldXml);
                newXmlDodument.LoadXml(newXml);
            }
            catch (Exception e)
            {
                return "";
            }

            if (oldXmlDocument.DocumentElement != null && newXmlDodument != null)
            {

                XmlNodeList xmlOldNodeList = oldXmlDocument.DocumentElement.SelectNodes("exam_answers/exam_answer");       //取出原XML文件中所有答案
                XmlNodeList xmlNewNodeList = newXmlDodument.DocumentElement.SelectNodes("exam_answers/exam_answer");       //取出新XML文件中所有答案


                ArrayList oldNodeQuestionUidList = new ArrayList();
                for (int i = 0; i < xmlOldNodeList.Count; i++)
                {
                    //取出原XML文件的试题 Uid
                    if (!oldNodeQuestionUidList.Contains(xmlOldNodeList[i].FirstChild.InnerText))
                    {
                        oldNodeQuestionUidList.Add(xmlOldNodeList[i].FirstChild.InnerText);
                    }
                }

                bool hasUpdateQuestionAnswer = false; //是否更新了考生试题答案，如果有更新那么需要重新计算考生成绩
                for (int i = 0; i < xmlNewNodeList.Count; i++)
                {
                    string questionUid = xmlNewNodeList[i].FirstChild.InnerText;
                    XmlNode examAnswersNode = oldXmlDocument.DocumentElement.SelectNodes("exam_answers")[0];      //查找<exam_answers> 
                    if (oldNodeQuestionUidList.Contains(questionUid))
                    {
                        //如果当前答案已存在，则删除并插入新的答案
                        XmlNode deleteNode = examAnswersNode.SelectSingleNode("exam_answer[question_uid='" + questionUid + "']");
                        if (deleteNode != null)
                        {
                            examAnswersNode.RemoveChild(deleteNode);
                            hasUpdateQuestionAnswer = true; //此处更新了考生试题答案
                        }
                    }

                    //插入节点
                    XmlElement examAnswerElement = oldXmlDocument.CreateElement("exam_answer");

                    XmlElement questionUidElement = oldXmlDocument.CreateElement("question_uid");
                    questionUidElement.InnerText = ConvertUtil.ToString(questionUid, "");
                    examAnswerElement.AppendChild(questionUidElement);

                    XmlElement answerTextElement = oldXmlDocument.CreateElement("answer_text");
                    answerTextElement.InnerText = ConvertUtil.ToString(xmlNewNodeList[i].ChildNodes[1].InnerText, "");
                    examAnswerElement.AppendChild(answerTextElement);

                    XmlElement answerTimeElement = oldXmlDocument.CreateElement("answer_time");
                    answerTimeElement.InnerText = ConvertUtil.ToString(xmlNewNodeList[i].ChildNodes[2].InnerText, "");
                    examAnswerElement.AppendChild(answerTimeElement);

                    XmlElement judgeScoreElement = oldXmlDocument.CreateElement("judge_score");
                    judgeScoreElement.InnerText = ConvertUtil.ToString(xmlNewNodeList[i].ChildNodes[3].InnerText, "");
                    examAnswerElement.AppendChild(judgeScoreElement);

                    XmlElement judgeResultCodeElement = oldXmlDocument.CreateElement("judge_result_code");
                    judgeResultCodeElement.InnerText = ConvertUtil.ToString(xmlNewNodeList[i].ChildNodes[4].InnerText, "");
                    examAnswerElement.AppendChild(judgeResultCodeElement);

                    XmlElement judgeRemarksElement = oldXmlDocument.CreateElement("judge_remarks");
                    judgeRemarksElement.InnerText = ConvertUtil.ToString(xmlNewNodeList[i].ChildNodes[5].InnerText, "");
                    examAnswerElement.AppendChild(judgeRemarksElement);

                    examAnswersNode.AppendChild(examAnswerElement);

                }

                //更新考生成绩
                XmlNode oldGradeScoreNode = oldXmlDocument.DocumentElement.SelectNodes("grade_score")[0];
                if (hasUpdateQuestionAnswer)
                {
                    //遍历所有试题节点，更新考生成绩
                    XmlNodeList examAnswerList = oldXmlDocument.DocumentElement.SelectNodes("exam_answers/exam_answer");
                    decimal gradeScore = 0;
                    for (int i = 0; i < examAnswerList.Count; i++)
                    {
                        gradeScore += ConvertUtil.ToDecimal(examAnswerList[i].ChildNodes[3].InnerText, 0);
                    }
                    oldGradeScoreNode.InnerText = gradeScore.ToString();
                }
                else
                {
                    XmlNode newGradeScoreNode = newXmlDodument.DocumentElement.SelectNodes("grade_score")[0];
                    oldGradeScoreNode.InnerText = ConvertUtil.ToString(ConvertUtil.ToDecimal(oldGradeScoreNode.InnerText, 0) + ConvertUtil.ToDecimal(newGradeScoreNode.InnerText, 0));
                }
            }
            return oldXmlDocument.InnerXml;
        }


        public async Task<string> SubmitPaper(ExamGradeInputDto examGradeInputDto)
        {
            try
            {

                examGradeInputDto.userAnswer = System.Web.HttpUtility.UrlDecode(examGradeInputDto.userAnswer);
                string sMessage = string.Empty;
                if (examGradeInputDto.examGradeUid == Guid.Empty)
                {
                    return BuilderJson(true, ("参数丢失，请重新提交。"), "-1");

                }


                var examGradeRow = await _iExamGradeRep.FirstOrDefaultAsync(a => a.Id == examGradeInputDto.examGradeUid);
                if (examGradeRow == null)
                {

                    return BuilderJson(true, ("找不到成绩记录，无法保存。"), "1");
                }

                if (examGradeRow.gradeStatusCode != "pause" && examGradeRow.gradeStatusCode != "examing")
                {
                    return BuilderJson(true, ("答卷已经提交,无法再次提交，请关闭考试页面退出考试．"), "1");
                }
                //考生答案为空
                if (string.IsNullOrEmpty(examGradeInputDto.userAnswer))
                {

                    Dictionary<string, string> list = new Dictionary<string, string>();
                    list.Add("returnCode", "ForbitSubmit");
                    return BuilderJson(true, ("答卷记录为空，请保存后重新提交。"), "-1", list);
                }

                //程序题提交新课云
                try
                {
                    await SubmitProgramQuestionAnswer(examGradeRow, examGradeInputDto.userAnswer);
                }
                catch (Exception e)
                {
                    Logger.Error(e.ToString());
                    return BuilderJson(true, ("编译服务出现问题，请尝试重新提交。"), "-1");
                }

                //保存考生答案
                var examRow = await _iExamExamRep.FirstOrDefaultAsync(a => a.Id.Equals(examGradeRow.examUid));
                var userAnswerModel = await _iExamUserAnswerRep.FirstOrDefaultAsync(examGradeRow.userAnswerUid);
                if (userAnswerModel == null) //创建一条记录
                {
                    userAnswerModel = new ExamUserAnswer
                    {
                        Id = Guid.NewGuid(),
                        userAnswer = examGradeInputDto.userAnswer
                    };
                    await _iExamUserAnswerRep.InsertAsync(userAnswerModel);
                }

                if ((examRow.gateNum ?? 0) > 0)
                {

                    string userAnswerXML = "";
                    userAnswerXML = userAnswerModel.userAnswer;
                    if (userAnswerXML == "")
                    {
                        userAnswerModel.userAnswer = examGradeInputDto.userAnswer;
                    }
                    else
                    {
                        userAnswerXML = UpdateUserAnswerXml(userAnswerXML, examGradeInputDto.userAnswer);
                        userAnswerModel.userAnswer = userAnswerXML;
                    }
                }
                else
                {

                    userAnswerModel.userAnswer = examGradeInputDto.userAnswer;

                }



                var examTime = (examGradeRow.examTime ?? 0) + DateTimeUtil.Now -
                               newv.common.DateTimeUtil.ConvertToUnixTime(examGradeRow.lastUpdateTime);
                examGradeRow.examTime = (int)examTime;
                examGradeRow.lastUpdateTime = DateTime.Now;
                examGradeRow.userAnswerUid = userAnswerModel.Id;

                ReturnValue retValue = SubmitPaper(examGradeRow, true, userAnswerModel.userAnswer);
                if (retValue.HasError == true)
                {

                    Dictionary<string, string> list = new Dictionary<string, string>();
                    list.Add("returnCode", retValue.ErrorCode);
                    return BuilderJson(true, retValue.Message, "-1", list);
                }

                var examExamRow = (ExamExam)retValue.GetValue("ExamExamRow");

                bool canSeeGrade = (examExamRow.isAllowSeeGrade == "Y") ? true : false;
                //如果需要手工发布,则不显示成绩
                if (examExamRow.gradeReleaseType == "by_human")
                    canSeeGrade = false;

                //bool canSeeAnswer = (examExamRow.isAllowSeeAnswer == "Y") ? true : false;
                bool isNeedJudge = (examExamRow.isNeedJudge == "Y") ? true : false;
                bool isDisplayResult = (examExamRow.isDisplayResult == "Y") ? true : false;
                bool isAllowSeePaper = (examExamRow.isAllowSeePaper == "Y") ? true : false;

                decimal userScore = 0;

                string isPass = examGradeRow.isPass;

                DateTime currentDateTime = DateTime.Now;
                string publishGradeDate = examExamRow.publishGradeDate ?? "";
                userScore = examGradeRow.gradeScore ?? 0;
                string isAllowViewPaperNow = "N";
                if (isAllowSeePaper == true &&
                    (string.IsNullOrEmpty(publishGradeDate) ||
                     DateTimeUtil.ToDateTime(publishGradeDate) < currentDateTime))
                {
                    isAllowViewPaperNow = "Y";
                }
                else
                {
                    isAllowViewPaperNow = "N";
                }

                if (examGradeInputDto.isForceToSubmit == "true")
                {
                    sMessage += (examGradeInputDto.forceReasonMessage);
                }

                //如果是闯关竞赛则比较特殊
                if (examExamRow.examClassCode == "race")
                {
                    Dictionary<string, string> list = new Dictionary<string, string>();
                    list.Add("returnCode", retValue.ErrorCode);
                    list.Add("examClassCode", "race");
                    list.Add("passGateNum", examGradeRow.passGateNum.ToString());
                    return BuilderJson(false, retValue.Message, "0", list);
                }

                if (examGradeRow.paperTotalScore == 0)
                {
                    sMessage += ("您的答卷已经提交成功,感谢您的参与。");
                }
                else
                {
                    if (canSeeGrade && (string.IsNullOrEmpty(publishGradeDate) || DateTimeUtil.ToDateTime(publishGradeDate) < currentDateTime))
                    {
                        if (isNeedJudge == false)
                        {
                            sMessage += string.Format(("您的答卷已经提交成功，{0} 您的成绩是：{1:0.##}分，得分率是：{2:0.##}%。"),
                                examGradeInputDto.afterAnswerMessage, userScore, ((decimal)examGradeRow.gradeRate));
                            if (isDisplayResult)
                            {
                                if (isPass == "Y")
                                {
                                    sMessage = sMessage + ("恭喜您，您已通过本次考试。");
                                    string fixMessageTextAfterPassExam = "";
                                    if (!string.IsNullOrEmpty(examExamRow.passExamMessage))
                                        fixMessageTextAfterPassExam =
                                            FilePathUtil.GetContentTextWithFilePath(examExamRow.Id.ToString(),
                                                "exam", examExamRow.passExamMessage);
                                    if (fixMessageTextAfterPassExam != "")
                                    {
                                        sMessage = sMessage + fixMessageTextAfterPassExam; //加上其它考试完后的固定的提示
                                    }
                                }
                                else
                                {
                                    sMessage = sMessage + ("很遗憾，您没有通过本次考试。");
                                    string fixMessageTextAfterNoPassExam = "";
                                    if (!string.IsNullOrEmpty(examExamRow.noPassExamMessage))
                                        fixMessageTextAfterNoPassExam =
                                            FilePathUtil.GetContentTextWithFilePath(examExamRow.Id.ToString(),
                                                "exam", examExamRow.noPassExamMessage);
                                    if (fixMessageTextAfterNoPassExam != "")
                                    {
                                        sMessage = sMessage + fixMessageTextAfterNoPassExam; //加上其它考试完后的固定的提示
                                    }
                                }
                            }
                        }
                        else
                        {
                            sMessage +=
                                string.Format(("您的答卷已经提交成功，{0} 您所答客观题的成绩是：{1}分，得分率是：{2}%。 考卷还需手工评分，请留意评分结果。"),
                                    examGradeInputDto.afterAnswerMessage,
                                    ((decimal)examGradeRow.externalScore).ToString("0.##"),
                                    ((decimal)examGradeRow.gradeRate).ToString("0.##"));
                        }
                    }
                    else
                    {
                        sMessage += ("您的答卷已经提交成功，感谢您参与，成绩可能不会立即公布，请关注考试列表成绩刷新情况。");
                    }
                }
                _iUnitOfWorkManager.Current.SaveChanges(); //写入当前成绩  写学习成绩时需要统计最高分写入学习记录表

                Dictionary<string, string> msgList = new Dictionary<string, string>();
                msgList.Add("isAllowViewPaper", isAllowViewPaperNow);

                await UpdateUserLabelScoreByAnswerXml(examGradeRow.paperUid, examGradeRow.userUid, examGradeRow.Id,
                    userAnswerModel.userAnswer);

                return BuilderJson(false, sMessage, "0", msgList);

            }
            catch (Exception ex)
            {

                return BuilderJson(true, "服务器保存答卷失败！请保存答卷稍后重试或联系管理员" + ex.Message);
            }
        }

        

        /// <summary>
        /// 提交答案,更改考试状态,及对试卷进行判分
        /// </summary>
        /// <param name="examGradeRow"></param>
        /// <param name="isCheckForbitSubmitBeforeTime"></param>
        /// <param name="userAnswer">考生答题数据</param>
        /// <returns></returns>
        private ReturnValue SubmitPaper(ExamGrade examGradeRow, bool isCheckForbitSubmitBeforeTime, string userAnswer)
        {
            ReturnValue retValue = new ReturnValue(false, "");
            //=========1.获取相关数据===================   
            string gradeStatusCode = examGradeRow.gradeStatusCode;
            if (gradeStatusCode != "examing" && gradeStatusCode != "pause")
            {
                retValue.HasError = true;
                retValue.Message = ("该试卷已提交，无法再次提交！");
                return retValue;
            }
            var examExamRow = _iExamExamRep.FirstOrDefault(a => a.Id.Equals(examGradeRow.examUid));

            if (examExamRow == null)
            {
                retValue.HasError = true;
                retValue.Message = ("找不到考试记录，提交失败！");
                return retValue;
            }

            //设置了禁止提前提交时间
            if (isCheckForbitSubmitBeforeTime && (examExamRow.forbitSubmitBeforeTime ?? 0) > 0)
            {

                var examTime2 = (examGradeRow.examTime ?? 0) + DateTimeUtil.Now - DateTimeUtil.ConvertToUnixTime(examGradeRow.lastUpdateTime);
                if (examTime2 < (examExamRow.forbitSubmitBeforeTime ?? 0))
                {
                    retValue.HasError = true;
                    retValue.Message = string.Format(("答卷时间少于{0}分钟不允许提交试卷"), ConvertUtil.ToString(examExamRow.forbitSubmitBeforeTime / 60));
                    retValue.ErrorCode = "ForbitSubmit";
                    return retValue;
                }
            }

            if (examExamRow.isRealtimeSaveAnswerToDb == "Y")
            {
                retValue = SaveUserAnswerToDbFromXML(examGradeRow, userAnswer);
                if (retValue.HasError)
                {
                    examGradeRow.hasSaveAnswerToDb = "N";
                }
                else
                {
                    // retValue = ExamGradeTable.AutoJudgePaper(examExamRow, examGradeRow);
                }
            }
            else
            {
                examGradeRow.hasSaveAnswerToDb = "N"; //如果不直接生成答案统计数据, 把标记设置为 N
                var totalScore = ConvertUtil.ToDecimal(GetFirstNodeValueFromXml(userAnswer, "grade_score"), 0);

                decimal gradeRate = 0;
                var paperTotalScore = examGradeRow.paperTotalScore ?? 0;
                //判断最高得分限制
                var markPaperMaxScore = examExamRow.markPaperMaxScore ?? 0;
                if (markPaperMaxScore > 0)
                {
                    if (totalScore > markPaperMaxScore)
                    {
                        totalScore = markPaperMaxScore;
                    }
                    paperTotalScore = markPaperMaxScore;
                }

                //计算总成绩和通过状态               
                if (paperTotalScore > 0)
                {
                    gradeRate = totalScore * 100 / (decimal)paperTotalScore;
                }

                //==========2.改更成绩状态================
                examGradeRow.gradeScore = totalScore;
                examGradeRow.externalScore = totalScore;
                examGradeRow.gradeRate = Math.Round(gradeRate, 2, MidpointRounding.AwayFromZero);
                if (examExamRow.passGradeRate > 0)
                {
                    examGradeRow.isPass = (gradeRate >= examExamRow.passGradeRate) ? "Y" : "N";
                }
                else
                {
                    examGradeRow.isPass = (totalScore >= examExamRow.passGradeRate) ? "Y" : "N";
                }
            }

            //自动评分(如果实时保存到数据库才评)
            string isNeedJudge = examExamRow.isNeedJudge;
            //非竞赛模式
            if (isNeedJudge == "Y")
            {
                examGradeRow.gradeStatusCode = "submitted";
            }
            else
            {
                if (examExamRow.gradeReleaseType == "by_human")
                    examGradeRow.gradeStatusCode = "judged";
                else if (examExamRow.gradeReleaseType == "by_time")
                    examGradeRow.gradeStatusCode = "submitted";
                else
                    examGradeRow.gradeStatusCode = "release";
            }


            //=============3.更改时间=======================

            examGradeRow.endTime = examGradeRow.lastUpdateTime;
            var examTime = examGradeRow.examTime ?? 0;
            if (examTime == 0)
                examTime = (int)(DateTimeUtil.ConvertToUnixTime((DateTime)examGradeRow.endTime) - DateTimeUtil.ConvertToUnixTime(examGradeRow.beginTime));

            examGradeRow.examTime = examTime;
            examGradeRow.paperQuestionUids = "";
            examGradeRow.currentQuestionIndex = 0;

            //提取未答试题数
            if (!string.IsNullOrEmpty(userAnswer))
            {
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(userAnswer);
                    XmlNode xmlNode = xmlDoc.SelectSingleNode("exam_grade_object/no_answer_question_num");
                    if (xmlNode != null)
                    {
                        examGradeRow.noAnswerQuestionNum = ConvertUtil.ToInt(xmlNode.InnerText.Trim());
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("获取未答试题数失败：" + ex);
                }
            }
            _iExamGradeRep.Update(examGradeRow);
            //_unitOfWorkManager.Current.SaveChanges();

            retValue.PutValue("ExamExamRow", examExamRow);
            return retValue;
        }

        /// <summary>
        /// 从XML中获取第一个节点的值
        /// </summary>
        /// <param name="xmlText"></param>
        /// <param name="nodeTag"></param>
        /// <returns></returns>
        public static string GetFirstNodeValueFromXml(string xmlText, string nodeTag)
        {
            string nodeValue = "";

            string beginTag = "<" + nodeTag + ">";
            string endTag = "</" + nodeTag + ">";

            int beginTagPos = xmlText.IndexOf(beginTag, StringComparison.Ordinal);
            int endTagPos = xmlText.IndexOf(endTag, StringComparison.Ordinal);
            if (beginTagPos > -1 && endTagPos > -1 && endTagPos > beginTagPos)
            {
                nodeValue = xmlText.Substring(beginTagPos + beginTag.Length, endTagPos - beginTagPos - beginTag.Length);
            }
            return nodeValue;
        }
        /// <summary>
        /// 保存一整块的考生答案到数据库里分题保存
        /// </summary>
        /// <returns></returns>
        public ReturnValue SaveUserAnswerToDbFromXML(ExamGrade examGradeRow, string userAnswer)
        {
            ReturnValue retValue = new ReturnValue(false, "");
            XmlNode tempXmlNode;
            string examGradeUid = string.Empty;

            string userAnswerXML = userAnswer;
            if (string.IsNullOrEmpty(userAnswerXML))
            {
                examGradeRow.hasSaveAnswerToDb = "Y";
                _iExamGradeRep.Update(examGradeRow);
                return retValue;
            }

            XmlDocument xmlDocument = new XmlDocument();
            try
            {

                xmlDocument.LoadXml(userAnswerXML);
            }
            catch (Exception e)
            {
                try
                {
                    userAnswerXML = ReplaceLowOrder(userAnswerXML);
                    xmlDocument.LoadXml(userAnswerXML);
                }
                catch (Exception)
                {
                    retValue.HasError = true;
                    retValue.Message = ("考生答案信息格式有误.");
                    return retValue;
                }

            }

            XmlNodeList xmlNodeList = xmlDocument.DocumentElement.SelectNodes("exam_answers/exam_answer");

            tempXmlNode = xmlDocument.DocumentElement.SelectSingleNode("exam_grade_uid");
            if (tempXmlNode != null) examGradeUid = tempXmlNode.InnerText;

            if (examGradeUid == "")
            {
                retValue.HasError = true;
                retValue.Message = ("考生答案信息格式有误.");
                return retValue;
            }

            //清除考生答案
            string sql = string.Format("delete  from exam_answer where examGradeUid='{0}'", examGradeUid);
            _sqlExecuter.Execute(sql);
            //重新生成考生答案
            var questionUid = Guid.Empty;
            string answer_text = string.Empty;
            decimal judge_score = 0;
            string judge_result_code = string.Empty;
            ExamAnswer examAnswerRow = null;
            var ht = new Hashtable();
            var examPaperQuestionUid = Guid.Empty;
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                questionUid = Guid.Empty;
                answer_text = string.Empty;
                judge_score = 0;
                judge_result_code = string.Empty;
                tempXmlNode = xmlNode.SelectSingleNode("question_uid");
                if (tempXmlNode != null) questionUid = Guid.Parse(tempXmlNode.InnerText);


                tempXmlNode = xmlNode.SelectSingleNode("answer_text");
                if (tempXmlNode != null) answer_text = tempXmlNode.InnerText;


                tempXmlNode = xmlNode.SelectSingleNode("judge_score");
                if (tempXmlNode != null) judge_score = ConvertUtil.ToDecimal(tempXmlNode.InnerText);


                tempXmlNode = xmlNode.SelectSingleNode("judge_result_code");
                if (tempXmlNode != null) judge_result_code = tempXmlNode.InnerText;

                examAnswerRow = new ExamAnswer();
                examAnswerRow.examGradeUid = new Guid(examGradeUid);
                examAnswerRow.questionUid = questionUid;
                examAnswerRow.answerText = answer_text;
                examAnswerRow.judgeScore = judge_score;
                examAnswerRow.judgeResultCode = judge_result_code;
                _iExamAnswerRep.Insert(examAnswerRow);

                //记录写入的答案
                examPaperQuestionUid = questionUid;
                if (!ht.ContainsKey(examPaperQuestionUid))
                {
                    ht.Add(examPaperQuestionUid, examPaperQuestionUid);
                }
            }

            //将没有回答的题目也写入答案表

            //var listExamAnswer = db.ExamAnswers.Where(a => a.exam_grade_uid.Equals(examGradeUid)).ToList();

            //foreach (var examAnswer in listExamAnswer)
            //{
            //    examPaperQuestionUid = examAnswer.question_uid.ToString();
            //    if (!ht.ContainsKey(examPaperQuestionUid))
            //    {
            //        ht.Add(examPaperQuestionUid, examPaperQuestionUid);
            //    }
            //}

            var dtExamPaperNodeQuestion =
                _iExamPaperNodeQuestionRep.GetAllList(a => a.paperUid.Equals(examGradeRow.paperUid)).ToList();
            foreach (var paperNodeQuestion in dtExamPaperNodeQuestion)
            {
                examPaperQuestionUid = paperNodeQuestion.questionUid;
                if (!ht.ContainsKey(examPaperQuestionUid))
                {
                    examAnswerRow = new ExamAnswer();
                    examAnswerRow.examGradeUid = new Guid(examGradeUid);
                    examAnswerRow.questionUid = examPaperQuestionUid;
                    examAnswerRow.answerText = string.Empty;
                    examAnswerRow.judgeScore = 0;
                    examAnswerRow.judgeResultCode = string.Empty;
                    _iExamAnswerRep.Insert(examAnswerRow);

                    ht.Add(examPaperQuestionUid, examPaperQuestionUid);
                }
            }

            examGradeRow.hasSaveAnswerToDb = "Y";
            return new ReturnValue(false, "");
        }
        public static string ReplaceLowOrder(string tmp)
        {
            StringBuilder info = new StringBuilder();
            foreach (char cc in tmp)
            {
                int ss = (int)cc;
                if (((ss >= 0) && (ss <= 8)) || ((ss >= 11) && (ss <= 12)) || ((ss >= 14) && (ss <= 32)))
                    info.AppendFormat(" ", ss);//&#x{0:X}; 
                else info.Append(cc);
            }
            return info.ToString();
        }
        #endregion

        #region 查看答卷记录

        public async Task<ExamPaperViewOutputDto> GetUserPaperView(string examGradeUid, string filterType)
        {


            var examGrade = await _iExamGradeRep.FirstOrDefaultAsync(a => a.Id.Equals(new Guid(examGradeUid)));
            if (examGrade == null)
            {
                return new ExamPaperViewOutputDto { viewHtml = "找不到成绩信息!" };
            }
            var examExam = _iExamExamRep.FirstOrDefault(a => a.Id.Equals(examGrade.examUid));
            var paperUid = examGrade.paperUid;
            var paper = _iExamPaperRep.FirstOrDefault(a => a.Id.Equals(paperUid));
            //得到考试编号
            var user = _iUserBaseRepository.FirstOrDefault(a => a.Id.Equals(examGrade.userUid));
            var examUserGradeID = examExam != null ? examExam.ExamCode : "";
            var examUserName = user != null ? user.userFullName : "";
            var examBeginTime = examGrade.beginTime;
            var examEndTime = examGrade.endTime;
            var examTime = DateTimeUtil.ToTimeStrFromSecond(examGrade.examTime ?? 0);

            var currentDateTime = DateTime.Now;
            string publishGradeDate = examExam.publishGradeDate ?? "";

            //允许查看成绩天数
            int allowSeeGradeDays = examExam.allowSeeGradeDays ?? 0;
            bool isAllowSeeGradeDays = true;
            if (allowSeeGradeDays > 0)
            {

                var _allowSeeGradeTime = examGrade.lastUpdateTime.AddDays(allowSeeGradeDays);

                if (_allowSeeGradeTime > DateTime.Now) isAllowSeeGradeDays = false;
            }
            var examTotalScore = "******";
            if (examExam.isAllowSeeGrade == "Y" && (publishGradeDate == "" || DateTimeUtil.ToDateTime(publishGradeDate) < currentDateTime) && isAllowSeeGradeDays == true)
            {
                examTotalScore = (examGrade.gradeScore ?? 0).ToString("0.##");
            }


            //考试设置时的评卷人与查看试卷结果的评卷人保持一致
            var exam_uid = examGrade.examUid;
            var examJudge = from j in _iExamJudgeRepository.GetAll()
                            join u in _iUserBaseRepository.GetAll() on j.ownerUid equals u.Id
                            where j.examUid == exam_uid
                            select u.userFullName;
            string judge_user_name = string.Empty;
            if (examJudge != null && examJudge.Count() > 0)
            {
                judge_user_name = examJudge.ToList()[0];
            }

            var judgeRealName = string.IsNullOrEmpty(judge_user_name) ? ("系统自动评卷") : judge_user_name;
            //var judgeRealName = string.IsNullOrEmpty(examGrade.judge_user_name) ? ("系统自动评卷") : examGrade.judge_user_name;


            var judgeBeginTime = examGrade.judgeBeginTime.ToString() == "" ? examGrade.lastUpdateTime : examGrade.judgeBeginTime;
            var viewHtml = "";//GetUserPaperHtml(paper, examExam, examGrade, "", filterType);
            return new ExamPaperViewOutputDto()
            {
                examUserGradeID = examUserGradeID,
                examUserName = examUserName,
                examBeginTime = DateTimeUtil.ConvertToDateTimeStr(examBeginTime.ToString()),
                examEndTime = DateTimeUtil.ConvertToDateTimeStr(examEndTime.ToString()),
                examTime = examTime,
                judgeRealName = judgeRealName,
                JudgeBeginTime = DateTimeUtil.ConvertToDateTimeStr(judgeBeginTime.ToString()),
                examTotalScore = examTotalScore,
                viewHtml = viewHtml,
                Title = paper == null ? "" : paper.paperName,
                subTitle = paper == null ? "" : string.Format(("总共{0}题共{1}分"), paper.questionNum, paper.totalScore.ToString("0.##"))

            };
        }
        
        #endregion

        #region 随机试卷缓存试卷生成
        /// <summary>
        /// 随机试卷生成缓存试卷
        /// </summary>
        /// <param name="paperNum"></param>
        /// <param name="paperUid"></param>
        /// <param name="examUid"></param>
        private void CreateFixPaper(int paperNum, Guid paperUid, Guid examUid)
        {
            for (int i = 0; i < paperNum; i++)
            {
                var retValue = CreatePaperByPolicy(paperUid);
                if (retValue.HasError == true)
                {
                    return;
                }

                //试卷信息
                var examPaperRow = (Exam.ExamPaper)retValue.GetValue("examPaperRow");
                //保存试卷对象到数据库中
                if (InsertPaperObject(retValue) == false)
                {
                    return;
                }
                //生成XML文本
                _iExamPaperService.UpdatePaperXml(retValue);
                //插入考试与试卷的关联信息
                var examExamPaperRow = new ExamExamPaper
                {
                    Id = Guid.NewGuid(),
                    isActive = "Y",
                    examUid = examUid,
                    paperUid = examPaperRow.Id,
                    createTime = DateTime.Now
                };
                _iExamExamPaperRep.Insert(examExamPaperRow);
                _iUnitOfWorkManager.Current.SaveChanges();
            }

        }

        /// <summary>
        /// 删除原有的缓存试卷
        /// </summary>
        /// <param name="examUid"></param>
        private void DeleteExamExamPaperByExamUid(Guid examUid)
        {
            var paperIds =
                _iExamExamPaperRep.GetAll()
                    .Where(a => a.examUid == examUid)
                    .Select(a => a.paperUid)
                    .ToArray();
            var idStr = string.Join(",", paperIds);

            _iExamPaperService.Delete(idStr);
            _iExamExamPaperRep.DeleteAsync(a => a.examUid == examUid);
        }

        #endregion

        #region 新课云编译服务

        /// <summary>
        /// 提交程序题答案（到新课云）
        /// </summary>
        /// <param name="examGrade"></param>
        /// <param name="answerXmlStr"></param>
        /// <returns></returns>
        private async Task SubmitProgramQuestionAnswer(ExamGrade examGrade, string answerXmlStr)
        {
            var xml = new XmlDocument();
            try
            {
                xml.LoadXml(answerXmlStr);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                throw;
            }
            var questionIdList = await (from pq in _iExamPaperNodeQuestionRep.GetAll()
                                        join q in _iExamQuestionRep.GetAll() on pq.questionUid equals q.Id
                                        where pq.paperUid == examGrade.paperUid && (q.questionBaseTypeCode == "program" || q.questionBaseTypeCode == "program_fill")
                                        select q.Id).ToListAsync();
            //没有编程题
            if (!questionIdList.Any())
            {
                examGrade.IsCompiled = true;
                return;
            }
            var cloud = _iCloudRep.GetAll().FirstOrDefault();
            var dto = new UserAnswerInputDto
            {
                ExamId = examGrade.examUid,
                GradeId = examGrade.Id,
                UserId = examGrade.userUid,
                CloudId = cloud==null?0: cloud.CloudId
            };

            var emptyAnswerQuestionIds = new List<Guid>();
            foreach (var qId in questionIdList)
            {
                var xpath = $"/exam_grade_object/exam_answers/exam_answer[question_uid='{qId}']/answer_text";
                var node = xml.SelectSingleNode(xpath);
                if (node == null)
                {
                    emptyAnswerQuestionIds.Add(qId);
                    continue;
                }
                var answer = HttpUtility.UrlDecode(node.InnerText);
                var item = new UserAnswerDto { QuestionId = qId, Answer = answer };
                dto.UserAnswers.Add(item);
            }
            //未答题的不通过
            foreach (var qId in emptyAnswerQuestionIds)
            {
                await _iLibLabelService.CreateUserAnswerRecords(examGrade.userUid, qId, examGrade.Id, "exam", false);
            }

            //没做编程题
            if (!dto.UserAnswers.Any())
            {
                examGrade.IsCompiled = true;
                return;
            }
            examGrade.IsCompiled = false;
            var url = L("payUrl").TrimEnd('/') + "/api/compile?sign=";
            var jsonValue = JsonValue.Parse(JsonConvert.SerializeObject(dto));
            if (jsonValue != null)
            {
                var apiContent = HttpUtility.UrlEncode(jsonValue.ToString(), Encoding.UTF8);
                await HttpHelper.PostResponseSerializeData<ApiResponseResult<dynamic>>(url, apiContent);
            }
            else
            {
                throw new UserFriendlyException("无效的数据");
            }
        }

        /// <summary>
        /// 拉取编译成绩
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="examId"></param>
        /// <returns></returns>
        public async Task CheckCompiled(Guid userId,Guid examId)
        {
            //获取未编译成绩
            var gradeList = _iExamGradeRep.GetAll().Where(a => !a.IsCompiled && a.gradeStatusCode == "release" && a.userUid == userId).WhereIf(!Guid.Empty.Equals(examId),a=>a.examUid.Equals(examId))
                .ToList();
            if (!gradeList.Any())
            {
                return;
            }
            var url = L("payUrl").TrimEnd('/') + "/api/compile?sign=";
            foreach (var examGrade in gradeList)
            {
                var param = new Dictionary<string, string> { { "userId", examGrade.userUid.ToString() }, { "examId", examGrade.examUid.ToString() }, { "gradeId", examGrade.Id.ToString() } };
                var sign = SignatureHelper.GetSignature(param);
                var newUrl = url + sign +
                             $"&userId={examGrade.userUid}&examId={examGrade.examUid}&gradeId={examGrade.Id}";
                var result = await HttpHelper.GetAsync<ApiResponseResult<List<ExamCompileScore>>>(newUrl, null);
                if (result.IsSuccess && result.Data.InKey.Any())
                {
                    await SetProgramQuestionScore(examGrade, result.Data.InKey);
                }
            }

            _iUnitOfWorkManager.Current.SaveChanges();
        }

        /// <summary>
        /// 刷新有未更新考试成绩的考试
        /// </summary>
        /// <param name="examId"></param>
        /// <returns></returns>
        public async Task RefreshExamGradeCompiled(Guid examId)
        {
            var url = L("payUrl").TrimEnd('/') + "/api/compile/" + examId + "?sign=";
            var param = new Dictionary<string, string> { {"examId", examId.ToString()} };
            var sign = SignatureHelper.GetSignature(param);
            url += sign + "&examId=" + examId;
            var result = await HttpHelper.GetAsync<ApiResponseResult<List<UserExamCompileScore>>>(url, null);
            if (result.IsSuccess && result.Data.InKey.Any())
            {
                var data = result.Data.InKey.GroupBy(a => a.GradeId,
                        a => new ExamCompileScore
                        {
                            QuestionId = a.QuestionId,
                            GradeRate = a.GradeRate,
                            Result = a.Result,
                            IsPass = a.IsPass
                        })
                    .ToDictionary(a => a.Key, a => a.ToList());
                foreach (var item in data)
                {
                    var examGrade = await _iExamGradeRep.FirstOrDefaultAsync(item.Key);
                    if (examGrade != null && !examGrade.IsCompiled)
                    {
                        await SetProgramQuestionScore(examGrade, item.Value);
                    }
                }
            }
            _iUnitOfWorkManager.Current.SaveChanges();
        }

        /// <summary>
        /// 答卷是否可以查看
        /// </summary>
        /// <param name="examGradeId"></param>
        /// <returns></returns>
        public async Task<bool> CheckShowUserExamPreview(Guid examGradeId)
        {
            return await _iExamGradeRep.GetAll()
                .AnyAsync(a => a.Id == examGradeId && a.IsCompiled && a.gradeStatusCode == "release");
        }

        /// <summary>
        /// 编程题成绩加入考试成绩
        /// </summary>
        /// <param name="examGrade"></param>
        /// <param name="scoreList"></param>
        /// <returns></returns>
        private async Task SetProgramQuestionScore(ExamGrade examGrade, List<ExamCompileScore> scoreList)
        {
            
            var userAnswer = await _iExamUserAnswerRep.FirstOrDefaultAsync(examGrade.userAnswerUid);
            if (userAnswer == null)
            {
                throw new UserFriendlyException("用户答案丢失！");
            }
            var xml = new XmlDocument();
            try
            {
                xml.LoadXml(userAnswer.userAnswer);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString);
                throw;
            }

            var gradeScore = 0m;
            foreach (var score in scoreList)
            {
                var paperQuestion = await _iExamPaperNodeQuestionRep.FirstOrDefaultAsync(a =>
                    a.paperUid == examGrade.paperUid && a.questionUid == score.QuestionId);
                if (paperQuestion == null)
                {
                    throw new UserFriendlyException("试题信息丢失！");
                }

                var questionScore = paperQuestion.paperQuestionScore;
                if (score.IsPass == 0)
                {
                    var maxPointRate = (float)Convert.ToInt32(BaseSiteSetDto.maxPointRate) / 100;
                    var gradeRate = score.GradeRate;
                    if (gradeRate > maxPointRate)
                    {
                        gradeRate = maxPointRate;
                    }

                    questionScore = (decimal)gradeRate * paperQuestion.paperQuestionScore;
                }
                
                gradeScore += questionScore;
                var xpath = $"/exam_grade_object/exam_answers/exam_answer[question_uid='{score.QuestionId}']";
                var examAnswerNode = xml.SelectSingleNode(xpath);
                var pass = false;
                if (examAnswerNode != null)
                {
                    if (score.IsPass == 1)
                    {
                        var resultCodeNode = examAnswerNode.SelectSingleNode("judge_result_code");
                        if (resultCodeNode != null)
                        {
                            resultCodeNode.InnerText = "right";
                            pass = true;
                        }
                    }
                    var scoreNode = examAnswerNode.SelectSingleNode("judge_score");
                    if (scoreNode != null)
                    {
                        scoreNode.InnerText = questionScore.ToString("0.##");
                    }
                }
                //保存考试编程题编译结果
                var result = new ExamProgramResult
                {
                    GradeId = examGrade.Id,
                    QuestionId = paperQuestion.questionUid,
                    Result = score.Result
                };
                await _iExamProgramRep.InsertAsync(result);
                await _iLibLabelService.CreateUserAnswerRecords(examGrade.userUid, result.QuestionId, examGrade.Id, "exam", pass);
            }
            examGrade.IsCompiled = true;
            if (gradeScore > 0)
            {
                if (examGrade.gradeScore.HasValue)
                {
                    examGrade.gradeScore += gradeScore;
                }
                else
                {
                    examGrade.gradeScore = gradeScore;
                }
                var node = xml.SelectSingleNode("/exam_grade_object/grade_score");
                if (node != null && examGrade.gradeScore.HasValue)
                {
                    node.InnerText = examGrade.gradeScore.Value.ToString("0.##");
                    userAnswer.userAnswer = ConvertXmlToString(xml);
                    await _iExamUserAnswerRep.UpdateAsync(userAnswer);
                }
                var exam = await _iExamExamRep.FirstOrDefaultAsync(a=>a.Id == examGrade.examUid);
                if (exam != null)
                {
                    var passGradeScore = exam.passGradeScore ?? 0;
                    if (exam.passGradeType == "passGradeRate")
                    {
                        var paper = await _iExamPaperRep.FirstOrDefaultAsync(a => a.Id == examGrade.paperUid);
                        if (paper != null)
                        {
                            var passGradeRate = exam.passGradeRate ?? 0;
                            passGradeScore = paper.totalScore * passGradeRate / 100;
                        }
                    }

                    examGrade.isPass = (examGrade.gradeScore.HasValue && examGrade.gradeScore.Value >= passGradeScore) ? "Y" : "N";
                }
                
            }
            await _iExamGradeRep.UpdateAsync(examGrade);
            
        }

        /// <summary>         
        /// 将XmlDocument转化为string   
        /// </summary>        
        /// <param name="xmlDoc"></param>    
        /// <returns></returns>     
        private static string ConvertXmlToString(XmlDocument xmlDoc)
        {
            var stream = new MemoryStream();
            var writer = new XmlTextWriter(stream, null) {Formatting = System.Xml.Formatting.Indented};
            xmlDoc.Save(writer);
            var sr = new StreamReader(stream, Encoding.UTF8);
            stream.Position = 0;
            var xmlString = sr.ReadToEnd();
            sr.Close();
            stream.Close();
            return xmlString;
        }
        #endregion

        #region 考试试题标签积分处理
        /// <summary>
        /// 根据用户考试答案更新用户试题标签积分
        /// </summary>
        /// <param name="paperId"></param>
        /// <param name="userId"></param>
        /// <param name="gradeId"></param>
        /// <param name="xmlStr"></param>
        /// <returns></returns>
        private async Task UpdateUserLabelScoreByAnswerXml(Guid paperId, Guid userId, Guid gradeId, string xmlStr)
        {
            var xml = new XmlDocument();
            try
            {
                xml.LoadXml(xmlStr);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                throw;
            }

            var xpath = "/exam_grade_object/exam_answers/exam_answer";
            var examAnswerNodes = xml.SelectNodes(xpath);
            if (examAnswerNodes == null)
            {
                return;
            }
            var questionIdList = await(from pq in _iExamPaperNodeQuestionRep.GetAll()
                join q in _iExamQuestionRep.GetAll() on pq.questionUid equals q.Id
                where pq.paperUid == paperId && q.questionBaseTypeCode != "program" && q.questionBaseTypeCode != "program_fill"
                select q.Id).ToListAsync();
            foreach (XmlNode node in examAnswerNodes)
            {
                var questionUidNode = node.SelectSingleNode("question_uid");
                if (questionUidNode == null)
                {
                    continue;
                }
                var questionId = questionUidNode.InnerText.TryParseGuid();
                if (!questionIdList.Contains(questionId))
                {
                    continue;
                }
                var resultCodeNode = node.SelectSingleNode("judge_result_code");
                if (resultCodeNode == null)
                {
                    continue;
                }
                var pass = resultCodeNode.InnerText == "right";
                await _iLibLabelService.CreateUserAnswerRecords(userId, questionId, gradeId, "exam", pass);
            }
        }

        #endregion

        private string CreateNewCode()
        {
            var code = "E000001";
            var entity = _iExamExamRep.GetAll().Where(a => !a.isCustomCode).OrderByDescending(a => a.createTime).FirstOrDefault();
            if (entity != null)
            {
                code = entity.ExamCode;
                do
                {
                    code = StringUtil.GetNextCodeByAuto(code);
                } while (_iExamExamRep.GetAll().Any(a => a.ExamCode == code));
            }
            return code;
        }
    }
}
