using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;

namespace SPOC.QuestionBank.Dto
{
    /// <summary>
    /// 获取分类下指定难度试题数
    /// </summary>
    public class QuestionNumInputDto: IShouldNormalize
    {
        /// <summary>
        /// 试题类型UID
        /// </summary>
        public Guid questionTypeUid { get; set; }
        /// <summary>
        /// 分类ID列表
        /// </summary>
        public List<Guid> folderUids { get; set; }
        /// <summary>
        /// 知识点ID劣币
        /// </summary>
        public List<Guid> labelIdList { get; set; }
        /// <summary>
        /// 难度
        /// </summary>
        public string hardGrade { get; set; }

        /// <summary>
        /// 试题状态
        /// </summary>
        public string questionStatusCode { get; set; }

        public void Normalize()
        {
            if (folderUids == null)
            {
                folderUids = new List<Guid>();
            }

            if (string.IsNullOrEmpty(questionStatusCode))
            {
                questionStatusCode = "normal";
            }

            if (labelIdList == null)
            {
                labelIdList = new List<Guid>();
            }
        }
    }
}