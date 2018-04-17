using Abp.Application.Services;
using SPOC.Core.Dto.CodeCompile;
using System.Threading.Tasks;

namespace SPOC.Core
{
    /// <summary>
    /// 编译服务
    /// </summary>
    public interface ICodeComplieService:IApplicationService
    {
        /// <summary>
        /// 编译代码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ComplieOutDto<string>> Compile(CompileInputDto input);
        /// <summary>
        /// 多次编译代码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ComplieOutDto<string>> MultiCompile(CompileInputDto input);
    }
}