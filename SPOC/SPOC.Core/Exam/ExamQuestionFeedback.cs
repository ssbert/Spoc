using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using SPOC.User;

namespace SPOC.Exam
{
    /// <summary>
    /// 试题反馈
    /// </summary>
    public class ExamQuestionFeedback : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 试题ID
        /// </summary>
        public Guid questionUid { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("questionUid")]
        public ExamQuestion Question { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid userUid { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("userUid")]
        public UserBase User { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [StringLength(64)]
        public string userName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createTime { get; set; }

        /// <summary>
        /// 所在试卷ID
        /// </summary>
        public Guid paperUid { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("paperUid")]
        public ExamPaper Paper { get; set; }

        /// <summary>
        /// 考试成绩ID
        /// </summary>
        public Guid examGradeUid { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("examGradeUid")]
        public ExamGrade Grade { get; set; }

        /// <summary>
        /// 考试ID
        /// </summary>
        public Guid examUid { get; set; }

        [ForeignKey("examUid")]
        public ExamExam Exam { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [StringLength(16)]
        public string statusCode { get; set; }

        /// <summary>
        /// 反馈内容
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 处理者用户ID
        /// </summary>
        [StringLength(64)]
        public string processUser { get; set; }

        /// <summary>
        /// 处理结果
        /// </summary>
        public string result { get; set; }

        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime updateTime { get; set; }
    }
}
