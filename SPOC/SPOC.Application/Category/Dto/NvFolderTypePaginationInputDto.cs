using SPOC.Common.Pagination;

namespace SPOC.Category.Dto
{
    public class NvFolderTypePaginationInputDto: PaginationInputDto
    {
        public string folderTypeName { get; set; }
        public string folderTypeCode { get; set; }
    }
}