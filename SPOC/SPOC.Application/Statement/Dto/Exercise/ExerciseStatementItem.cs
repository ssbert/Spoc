using System;
using Newtonsoft.Json;

namespace SPOC.Statement.Dto.Exercise
{
    /// <summary>
    /// 练习报表项
    /// </summary>
    public class ExerciseStatementItem
    {
        /// <summary>
        /// 练习ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 练习名称
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 通过率
        /// </summary>
        public float PassRate { get; set; }
        /// <summary>
        /// 通过人数
        /// </summary>
        public int PassNum { get; set; }
        /// <summary>
        /// 未通过人数
        /// </summary>
        public int FailNum { get; set; }
        /// <summary>
        /// 参加率
        /// </summary>
        public float JoinRate { get; set; }
        /// <summary>
        /// 参加人数
        /// </summary>
        public int JoinNum { get; set; }
        /// <summary>
        /// 未参加人数
        /// </summary>
        public int WithoutNum { get; set; }
        /// <summary>
        /// 总学生人数
        /// </summary>
        public int StudentNum { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建者Id
        /// </summary>
        public Guid CreatorId { get; set; }
        /// <summary>
        /// 创建者姓名
        /// </summary>
        public string UserFullName { get; set; }
        /// <summary>
        /// 创建者用户名
        /// </summary>
        public string UserLoginName { get; set; }
    }
}