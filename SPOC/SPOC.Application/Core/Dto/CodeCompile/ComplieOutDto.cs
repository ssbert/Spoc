using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPOC.Core.Dto.CodeCompile
{
    /// <summary>
    /// 编译服务器返回对象
    /// </summary>
    [Serializable]
    public class ComplieOutDto<T>
    {
        /// <summary>
        /// 返回码
        /// 0：正常
        /// 1：服务启动失败
        /// 2：有错误
        /// 3：超时
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 返回结果
        /// </summary>
        public T Result { get; set; }

        public ComplieOutDto()
        {
            Code = 1;
            Msg = "";
        }

    }
}
