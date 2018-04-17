using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using SPOC.Common.Cookie;
using SPOC.Common.Pagination;
using SPOC.Exercises.Dto;
using SPOC.User;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace SPOC.Exercises
{
    /// <summary>
    /// 练习排行榜服务接口实现
    /// </summary>
    public class ExerciseRankingViewService : ApplicationService, IExerciseRankingViewService
    {
        private readonly IRepository<ExerciseRecord, Guid> _iExerciseRecordRep;
        private readonly IRepository<UserBase, Guid> _iUserBaseRep;
        private readonly IRepository<ClassStudent, Guid> _iClassStudentRep;

        /// <summary>
        /// 效率排序条件结构
        /// </summary>
        private struct EfficiencySortCondition
        {
            /// <summary>
            /// 是否通过
            /// </summary>
            public bool IsPass { get; set; }
            /// <summary>
            /// 用时
            /// </summary>
            public TimeSpan? UseTime { get; set; }
            /// <summary>
            /// 练习次数
            /// </summary>
            public int ExerciseCount { get; set; }
        }

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerciseRankingViewService(IRepository<ExerciseRecord, Guid> iExerciseRecordRep,
            IRepository<UserBase, Guid> iUserBaseRep, IRepository<ClassStudent, Guid> iClassStudentRep)
        {
            _iExerciseRecordRep = iExerciseRecordRep;
            _iUserBaseRep = iUserBaseRep;
            _iClassStudentRep = iClassStudentRep;
        }

        #endregion

        /// <summary>
        /// 获取某人效率排行榜
        /// </summary>
        /// <param name="exerciseId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<EfficiencyRankingViewItem> GetEfficiencyRanking(Guid exerciseId, Guid userId)
        {
            var classId = await _iClassStudentRep.GetAll()
                .Where(a => a.UserId == userId).OrderByDescending(a => a.CreateTime)
                .Select(a => a.ClassId)
                .FirstOrDefaultAsync();
            if (classId == Guid.Empty)
            {
                throw new UserFriendlyException("没有班级");
            }

            var list = await GetEfficiencyRankingList(exerciseId, classId);

            return list.Find(a => a.UserId == userId);
        }

        /// <summary>
        /// 获取效率排行榜分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PaginationOutputDto<EfficiencyRankingViewItem>> GetEfficiencyRankingPagination(ExerciseRankingPaginationInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                return new PaginationOutputDto<EfficiencyRankingViewItem>();
            }
            var classId = await _iClassStudentRep.GetAll()
                .Where(a => a.UserId == cookie.Id).OrderByDescending(a => a.CreateTime)
                .Select(a => a.ClassId)
                .FirstOrDefaultAsync();
            if (classId == Guid.Empty)
            {
                throw new UserFriendlyException("没有班级");
            }

            var list = await GetEfficiencyRankingList(input.ExerciseId, classId);

            return new PaginationOutputDto<EfficiencyRankingViewItem>
            {
                rows = list.Skip(input.skip).Take(input.pageSize).ToList(),
                total = list.Count
            };
        }

        /// <summary>
        /// 获取效率排行
        /// </summary>
        /// <param name="exerciseId"></param>
        /// <param name="classId"></param>
        /// <returns></returns>
        private async Task<List<EfficiencyRankingViewItem>> GetEfficiencyRankingList(Guid exerciseId, Guid classId)
        {
            var students = _iClassStudentRep.GetAll().Where(a => a.ClassId == classId)
                .Join(_iUserBaseRep.GetAll().Where(a=>a.approvalStatus == "approved"), student => student.UserId, user => user.Id,
                    (s, u) => new { s.UserId, UserLoginName = u.userLoginName, UserFullName = u.userFullName });

            var records = from r in _iExerciseRecordRep.GetAll()
                          join student in students on r.UserId equals student.UserId
                          where r.ExerciseId == exerciseId && r.EndTime.HasValue
                          select r;
            var recordDic = await records.GroupBy(a => a.UserId).ToDictionaryAsync(a => a.Key, a => a.ToList());
            var conditionList = new List<EfficiencySortCondition>();
            var passDic = new Dictionary<Guid, EfficiencySortCondition>();
            var failDic = new Dictionary<Guid, EfficiencySortCondition>();

            foreach (var kv in recordDic)
            {
                var condition = new EfficiencySortCondition { ExerciseCount = kv.Value.Count };
                var record = kv.Value.FirstOrDefault(a => a.IsPass);
                if (record != null)
                {
                    condition.IsPass = true;
                    condition.UseTime = record.EndTime - record.BeginTime;
                    if (!conditionList.Any(c => c.Equals(condition)))
                    {
                        conditionList.Add(condition);
                    }
                    passDic.Add(kv.Key, condition);
                }
                else
                {
                    condition.IsPass = false;
                    record = kv.Value.OrderBy(a => a.EndTime - a.BeginTime).First();
                    condition.UseTime = record.EndTime - record.BeginTime;
                    failDic.Add(kv.Key, condition);
                }

            }
            conditionList = conditionList.OrderByDescending(a => a.IsPass)
                .ThenBy(a => a.ExerciseCount)
                .ThenBy(a => a.UseTime)
                .ToList();

            var studentList = await students.ToListAsync();
            //总排名
            var sortList = new List<EfficiencyRankingViewItem>();
            //通过的
            foreach (var kv in passDic)
            {
                var student = studentList.Find(a => a.UserId == kv.Key);
                var item = new EfficiencyRankingViewItem
                {
                    UserId = student.UserId,
                    UserName = string.IsNullOrEmpty(student.UserFullName) ? student.UserLoginName : student.UserFullName,
                    UseTime = kv.Value.UseTime,
                    ExerciseCount = kv.Value.ExerciseCount,
                    IsPass = true,
                    Ranking = conditionList.IndexOf(kv.Value) + 1
                };
                sortList.Add(item);
            }
            //未通过的
            var failRanking = conditionList.Count + 1;
            foreach (var kv in failDic)
            {
                var student = studentList.Find(a => a.UserId == kv.Key);
                var item = new EfficiencyRankingViewItem
                {
                    UserId = student.UserId,
                    UserName = string.IsNullOrEmpty(student.UserFullName) ? student.UserLoginName : student.UserFullName,
                    UseTime = kv.Value.UseTime,
                    ExerciseCount = kv.Value.ExerciseCount,
                    IsPass = false,
                    Ranking = failRanking
                };
                sortList.Add(item);
            }
            //未提交的
            var lastRanking = failDic.Any() ? failRanking + 1 : failRanking;
            var userIdList = recordDic.Keys.ToList();
            var studentList2 = studentList.Where(s => !userIdList.Contains(s.UserId)).ToList();
            foreach (var student in studentList2)
            {
                var item = new EfficiencyRankingViewItem
                {
                    UserId = student.UserId,
                    UserName = string.IsNullOrEmpty(student.UserFullName) ? student.UserLoginName : student.UserFullName,
                    IsPass = false,
                    Ranking = lastRanking
                };
                sortList.Add(item);
            }

            sortList = sortList.OrderBy(a => a.Ranking).ToList();
            return sortList;
        }

        /// <summary>
        /// 获取某人积极性排行榜
        /// </summary>
        /// <param name="exerciseId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<EnthusiasmRankingViewItem> GetEnthusiasmRanking(Guid exerciseId, Guid userId)
        {
            var classId = await _iClassStudentRep.GetAll()
                .Where(a => a.UserId == userId).OrderByDescending(a => a.CreateTime)
                .Select(a => a.ClassId)
                .FirstOrDefaultAsync();
            if (classId == Guid.Empty)
            {
                throw new UserFriendlyException("没有班级");
            }

            var list = await GetEnthusiasmRankingList(exerciseId, classId);
            return list.Find(a=>a.UserId == userId);
        }

        /// <summary>
        /// 获取积极性排行榜分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PaginationOutputDto<EnthusiasmRankingViewItem>> GetEnthusiasmRankingPagination(ExerciseRankingPaginationInputDto input)
        {
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                return new PaginationOutputDto<EnthusiasmRankingViewItem>();
            }
            var classId = await _iClassStudentRep.GetAll()
                .Where(a => a.UserId == cookie.Id).OrderByDescending(a => a.CreateTime)
                .Select(a => a.ClassId)
                .FirstOrDefaultAsync();
            if (classId == Guid.Empty)
            {
                throw new UserFriendlyException("没有班级");
            }
            var list = await GetEnthusiasmRankingList(input.ExerciseId, classId);
            
            return new PaginationOutputDto<EnthusiasmRankingViewItem>
            {
                rows = list.Skip(input.skip).Take(input.pageSize).ToList(),
                total = list.Count
            };
        }

        private async Task<List<EnthusiasmRankingViewItem>> GetEnthusiasmRankingList(Guid exerciseId, Guid classId)
        {
            var students = _iClassStudentRep.GetAll().Where(a => a.ClassId == classId)
                .Join(_iUserBaseRep.GetAll().Where(a=>a.approvalStatus == "approved"), student => student.UserId, user => user.Id,
                    (s, u) => new {  s.UserId, UserLoginName = u.userLoginName, UserFullName = u.userFullName });

            var records = from r in _iExerciseRecordRep.GetAll()
                join student in students on r.UserId equals student.UserId
                where r.ExerciseId == exerciseId && r.EndTime.HasValue
                select r;

            var rankingDataList = await records.GroupBy(a => a.UserId)
                .Select(a=>new
                {
                    UserId = a.Key,
                    BeginTime = a.Select(r=> r.BeginTime).Min(),
                    IsPass = a.Any(r=>r.IsPass)
                }).ToListAsync();

            var sortList = rankingDataList.Select(a => a.BeginTime).Distinct().ToList();
            sortList.Sort();

            var studentList = await students.ToListAsync();
            var list = new List<EnthusiasmRankingViewItem>();
            //已提交
            foreach (var data in rankingDataList)
            {
                var student = studentList.Find(a => a.UserId == data.UserId);
                var item = new EnthusiasmRankingViewItem
                {
                    Ranking = sortList.IndexOf(data.BeginTime) + 1,
                    UserId = student.UserId,
                    UserName = string.IsNullOrEmpty(student.UserFullName) ? student.UserLoginName : student.UserFullName,
                    BeginTime = data.BeginTime,
                    IsPass = data.IsPass
                };
                list.Add(item);
            }
            //未提交
            var lastRanking = rankingDataList.Count + 1;
            var idList = rankingDataList.Select(a => a.UserId).ToList();
            var studentList2 = studentList.Where(a => !idList.Contains(a.UserId)).ToList();
            foreach (var student in studentList2)
            {
                var item = new EnthusiasmRankingViewItem
                {
                    Ranking = lastRanking,
                    UserId = student.UserId,
                    UserName = string.IsNullOrEmpty(student.UserFullName) ? student.UserLoginName : student.UserFullName,
                    IsPass = false
                };
                list.Add(item);
            }
            list = list.OrderBy(a => a.Ranking).ToList();
            return list;
        }
    }
}