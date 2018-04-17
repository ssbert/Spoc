using Abp.AutoMapper;
using Abp.Runtime.Validation;
using SPOC.Exam;
using System;
using System.ComponentModel.DataAnnotations;

namespace SPOC.ExamPaper.Dto
{
    [AutoMapTo(typeof(ExamPaperNode))]
    public class ExamPaperNodeInputDto: IShouldNormalize, ICustomValidate
    {
        public ExamPaperNodeInputDto()
        {
            Id = Guid.Empty;
        }
        public Guid Id { get; set; }
        /// <summary>
        /// 试卷ID
        /// </summary>
        public Guid paperUid { get; set; }

        /// <summary>
        /// 题型ID
        /// </summary>
        public Guid questionTypeUid { get; set; }

        /// <summary>
        /// 试卷大题名称
        /// </summary>
        [Required, MaxLength(64)]
        public string paperNodeName { get; set; }

        /// <summary>
        /// 试题个数
        /// </summary>
        public int questionNum { get; set; }

        /// <summary>
        /// 每题分数
        /// </summary>
        public decimal questionScore { get; set; }

        /// <summary>
        /// 大题总分
        /// </summary>
        public decimal totalScore { get; set; }

        /// <summary>
        /// 大题顺序
        /// </summary>
        public int listOrder { get; set; }

        /// <summary>
        /// 试卷大题说明
        /// </summary>
        public string paperNodeDesc { get; set; }

        /// <summary>
        /// 计划题目数
        /// </summary>
        public int planQuestionNum { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(paperNodeDesc))
            {
                paperNodeDesc = "";
            }
        }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (paperUid == Guid.Empty)
            {
                context.Results.Add(new ValidationResult("paperUid为必填字段"));
            }
        }
    }
}