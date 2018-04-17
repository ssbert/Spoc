namespace SPOC.ExamPaper.Dto
{
    /// <summary>
    /// 学生答卷信息输出
    /// </summary>
    public class ExamPaperViewOutputDto
    {
        /// <summary>
        /// 成绩ID
        /// </summary>
        public string examUserGradeID { get; set; }
        /// <summary>
        /// 考生姓名
        /// </summary>
        public string examUserName { get; set; }
        /// <summary>
        /// 考试开始时间
        /// </summary>
        public string examBeginTime { get; set; }
        /// <summary>
        /// 考试结束时间
        /// </summary>
        public string examEndTime { get; set; }
        /// <summary>
        /// 考试时间
        /// </summary>
        public string examTime { get; set; }
        /// <summary>
        /// 评卷人
        /// </summary>
        public string judgeRealName { get; set; }
        /// <summary>
        /// 评卷开始时间
        /// </summary>
        public string JudgeBeginTime { get; set; }
        /// <summary>
        /// 成绩
        /// </summary>
        public string examTotalScore { get; set; }
        /// <summary>
        /// 答卷显示内容
        /// </summary>
        public string viewHtml { get; set; }
        /// <summary>
        /// 试卷标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 试卷子标题
        /// </summary>
        public string subTitle { get; set; }
    }
    public class EnumJudgeResultCode
    {
        public const string Right = "right";
        public const string Error = "error";
        public const string Middle = "middle";
    }
    public class EnumExamDoModeCode
    {
        public const string Paper = "paper";
        public const string Question = "question";
        public const string Node = "node";
    }
    /// <summary>
    /// 试卷查看类型
    /// </summary>
    public class EnumPaperViewType
    {
        /// <summary>
        /// 预览试卷
        /// </summary>
        public const string Preview = "preview";

        /// <summary>
        /// 考试时用
        /// </summary>
        public const string Exam = "exam";

        /// <summary>
        /// 练习时用
        /// </summary>
        public const string Exercise = "exercise";

        /// <summary>
        /// 查看练习答卷
        /// </summary>
        public const string ViewExerciseAnswer = "view_exercise_answer";

        /// <summary>
        /// 查看考生答卷
        /// </summary>
        public const string ViewAnswer = "view_answer";

        /// <summary>
        /// 查看考生答卷(无成绩)
        /// </summary>
        public const string ViewAnswerNoGrade = "ViewAnswerNoGrade";

        /// <summary>
        /// 评卷时用的
        /// </summary>
        public const string Judge = "judge";

        /// <summary>
        /// 统计时用
        /// </summary>
        public const string Analyze = "analyze";

        /// <summary>
        /// 练习试题统计
        /// </summary>
        public const string ExerciseQuestionAnalyze = "exercise_question_analyze";

        /// <summary>
        /// 试卷分析时用
        /// </summary>
        public const string ExamAnalyze = "examanalyze";
        /// <summary>
        /// 显示直方图时用
        /// </summary>
        public const string Histogram = "histogram";

        public const string ViewUserAnswer = "viewUserAnswer";

        public const string ViewUserAnswerWithAnswer = "viewUserAnswerWithAnswer";

    }
}
