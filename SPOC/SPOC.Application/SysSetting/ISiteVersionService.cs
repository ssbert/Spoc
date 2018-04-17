using Abp.Application.Services;
using SPOC.Common.EasyUI;
using SPOC.SysSetting.SiteVersionDto;
using SPOC.SystemSet;

namespace SPOC.SysSetting
{
   public interface ISiteVersionService: IApplicationService
    {
        void Save(Site site);
        EasyUiListResultDto<SiteVersion> GetAllSiteVersion(SiteVersionInputDto input);
       void ModifySiteVersion(SiteVersionInputDto input);
       SiteVersionInputDto GetSiteVersionById(string id);

       APPSiteVersionDto GetSiteVersionByCreateTime(string platform="");

    }
}
