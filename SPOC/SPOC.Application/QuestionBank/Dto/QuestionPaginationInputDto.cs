using Abp.Runtime.Validation;
using SPOC.Common.Pagination;
using System;
using System.Collections.Generic;

namespace SPOC.QuestionBank.Dto
{
    public class QuestionPaginationInputDto : PaginationInputDto, IShouldNormalize
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }
        //创建人登录名
        public string userLoginName { get; set; }
        //创建者姓名
        public string userFullName { get; set; }
        //内容
        public string language { get; set; }
        //内容
        public string questionText { get; set; }
        //编号
        public string questionCode { get; set; }
        //题型
        public string questionBaseTypeCode { get; set; }
        //父试题ID
        public Guid parentQuestionUid { get; set; }
        //试题分类
        public List<Guid> folderUidList { get; set; }
        //状态
        public string questionStatusCode { get; set; }
        //难度
        public string hardGrade { get; set; }
        //操作题型
        public string operateTypeCode { get; set; }
        //是否包含子试题（是否是组合题）
        public string hasChild { get; set; }
        //是否包含试题分析
        public string hasAnalysis { get; set; }
        //学科ID
        public List<Guid> subjectUidList { get; set; }
        //组织架构ID
        public List<Guid> departmentUidList { get; set; }
        //排除在外的试题id
        public List<Guid> exceptionIdList { get; set; }
        /// <summary>
        /// 标签ID列表
        /// </summary>
        public List<Guid> labelIdList { get; set; }
        /// <summary>
        /// 标签分类ID列表
        /// </summary>
        public List<Guid> labelFolderIdList { get; set; }


        public void Normalize()
        {
            if (string.IsNullOrEmpty(questionText))
            {
                questionText = "";
            }
            if (string.IsNullOrEmpty(questionCode))
            {
                questionCode = "";
            }

            if (folderUidList == null)
            {
                folderUidList = new List<Guid>();
            }

            if (exceptionIdList == null)
            {
                exceptionIdList = new List<Guid>();
            }

            if (subjectUidList == null)
            {
                subjectUidList = new List<Guid>();
            }

            if (departmentUidList == null)
            {
                departmentUidList = new List<Guid>();
            }

            if (labelIdList == null)
            {
                labelIdList = new List<Guid>();
            }
            if (labelFolderIdList == null)
            {
                labelFolderIdList = new List<Guid>();
            }
        }
    }
}