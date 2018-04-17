using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Lib
{
    /// <summary>
    /// 用户作答记录
    /// </summary>
    [Table("user_answer_records")]
    public class UserAnswerRecords : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        
        /// <summary>
        /// 试题Id
        /// </summary>
        [Column("questionId")]
        public Guid QuestionId { get; set; }

        /// <summary>
        /// 标签Id
        /// </summary>
        [Column("labelId")]
        public Guid LabelId { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        [Column("userId")]
        public Guid UserId { get; set; }

        /// <summary>
        /// 作答详细Id
        /// examGradeId, exerciseRecordId, challengeGradeId
        /// </summary>
        [Column("recordId")]
        public Guid RecordId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("createTime")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 来源 (exam, exercise, challenge)
        /// </summary>
        [Column("source"), StringLength(16)]
        public string Source { get; set; }

        /// <summary>
        /// 标签分值（只用来记录加扣分，不参与实际计算）
        /// </summary>
        [Column("score")]
        public int Score { get; set; }
        
    }
}