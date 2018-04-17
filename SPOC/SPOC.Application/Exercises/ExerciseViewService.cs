using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using SPOC.Common.Cookie;
using SPOC.Common.Pagination;
using SPOC.Core;
using SPOC.Core.Dto.CodeCompile;
using SPOC.Exam;
using SPOC.Exercises.Dto;
using SPOC.Exercises.Enum;
using SPOC.User;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using SPOC.Lib;
using SPOC.SysSetting;

namespace SPOC.Exercises
{
    /// <summary>
    /// 练习视图服务接口实现
    /// </summary>
    public class ExerciseViewService:SPOCAppServiceBase, IExerciseViewService
    {
        private readonly IRepository<Exercise, Guid> _iExerciseRep;
        private readonly IRepository<ExerciseClass, Guid> _iExerciseClassRep;
        private readonly IRepository<ExerciseAnswer, Guid> _iExerciseAnswerRep;
        private readonly IRepository<ExerciseRecord, Guid> _iExerciseRecordRep;
        private readonly IRepository<UserBase, Guid> _iUserBaseRep;
        private readonly IRepository<ClassStudent, Guid> _iClassStudentRep;
        private readonly IRepository<ExamQuestion, Guid> _iExamQuestionRep;
        private readonly IRepository<QuestionStandardCode, Guid> _iQuestionStandardCodeRep;

        private readonly IExerciseRankingViewService _iExerciseRankingViewService;
        private readonly ICodeComplieService _iCodeComplieService;
        private readonly ILibLabelService _iLibLabelService;

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerciseViewService(IRepository<Exercise, Guid> iExerciseRep, IRepository<ExerciseClass, Guid> iExerciseClassRep, 
            IRepository<ExerciseAnswer, Guid> iExerciseAnswerRep, IRepository<ExerciseRecord, Guid> iExerciseRecordRep, 
            IRepository<UserBase, Guid> iUserBaseRep, IRepository<ClassStudent, Guid> iClassStudentRep,
            IRepository<ExamQuestion, Guid> iExamQuestionRep, IExerciseRankingViewService iExerciseRankingViewService, 
            ICodeComplieService iCodeComplieService, IRepository<QuestionStandardCode, Guid> iQuestionStandardCodeRep, 
            ILibLabelService iLibLabelService)
        {
            _iExerciseRep = iExerciseRep;
            _iExerciseClassRep = iExerciseClassRep;
            _iExerciseAnswerRep = iExerciseAnswerRep;
            _iExerciseRecordRep = iExerciseRecordRep;
            _iUserBaseRep = iUserBaseRep;
            _iClassStudentRep = iClassStudentRep;
            _iExamQuestionRep = iExamQuestionRep;
            _iExerciseRankingViewService = iExerciseRankingViewService;
            _iCodeComplieService = iCodeComplieService;
            _iQuestionStandardCodeRep = iQuestionStandardCodeRep;
            _iLibLabelService = iLibLabelService;
        }

        #endregion

        /// <summary>
        /// 获取练习基础信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ExerciseBaseViewOutputDto> GetBase(Guid id)
        {
            var dto = await (from e in _iExerciseRep.GetAll()
                join u in _iUserBaseRep.GetAll() on e.CreatorId equals u.Id
                where e.Id == id
                select new ExerciseBaseViewOutputDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    UserId = e.CreatorId,
                    UserName = string.IsNullOrEmpty(u.userFullName) ? u.userLoginName : u.userFullName,
                    UserImg = u.smallAvatar,
                    QuestionId = e.QuestionId,
                    EndTime = e.EndTime,
                    ShowAnswer = e.ShowAnswer,
                    ShowAnswerType = e.ShowAnswerType
                }).FirstOrDefaultAsync();
            return dto;
        }

