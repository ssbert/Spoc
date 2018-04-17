using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using SPOC.SysSetting.CloudDTO;

namespace SPOC.SysSetting
{
    public interface ICloudService:IApplicationService
    {
        /// <summary>
        /// 获取省份
        /// </summary>
        /// <returns></returns>
        IEnumerable<object> GetProvince();
        /// <summary>
        /// 获取城市
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<object> GetCity(int id);
        /// <summary>
        /// 获取云授权相关信息
        /// </summary>
        /// <param name="forgetHost">是否忽略域名信息</param>
        /// <returns></returns>
        CloudDto GetCloud(bool forgetHost=false);
        /// <summary>
        /// 新课网授权or更新授权信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CloudDto> CreateOrUpdateCloud(CloudDto input);
    }
}
