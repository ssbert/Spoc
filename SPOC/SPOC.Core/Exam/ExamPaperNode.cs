using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using SPOC.Attribute;

namespace SPOC.Exam
{
    /// <summary>
    /// 固定试卷大题
    /// </summary>
    public class ExamPaperNode : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        public ExamPaperNode()
        {
            paperNodeDesc = "";
        }

        #region Model

        /// <summary>
        /// 试卷ID
        /// </summary>
        public Guid paperUid { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("paperUid")]
        public ExamPaper Paper { get; set; }

        /// <summary>
        /// 题型ID
        /// </summary>
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid questionTypeUid { get; set; }

        //[ForeignKey("questionTypeUid")]
        //public ExamQuestionType QuestionType { get; set; }

        /// <summary>
        /// 试卷大题名称
        /// </summary>
        [StringLength(64)]
        [Required]
        public string paperNodeName { get; set; }

        /// <summary>
        /// 试题个数
        /// </summary>
        public int questionNum { get; set; }

        /// <summary>
        /// 每题分数
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal questionScore { get; set; }

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
        /// 试卷大题说明
        /// </summary>
        [DataType(DataType.Text)]
        public string paperNodeDesc { get; set; }

        /// <summary>
        /// 计划题目数
        /// </summary>
        public int planQuestionNum { get; set; }

        #endregion Model

    }
}

