using System;
using Newtonsoft.Json;

namespace SPOC.Lib.Dto
{
    /// <summary>
    /// 用户作答记录
    /// </summary>
    public class UserAnswerRecordsTemp
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
        /// 标签ID
        /// </summary>
        public Guid LabelId { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 作答详细Id
        /// examGradeId, exerciseRecordId, challengeGradeId
        /// </summary>
        public Guid RecordId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 来源 (exam, exercise, challenge)
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 标签分值（只用来记录加扣分，不参与实际计算）
        /// </summary>
        public int Score { get; set; }
    }
}