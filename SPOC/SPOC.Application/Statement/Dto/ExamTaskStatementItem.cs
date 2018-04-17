using System;
using Newtonsoft.Json;

namespace SPOC.Statement.Dto
{
    /// <summary>
    /// 考试报表项
    /// </summary>
    public class ExamTaskStatementItem
    {
        /// <summary>
        /// 考试任务ID
        /// </summary>
        public Guid TaskId { get; set; }
        /// <summary>
        /// 考试ID
        /// </summary>
        public Guid ExamId { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 考试任务标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 创建人Id
        /// </summary>
        public Guid CreatorId { get; set; }
        /// <summary>
        /// 创建人登录名
        /// </summary>
        public string UserLoginName { get; set; }
        /// <summary>
        /// 创建者姓名
        /// </summary>
        public string UserFullName { get; set; }
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
        /// 参加人数
        /// </summary>
        public int JoinNum { get; set; }
        /// <summary>
        /// 未参加人数
        /// </summary>
        public int WithoutNum { get; set; }
        /// <summary>
        /// 已出成绩人数
        /// </summary>
        public int CompiledNum { get; set; }
        /// <summary>
        /// 未统计人数
        /// </summary>
        public int UnCompiledNum { get; set; }
        /// <summary>
        /// 提交人数
        /// </summary>
        public int SubmitNum { get; set; }
        /// <summary>
        /// 未提交人数
        /// </summary>
        public int UnSubmitNum { get; set; }
        /// <summary>
        /// 通过人数
        /// </summary>
        public int PassNum { get; set; }
        /// <summary>
        /// 未通过人数
        /// </summary>
        public int FailNum { get; set; }
        /// <summary>
        /// 总人数
        /// </summary>
        public int StudentNum { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 有补考
        /// </summary>
        public bool HasReTest { get; set; }
    }
}