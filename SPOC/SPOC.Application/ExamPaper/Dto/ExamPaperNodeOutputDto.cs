using Abp.AutoMapper;
using SPOC.Exam;
using System;

namespace SPOC.ExamPaper.Dto
{
    [AutoMapFrom(typeof(ExamPaperNode))]
    public class ExamPaperNodeOutputDto
    {
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

        public string questionBaseTypeCode { get; set; }
        public string questionTypeName { get; set; }
    }
}
