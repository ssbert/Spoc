using System;

namespace SPOC.Lib.Dto
{
    /// <summary>
    /// 用户作答详情对应试题
    /// </summary>
    public class UserAnswerRecordsQuestion
    {
        /// <summary>
        /// 记录ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 试题ID
        /// </summary>
        public Guid QuestionId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 试题题干
        /// </summary>
        public string QuestionText { get; set; }
        /// <summary>
        /// 题型
        /// </summary>
        public string QuestionBaseTypeCode { get; set; }
        /// <summary>
        /// 编程语言
        /// </summary>
        public string Language { get; set; }
        /// <summary>
        /// 候选答案
        /// </summary>
        public string SelectAnswer { get; set; }
        /// <summary>
        /// 用户答案
        /// </summary>
        public string UserAnswer { get; set; }
        /// <summary>
        /// 试题类型
        /// normal：普通试题
        /// challenge：挑战试题
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 运行结果
        /// </summary>
        public string Result { get; set; }
        /// <summary>
        /// 是否通过
        /// </summary>
        public bool IsPass { get; set; }
    }
}