using Abp.AutoMapper;
using SPOC.Exam;
using System;
using System.Collections.Generic;

namespace SPOC.PolicyPaper.Dto
{
    /// <summary>
    /// 随机试卷策略项
    /// </summary>
    [AutoMapTo(typeof(ExamPolicyItem))]
    public class ExamPolicyItemInputDto
    {
        /// <summary>
        /// id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 随机试卷大题ID
        /// </summary>
        public Guid policyNodeUid { get; set; }

        /// <summary>
        /// 试题类型ID
        /// </summary>
        public Guid questionTypeUid { get; set; }

        /// <summary>
        /// 抽题试题分类ID串
        /// </summary>
        public string folderUid { get; set; }

        /// <summary>
        /// 抽题试题分类名称串
        /// </summary>
        public string folderName { get; set; }

        /// <summary>
        /// 试题个数
        /// </summary>
        public int questionNum { get; set; }

        /// <summary>
        /// 每题分数
        /// </summary>
        public decimal questionScore { get; set; }

        /// <summary>
        /// 难度
        /// </summary>
        public string hardGrade { get; set; }

        /// <summary>
        /// 标签Id列表
        /// </summary>
        public List<Guid> labelIdList { get; set; }
    }
}