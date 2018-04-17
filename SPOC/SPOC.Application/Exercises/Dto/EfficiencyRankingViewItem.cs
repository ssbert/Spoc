using System;

namespace SPOC.Exercises.Dto
{
    /// <summary>
    /// 效率排行榜
    /// </summary>
    public class EfficiencyRankingViewItem
    {
        /// <summary>
        /// 排名
        /// </summary>
        public int Ranking { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 练习次数
        /// </summary>
        public int ExerciseCount { get; set; }
        /// <summary>
        /// 消耗时间
        /// </summary>
        public TimeSpan? UseTime { get; set; }
        /// <summary>
        /// 是否通过
        /// </summary>
        public bool IsPass { get; set; }
    }
}