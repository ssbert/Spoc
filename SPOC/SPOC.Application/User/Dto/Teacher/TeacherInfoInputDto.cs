using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using SPOC.Common.Dto;
using SPOC.Common.Encrypt;
using SPOC.Common.File;
using SPOC.Common.Pagination;

namespace SPOC.User.Dto.Teacher
{
    public class TeacherInfoInputDto : EasyuiDto
    {

        public TeacherInfoInputDto() {

          //  teacherJobStatusCode = -1;
        }
        /// <summary>
        /// 用户名
        /// </summary>
        public string user_login_name { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string user_mobile { get; set; }

        /// <summary>
        /// 教师号
        /// </summary>
        public string teacherCode { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string user_name { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string user_gender { get; set; }

        /// <summary>
        /// 职称
        /// </summary>
        public string teacherTitle { get; set; }

        /// <summary>
        /// 在职状态码(0:离职 1:在职  2:停职)
        /// </summary>
        public int? teacherJobStatusCode { get; set; }

        /// <summary>
        /// 部门(组织架构Id)
        /// </summary>
        public string department { get; set; }

        /// <summary>
        /// 是否是推荐教师
        /// </summary>
        public bool isRecommend { get; set; }
    }
    /// <summary>
    ///教师导入表
    /// </summary>
    [Serializable]
    [DataContract]
    public class tacher_info_import
    {
        /// <summary>
        /// GUID Primary Key
        /// </summary>
        [Key]
        [StringLength(36)]
        public string id
        {
            get;
            set;
        }
      



        /// <summary>
        /// 登录名
        /// </summary>
        [StringLength(64)]
        public string user_login_name { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [StringLength(10)]
        public string user_password { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [StringLength(10)]
        public string user_name { get; set; }
        /// <summary>
        /// 教师号
        /// </summary>
        [StringLength(19)]
        public string teacherCode { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        [StringLength(32)]
        public string user_idcard { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [StringLength(32)]
        public string user_mobile { get; set; }
        /// <summary>
        /// 邮箱地址
        /// </summary>
        [StringLength(64)]
        public string user_email { get; set; }
        /// <summary>
        /// 职称
        /// </summary>
        [StringLength(60)]
        public string teacherTitleCreate { get; set; }

        /// <summary>
        /// 在职状态
        /// </summary>
        [StringLength(60)]
        public string teacherJobStatusCode { get; set; }


        /// <summary>
        /// 性别
        /// </summary>
        [StringLength(100)]
        public string user_gender { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        [StringLength(60)]
        public string user_birthday { get; set; }


        /// <summary>
        /// 名族
        /// </summary>
        [StringLength(60)]
        public string user_national { get; set; }

        /// <summary>
        /// 政治面貌
        /// </summary>
        [StringLength(36)]
        public string user_political { get; set; }

        /// <summary>
        /// 籍贯
        /// </summary>
        [StringLength(10)]
        public string user_province { get; set; }
        /// <summary>
        /// 专业方向
        /// </summary>
        [StringLength(36)]
        public string teacherProfessionalDirection { get; set; }

        /// <summary>
        /// 任职日期
        /// </summary>
        [StringLength(32)]
        public string teacherEntryDate { get; set; }

        /// <summary>
        /// 参加工作日期
        /// </summary>
        [StringLength(36)]
        public string teacherStartworkDate { get; set; }

        /// <summary>
        /// 高校教龄
        /// </summary>
        [StringLength(36)]
        public string teacherEduAge { get; set; }
        /// <summary>
        /// 学历
        /// </summary>
        [StringLength(32)]
        public string teacherEducation { get; set; }
        /// <summary>
        /// 学位
        /// </summary>
        [StringLength(10)]
        public string teacherAcademicDegree { get; set; }
        /// <summary>
        /// 毕业学校
        /// </summary>
        [StringLength(36)]
        public string teacherGraduateSchool { get; set; }
        /// <summary>
        /// 毕业时间
        /// </summary>
        [StringLength(32)]
        public string teacherGraduateDate { get; set; }


        /// <summary>
        /// 所学专业
        /// </summary>
        [StringLength(10)]
        public string teacherStudyProfessional { get; set; }
        /// <summary>
        /// 个人简历
        /// </summary>
        [StringLength(19)]
        public string teacherPersonalResume { get; set; }
        
    }

    public static class tacher_info_importExt {

        public static UserBase GetUserInfo(this tacher_info_import model)
        {

            var obj= new UserBase()
            {
                    Id=Guid.NewGuid(),
                     about="",
                      identity=2,
                       isCompleted=false,
                        smallAvatar=UserInfoImg.GetDefaultUserAvator(""),
                    largeAvatar = UserInfoImg.GetDefaultUserAvator(""),
                    mediumAvatar = UserInfoImg.GetDefaultUserAvator(""),
                    //  userBirthday=model.user_birthday,
                        userEmail=model.user_email,
                         userFullName=model.user_name,
                          userEnbleFlag=false,
                    userGender = model.user_gender == "男" ? "1" : (model.user_gender == "女"?"2":"0"),
                     userIdcard=model.user_idcard,
                      userLoginName=model.user_login_name,
                       userMobile=model.user_mobile,
                        userNational=model.user_national,
                         userPassWord=sha1Encrypt.getSHA1Value(model.user_password ?? ""),
                          userPolitical=model.user_political,
            };
            if (!string.IsNullOrEmpty(model.user_birthday))
            {
                try
                {
                    var v = DateTime.Parse(model.user_birthday);
                    obj.userBirthday = v;
                }
                catch (Exception ex)
                {
                }
            }
            return obj;
        }
        public static TeacherInfo GetTeacherInfo(this tacher_info_import model) {
            var obj= new TeacherInfo()
            {
                 createTime=DateTime.Now,
                  Id=Guid.NewGuid(),
                   isDel=false,
                 teacherAcademicDegree = model.teacherAcademicDegree,
                  teacherCode=model.teacherCode,
                    teacherEducation=model.teacherEducation,
                     teacherEnbleFlag=false,
                      teacherGraduateSchool=model.teacherGraduateSchool,
                        teacherIsDisplay=false,
                         teacherIsRecommend=false,
                           teacherPersonalResume=model.teacherPersonalResume??"",
                            teacherProfessionalDirection=model.teacherProfessionalDirection,
                             teacherStudyProfessional=model.teacherStudyProfessional,
                          //   teacherTitle = model.teacherTitleCreate,
                           
            };
            if (!string.IsNullOrEmpty(model.teacherTitleCreate)) {

                switch (model.teacherTitleCreate) {
                    case "教授": { obj.teacherTitle="1";} break;
                    case "副教授": { obj.teacherTitle = "2"; } break;
                    case "讲师": { obj.teacherTitle = "3"; } break;
                    case "助教": { obj.teacherTitle = "4"; } break;
                }
            }
            if (!string.IsNullOrEmpty(model.teacherJobStatusCode))
            {
                switch (model.teacherJobStatusCode)
                {
                    case "在职": { obj.teacherJobStatusCode = 1; } break;
                    case "停职": { obj.teacherJobStatusCode = 2; } break;
                    case "离职": { obj.teacherJobStatusCode = 3; } break;
                }
            }
            if (!string.IsNullOrEmpty(model.teacherEduAge)) {
                try
                {
                    var v = float.Parse(model.teacherEduAge);
                    obj.teacherEduAge = v;
                }
                catch (Exception ex) { 
                }
            }
            if (!string.IsNullOrEmpty(model.teacherEntryDate))
            {
                try
                {
                    var v = DateTime.Parse(model.teacherEntryDate);
                    obj.teacherEntryDate = v;
                }
                catch (Exception ex)
                {
                }
            }
            if (!string.IsNullOrEmpty(model.teacherGraduateDate))
            {
                try
                {
                    var v = DateTime.Parse(model.teacherGraduateDate);
                    obj.teacherGraduateDate = v;
                }
                catch (Exception ex)
                {
                }
            }
            
            if (!string.IsNullOrEmpty(model.teacherStartworkDate))
            {
                try
                {
                    var v = DateTime.Parse(model.teacherStartworkDate);
                    obj.teacherStartworkDate = v;
                }
                catch (Exception ex)
                {
                }
            }
            //teacherStartworkDate
            return obj;
        
        }

    
    }
    /// <summary>
    /// 选择教师 待选
    /// </summary>
    public class LeftTeacherInfoDto : PaginationInputDto
    {
        public string UserId { get; set; }
        public string RelationId { get; set; }
        public string UserLoginName { get; set; }

        public string UserFullName { get; set; }

        public string Gender { get; set; }


        public IList<LeftTeacherInfoOutDto> Teachers { get; set; }

        public LeftTeacherInfoDto()
        {
            Teachers = new List<LeftTeacherInfoOutDto>();
        }

    }
    public class LeftTeacherInfoOutDto 
    {
        public string UserId { get; set; }
        public string UserLoginName { get; set; }

        public string UserFullName { get; set; }

        public string Gender { get; set; }


    }
    /// <summary>
    /// 选择教师 预选
    /// </summary>
    public class RigthTeacherInputDto
    {
        /// <summary>
        /// 教师关联对象id 添加教师功能多个地方引用统一改为关联ID
        /// </summary>
        public string relationId { get; set; }
        public IList<TeacherInputDto> TeacherInputDtos { get; set; }
    }
    public class TeacherInputDto
    {
        public string userId { get; set; }


    }
}
