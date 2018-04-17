using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using SPOC.Category;

namespace SPOC.Exam
{
    /// <summary>
    /// 试卷接口表
    /// </summary>
    public class ExamPaperRelative : Entity<Guid>
    {
        public ExamPaperRelative()
        { }
        #region Model
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 试卷ID
        /// </summary>
        public Guid paperUid { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("paperUid")]
        public ExamPaper Paper { get; set; }
        /// <summary>
        /// 接口ID
        /// </summary>
        public Guid relativeUid { get; set; }
        /// <summary>
        /// 关联分类ID
        /// </summary>
        public Guid relativeFolderUid { get; set; }
        [ForeignKey("relativeFolderUid")]
        public NvFolder Folder { get; set; }
        /// <summary>
        /// 试卷类型(fix固定试卷、radom随机试卷)
        /// </summary>
        [StringLength(16)]
        public string paperTypeCode { get; set; }
        /// <summary>
        /// 接口类型(exam考试、task作业，exercise练习)
        /// </summary>
        [StringLength(16)]
        public string relativeTypeCode { get; set; }
        /// <summary>
        /// 答题模式
        /// </summary>
        [StringLength(36)]
        public string relativeExamDoModeCode { get; set; }
        /// <summary>
        /// 考试时长
        /// </summary>
        [StringLength(36)]
        public string relativeExamTime { get; set; }
        /// <summary>
        /// 考试时间策略
        /// </summary>
        [StringLength(36)]
        public string relativeExamTimeModule { get; set; }
        /// <summary>
        /// 补考次数
        /// </summary>
        [StringLength(36)]
        public string relativeExaminationCount { get; set; }
        /// <summary>
        /// 允许考生查看标准答案
        /// </summary>
        [StringLength(36)]
        public string relativeIsAllowSeeAnswer { get; set; }
        /// <summary>
        /// 允许考生查看成绩
        /// </summary>
        [StringLength(36)]
        public string relativeIsAllowSeeGrade { get; set; }
        /// <summary>
        /// 允许考生查看答卷
        /// </summary>
        [StringLength(36)]
        public string relativeIsAllowSeePaper { get; set; }
        /// <summary>
        /// 允许考生自己评卷
        /// </summary>
        [StringLength(36)]
        public string relativeIsAllowUserJudgePaper { get; set; }
        /// <summary>
        /// 允许考生报名参加考试
        /// </summary>
        [StringLength(36)]
        public string relativeIsAllowUserRegExam { get; set; }
        /// <summary>
        /// 允许考生查看本机资料
        /// </summary>
        [StringLength(36)]
        public string relativeIsOpenBook { get; set; }
        /// <summary>
        /// 考生报名考试需要审批
        /// </summary>
        [StringLength(36)]
        public string relativeIsUserRegExamApprove { get; set; }
        /// <summary>
        /// 参加次数
        /// </summary>
        [StringLength(36)]
        public string relativeMaxExamNum { get; set; }
        /// <summary>
        /// 考试记录系统积分
        /// </summary>
        [StringLength(36)]
        public string relativeExamIntegral { get; set; }
        #endregion Model

    }
}

