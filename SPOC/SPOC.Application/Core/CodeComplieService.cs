using Abp.Application.Services;
using Abp.UI;
using Castle.Core.Logging;
using SPOC.Common.Cookie;
using SPOC.Common.Http;
using SPOC.Core.Dto.CodeCompile;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Abp.Json;
using SPOC.Common;
using SPOC.Common.Helper;

namespace SPOC.Core
{
    /// <summary>
    /// 编译服务
    /// </summary>
    public class CodeComplieService : SPOCAppServiceBase, ICodeComplieService
    {
        private string NewMoocApiUrl => L("payUrl").TrimEnd('/') + "/api/";
        private readonly ILogger _iLogger;
        /// <summary>
        /// 构造函数注入
        /// </summary>
        public CodeComplieService()
        {
            _iLogger = new NullLogger();
        }

        /// <summary>
        /// 编译代码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ComplieOutDto<string>> Compile(CompileInputDto @input)
        {

            var cookie = CookieHelper.GetLoginInUserInfo();
            if (cookie == null || cookie.Id == Guid.Empty)
            {
                throw new UserFriendlyException(-1, "未登录系统或登录已经失效，请重新登录");
            }
            if (string.IsNullOrWhiteSpace(input.Code))
            {
                return new ComplieOutDto<string> { Code =2, Msg = "空白代码" };
            }
            var postParam = new Dictionary<string, string> { { "Language", input.Language }, { "AnswerCode", HttpUtility.UrlEncode(input.Code, Encoding.UTF8)  }, { "Param", input.Param } ,{ "InputParam", input.InputParam } };
            var sign = SignatureHelper.GetSignature(postParam);
            var postData = new {  input.Language , AnswerCode = HttpUtility.UrlEncode(input.Code, Encoding.UTF8) ,  input.Param , input.InputParam };
            var url = NewMoocApiUrl + "/compile/run?sign="+ sign;
            var result = await HttpHelper.PostResponseJson<ApiResponseResult<ComplieOutDto<string>>>(url, postData.ToJsonString());
            return result.Data.InKey;

        }
        /// <summary>
        /// 多次编译运行代码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ComplieOutDto<string>> MultiCompile(CompileInputDto input)
        {
            var result = new ComplieOutDto<string>{Code = 0};
            foreach (var para in input.InputParam.Split('|'))
            {
                var compileResult = await Compile(new CompileInputDto{
                    Code = input.Code,InputParam = para,Language= input.Language,Param = input.Param
                });
                if (compileResult.Code == 0)
                    result.Result += compileResult.Result + "|";
                else
                {
                    //编译出现失败 停止多次编译返回错误信息
                    result = compileResult;
                    result.Result = "";
                    result.Msg += string.IsNullOrEmpty(input.Param)
                        ? ""
                        : "\r\nMain函数参数:" + input.Param;
                    result.Msg += string.IsNullOrEmpty(para)
                        ? ""
                        : "\r\n输入流参数:" + para;

                    break;
                }
            }
            result.Result=result.Result.TrimEnd('|');
            return result;
        }
    }

}