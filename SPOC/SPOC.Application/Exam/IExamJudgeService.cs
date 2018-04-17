using Abp.Application.Services;
using SPOC.Exam.Dto.Judge;
using System.Threading.Tasks;

namespace SPOC.Exam
{
    /// <summary>
    /// 评卷操作接口
    /// </summary>
    public interface IExamJudgeService: IApplicationService
    {
        /// <summary>
        /// 提交评卷
        /// </summary>
        /// <returns></returns>
        Task<JudgeResultOutputDto> SubmitJudge();
    }
}
