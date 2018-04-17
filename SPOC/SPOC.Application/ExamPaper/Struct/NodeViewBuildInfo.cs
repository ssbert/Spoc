using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using SPOC.Exam;
using SPOC.QuestionBank.Dto;

namespace SPOC.ExamPaper.Struct
{
    public struct NodeViewBuildInfo
    {
        public string ViewType;
        public bool IsMixOrder;
        public Guid ExamGradeUid;
        public Exam.ExamPaper ExamPaperRow;
        public List<ExamPaperNodeQuestion> ExamPaperNodeQuestionRowCollection;
        public List<ExamQuestionDto> ExamQuestionRowCollection;
        public DataTable UserAnswerDataTable;
        public Hashtable QuestionIndex;
        public int PaperNodeNumber;
        public ExamPaperNode ExamPaperNodeRow;
        public bool IsSingleAsMulti;
        public bool IsAllowModifyObjectAnswer;
    }
}