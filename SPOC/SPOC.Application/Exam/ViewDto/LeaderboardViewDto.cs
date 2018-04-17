using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPOC.Exam.ViewDto
{
    /// <summary>
    /// 排行榜显示
    /// </summary>
    public class LeaderboardViewDto
    {
        public string ClassName { get; set; }
        /// <summary>
        /// 等级数据统计
        /// </summary>
        public LeaderboardLevelViewDto LevelView { get; set; }
        /// <summary>
        /// 班级成绩统计排名情况
        /// </summary>
        public List<GradeRankItem> GradeRankList { get; set; }
        /// <summary>
        /// 个人成绩统计情况
        /// </summary>
        public UserGradeRank UserGradeRank { get; set; }
    }
    /// <summary>
    /// 考试分数等级人数统计
    /// </summary>
    public class LeaderboardLevelViewDto
    {
        /// <summary>
        /// 0-49 分数级
        /// </summary>
        public int Level0 { get; set; }
        /// <summary>
        /// 50-59 分数级
        /// </summary>
        public int Level1 { get; set; }
        /// <summary>
        /// 60-69 分数级
        /// </summary>
        public int Level2 { get; set; }
        /// <summary>
        /// 70-79 分数级
        /// </summary>
        public int Level3 { get; set; }
        /// <summary>
        /// 80-89 分数级
        /// </summary>
        public int Level4 { get; set; }
        /// <summary>
        /// 90-100 分数级
        /// </summary>
        public int Level5 { get; set; }
        /// <summary>
        /// 总人数
        /// </summary>
        public int Total { get; set; }
    }

    public class UserGradeRank
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 所在成绩区间
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 我的成绩
        /// </summary>
        public decimal? Score { get; set; }
        /// <summary>
        /// 班级中排名
        /// </summary>
        public int RankingInClass { get; set; }
        /// <summary>
        /// 总排名
        /// </summary>
        public int Ranking { get; set; }
    }

    public class GradeRankItem
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string UserFullName { get; set; }

        /// <summary>
        /// 得分
        /// </summary>
        public decimal? Score { get; set; }
        /// <summary>
        /// 班级中排名
        /// </summary>
        public int RankingInClass { get; set; }
        /// <summary>
        /// 总排名
        /// </summary>
        public int Ranking { get; set; }
       
        /// <summary>
        /// 院系名称
        /// </summary>
        public string FacultyName { get; set; }
     
        /// <summary>
        /// 专业名称
        /// </summary>
        public string MajorName { get; set; }
      
        /// <summary>
        /// 班级名称
        /// </summary>
        public string ClassName { get; set; }
       

    }

}
