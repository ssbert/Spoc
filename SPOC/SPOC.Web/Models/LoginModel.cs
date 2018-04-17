using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SPOC.Web.Models
{
    /// <summary>
    /// 提交登录信息Model
    /// </summary>
    public class LoginModel
    {
        [Required]
        public string UserLoginName { get; set; }
        [Required]
        public string UserPassWord { get; set; }
        [DefaultValue(false)]
        public bool RememberMe { get; set; }
    }
}