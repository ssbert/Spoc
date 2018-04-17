using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using newv.common;
using Newtonsoft.Json;

namespace SPOC.Common.Http
{
    /// <summary>
    /// http请求类
    /// </summary>
    public class HttpHelper
    {
        private const int TimeOut = 10000; //设置连接超时时间，默认10秒，可以根据具体需求适当更改timeOut的值

        /// <summary>
        /// HttpGet请求
        /// </summary>
        /// <param name="targetUrl">请求地址</param>
        /// <param name="dc">签名参数</param>
        /// <typeparam name="T">返回类型</typeparam>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(string targetUrl, Dictionary<string, string> dc) where T : class
        {

            var url = targetUrl;
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            var returnValue = "";
            using (var http = new HttpClient(handler))
            {
                // http.Timeout =new TimeSpan(TimeOut);
                var response = http.GetAsync(url).Result;
                returnValue = await response.Content.ReadAsStringAsync();
            }
            var responseRes = JsonConvert.DeserializeObject<T>(returnValue);
            return responseRes;
        }
        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="targetUrl">请求地址</param>
        /// <param name="dc">参数列表</param>
        /// <typeparam name="T">返回类型</typeparam>
        /// <returns></returns>
        public static async Task<T> PostAsync<T>(string targetUrl, Dictionary<string, string> dc) where T : class, new()
        {

            var url = targetUrl;
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            var returnValue = "";
            using (var http = new HttpClient(handler))
            {
                var content = new FormUrlEncodedContent(dc);
                var response = await http.PostAsync(url, content);
                returnValue = await response.Content.ReadAsStringAsync();
            }
            var responseRes = JsonConvert.DeserializeObject<T>(returnValue);
            return responseRes;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">调用的Api地址</param>
        /// <param name="requestJson">表单数据（json格式）</param>
        /// <returns></returns>
        public static async Task<T> PostResponseJson<T>(string url, string requestJson) where T : class
        {


            HttpContent httpContent = new StringContent(requestJson);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            var returnValue = "";
            using (var http = new HttpClient(handler))
            {
                var response = await http.PostAsync(url, httpContent);
                returnValue = await response.Content.ReadAsStringAsync();
            }
            var responseRes = JsonConvert.DeserializeObject<T>(returnValue);
            return responseRes;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="requestJson"></param>
        /// <returns></returns>
        public static async Task<T> PostResponseSerializeData<T>(string url, string requestJson) where T : class
        {
            var handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip };
            var returnValue = "";
            using (var http = new HttpClient(handler))
            {
                var content = new StringContent(requestJson);
                var response = await http.PostAsync(url, content);
                returnValue = await response.Content.ReadAsStringAsync();
            }
            var responseRes = JsonConvert.DeserializeObject<T>(returnValue);
            return responseRes;

        }
        /// <summary>
        /// post json数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataStr"></param>
        /// <returns></returns>
        public static string HttpPostJson(string url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] payload = System.Text.Encoding.UTF8.GetBytes((postDataStr));
            request.ContentLength = payload.Length;
            Stream writer = request.GetRequestStream();
            writer.Write(payload, 0, payload.Length);
            writer.Flush();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string encoding = response.ContentEncoding;
            if (encoding == null || encoding.Length < 1)
            {
                encoding = "UTF-8"; //默认编码
            }
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
            string retString = reader.ReadToEnd();
            return retString;
        }
        public string PostJson(string url, string jsonParas)
        {

            //创建一个HTTP请求  
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //Post请求方式  
            request.Method = "POST";
            //内容类型
            request.ContentType = "application/x-www-form-urlencoded";

            //设置参数，并进行URL编码  
            //虽然我们需要传递给服务器端的实际参数是JsonParas(格式：[{\"UserID\":\"0206001\",\"UserName\":\"ceshi\"}])，
            //但是需要将该字符串参数构造成键值对的形式（注："paramaters=[{\"UserID\":\"0206001\",\"UserName\":\"ceshi\"}]"），
            //其中键paramaters为WebService接口函数的参数名，值为经过序列化的Json数据字符串
            //最后将字符串参数进行Url编码
            string paraUrlCoded = System.Web.HttpUtility.UrlEncode(jsonParas);

            byte[] payload;
            //将Json字符串转化为字节  
            payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
            //设置请求的ContentLength   
            request.ContentLength = payload.Length;
            //发送请求，获得请求流  

            Stream writer;
            try
            {
                writer = request.GetRequestStream();//获取用于写入请求数据的Stream对象
            }
            catch (Exception)
            {
                writer = null;
                Console.Write("连接服务器失败!");
            }
            //将请求参数写入流
            writer.Write(payload, 0, payload.Length);
            writer.Close();//关闭请求流

            String strValue = "";//strValue为http响应所返回的字符流
            HttpWebResponse response;
            try
            {
                //获得响应流
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                response = ex.Response as HttpWebResponse;
            }
            string encoding = response.ContentEncoding;
            if (encoding == null || encoding.Length < 1)
            {
                encoding = "UTF-8"; //默认编码
            }
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
            string retString = reader.ReadToEnd();
            return retString;
        }
    }
}
