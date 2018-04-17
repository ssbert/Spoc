using System;
using Abp.Domain.Entities;

namespace SPOC.User
{
    public class UserLoginRemember : Entity<Guid>
    {

        public string key { get; set; }
        public string userName { get; set; }

        public string passWord { get; set; }

        public DateTime expiringDate { get; set; }
    }
}
