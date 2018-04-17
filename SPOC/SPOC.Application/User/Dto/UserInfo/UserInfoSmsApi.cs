using Newtonsoft.Json;

namespace SPOC.User.Dto.UserInfo
{
    public class UserInfoSmsApi
    {

        /// <summary>
        /// 非0表示失败
        /// </summary>
       [JsonProperty("result")]
        public string result { get; set; }


        /// <summary>
        /// result非0时的具体错误信息
        /// </summary>
        [JsonProperty("errmsg")]
        public string errmsg { get; set; }
        /// <summary>
        /// 用户的session内容，腾讯server回包中会原样返回
        /// </summary>
        [JsonProperty("ext")]
  
        public string ext { get; set; }
        /// <summary>
        /// 标识本次发送id
        /// </summary>
        [JsonProperty("sid")]
      
        public string sid { get; set; }
        /// <summary>
        /// 短信计费的条数
        /// </summary>
        [JsonProperty("fee")]
     
        public int fee { get; set; }
    }

    public class ApiSmsRequest<T> where T : class
    {
        private telModel<T> _tel = new telModel<T>();
        public telModel<T> tel
        {
            get
            {
                return _tel;
            }
            set
            {
                value = _tel;
            }
        }
        /// <summary>
        /// 0:普通短信;1:营销短信
        /// </summary>
        [JsonProperty("type")]
      
        public string type { get; set; }
        /// <summary>
        /// 验证码,utf8编码
        /// </summary>
        [JsonProperty("msg")]
   
        public string msg { get; set; }
        /// <summary>
        /// app凭证
        /// </summary>
        [JsonProperty("sig")]
      
        public string sig { get; set; }
        /// <summary>
        /// 可选字段，默认没有开通(需要填空)。通道扩展码，在短信回复场景中，腾讯server会原样返回，开发者可依此区分是哪种类型的回复
        /// </summary>
        [JsonProperty("extend")]
       
        public string extend { get; set; }
        /// <summary>
        /// 可选字段，用户的session内容,腾讯server回包中会原样返回
        /// </summary>
        [JsonProperty("ext")]
      
        public string ext { get; set; }

    }

    public class telModel<T> where T : class
    {  /// <summary>
        /// 国家码
        /// </summary>
        [JsonProperty("nationcode")]
      
        public string nationcode { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        [JsonProperty("phone")]
      
        public string phone { get; set; }
    }
}
