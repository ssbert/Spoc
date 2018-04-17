using System;
using System.Reflection;

namespace SPOC.Common.Helper
{
   public  class StringHelper
    {
        /// <summary>  
        /// 去除类中字符串属性中的多余空格  
        /// </summary>  
        /// <param name="obj"></param>  
        /// <returns></returns>  
       public static T TrimStr<T>(T obj) where T : class,new()
        {
            try {
               
            Type t = obj.GetType();
            PropertyInfo[] properties = t.GetProperties();
            foreach (PropertyInfo p in properties)
            {
                 
                if (p.PropertyType.Name == "String")//字符串属性  
                {
                    //获取值  
                    string str = (string)p.GetValue(obj);
                    //重新赋值  
                    if (!string.IsNullOrEmpty(str))
                    {
                        if (p.SetMethod != null)
                        {
                            p.SetValue(obj, str.Trim(), null);
                        }
                    }
                }
            }

            return obj;
           }catch(Exception e){
               Abp.Logging.LogHelper.Logger.Error("字符串去空格出错！", e);
               return null;
           }

        }   
    }
}
