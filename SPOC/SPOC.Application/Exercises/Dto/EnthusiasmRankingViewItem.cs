using System;

namespace SPOC.Exercises.Dto
{
    /// <summary>
    /// 前台积极性排行榜数据项
    /// </summary>
    public class EnthusiasmRankingViewItem
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
        /// 开始时间
        /// </summary>
        public DateTime? BeginTime { get; set; }
        /// <summary>
        /// 是否通过
        /// </summary>
        public bool IsPass { get; set; }
    }
}