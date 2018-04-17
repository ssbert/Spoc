using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Core
{
    /// <summary>
    /// 题库(含挑战题库)-编程题标准答案 与试题一对多
    /// </summary>
    [Table("question_standard_code")]
    public class QuestionStandardCode: Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 编程题ID
        /// </summary>
        public  Guid questionId { get; set; }

        /// <summary>
        /// 答案
        /// </summary>
        [DefaultValue("")]
        public string code { get; set; }
        /// <summary>
        /// 类型 challenge(挑战) | normal(默认题库)
        /// </summary>
        [StringLength(16)]
        [DefaultValue("normal")]
        public string type { get; set; }

        /// <summary>
        /// 是否默认答案 试题管理教师配置的标准答案为默认答案
        /// </summary>
        [DefaultValue(0)]
        public byte isDefault { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime modifyTime { get; set; }
    }
}
