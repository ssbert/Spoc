using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SPOC.Exam.Dto
{
    public class UserExamInfo 
    {
        public Guid examUid { get; set; }
        public string examName { get; set; }
        /// <summary>
        /// 考试时长，0为不限
        /// </summary>
        public int examTime { get; set; }
        /// <summary>
        /// 考试类型 exam_normal: 正考, exam_retest: 补考
        /// </summary>
        public string examTypeCode { get; set; }
        public int? maxExamNum { get; set; }
        public List<DateTime?[]> timeArrange { get; set; } 
        /// <summary>
        /// 用户考试次数
        /// </summary>
        public int userExamNum { get; set; }
        /// <summary>
        /// 是否通过
        /// </summary>
        public bool isPass { get; set; }
        /// <summary>
        /// 是否开卷
        /// </summary>
        public bool isOpenBook { get; set; }
        [JsonConverter(typeof(DateFormat))]
        public DateTime createTime { get; set; }
    }
}
