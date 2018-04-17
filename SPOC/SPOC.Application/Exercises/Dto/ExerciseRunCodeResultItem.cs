using System;
using Newtonsoft.Json;

namespace SPOC.Exercises.Dto
{
    /// <summary>
    /// 练习代码编译运行结果
    /// </summary>
    public class ExerciseRunCodeResultItem
    {
        /// <summary>
        /// 是否成功运行
        /// </summary>
        public bool IsSuccessedRun { get; set; }
        /// <summary>
        /// 编译错误
        /// </summary>
        public string ComplieError { get; set; }
        /// <summary>
        /// 代码输出
        /// </summary>
        public string Output { get; set; }
        /// <summary>
        /// 正确答案
        /// </summary>
        public string Answer { get; set; }
        /// <summary>
        /// 是否通过
        /// </summary>
        public bool Pass { get; set; }
        /// <summary>
        /// 服务器时间
        /// </summary>
        [JsonConverter(typeof(DateFormat))]
        public DateTime Time { get; set; }
    }
}