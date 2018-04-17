using System;
using Newtonsoft.Json;

namespace SPOC.QuestionBank.Dto
{
    public class QuestionItem
    {
        public Guid Id { get; set; }
        public string title { get; set; }
        public string language { get; set; }
        public Guid questionTypeUid { get; set; }
        public string questionCode { get; set; }
        public Guid parentQuestionUid { get; set; }
        public string questionTypeName { get; set; }
        public string questionBaseTypeCode { get; set; }
        public string questionText { get; set; }
        public string questionPureText { get; set; }
        public string hardGrade { get; set; }
        public decimal score { get; set; }
        public int outdatedDate { get; set; }
        public int examTime { get; set; }
        public string questionStatusCode { get; set; }
        public int listOrder { get; set; }
        [JsonConverter(typeof(DateFormat))]
        public DateTime createTime { get; set; }

        public Guid creatorUid { get; set; }
        public string userLoginName { get; set; }
        public string userFullName { get; set; }
        /// <summary>
        /// 是否允许编辑
        /// </summary>
        public bool allowEdit { get; set; }
    }
}