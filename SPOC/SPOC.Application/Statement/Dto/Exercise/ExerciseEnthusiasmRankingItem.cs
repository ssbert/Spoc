using System;
using Newtonsoft.Json;

namespace SPOC.Statement.Dto.Exercise
{
    /// <summary>
    /// 练习积极性排行榜分页项
    /// </summary>
    public class ExerciseEnthusiasmRankingItem
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string UserFullName { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserLoginName { get; set; }
        /// <summary>
        /// 班级ID
        /// </summary>
        public Guid ClassId { get; set; }
        /// <summary>
        /// 班级名
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 是否通过
        /// </summary>
        public bool IsPass { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime? BeginTime { get; set; }
        /// <summary>
        /// 总排名
        /// </summary>
        public int Ranking { get; set; }
        /// <summary>
        /// 班级名
        /// </summary>
        public int ClassRanking { get; set; }
    }
}