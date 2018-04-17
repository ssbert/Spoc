using System;
using System.Collections.Generic;
using Abp.AutoMapper;

namespace SPOC.Exam.Dto
{
    /// <summary>
    /// 考试任务
    /// </summary>
    [AutoMapFrom(typeof(ExamTask))]
    public class ExamTaskOutputDto
    {
        public ExamTaskOutputDto()
        {
            Exams = new List<ExamItem>();
            Classes = new Dictionary<Guid, string>();
        }
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 考试任务标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 是否自定义编号
        /// </summary>
        public bool IsCustomCode { get; set; }

        /// <summary>
        /// 考试列表
        /// </summary>
        public List<ExamItem> Exams { get; set; }

        /// <summary>
        /// 班级列表
        /// </summary>
        public Dictionary<Guid, string> Classes { get; set; }
    }
}