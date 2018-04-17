using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SPOC.Common.Helper
{
   public  class ObjHelper
    {

       /// <summary>
       /// 对象比较,将T2对象的值赋到T1对象
       /// </summary>
       /// <typeparam name="T1"></typeparam>
       /// <typeparam name="T2"></typeparam>
       /// <param name="obj1"></param>
       /// <param name="obj2"></param>
       public static void ObjSetValue<T1, T2>(ref T1 obj1, ref T2 obj2)
           where T1 : class ,new()
           where T2 : class ,new() 
       {
           Type t = obj1.GetType();
           PropertyInfo[] properties = t.GetProperties();
           PropertyInfo[] properties2 = obj2.GetType().GetProperties();
           PropertyInfo per;
           foreach (PropertyInfo p in properties2)
           { 
              if (p.PropertyType.Name != "Method")//字符串属性  
              {
                 // per = properties.Where(a => a.Name == p.Name).FirstOrDefault();
                  per = obj1.GetType().GetProperty(p.Name)??null;
                  if (per!=null&&per.SetMethod != null) 
                  { 
                      per.SetValue(obj1, p.GetValue(obj2));
                  }
              }
           }
       }

       /// <summary>
       /// 实体映射
       /// </summary>
       /// <typeparam name="T1"></typeparam>
       /// <typeparam name="T2"></typeparam>
       /// <param name="obj1"></param>
       /// <param name="obj2"></param>
       public static void ObjMapper<T1, T2>(ref T1 obj1, ref T2 obj2)
           where T1 : class ,new()
           where T2 : class ,new()
       {
          // obj2 = new T2();
           PropertyInfo[] properties =  obj1.GetType().GetProperties();
           PropertyInfo[] properties2 = obj2.GetType().GetProperties();
           PropertyInfo per;
           foreach (PropertyInfo p in properties)
           {
               if (p.PropertyType.Name != "Method")//字符串属性  
               {
                  // per = properties2.Where(a => a.Name == p.Name).FirstOrDefault();
                   per = obj2.GetType().GetProperty(p.Name) ?? null;
                   if (per != null)
                   {
                       per.SetValue(obj2, p.GetValue(obj1));
                   }
               }
           }
       }

       /// <summary>
       /// 根据属性列表进行值隐射
       /// </summary>
       /// <typeparam name="T1"></typeparam>
       /// <typeparam name="T2"></typeparam>
       /// <param name="obj1"></param>
       /// <param name="obj2"></param>
       /// <param name="perLiset"></param>
       public static void ObjSetValueByPerList<T1, T2>(ref T1 obj1, ref T2 obj2, List<string> perLiset)
           where T1 : class ,new()
           where T2 : class ,new() {
               foreach (var item in perLiset)
               {
                   PropertyInfo p1 = obj1.GetType().GetProperty(item) ?? null;
                   PropertyInfo p2 = obj2.GetType().GetProperty(item) ?? null;
                   if (p1 != null && p2 != null) {
                       p1.SetValue(obj1,p2.GetValue(obj2));
                   }
               }
       }


       
        public static string GetPropertyName<T>(Expression<Func<T, object>> expr)
        {
            var rtn = "";
            if (expr.Body is UnaryExpression)
            {
                rtn = ((MemberExpression)((UnaryExpression)expr.Body).Operand).Member.Name;
            }
            else if (expr.Body is MemberExpression)
            {
                rtn = ((MemberExpression)expr.Body).Member.Name;
            }
            else if (expr.Body is ParameterExpression)
            {
                rtn = ((ParameterExpression)expr.Body).Type.Name;
            }
            return rtn;
        }  
    }
   
}
