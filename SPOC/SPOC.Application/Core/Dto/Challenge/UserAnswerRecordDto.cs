using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SPOC.Core.Dto.Challenge
{
    /// <summary>
    /// 用户作答记录
    /// </summary>
    public class UserAnswerRecordDto
    {
        /// <summary>
        /// 答题记录Id
        /// </summary>
        public Guid gradeId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 试题内容
        /// </summary>
        public string questionText { get; set; }
        /// <summary>
        /// 难度
        /// </summary>
        public string hardGrade { get; set; }

        /// <summary>
        /// 分数
        /// </summary>
        public decimal score { get; set; }
        /// <summary>
        /// 用户得分
        /// </summary>
        public decimal userScore { get; set; }
        /// <summary>
        /// 是否通过 1通过 0不通过
        /// </summary>
        public int? isPass { get; set; }

        [JsonConverter(typeof(DateFormat))]
        public DateTime createTime { get; set; }
    }

}
