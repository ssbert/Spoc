using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPOC.Lib.Dto
{


    /// <summary>
    /// 标签信息
    /// </summary>
    public class LabelDto
    {
        public Guid id { get; set; }

        /// <summary>
        /// 分类Id 
        /// </summary>
        public Guid folderId { get; set; }

        /// <summary>
        /// 标签标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string describe { get; set; }

        public List<LabelRulesDto> rules { get; set; }
    }

    public class LabelRulesDto
    {
        public Guid id { get; set; }

        /// <summary>
        /// 匹配关键字
        /// </summary>
        public string matchText { get; set; }

        /// <summary>
        /// 逻辑 1与 0或  默认0(或)关系
        /// </summary>
        public byte logic { get; set; }

        /// <summary>
        /// 说明描述
        /// </summary>
        public string describe { get; set; }

        /// <summary>
        /// 正则表达式
        /// </summary>
        public string regExpressions { get; set; }
    }

    public class MatchTextInputDto
    {
        public string code { get; set; }
        public string questionText { get; set; }
    }

 
}
