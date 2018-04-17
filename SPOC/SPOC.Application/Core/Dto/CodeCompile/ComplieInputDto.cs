using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SPOC.Core.Dto.CodeCompile
{
    /// <summary>
    /// 编译输入
    /// </summary>
    public class CompileInputDto
    {

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

    }
}
