using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using Abp.Runtime.Validation;
using SPOC.SystemSet;

namespace SPOC.Faqs.Dtos
{
    [AutoMapTo(typeof(Faq))]
    public class FaqEditDto:IShouldNormalize
    {

        public Guid? Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [StringLength(50)]
        public string title { get; set; }
        public string content { get; set; }
        public DateTime updateTime { get; set; }
        public int seq { get; set; }
        public Guid folderId { get; set; }
        public int userFul { get; set; }
        public int userLess { get; set; }
        public bool IsActive { get; set; }
        public void Normalize()
        {
            updateTime = DateTime.Now;
        }
    }
}