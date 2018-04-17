using System;
using Abp.Application.Services.Dto;

namespace SPOC.User.Dto.Teacher
{
    public class CreateTeacherInfoInputDto : EntityDto<Guid>
    {
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
        public string teacherTitleCreate { get; set; }
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
        /// 部门(组织架构Id)
        /// </summary>
        public string departmentId { get; set; }
        public string classId { get; set; }
        public TeacherInfo GetTeacherInfo()
        {

            return new TeacherInfo()
            {
                Id = this.Id,
                userId = this.user_id,
                teacherCode = this.teacherCode,
                teacherTitle = this.teacherTitleCreate,
                teacherJobStatusCode = this.teacherJobStatusCode,
                teacherProfessionalDirection = this.teacherProfessionalDirection,

                teacherEntryDate = this.teacherEntryDate,
                teacherStartworkDate = this.teacherStartworkDate,
                teacherEduAge = this.teacherEduAge,
                teacherEducation = this.teacherEducation,
                teacherAcademicDegree = this.teacherAcademicDegree,
                teacherGraduateSchool = this.teacherGraduateSchool,
                teacherGraduateDate = this.teacherGraduateDate,
                teacherStudyProfessional = this.teacherStudyProfessional,
                teacherIsAddCourse = this.teacherIsAddCourse,
                teacherInviteCode = this.teacherInviteCode,
                teacherIsRecommend = this.teacherIsRecommend,
                teacherPersonalResume = this.teacherPersonalResume,
                isDel = this.isDel,
                teacherEnbleFlag = this.teacherEnbleFlag,
                createTime = this.createTime,
                updateTime = this.createTime
                //  ,  teacherDescribe = this.teacherDescribe,
                //  teacherHeadImg = this.teacherDescribe
            };
        }

        public UserBase GetUser()
        {
            return new UserBase()
            {
                Id = this.user_id,
                userLoginName = this.user_login_name,
                userMobile = this.user_mobile,
                userEmail = this.user_email,
                userFullName = this.user_name,
                userGender = this.user_gender,
                userPassWord = this.user_password,
                userBirthday = this.teacherBirthday,
                userIdcard = this.teacherIdCode,
                userNational = this.teacherNational,
                userPolitical = this.teacherPolitical,
                identity = 2
                ,
                about = this.about,
                largeAvatar = this.largeAvatar,
                mediumAvatar = this.mediumAvatar,
                smallAvatar = this.smallAvatar,
                 approvalStatus=""
                 , signature=this.signature

            };
        }
    }
}
