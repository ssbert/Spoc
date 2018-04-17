using Newtonsoft.Json.Converters;

namespace SPOC
{
    /// <summary>
    /// 日期格式化
    /// </summary>
    public class DateFormat: IsoDateTimeConverter
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DateFormat()
        {
            base.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        }
    }
}