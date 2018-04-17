using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPOC.User
{
    public class TeacherInfo : Entity<Guid>
    {

        [Column("id")]
        public override Guid Id { get; set; }
        public TeacherInfo()
        {
            this.Id = Guid.NewGuid();
            this.teacherEnbleFlag = false;
            this.isDel = false;
            this.createTime = DateTime.Now;
            this.updateTime = DateTime.Now;
        }

       
        public Guid userId { get; set; }

        /// <summary>
        /// 教师号
        /// </summary>
        [StringLength(255)]
        public string teacherCode { get; set; }


        /// <summary>
        /// 职称 
        /// </summary>
        [StringLength(255)]
        public string teacherTitle { get; set; }
        /// <summary>
        /// 在职状态码( 1:在职  2:停职 0:离职)
        /// </summary>
        public int? teacherJobStatusCode { get; set; }
        /// <summary>
        /// 专业方向
        /// </summary>
        [StringLength(255)]
        public string teacherProfessionalDirection { get; set; }
        
       


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
        [StringLength(255)]
        public string teacherEducation { get; set; }
        /// <summary>
        /// 学位
        /// </summary>
        [StringLength(255)]
        public string teacherAcademicDegree { get; set; }

        /// <summary>
        /// 毕业学校
        /// </summary>
        [StringLength(255)]
        public string teacherGraduateSchool { get; set; }

        /// <summary>
        /// 毕业日期
        /// </summary>
        public DateTime? teacherGraduateDate { get; set; }

        /// <summary>
        /// 所学专业
        /// </summary>
        [StringLength(255)]
        public string teacherStudyProfessional { get; set; }

        /// <summary>
        /// 课程权限 1:有权限 0：没权限
        /// </summary>
        public bool teacherIsAddCourse { get; set; }

        /// <summary>
        /// 邀请码
        /// </summary>
        [StringLength(6)]
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
        /// 教师简历
        /// </summary>
        public string teacherPersonalResume { get; set; }


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
        /// 直播频道
        /// </summary>
        public string channelId { get; set; }
        /// <summary>
        /// 推流地址
        /// </summary>
        public string pushUrl { set; get; }
      
    }
}
