using System;

namespace SPOC.Statement.Dto.Exercise
{
    /// <summary>
    /// 练习报表班级排行
    /// </summary>
    public class ExerciseClassRankingItem
    {
        /// <summary>
        /// 班级Id
        /// </summary>
        public Guid ClassId { get; set; }
        /// <summary>
        /// 班级名
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 通过率
        /// </summary>
        public float PassRate { get; set; }
        /// <summary>
        /// 参加率
        /// </summary>
        public float JoinRate { get; set; }
        /// <summary>
        /// 通过人数
        /// </summary>
        public int PassNum { get; set; }
        /// <summary>
        /// 参加人数
        /// </summary>
        public int JoinNum { get; set; }
        /// <summary>
        /// 总学生人数
        /// </summary>
        public int StudentNum { get; set; }
        /// <summary>
        /// 排名
        /// </summary>
        public int Ranking { get; set; }
    }
}