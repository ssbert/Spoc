using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using SPOC.Attribute;

namespace SPOC.Exam
{
    /// <summary>
    /// ������Ϣ
    /// </summary>
    public class ExamExam : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        public ExamExam()
        {
            #region ��ʼ������
            isNeedLimitedTime = "N";
            allowFeedbook = "N";
            isNeedJudge = "N";
            autoSaveToServer = "N";
            isAllowSeeGrade = "N";
            publishGradeDate = "";
            isAllowSeeAnswer = "N";
            isAllowSeeReport = "Y";
            isPublishGrade = "N";
            isRealTimeControll = "N";
            isRealtimeSaveAnswerToDb = "N";
            isDeductScoreWhenError = "N";
            examDescription = "";
            remarks = "";
            isDisplayResult = "N";
            isMixOrder = "N";
            isAllowSeePaper = "N";
            isLimitByIp = "Y";
            isAllowModifyUserAnswer = "N";
            isAllowModifyObjectAnswer = "N";
            isAllowUserJudgePaper = "N";
            isNeedIntegral = "N";
            gateQuestionMode = "";
            questionireUidName = "";
            passExamMessage = "";
            noPassExamMessage = "";
            isAllowUserRegExam = "N";
            isUserRegExamApprove = "N";
            isAllowObjectJudge = "Y";
            isOfflineExam = "N";
            mobileFlag = "";
            passGradeType = "passGradeRate";
            IsExamination = "N";

