using System;
using System.Linq;
using System.Linq.Dynamic;
using Abp.Application.Services;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Newtonsoft.Json;
using SPOC.Common.EasyUI;
using SPOC.Common.Helper;
using SPOC.SysSetting.SiteVersionDto;
using SPOC.SystemSet;

namespace SPOC.SysSetting
{
    public class SiteVersionService : ApplicationService, ISiteVersionService
    {

        private readonly IRepository<SiteVersion, Guid> _iSiteVersionRepository;
        private readonly IRepository<Site, Guid> _siteRepository;

        public SiteVersionService(IRepository<SiteVersion, Guid> iSiteVersionRepository, IRepository<Site, Guid> siteRepository)
        {
            _iSiteVersionRepository = iSiteVersionRepository;
            _siteRepository = siteRepository;
        }
        public void Save(Site site)
        {
            site.Id = GuidHelper.Init(site.Id);
            _siteRepository.Insert(site);
            Logger.Info("保存-site：" + JsonConvert.SerializeObject(site));
        }
        public EasyUiListResultDto<SiteVersion> GetAllSiteVersion(SiteVersionInputDto input)
        {
            EasyUiListResultDto<SiteVersion> result = new EasyUiListResultDto<SiteVersion>();
            try
            {
                var list = _iSiteVersionRepository.GetAll();
                if (list.Count() == 0 || list == null)
                {
                    return result;
                }

                if (!string.IsNullOrEmpty(input.version)) {
                    list = list.Where(d => d.version != null && d.version.Contains(input.version));
                }

                result.total = list.Count();
                result.rows = list.OrderBy(input.OrderExpression).Skip(input.Skip).Take(input.PageSize).ToList();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }

            return result;
        }

        public void ModifySiteVersion(SiteVersionInputDto input)
        {
            try
            {
                SiteVersion sv = _iSiteVersionRepository.FirstOrDefault(d=>d.Id==input.id);
                sv.isForceUpdate = input.isForceUpdate;
                sv.updateInfo = input.updateInfo;
                sv.url = input.url;
                sv.version = input.version;
                sv.platform = string.IsNullOrEmpty( input.platform)?1:int.Parse(input.platform);
                sv.createtime = DateTime.Now;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }

        }

        public SiteVersionInputDto GetSiteVersionById(string id)
        {
            SiteVersionInputDto dto = new SiteVersionInputDto();
            try
            {
                var siteVersion = _iSiteVersionRepository.FirstOrDefault(d=>d.Id.ToString()== id);
                if (siteVersion == null || string.IsNullOrWhiteSpace(siteVersion.Id.ToString())) {
                    return dto;
                }
                dto.id = siteVersion.Id;
                dto.updateInfo = siteVersion.updateInfo;
                dto.version = siteVersion.version;
                dto.url = siteVersion.url;
                dto.platform = siteVersion.platform==1?"android":"ios";
                dto.isForceUpdate = siteVersion.isForceUpdate;
               
            }
            catch (Exception ex) {
                Logger.Error(ex.ToString());
            }

            return dto;
        }



        #region APP Interface

        public APPSiteVersionDto GetSiteVersionByCreateTime(string platform="")
        {
            APPSiteVersionDto dto = new APPSiteVersionDto();
            try
            {

                var data = _iSiteVersionRepository.GetAll().WhereIf(!string.IsNullOrEmpty(platform) && (platform.ToLower() == "ios" || platform.ToLower() == "andriod"), a => a.platform == (platform.ToLower() == "ios" ? 2 : 1)).FirstOrDefault();
                if (data == null)
                {
                    return dto;
                }
                dto.forceUpdate = data.isForceUpdate == true ? "Y" : "N";
                dto.updateInfo = data.updateInfo;
                dto.url = data.url;
                dto.version = data.version;
            }
            catch (Exception ex) {

                Logger.Error(ex.ToString());
            }

            return dto;
        }
        #endregion
    }
}
