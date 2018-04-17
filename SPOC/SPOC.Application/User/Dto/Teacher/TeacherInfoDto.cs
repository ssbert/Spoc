using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using SPOC.User.Dto.UserInfo;

namespace SPOC.User.Dto.Teacher
{
    [AutoMapFrom(typeof(TeacherInfo))]
    public class TeacherInfoDto : EntityDto<Guid>
    {
        public Guid id => Id;
        public Guid user_id { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string user_login_name { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string user_password { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string user_name { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string user_mobile { get; set; }


        /// <summary>
        /// 邮箱
        /// </summary>
        public string user_email { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string user_gender { get; set; }

        /// <summary>
        /// 教师号
        /// </summary>
        public string teacherCode { get; set; }

        /// <summary>
        /// 职称
        /// </summary>
        public string teacherTitle { get; set; }
        /// <summary>
        /// 在职状态码(0:离职 1:在职  2:停职)
        /// </summary>
        public int? teacherJobStatusCode { get; set; }
        /// <summary>
        /// 专业方向
        /// </summary>
        public string teacherProfessionalDirection { get; set; }

        ///  /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime? teacherBirthday { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string teacherIdCode { get; set; }
        /// <summary>
        /// 民族
        /// </summary>
        public string teacherNational { get; set; }
        /// <summary>
        /// 政治面貌
        /// </summary>
        public string teacherPolitical { get; set; }
        /// <summary>
        /// 任职日期
        /// </summary>
        public DateTime? teacherEntryDate { get; set; }
        /// <summary>
        /// 参加工作日期
        /// </summary>
        public DateTime? teacherStartworkDate { get; set; }
        /// <summary>
        /// 高校教龄
        /// </summary>
        public float? teacherEduAge { get; set; }
        /// <summary>
        /// 学历
        /// </summary>
        public string teacherEducation { get; set; }
        /// <summary>
        /// 学位
        /// </summary>
        public string teacherAcademicDegree { get; set; }

        /// <summary>
        /// 毕业学校
        /// </summary>
        public string teacherGraduateSchool { get; set; }

        /// <summary>
        /// 毕业日期
        /// </summary>
        public DateTime? teacherGraduateDate { get; set; }

        /// <summary>
        /// 所学专业
        /// </summary>
        public string teacherStudyProfessional { get; set; }

        /// <summary>
        /// 课程权限 1:有权限 0：没权限
        /// </summary>
        public bool teacherIsAddCourse { get; set; }

        /// <summary>
        /// 邀请码
        /// </summary>
        public string teacherInviteCode { get; set; }


        /// <summary>
        /// 是否是推荐教师
        /// </summary>
        public bool teacherIsRecommend { get; set; }
        /// <summary>
        /// 是否首页显示
        /// </summary>
        public bool teacherIsDisplay { get; set; }
        
        /// <summary>
        /// 个人简历
        /// </summary>
        public string teacherPersonalResume { get; set; }



        /// <summary>
        /// 小头像
        /// </summary>
        public string smallAvatar { get; set; }

        /// <summary>
        /// 中头像
        /// </summary>
        public string mediumAvatar { get; set; }

        /// <summary>
        /// 大头像
        /// </summary>
        public string largeAvatar { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public string about { get; set; }

        public string signature { get; set; }

        /// <summary>
        /// 未读私信条数
        /// </summary>
        public int? newMessageNum { get; set; }


        /// <summary>
        /// 未读消息数目
        /// </summary>
        public int? newNotificationNum { get; set; }

        /// <summary>
        /// 实名认证状态
        /// </summary>
        public string approvalStatus { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool isDel { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool teacherEnbleFlag { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime updateTime { get; set; }
        /// <summary>
        /// 直播推流地址
        /// </summary>
        public string pushUrl { get; set; }


        /// <summary>
        /// 登录ip
        /// </summary>
        public string loginIp { get; set; }

        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime? loginTime { get; set; }
        /// <summary>
        /// 班级列表 多个班级逗号分隔显示在页面上
        /// </summary>
        public string  classNames { get; set; }
        public string  classIds { get; set; }
    }

    public class RecommendTeacherViewModel
    {
        private List<RecommendTeacherObj> _RecommendTeacherList = new List<RecommendTeacherObj>();
        public List<RecommendTeacherObj> RecommendTeacherList
        {
            get { return _RecommendTeacherList; }
            set { _RecommendTeacherList = value; }
        }

        /// <summary>
        /// 关注
        /// </summary>
        private List<UserInfoCard> _followingList = new List<UserInfoCard>();
        public List<UserInfoCard> FollowingList
        {
            get { return _followingList; }
            set { _followingList = value; }
        }

        private string  _uname =string.Empty;
        public string Uname
        {
            get { return _uname; }
            set { _uname = value; }
        }
        private int _CurrentPage = 0;
        public int CurrentPage
        {
            get { return _CurrentPage; }
            set { _CurrentPage = value; }
        }

        private int _SumPage = 0;
        public int SumPage
        {
            get { return _SumPage; }
            set { _SumPage = value; }
        }
    }

    public class RecommendTeacherObj
    {
        
        /// <summary>
        /// 教师真实姓名
        /// </summary>
        private string _userFullName = string.Empty;
        public string userFullName
        {
            get { return _userFullName; }
            set { _userFullName = value; }
        }
        /// <summary>
        /// 用户的ID
        /// </summary>
        private string _UserId = string.Empty;
        public string UserId
        {
            get { return _UserId; }
            set { _UserId = value; }
        }
        private bool _teacherIsDisplay = false;
        public bool teacherIsDisplay
        {
            get { return _teacherIsDisplay; }
            set { _teacherIsDisplay = value; }
        }

        
        /// <summary>
        /// 教师的名称
        /// </summary>
        private string _UserName = string.Empty;
        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }



        /// <summary>
        /// 教师的头像
        /// </summary>
        private string _UserHeadImg = string.Empty;
        public string UserHeadImg
        {
            get { return _UserHeadImg; }
            set { _UserHeadImg = value; }
        }

        /// <summary>
        /// 教师的职称
        /// </summary>
        private string _TeacherTitle = string.Empty;
        public string TeacherTitle
        {
            get { return _TeacherTitle; }
            set { _TeacherTitle = value; }
        }

        /// <summary>
        /// 教师的简介
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
    }

    public class CourseMemberTeacherInfoDto
    {
        private TeacherInfoDto _TeacherInfoDto = new TeacherInfoDto();
        public TeacherInfoDto TeacherInfoDto
        {
            get { return _TeacherInfoDto; }
            set { _TeacherInfoDto = value; }
        }

        /// <summary>
        /// 是否显示
        /// </summary>
        private bool _IsVisible = false;
        public bool IsVisible
        {
            get { return _IsVisible; }
            set { _IsVisible = value; }
        }
    }

    public class TeacherDepartmentItem
    {
        public string departmentId { get; set; }
        public string departmentName { get; set; }
    }

    public static class TeacherInfoDtoExt {
        public static tacher_info_import GetTacherInfoImport(this  TeacherInfoDto model) { 
        var obj= new  tacher_info_import(){
             
          user_login_name=model.user_login_name,
          // teacherTitleCreate=model.teacherTitle,
            teacherGraduateDate=model.teacherStartworkDate.ToString(),
             teacherCode=model.teacherCode,
              user_political=model.teacherPolitical,
               user_password=model.user_password,
                user_national=model.teacherNational,
                 user_mobile=model.user_mobile,
                  user_idcard=model.teacherIdCode,
                  id = model.Id.ToString(),
                   teacherAcademicDegree=model.teacherAcademicDegree,
                    teacherEduAge=model.teacherEduAge.ToString(),
                     teacherEducation=model.teacherEducation,
                      teacherEntryDate=model.teacherEntryDate.ToString(),
                       teacherGraduateSchool=model.teacherGraduateSchool,
                      //  teacherJobStatusCode=model.teacherInviteCode,
                         teacherPersonalResume=model.teacherPersonalResume,
                          teacherProfessionalDirection=model.teacherProfessionalDirection,
                           teacherStartworkDate=model.teacherStartworkDate.ToString(),
                            teacherStudyProfessional=model.teacherStudyProfessional,
                             user_birthday=model.teacherBirthday.ToString(),
                              user_email=model.user_email,
                               user_gender=model.user_gender=="1"?"男":(model.user_gender=="2"?"女":""),
                                user_name=model.user_name
 
            
        };
        if (!string.IsNullOrEmpty(model.teacherTitle))
        {

            switch (model.teacherTitle)
            {
                case "1": { obj.teacherTitleCreate = "教授"; } break;
                case "2": { obj.teacherTitleCreate = "副教授"; } break;
                case "3": { obj.teacherTitleCreate = "讲师"; } break;
                case "4": { obj.teacherTitleCreate = "助教"; } break;
            }
        }
        if (model.teacherJobStatusCode.HasValue)
        {
            switch (model.teacherJobStatusCode)
            {
                case 1: { obj.teacherJobStatusCode = "在职"; } break;
                case 2: { obj.teacherJobStatusCode = "停职"; } break;
                case 3: { obj.teacherJobStatusCode = "离职"; } break;
            }
        }
        return obj;
        }
    }
}
