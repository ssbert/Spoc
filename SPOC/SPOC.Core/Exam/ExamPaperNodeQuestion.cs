using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using SPOC.Attribute;

namespace SPOC.Exam
{
    /// <summary>
    /// 固定试卷大题与实体链接
    /// </summary>
    public class ExamPaperNodeQuestion : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        public ExamPaperNodeQuestion()
        {
        }

        #region Model

        /// <summary>
        /// 试卷大题ID
        /// </summary>
        public Guid paperNodeUid { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("paperNodeUid")]
        public ExamPaperNode PaperNode { get; set; }
        /// <summary>
        /// 试题ID
        /// </summary>
        public Guid questionUid { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("questionUid")]
        public ExamQuestion Question { get; set; }
        /// <summary>
        /// 试卷ID（为了方便查询的冗余字段）
        /// </summary>
        public Guid paperUid { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("paperUid")]
        public ExamPaper Paper { get; set; }

        /// <summary>
        /// 试题在试卷中的分数
        /// </summary>
        [DecimalPrecision(18,2)]
        public decimal paperQuestionScore { get; set; }

        /// <summary>
        /// 答题时限（以秒为单位）
        /// </summary>
        public int paperQuestionExamTime { get; set; }

        /// <summary>
        /// 试题顺序
        /// </summary>
        public int listOrder { get; set; }
        
        /// <summary>
        /// 记录更新时间？
        /// </summary>
        public DateTime dataUpdateTime { get; set; }

        #endregion Model

    }
}

