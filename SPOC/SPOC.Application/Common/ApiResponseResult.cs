/********************************************************************************
** auth： bert
** date： 2016/5/24 15:29:08
** desc： 
*********************************************************************************/

using System.Collections.Generic;
using Newtonsoft.Json;

namespace SPOC.Common
{
    /// <summary>
    /// 新课网API接口返回对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResponseResult<T> where T : class
    {
        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; }
        [JsonProperty("errMsg")]
        public string ErrMsg { get; set; }
        [JsonProperty("timeStamp")]
        public string TimeStamp { get; set; }
        [JsonProperty("nonce")]
        public string Nonce { get; set; }
        [JsonProperty("sign")]
        public string Sign { get; set; }

        public ApiResponseTrueResult<T> Data { get; set; }
    }
    public class ApiResponseTrueResult<T> where T : class
    {
        [JsonProperty("msg")]
        public string Msg { get; set; }
        [JsonProperty("inKey")]
        public T InKey { get; set; }
    }

    public class ApiResponseListResult<T> where T : class
    {
        public bool isSuccess { get; set; }
        public string errMsg { get; set; }
        public string timeStamp { get; set; }
        public string nonce { get; set; }
        public string sign { get; set; }

        public ApiReponseTrueListResult<T> data { get; set; }
    }

    public class ApiReponseTrueListResult<T> where T : class
    {
        public string msg { get; set; }

        public List<T> inkey { get; set; }
    }

}
