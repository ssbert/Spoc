using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;

namespace SPOC.User
{
    /// <summary>
    /// 学生教学班级关联 学生与教学班级多对多
    /// </summary>
    [Table("class_student")]
   public class ClassStudent : Entity<Guid>
    {
        /// <summary>
        /// 教学班级ID
        /// </summary>
        [Column("classId")]
        public Guid ClassId { get; set; }
        /// <summary>
        /// 学生ID
        /// </summary>
        [Column("userId")]
        public Guid UserId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("createTime")]
        public DateTime CreateTime { get; set; }
    }
}
