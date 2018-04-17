using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;

namespace SPOC.User
{
    /// <summary>
    /// 教学班级
    /// </summary>
    [Table("class")]
    public class Class : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 院系Id
        /// </summary>
        public Guid facultyId { get; set; }
        /// <summary>
        /// 专业ID
        /// </summary>
        public Guid majorId { get; set; }
        /// <summary>
        /// 班级名称
        /// </summary>
        [StringLength(36)]
        public string name { get; set; }

      
        /// <summary>
        /// 更新人
        /// </summary>
        public Guid updateUserId { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime updateTime { get; set; }
        /// <summary>
        /// 学生容量
        /// </summary>
        public int studentNum { get; set; }
    }
}
