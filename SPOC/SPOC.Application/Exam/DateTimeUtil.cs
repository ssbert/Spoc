using System;

namespace SPOC.Exam
{
    public class DateTimeUtil
    {
        /// <summary>
        /// 当前Unix时间戳
        /// </summary>
        public static double Now
        {
            get
            {
                DateTime dtNow = DateTime.Parse(DateTime.Now.ToString());
                return ConvertToUnixTime(dtNow);
            }
        }

        public static string NowData
        {
            get
            {
                return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        /// <summary>
        /// 将时间字符串转换为秒数
        /// </summary>
        /// <param name="timeStr">时间字符串</param>
        /// <returns>转换后的秒数</returns>
        public static int ToSecondsFromTimeStr(string timeStr)
        {
            DateTime dt = ToTime(timeStr);
            return dt.Hour * 3600 + dt.Minute * 60 + dt.Second;
        }
        /// <summary>
        /// 转换时间类型的字符串至日期时间类型
        /// </summary>
        /// <param name="strTime">时间字符串</param>
        /// <returns>时间值</returns>
        /// <remarks>如果格式不正确，则返回的是"00:00:00"的时间值</remarks>
        public static DateTime ToTime(string strTime)
        {
            try
            {
                return DateTime.Parse(strTime);
            }
            catch
            {
                return DateTime.Parse("00:00:00");
            }
        }

        /// <summary>
        /// 将时间值转换至时间字符串
        /// </summary>
        /// <param name="time">时间值</param>
        /// <returns>时间字符串</returns>
        public static string ToTimeStr(DateTime time)
        {
            return time.ToString("HH:mm:ss");
        }

        /// <summary>
        /// 时间转化
        /// </summary>
        /// <param name="date">Unix时间戳</param>
        /// <returns></returns>
        public static DateTime ConvertToData(double date)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return dtStart.AddSeconds(date);
        }
        /// <summary>
        /// 时间转化
        /// </summary>
        /// <param name="date">Unix时间戳</param>
        /// <returns></returns>
        public static string ConvertToData(string doubledate)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            if (string.IsNullOrEmpty(doubledate))
                return "";
            try
            {
                return ConvertToDataStr(Double.Parse(doubledate));
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// 时间转化
        /// </summary>
        /// <param name="date">Unix时间戳</param>
        /// <returns></returns>
        public static string ConvertToDataStr(double date)
        {
            return ConvertToData(date).ToString("yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// 时间转化
        /// </summary>
        /// <param name="date">Unix时间戳</param>
        /// <returns></returns>
        public static string ConvertToDataStr(object dateDouble)
        {
            if (dateDouble == null || dateDouble.ToString() == "")
            {
                return "";
            }
            return ConvertToData(Convert.ToDouble(dateDouble)).ToString("yyyy-MM-dd HH:mm:ss");

        }
        /// <summary>
        /// 时间转化
        /// </summary>
        /// <param name="dataStr"></param>
        /// <returns></returns>
        public static double ConvertToUnixTime(string dataStr)
        {
            DateTime dtNow = DateTime.Parse(dataStr);
            return ConvertToUnixTime(dtNow);
        }
        /// <summary>
        /// 时间转化
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static double ConvertToUnixTime(DateTime date)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            TimeSpan toNow = date.Subtract(dtStart);
            return toNow.TotalSeconds;
        }

        /// <summary>
        /// 时间转化yyyy-MM-dd
        /// </summary>
        /// <param name="date">时间</param>
        /// <returns></returns>
        public static string ConvertToShortDataStr(string dateStr)
        {
            DateTime date = DateTime.Parse(dateStr);
            return date.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 时间转化：HH:mm:ss
        /// </summary>
        /// <param name="date">时间</param>
        /// <returns></returns>
        public static string ConvertToShortTimeStr(string dateStr)
        {
            DateTime date = DateTime.Parse(dateStr);
            return date.ToString("HH:mm:ss");
        }

        /// <summary>
        /// 时间转化：yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="date">时间</param>
        /// <returns></returns>
        public static string ConvertToDateTimeStr(string dateStr)
        {
            DateTime date = DateTime.Parse(dateStr);
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public static string ToDateStr(DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }
        public static bool IsDateTime(string strDate)
        {
            try
            {
                DateTime.Parse(strDate);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static DateTime ToDateTime(string strDateTime)
        {
            try
            {
                return DateTime.Parse(strDateTime);
            }
            catch
            {
                return DateTime.Now;
            }
        }
        /// <summary>
        /// 从秒数转换为时间字符串
        /// </summary>
        /// <param name="Second">秒数</param>
        /// <returns>时间字符串</returns>
        public static string ToTimeStrFromSecond(int Second)
        {
            //=========== 1. 得到小时、分钟和秒数 ===========
            string sTimeStr = "";

            int NewSecond = 0;
            int hour = Math.DivRem(Second, 3600, out NewSecond);    //小时

            Second = NewSecond;
            NewSecond = 0;
            int minute = Math.DivRem(Second, 60, out NewSecond);    //分钟

            //============ 2. 得到返回的字符串 ============
            if (hour < 10)
                sTimeStr = sTimeStr + "0" + hour.ToString() + ":";
            else
                sTimeStr = sTimeStr + hour.ToString() + ":";

            if (minute < 10)
                sTimeStr = sTimeStr + "0" + minute.ToString() + ":";
            else
                sTimeStr = sTimeStr + minute.ToString() + ":";

            if (NewSecond < 10)
                sTimeStr = sTimeStr + "0" + NewSecond.ToString();
            else
                sTimeStr = sTimeStr + NewSecond.ToString();

            return sTimeStr;
        }
        public static string AddDays(string dtString, int offset)
        {
            return DateTime.Parse(dtString).AddDays((double)offset).ToString("yyyy-MM-dd HH:mm:ss");
        }
        public static int SecondsAfter(string dtFromString, string dtToString)
        {
            DateTime dateTime = DateTime.Parse(dtFromString);
            return (int)(DateTime.Parse(dtToString) - dateTime).TotalSeconds;
        }
    }
}