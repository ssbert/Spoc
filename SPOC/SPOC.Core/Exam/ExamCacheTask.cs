using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace SPOC.Exam
{
    /// <summary>
    /// 考试缓存任务
    /// </summary>
    public class ExamCacheTask : Entity<Guid>
    {
        [Column("id")]
        public override Guid Id { get; set; }
        public ExamCacheTask()
        {
            statusCode = "no_start";
        }
        /// <summary>
        /// 变更标题
        /// </summary>
        [StringLength(256)]
        public string changeTitle { get; set; }
        /// <summary>
        /// 变动种类
        /// exam_info:考试变动
        /// exam_arrange:考试安排
        /// exam_paper:试卷变动
        /// exam_grade:考试成绩
        /// exam_change_grade:考试记录变更
        /// </summary>
        [StringLength(64)]
        public string changeClass { get; set; }
        /// <summary>
        /// 变动类型(add:新增，edit:编辑，delete:删除)
        /// </summary>
        [StringLength(16)]
        public string changeType { get; set; }
        /// <summary>
        /// 变动对象的ID
        /// </summary>
        public Guid objectUid { get; set; }
        /// <summary>
        /// 关联对象的ID
        /// </summary>
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid relativeUid { get; set; }

        /// <summary>
        /// 状态(no_start:未启动，updating:更新中，has_done:已完成)
        /// </summary>
        [StringLength(16)]
        [DefaultValue("no_start")]
        public string statusCode { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createTime { get; set; }
        /// <summary>
        /// 创建者ID
        /// </summary>
        public Guid creatorUid { get; set; }
        /// <summary>
        /// 开始处理时间
        /// </summary>
        public DateTime beginTime { get; set; }
        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime doneTime { get; set; }
    }
}
