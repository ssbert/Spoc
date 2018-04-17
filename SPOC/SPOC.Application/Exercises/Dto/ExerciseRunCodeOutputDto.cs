using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SPOC.Exercises.Dto
{
    /// <summary>
    /// 练习代码运行结果返回
    /// </summary>
    public class ExerciseRunCodeOutputDto
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerciseRunCodeOutputDto()
        {
            ResultList = new List<ExerciseRunCodeResultItem>();
        }
        /// <summary>
        /// 是否测试运行
        /// </summary>
        public bool IsTestRun { get; set; }
        /// <summary>
        /// 是否通过
        /// </summary>
        public bool IsPass { get; set; }
        /// <summary>
        /// 结果列表
        /// </summary>
        public List<ExerciseRunCodeResultItem> ResultList { get; set; }
    }
}