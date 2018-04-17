using Abp.Domain.Entities;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOC.User
{
    public class StudentInfo : Entity<Guid>
    {

        [Column("id")]
        public override Guid Id { get; set; }
        public StudentInfo()
        {

            if (Id == null)
            {
                Id = Guid.NewGuid();
            }

            this.createTime = DateTime.Now;
            this.updateTime = DateTime.Now;
        }

        /// <summary>
        /// user表id
        /// </summary>
        ///
        public Guid userId { get; set; }

       

        /// <summary>
        /// 学号
        /// </summary>
        [StringLength(255)]
        public string userCode { get; set; }


        /// <summary>
        /// 昵称
        /// </summary>
        [StringLength(255)]
        public string userNickname { get; set; }



        /// <summary>
        /// 所在省份(籍贯)
        /// </summary>
        [StringLength(50)]
        public string userProvince { get; set; }

        /// <summary>
        /// 所在城市(来源地区)
        /// </summary>
        [StringLength(255)]
        public string userCity { get; set; }



     

        /// <summary>
        /// 院系
        /// </summary>
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid facultyId { get; set; }


        /// <summary>
        ///专业ID 
        /// </summary>
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid majorId { get; set; }


        /// <summary>
        /// 行政班级ID
        /// </summary>
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid classId { get; set; }


        /// <summary>
        /// 学习形式
        /// </summary>
        [StringLength(255)]
        public string studyForm { get; set; }

        /// <summary>
        /// 学习标记
        /// </summary>
        [StringLength(255)]
        public string studyFlag { get; set; }

        /// <summary>
        /// 层次
        /// </summary>
        [StringLength(255)]
        public string level { get; set; }

        /// <summary>
        ///学制
        /// </summary>
        [StringLength(255)]
        public string userEductional { get; set; }

        /// <summary>
        /// 入学时间
        /// </summary>
        public DateTime? userAdmission { get; set; }

        /// <summary>
        /// 年级
        /// </summary>
        [StringLength(255)]
        public string userGrade { get; set; }

        /// <summary>
        /// 宿舍
        /// </summary>
        [StringLength(255)]
        public string userDormitory { get; set; }



        /// <summary>
        ///邮编 
        /// </summary>
        [StringLength(255)]
        public string userZipcode { get; set; }

        /// <summary>
        /// 学生邀请码
        /// </summary>
        [StringLength(255)]
        public string userInviteCode { get; set; }

        /// <summary>
        ///  注册邀请码
        /// </summary>
        [StringLength(255)]
        public string userRegisterInviteCode { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public int userEnbleFlag { get; set; }

        /// <summary>
        /// 是否毕业
        /// </summary>
        public bool isGraduation { get; set; }

        /// <summary>
        /// 毕业时间
        /// </summary>
        public DateTime? graduationDate { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime updateTime { get; set; }



        /// <summary>
        /// 是否删除
        /// </summary>
        public bool isDel { get; set; }



    

    }
}
