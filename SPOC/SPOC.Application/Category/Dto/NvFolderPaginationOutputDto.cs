using System.Collections.Generic;

namespace SPOC.Category.Dto
{
    public class NvFolderPaginationOutputDto
    {
        public List<NvFolderItemOutputDto> rows { get; set; }
        public int total { get; set; }
    }
}