using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using SPOC.EntityFramework;

namespace SmartUFO.EntityFramework
{
    public class ContextFactory
    {
        /// <summary>
        /// 获取当前数据上下文
        /// </summary>
        /// <returns></returns>
        public static SPOCDbContext GetCurrentContext()
        {
            SPOCDbContext db = CallContext.GetData("Default") as SPOCDbContext;
            if (db == null)
            {
                db = new SPOCDbContext();
                CallContext.SetData("Default", db);
            }
            return db;
        }
    }
}
