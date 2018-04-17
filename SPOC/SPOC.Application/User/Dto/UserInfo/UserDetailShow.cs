using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace SPOC.User.Dto.UserInfo
{
   public  class UserDetailShow:Entity<Guid>
    {
       private string userName = "";
       private string title = "";

       private string about = "";

       private string signature = "";

       private int identity = 1;

       private bool isAdmin = false;

       private bool isTeacher = false;

       private bool isMyself = true;

       private string hederImg = "";

       private string weiboIsOpen = "";

       private string weibo = "";

       private string weChat = "";

       private string weChatIsOpen = "";

       private string qqNumber = "";

       private string qqIsOpen = "";
 

       public string HederImg { get { return hederImg; } set { hederImg = value; } }

       /// <summary>
       /// 是否为当前本人
       /// </summary>
       public bool IsMyself { get { return isMyself; } set { isMyself = value; } }


       private bool isConcern = false;



       /// <summary>
       /// 是否已关注该用户
       /// </summary>
       public bool IsConcern { get { return isConcern; } set { isConcern = value; } }
       /// <summary>
       /// 登录名
       /// </summary>
       public string UserLoginName { get; set; }
       public string UserName { get { return userName; } set { userName = value; } }
       public string Title { get { return title; } set { title = value; } }

       public string Signature { get { return signature; } set { signature = value; } }
       public string About { get { return about; } set { about = value; } }
       public int Identity { get { return identity; } set { identity = value; } }

       public bool IsAdmin { get { return isAdmin; } set { isAdmin = value; } }

       public bool IsTeacher { get { return isTeacher; } set { isTeacher = value; } }


       public string Weibo { get { return weibo; } set { weibo = value; } }

       public string WeiboIsOpen { get { return weiboIsOpen; } set { weiboIsOpen = value; } }

       public string WeChat { get { return weChat; } set { weChat = value; } }

       public string WeChatIsOpen { get { return weChatIsOpen; } set { weChatIsOpen = value; } }

       public string QQNumber { get { return qqNumber; } set { qqNumber = value; } }

       public string QQIsOpen { get { return qqIsOpen; } set { qqIsOpen = value; } }

       private string  personalPage="";
       public string PersonalPage { get { return personalPage; } set { personalPage = value; } }

       /// <summary>
       /// 粉丝数
       /// </summary>
       private int fansCount = 0;
       public int FansCount { get { return fansCount; } set { fansCount = value; } }

       private int followCount = 0;
       /// <summary>
       /// 关注的总人数
       /// </summary>
       public int FollowCount { get { return followCount; } set { followCount = value; } }
     
       /// <summary>
       /// 关注
       /// </summary>
       private List<UserInfoCard> _followingList = new List<UserInfoCard>();
       public List<UserInfoCard> FollowingList
       {
           get { return _followingList; }
           set { _followingList = value; }
       }

       /// <summary>
       /// 粉丝
       /// </summary>
       private List<UserInfoCard> _fansList = new List<UserInfoCard>();
       public List<UserInfoCard> FansList
       {
           get { return _fansList; }
           set { _fansList = value; }
       }

       private PageParameter pageParameter = new PageParameter();
       public PageParameter PageParameter {
           get { return pageParameter; }
           set { pageParameter = value; }

       }

       /// <summary>
       /// 个人简历
       /// </summary>
       private string _teacherPersonalResume = string.Empty;
       public string teacherPersonalResume {
           get { return _teacherPersonalResume; }
           set { _teacherPersonalResume = value; }
       }

    }

   public class PageParameter {
        
   
       private int currentPage=0;
       private int pageSize = 20;
       private int dataTotal = 0;
       private int pageIndex = 0;
       private int pageCount = 0;

       public int CurrentPage { get { return currentPage; } set { currentPage = value; } }

       public int PageSize { get { return pageSize; } set { pageSize = value; } }
       public int DataTotal { get { return dataTotal; } set { dataTotal = value; } }
       public int PageIndex { get { return pageIndex; } set { pageIndex = value; } }
       public int PageCount { get { return pageCount; } set { pageCount = value; } }

       public void SetPageCount() {

           this.pageCount = this.dataTotal / pageSize + (this.dataTotal % pageSize > 0 ? 1 : 0);
       }

   }

    /// <summary>
    /// 用户卡片基本信息
    /// </summary>
   public class UserInfoCard
   {
       /// <summary>
       /// 用户的ID
       /// </summary>
       private string _UserId = string.Empty;
       public string UserId
       {
           get { return _UserId; }
           set { _UserId = value; }
       }

       /// <summary>
       /// 用户名
       /// </summary>
       private string _UserName = string.Empty;
       public string UserName
       {
           get { return _UserName; }
           set { _UserName = value; }
       }

       /// <summary>
       ///  头像
       /// </summary>
       private string _UserHeadImg = string.Empty;
       public string UserHeadImg
       {
           get { return _UserHeadImg; }
           set { _UserHeadImg = value; }
       }

       /// <summary>
       /// 头衔
       /// </summary>
       private string _userTitle = string.Empty;
       public string UserTitle
       {
           get { return _userTitle; }
           set { _userTitle = value; }
       }

       /// <summary>
       ///  简介
       /// </summary>
       private string _About = string.Empty;
       public string About
       {
           get { return _About; }
           set { _About = value; }
       }

       /// <summary>
       /// 是否关注
       /// </summary>
       private bool _isFollow = true;
       public bool IsFollow
       {
           get { return _isFollow; }
           set { _isFollow = value; }
       }

       private bool _isMyself = false;
       public bool IsMyself
       {
           get { return _isMyself; }
           set { _isMyself = value; }
       }
   }



    public class UserInfoCardbyApp
    {
        /// <summary>
        /// 用户的ID
        /// </summary>
        private string _friendId = string.Empty;
        public string friendId
        {
            get { return _friendId; }
            set { _friendId = value; }
        }

        /// <summary>
        /// 用户名
        /// </summary>
        private string _name = string.Empty;
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        ///  头像
        /// </summary>
        private string _avatar = string.Empty;
        public string avatar
        {
            get { return _avatar; }
            set { _avatar = value; }
        }



        /// <summary>
        ///  性别
        /// </summary>
        private string _sex = string.Empty;
        public string sex
        {
            get { return _sex; }
            set { _sex = value; }
        }


    }
}
