using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SPOC.Common.Helper
{
    /// <summary>
    /// 处理各个类型之间的转换
    /// </summary>
    public class ConvertUtil
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        private ConvertUtil()
        {
        }

        #region NotNullStr

        /// <summary>
        /// 返回非空(null)字符串
        /// </summary>
        /// <param name="canNullStr">待转换的对象</param>
        /// <param name="defaultStr">缺省字符串</param>
        /// <returns>得到字符串</returns>
        public static string NotNullStr(object canNullStr, string defaultStr)
        {
            if (canNullStr == null)
            {
                if (defaultStr != null)
                {
                    return defaultStr;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return Convert.ToString(canNullStr);
            }
        }

        /// <summary>
        /// 返回非空(null)字符串
        /// </summary>
        /// <param name="canNullStr">待转换的对象</param>
        /// <returns>得到的字符串。如果为对象null，则返回""</returns>
        public static string NotNullStr(object canNullStr)
        {
            return NotNullStr(canNullStr, "");
        }

        #endregion NotNullStr


        #region ToInt

        /// <summary>
        /// 将对象转换为整型
        /// </summary>
        /// <param name="objInt">对象</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns>得到的整数</returns>
        public static int ToInt(object objInt, int defaultValue)
        {
            if (objInt == null)
            {
                return defaultValue;
            }
            else
            {
                try
                {
                    return Convert.ToInt32(Convert.ToDecimal(objInt));
                }
                catch
                {
                    return defaultValue;
                }
            }
        }

        /// <summary>
        /// 将对象转换为整型
        /// </summary>
        /// <param name="objInt">对象</param>
        /// <returns>得到的整数。缺省为0</returns>
        public static int ToInt(object objInt)
        {
            return ToInt(objInt, 0);
        }

        #endregion ToInt

        #region ToBool
        /// <summary>
        /// 将对象转换为布尔型
        /// </summary>
        /// <param name="objBool">待转换的对象</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns>得到的布尔型</returns>
        public static bool ToBool(object objBool, bool defaultValue)
        {
            if (objBool == null || objBool == DBNull.Value)
            {
                return defaultValue;
            }
            else
            {
                try
                {
                    return Convert.ToBoolean(objBool);
                }
                catch
                {
                    return defaultValue;
                }
            }
        }

        /// <summary>
        /// 将对象转换为布尔型
        /// </summary>
        /// <param name="objBool">待转换的对象</param>
        /// <returns>得到的布尔型。缺省为false。</returns>
        public static bool ToBool(object objBool)
        {
            return ToBool(objBool, false);
        }
        #endregion

        #region ToDecimal

        /// <summary>
        /// 将对象转换为Decimal类型
        /// </summary>
        /// <param name="objDecimal">待转换的对象</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns>得到的Decimal类型</returns>
        public static decimal ToDecimal(object objDecimal, decimal defaultValue)
        {
            if (objDecimal == null)
            {
                return defaultValue;
            }
            else
            {
                try
                {
                    return Convert.ToDecimal(objDecimal);
                }
                catch
                {
                    return defaultValue;
                }
            }
        }

        /// <summary>
        /// 将对象转换为Decimal类型
        /// </summary>
        /// <param name="objDecimal">待转换的对象</param>
        /// <returns>得到的Decimal类型。缺省为0</returns>
        public static decimal ToDecimal(object objDecimal)
        {
            return ToDecimal(objDecimal, 0);
        }

        #endregion ToDecimal

        #region ToFloat

        /// <summary>
        /// 将对象转换为Float类型
        /// </summary>
        /// <param name="objFloat">待转换的对象</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns>得到的Float类型</returns>
        public static double ToFloat(object objFloat, double defaultValue)
        {
            if (objFloat == null)
            {
                return defaultValue;
            }
            else
            {
                try
                {
                    return Convert.ToDouble(objFloat);
                }
                catch
                {
                    return defaultValue;
                }
            }
        }

        /// <summary>
        /// 将对象转换为Float类型
        /// </summary>
        /// <param name="objFloat">待转换的对象</param>
        /// <returns>得到的Float类型。缺省为0</returns>
        public static double ToFloat(object objFloat)
        {
            return ToFloat(objFloat, 0);
        }

        #endregion ToFloat

        #region ToString

        /// <summary>
        /// 将对象转换为字符串类型
        /// </summary>
        /// <param name="objString">待转换为对象</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns>得到的字符串类型</returns>
        public static string ToString(object objString, string defaultValue)
        {
            if (objString == null || objString == DBNull.Value)
            {
                return defaultValue;
            }
            else
            {
                try
                {
                    return Convert.ToString(objString);
                }
                catch
                {
                    return defaultValue;
                }
            }
        }

        /// <summary>
        /// 把字符串数组转换为<see cref="ArrayList">ArrayList</see>
        /// </summary>
        /// <param name="arrStr">字符串数组</param>
        /// <returns><see cref="ArrayList">ArrayList</see>对像</returns>
        public static ArrayList ToArrayList(string[] arrStr)
        {
            ArrayList list = new ArrayList();
            for (int i = 0; i < arrStr.Length; i++)
            {
                list.Add(arrStr[i]);
            }
            return list;
        }

        /// <summary>
        /// 将对象转换为字符串类型
        /// </summary>
        /// <param name="objString"></param>
        /// <returns></returns>
        public static string ToString(object objString)
        {
            return ToString(objString, "");
        }

        #endregion

        #region ToLong

        /// <summary>
        /// 将对象转换为long类型
        /// </summary>
        /// <param name="objLong">待转换的对象</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns>得到的long类型。缺省为0</returns>
        public static long ToLong(object objLong, long defaultValue = 0)
        {
            if (objLong == null)
            {
                return defaultValue;
            }
            else
            {
                try
                {
                    return Convert.ToInt64(objLong);
                }
                catch
                {
                    return defaultValue;
                }
            }
        }

        #endregion
        /// <summary>
        /// key-value对转为Model实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static T ConvertDic<T>(Dictionary<string, object> dic)
        {
            T model = Activator.CreateInstance<T>();
            PropertyInfo[] modelPro = model.GetType().GetProperties();
            if (modelPro.Length > 0 && dic.Any())
            {
                foreach (PropertyInfo t in modelPro)
                {
                    if (dic.ContainsKey(t.Name))
                    {
                        t.SetValue(model, dic[t.Name], null);
                    }
                }
            }
            return model;
        }

        /// <summary>
        /// 将对象属性转换为key-value对
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ToMap(Object o)
        {
            var map = new Dictionary<string, object>();

            Type t = o.GetType();

            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo p in pi)
            {
                MethodInfo mi = p.GetGetMethod();

                if (mi != null && mi.IsPublic)
                {
                    map.Add(p.Name, mi.Invoke(o, new Object[] { }));
                }
            }

            return map;

        }
        public static Dictionary<string, string> ToMapValueOfString(Object o)
        {
            var map = new Dictionary<string, string>();

            Type t = o.GetType();

            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo p in pi)
            {
                MethodInfo mi = p.GetGetMethod();

                if (mi != null && mi.IsPublic)
                {
                    map.Add(p.Name, mi.Invoke(o, new Object[] { }) == null ? "" : mi.Invoke(o, new Object[] { }).ToString());
                }
            }

            return map;

        }

        

    }
}