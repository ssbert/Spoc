using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.AutoMapper;
using AutoMapper;
using Newtonsoft.Json;

namespace SPOC.Core.Dto.Challenge
{
    public class ChallengeFolderDto
    {
        /// <summary>
        /// 分类ID
        /// </summary>
        public Guid id { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        public string name { get; set; }
    }
    public class PointsRankDto
    {
        /// <summary>
        /// 我的得分
        /// </summary>
        public decimal points { get; set; }
        /// <summary>
        /// 排名
        /// </summary>
        public int rank { get; set; }
    }
    /// <summary>
    /// 挑战列表项
    /// </summary>
    public class ChallengeQuestionViewDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid id { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 通过率
        /// </summary>
        public string passRate { get; set; }
        /// <summary>
        /// 提交数
        /// </summary>
        public int submitNum { get; set; }
        /// <summary>
        /// 通过数
        /// </summary>
        public int passNum { get; set; }
        public decimal score { get; set; }
        public string hard { get; set; }
        public int seq { get; set; }
        /// <summary>
        /// 挑战状态
        /// </summary>
        public int status { get; set; }
        public string folderPath { get; set; }
    }
    /// <summary>
    /// 挑战列表
    /// </summary>
    public class ChallengeQuestionViewModel
    {
        public List<ChallengeQuestionViewDto> ChallengeList { get; set; }
        public List<ChallengeFolderDto> folderList { get; set; }
        public PointsRankDto PointsRank { get; set; }
        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage { get; set; } = 0;
        /// <summary>
        /// 每页显示数
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 总条数
        /// </summary>
        public int Total { get; set; } = 0;


    }
    /// <summary>
    /// 挑战问题详情
    /// </summary>
    public class ProblemViewDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid id { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string questionText { get; set; }
        /// <summary>
        /// 提交数
        /// </summary>
        public int? submitNum { get; set; }
        /// <summary>
        /// 通过数
        /// </summary>
        public int? passNum { get; set; }
        /// <summary>
        /// 分数
        /// </summary>
        public decimal score { get; set; }
        /// <summary>
        /// 难度
        /// </summary>
        public string hard { get; set; }
        public int seq { get; set; }
        /// <summary>
        /// 打开挑战题时间
        /// </summary>
        public int answerTime { get; set; }
        /// <summary>
        /// 是否含有程序参数
        /// </summary>
        public bool hasParam { get; set; }
        /// <summary>
        /// 是否含有输入流参数
        /// </summary>
        public bool hasInputParam { get; set; }
        /// <summary>
        /// 预设代码
        /// </summary>
        public string preinstallCode { get; set; }
    }
    /// <summary>
    /// 编译运行挑战
    /// </summary>
    public class ProblemCompileResultDto
    {
        /// <summary>
        /// Main参数
        /// </summary>
        public string param { get; set; }
        /// <summary>
        /// 输入流参数
        /// </summary>
        public string inputParam { get; set; }
        /// <summary>
        /// 编译结果
        /// </summary>
        public string compileResult { get; set; }
        /// <summary>
        /// 期待结果
        /// </summary>
        public string answer { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 编码 1:成功  2:结果错误  3:编译出现错误
        /// </summary>
        public int code { get; set; }
    }
    /// <summary>
    /// 提交挑战答案
    /// </summary>
    public class ProblemSubmitResultDto
    {
        /// <summary>
        /// 下一个挑战题ID
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 编码 1:成功  2:结果错误  3:编译出现错误 4:超时
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 得分
        /// </summary>
        public decimal score { get; set; }
        /// <summary>
        /// 代码输出
        /// </summary>
        public string result { get; set; }
        /// <summary>
        /// 挑战答案
        /// </summary>
        public string standardAnswer { get; set; }
    }
    #region 提交记录
    /// <summary>
    /// 挑战提交列表
    /// </summary>
    public class SubmissionListViewDto
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage { get; set; } = 0;


