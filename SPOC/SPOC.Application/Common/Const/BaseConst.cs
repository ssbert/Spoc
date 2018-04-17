namespace SPOC.Common.Const
{
    public class BaseConst<T>
    {
        /// <summary>
        /// 是否拥有该常量值
        /// </summary>
        /// <typeparam name="TK"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasConstValue<TK>(TK value)
        {
            var type = typeof(T);
            var props = type.GetFields();
            foreach (var propertyInfo in props)
            {
                if (propertyInfo.GetValue(null).Equals(value))
                {
                    return true;
                }
            }
            return false;
        }
    }
}