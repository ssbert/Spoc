using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace SPOC.Exercises.Dto
{
    /// <summary>
    /// 练习inputDto
    /// </summary>
    [AutoMapTo(typeof(Exercise))]
    public class ExerciseInputDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 试题ID
        /// </summary>
        public Guid QuestionId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [StringLength(256), Required]
        public string Title { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 是否显示答案
        /// </summary>
        public bool ShowAnswer { get; set; }

        /// <summary>
        /// 显示答案的类型
        /// 0：考试时间结束后显示
        /// 1：任何时候都显示
        /// </summary>
        public byte ShowAnswerType { get; set; }

    }
}