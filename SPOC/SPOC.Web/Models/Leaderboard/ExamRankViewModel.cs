using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SPOC.Web.Models.Leaderboard
{
    public class ExamRankViewModel
    {
        
        public string Exam { get; set; }
        /// <summary>
        /// 参加过的考试列表
        /// </summary>
        public IEnumerable<SelectListItem> ExamListItems { get; set; }
    }
}