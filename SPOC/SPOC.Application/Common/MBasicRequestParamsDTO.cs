using System;
using System.ComponentModel.DataAnnotations;

namespace SPOC.Common
{
    /// <summary>
    /// 基础请求 参数
    /// </summary>
    [Serializable]
    public class MBasicRequestParamsDTO
    {
        private int _pageIndex = 1;
        public int pIndex
        {
            get { return _pageIndex; }
            set { this._pageIndex = value; }
        }

        private int _pageSize = 10;
        public int pSize
        {
            get { return _pageSize; }
            set { this._pageSize = value; }
        }
        /// <summary>
        /// 最后数据ID
        /// </summary>
        public string lastId
        {
            get;
            set;
        }
        /// <summary>
        /// 时间戳
        /// </summary>
        [Required]
        public string timeStamp { get; set; }

        /// <summary>
        /// 随机数
        /// </summary>
        [Required]
        public string nonce { get; set; }


        private string _iver = "1.0";
        /// <summary>
        /// 接口版本号
        /// </summary>
        public string iver { get { return _iver; } set { this._iver = value; } }

        /// <summary>
        /// 平台标识1,2 Android 3,4 IOS
        /// </summary>
        [Required]
        public string sPlat { get; set; }

        /// <summary>
        /// 签名串
        /// </summary>
        [Required]
        public string sign { get; set; }
    }
}
