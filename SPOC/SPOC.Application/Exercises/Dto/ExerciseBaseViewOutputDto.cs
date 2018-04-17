using System;

namespace SPOC.Exercises.Dto
{
    /// <summary>
    /// 前台练习任务基础信息
    /// </summary>
    public class ExerciseBaseViewOutputDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 任务标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户图片
        /// </summary>
        public string UserImg { get; set; }

        /// <summary>
        /// 试题ID
        /// </summary>
        public Guid QuestionId { get; set; }

        /// <summary>
        /// 练习结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 显示答案
        /// </summary>
        public bool ShowAnswer { get; set; }

        /// <summary>
        /// 显示答案类型
        /// </summary>
        public byte ShowAnswerType { get; set; }
    }
}