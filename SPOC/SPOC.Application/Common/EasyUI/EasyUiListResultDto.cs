using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace SPOC.Common.EasyUI
{
    /// <summary>
    /// Implements <see cref="IListResult{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of the items in the <see cref="rows"/> list</typeparam>
    [Serializable]
    public class EasyUiListResultDto<T> : EasyUiListResult,IEasyUiListResult<T>
    {
        /// <summary>
        /// List of items.
        /// </summary>
        public IReadOnlyList<T> rows
        {
            get { return _rows ?? new List<T>(); }
            set { _rows = value; }
        }
        private IReadOnlyList<T> _rows;

        /// <summary>
        /// Creates a new <see cref="EasyUiListResultDto{T}"/> object.
        /// </summary>
        public EasyUiListResultDto()
        {
            
        }
        /// <summary>
        /// total record .
        /// </summary>
       public long total { get; set; }
    }
}