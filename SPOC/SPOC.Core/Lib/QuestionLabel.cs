using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;

namespace SPOC.Lib
{
    [Table("question_label")]
    public class QuestionLabel : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 题库类型 （challenge:挑战题 normal:题库）
        /// </summary>
        [StringLength(16)]
        [DefaultValue("normal")]
        public string questionType { get; set; }
        /// <summary>
        /// 题库Id
        /// </summary>
        public Guid questionId { get; set; }
        /// <summary>
        /// 标签Id
        /// </summary>
        public Guid labelId { get; set; }
        /// <summary>
        /// 标签类型 0:辅标签 1:主标签
        /// </summary>

        [DefaultValue(0)]
        public byte labelType { get; set; }
    }
}
