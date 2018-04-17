using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPOC.Exam.Dto.Judge
{
    /// <summary>
    /// 保存评卷结果返回结果
    /// </summary>
    public class JudgeResultOutputDto
    {
        /// <summary>
        /// 考试成绩
        /// </summary>
        public decimal GradeScore { get; set; }
        /// <summary>
        /// 得分率
        /// </summary>
        public decimal GradeRate { get; set; }
    }
}
