using System;
using Abp.Application.Services;

namespace SPOC.Exam
{
    public interface IExamInfoChangeService:IApplicationService
    {
        /// <summary>
        /// 考试信息发生变动事件
        /// </summary>
        /// <param name="eventArg"></param>
        void ExamInfoChanged(ExamEventArg eventArg);
        /// <summary>
        /// 考试安排信息发生变动事件
        /// </summary>
        /// <param name="eventArg"></param>
        void ExamArrangeChanged(ExamEventArg eventArg);
        /// <summary>
        /// 试卷信息发生变动事件
        /// </summary>
        /// <param name="eventArg"></param>
        void ExamPaperInfoChanged(ExamEventArg eventArg);
        /// <summary>
        /// 考试变更通知(ExamInfoChange)
        /// </summary>
        /// <param name="title">信息</param>
        /// <param name="changeClass">分组</param>
        /// <param name="changeType">变更类别</param>
        /// <param name="objectUid"></param>
        /// <param name="relativeUid"></param>
        /// <param name="remarks">记录说明</param>
        /// <param name="operatorUid"></param>
        void InsertExamInfoChange(string title, string changeClass, string changeType, Guid objectUid,
            string relativeUid, string remarks, Guid operatorUid);
    }
}