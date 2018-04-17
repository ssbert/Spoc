using System;
using System.Collections.Generic;
using SPOC.Lib.Dto;

namespace SPOC.Web.Models.LibLabel
{
    public class LibLabelViewModel
    {
        /// <summary>
        /// LabelId
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 当前页
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// 试题类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 试题来源
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 题干
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 答题状态
        /// 0：所有
        /// 1：错误（未通过）
        /// 2：正确（通过）
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 分页数据
        /// </summary>
        public List<UserAnswerRecordsPaginationItem> Rows { get; set; }

        /// <summary>
        /// 数据总数
        /// </summary>
        public int Total { get; set; }
    }
}