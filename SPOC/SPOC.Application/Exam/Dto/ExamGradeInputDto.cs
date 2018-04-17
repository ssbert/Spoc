using System;

namespace SPOC.Exam.Dto
{
    /// <summary>
    /// 提交答卷到服务器DTO
    /// </summary>
    public  class ExamGradeInputDto
    {
        /// <summary>
        /// 考试ID
        /// </summary>
        public Guid examGradeUid { get; set; }
        /// <summary>
        /// 考试答卷信息
        /// </summary>
        public string userAnswer { get; set; }
        /// <summary>
        /// 考生ID
        /// </summary>
        public Guid examUserUid { get; set; } 
        /// <summary>
        /// 来自考试管理员的强行提交答卷命令
        /// </summary>
        public string isForceToSubmit { get; set; }
        /// <summary>
        /// 来自考试管理员的强行提交答卷命令
        /// </summary>
        public string forceReasonMessage { get; set; }
        /// <summary>
        /// 来自考试管理员的强行提交答卷命令
        /// </summary>
        public string afterAnswerMessage { get; set; }

    }
}
