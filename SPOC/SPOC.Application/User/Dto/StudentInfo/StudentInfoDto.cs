using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Newtonsoft.Json;

namespace SPOC.User.Dto.StudentInfo
{
    [AutoMapFrom(typeof(User.StudentInfo))]
    [Serializable]
    public class StudentInfoDto : EntityDto<Guid>
    {

        public Guid id => Id;


        public Guid user_id { get; set; }

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
        /// 院系 
        /// </summary>
        public string user_facultyid { get; set; }
        public string user_faculty { get; set; }
        /// <summary>
        ///专业ID 
        /// </summary>
        public string user_majorid { get; set; }
        /// <summary>
        /// 专业
        /// </summary>
        public string user_major { get; set; }

        /// <summary>
        /// 行政班级名称
        /// </summary>
        public string user_administrativeclass { get; set; }
        public string user_administrativeclassid { get; set; }

        /// <summary>
        /// 教学班级名称
        /// </summary>
        public string user_class { get; set; }
        public string user_classid { get; set; }

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

        /// <summary>
        /// 登录ip
        /// </summary>
        public string loginIp { get; set; }

        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime? loginTime { get; set; }




    }



    public static class StudentInfoDtoExt
    {

        public static user_info_import GetUserInfoImport(this StudentInfoDto model)
        {

            return new user_info_import()
            {
                administrative_class = model.user_administrativeclass,
                user_email = model.user_email,
                user_idcard = model.user_idcard,
                user_code = model.user_code,
                user_mobile = model.user_mobile,
                login_name = model.user_login_name,
                create_time = model.create_time.ToString("yyyy-MM-dd HH:mm:ss"),
                user_zipcode = model.user_zipcode,
                is_graduation = model.is_graduation,
                user_major = model.user_major,
                user_faculty = model.user_faculty,
                user_class = model.user_class,
                user_majorid = model.user_majorid,
                user_province = model.user_province,
                id = model.user_id.ToString(),
                user_political = model.user_political,
                user_password = model.user_password,
                user_nickname = model.user_nickname,
                user_national = model.user_national,
                user_name = model.user_name,
                user_gender = model.user_gender,
                user_eductional = model.user_eductional,
                graduation_date = model.graduation_date.ToString(),
                user_birthday = model.user_birthday.ToString(),
                user_admission = model.user_admission.ToString(),
                user_grade = model.user_grade,
                user_dormitory = model.user_dormitory,
                user_city = model.user_city,
                study_form = model.study_form,
                level = model.level,


            };
        }
    }

    public class ApplyStudentOutDto
    {
        //主键ID
        public Guid id { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        public string userLoginName { get; set; }
        /// <summary>
        /// 用户真实姓名
        /// </summary>
        public string userFullName { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string userMobile { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string userEmail { get; set; }
        /// <summary>
        /// 院系
        /// </summary>
        public string facultyName { get; set; }
        /// <summary>
        /// 专业
        /// </summary>
        public string majorName { get; set; }
        /// <summary>
        /// 班级名称
        /// </summary>
        public string className { get; set; }
        /// <summary>
        /// 班级预设学生容量
        /// </summary>
        public int classStudentName { get; set; }
        /// <summary>
        /// 当前学生数量
        /// </summary>
        public int studentName { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public string createTime { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string status { get; set; }
    }

    public class ClassStudentDto
    {
        public Guid userId { get; set; }
        public string classIds { get; set; }
        public string classNames { get; set; }
    }
}
