using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPOC.Web.Models
{
    public class UserAvtarSet 
    {
        public Guid userId { get; set; }
        public float x1 { get; set; }
        public float x2 { get; set; }
        public float y1 { get; set; }
        public float y2 { get; set; }


        public float selectionW { get; set; }
        public float selectionH { get; set; }

        public float defaultImgLen { get; set; }


        /// <summary>
        /// 图片路径
        /// </summary>
        public string AcatarImg { get; set; }
    }
}