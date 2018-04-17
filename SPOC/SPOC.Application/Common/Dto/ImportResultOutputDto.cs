using System.Collections.Generic;

namespace SPOC.Common.Dto
{
    public class ImportResultOutputDto
    {
        public int successCount { get; set; }
        public string errMessage { get; set; }
    }

    #region add by kelei

    public class SelectListItemDto
    {
        public string id { get; set; }
        public string text { get; set; }
        public bool @checked { get; set; }
        public bool selected { get; set; }

        private string isAllowChecked = "true";
        public string IsAllowChecked { get { return isAllowChecked; } set { isAllowChecked = value; } }
        public IList<SelectListItemDto> children { get; set; }

        public SelectListItemDto()
        {
            children = new List<SelectListItemDto>();
            @checked = false;
            selected = false;
        }
    }

    #endregion
}