        /// <summary>
        /// 总条数
        /// </summary>
        public int Total { get; set; } = 0;
        /// <summary>
        /// 排名列表
        /// </summary>
        public List<SubmissionDto> SubmissionList { get; set; }

    }
    public class SubmissionDto
    {
        public Guid id { get; set; }
        /// <summary>
        /// 代码
        /// </summary>
        public string code { get; set; }
        public decimal score { get; set; }
        /// <summary>
        /// 是否通过 1表示通过
        /// </summary>
        public int isPass { get; set; }
        /// <summary>
        /// 提交时间
        /// </summary>
        public DateTime submiTime { get; set; }
        /// <summary>
        /// 挑战用时
        /// </summary>
        public int challengeTime { get; set; }

    }
    #endregion
    #region 排行榜
    /// <summary>
    /// 单项挑战排行榜
    /// </summary>
    [AutoMapFrom(typeof(RankSqlDto))]
    public class ChallengeRankDto
    {
        /// <summary>
        /// 排行
        /// </summary>
        public int rank { get; set; }
        /// <summary>
        /// 挑战者
        /// </summary>
        public string userName { get; set; }
  
        /// <summary>
        /// 提交次数
        /// </summary>
        public int submitTimes { get; set; }
        /// <summary>
        /// 挑战时间
        /// </summary>
        public int challengeTime { get; set; }
        /// <summary>
        /// 挑战时间
        /// </summary>
        public string time { get; set; }
        /// <summary>
        /// 是否通过
        /// </summary>
        public int isPass { get; set; }
        /// <summary>
        /// 得分
        /// </summary>
        public decimal? score { get; set; }
    }
    /// <summary>
    /// 排名显示
    /// </summary>
    public class RankListViewDto
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage { get; set; } = 0;


        /// <summary>
        /// 总条数
        /// </summary>
        public int Total { get; set; } = 0;
        /// <summary>
        /// 排名列表
        /// </summary>
        public List<ChallengeRankDto> RankList { get; set; }
        /// <summary>
        /// 我的排名
        /// </summary>
        public ChallengeRankDto Rank { get; set; }
    }
    /// <summary>
    /// 排名sql对应model
    /// </summary>
    public class RankSqlDto
    {
        public Guid? userId { get; set; }
        public string userName { get; set; }
        public decimal? score { get; set; }
        /// <summary>
        /// 提交次数
        /// </summary>
        public int? submitTimes { get; set; }
        /// <summary>
        /// 是否通过 1通过 0不通过
        /// </summary>
        public int? isPass { get; set; }
        /// <summary>
        /// 挑战用时
        /// </summary>
        public int challengeTime { get; set; }
        /// <summary>
        /// 排名
        /// </summary>
        public int? rank { get; set; }
    }
    #endregion
    #region 挑战排行榜
    /// <summary>
    /// 排行榜sql对应实体Model
    /// </summary>
    public class ChallengeLeaderboardDto
    {
        public Guid userId { get; set; }
        public string userName { get; set; }
        public string loginName{ get; set; }
        /// <summary>
        /// 总成绩
        /// </summary>
        public decimal score { get; set; }
        /// <summary>
        /// 院系
        /// </summary>
        public string facultyName { get; set; }
        /// <summary>
        /// 专业
        /// </summary>
        public string majorName { get; set; }
        /// <summary>
        /// 班级
        /// </summary>
        public string className { get; set; }
        /// <summary>
        /// 班级Id
        /// </summary>
        public Guid? classId { get; set; }
        /// <summary>
        /// 排名
        /// </summary>
        public int rank { get; set; }
    }

    public class ChallengeLeaderboardView
    {
        /// <summary>
        /// 排名列表
        /// </summary>
        public List<ChallengeLeaderboardDto> RankList { get; set; }
        /// <summary>
        /// 总记录数
        /// </summary>
        public int Total { get; set; } = 0;
        /// <summary>
        /// 我的得分排名
        /// </summary>
        public PointsRankDto PointsRank { get; set; }
    }

    public class ChallengeClassSelect
    {
        public string text { get; set; }
        public List<SelectDto> children { get; set; }
    }
    public class SelectDto
    {
        public string id { get; set; }
        public string text { get; set; }
    }

    #endregion
    }
