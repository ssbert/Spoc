using System;
using System.Collections.Generic;
using System.Linq;

namespace SPOC.Common.Dto
{
    /// <summary>
    /// UI用数据结构
    /// </summary>
    public class CombotreeNode
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public CombotreeNode()
        {
            children = new List<CombotreeNode>();
            parentId = Guid.Empty;
        }
        public Guid id { get; set; }
        public Guid parentId { get; set; }
        public string text { get; set; }
        public object data { get; set; }
        public int seq { get; set; }
        public List<CombotreeNode> children { get; set; }

        /// <summary>
        /// 根据list中数据构建出树结构
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<CombotreeNode> BuildTree(List<CombotreeNode> list)
        {
            var root = list.Where(o=>o.parentId == Guid.Empty).OrderBy(o=>o.seq).ToList();
            root.ForEach(r =>
            {
                Build(r, list);
            });
            
            return root;
        }

        private static void Build(CombotreeNode parent, List<CombotreeNode> list)
        {
            parent.children.AddRange(list.Where(c => c.parentId == parent.id).OrderBy(c => c.seq).ToList());
            if (parent.children.Any())
            {
                parent.children.ForEach(r =>
                {
                    Build(r, list);
                });
            }
        }
    }
}