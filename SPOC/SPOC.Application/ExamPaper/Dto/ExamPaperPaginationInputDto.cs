using Abp.Runtime.Validation;
using SPOC.Common.Pagination;
using System;
using System.Collections.Generic;

namespace SPOC.ExamPaper.Dto
{
    public class ExamPaperPaginationInputDto:PaginationInputDto, IShouldNormalize
    {
        /// <summary>
        /// 编号 
        /// </summary>
        public string paperCode { get; set; }
        /// <summary>
        /// 试卷标题
        /// </summary>
        public string paperName { get; set; }
        /// <summary>
        /// 创建者姓名
        /// </summary>
        public string userFullName { get; set; }
        /// <summary>
        /// 创建者用户名
        /// </summary>
        public string userLoginName { get; set; }
        /// <summary>
        /// 试卷类型
        /// </summary>
        public string paperTypeCode { get; set; }
        /// <summary>
        /// 组织架构
        /// </summary>
        public List<Guid> departmentUidList { get; set; }
        /// <summary>
        /// 学科
        /// </summary>
        public List<Guid> subjectUidList { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public List<Guid> folderUidList { get; set; }
        /// <summary>
        /// 检测过期时间
        /// </summary>
        public bool checkOutDate { get; set; }
        public void Normalize()
        {
            if (subjectUidList == null)
            {
                subjectUidList = new List<Guid>();
            }

            if (departmentUidList == null)
            {
                departmentUidList = new List<Guid>();
            }

            if (folderUidList == null)
            {
                folderUidList = new List<Guid>();
            }
        }
    }
}