using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using SPOC.Attribute;
using SPOC.Category;

namespace SPOC.Exam
{
    /// <summary>
    /// 固定试卷
    /// </summary>
    public class ExamPaper : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        public ExamPaper()
        {
            isSingleAsMulti = "N";
            isShowScore = "Y";
            remarks = "";
            paperHardGrade = "";
            paperXml = "";
            courseUid = Guid.Empty;
            paperClassCode = "exam";
            statusCode = "approved";
            policyUid = Guid.Empty;
        }
        /// <summary>
        /// 课程ID
        /// </summary>
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid courseUid { get; set; }
        /// <summary>
        /// 试卷分类ID
        /// </summary>
        public Guid folderUid { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("folderUid")]
        public NvFolder Folder { get; set; }
        /// <summary>
        /// 试卷编号
        /// </summary>
        [StringLength(64)]
        [Required]
        public string paperCode { get; set; }

        /// <summary>
        /// 是否是自定义编号
        /// </summary>
        [Required]
        public bool isCustomCode { get; set; }
        /// <summary>
        /// 试卷名称
        /// </summary>
        [StringLength(256)]
        [Required]
        public string paperName { get; set; }
        /// <summary>
        /// 单选变为不定项
        /// </summary>
        [StringLength(1)]
        [DefaultValue("N")]
        [Required]
        public string isSingleAsMulti { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [StringLength(36)]
        [Required]
        public string statusCode { get; set; }
        /// <summary>
        /// 创建方式（fix 为固定试题，random 为随机试卷）
        /// </summary>
        [StringLength(16)]
        [Required]
        public string paperTypeCode { get; set; }
        /// <summary>
        /// 策略ID
        /// </summary>
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid policyUid { get; set; }
        /// <summary>
        /// 总分
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal totalScore { get; set; }
        /// <summary>
        /// 试题个数
        /// </summary>
        public int questionNum { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(256)]
        public string remarks { get; set; }
        /// <summary>
        /// 创建者ID
        /// </summary>
        public Guid creatorUid { get; set; }
      
        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        public DateTime createTime { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        [Required]
        public DateTime lastUpdateTime { get; set; }
        /// <summary>
        /// 试卷xml最后更新的时间
        /// </summary>
        public DateTime? paperXmlLastUpdateTime { get; set; }
        /// <summary>
        /// 每选项分数（用于多选题不全对时选对或选错一项时的得扣分）
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal eachOptionScore { get; set; }
        /// <summary>
        /// 是否显示分数
        /// </summary>
        [StringLength(1)]
        [DefaultValue("Y")]
        [Required]
        public string isShowScore { get; set; }
        /// <summary>
        /// 计划分数
        /// </summary>
        [DecimalPrecision(18, 2)]
        public Nullable<decimal> planTotalScore { get; set; }
        /// <summary>
        /// 试卷类型（exam考试, task作业，testing 测评exam考试, task作业，testing 测评）
        /// </summary>
        [StringLength(16)]
        [Required]
        public string paperClassCode { get; set; }
        /// <summary>
        /// 试卷的XML
        /// </summary>
        public string paperXml { get; set; }
        /// <summary>
        /// 难度系数
        /// </summary>
        [StringLength(36)]
        public string paperHardGrade { get; set; }
        /// <summary>
        /// 过期日期
        /// </summary>
        public DateTime? outdatedDate { get; set; }
       
        /// <summary>
        /// 移动端标识
        /// </summary>
        [StringLength(10)]
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
