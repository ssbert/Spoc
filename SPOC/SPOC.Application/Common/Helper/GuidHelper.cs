using System;

namespace SPOC.Common.Helper
{
    public class GuidHelper
    {
        public static Guid Init(Guid guid)
        {
            if (guid.ToString() == "00000000-0000-0000-0000-000000000000")
            {
                return Guid.NewGuid();
            }
            else
            {
                return guid;
            }
        }
    }
}
