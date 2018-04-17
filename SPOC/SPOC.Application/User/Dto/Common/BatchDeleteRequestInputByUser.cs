using System;
using Abp.Application.Services.Dto;

namespace SPOC.User.Dto.Common
{
    [Serializable]
    public class BatchDeleteRequestInputByUser : EntityDto<string>
    {
        public BatchDeleteRequestInputByUser()
        {

        }

        public BatchDeleteRequestInputByUser(string id ) :
            base(id)
        {
            //this.user_id = user_id;
        }

     //   public Guid user_id { get; set; }
        public string user_id { get; set; }
    }
}
