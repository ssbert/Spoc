using System.Linq;
using Abp.Dependency;
using Abp.EntityFramework;
using MySql.Data.MySqlClient;
using SPOC.SqlExecuter;

namespace SPOC.EntityFramework
{
    public class SqlExecuter : ISqlExecuter, ITransientDependency
    {
        private readonly IDbContextProvider<SPOCDbContext> _dbContextProvider;

        public SqlExecuter(IDbContextProvider<SPOCDbContext> dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        /// <summary>       
        ///  执行给定的命令      
        /// </summary>      
        /// <param name="sql">命令字符串</param>    
        /// <param name="parameters">要应用于命令字符串的参数</param>     
        /// <returns>执行命令后由数据库返回的结果</returns>     
        public int Execute(string sql, params object[] parameters)
        {
         
            return _dbContextProvider.GetDbContext().Database.ExecuteSqlCommand(sql, parameters);
        }

        /// <summary>       
        /// 创建一个原始 SQL 查询，该查询将返回给定泛型类型的元素。    
        /// </summary>        /// <typeparam name="T">查询所返回对象的类型</typeparam>     
        /// <param name="sql">SQL 查询字符串</param>       
        /// <param name="parameters">要应用于 SQL 查询字符串的参数</param>     
        /// <returns></returns>     
        public IQueryable<T> SqlQuery<T>(string sql, params object[] parameters)
        {
            return _dbContextProvider.GetDbContext().Database.SqlQuery<T>(sql, parameters).AsQueryable<T>();

        }
        /// <summary>
        /// 获取指定ID在指定表中所在记录的位置 用于移动端根据最后记录ID分页过滤
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="lid"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public int GetRowNumById(string tableName,string lid,string orderby)
        {
            #region MySqlParameter参数
            var paras = new MySqlParameter[3];
            paras[0] = new MySqlParameter("?table_name", MySqlDbType.String) { Value = tableName };
            paras[1] = new MySqlParameter("?rowid", MySqlDbType.String) { Value = lid };
            paras[2] = new MySqlParameter("?orderby", MySqlDbType.String)
            {
                Value = orderby
            };
            #endregion
            var result = SqlQuery<int>(
                   "call proc_get_rownum(?table_name,?rowid,?orderby)",
                   paras).ToList().FirstOrDefault();
            return result;
        }

    }
}