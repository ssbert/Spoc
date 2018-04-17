using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Runtime.Validation;
using Newtonsoft.Json;
using SPOC.Common.Pagination;
using SPOC.Exam;

namespace SPOC.Core.Dto.Challenge
{
    public class ChallengeInputDto: PaginationInputDto
    {
       
        /// <summary>
        /// 分类Id
        /// </summary>
        public string folderId { get; set; }
        /// <summary>
        /// 难度
        /// </summary>
        public string hard { get; set; }
        /// <summary>
        /// 是否通过
        /// </summary>
        public string isPass { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        public string label { get; set; }
        /// <summary>
        /// 当前页
        /// </summary>
        public int currentPage { get; set; }
    }
    #region 挑战题
    [AutoMapTo(typeof(ChallengeQuestion))]
    public class ChallengeQuestionInputDto : IShouldNormalize
    {
        public ChallengeQuestionInputDto()
        {
            operateTypeCode = "";
            hardGrade = "";
            selectAnswer = "";
            selectAnswerScore = "";
            standardAnswer = "";
            questionAnalysis = "";
            questionStatusCode = "";
        }

        public Guid Id { get; set; }
        public string title { get; set; }
        /// <summary>
        /// 分类ID
        /// </summary>
        [Required]
        public Guid folderUid { get; set; }

        /// <summary>
        /// 上级试题ID
        /// </summary>
        public Guid parentQuestionUid { get; set; }

        /// <summary>
        /// 试题类型ID
        /// </summary>
        [Required]
        public Guid questionTypeUid { get; set; }

        /// <summary>
        /// 试题类型编号
        /// </summary>
        [Required]
        public string questionBaseTypeCode { get; set; }

        /// <summary>
        /// 操作题类型
        /// </summary>
        public string operateTypeCode { get; set; }

        /// <summary>
        /// 编程语言
        /// </summary>
        public string language { get; set; }

        /// <summary>
        /// 答题时间
        /// </summary>
        public int examTime { get; set; }

        /// <summary>
        /// 试题内容
        /// </summary>
        [Required]
        public string questionText { get; set; }

        /// <summary>
        /// 难度
        /// </summary>
        public string hardGrade { get; set; }

        /// <summary>
        /// 分数
        /// </summary>
        public decimal score { get; set; }

        /// <summary>
        /// 可选答案
        /// </summary>
        public string selectAnswer { get; set; }

        /// <summary>
        /// 可选答案分数
        /// </summary>
        public string selectAnswerScore { get; set; }

        /// <summary>
        /// 每选项得扣分
        /// </summary>
        public decimal eachOptionScore { get; set; }

        /// <summary>
        /// 可选答案个数
        /// </summary>
        public int answerNum { get; set; }

        /// <summary>
        /// 标准答案
        /// </summary>
        public string standardAnswer { get; set; }

        /// <summary>
        /// 参考标准代码
        /// </summary>
        public string standardCode { get; set; }
        /// <summary>
        /// 试题分析
        /// </summary>
        public string questionAnalysis { get; set; }

        /// <summary>
        /// 过期日期
        /// </summary>
        public int outdatedDate { get; set; }

        /// <summary>
        /// 试题状态
        /// </summary>
        public string questionStatusCode { get; set; }

        /// <summary>
        /// 试题排序号
        /// </summary>
        public int listOrder { get; set; }

        /// <summary>
        /// 试题编号
        /// </summary>
        [MaxLength(64)]
        public string questionCode { get; set; }

        /// <summary>
        /// 是否是自定义编号
        /// </summary>
        [Required]
        public bool isCustomCode { get; set; }

        /// <summary>
        /// 学科Id
        /// </summary>
        public Guid subjectUid { get; set; }
        /// <summary>
        /// 组织架构ID
        /// </summary>
        public Guid departmentUid { get; set; }

        /// <summary>
        /// 程序题参数
        /// </summary>
        public string param { get; set; }

        /// <summary>
        /// 输入流参数
        /// </summary>
        public string InputParam { get; set; }

        /// <summary>
        /// 多次测试
        /// </summary>
        public bool MultiTest { get; set; }
        /// <summary>
        /// 预设代码
        /// </summary>
        public string PreinstallCode { get; set; }
        /// <summary>
        /// 主标签
        /// </summary>
        public List<Guid> label { get; set; }
        /// <summary>
        /// 辅助标签
        /// </summary>
        public List<Guid> seclabel { get; set; }
        public void Normalize()
        {
            if (param == null)
            {
                param = string.Empty;
            }
        }
    }
    #endregion
    /// <summary>
    /// 编译输入
    /// </summary>
    public class ProblemInputDto
    {
        public string id { get; set; }
        /// <summary>
        /// 答题代码
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        [JsonProperty("param")]
        public string Param { get; set; }
        /// <summary>
        /// 控制台参数
        /// </summary>
        [JsonProperty("inputParam")]
        public string InputParam { get; set; }
        /// <summary>
        /// 编程语言 1:C++  0：C
        /// </summary>
        [JsonProperty("language")]
        public string Language { get; set; }
        /// <summary>
        /// 开始答卷时间
        /// </summary>
        [JsonProperty("answerTime")]
        public int AnswerTime { get; set; }
    }
    #region 报表排行榜
    public class RankInputDto : PaginationInputDto
    {
        /// <summary>
        /// 挑战题ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 学生ID
        /// </summary>
        public string userId { get; set; }
        /// <summary>
        /// 学生姓名
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string loginName { get; set; }
        /// <summary>
        /// 班级
        /// </summary>
        public string classId { get; set; }
    }

    public class UserAnswerInputDto : PaginationInputDto
    {
        /// <summary>
        /// 数据ID
        /// </summary>
        public Guid id { get; set; }
        /// <summary>
        /// 学生ID
        /// </summary>
        public Guid userId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 题干
        /// </summary>
        public string questionText { get; set; }
        /// <summary>
        /// 难度
        /// </summary>
        public string hardGrade { get; set; }

    }

    #endregion
}
