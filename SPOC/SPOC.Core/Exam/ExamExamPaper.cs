using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Exam
{
    /// <summary>
    /// 考试用到随机试卷关联表, 1考试 ： n随机试卷
    /// </summary>
    public class ExamExamPaper : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 考试ID
        /// </summary>
        public Guid examUid { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("examUid")]
        public ExamExam Exam { get; set; }
        /// <summary>
        /// 试卷ID
        /// </summary>
        public Guid paperUid { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("paperUid")]
        public ExamPaper Paper { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        [StringLength(1)]
        [DefaultValue("Y")]
        public string isActive { get; set; }
        /// <summary>
        /// 生成时间
        /// </summary>
        public DateTime createTime { get; set; }
    }
}
