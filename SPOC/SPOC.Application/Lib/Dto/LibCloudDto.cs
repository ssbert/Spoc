using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPOC.Core;
using SPOC.Exam;

namespace SPOC.Lib.Dto
{
    /// <summary>
    /// 标签 同步云端
    /// </summary>
    public class LabelInputDto
    {
        /// <summary>
        /// 标签信息
        /// </summary>
        public Label Label { set; get; }
        /// <summary>
        /// 标签规则信息
        /// </summary>
        public List<LabelRule> LabelRules { set; get; }
    }
    /// <summary>
    /// 挑战编程题输入DTO SPOC同步云端
    /// </summary>
    public class QuestionToCloudDto
    {
        /// <summary>
        /// 基本信息
        /// </summary>
        public ExamQuestion ExamQuestion { set; get; }
        /// <summary>
        /// 标签信息
        /// </summary>
        public ChallengeQuestion ChallengeQuestion { set; get; }
        /// <summary>
        /// 参考答案
        /// </summary>
        public List<QuestionStandardCode> QuestionStandardCodes { set; get; }
        /// <summary>
        /// 标签规则信息
        /// </summary>
        public List<QuestionLabel> QuestionLabels { set; get; }
    }
}
