using Abp.AutoMapper;
using SPOC.Exam;
using System;
using Newtonsoft.Json;

namespace SPOC.ExamPaper.Dto
{
    [AutoMap(typeof(Exam.ExamPaper), typeof(ExamPolicy))]
    public class ExamPaperDto
    {
        public Guid Id { get; set; }

        public string paperCode { get; set; }

        public bool isCustomCode { get; set; }

        public string paperName { get; set; }
        
        /// <summary>
        /// 单选变不定项
        /// </summary>
        public string isSingleAsMulti { get; set; }

        public string paperTypeCode { get; set; }

        public Guid policyUid { get; set; }

        public decimal totalScore { get; set; }

        public string remarks { get; set; }
        /// <summary>
        /// 难度系数
        /// </summary>
        public string paperHardGrade { get; set; }
        /// <summary>
        /// 过期日期
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime? outdatedDate { get; set; }

        public Guid folderUid { get; set; }
    }
}