using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPOC.Faqs.Dtos
{
    public class FaqFolderDto
    {
        /// <summary>
        /// 分类ID
        /// </summary>
        public Guid id { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 子分类
        /// </summary>
        public List<FaqFolderDto> faqFolders { get; set; }
    }
}
