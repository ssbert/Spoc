using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using SPOC.Attribute;

namespace SPOC.Exam
{
    /// <summary>
    /// 随机试卷策略项
    /// </summary>
    public class ExamPolicyItem : Entity<Guid>
    {
        public ExamPolicyItem()
        {
            advanceOption = "";
            folderUid = "";
            excludeFolderUids = "";
            hardGrade = "";
            folderType = "question_folder";
        }

        #region Model
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 随机试卷大题ID
        /// </summary>
        public Guid policyNodeUid { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("policyNodeUid")]
        public ExamPolicyNode PolicyNode { get; set; }

        /// <summary>
        /// 试题类型ID
        /// </summary>
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid questionTypeUid { get; set; }


        /// <summary>
        /// 抽题试题分类ID
        /// </summary>
        [StringLength(4000)]
        public string folderUid { get; set; }

        /// <summary>
        /// 高级选项
        /// </summary>
        public string advanceOption { get; set; }

        /// <summary>
        /// 屏障试题分类ID串，逗号分隔
        /// </summary>
        [StringLength(2000)]
        public string excludeFolderUids { get; set; }

        /// <summary>
        /// 试题个数
        /// </summary>
        public int questionNum { get; set; }

        /// <summary>
        /// 每题分数
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal questionScore { get; set; }

        /// <summary>
        /// 难度
        /// </summary>
        [StringLength(64)]
        public string hardGrade { get; set; }

        /// <summary>
        /// 每题限时秒数
        /// </summary>
        public int examTime { get; set; }

        /// <summary>
        /// 大题顺序
        /// </summary>
        public int listOrder { get; set; }

        /// <summary>
        /// 抽题试题大类类型
        /// </summary>
        [StringLength(36)]
        [Required]
        public string folderType { get; set; }

        /// <summary>
        /// 抽题试题大类类型名称？
        /// </summary>
        [StringLength(4000)]
        public string folderName { get; set; }

        #endregion Model

    }
}

