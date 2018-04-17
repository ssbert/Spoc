using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Exercises
{
    /// <summary>
    /// 练习
    /// </summary>
    [Table("exercise")]
    public class Exercise : Entity<Guid>
    {
        public Exercise()
        {
            Id = Guid.NewGuid();
            CreateTime = DateTime.Now;
        }
        [Column("id")]
        public override Guid Id { get; set; }

        /// <summary>
        /// 试题Id
        /// </summary>
        [Column("questionId")]
        public Guid QuestionId { get; set; }

        /// <summary>
        /// 创建者Id
        /// </summary>
        [Column("creatorId")]
        public Guid CreatorId { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Column("title"), StringLength(256), Required]
        public string Title { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("createTime")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Column("endTime")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 是否显示答案
        /// </summary>
        [Column("showAnswer")]
        public bool ShowAnswer { get; set; }

        /// <summary>
        /// 显示答案的类型
        /// 0：练习时间结束后显示
        /// 1：任何时候都显示
        /// </summary>
        [Column("showAnswerType")]
        public byte ShowAnswerType { get; set; }
    }
}