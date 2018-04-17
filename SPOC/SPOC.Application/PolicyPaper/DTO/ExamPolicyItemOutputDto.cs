using Abp.AutoMapper;
using SPOC.Exam;
using System;
using System.Collections.Generic;

namespace SPOC.PolicyPaper.Dto
{
    /// <summary>
    /// 随机试卷策略项
    /// </summary>
    [AutoMapFrom(typeof(ExamPolicyItem))]
    public class ExamPolicyItemOutputDto
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
        public Guid  questionTypeUid { get; set; }

        /// <summary>
        /// 试题类型
        /// </summary>
        public string questionTypeName { get; set; }

        /// <summary>
        /// 抽题试题分类ID
        /// </summary>
        public string folderUid { get; set; }

        /// <summary>
        /// 抽题分类名
        /// </summary>
        public string folderName { get; set; }

        /// <summary>
        /// 每题分数
        /// </summary>
        public decimal questionScore { get; set; }
        /// <summary>
        /// 试题个数
        /// </summary>
        public int questionNum { get; set; }
        /// <summary>
        /// 难度
        /// </summary>
        public string hardGrade { get; set; }
        /// <summary>
        /// 知识点Id列表
        /// </summary>
        public List<Guid> labelIdList { get; set; }
        /// <summary>
        /// 知识点列表
        /// </summary>
        public List<string> labelList { get; set; }
    }
}