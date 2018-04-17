using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Runtime.Validation;
using SPOC.Common.Pagination;

namespace SPOC.Faqs.Dtos
{
    public class FaqInputDto:PaginationInputDto, IShouldNormalize
    {
        public string title { get; set; }
        public string content { get; set; }

        public List<Guid> folderId { get; set; }

        public void Normalize()
        {
            if (folderId==null)
            {
                folderId = new List<Guid>();
            }
        }
    }
}
