using System;
using System.Collections;
using System.Web;
using System.Web.Caching;

namespace SPOC.Common.Tools
{
    /// <summary>
    /// 基于Asp.Net内置缓存的缓存策略
    /// </summary>
    public static class CacheStrategy// : ICacheStrategy
    {
        private static Cache _cache;

        static CacheStrategy()
        {
            _cache = HttpRuntime.Cache;
        }

        /// <summary>
        /// 获得指定键的缓存值
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>缓存值</returns>
        public static object Get(string key)
        {
            return _cache.Get(key);
        }

        /// <summary>
        /// 将指定键的对象添加到缓存中
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存值</param>
        public static void Insert(string key, object data, bool tokenStore = false)
        {
            _cache.Insert(key, data, null, tokenStore ? DateTime.Now.AddDays(7) : DateTime.Now.AddSeconds(_timeout), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
        }

        /// <summary>
        /// 将指定键的对象添加到缓存中，并指定过期时间
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存值</param>
        /// <param name="cacheTime">缓存过期时间</param>
        public static void Insert(string key, object data, int cacheTime)
        {
            _cache.Insert(key, data, null, DateTime.Now.AddSeconds(cacheTime), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
        }

        /// <summary>
        /// 从缓存中移除指定键的缓存值
        /// </summary>
        /// <param name="key">缓存键</param>
        public static void Remove(string key)
        {
            _cache.Remove(key);
        }

        /// <summary>
        /// 清空所有缓存对象
        /// </summary>
        public static void Clear()
        {
            IDictionaryEnumerator cacheEnum = _cache.GetEnumerator();
            while (cacheEnum.MoveNext())
                _cache.Remove(cacheEnum.Key.ToString());
        }

        /// <summary>
        /// 刷新过期时间
        /// </summary>
        public static void RefreshExpiredTime(string key, object value)
        {
            _cache.Remove(key);
            Insert(key, value, true);
        }


        private static int _timeout = 3600;//单位秒

        /// <summary>
        /// 缓存过期时间
        /// </summary>
        /// <value></value>
        public static int TimeOut
        {
            get
            {
                return _timeout;
            }
            set
            {
                _timeout = value > 0 ? value : 3600;
            }
        }
        /// <summary>
        /// 清除给app的所有缓存
        /// </summary>
        public static void ClearMobileCache()
        {
            IDictionaryEnumerator mCacheEnum = _cache.GetEnumerator();
            while (mCacheEnum.MoveNext())
            {
                string[] mKeys = MoocMobileCacheKeys.MValues;
                for (int i = 0; i < mKeys.Length; i++)
                {
                    if (mCacheEnum.Key.ToString().Contains(mKeys[i]))
                    {
                        _cache.Remove(mCacheEnum.Key.ToString());
                    }
                }
            }
        }


    }

    /// <summary>
    /// 统一管理为学缓存键
    /// </summary>
    public class MoocMobileCacheKeys
    {
        public readonly static string[] MValues = { "MobileCourseCategory", "MobileExamPaper", "MobileExamList", "MobileNewsList", "MobileCourseLesson", "MobileCourseSort" }; //注意添加缓存键的时候相应在数组中添加其value

        /// <summary>
        /// 课程分类缓存键
        /// </summary>
        public const string MCOURSE_CATEGORY = "MobileCourseCategory";

        /// <summary>
        /// 考试试卷缓存件
        /// </summary>
        public const string M_ExamPaper = "MobileExamPaper";
        /// <summary>
        /// 考试列表缓存键
        /// </summary>
        public const string M_ExamList = "MobileExamList";
        /// <summary>
        /// 新闻列表(为了让用户在使用app查看新闻详情后切回列表的时候短暂减少对数据的操作)
        /// </summary>
        public const string M_NewsList = "MobileNewsList";

        /// <summary>
        /// 课时列表
        /// </summary>
        public const string M_CourseLesson = "MobileCourseLesson";
        /// <summary>
        /// 课时内容,用户获取考试相关数据
        /// </summary>
        // public const string M_CourseLessonInfo = "MobileCourseLessonInfo";
        public const string M_CourseSort = "MobileCourseSort";
    }
}
