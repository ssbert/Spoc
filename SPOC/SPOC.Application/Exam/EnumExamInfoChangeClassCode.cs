namespace SPOC.Exam
{
    public class EnumExamInfoChangeClassCode
    {
        /// <summary>
        /// 考试信息
        /// </summary>
        public const string ExamInfo = "exam_info";

        /// <summary>
        /// 考试安排信息
        /// </summary>
        public const string ExamArrange = "exam_arrange";

        /// <summary>
        /// 试卷信息
        /// </summary>
        public const string ExamPaper = "exam_paper";

        /// <summary>
        /// 考试成绩
        /// </summary>
        public const string ExamGrade = "exam_grade";

        /// <summary>
        /// 考试记录变更
        /// </summary>
        public const string ExamChangeGrade = "exam_change_grade";

        /// <summary>
        /// 考试记录变更（供批量生成答卷使用）
        /// </summary>
        public const string ExamChangeGradePaper = "exam_change_grade_paper";
    }
    public class EnumQuestionBaseTypeCode
    {
        /// <summary>
        /// 基础题型,单项选择题
        /// </summary>
        public const string Single = "single";

        /// <summary>
        /// 基础题型,多项选择题题
        /// </summary>
        public const string Multi = "multi";

        /// <summary>
        /// 基础题型,判断题
        /// </summary>
        public const string Judge = "judge";

        /// <summary>
        /// 基础题型,填空题
        /// </summary>
        public const string Fill = "fill";

        /// <summary>
        /// 基础题型,简答题
        /// </summary>
        public const string Answer = "answer";

        /// <summary>
        /// 基础题型,语音题
        /// </summary>
        public const string Voice = "voice";

        /// <summary>
        /// 基础题型,组合题
        /// </summary>
        public const string Compose = "compose";

        /// <summary>
        /// 基础题型,操作题
        /// </summary>
        public const string Operate = "operate";

        /// <summary>
        /// 基础题型,打字题
        /// </summary>
        public const string Typing = "typing";

        /// <summary>
        /// 基础题型,测评单选题
        /// </summary>
        public const string EvaluationSingle = "eva_single";

        /// <summary>
        /// 基础题型,测评多选题
        /// </summary>
        public const string EvaluationMulti = "eva_multi";

        /// <summary>
        /// 判断改错题
        /// </summary>
        public const string JudgeCorrect = "judge_correct";

        /// <summary>
        /// 基础题型，编程题
        /// </summary>
        public const string Program = "program";

        /// <summary>
        /// 编程填空题
        /// </summary>
        public const string ProgramFill = "program_fill";
    }

}