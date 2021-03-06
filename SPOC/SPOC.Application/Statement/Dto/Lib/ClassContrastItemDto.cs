﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPOC.Statement.Dto.Lib
{
    /// <summary>
    /// 班级知识点掌握统计对比
    /// </summary>
    public class ClassContrastItemDto
    {
        /// <summary>
        /// 班级ID
        /// </summary>
        public Guid ClassId { get; set; }
        /// <summary>
        /// 班级名称
        /// </summary>
        public string ClassName { get; set; }
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
    /// 班级对比chart折线图
    /// </summary>
    public class ClassContrastChartDto
    {
        /// <summary>
        /// 图例 班级列表
        /// </summary>
        public List<string> Legend { get; set; }
        /// <summary>
        /// x轴 知识点
        /// </summary>
        public List<string> XAxis { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public List<SeriesItem> SeriesData { get; set; }
    }

    public class SeriesItem
    {
        /// <summary>
        /// 线条名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 填充数据
        /// </summary>
        public List<decimal> Data { get; set; }
    }


}
