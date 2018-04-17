using System.Linq;

namespace SPOC.SqlExecuter
{
    public interface ISqlExecuter
    {

        /// <summary>
        /// 执行给定的命令
        /// </summary>
        /// <param name="sql">命令字符串</param>
        /// <param name="parameters">要应用于命令字符串的参数</param>
        /// <returns>执行命令后由数据库返回的结果</returns>
        int Execute(string sql, params object[] parameters);

        /// <summary>
        /// 创建一个原始 SQL 查询，该查询将返回给定泛型类型的元素。
        /// </summary>
        /// <typeparam name="T">查询所返回对象的类型</typeparam>
        /// <param name="sql">查询字符串</param>
        /// <param name="parameters">要应用于 SQL 查询字符串的参数></param>
        /// <returns>执行命令后由数据库返回的结果</returns>
        IQueryable<T> SqlQuery<T>(string sql, params object[] parameters);

        /// <summary>
        /// 执行存储过程数据ID获取数据所在行数
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="lid"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        int GetRowNumById(string tableName, string lid, string orderby);
    }
}
