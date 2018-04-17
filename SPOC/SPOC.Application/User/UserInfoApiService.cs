using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Runtime.Validation;
using Newtonsoft.Json;
using SPOC.Common;
using SPOC.Common.Encrypt;
using SPOC.SystemSet;
using SPOC.User.Dto.UserInfo;
using SignatureHelper = SPOC.Common.Helper.SignatureHelper;

namespace SPOC.User
{
    [DisableValidation]
    public class UserInfoApiService : SPOCAppServiceBase, IUserInfoApiService
    {
        private readonly IRepository<Cloud, Guid> _iCloudRepository;

        public UserInfoApiService(IRepository<Cloud, Guid> iCloudRepository)
        {
            _iCloudRepository = iCloudRepository;
        }

        private string NewMoocApiUrl
        {
            get { return L("payUrl").TrimEnd('/') + "/api/"; }
        }


        public static async Task<ApiResponseResult<T>> GetRquestApiResNew<T>(MBasicRequestParamsDTO basicParamsDto, string targetUrl, Dictionary<string, string> dc) where T : class
        {
            var sign = SignatureHelper.GetSignature(dc);
            var url =  targetUrl + sign;
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            var returnValue = "";
            using (var http = new HttpClient(handler))
            {
                var content = new FormUrlEncodedContent(dc);
                //var response = await http.PostAsync(url, content);
                var response = http.PostAsync(url, content).Result;

                //确保HTTP成功状态值 签名失败403
                //response.EnsureSuccessStatusCode();
                //await异步读取最后的JSON
                returnValue = await response.Content.ReadAsStringAsync();
            }
            var responseRes = JsonConvert.DeserializeObject<ApiResponseResult<T>>(returnValue);
            return responseRes;
        }


