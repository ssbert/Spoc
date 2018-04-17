using System;
using System.Threading.Tasks;
using Abp.Application.Services;

namespace SPOC.Core
{
    /// <summary>
    /// 上传文件服务接口
    /// </summary>
    public interface IUploadFileService : IApplicationService
    {
        /// <summary>
        /// 获取上传文件信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<UploadFile> Get(Guid id);
    }
}