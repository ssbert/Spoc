using SPOC.Common.Const;

namespace SPOC.QuestionBank.Const
{
    /// <summary>
    /// 试题类型常量
    /// </summary>
    public class QuestionTypeConst:BaseConst<QuestionBankService>
    {
        /// <summary>
        /// 单选题
        /// </summary>
        public const string Single = "single";
        /// <summary>
        /// 多选题
        /// </summary>
        public const string Multi = "multi";
        /// <summary>
        /// 判断题
        /// </summary>
        public const string Judge = "judge";
        /// <summary>
        /// 填空题
        /// </summary>
        public const string Fill = "fill";
        /// <summary>
        /// 问答题
        /// </summary>
        public const string Answer = "answer";
        /// <summary>
        /// 语音题
        /// </summary>
        public const string Voice = "voice";
        /// <summary>
        /// 组合题
        /// </summary>
        public const string Compose = "compose";
        /// <summary>
        /// 操作题
        /// </summary>
        public const string Operate = "operate";
        /// <summary>
        /// 打字题
        /// </summary>
        public const string Typing = "typing";
        /// <summary>
        /// 判断改错题
        /// </summary>
        public const string JudgeCorrect = "judge_correct";

        /// <summary>
        /// 测评单选题
        /// </summary>
        public const string EvaluationSingle = "eva_single";

        /// <summary>
        /// 测评多选题
        /// </summary>
        public const string EvaluationMulti = "eva_multi";

        /// <summary>
        /// 编程题
        /// </summary>
        public const string Program = "program";

        /// <summary>
        /// 编程填空题
        /// </summary>
        public const string ProgramFill = "program_fill";
    }
}