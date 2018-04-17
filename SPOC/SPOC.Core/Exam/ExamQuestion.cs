using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using SPOC.Attribute;
using SPOC.Category;

namespace SPOC.Exam
{
    /// <summary>
    /// 题库
    /// </summary>
    public class ExamQuestion : Entity<Guid>
    {
        public ExamQuestion()
        {
            parentQuestionUid = Guid.Empty;
            operateTypeCode = "";
            hardGrade = "";
            selectAnswer = "";
            selectAnswerScore = "";
            standardAnswer = "";
            questionAnalysis = "";
            courseUid = Guid.Empty;
            isAnswerByHtml = "N";
            hasFile = "N";
            isOnlyUploadFile = "N";
        }
        #region Model
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 课程ID
        /// </summary>
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid courseUid { get; set; }

        /// <summary>
        /// 分类ID
        /// </summary>
        public Guid folderUid { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("folderUid")]
        public NvFolder Folder { get; set; }
        /// <summary>
        /// 上级试题ID
        /// </summary>
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid parentQuestionUid { get; set; }

        /// <summary>
        /// 试题类型ID
        /// </summary>
        public Guid questionTypeUid { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("questionTypeUid")]
        public ExamQuestionType QuestionType { get; set; }

        /// <summary>
        /// 试题类型编号
        /// </summary>
        [StringLength(16)]
        [Required]
        public string questionBaseTypeCode { get; set; }

        /// <summary>
        /// 操作题类型
        /// </summary>
        [StringLength(16)]
        [DefaultValue("")]
        public string operateTypeCode { get; set; }

        /// <summary>
        /// 程序语言，当前默认cpp，以后可更改
        /// </summary>
        [StringLength(16), DefaultValue("")]
        public string language { get; set; }

        /// <summary>
        /// 答题时间
        /// </summary>
        public int examTime { get; set; }

        /// <summary>
        /// 试题内容（原内容）
        /// </summary>
        [Required]
        public string questionText { get; set; }

        /// <summary>
        /// 试题内容（去除html标签的）
        /// </summary>
        public string questionPureText { get; set; }

        /// <summary>
        /// 难度
        /// </summary>
        [StringLength(16)]
        [DefaultValue("")]
        public string hardGrade { get; set; }

        /// <summary>
        /// 分数
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal score { get; set; }

        /// <summary>
        /// 可选答案
        /// </summary>
        [DefaultValue("")]
        public string selectAnswer { get; set; }

        /// <summary>
        /// 可选答案分数
        /// </summary>
        [StringLength(1000)]
        [DefaultValue("")]
        public string selectAnswerScore { get; set; }

        /// <summary>
        /// 每选项倒扣分
        /// </summary>
        [DecimalPrecision(18, 2)]
        public decimal eachOptionScore { get; set; }

        /// <summary>
        /// 可选答案个数
        /// </summary>
        public int answerNum { get; set; }

        /// <summary>
        /// 标准答案
        /// </summary>
        [DefaultValue("")]
        public string standardAnswer { get; set; }

        /// <summary>
        /// 填空题答案按顺序
        /// </summary>
        public bool inOrder { get; set; }

        /// <summary>
        /// 试题分析
        /// </summary>
        [DefaultValue("")]
        public string questionAnalysis { get; set; }

        /// <summary>
        /// 过期日期
        /// </summary>
        public int outdatedDate { get; set; }

        /// <summary>
        /// 试题状态
        /// </summary>
        [StringLength(16)]
        [Required]
        public string questionStatusCode { get; set; }

        [StringLength(1), Required, DefaultValue("N")]
        public string isAnswerByHtml { get; set; }

        [StringLength(1), Required, DefaultValue("N")]
        public string hasFile { get; set; }

        [StringLength(1), Required, DefaultValue("N")]
        public string isOnlyUploadFile { get; set; }

        /// <summary>
        /// 试题排序号
        /// </summary>
        public int listOrder { get; set; }

        /// <summary>
        /// 创建者userid
        /// </summary>
        public Guid creatorUid { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createTime { get; set; }

        /// <summary>
        /// 修改者
        /// </summary>
        public Guid modifier { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime modifyTime { get; set; }

        /// <summary>
        /// 试题编号
        /// </summary>
        [StringLength(64)]
        [Required]
        public string questionCode { get; set; }

        [Required, DefaultValue(false)]
        public bool isCustomCode { get; set; }

        /// <summary>
        /// 学科Id
        /// </summary>
        public Guid subjectUid { get; set; }
       
        /// <summary>
        /// 组织架构ID
        /// </summary>
        public Guid departmentUid { get; set; }

        /// <summary>
        /// 程序题参数，用“|”分割
        /// </summary>
        public string param { get; set; }

        /// <summary>
        /// 程序题输入流参数，用空格分隔单次测试的输入参数，用“|”分隔每次测试的参数
        /// 例：一共2次测试，第1次测试参数为 “1 2 3”，第2次测试参数为“4 5 6”
        /// 实际填写：1 2 3|4 5 6
        /// </summary>
        [Column("inputParam")]
        public string InputParam { get; set; }

        /// <summary>
        /// 多次测试
        /// </summary>
        [Column("multiTest"), DefaultValue(false)]
        public bool MultiTest { get; set; }

        /// <summary>
        /// 预设代码
        /// </summary>
        [Column("preinstallCode")]
        public string PreinstallCode { get; set; }

        #endregion Model

    }
}

