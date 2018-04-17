using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Exam
{
    /// <summary>
    /// ���Ի�������
    /// </summary>
    public class ExamCacheTask : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        public ExamCacheTask()
        {
            statusCode = "no_start";
        }
        /// <summary>
        /// �������
        /// </summary>
        [StringLength(256)]
        public string changeTitle { get; set; }
        /// <summary>
        /// �䶯����
        /// exam_info:���Ա䶯
        /// exam_arrange:���԰���
        /// exam_paper:�Ծ�䶯
        /// exam_grade:���Գɼ�
        /// exam_change_grade:���Լ�¼���
        /// </summary>
        [StringLength(64)]
        public string changeClass { get; set; }
        /// <summary>
        /// �䶯����(add:������edit:�༭��delete:ɾ��)
        /// </summary>
        [StringLength(16)]
        public string changeType { get; set; }
        /// <summary>
        /// �䶯�����ID
        /// </summary>
        public Guid objectUid { get; set; }
        /// <summary>
        /// ���������ID
        /// </summary>
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid relativeUid { get; set; }

        /// <summary>
        /// ״̬(no_start:δ������updating:�����У�has_done:�����)
        /// </summary>
        [StringLength(16)]
        [DefaultValue("no_start")]
        public string statusCode { get; set; }
        /// <summary>
        /// ��ע
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime createTime { get; set; }
        /// <summary>
        /// ������ID
        /// </summary>
        public Guid creatorUid { get; set; }
        /// <summary>
        /// ��ʼ����ʱ��
        /// </summary>
        public DateTime beginTime { get; set; }
        /// <summary>
        /// ���ʱ��
        /// </summary>
        public DateTime doneTime { get; set; }
    }
}
