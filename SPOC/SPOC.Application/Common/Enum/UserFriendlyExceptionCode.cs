using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPOC.Common.Enum
{
    [Flags]
    public enum UserFriendlyExceptionCode
    {
        /// <summary>
        /// Error.
        /// </summary>
        Error = 0,

        /// <summary>
        /// Info.
        /// </summary>
        Info = 1,

        /// <summary>
        /// Warn
        /// </summary>
        Warn = 2
    }
}
