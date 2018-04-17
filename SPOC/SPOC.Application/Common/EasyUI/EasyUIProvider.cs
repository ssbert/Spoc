using System.IO;
using System.Web;
using Newtonsoft.Json;

namespace SPOC.Common.EasyUI
{
    public class EasyUIProvider
    {
        #region 获取分页参数
        /// <summary>
        /// 获取分页参数
        /// </summary>
        /// <param name="currentRequest"></param>
        /// <returns></returns>
        public EasyUIPager GetPager(HttpRequestBase currentRequest)
        {
            EasyUIPager pager = new EasyUIPager();
            if (currentRequest.Form["page"] != null)
            {
                pager.CurrentPage = int.Parse(currentRequest.Form["page"]);
            }
            if (currentRequest.Form["rows"] != null)
            {
                pager.PageSize = int.Parse(currentRequest.Form["rows"]);
            }
            pager.SortCloumnName = currentRequest.Form["sort"];
            pager.SortOrder = currentRequest.Form["order"];
            pager.Filter = currentRequest.Form["filter"];
            return pager;
        }
        #endregion

       

        #region 获取 Request 值
        /// <summary>
        /// 获取 Request 值
        /// </summary>
        /// <param name="currentRequest"></param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public string RequestStr(HttpRequestBase currentRequest, string param)
        {
            if (currentRequest.Form[param] != null)
            {
                return currentRequest.Form[param];
            }
            return string.Empty;
        }
        #endregion

        /// <summary>
        /// 默认的每页显示的数目
        /// </summary>
        public int PageSize
        {
            get { return 10;}
        }


        #region 把HttpRequest序列化为字符串，之后转换为Json
        /// <summary>
        /// 把HttpRequest序列化为字符串
        /// </summary>
        /// <param name="request">HttpRequest</param>
        /// <returns>String</returns>
        public string GetJson(HttpRequestBase request)
        {
            return HttpUtility.UrlDecode(new StreamReader(request.InputStream).ReadToEnd());
        }
        /// <summary>
        /// 把字符串序列化为对象
        /// </summary>
        /// <typeparam name="T">序列化的对象</typeparam>
        /// <param name="request">HttpRequest</param>
        /// <returns>对象</returns>
        public T DeserializeObject<T>(HttpRequestBase request)
        {
            return JsonConvert.DeserializeObject<T>(GetJson(request));
        }

        /// <summary>
        /// 把字符串序列化为对象
        /// </summary>
        /// <typeparam name="T">序列化的对象</typeparam>
        /// <param name="json">json 字符串</param>
        /// <returns>对象</returns>
        public T DeserializeObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        #endregion
    }
}
