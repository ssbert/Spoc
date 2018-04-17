using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using SPOC.Attribute;

namespace SPOC.Exam
{
    /// <summary>
    /// ����������Ϣ��
    /// </summary>
    [Table("exam_judge_info")]
    public class ExamJudgeInfo : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }

        /// <summary>
        /// �����ɼ�ID
        /// </summary>
        [Column("examGradeUid")]
        public Guid ExamGradeUid { get; set; }

        /// <summary>
        /// ����ID
        /// </summary>
        [Column("questionUid")]
        public Guid QuestionUid { get; set; }

        /// <summary>
        /// ������ID
        /// </summary>
        [Column("judgeUserUid")]
        public Guid JudgeUserUid { get; set; }

        /// <summary>
        /// ������״̬��ţ�rightΪ��,errorΪ��,middleΪ��ԣ�
        /// </summary>
        [Column("judgeResultCode"), StringLength(16)]
        public string JudgeResultCode { get; set; }

        /// <summary>
        /// �÷�
        /// </summary>
        [Column("judgeScore"), DecimalPrecision(18, 2)]
        public decimal? JudgeScore { get; set; }

        /// <summary>
        /// ������ע
        /// </summary>
        [Column("judgeRemarks"), StringLength(256)]
        public string JudgeRemarks { get; set; }

        /// <summary>
        /// �÷ֵ�
        /// </summary>
        [Column("judgeScoreText"), StringLength(256)]
        public string JudgeScoreText { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        [Column("createTime")]
        public DateTime CreateTime { get; set; }
    }
}
