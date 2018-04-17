using Castle.Components.DictionaryAdapter;
using SPOC.Common.File;
using SPOC.Exam;
using SPOC.QuestionBank.Const;
using System;
using System.Collections.Generic;
using SPOC.QuestionBank.Dto;

namespace SPOC.Web.Areas.QuestionBank.Models
{
    public class QuestionPreviewModel
    {
        private readonly Guid _id;
        private readonly string _questionType;
        private readonly decimal _score;
        private readonly string _examTime;
        private readonly string _status;
        private readonly string _operateType;
        private readonly string _hardGrade;
        private readonly string _questionCode;
        private readonly string _questionText;
        private readonly string _questionAnalysis;
        private readonly string _standardAnswer;
        private readonly string _standardCode;
        private readonly string[] _standardAnswers;
        private readonly string _selecteAnswer;
        private readonly string[] _selectAnswers;
        public QuestionPreviewModel(ExamQuestionDto question)
        {
            _id = question.Id;
            _questionType = QuestionTypeFormat(question.questionBaseTypeCode);
            _score = question.score;
            _examTime = ExamTimeFormat(question.examTime);
            _status = QuestionStatusCodeFormat(question.questionStatusCode);
            _operateType = OperateTypeCodeFormat(question.operateTypeCode);
            _hardGrade = question.hardGrade;
            _questionCode = question.questionCode;
            _questionText = FilePathUtil.GetContentTextWithFilePath(question.Id.ToString(), "question", question.questionText, false);
            _questionAnalysis = FilePathUtil.GetContentTextWithFilePath(question.Id.ToString(), "question", question.questionAnalysis, false);
            _standardAnswer = FilePathUtil.GetContentTextWithFilePath(question.Id.ToString(), "question", question.standardAnswer, false);
            _standardAnswers = _standardAnswer.Split('|');
            _selecteAnswer = FilePathUtil.GetContentTextWithFilePath(question.Id.ToString(), "question", question.selectAnswer, false);
            _selectAnswers = _selecteAnswer.Split('|');
            if (!string.IsNullOrEmpty(question.param))
            {
                Param = question.param.Replace("|", " ");
            }
            InputParam = question.InputParam;
            MultiTest = question.MultiTest;
            _standardCode = question.standardCode;
            PreinstallCode = question.PreinstallCode;
        }

        public Guid Id { get { return _id; } }

        /// <summary>
        /// 试题类型
        /// </summary>
        public string QuestionType { get { return _questionType;} }

        /// <summary>
        /// 试题分数
        /// </summary>
        public decimal Score { get { return _score; } }

        /// <summary>
        /// 答题时限
        /// </summary>
        public string ExamTime { get { return _examTime; } }

        /// <summary>
        /// 试题状态
        /// </summary>
        public string Status{ get { return _status; } }

        /// <summary>
        /// 试题难度
        /// </summary>
        public string HardGrade { get { return _hardGrade; } }

        /// <summary>
        /// 操作题型
        /// </summary>
        public string OperateType { get { return _operateType; } }

        /// <summary>
        /// 试题编号
        /// </summary>
        public string QuestionCode { get { return _questionCode; } }
        /// <summary>
        /// 试题内容
        /// </summary>
        public string QuestionText { get { return _questionText; } }

        /// <summary>
        /// 标准答案
        /// </summary>
        public string StandardAnswer { get { return _standardAnswer; } }
        /// <summary>
        /// 标准代码
        /// </summary>
        public string StandardCode { get { return _standardCode; } }
        /// <summary>
        /// 试题分析
        /// </summary>
        public string QuestionAnalysis { get { return _questionAnalysis; } }
        /// <summary>
        /// 标准答案列表
        /// </summary>
        public string[] StandardAnswers { get { return _standardAnswers; } }

        /// <summary>
        /// 候选答案
        /// </summary>
        public string SelectAnswer { get { return _selecteAnswer; } }
        /// <summary>
        /// 候选答案列表
        /// </summary>
        public string[] SelectAnswers { get { return _selectAnswers; } }

        /// <summary>
        /// 编程题参数
        /// </summary>
        public string Param { get; set; }

        /// <summary>
        /// 输入流参数
        /// </summary>
        public string InputParam { get; set; }

        /// <summary>
        /// 多次测试
        /// </summary>
        public bool MultiTest { get; set; }

        /// <summary>
        /// 预设代码
        /// </summary>
        public string PreinstallCode { get; set; }

        private readonly List<QuestionPreviewModel> _children = new EditableList<QuestionPreviewModel>();
        public List<QuestionPreviewModel> Children { get { return _children; } }

        public string IndexToLetter(int index)
        {
            var bytes = new byte[1];
            bytes[0] = (byte)Convert.ToInt32(index);
            return Convert.ToString(System.Text.Encoding.ASCII.GetString(bytes));
        }

        public int LatterToIndex(string latter)
        {
            var bytes = System.Text.Encoding.ASCII.GetBytes(latter);
            return bytes[0];
        }

        private string QuestionTypeFormat(string value)
        {
            switch (value)
            {
                case QuestionTypeConst.Single:
                    return "单选题";
                case QuestionTypeConst.Multi:
                    return "多选题";
                case QuestionTypeConst.Judge:
                    return "判断题";
                case QuestionTypeConst.Fill:
                    return "填空题";
                case QuestionTypeConst.Answer:
                    return "问答题";
                case QuestionTypeConst.Voice:
                    return "语音题";
                case QuestionTypeConst.Operate:
                    return "操作题";
                case QuestionTypeConst.Typing:
                    return "打字题";
                case QuestionTypeConst.JudgeCorrect:
                    return "判断改错题";
                case QuestionTypeConst.Compose:
                    return "组合题";
                case QuestionTypeConst.Program:
                    return "编程题";
                case QuestionTypeConst.ProgramFill:
                    return "编程填空题";
            }
            return value;
        }

        private string ExamTimeFormat(int value)
        {
            var h = (int)Math.Floor((double)value / 3600);
            var m = (int)Math.Floor((double)value % 3600 / 60);
            var s = value % 60;
            var formart = new Func<int, string>(num =>
            {
                if (num < 10)
                {
                    return "0" + num;
                }
                return "" + num;
            });

            return formart(h) + ":" + formart(m) + ":" + formart(s);
        }

        private string QuestionStatusCodeFormat(string value)
        {
            switch (value)
            {
                case QuestionStautsConst.Normal:
                    return "正常";
                case QuestionStautsConst.Disabled:
                    return "禁用";
                case QuestionStautsConst.Outdated:
                    return "已过期";
                case QuestionStautsConst.Draft:
                    return "草稿";
            }

            return value;
        }

        private string OperateTypeCodeFormat(string value)
        {
            switch (value)
            {
                case OperateTypeConst.Excel:
                    return "Excel";
                case OperateTypeConst.Word:
                    return "Word";
                case OperateTypeConst.PowerPoint:
                    return "Powerpoint";
                case OperateTypeConst.Html:
                    return "html";
            }

            return value;
        }

    }
}