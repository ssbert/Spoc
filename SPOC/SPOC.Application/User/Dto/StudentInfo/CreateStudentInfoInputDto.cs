using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace SPOC.User.Dto.StudentInfo
{
    [AutoMapFrom(typeof(StudentInfoDto))]
    public class CreateStudentInfoInputDto : EntityDto<Guid>
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid user_id { get; set; }
        /// <summary>
        /// 代理商用户ID
        /// </summary>
        public Guid agent_user_id { get; set; }
        /// <summary>
        ///学生用户名
        /// </summary>
        public string user_login_name { get; set; }

        /// <summary>
        /// 学号
        /// </summary>
        public string user_code { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string user_mobile { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string user_email { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string user_name { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string user_nickname { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        public string user_national { get; set; }

        /// <summary>
        /// 政治面貌
        /// </summary>
        public string user_political { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string user_password { get; set; }

        /// <summary>
        /// 所在省份(籍贯)
        /// </summary>
        public string user_province { get; set; }

        /// <summary>
        /// 所在城市(来源地区)
        /// </summary>
        public string user_city { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? user_birthday { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string user_gender { get; set; }



        /// <summary>
        /// 院系 |部门id
        /// </summary>
        public string user_facultyid { get; set; }

        /// <summary>
        /// 院系 |部门
        /// </summary>
        public string user_faculty { get; set; }

        /// <summary>
        ///专业ID 
        /// </summary>
        public string user_majorid { get; set; }

        /// <summary>
        /// 专业
        /// </summary>
        public string user_specialty { get; set; }

        /// <summary>
        /// 教学班级ID
        /// </summary>
        public string user_classid { get; set; }

        /// <summary>
        /// 行政班级ID
        /// </summary>
        public string administrative_classid { get; set; }
        /// <summary>
        /// 班级名称
        /// </summary>
        public string user_class { get; set; }


        /// <summary>
        /// 学生形式
        /// </summary>
        public string study_form { get; set; }

        /// <summary>
        /// 学习标记
        /// </summary>
        public string study_flag { get; set; }

        /// <summary>
        /// 层次
        /// </summary>
        public string level { get; set; }

        /// <summary>
        ///学制
        /// </summary>
        public string user_eductional { get; set; }

        /// <summary>
        /// 入学时间
        /// </summary>
        public DateTime? user_admission { get; set; }

        /// <summary>
        /// 年级
        /// </summary>
        public string user_grade { get; set; }

        /// <summary>
        /// 宿舍
        /// </summary>
        public string user_dormitory { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string user_idcard { get; set; }

        /// <summary>
        ///邮编 
        /// </summary>
        public string user_zipcode { get; set; }

        /// <summary>
        /// 学生邀请码
        /// </summary>
        public string user_inviteCode { get; set; }

        /// <summary>
        ///  注册邀请码
        /// </summary>
        public string user_register_inviteCode { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public int user_enble_flag { get; set; }

        /// <summary>
        /// 是否毕业
        /// </summary>
        public bool is_graduation { get; set; }

        /// <summary>
        /// 毕业时间
        /// </summary>
        public DateTime? graduation_date { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_time { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime updateTime { get; set; }


        /// <summary>
        /// 是否删除
        /// </summary>
        public bool isDel { get; set; }

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

     
        public User.StudentInfo GetStudentInfo()
        {
            try
            {
                return new User.StudentInfo()
                {
                    Id = this.Id,
                    userId = this.user_id,
                    userCode = this.user_code,
                    userNickname = this.user_nickname,

                    userProvince = this.user_province,
                    userCity = this.user_city,
                    facultyId = 
                        string.IsNullOrEmpty(this.user_facultyid) ? Guid.Empty : Guid.Parse(this.user_facultyid),
                  
                    majorId = 
                        string.IsNullOrEmpty(this.user_majorid) ? Guid.Empty : Guid.Parse(this.user_majorid),
                  
                    classId = string.IsNullOrEmpty(this.administrative_classid) ? Guid.Empty : Guid.Parse(this.administrative_classid),
                   
                    studyForm = this.study_form,
                    studyFlag = this.study_flag,
                    level = this.level,
                    userEductional = this.user_eductional,
                    userAdmission = this.user_admission,
                    userGrade = this.user_grade,
                    userDormitory = this.user_dormitory,

                    userZipcode = this.user_zipcode,
                    userInviteCode = this.user_inviteCode,
                    userRegisterInviteCode = this.user_register_inviteCode,
                    userEnbleFlag = this.user_enble_flag,
                    isGraduation = this.is_graduation,
                    graduationDate = this.graduation_date,
                    createTime = this.create_time,
                    updateTime = this.updateTime

                    ,
                    isDel = this.isDel

                };
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public UserBase GetUser() {

            return new UserBase()
            {
                Id = this.user_id,
                userLoginName = this.user_login_name,
                userMobile = this.user_mobile,
                userEmail = this.user_email,
                userFullName = this.user_name,
                userGender = this.user_gender,
                userPassWord = this.user_password,
                userNational = this.user_national,
                userPolitical = this.user_political,
                userBirthday = this.user_birthday,
           //     userBirthday = this.user_birthday,
                userIdcard = this.user_idcard,
                identity=1
                 ,
                about = this.about,
                largeAvatar = this.largeAvatar,
                mediumAvatar = this.mediumAvatar,
                smallAvatar = this.smallAvatar,
                 approvalStatus= "approved",
                 signature=this.signature
                  
            };
        }
 
    }

      
}
