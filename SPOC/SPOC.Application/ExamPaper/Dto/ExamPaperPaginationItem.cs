using System;
using Abp.AutoMapper;
using Newtonsoft.Json;

namespace SPOC.ExamPaper.Dto
{
    [AutoMapFrom(typeof(Exam.ExamPaper))]
    public class ExamPaperPaginationItem
    {
        public Guid Id { get; set; }
        public string paperName { get; set; }
        public string paperCode { get; set; }
        public string paperTypeCode { get; set; }
        public string remarks { get; set; }
        public string userLoginName { get; set; }
        public string userFullName { get; set; }
        [JsonConverter(typeof(DateFormat))]
        public DateTime createTime { get; set; }
    }
}