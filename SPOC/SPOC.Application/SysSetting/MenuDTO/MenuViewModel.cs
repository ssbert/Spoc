using System.Collections.Generic;
using SPOC.User;
using SPOC.User.Dto.UserInfo;

namespace SPOC.SysSetting.MenuDTO
{
    public class MenuViewModel
    {
        private List<MenuList> _MenuList = new List<MenuList>();
        public List<MenuList> MenuList
        {
            get { return _MenuList; }
            set { _MenuList = value; }
        }



        /// <summary>
        /// 用户信息
        /// </summary>
        public UserCookie UserCookie { get; set; }


        

        /// <summary>
        /// 用户信息
        /// </summary>
        private UserBase _UserBase = new UserBase();
        public UserBase UserBase
        {
            get { return _UserBase; }
            set { _UserBase = value; }
        }

        private string _Logo = string.Empty;
        public string Logo
        {
            get { return _Logo; }
            set { _Logo = value; }
        }

        private string _LogoUrl = string.Empty;
        public string LogoUrl
        {
            get { return _LogoUrl; }
            set { _LogoUrl = value; }
        }

        private bool _IsLogin = false;
        public bool IsLogin
        {
            get { return _IsLogin; }
            set { _IsLogin = value; }
        }

        private string _UserName = string.Empty;
        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }

        private string _UserHeadImg = string.Empty;
        public string UserHeadImg
        {
            get { return _UserHeadImg; }
            set { _UserHeadImg = value; }
        }

        private int _ShopNum = 0;
        public int ShopNum
        {
            get { return _ShopNum; }
            set { _ShopNum = value; }
        }
        private int _Notification = 0;
        public int Notification
        {
            get { return _Notification; }
            set { _Notification = value; }
        }
        
        private string userRegisterDisplay = "true";
        public string UserRegisterDisplay 
        {
            get { return string.IsNullOrEmpty(userRegisterDisplay) ? "true" : userRegisterDisplay; }
            set { userRegisterDisplay = value ?? "true"; }
        }
    }

    public class MenuList
    {
        private bool _IsNewOpening = false;
        public bool IsNewOpening
        {
            get { return _IsNewOpening; }
            set { _IsNewOpening = value; }
        }
        private string _MenuName = string.Empty;
        public string MenuName
        {
            get { return _MenuName; }
            set { _MenuName = value; }
        }

        private string _Url = string.Empty;
        public string Url
        {
            get { return _Url; }
            set { _Url = value; }
        }

        private bool _IsFoucus = false;
        public bool IsFoucus
        {
            get { return _IsFoucus; }
            set { _IsFoucus = value; }
        }

        private List<MenuList> _Children = new List<MenuList>();
        public List<MenuList> Children
        {
            get { return _Children; }
            set { _Children = value; }
        }
    }
}
