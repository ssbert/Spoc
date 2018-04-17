using System;
using Abp.Application.Services.Dto;

namespace SPOC.Common.EasyUI
{
    /// <summary>
    /// This <see cref="IEntityDto"/> can be directly used (or inherited)
    /// to pass an Id value to an application service method.
    /// </summary>
    /// <typeparam name="TId">Type of the Id</typeparam>
    [Serializable]
    public class BatchRequestInput : EntityDto<string>, IEntityDto<string>
    {
        public BatchRequestInput()
        {

        }

        public BatchRequestInput(string id) :
            base(id)
        {

        }
    }
}