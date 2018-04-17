using System;

namespace SPOC.Exercises.Dto
{
    /// <summary>
    /// 编程练习代码提交
    /// </summary>
    public class ExerciseCodeInputDto
    {
        /// <summary>
        /// 练习ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 程序参数
        /// </summary>
        public string Param { get; set; }

        /// <summary>
        /// 输入流参数
        /// </summary>
        public string InputParam { get; set; }

        /// <summary>
        /// 是否是测试运行
        /// </summary>
        public bool IsTestRun { get; set; }
    }
}