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
    /// 随机试卷
    /// </summary>
    public class ExamPolicy : Entity<Guid>
    {
        public ExamPolicy()
        {
            paperHardGrade = "";
            remarks = "";
            isShowScore = "Y";
            isConvertScore = "N";
            isSingleAsMulti = "N";
            courseUid = Guid.Empty;
        }

        #region Model
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 随机试卷分类ID
        /// </summary>
        public Guid folderUid { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("folderUid")]
        public NvFolder Folder { get; set; }

        /// <summary>
        /// 随机试卷编号
        /// </summary>
        [StringLength(64)]
        [Required]
        public string policyCode { get; set; }

        /// <summary>
        /// 是否是自定义编号
        /// </summary>
        public bool isCustomCode { get; set; }

        /// <summary>
        /// 随机试卷名称
        /// </summary>
        [StringLength(64)]
        [Required]
        public string policyName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [StringLength(36), DefaultValue("approved")]
        [Required]
        public string statusCode { get; set; }

        /// <summary>
        /// 试卷类型（exam 考试，task 作业, testing 测评）
        /// </summary>
        [StringLength(16), DefaultValue("exam")]
        [Required]
        public string paperClassCode { get; set; }

        /// <summary>
        /// 单选变不定项
        /// </summary>
        [StringLength(1)]
        [DefaultValue("N")]
        public string isSingleAsMulti { get; set; }

        /// <summary>
        /// 是否折算分数
        /// </summary>
        [StringLength(1)]
        [DefaultValue("N")]
        public string isConvertScore { get; set; }

        /// <summary>
        /// 卷面总分数
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal policyTotalScore { get; set; }

        /// <summary>
        /// 试卷总分
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
        /// 创建者
        /// </summary>
        public Guid creatorUid { get; set; }
      

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createTime { get; set; }

        /// <summary>
        /// 每选项分数（用于多选题不全对时选对或选错一项时的得扣）
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal eachOptionScore { get; set; }

        /// <summary>
        /// 是否显示分数
        /// </summary>
        [StringLength(1)]
        [DefaultValue("Y")]
        public string isShowScore { get; set; }

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
        /// 学科
        /// </summary>
        public Guid subjectUid { get; set; }

        /// <summary>
        /// 组织架构
        /// </summary>
        public Guid departmentUid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [StringLength(10)]
        public string mobileFlag { get; set; }

        /// <summary>
        ///  关联课程ID
        /// </summary>
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid courseUid { get; set; }

        #endregion Model

    }
}

