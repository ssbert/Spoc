using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using SPOC.Common.Dto;
using SPOC.Common.EasyUI;
using SPOC.Common.Pagination;

namespace SPOC.User.Dto.StudentInfo
{
    public class StudentInfoInputDto : EasyuiDto
    {
      

        /// <summary>
        ///学生用户名
        /// </summary>
        public string user_login_name { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string user_mobile { get; set; }

        /// <summary>
        /// 学号
        /// </summary>
        public string user_code { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string user_email { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string user_name { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string user_gender { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string user_idcard { get; set; }

        /// <summary>
        /// 部门(组织架构Id)
        /// </summary>
        public string department  { get; set; }
    }

    /// <summary>
    /// 学生导入表
    /// </summary>
    [Serializable]
    [DataContract]
    public class user_info_import
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
        [StringLength(36)]
        public string import_id { get; set; }
        /// <summary>
        /// 学生学号
        /// </summary>
        [StringLength(64)]
        public string user_code { get; set; }
        /// <summary>
        /// 学生名称
        /// </summary>
        [StringLength(10)]
        public string user_name { get; set; }
        /// <summary>
        /// 性别'male','female','secret'
        /// </summary>
        [StringLength(10)]
        public string user_gender { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        [StringLength(19)]
        public string user_birthday { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        [StringLength(32)]
        public string user_national { get; set; }

        /// <summary>
        /// 政治面貌
        /// </summary>
        [StringLength(32)]
        public string user_political { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        [StringLength(64)]
        public string user_password { get; set; }
        /// <summary>
        /// 所在省份（籍贯）
        /// </summary>
        [StringLength(60)]
        public string user_province { get; set; }

        /// <summary>
        /// 所在城市 （来源地区）
        /// </summary>
        [StringLength(60)]
        public string user_city { get; set; }


        /// <summary>
        /// 出生地区
        /// </summary>
        [StringLength(100)]
        public string user_birth { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        [StringLength(60)]
        public string user_nickname { get; set; }


        /// <summary>
        /// Email邮箱地址
        /// </summary>
        [StringLength(60)]
        public string user_email { get; set; }

    
        /// <summary>
        /// 院系 |部门id
        /// </summary>
        [StringLength(36)]
        public string user_facultyid { get; set; }

        /// <summary>
        /// 院系 |部门
        /// </summary>
        [StringLength(32)]
        public string user_faculty { get; set; }

        [StringLength(36)]
        public string faculty_code { get; set; }

        /// <summary>
        /// 专业id
        /// </summary>
        [StringLength(36)]
        public string user_majorid { get; set; }
        /// <summary>
        /// 专业
        /// </summary>
        [StringLength(32)]
        public string user_major { get; set; }
        /// <summary>
        /// 专业代码
        /// </summary>
        [StringLength(10)]
        public string major_code { get; set; }
        /// <summary>
        /// 教学班级id
        /// </summary>
        [StringLength(36)]
        public string user_classid { get; set; }
        /// <summary>
        /// 教学班级名称
        /// </summary>
        [StringLength(32)]
        public string user_class { get; set; }

        /// <summary>
        /// 行政班级ID
        /// </summary>
        public string administrative_classid { get; set; }
        /// <summary>
        /// 行政班级名称
        /// </summary>
        public string administrative_class { get; set; }


        /// <summary>
        /// 学制
        /// </summary>
        [StringLength(10)]
        public string user_eductional { get; set; }
        /// <summary>
        /// 入学时间
        /// </summary>
        [StringLength(19)]
        public string user_admission { get; set; }
        /// <summary>
        /// 年级
        /// </summary>
        [StringLength(10)]
        public string user_grade { get; set; }
        /// <summary>
        /// 宿舍
        /// </summary>
        [StringLength(10)]
        public string user_dormitory { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        [StringLength(18)]
        public string user_mobile { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        [StringLength(20)]
        public string user_idcard { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        [StringLength(10)]
        public string user_zipcode { get; set; }

        /// <summary>
        /// 毕业时间
        /// </summary>
        [StringLength(19)]
        public string graduation_date { get; set; }


        /// <summary>
        /// 0未毕业 1已毕业
        /// </summary>
        [DefaultValue(false)]
        public bool is_graduation { get; set; }
        /// <summary>
        /// 站点ID
        /// </summary>
        [StringLength(36)]
        public string user_siteid { get; set; }

        /// <summary>
        /// 站点名称
        /// </summary>
        [StringLength(128)]
        public string user_sitename { get; set; }

        [StringLength(8)]
        public string study_flag { get; set; }
        /// <summary>
        /// 层次（专科、专升本、本科）
        /// </summary>
        [StringLength(8)]
        public string level { get; set; }
        /// <summary>
        /// 学习形式（业余、函授）
        /// </summary>
        [StringLength(8)]
        public string study_form { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [StringLength(36)]
        public string creator_id { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [StringLength(19)]
        public string create_time { get; set; }
        /// <summary>
        /// 函授站名称
        /// </summary>
        [StringLength(35)]
        public string coo_name { get; set; }
        /// <summary>
        /// 函授站代码
        /// </summary>
        [StringLength(35)]
        public string coo_code { get; set; }

        /// <summary>
        /// 考生号
        /// </summary>
        [StringLength(64)]
        public string exam_num { get; set; }

        /// <summary>
        /// 登录名
        /// </summary>
        [StringLength(64)]
        public string login_name { get; set; }
    }
    public class UserInfoSeacheExport
    {
        public string userName_index { get; set; }
        public string user_code_index { get; set; }
        public string user_name_index { get; set; }
        public string user_gender_index { get; set; }
        public string school_id { get; set; }
        public string school_name_index { get; set; }
        public string user_facultyid { get; set; }
        public string user_specialtyid { get; set; }
        public string user_classid_index { get; set; }
        public string class_name_index { get; set; }
        public string teach_address_index { get; set; }
        public bool is_graduation_index { get; set; }
        public string user_idcard_index { get; set; }
    }
    public static class user_info_importExt {

        public static UserBase user_info_import2userBase(this user_info_import model) {
            var user = new UserBase()
            {
                Id = string.IsNullOrEmpty(model.id) ? Guid.Empty : Guid.Parse(model.id),
                userPolitical = model.user_political,
                userPassWord = model.user_password,
                userNational = model.user_national,
                userMobile = model.user_mobile,
                userLoginName = model.login_name,
                userFullName = model.user_name,
                userIdcard = model.user_idcard,
                userGender = model.user_gender == "男" ? "1" : (model.user_gender == "女" ? "2" : ""),
                userEmail = model.user_email,
                //userBirthday=string.IsNullOrEmpty(model.user_birthday)?DateTime.,

            };
            if (!string.IsNullOrEmpty(model.user_birthday)) {
                try
                {
                    var date = DateTime.Parse(model.user_birthday);
                    user.userBirthday = date;
                }
                catch { 
                
                }
               // DateTime d=DateTime.Parse("1991-01-01");
               // user.userBirthday = DateTime.TryParse(model.user_birthday, out d) ? DateTime.Parse("1991-01-01") : DateTime.Parse(model.user_birthday);
            }
            return user;
        }
        public static User.StudentInfo user_info_import2StudentInfo(this user_info_import model) {
            if (!string.IsNullOrEmpty(model.user_eductional)) {
                model.user_eductional=model.user_eductional.Replace("年制","");
            }
            if (!string.IsNullOrEmpty(model.level))
            {
                switch (model.level) {
                    case "专科": { model.level = "1"; } break;
                    case "专升本": { model.level = "2"; } break;
                    case "本科": { model.level = "3"; } break;
                }
            }
            var stu = new User.StudentInfo()
            {
                createTime = DateTime.Now,
                userZipcode = model.user_zipcode,
                isDel = false,
                isGraduation = model.is_graduation,
                userNickname = model.user_nickname,
                userProvince = model.user_province,
                userGrade = model.user_grade,
                userCode = model.user_code,
                userCity = model.user_city,
                userDormitory = model.user_dormitory,
                userEductional = model.user_eductional,
                level=model.level,
                userEnbleFlag = 0
            };
            if (!string.IsNullOrEmpty(model.graduation_date))
            {
                try
                {
                    var date = DateTime.Parse(model.graduation_date);
                    stu.graduationDate = date;
                }
                catch
                {

                }
               // DateTime d=DateTime.Parse("1991-01-01");
               // stu.graduationDate = DateTime.TryParse(model.graduation_date, out d) ? DateTime.Parse("1991-01-01") : DateTime.Parse(model.graduation_date);
            }
            if (!string.IsNullOrEmpty(model.user_admission))
            {
                try
                {
                    var date = DateTime.Parse(model.user_admission);
                    stu.userAdmission = date;
                }
                catch
                {

                }
               // DateTime d = DateTime.Parse("1991-01-01");
               // stu.userAdmission = DateTime.TryParse(model.user_admission, out d) ? DateTime.Parse("1991-01-01") : DateTime.Parse(model.user_admission);
            }
            return stu;
        }
    }


    public class ApplyStudentInputDto: PaginationInputDto
    {
        public string classId { get; set; }
        public string userLoginName { get; set; }
        public string status { get; set; }
        public string userFullName { get; set; }
        public string userMobile { get; set; }
        public string userEmail { get; set; }
    }
    //审核学生
    public class ApplyStudent: BatchRequestInput
    {
        //approved  refused
        public string status { get; set; }
      
    }
}
