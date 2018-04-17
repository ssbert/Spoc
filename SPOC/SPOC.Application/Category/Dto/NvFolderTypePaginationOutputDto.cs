using System.Collections.Generic;

namespace SPOC.Category.Dto
{
    public class NvFolderTypePaginationOutputDto
    {
        public List<NvFolderTypeDto> rows { get; set; }
        public int total { get; set; }
    }
}