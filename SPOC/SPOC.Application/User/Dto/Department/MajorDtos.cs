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
    /// 专业输出Dto
    /// </summary>
    public class MajorOutDto
    {
        public Guid id { get; set; }
        public Guid facultyId { get; set; }
        public string facultyName { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string updateTime { get; set; }
        public string updateUser { get; set; }
    }
    /// <summary>
    /// 专业输入Dto
    /// </summary>
    public class MajorInputDto: PaginationInputDto
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public Guid facultyId { get; set; }
        public string facultyName { get; set; }
    }
}
