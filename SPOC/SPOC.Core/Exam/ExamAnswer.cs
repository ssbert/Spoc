using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using SPOC.Attribute;

namespace SPOC.Exam
{
    /// <summary>
    /// 考生答案
    /// </summary>
    public class ExamAnswer : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 考生成绩ID
        /// </summary>
        public Guid examGradeUid { get; set; }
         [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("examGradeUid")]
        public ExamGrade Grade { get; set; }
        /// <summary>
        /// 试题ID
        /// </summary>
        public Guid questionUid { get; set; }
         [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("questionUid")]
        public ExamQuestion Question { get; set; }
        /// <summary>
        /// 答题内容
        /// </summary>
        [StringLength(7600)]
        public string answerText { get; set; }
        /// <summary>
        /// 答题时间(以秒为单位)
        /// </summary>
        public int answerTime { get; set; }
        /// <summary>
        /// 得分
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal judgeScore { get; set; }
        /// <summary>
        /// 评卷结果态编号
        /// </summary>
        [StringLength(16)]
        public string judgeResultCode { get; set; }
        /// <summary>
        /// 评卷批注
        /// </summary>
        [StringLength(256)]
        public string judgeRemarks { get; set; }
        /// <summary>
        /// 得分点
        /// </summary>
        [StringLength(256)]
        public string judgeScoreText { get; set; }
        /// <summary>
        /// 评卷人ID
        /// </summary>
        public Guid judgeUserUid { get; set; }
      
        /// <summary>
        /// 评卷人姓名
        /// </summary>
        [StringLength(256)]
        public string judgeUserName { get; set; }
    }
}
