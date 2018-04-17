using System;
using System.Collections.Generic;

namespace SPOC.PolicyPaper.Dto
{
    /// <summary>
    /// 随机试卷策略项
    /// </summary>
    public class ExamPolicyItemItem
    {
        /// <summary>
        /// id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 随机试卷大题ID
        /// </summary>
        public Guid PolicyNodeUid { get; set; }

        /// <summary>
        /// 随机试卷大题名
        /// </summary>
        public string PolicyNodeName { get; set; }

        /// <summary>
        /// 试题类型ID
        /// </summary>
        public Guid QuestionTypeUid { get; set; }

        /// <summary>
        /// 试题类型
        /// </summary>
        public string QuestionTypeName { get; set; }

        /// <summary>
        /// 抽题试题分类ID
        /// </summary>
        public string FolderUid { get; set; }

        /// <summary>
        /// 抽题分类名
        /// </summary>
        public string FolderName { get; set; }

        /// <summary>
        /// 每题分数
        /// </summary>
        public decimal QuestionScore { get; set; }
        /// <summary>
        /// 试题个数
        /// </summary>
        public int QuestionNum { get; set; }
        /// <summary>
        /// 难度
        /// </summary>
        public string HardGrade { get; set; }
        /// <summary>
        /// 知识点Id列表
        /// </summary>
        public List<Guid> LabelIdList { get; set; }
        /// <summary>
        /// 知识点列表
        /// </summary>
        public List<string> LabelList { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int ListOrder { get; set; }
    }
}
