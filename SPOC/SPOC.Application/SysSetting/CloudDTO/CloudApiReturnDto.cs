/********************************************************************************
** auth： bert
** date： 2016/5/24 17:25:52
** desc： 
*********************************************************************************/

using System;

namespace SPOC.SysSetting.CloudDTO
{
    [Serializable]
    public class CloudApiReturnDto
    {
        public int id { get; set; }
        public string accessKey { get; set; }
        public string secretKey { get; set; }
    }
}