            #endregion
        }
        /// <summary>
        /// ��������Id
        /// </summary>
        [Column("taskId")]
        public Guid TaskId { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        [Column("examName"), Required, StringLength(256)]
        public string ExamName { get; set; }
        /// <summary>
        /// ���Ա��
        /// </summary>
        [Column("examCode"), Required, StringLength(64)]
        public string ExamCode { get; set; }
        /// <summary>
        /// �Ƿ����Զ�����
        /// </summary>
        public bool isCustomCode { get; set; }
        /// <summary>
        /// �������ͣ�exam����, task��ҵ��
        /// </summary>
        [StringLength(16)]
        [Required]
        public string examClassCode { get; set; }
        /// <summary>
        /// �������ͣ�exam_normal: ����, exam_retest: ����, exam_train:��ѵ�ƻ��еĿ���, task_normal:��ҵ, task_train: ��ѵ�ƻ��е���ҵ��
        /// </summary>
        [StringLength(16)]
        [Required]
        public string examTypeCode { get; set; }
        /// <summary>
        /// ����ģʽ��Paper����question���⡢node���⣩
        /// </summary>
        [StringLength(16), Required]
        public string examDoModeCode { get; set; }
        /// <summary>
        /// �Ƿ���ʱ�����÷���ģʽ��
        /// </summary>
        [StringLength(1), Required, DefaultValue("N")]
        public string isNeedLimitedTime { get; set; }
        /// <summary>
        /// �Ծ����ͣ�fix�̶��Ծ�radom����Ծ�
        /// </summary>
        [StringLength(16), Required]
        public string paperTypeCode { get; set; }
        /// <summary>
        /// �Ծ�ID���̶�������Ⱦ��п��ܣ�
        /// </summary>
        public Guid paperUid { get; set; }
        [Newtonsoft.Json.JsonIgnore]

        [ForeignKey("paperUid")] 
        public ExamPaper Paper { get; set; }
        /// <summary>
        /// ����ʱ�䣨����Ϊ��λ��
        /// </summary>
        public int examTime { get; set; }
        /// <summary>
        /// ����ʱ����ԣ�join_exam ���뿼��ʱ�䡢end_exam����ο�ʱ�䣩
        /// </summary>
        [StringLength(36), Required]
        public string examTimeModule { get; set; }
        /// <summary>
        /// �������ⷴ��
        /// </summary>
        [StringLength(1), Required, DefaultValue("N")]
        public string allowFeedbook { get; set; }
        /// <summary>
        /// ͨ�������ж����ͣ�passGradeRate | passGradeScore��
        /// </summary>
        [StringLength(16), Required, DefaultValue("passGradeRate")]
        public string passGradeType { get; set; }
        /// <summary>
        /// ͨ���ĵ÷��ʣ�ͨ��������ͨ���÷���ֻ����һ����Ϊ0��
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal? passGradeRate { get; set; }
        /// <summary>
        /// ͨ���ķ���
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal? passGradeScore { get; set; }
        /// <summary>
        /// �Ƿ񿪾���
        /// </summary>
        [StringLength(1), Required, DefaultValue("N")]
        public string isOpenBook { get; set; }
        /// <summary>
        /// �Ƿ���Ҫ����
        /// </summary>
        [StringLength(1), Required, DefaultValue("N")]
        public string isNeedJudge { get; set; }
        /// <summary>
        /// �Զ���������
        /// </summary>
        public int autoSaveSecond { get; set; }
        /// <summary>
        /// �Զ�����𰸵���������ֻ���������Զ������ʱ��Ч��
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("N")]
        public string autoSaveToServer { get; set; }
        /// <summary>
        /// �������μӴ���
        /// </summary>
        public int? maxExamNum { get; set; }
        /// <summary>
        /// ������Ҫ�μӴ���
        /// </summary>
        public int? minExamNum { get; set; }
        /// <summary>
        /// �Ƿ�����鿴�ɼ�
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("N")]
        public string isAllowSeeGrade { get; set; }
        /// <summary>
        /// ����鿴�ɼ�����(0��������)
        /// </summary>
        public int? allowSeeGradeDays { get; set; }
        /// <summary>
        /// �����ɼ�����
        /// </summary>
        [StringLength(10), DefaultValue("")]
        public string publishGradeDate { get; set; }
        /// <summary>
        /// �Ƿ�����鿴���
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("Y")]
        public string isAllowSeeAnswer { get; set; }
        /// <summary>
        /// �Ƿ�����鿴����(ֻ�в����Ծ���б���)
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("Y")]
        public string isAllowSeeReport { get; set; }
        /// <summary>
        /// �Ƿ�������˹����ɼ�
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("N")]
        public string isPublishGrade { get; set; }
        /// <summary>
        /// �Ƿ�ʵʱ���
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("N")]
        public string isRealTimeControll { get; set; }
        /// <summary>
        /// �Ƿ񱣴�ÿ��𰸵�����
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("N")]
        public string isRealtimeSaveAnswerToDb { get; set; }
        /// <summary>
        /// ���Ͳ��ش�����۷�
        /// ˵����������۷ֺ��򲻻����𣨲���û�д𵽣�����Ŀն�����۷ֹ��ܣ��������յ÷���Ҫ��ȥ������ķ����Ͳ����ķ�����ѡ�����յ÷�=������ķ���-������ķ���-��������ķ�����
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("N")]
        public string isDeductScoreWhenError { get; set; }
        /// <summary>
        /// ����˵��(�ڿ���ʱ����ʾ���Ծ��Ϸ���һ��˵�����ʵ�����)
        /// </summary>
        [DefaultValue("")]
        public string examDescription { get; set; }
        /// <summary>
        /// ��ע
        /// </summary>
        [StringLength(256), DefaultValue("")]
        public string remarks { get; set; }
        /// <summary>
        /// ���Լ�¼״̬(normal����)
        /// </summary>
        [StringLength(16), Required]
        public string examStatusCode { get; set; }
        /// <summary>
        /// �Ƿ���ʾ����
        /// </summary>
        [StringLength(1), DefaultValue("N"), Required]
        public string isDisplayResult { get; set; }
        /// <summary>
        /// �Ƿ����˳��
        /// </summary>
        [StringLength(1), DefaultValue("N"), Required]
        public string isMixOrder { get; set; }
        /// <summary>
        /// �����Ծ����
        /// </summary>
        public int? bufferPaperNum { get; set; }
        /// <summary>
        /// �����������
        /// </summary>
        public int? bufferExpireSecond { get; set; }
        /// <summary>
        /// ������ID
        /// </summary>
        public Guid creatorUid { get; set; }
   
        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime createTime { get; set; }
        /// <summary>
        /// �޸�ʱ��
        /// </summary>
        public DateTime modifyTime { get; set; }
        /// <summary>
        /// ��ֹ��ʼʱ��
        /// </summary>
        public int? forbidTime { get; set; }
        /// <summary>
        /// �ɼ���������
        /// </summary>
        [StringLength(16)]
        public string gradeReleaseType { get; set; }
        /// <summary>
        /// ���Ժ�����췢��
        /// </summary>
        public int? publishGradeDays { get; set; }
        /// <summary>
        /// �Ƿ�����鿴���
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("Y")]
        public string isAllowSeePaper { get; set; }
        /// <summary>
        /// ������Ϣ��ǰ����ʱ��(��)
        /// </summary>
        public int? forwardPublishExamTime { get; set; }
        /// <summary>
        /// �Ƿ���IP����
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("Y")]
        public string isLimitByIp { get; set; }
        /// <summary>
        /// ��ֹ��ǰ�ύʱ��
        /// </summary>
        public int? forbitSubmitBeforeTime { get; set; }
        /// <summary>
        /// �Ƿ������޸Ĵ�
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("N")]
        public string isAllowModifyUserAnswer { get; set; }
        /// <summary>
        /// �Ƿ������޸Ŀ͹���
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("N")]
        public string isAllowModifyObjectAnswer { get; set; }
        /// <summary>
        /// �Ƿ�����������
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("N")]
        public string isAllowUserJudgePaper { get; set; }
        /// <summary>
        /// �Ƿ�������
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("N")]
        public string isNeedIntegral { get; set; }
        /// <summary>
        /// ���Ի���
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal examIntegral { get; set; }
        /// <summary>
        /// �ܹ���
        /// </summary>
        public int? gateNum { get; set; }
        /// <summary>
        /// ÿ�س��ⷽʽ
        /// </summary>
        [StringLength(36), DefaultValue("")]
        public string gateQuestionMode { get; set; }
        /// <summary>
        /// ÿ�ص÷���
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal? gatePassGratdeRate { get; set; }
        /// <summary>
        /// �������Ŵ���
        /// </summary>
        public int examinationCount { get; set; }
        /// <summary>
        /// �ʾ����ı�ź�����
        /// </summary>
        [StringLength(256), DefaultValue("")]
        public string questionireUidName { get; set; }
        /// <summary>
        /// ͨ��������ʾ��Ϣ
        /// </summary>
        [DefaultValue("")]
        public string passExamMessage { get; set; }
        /// <summary>
        /// δͨ��������ʾ��Ϣ
        /// </summary>
        [DefaultValue("")]
        public string noPassExamMessage { get; set; }
        /// <summary>
        /// �Ƿ���������������
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("N")]
        public string isAllowUserRegExam { get; set; }
        /// <summary>
        /// ���������Ƿ���Ҫ����
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("N")]
        public string isUserRegExamApprove { get; set; }
        /// <summary>
        /// ������ߵ÷�
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal? markPaperMaxScore { get; set; }
        /// <summary>
        /// ����͹����ֹ�����
        /// </summary>
        [StringLength(1), Required]
        [DefaultValue("Y")]
        public string isAllowObjectJudge { get; set; }
        /// <summary>
        /// �Ƿ����¿���
        /// </summary>
        [StringLength(1), Required, DefaultValue("N")]
        public string isOfflineExam { get; set; }
        /// <summary>
        /// �ƶ��˱�ʶ
        /// </summary>
        [StringLength(10), DefaultValue("")]
        public string mobileFlag { get; set; }

        /// <summary>
        /// ���Կ�ʼʱ��
        /// </summary>
        [Column("beginTime")]
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// ���Խ���ʱ��
        /// </summary>
        [Column("endTime")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// �Ƿ񲹿�
        /// </summary>
        [Column("isExamination"), DefaultValue("N")]
        public string IsExamination { get; set; }
    }
}
