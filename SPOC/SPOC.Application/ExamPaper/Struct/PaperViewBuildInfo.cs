using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using SPOC.Exam;
using SPOC.QuestionBank.Dto;

namespace SPOC.ExamPaper.Struct
{
    /// <summary>
    /// 生成预览试卷时所需要的参数
    /// </summary>
    public struct PaperViewBuildInfo
    {
        public string ViewType;
        public bool IsMixOrder;
        public Guid ExamGradeUid;
        public Exam.ExamPaper ExamPaperRow;
        public List<ExamPaperNode> ExamPaperNodeRowCollection;
        public List<ExamPaperNodeQuestion> ExamPaperNodeQuestionRowCollection;
        public List<ExamQuestionDto> ExamQuestionRowCollection;
        public DataTable UserAnswerDataTable;
        public Hashtable QuesionIndex;
        public bool IsAllowModifyObjectAnswer;
    }
}