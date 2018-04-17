using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SPOC.Common.Dto
{
    /// <summary>
    /// id列表inputDto
    /// </summary>
    public class IdListInputDto
    {
        public IdListInputDto()
        {
            idList = new List<Guid>();
        }
        [Required]
        public List<Guid> idList { get; set; }
    }
}