using System;
using System.Linq;
using Abp.Application.Services;
using Abp.Domain.Repositories;

namespace SPOC.Exam
{
    public class ExamInfoChangeService:ApplicationService, IExamInfoChangeService
    {
        private readonly IRepository<ExamInfoChange, Guid> _iExamInfoChangeRep;
        private readonly IRepository<ExamCacheTask, Guid> _iExamCacheTaskRep;

        public ExamInfoChangeService(IRepository<ExamInfoChange, Guid> iExamInfoChangeRep, IRepository<ExamCacheTask, Guid> iExamCacheTaskRep)
        {
            _iExamInfoChangeRep = iExamInfoChangeRep;
            _iExamCacheTaskRep = iExamCacheTaskRep;
        }

        /// <summary>
        /// 考试信息发生变动事件
        /// </summary>
        /// <param name="eventArg"></param>
        public void ExamInfoChanged(ExamEventArg eventArg)
        {

            //===2、生成考试相关文件==========
            InsertExamInfoChange("考试信息变动", EnumExamInfoChangeClassCode.ExamInfo, eventArg.ChangeType, eventArg.ObjectUid, eventArg.RelativeUid, string.Empty, eventArg.OperatorUid);
        }

        /// <summary>
        /// 考试安排信息发生变动事件
        /// </summary>
        /// <param name="eventArg"></param>
        public void ExamArrangeChanged(ExamEventArg eventArg)
        {

            //===2、生成考试相关文件==========
            InsertExamInfoChange("考试安排信息变动", EnumExamInfoChangeClassCode.ExamArrange, eventArg.ChangeType, eventArg.ObjectUid, eventArg.RelativeUid, string.Empty, eventArg.OperatorUid);
        }

        /// <summary>
        /// 试卷信息发生变动事件
        /// </summary>
        /// <param name="eventArg"></param>
        public void ExamPaperInfoChanged(ExamEventArg eventArg)
        {
            //===1、生成考试相关文件==========
            InsertExamInfoChange("试卷信息变动", EnumExamInfoChangeClassCode.ExamPaper, eventArg.ChangeType, eventArg.ObjectUid, eventArg.RelativeUid, string.Empty, eventArg.OperatorUid);

        }

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
        public void InsertExamInfoChange(string title, string changeClass, string changeType, Guid objectUid, string relativeUid, string remarks, Guid operatorUid)
        {
            //检查数据库中是否有相同的任务未处理，如果有则直接返回
            var changeInfos = _iExamInfoChangeRep.GetAll().Where(
                a =>
                    a.objectUid.Equals(objectUid) && a.changeClass.Equals(changeClass) &&
                    a.statusCode.Equals(EnumExamInfoChangeStatusCode.NoStart));

            if (changeInfos.Any())
            {
                //检查ExamCacheTask是否存在相同任务 否则写入ExamCacheTask表
                var taskInfos = _iExamCacheTaskRep.GetAll().Where(
                    a =>
                        a.objectUid.Equals(objectUid) && a.changeClass.Equals(changeClass) &&
                        a.statusCode.Equals(EnumExamInfoChangeStatusCode.NoStart));

                if (taskInfos.Any())
                {
                    return;
                }
                if (changeClass.Equals(EnumExamInfoChangeClassCode.ExamArrange) 
                    || changeClass.Equals(EnumExamInfoChangeClassCode.ExamInfo)
                    || changeClass.Equals(EnumExamInfoChangeClassCode.ExamPaper))
                {
                    var cacheTaskRow = new ExamCacheTask
                    {
                        Id = Guid.NewGuid(),
                        changeTitle = title,
                        changeClass = changeClass,
                        changeType = changeType,
                        createTime = DateTime.Now,
                        creatorUid = operatorUid,
                        objectUid = objectUid,
                        relativeUid = string.IsNullOrEmpty(relativeUid) ? Guid.Empty : Guid.Parse(relativeUid),
                        remark = remarks,
                        statusCode = EnumExamInfoChangeStatusCode.NoStart
                    };
                    _iExamCacheTaskRep.InsertAsync(cacheTaskRow);
                }
                return;
            }


            //写入任务
            var rowObject = new ExamInfoChange
            {
                Id = Guid.NewGuid(),
                changeTitle = title,
                changeClass = changeClass,
                changeType = changeType,
                createTime = DateTime.Now,
                creatorUid = operatorUid,
                objectUid = objectUid,
                relativeUid = string.IsNullOrEmpty(relativeUid) ? Guid.Empty : Guid.Parse(relativeUid),
                remark = remarks,
                statusCode = EnumExamInfoChangeStatusCode.NoStart
            };
            _iExamInfoChangeRep.InsertAsync(rowObject);
            if (rowObject.changeClass.Equals(EnumExamInfoChangeClassCode.ExamArrange) 
                || rowObject.changeClass.Equals(EnumExamInfoChangeClassCode.ExamInfo) 
                || rowObject.changeClass.Equals(EnumExamInfoChangeClassCode.ExamPaper))
            {
                //检查ExamCacheTask是否存在相同任务 否则写入ExamCacheTask表
                var taskInfos = _iExamCacheTaskRep.GetAll().Where(
                    a =>
                        a.objectUid.Equals(objectUid) && a.changeClass.Equals(changeClass) &&
                        a.statusCode.Equals(EnumExamInfoChangeStatusCode.NoStart));

                if (taskInfos.Any())
                {
                    return;
                }
                if (changeClass.Equals(EnumExamInfoChangeClassCode.ExamArrange) 
                    || changeClass.Equals(EnumExamInfoChangeClassCode.ExamInfo) 
                    || changeClass.Equals(EnumExamInfoChangeClassCode.ExamPaper))
                {
                    var cacheTaskRow = new ExamCacheTask
                    {
                        Id = Guid.NewGuid(),
                        changeTitle = title,
                        changeClass = changeClass,
                        changeType = changeType,
                        createTime = DateTime.Now,
                        creatorUid = operatorUid,
                        objectUid = objectUid,
                        relativeUid = string.IsNullOrEmpty(relativeUid) ? Guid.Empty : Guid.Parse(relativeUid),
                        remark = remarks,
                        statusCode = EnumExamInfoChangeStatusCode.NoStart
                    };
                    _iExamCacheTaskRep.InsertAsync(cacheTaskRow);
                }
            }
        }
    }
}