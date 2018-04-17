using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPOC.Statement.Dto.Lib
{
    /// <summary>
    /// 班级知识点掌握统计数据项
    /// </summary>
    public class ClassLabelGettingItem
    {

        /// <summary>
        /// 知识点名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 标签Id
        /// </summary>
        public Guid LabelId { get; set; }

        /// <summary>
        /// 通过人数
        /// </summary>
        public int PassNumber { get; set; }

        /// <summary>
        /// 未通过人数
        /// </summary>
        public int FailNumber { get; set; }

        /// <summary>
        /// 未参与人数 无反馈
        /// </summary>
        public int NotJoinNumber { get; set; }

        /// <summary>
        /// 不稳定的
        /// </summary>
        public int UnstableNumber { get; set; }

        /// <summary>
        /// 班级总学生数
        /// </summary>
        public int StudentNumber { get; set; }

        /// <summary>
        /// 通过率
        /// </summary>
        public decimal PassRate { get; set; }
    }
    /// <summary>
    /// 学生标签掌握情况详细信息
    /// </summary>
    public class UserLabelGettingItem
    {
        /// <summary>
        /// 学生ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        public string UserLoginName { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string UserFullName { get; set; }
        /// <summary>
        /// 班级ID
        /// </summary>
        public Guid ClassId { get; set; }
        /// <summary>
        /// 班级名称
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 标签掌握状态
        /// </summary>
        public int Status { get; set; }

    }
}
