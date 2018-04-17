using Abp.AutoMapper;
using SPOC.Exam;
using System;

namespace SPOC.ExamPaper.Dto
{
    [AutoMapFrom(typeof(ExamPaperNodeQuestion), typeof(ExamQuestion), typeof(ExamQuestionType))]
    public class ExamPaperNodeQuestionOutputDto
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 试卷大题ID
        /// </summary>
        public Guid paperNodeUid { get; set; }
        /// <summary>
        /// 试题ID
        /// </summary>
        public Guid questionUid { get; set; }
        /// <summary>
        /// 试卷ID（为了方便查询的冗余字段）
        /// </summary>
        public Guid paperUid { get; set; }
        /// <summary>
        /// 试题在试卷中的分数
        /// </summary>
        public decimal paperQuestionScore { get; set; }

        /// <summary>
        /// 答题时限（以秒为单位）
        /// </summary>
        public int paperQuestionExamTime { get; set; }

        /// <summary>
        /// 试题顺序
        /// </summary>
        public int listOrder { get; set; }

        public string questionCode { get; set; }

        //题干
        public string questionText { get; set; }
        //试题类型
        public string questionTypeName { get; set; }
    }
}