        /// <summary>
        /// 练习列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PaginationOutputDto<ExerciseViewItem>> GetPagination(PaginationInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                return new PaginationOutputDto<ExerciseViewItem>();
            }
            var classId = await _iClassStudentRep.GetAll()
                .Where(a => a.UserId == cookie.Id).OrderByDescending(a => a.CreateTime)
                .Select(a => a.ClassId)
                .FirstOrDefaultAsync();
            if (classId == Guid.Empty)
            {
                return new PaginationOutputDto<ExerciseViewItem>();
            }
            //练习任务
            var exercises = from ec in _iExerciseClassRep.GetAll()
                join e in _iExerciseRep.GetAll() on ec.Id equals e.Id
                where ec.ClassId == classId
                select e;
            
            //记录
            var recordList = await (from r in _iExerciseRecordRep.GetAll()
                join e in exercises on r.ExerciseId equals e.Id
                join a in _iExerciseAnswerRep.GetAll() on r.Id equals a.Id
                where r.UserId == cookie.Id
                select r).ToListAsync();

            //已经开始并有结束时间且未通过的（结束时间正序）
            var exerciseList1 = new List<ExerciseViewItem>();
            //已经开始并没有结束时间且未通过的（创建时间倒叙）
            var exerciseList2 = new List<ExerciseViewItem>();
            //已经过期或已通过的（创建时间倒序）
            var exerciseList3 = new List<ExerciseViewItem>();
            var exerciseList = await exercises.ToListAsync();
            foreach (var e in exerciseList)
            {
                var reList = recordList.Where(a => a.ExerciseId == e.Id).ToList();
                var item = new ExerciseViewItem
                {
                    Id = e.Id,
                    Title = e.Title,
                    CreateTime = e.CreateTime,
                    EndTime = e.EndTime
                };

                if (!reList.Any())
                {
                    item.UserState = UserExerciseStateEnum.NotSubmitted;
                    if (e.EndTime.HasValue)
                    {
                        if (e.EndTime > DateTime.Now)
                        {
                            exerciseList1.Add(item);//未提交
                        }
                        else
                        {
                            exerciseList3.Add(item);//过期
                        }
                    }
                    else
                    {
                        exerciseList2.Add(item);//未提交
                    }
                    continue;
                }
                var re = reList.FirstOrDefault(a => a.IsPass);
                if (re != null)
                {
                    item.UserState = UserExerciseStateEnum.Pass;
                    item.UseTime = re.EndTime - re.BeginTime;
                    exerciseList3.Add(item);//已通过
                    continue;
                }
                item.UserState = UserExerciseStateEnum.Fail;
                if (e.EndTime.HasValue)//有结束时间
                {
                    if (e.EndTime > DateTime.Now)
                    {
                        exerciseList1.Add(item); //未通过
                    }
                    else
                    {
                        exerciseList3.Add(item); //过期
                    }
                }
                else//没有结束时间
                {
                    exerciseList2.Add(item); //未通过
                }
            }

            var list = new List<ExerciseViewItem>();
            list.AddRange(exerciseList1.OrderBy(item => item.EndTime));
            list.AddRange(exerciseList2.OrderByDescending(item => item.CreateTime));
            list.AddRange(exerciseList3.OrderByDescending(item => item.CreateTime));
            var rows = list.Skip(input.skip).Take(input.pageSize).ToList();
            var students = _iClassStudentRep.GetAll().Where(a => a.ClassId == classId);

            foreach (var item in rows)
            {
                var recordsList =  await _iExerciseRecordRep.GetAll()
                    .Where(a => a.ExerciseId == item.Id && a.EndTime.HasValue)
                    .Join(students, record => record.UserId, student => student.UserId, (record, student) => new {record.Id, record.ExerciseId, record.IsPass, record.UserId})
                    .ToListAsync();
                
                //完成人数
                item.FinishedNum = recordsList.Count(a => a.IsPass);

                //通过率
                var userCount = recordsList.GroupBy(a => a.UserId).Count();
                item.PassRate = userCount == 0 ? 0 : (decimal)item.FinishedNum / userCount * 100;

                //效率排名
                if (item.UserState != UserExerciseStateEnum.NotSubmitted)
                {
                    var rankingItem = await _iExerciseRankingViewService.GetEfficiencyRanking(item.Id, cookie.Id);
                    item.Ranking = rankingItem.Ranking;
                }
            }
            return new PaginationOutputDto<ExerciseViewItem>
            {
                rows = rows,
                total = list.Count
            };
        }

        /// <summary>
        /// 运行代码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ExerciseRunCodeOutputDto> RunCode(ExerciseCodeInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();

            #region 验证

            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }

            //已通过
            if (await _iExerciseRecordRep.GetAll()
                .AnyAsync(a => a.IsPass && a.ExerciseId == input.Id && a.UserId == cookie.Id))
            {
                throw new UserFriendlyException("已完成的练习请不要再次提交，请关闭页面退出当前练习");
            }

            var classId = await _iClassStudentRep.GetAll()
                .Where(a => a.UserId == cookie.Id).OrderByDescending(a => a.CreateTime)
                .Select(a => a.ClassId)
                .FirstOrDefaultAsync();
            if (classId == Guid.Empty)
            {
                throw new UserFriendlyException("无效的班级信息");
            }
            var exercise = await _iExerciseRep.FirstOrDefaultAsync(a => a.Id == input.Id);
            if (exercise == null)
            {
                throw new UserFriendlyException("无效的练习");
            }

            if (!await _iExerciseClassRep.GetAll().AnyAsync(a => a.ClassId == classId && a.Id == input.Id))
            {
                throw new UserFriendlyException("未发布的练习");
            }
            var record = await _iExerciseRecordRep.FirstOrDefaultAsync(a =>
                a.ExerciseId == input.Id && !a.EndTime.HasValue && a.UserId == cookie.Id);
            if (record == null)
            {
                throw new UserFriendlyException("未找到练习开始记录");
            }
            #endregion

            var question = await _iExamQuestionRep.FirstOrDefaultAsync(a => a.Id == exercise.QuestionId);
          

            var param = "";
            var dto = new ExerciseRunCodeOutputDto
            {
                IsTestRun = input.IsTestRun
            };
            ComplieOutDto<string> result;
            if (input.IsTestRun)
            {
                var inputParam = "";
                if (!string.IsNullOrEmpty(input.Param))
                {
                    param = input.Param;
                }

                if (!string.IsNullOrEmpty(input.InputParam))
                {
                    inputParam = input.InputParam;
                }

                result = await _iCodeComplieService.Compile(new CompileInputDto
                {
                    Code = input.Code,
                    Language = question.language,
                    Param = param,
                    InputParam = inputParam
                });
                if (result.Code == 3)
                {
                    result.Msg = "代码运行超时，请检查是否有无限循环或递归代码。\n" + result.Msg;
                }
                dto.ResultList.Add(new ExerciseRunCodeResultItem
                {
                    IsSuccessedRun = result.Code == 0,
                    ComplieError = result.Msg,
                    Output = result.Result,
                    Time = DateTime.Now
                });
            }
            else
            {
                if (!string.IsNullOrEmpty(question.param))
                {
                    param = question.param;
                }

                string[] inputParams;
                string[] answers;
                if (question.MultiTest)
                {
                    inputParams = !string.IsNullOrEmpty(question.InputParam)
                        ? question.InputParam.Split('|')
                        : new[] {""};
                    answers = question.standardAnswer.Split('|');
                }
                else
                {
                    inputParams = !string.IsNullOrEmpty(question.InputParam) ? new []{ question.InputParam } : new[] { "" };
                    answers = new[] { question.standardAnswer };
                }

                var passCount = 0;
                for (var i = 0; i < inputParams.Length; i++)
                {
                    result = await _iCodeComplieService.Compile(new CompileInputDto
                    {
                        Code = input.Code,
                        Language = question.language,
                        Param = param,
                        InputParam = inputParams[i]
                    });

                    if (result.Code == 0 && result.Result == answers[i])
                    {
                        passCount++;
                    }

                    if (result.Code == 3)
                    {
                        result.Msg = "代码运行超时，请检查是否有无限循环或递归代码。\n" + result.Msg;
                    }
                    var resultItem = new ExerciseRunCodeResultItem
                    {
                        IsSuccessedRun = result.Code == 0,
                        ComplieError = result.Msg,
                        Output = result.Result,
                        Answer = answers[i],
                        Time = DateTime.Now
                    };
                    resultItem.Pass = resultItem.Answer == resultItem.Output;
                    dto.ResultList.Add(resultItem);
                    if (inputParams.Length > 1)
                    {
                        record.CompiledResults += "第" + (i + 1) + "次测试：\n";
                    }
                    
                    if (resultItem.IsSuccessedRun)
                    {
                        record.CompiledResults += "编译成功！\n运行结果:\n" + resultItem.Output + "\n";
                    }
                    else
                    {
                        record.CompiledResults += "编译失败！\n编译器返回:\n" + resultItem.ComplieError + "\n";
                    }
                    record.CompiledResults += "标准答案：\n" + resultItem.Answer + "\n";
                    var pass = resultItem.Pass ? "通过" : "未通过";
                    #region 检测代码是否直接包含输出答案
                    //校验是否作弊代码 多次运行不进行判断
                    if (resultItem.Pass && !question.MultiTest)
                    {
                       
                        bool CheckCodeFunc(Guid questionId, string code, string standardAnswer)
                        {
                            var returnResult = false; //作答代码校验结果 假设不含作弊代码
                            ////检验作答代码是否直接输出答案
                            //拆分参考答案检测是否多次输出拼装答案
                            var standardAnswers = standardAnswer.Split('\n').Union(new[] { standardAnswer.Replace("\n", "\\n") }).ToArray();
                            foreach (var strAnswer in standardAnswers)
                            {
                                if (string.IsNullOrWhiteSpace(strAnswer))
                                    continue;
                                if ((code.Contains("'" + strAnswer + "'") || code.Contains("\"" + strAnswer + "\"")))
                                    returnResult = true;

                            }
                            if (!returnResult)
                                return false;

                            //检验参考代码是否直接输出答案 二次校验代码
                            var standardCodeCheck = true; //参考代码校验结果 第一次校验已默认含作弊代码
                            var standardCodes = _iQuestionStandardCodeRep.GetAll().Where(q => q.questionId.Equals(questionId)).Select(a => a.code)
                                .ToList();
                            standardCodes.ForEach(c =>
                            {
                                foreach (var strAnswer in standardAnswers)
                                {
                                    if (string.IsNullOrWhiteSpace(strAnswer))
                                        continue;
                                    if (c.Contains("'" + strAnswer + "'") || c.Contains("\"" + strAnswer + "\""))
                                        standardCodeCheck = false;

                                }
                            });
                            return standardCodeCheck;
                        }
                       
                        if (CheckCodeFunc(question.Id, input.Code, question.standardAnswer))
                        {
                            record.CompiledResults += "系统检测疑是作弊代码\n";
                            pass = "未通过";
                            passCount = 0;
                            resultItem.Pass = false;
                            resultItem.ComplieError += "系统检测疑是作弊代码\n";
                        }
                    }
                    #endregion
                    record.CompiledResults += "是否通过：" + pass + "\n\n";
                }

                var answer = await _iExerciseAnswerRep.FirstOrDefaultAsync(record.Id);
                if (answer == null)
                {
                    answer = new ExerciseAnswer { Id = record.Id, Answer = input.Code };
                    await _iExerciseAnswerRep.InsertAsync(answer);
                }
                else
                {
                    await _iExerciseAnswerRep.UpdateAsync(answer);
                }

                record.IsPass = passCount == inputParams.Length;
           
                record.EndTime = DateTime.Now;
                dto.IsPass = record.IsPass;
                
                await _iExerciseRecordRep.UpdateAsync(record);

                await _iLibLabelService.CreateUserAnswerRecords(cookie.Id, question.Id, record.Id, "exercise", record.IsPass);
            }
            
            return dto;
        }

        /// <summary>
        /// 检测用户使用权限
        /// </summary>
        /// <param name="exerciseId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> CheckUserAuthorization(Guid exerciseId, Guid userId)
        {
            var classId = await _iClassStudentRep.GetAll()
                .Where(a => a.UserId == userId).OrderByDescending(a => a.CreateTime)
                .Select(a => a.ClassId)
                .FirstOrDefaultAsync();
            if (classId == Guid.Empty)
            {
                return false;
            }

            var hasExerciseClass =
                await _iExerciseClassRep.GetAll().AnyAsync(a => a.Id == exerciseId && a.ClassId == classId);
            if (!hasExerciseClass)
            {
                return false;
            }

            //时间内
            var inTheTime = await _iExerciseRep.GetAll()
                .AnyAsync(a => a.Id == exerciseId && (!a.EndTime.HasValue || a.EndTime > DateTime.Now));

            if (!inTheTime)
            {
                return false;
            }
            
            //已通过
            if (await _iExerciseRecordRep.GetAll()
                .AnyAsync(a => a.IsPass && a.ExerciseId == exerciseId && a.UserId == userId))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 开始练习
        /// </summary>
        /// <param name="exerciseId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task StartExercise(Guid exerciseId, Guid userId)
        {
            var record = await _iExerciseRecordRep.FirstOrDefaultAsync(a =>
                a.ExerciseId == exerciseId && !a.EndTime.HasValue && a.UserId == userId);

            if (record == null)
            {
                record = new ExerciseRecord
                {
                    ExerciseId = exerciseId,
                    UserId = userId,
                    BeginTime = DateTime.Now,
                    IsPass = false
                };

                await _iExerciseRecordRep.InsertAsync(record);
            }
        }

        /// <summary>
        /// 获取某人练习记录
        /// </summary>
        /// <param name="exerciseId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<ExerciseRecordItem>> GetExerciseRecordList(Guid exerciseId, Guid userId)
        {
            var list = await _iExerciseRecordRep.GetAll()
                .Where(a => a.ExerciseId == exerciseId && a.UserId == userId && a.EndTime.HasValue)
                .Select(a => new ExerciseRecordItem
                {
                    Id = a.Id,
                    BeginTime = a.BeginTime,
                    EndTime = a.EndTime.Value,
                    IsPass = a.IsPass
                }).ToListAsync();
            return list;
        }

        /// <summary>
        /// 获取某人练习的提交答案
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        public async Task<string> GetUserExerciseAnswer(Guid recordId)
        {
            return await _iExerciseAnswerRep.GetAll().Where(a => a.Id == recordId)
                .Select(a => a.Answer).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 获取练习参考答案
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> GetExerciseAnswer(Guid id)
        {
            var answer = await (from e in _iExerciseRep.GetAll()
                join q in _iQuestionStandardCodeRep.GetAll() on e.QuestionId equals q.questionId
                where e.Id == id && q.isDefault == 1
                select q.code).FirstOrDefaultAsync();
            return answer;
        }
    }
}