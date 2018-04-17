namespace SPOC.Web.Models
{
    public class ResultModel<T>
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 返回结果
        /// </summary>
        public T Result { get; set; }
    }
}