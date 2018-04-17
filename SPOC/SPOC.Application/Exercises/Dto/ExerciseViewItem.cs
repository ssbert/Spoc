using System;
using SPOC.Exercises.Enum;

namespace SPOC.Exercises.Dto
{
    /// <summary>
    /// 前台练习展示数据项
    /// </summary>
    public class ExerciseViewItem
    {
        /// <summary>
        /// 练习ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 练习标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 通过率
        /// </summary>
        public decimal PassRate { get; set; }

        /// <summary>
        /// 完成人数
        /// </summary>
        public int FinishedNum { get; set; }

        /// <summary>
        /// 用户练习状态
        /// </summary>
        public UserExerciseStateEnum UserState { get; set; }

        /// <summary>
        /// 练习结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 练习花费时间
        /// </summary>
        public TimeSpan? UseTime { get; set; }

        /// <summary>
        /// 效率排名
        /// </summary>
        public int Ranking { get; set; }
    }
}