using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Exam
{
    /// <summary>
    /// 多人评卷策略表
    /// </summary>
    public class ExamJudgeMany : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 考试编号
        /// </summary>
        public Guid examUid { get; set; }

        [ForeignKey("examUid")]
        public ExamExam Exam { get; set; }

        /// <summary>
        /// 评分模式（all：所有人评分one：只要有一个人评分）
        /// </summary>
        [StringLength(36)]
        public string judgeModeCode { get; set; }

        /// <summary>
        /// 计算模式（avg:取平均分max:取最高分min:取最低分assign:取指定人interface:取指定接口）
        /// </summary>
        [StringLength(64)]
        public string judgePolicyCode { get; set; }

        /// <summary>
        /// 指定评分人
        /// </summary>
        public Guid judgeUserUid { get; set; }
        [ForeignKey("judgeUserUid")]
        public User.UserBase User { get; set; }

        /// <summary>
        /// 计算接口
        /// </summary>
        [StringLength(2000)]
        public string @interface { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remarks { get; set; }
    }
}
