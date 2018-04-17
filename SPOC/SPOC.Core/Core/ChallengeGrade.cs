using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using SPOC.Attribute;
using SPOC.Category;
using SPOC.Exam;

namespace SPOC.Core
{
    /// <summary>
    /// 挑战成绩表
    /// </summary>
    [Table("challenge_grade")]
    public class ChallengeGrade : Entity<Guid>
    {
       
        #region Model
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 挑战人ID
        /// </summary>
        public Guid userId { get; set; }

        /// <summary>
        /// 挑战题ID
        /// </summary>
        public Guid questionId { get; set; }
        /// <summary>
        /// 答题内容
        /// </summary>
        public string answer { get; set; }
        /// <summary>
        /// 成绩得分
        /// </summary>
        [DecimalPrecision(5, 2)]
        public decimal score { get; set; }
        /// <summary>
        /// 是否通过
        /// </summary>
        [DefaultValue(0)]
        public byte isPass { get; set; }
        /// <summary>
        /// 返回结果
        /// </summary>
        public string result { get; set; }
        /// <summary>
        /// 挑战时间 答题开始到结束
        /// </summary>
        public int challengeTime { get; set; }
        /// <summary>
        /// 提交时间
        /// </summary>
        public DateTime createTime { get; set; }
        #endregion Model

    }
}

