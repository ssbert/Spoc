using System;

namespace SPOC.Statement.Dto
{
    /// <summary>
    /// 成绩排名项
    /// </summary>
    public class GradeRankingItem
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
        /// 用户登陆名
        /// </summary>
        public string UserLoginName { get; set; }
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
        /// 院系ID
        /// </summary>
        public Guid FacultyId { get; set; }
        /// <summary>
        /// 院系名称
        /// </summary>
        public string FacultyName { get; set; }
        /// <summary>
        /// 专业ID
        /// </summary>
        public Guid MajorId { get; set; }
        /// <summary>
        /// 专业名称
        /// </summary>
        public string MajorName { get; set; }
        /// <summary>
        /// 班级ID
        /// </summary>
        public Guid ClassId { get; set; }
        /// <summary>
        /// 班级名称
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 院系/专业/班级
        /// </summary>
        public string ClassFullName => FacultyName + "/" + MajorName + "/" + ClassName;
        /// <summary>
        /// 参加了考试
        /// </summary>
        public bool JoinExam { get; set; }

    }
}