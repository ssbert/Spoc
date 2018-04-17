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
    /// 院系输出Dto
    /// </summary>
    public class FacultyOutDto
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string updateTime { get; set; }
        public string updateUser { get; set; }
    }
    /// <summary>
    /// 院系输入Dto
    /// </summary>
    public class FacultyInputDto: PaginationInputDto
    {
        public string id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
     
    }

  
}