        private async Task<ApiResponseResult<T>> GetRquestApiRes<T>(MBasicRequestParamsDTO basicParamsDto, string targetUrl, Dictionary<string, string> dc) where T : class
        {
            var sign = SignatureHelper.GetSignature(dc);
            var url = NewMoocApiUrl + targetUrl + sign;
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            var returnValue = "";
            using (var http = new HttpClient(handler))
            {
                var content = new FormUrlEncodedContent(dc);
                  //var response = await http.PostAsync(url, content);
                var response =  http.PostAsync(url, content).Result;

                //确保HTTP成功状态值 签名失败403
                //response.EnsureSuccessStatusCode();
                //await异步读取最后的JSON
                returnValue = await response.Content.ReadAsStringAsync();
            }
            var responseRes = JsonConvert.DeserializeObject<ApiResponseResult<T>>(returnValue);
            return responseRes;
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="basicParamsDto"></param>
        /// <param name="targetUrl">目标Url</param>
        /// <param name="data">对象序列化后的字符串</param>
        /// <returns></returns>
        private async Task<ApiResponseResult<T>> GetRquestApiResByAddObjList<T>(MBasicRequestParamsDTO basicParamsDto, string targetUrl, string data) where T : class
        {
            var apiContent = System.Web.HttpUtility.UrlEncode(data, Encoding.UTF8);
            
            var url = NewMoocApiUrl + targetUrl ;
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            var returnValue = "";
            using (var http = new HttpClient(handler))
            {
                var content = new StringContent(apiContent);
                //   var response = await http.PostAsync(url, content);
                var response = http.PostAsync(url, content).Result;

                //确保HTTP成功状态值 签名失败403
                //response.EnsureSuccessStatusCode();
                //await异步读取最后的JSON
                returnValue = await response.Content.ReadAsStringAsync();
            }
            var responseRes = JsonConvert.DeserializeObject<ApiResponseResult<T>>(returnValue);
            return responseRes;
        }

        private async Task<ApiResponseResult<T>> GetRquestApiResByNotToken<T>(MBasicRequestParamsDTO basicParamsDto, string targetUrl, Dictionary<string, string> dc) where T : class
        {
            var sign = SignatureHelper.GetSignature(dc);
            var url = NewMoocApiUrl + targetUrl; 
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            var returnValue = "";
            using (var http = new HttpClient(handler))
            {
                var content = new FormUrlEncodedContent(dc);
                //   var response = await http.PostAsync(url, content);
                var response = http.PostAsync(url, content).Result;
               
                //确保HTTP成功状态值 签名失败403
                //response.EnsureSuccessStatusCode();
                //await异步读取最后的JSON
                returnValue = await response.Content.ReadAsStringAsync();
            }


            var responseRes = JsonConvert.DeserializeObject<ApiResponseResult<T>>(returnValue);
            return responseRes;
        }
        private async Task<ApiResponseResult<dynamic>> GetRquestApiResByGet(MBasicRequestParamsDTO basicParamsDto, string targetUrl, Dictionary<string, string> dc)
        {
            var sign = SignatureHelper.GetSignature(dc);
            var url = NewMoocApiUrl + targetUrl;
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            var returnValue = "";
            using (var http = new HttpClient(handler))
            {
                var content = new FormUrlEncodedContent(dc);
                //   var response = await http.PostAsync(url, content);
                var response = http.GetAsync(url + sign).Result;

                //确保HTTP成功状态值 签名失败403
                //response.EnsureSuccessStatusCode();
                //await异步读取最后的JSON
                returnValue = await response.Content.ReadAsStringAsync();
            }
            var responseRes = JsonConvert.DeserializeObject<ApiResponseResult<dynamic>>(returnValue);
            return responseRes;
        }

        /// <summary>
        /// 新课网登陆接口
        /// </summary>
        /// <param name="basicParamsDto"></param>
        /// <param name="uName"></param>
        /// <param name="pwd"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        [DisableValidation]
        public Task<ApiResponseResult<ApiUserInfoDto>> Login(MBasicRequestParamsDTO basicParamsDto, string uName, string pwd, string clientId)
        {
            var dc = new Dictionary<string, string>()
                {
                {"username",uName},
                {"password", pwd},
                {"loginkey", clientId}, 
               };
            return GetRquestApiRes<ApiUserInfoDto>(basicParamsDto, "user/UserLogin?sign=", dc);

        }
     

        /// <summary>
        /// 新课网注册接口
        /// </summary>
        /// <param name="basicParamsDto"></param>
        /// <param name="registDto"></param>
        /// <returns></returns>
        Task<ApiResponseResult<string>> IUserInfoApiService.Register(MBasicRequestParamsDTO basicParamsDto, UserRegisterModel registDto)
        {
            var cloud = _iCloudRepository.GetAll().FirstOrDefault();
            var dc = new Dictionary<string, string>()
            {
               // {"userName",registDto.UserName},
                {"mobile", registDto.RegisterMobile},
                {"password", sha1Encrypt.AirEncode(registDto.PassWord ?? "")},
                { "partnerid", cloud==null?"0":cloud.CloudId.ToString()},
                {"regip",registDto.RegisterIpAddress}
            };
            if (!string.IsNullOrEmpty(registDto.UserName)) {
                dc.Add("userName", registDto.UserName);
            }
            if (!string.IsNullOrEmpty(registDto.RegisterEmail))
            {
                dc.Add("email", registDto.RegisterEmail);
            }
            if (!string.IsNullOrEmpty(registDto.gender.ToString()))
            {
                dc.Add("gender", registDto.gender.ToString());
            }
            return GetRquestApiRes<string>(basicParamsDto, "user/UserRegister?sign=", dc);
        }

        /// <summary>
        /// 用户批量导入(批量注册)
        /// </summary>
        /// <param name="basicParamsDto"></param>
        /// <param name="registDtoList"></param>
        /// <returns></returns>
        public Task<ApiResponseResult<string>> UserRegisterByList(MBasicRequestParamsDTO basicParamsDto, List<UserRegisterModel> registDtoList)
        {
            var dc = new List<Dictionary<string, string>>();
            var cloud = _iCloudRepository.GetAll().FirstOrDefault();
            registDtoList.ForEach(registDto => dc.Add(new Dictionary<string, string>()
            {
                {"userName",registDto.UserName},
                {"mobile", registDto.RegisterMobile},
                {"email", registDto.RegisterEmail},
                {"password", sha1Encrypt.AirEncode(registDto.PassWord ?? "")},
                {"partnerid", cloud==null?"0":cloud.CloudId.ToString()},
                {"regip",registDto.RegisterIpAddress}
            }));
            var jsonValue = JsonValue.Parse(JsonConvert.SerializeObject(dc)).ToString();
            return GetRquestApiResByAddObjList<string>(basicParamsDto, "user/UserRegisterByList", jsonValue);
        }

        /// <summary>
        /// 新课网端验证手机号码是否存在
        /// </summary>
        /// <param name="basicParamsDto"></param>
        /// <param name="registDto"></param>
        /// <returns></returns>
        [DisableValidation]
        async Task<ApiResponseResult<object>> IUserInfoApiService.CheckMobile(MBasicRequestParamsDTO basicParamsDto, UserRegisterModel registDto)
            {
            var dc = new Dictionary<string, string>()
            {
                {"mobile", registDto.RegisterMobile}
            };
            return await GetRquestApiRes<object>(basicParamsDto, "user/CheckMobile?sign=", dc);
         //   return await GetRquestApiResByNotToken(basicParamsDto, "user/CheckMobile?mobile=" + registDto.RegisterMobile, dc);
        }

        /// <summary>
        /// 新课网端验证邮箱是否存在
        /// </summary>
        /// <param name="basicParamsDto"></param>
        /// <param name="registDto"></param>
        /// <returns></returns>
       async Task<ApiResponseResult<object>> IUserInfoApiService.CheckEmail(MBasicRequestParamsDTO basicParamsDto, UserRegisterModel registDto)
        {
            var dc = new Dictionary<string, string>()
            {
                {"email", registDto.RegisterEmail}
            };
         //   return await GetRquestApiRes(basicParamsDto, "user/CheckEmail?sign=", dc);
            return await GetRquestApiResByNotToken<object>(basicParamsDto, "user/CheckEmail?email=" + registDto.RegisterEmail, dc);
        }

        /// <summary>
        /// 新课网端验证用户名是否存在
        /// </summary>
        /// <param name="basicParamsDto"></param>
        /// <param name="registDto"></param>
        /// <returns></returns>
       async Task<ApiResponseResult<object>> IUserInfoApiService.CheckUserName(MBasicRequestParamsDTO basicParamsDto, UserRegisterModel registDto)
        {
            var dc = new Dictionary<string, string>()
            {
                {"username", registDto.UserName}
            };
           // return await GetRquestApiRes(basicParamsDto, "user/CheckUserName?sign=", dc);
            return await GetRquestApiResByNotToken<object>(basicParamsDto, "user/CheckUserName?username=" + registDto.UserName, dc);
        }

        /// <summary>
        /// 根据手机号修改用户信息
        /// </summary>
        /// <param name="basicParamsDto"></param>
        /// <param name="registDto"></param>
        /// <returns></returns>
       async Task<ApiResponseResult<object>> IUserInfoApiService.ModifryUserByMobile(MBasicRequestParamsDTO basicParamsDto, EditDto editDto)
       {

         /*  var dc = new Dictionary<string, string>()
            {
                {"gender",editDto.gender.ToString()}
               ,{"mobile",editDto.mobile}
               ,{"newpw",editDto.newpw}
                ,{"ignoreoldpw",editDto.ignoreoldpw.ToString()}
            };*/
          var dc = new Dictionary<string, string>()
            {
                {"gender",editDto.gender.ToString()}
               ,{"ignoreoldpw",editDto.ignoreoldpw.ToString()}
               ,{"questionid",editDto.questionid.ToString()}
            };
           if (!string.IsNullOrEmpty(editDto.idcard)) { dc.Add("idcard", editDto.idcard); }
           if (!string.IsNullOrEmpty(editDto.oldusername)) { dc.Add("oldusername", editDto.oldusername); }
           if (!string.IsNullOrEmpty(editDto.username)) { dc.Add("username", editDto.username); }
           if (!string.IsNullOrEmpty(editDto.truename)) { dc.Add("truename", editDto.truename); }
           if (!string.IsNullOrEmpty(editDto.oldpw)) { dc.Add("oldpw", editDto.oldpw); }
           if (!string.IsNullOrEmpty(editDto.newpw)) { dc.Add("newpw", editDto.newpw); }
           if (!string.IsNullOrEmpty(editDto.email)) { dc.Add("email", editDto.email); }
           if (!string.IsNullOrEmpty(editDto.mobile)) { dc.Add("mobile", editDto.mobile); }
           if (!string.IsNullOrEmpty(editDto.answer)) { dc.Add("answer", editDto.answer); }
           return await GetRquestApiRes<object>(basicParamsDto, "user/UserEdit?sign=", dc);
       }

        /// <summary>
        /// 修改手机号码
        /// </summary>
        /// <param name="basicParamsDto"></param>
        /// <param name="editDto"></param>
        /// <returns></returns>
       async Task<ApiResponseResult<object>> IUserInfoApiService.ModifryUserMobile(MBasicRequestParamsDTO basicParamsDto, ModifyMobile editDto)
       {
           var dc = new Dictionary<string, string>()
            {
               { "username",editDto.username},
                {"oldmobile",editDto.oldmobile},
                {"newmobile", editDto.newmobile} 
            };
           return await GetRquestApiRes<object>(basicParamsDto, "user/ModifyMobile?sign=", dc);
       }

        /// <summary>
        /// 用户搜索
        /// </summary>
        /// <param name="basicParamsDto"></param>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
       async Task<ApiResponseResult<ApiUserInfoDto>> IUserInfoApiService.UserSearch(MBasicRequestParamsDTO basicParamsDto, string userName, string email, string mobile)
       {
           var dc = new Dictionary<string, string>()
            {
                {"username",userName},
                {"email",email},
                {"mobile",mobile}
            };
           return await GetRquestApiRes<ApiUserInfoDto>(basicParamsDto, "user/UserSearch?sign=", dc);
       }

        /// <summary>
        /// 新课网根据用户名查询
        /// </summary>
        /// <param name="basicParamsDto"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
       async Task<ApiResponseResult<ApiUserInfoDto>> IUserInfoApiService.GetUserInfo(MBasicRequestParamsDTO basicParamsDto, string mobile)
       {
           var dc = new Dictionary<string, string>()
        {
            {"mobile",mobile}
        };
           return await GetRquestApiRes<ApiUserInfoDto>(basicParamsDto, "user/GetUserInfo?sign=", dc);
       }


    }


}
