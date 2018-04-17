using System;

namespace SPOC.Statement.Dto
{
    /// <summary>
    /// 班级排名报表项
    /// </summary>
    public class ClassRankingItem
    {
        /// <summary>
        /// 班级ID
        /// </summary>
        public Guid ClassId { get; set; }
        /// <summary>
        /// 班级名称
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 参与率
        /// </summary>
        public decimal JoinRate { get; set; }
        /// <summary>
        /// 通过率
        /// </summary>
        public decimal PassRate { get; set; }
        /// <summary>
        /// 最高分
        /// </summary>
        public decimal? MaxScore { get; set; }
        /// <summary>
        /// 最低分
        /// </summary>
        public decimal? MinScore { get; set; }
        /// <summary>
        /// 平均分
        /// </summary>
        public decimal? AverageScore { get; set; }
        /// <summary>
        /// 通过人数
        /// </summary>
        public int PassNum { get; set; }
        /// <summary>
        /// 参加考试人数
        /// </summary>
        public int JoinNum { get; set; }
        /// <summary>
        /// 班级总人数
        /// </summary>
        public int StudentNum { get; set; }
        /// <summary>
        /// 排名
        /// </summary>
        public int? Ranking { get; set; }
        /// <summary>
        /// 0-49分人数
        /// </summary>
        public int ScoreSectionNum1 { get; set; }
        /// <summary>
        /// 50-59分人数
        /// </summary>
        public int ScoreSectionNum2 { get; set; }
        /// <summary>
        /// 60-69分人数
        /// </summary>
        public int ScoreSectionNum3 { get; set; }
        /// <summary>
        /// 70-79分人数
        /// </summary>
        public int ScoreSectionNum4 { get; set; }
        /// <summary>
        /// 80-89分人数
        /// </summary>
        public int ScoreSectionNum5 { get; set; }
        /// <summary>
        /// 90-100分人数
        /// </summary>
        public int ScoreSectionNum6 { get; set; }
    }
}