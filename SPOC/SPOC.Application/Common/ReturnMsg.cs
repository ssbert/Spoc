using System;

namespace SPOC.Common
{
    public class ReturnMsg
    {
        /// <summary>
        /// 0 表示成功 
        /// 10001 表示失败
        /// 10002 记录已存在 
        /// 10003 系统内部异常
        /// 10004 数据验证不通过
        /// </summary>
        private string _MsgCode = string.Empty;
        public string MsgCode
        {
            get { return _MsgCode; }
            set { _MsgCode = value; }
        }

        private string _MsgContent = string.Empty;
        public string MsgContent
        {
            get { return _MsgContent; }
            set { _MsgContent = value; }
        }

        private object _Data = new object();
        public object Data
        {
            get { return _Data; }
            set { _Data = value; }
        }
    }
    /// <summary>
    /// ReturnMsg 状态码
    /// </summary>
    public enum ReturnMsgEnum
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success=0,
        /// <summary>
        /// 失败
        /// </summary>
        Fail = 10001,
        /// <summary>
        /// 记录已存在
        /// </summary>
        Exist = 10002,
        /// <summary>
        /// 系统内部异常
        /// </summary>
        SystemError = 10003,
        /// <summary>
        /// 数据校验不通过
        /// </summary>
        VerifyFail = 10004,
    }

   
}
