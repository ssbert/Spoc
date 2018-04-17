using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPOC.Common.Dto;
using SPOC.Common.Pagination;

namespace SPOC.User.Dto.Department
{
    /// <summary>
    /// 班级输出Dto
    /// </summary>
    public class ClassOutDto
    {
        public Guid id { get; set; }
        public Guid facultyId { get; set; }
        public string facultyName { get; set; }
        public Guid majorId { get; set; }
        public string majorName { get; set; }
        public string name { get; set; }
        public int studentNum { get; set; }
        public string updateTime { get; set; }
        public string updateUser { get; set; }
    }
    /// <summary>
    /// 班级输入Dto
    /// </summary>
    public class ClassInputDto: PaginationInputDto
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public Guid facultyId { get; set; }
        public string facultyName { get; set; }
        public Guid majorId { get; set; }
        public string majorName { get; set; }
        /// <summary>
        /// 学生容量
        /// </summary>
        public int studentNum { get; set; }
        /// <summary>
        /// 班级类型 行政班级或教学班级 
        /// </summary>
        public string classType { get; set; }
    }

    /// <summary>
    /// 班级教师输出Dto
    /// </summary>
    public class ClassTeacherOutDto
    {
        /// <summary>
        /// 数据主键
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 班级ID
        /// </summary>
        public Guid ClassId { get; set; }
        /// <summary>
        /// 教师登录名
        /// </summary>
        public string UserLoginName { get; set; }
        /// <summary>
        /// 教师全名
        /// </summary>
        public string UserFullName { get; set; }
        public string Gender { get; set; }
        
        public string CreateTime { get; set; }
    }
   
}
