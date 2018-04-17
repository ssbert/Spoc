using System;
using Newtonsoft.Json;

namespace SPOC.ExamPaper.Dto
{
    public class UserPaperViewOutputDto
    {
        public string examCode;
        public Guid gradeUid;
        public string examUserName;
        [JsonConverter(typeof(DateFormat))]
        public DateTime examBeginTime = new DateTime();
        [JsonConverter(typeof(DateFormat))]
        public DateTime examEndTime = new DateTime();
        public int examTime;
        public string judgeRealName;
        [JsonConverter(typeof(DateFormat))]
        public DateTime judgeBeginTime = new DateTime();
        public string examTotalScore;
        public string viewHtml;
        public string title;
        public string subTitle;
    }
}
