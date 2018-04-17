using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Domain.Repositories;

namespace SPOC.Core
{
    /// <summary>
    /// 上传文件服务接口实现
    /// </summary>
    public class UploadFileService:ApplicationService, IUploadFileService
    {
        private readonly IRepository<UploadFile, Guid> _iUploadFileRep;
        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public UploadFileService(IRepository<UploadFile, Guid> iUploadFileRep)
        {
            _iUploadFileRep = iUploadFileRep;
        }
        #endregion

        /// <summary>
        /// 获取上传文件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<UploadFile> Get(Guid id)
        {
            return await _iUploadFileRep.GetAsync(id);
        }
    }
}