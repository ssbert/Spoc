using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using SPOC.Attribute;

namespace SPOC.Exam
{
    /// <summary>
    /// 随机试卷大题
    /// </summary>
    public class ExamPolicyNode : Entity<Guid>
    {
        public ExamPolicyNode()
        {
            policyNodeDesc = "";
        }
        #region Model
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 随机试卷ID
        /// </summary>
        public Guid policyUid { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("policyUid")]
        public ExamPolicy Policy { get; set; }
        /// <summary>
        /// 题型（可以为空只是辅助作用）
        /// </summary>
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid questionTypeUid { get; set; }
        /// <summary>
        /// 随机试卷大题标题
        /// </summary>
        [StringLength(64)]
        [Required]
        public string policyNodeName { get; set; }
        /// <summary>
        /// 试题格数
        /// </summary>
        public int questionNum { get; set; }
        /// <summary>
        /// 大题总分
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal totalScore { get; set; }
        /// <summary>
        /// 大题顺序
        /// </summary>
        public int listOrder { get; set; }
        /// <summary>
        /// 大题说明
        /// </summary>
        public string policyNodeDesc { get; set; }
        #endregion Model

    }
}

