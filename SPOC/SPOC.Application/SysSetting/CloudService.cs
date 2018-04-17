/********************************************************************************
** auth： bert
** date： 2016/5/23 16:53:37
** desc： 
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using Newtonsoft.Json;
using SPOC.Common;
using SPOC.Common.Enum;
using SPOC.Common.Extensions;
using SPOC.Common.Helper;
using SPOC.SysSetting.CloudDTO;
using SPOC.SystemSet;

namespace SPOC.SysSetting
{
    public class CloudService : SPOCAppServiceBase, ICloudService
    {
        private readonly IRepository<SiteSet, Guid> _siteSetRepository;
        private readonly IRepository<CityArea> _cityAreaRepository;
        private readonly IRepository<Cloud, Guid> _cloudRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private string NewMoocApiUrl
        {
            get { return L("payUrl").TrimEnd('/') + "/api/"; }
        }
        public CloudService(IRepository<CityArea> iCityAreaRepository, IRepository<Cloud, Guid> iCloudRepository, IRepository<SiteSet, Guid> siteRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _cityAreaRepository = iCityAreaRepository;
            _cloudRepository = iCloudRepository;
            _siteSetRepository = siteRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public IEnumerable<object> GetProvince()
        {
            var obj =
                _cityAreaRepository.GetAll()
                    .Where(a => a.ParentId == 0)
                    .OrderBy(c => c.OrderId)
                    .Select(c => new { id = c.Id, c.City }).ToList();
            return obj;
        }

        public IEnumerable<object> GetCity(int id)
        {
            var obj =
                _cityAreaRepository.GetAll()
                    .Where(a => a.ParentId == id)
                    .OrderBy(c => c.OrderId)
                    .Select(c => new { id=c.Id, c.City }).ToList();
            return obj;
        }

        public CloudDto GetCloud(bool forgetHost=false)
        {
          
            var query = _cloudRepository.GetAllList();
            var cloud = query.FirstOrDefault() ?? new Cloud() { Id = Guid.Empty, AccessKey = "", City = 0, CloudId = 0, PartnerCode = "", PartnerName = "", Province = 0, SecretKey = "",LiveServiceStatus=0 };
            var cloudDto = cloud.MapTo<CloudDto>();
            if (!forgetHost)
            {
                //获取域名信息
                var siteSet =
                    _siteSetRepository.GetAll()
                        .FirstOrDefault(s => s.settingGroup.Equals("sys_site") && s.settingKey.Equals("site_url"));
                if (siteSet != null)
                    cloudDto.Host = siteSet.settingValue;
            }
            return cloudDto;

        }

        public async Task<CloudDto> CreateOrUpdateCloud(CloudDto input)
        {
            Cloud cloud;
            if (input.Id.Equals(Guid.Empty))
            {

                //通过输入参数，创建一个新的实体
                cloud = new Cloud { PartnerCode = input.PartnerCode, Id = Guid.NewGuid(), PartnerName = input.PartnerName, Province = input.Province, City = input.City };
                //调用仓储基类的Insert方法把实体保存到数据库中
                await _cloudRepository.InsertAsync(cloud);
                input.Id = cloud.Id;

            }
            else
            {
                //通过仓储基类的通用方法Get，获取指定id的实体对象
                cloud = _cloudRepository.Get(input.Id);
                cloud.PartnerCode = input.PartnerCode;
                //修改实体的属性值
                cloud.Province = input.Province;
                cloud.City = input.City;
                if (!string.IsNullOrEmpty(input.PartnerName))
                {
                    cloud.PartnerName = input.PartnerName;
                }
                await _cloudRepository.UpdateAsync(cloud);
            }
            //修改域名配置
            var siteSet =
                _siteSetRepository.GetAll()
                    .FirstOrDefault(s => s.settingGroup.Equals("sys_site") && s.settingKey.Equals("site_url"));
            if (siteSet != null)
            {
                if (!siteSet.settingValue.Trim().Equals(input.Host.Trim()))
                {
                    siteSet.settingValue = input.Host;
                    await _siteSetRepository.UpdateAsync(siteSet);
                }
            }
            else
            {
                siteSet = new SiteSet() { Id = Guid.NewGuid(), isVisible = "Y", settingGroup = "sys_site", settingKey = "site_url", modifyTime = DateTime.Now, settingName = "域名", settingRemark = "门户域名", settingValue = input.Host };
                await _siteSetRepository.InsertAsync(siteSet);
            }

            await _unitOfWorkManager.Current.SaveChangesAsync();  //提交本地修改
            //新课网授权
            var dc = new Dictionary<string, string>()
             {
                    {"partnerCode",input.PartnerCode},
                    {"partnerName", input.PartnerName},
                    {"province", input.Province.ToString()}, 
                    {"city", input.City.ToString()}, 
                    {"host", input.Host}, 
                    {"cloudId", cloud.CloudId.ToString()}
             };
            var sign = SignatureHelper.GetSignature(dc);
            var url = NewMoocApiUrl + "Common/PartnerRegister?sign=" + sign;
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            var returnValue = "";
            using (var http = new HttpClient(handler))
            {
                var content = new FormUrlEncodedContent(dc);
                var response = await http.PostAsync(url, content);
                //确保HTTP成功状态值 签名失败403
                //response.EnsureSuccessStatusCode();
                //await异步读取最后的JSON
                returnValue = await response.Content.ReadAsStringAsync();
            }
            var responseRes = JsonConvert.DeserializeObject<ApiResponseResult<CloudApiReturnDto>>(returnValue);
            if (responseRes.IsSuccess)
            {
               
                input.AccessKey = responseRes.Data.InKey.accessKey;
                input.SecretKey = responseRes.Data.InKey.secretKey;
                //var cloud = _cloudRepository.Get(input.id);
                cloud.AccessKey = input.AccessKey;
                cloud.SecretKey = input.SecretKey;
                cloud.CloudId = responseRes.Data.InKey.id;
                await _cloudRepository.UpdateAsync(cloud);
            }
            else
            {
                throw new UserFriendlyException((int)UserFriendlyExceptionCode.Info,"获取授权码失败！请稍后重试");
            }
            await _unitOfWorkManager.Current.SaveChangesAsync();
            return input;
        }
    }
}
