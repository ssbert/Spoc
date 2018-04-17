using Abp.AutoMapper;
using System;
using Newtonsoft.Json;

namespace SPOC.ExamPaper.Dto
{
    /// <summary>
    /// 输出
    /// </summary>
    [AutoMapFrom(typeof(Exam.ExamPaper))]
    public class ExamPaperOutputDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 课程ID
        /// </summary>
        public Guid courseUid { get; set; }
        /// <summary>
        /// 试卷分类ID
        /// </summary>
        public Guid folderUid { get; set; }
        /// <summary>
        /// 试卷编号
        /// </summary>
        public string paperCode { get; set; }
        /// <summary>
        /// 是否是自定义编号
        /// </summary>
        public bool isCustomCode { get; set; }
        /// <summary>
        /// 试卷名称
        /// </summary>
        public string paperName { get; set; }
        /// <summary>
        /// 单选变为不定项
        /// </summary>
        public string isSingleAsMulti { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string statusCode { get; set; }
        /// <summary>
        /// 创建方式（fix 为固定试题，random 为随机试卷）
        /// </summary>
        public string paperTypeCode { get; set; }
        /// <summary>
        /// 策略ID
        /// </summary>
        public Guid policyUid { get; set; }
        /// <summary>
        /// 总分
        /// </summary>
        public decimal totalScore { get; set; }
        /// <summary>
        /// 试题个数
        /// </summary>
        public int questionNum { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remarks { get; set; }
        /// <summary>
        /// 创建者ID
        /// </summary>
        public Guid creatorUid { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime createTime { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime lastUpdateTime { get; set; }
        /// <summary>
        /// 试卷xml最后更新的时间
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime? paperXmlLastUpdateTime { get; set; }
        /// <summary>
        /// 每选项分数（用于多选题不全对时选对或选错一项时的得扣分）
        /// </summary>
        public decimal eachOptionScore { get; set; }
        /// <summary>
        /// 是否显示分数
        /// </summary>
        public string isShowScore { get; set; }
        /// <summary>
        /// 计划分数
        /// </summary>
        public Nullable<decimal> planTotalScore { get; set; }
        /// <summary>
        /// 试卷类型（exam考试, task作业，testing 测评exam考试, task作业，testing 测评）
        /// </summary>
        public string paperClassCode { get; set; }
        /// <summary>
        /// 试卷的XML
        /// </summary>
        public string paperXml { get; set; }
        /// <summary>
        /// 难度系数
        /// </summary>
        public string paperHardGrade { get; set; }
        /// <summary>
        /// 过期日期
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime? outdatedDate { get; set; }

        /// <summary>
        /// 移动端标识
        /// </summary>
        public string mobileFlag { get; set; }
        /// <summary>
        /// 学科Id
        /// </summary>
        public Guid subjectUid { get; set; }
        /// <summary>
        /// 组织架构ID
        /// </summary>
        public Guid departmentUid { get; set; }
        /// <summary>
        /// 试卷的XML
        /// </summary>
        public string paperExtend01 { get; set; }
    }
}
