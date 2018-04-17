using System;
using System.Collections.Generic;

namespace SPOC.Exam.ViewDto
{
    /// <summary>
    /// 前台考试任务基础信息
    /// </summary>
    public class ExamTaskBaseViewOutputDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 考试标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 创建者ID
        /// </summary>
        public Guid CreatorId { get; set; }
        /// <summary>
        /// 主考试ID
        /// </summary>
        public Guid MainExamId { get; set; }
        /// <summary>
        /// 当前目标考试Id
        /// </summary>
        public Guid TargetExamId { get; set; }
        /// <summary>
        /// 创建者名称
        /// </summary>
        public string CreatorName { get; set; }
        /// <summary>
        /// 用户图片
        /// </summary>
        public string UserImg { get; set; }
        /// <summary>
        /// 补考列表
        /// </summary>
        public List<RetestItem> RetestList { get; set; }
    }
}