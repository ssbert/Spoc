using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Exam
{
    /// <summary>
    /// 试题类型
    /// </summary>
    public class ExamQuestionType : Entity<Guid>
    {

        #region Model
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 试题编号
        /// </summary>
        [StringLength(16)]
        [Required]
        public string questionBaseTypeCode { get; set; }

        /// <summary>
        /// 题型
        /// </summary>
        [StringLength(64)]
        [Required]
        public string questionTypeName { get; set; }

        /// <summary>
        /// 默认分数
        /// </summary>
        public decimal defaultQuestionScore { get; set; }

        /// <summary>
        /// 评分组件名
        /// </summary>
        [StringLength(255)]
        public string judgeDeviceName { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int listOrder { get; set; }

        #endregion Model

    }
}

