using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SPOC.Web.Models
{
    /// <summary>
    /// Login 视图展示Model
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// 跳转Url
        /// </summary>
        public string SkipUrl { get; set; }

        /// <summary>
        /// 加密key
        /// </summary>
        public string RsaKey { get; set; }

        /// <summary>
        /// ModulusKey
        /// </summary>
        public string ModulusKey { get; set; }

        /// <summary>
        /// 是否显示注册
        /// </summary>
        public bool RegisterDisplay { get; set; }

        /// <summary>
        /// 班级
        /// </summary>
        //[Required(ErrorMessage = "请选择班级")]
        public string Class { get; set; }
        /// <summary>
        /// 班级列表
        /// </summary>
        public IEnumerable<SelectListItem> ClassListItems { get; set; }
    }
}