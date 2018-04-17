using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.User
{
    /// <summary>
    /// 班级教师关联
    /// </summary>
    [Table("class_teacher")]
    public class ClassTeacher : Entity<Guid>
    {
        /// <summary>
        /// 班级ID
        /// </summary>
        [Column("classId")]
        public Guid ClassId { get; set; }
        /// <summary>
        /// 教师UserID
        /// </summary>
        [Column("userId")]
        public Guid UserId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("createTime")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [Column("createUserId")]
        public Guid CreateUserId { get; set; }
    }
}
