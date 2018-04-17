using System.Collections.Generic;
using SPOC.Exercises.Dto;

namespace SPOC.Web.Models.Exercises
{
    public class RecordViewModel
    {
        public RecordViewModel()
        {
            RecordList = new List<ExerciseRecordItem>();
            Answer = "";
        }
        /// <summary>
        /// 练习基础信息
        /// </summary>
        public ExerciseBaseViewOutputDto Base { get; set; }
        /// <summary>
        /// 记录列表
        /// </summary>
        public List<ExerciseRecordItem> RecordList { get; set; }
        /// <summary>
        /// 提交的答案
        /// </summary>
        public string Answer { get; set; }
    }
}