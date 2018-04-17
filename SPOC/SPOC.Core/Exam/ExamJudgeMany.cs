using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Exam
{
    /// <summary>
    /// ����������Ա�
    /// </summary>
    public class ExamJudgeMany : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// ���Ա��
        /// </summary>
        public Guid examUid { get; set; }

        [ForeignKey("examUid")]
        public ExamExam Exam { get; set; }

        /// <summary>
        /// ����ģʽ��all������������one��ֻҪ��һ�������֣�
        /// </summary>
        [StringLength(36)]
        public string judgeModeCode { get; set; }

        /// <summary>
        /// ����ģʽ��avg:ȡƽ����max:ȡ��߷�min:ȡ��ͷ�assign:ȡָ����interface:ȡָ���ӿڣ�
        /// </summary>
        [StringLength(64)]
        public string judgePolicyCode { get; set; }

        /// <summary>
        /// ָ��������
        /// </summary>
        public Guid judgeUserUid { get; set; }
        [ForeignKey("judgeUserUid")]
        public User.UserBase User { get; set; }

        /// <summary>
        /// ����ӿ�
        /// </summary>
        [StringLength(2000)]
        public string @interface { get; set; }

        /// <summary>
        /// ��ע
        /// </summary>
        public string remarks { get; set; }
    }
}
