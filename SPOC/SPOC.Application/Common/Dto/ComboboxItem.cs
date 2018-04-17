using System;

namespace SPOC.Common.Dto
{
    /// <summary>
    /// UI用数据结构
    /// </summary>
    public class ComboboxItem
    {
        public Guid id { get; set; }
        public string text { get; set; }
        public object data { get; set; }

        public int seq { get; set; }
    }
}