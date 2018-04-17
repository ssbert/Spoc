using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using AutoMapper;
using NPOI.SS.Formula.Functions;

namespace SPOC.Common.Extensions
{
    public static class AutoMapExtensions
    {
        /// <summary>
        /// Converts an object to another using AutoMapper library. Creates a new object of <see cref="TDestination"/>.
        /// There must be a mapping between objects before calling this method.
        /// </summary>
        /// <typeparam name="TDestination">Type of the destination object</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="createMap">need createMap</param>
        public static TDestination ExMapTo<TDestination>(this object source, bool createMap=true)
        {
            if (source == null) return default(TDestination);
            if (createMap) //默认不创建映射关系
            Mapper.Initialize(x=>x.CreateMap(source.GetType(), typeof(TDestination)));
            return Mapper.Map<TDestination>(source);
        }

        /// <summary>
        /// Execute a mapping from the source object to the existing destination object
        /// There must be a mapping between objects before calling this method.
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <param name="createMap">need createMap</param>
        /// <returns></returns>
        public static TDestination ExMapTo<TSource, TDestination>(this TSource source, TDestination destination,bool createMap=true)
            where TSource : class
            where TDestination : class
        {
            if (source == null) return destination;
            if (createMap) //默认不创建映射关系
                Mapper.Initialize(x => x.CreateMap<TSource, TDestination>());
            return Mapper.Map(source, destination);
        }
        // public static TDestination ExMapTo<TSource, TDestination>(this TSource source, TDestination destination)
        //{
        //    return Mapper.Map(source, destination);
        //}
        ///// <summary>
        /////  类型映射
        ///// </summary>
        //public  static T ExMapTo<T>(this object obj)
        //{

        //    if (obj == null) return default(T);
        //    Mapper.CreateMap(obj.GetType(), typeof(T));
        //    return Mapper.Map<T>(obj);
        //}
        /// <summary>
        /// 集合列表类型映射
        /// </summary>
        public static List<TDestination> ExMapToList<TDestination>(this IEnumerable source)
        {
            foreach (var first in source)
            {
                var type = first.GetType();
                Mapper.Initialize(x => x.CreateMap(type, typeof(TDestination)));
                break;
            }
            return Mapper.Map<List<TDestination>>(source);
        }
        /// <summary>
        /// 集合列表类型映射
        /// </summary>
        public static List<TDestination> ExMapToList<TSource, TDestination>(this IEnumerable<TSource> source)
        {
            //IEnumerable<T> 类型需要创建元素的映射
            Mapper.Initialize(x => x.CreateMap<TSource, TDestination>());
            return Mapper.Map<List<TDestination>>(source);
        }
        /// <summary>
        /// 类型映射
        /// </summary>
       
        /// <summary>
        /// DataReader映射
        /// </summary>
        public static IEnumerable<T> DataReaderMapTo<T>(this IDataReader reader)
        {

            Mapper.Initialize(x => x.CreateMap<IDataReader, IEnumerable<T>>());
            return Mapper.Map<IDataReader, IEnumerable<T>>(reader);
        }

        public static T ToEntity<T>(DataRow adaptedRow) where T : new()
        {
            T item = new T();
            if (adaptedRow == null)
            {
                return item;
            }

            item = Activator.CreateInstance<T>();
            CopyToEntity(item, adaptedRow);

            return item;
        }

        public static void CopyToEntity(object entity, DataRow adaptedRow)
        {
            if (entity == null || adaptedRow == null)
            {
                return;
            }
            PropertyInfo[] propertyInfos = entity.GetType().GetProperties();

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (!CanSetPropertyValue(propertyInfo, adaptedRow))
                {
                    continue;
                }

                try
                {
                    if (adaptedRow[propertyInfo.Name] is DBNull)
                    {
                        propertyInfo.SetValue(entity, null, null);
                        continue;
                    }
                    SetPropertyValue(entity, adaptedRow, propertyInfo);
                }
                finally
                {

                }
            }
        }

        private static bool CanSetPropertyValue(PropertyInfo propertyInfo, DataRow adaptedRow)
        {
            if (!propertyInfo.CanWrite)
            {
                return false;
            }

            if (!adaptedRow.Table.Columns.Contains(propertyInfo.Name))
            {
                return false;
            }

            return true;
        }

        private static void SetPropertyValue(object entity, DataRow adaptedRow, PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType == typeof(DateTime?) ||
                propertyInfo.PropertyType == typeof(DateTime))
            {
                DateTime date = DateTime.MaxValue;
                DateTime.TryParse(adaptedRow[propertyInfo.Name].ToString(),
                    CultureInfo.CurrentCulture, DateTimeStyles.None, out date);

                propertyInfo.SetValue(entity, date, null);
            }
            else
            {
                propertyInfo.SetValue(entity, adaptedRow[propertyInfo.Name], null);
            }
        }
        /// <summary>  
        /// 模型赋值  
        /// </summary>  
        /// <param name="source">数据源</param>  
        public static TDestination CopyModel<TDestination>(this object source )
        {

            var item = Activator.CreateInstance<TDestination>();
            Type destinationType = typeof(TDestination);
            Type sourceType = source.GetType();
            foreach (var mi in sourceType.GetProperties())
            {
                var des = destinationType.GetProperty(mi.Name);
                if (des != null)
                {
                    try
                    {
                        des.SetValue(item, mi.GetValue(source, null), null);
                    }
                    catch
                    { }
                }
            }
            return item;
        }
    }
}
