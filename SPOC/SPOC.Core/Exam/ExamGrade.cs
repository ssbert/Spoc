using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using SPOC.Attribute;

namespace SPOC.Exam
{
    /// <summary>
    /// �����ɼ�
    /// </summary>
    public class ExamGrade : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// ����ID
        /// </summary>
        public Guid userUid { get; set; }
        /// <summary>
        /// ����ID
        /// </summary>
        public Guid examUid { get; set; }
        /// <summary>
        /// �Ծ�ID
        /// </summary>
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid paperUid { get; set; }
        /// <summary>
        /// �Ծ��ܷ�
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal? paperTotalScore { get; set; }
        /// <summary>
        /// ��ʼʱ��
        /// </summary>
        public DateTime beginTime { get; set; }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime? endTime { get; set; }
        /// <summary>
        /// ������ʱ��
        /// </summary>
        public DateTime lastUpdateTime { get; set; }
        /// <summary>
        /// ������ʱ��������Ϊ��λ��
        /// </summary>
        public int? allowExamTime { get; set; }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        public int? examTime { get; set; }
        /// <summary>
        /// �ɼ�
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal? gradeScore { get; set; }
        /// <summary>
        /// �÷���
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal? gradeRate { get; set; }
        /// <summary>
        /// �͹������
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal? externalScore { get; set; }
        /// <summary>
        /// ���������
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal? subjectiveScore { get; set; }
        /// <summary>
        /// ������ID
        /// </summary>
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid judgeUserUid { get; set; }
        /// <summary>
        /// ����������
        /// </summary>
        [StringLength(64)]
        public string judgeUserName { get; set; }
        /// <summary>
        /// ����ʼʱ��
        /// </summary>
        public DateTime? judgeBeginTime { get; set; }
        /// <summary>
        /// �������ʱ��
        /// </summary>
        public DateTime? judgeEndTime { get; set; }
        /// <summary>
        /// �Ƿ�ͨ��
        /// </summary>
        [StringLength(1)]
        public string isPass { get; set; }
        /// <summary>
        /// �ɼ�����
        /// </summary>
        public int? gradeOrder { get; set; }
        /// <summary>
        /// �ɼ�״̬��release�ѷ�����submitted���ύ��judged������pause��ͣ�С�examing�����С�judging�����У�
        /// </summary>
        [StringLength(16)]
        public string gradeStatusCode { get; set; }
        /// <summary>
        /// �Ծ����˳����ID�����������⿼��ʱ���´��Һ��˳��
        /// </summary>
        public string paperQuestionUids { get; set; }
        /// <summary>
        /// ��ǰ���Ե�����˳��ţ��������⿼��ʱ���µ�ǰ����˳��ţ�
        /// </summary>
        public int? currentQuestionIndex { get; set; }
        /// <summary>
        /// �Ƿ��Ѿ����ɲ������棨ֻ�Բ���ϵͳʱ�����ã�
        /// </summary>
        [StringLength(1)]
        [DefaultValue("N")]
        public string hasCreateReport { get; set; }
        /// <summary>
        /// �Ƿ񱣴��˴𰸵�����
        /// </summary>
        [StringLength(1)]
        [DefaultValue("Y")]
        public string hasSaveAnswerToDb { get; set; }
        /// <summary>
        /// �����Ĵ�ID (ExamUserAnswer)
        /// </summary>
         [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid userAnswerUid { get; set; }
       
        /// <summary>
        /// ��ͨ������
        /// </summary>
        public int? passGateNum { get; set; }
        /// <summary>
        /// ���Խ��ͳ������(computer ���Կ�, paper ֽ�ʿ���, failure ʧЧ�Ծ�)
        /// </summary>
        [StringLength(36)]
        [DefaultValue("computer")]
        public string examResultType { get; set; }
        /// <summary>
        /// �μӿ���IP
        /// </summary>
        [StringLength(36)]
        public string lastExamIp { get; set; }
        /// <summary>
        /// �Ƿ�Ϊ�����ɼ�
        /// </summary>
        [StringLength(1)]
        [DefaultValue("N")]
        public string isExamination { get; set; }
        /// <summary>
        /// δ��������
        /// </summary>
        public int? noAnswerQuestionNum { get; set; }
        /// <summary>
        /// ��Դ(���ֶ�Ӧ���ڷֲ�ʽ���Թ����У���ʾ��ǰ�ɼ��Ǵ��ĸ���֧�ϴ������ģ���ֵΪ��֧վ��ı��)
        /// </summary>
        [StringLength(64)]
        public string source { get; set; }

        /// <summary>
        /// �Ƿ����ɴ��
        /// </summary>
        [StringLength(1)]
        public string hasCreateAnswerPaper { get; set; }

        /// <summary>
        /// �Ƿ�������
        /// </summary>
        [Column("isCompiled"), DefaultValue(true)]
        public bool IsCompiled { get; set; }
    }
}
