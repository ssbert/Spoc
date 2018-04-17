using Abp.Application.Services;
using Abp.Dependency;
using SPOC.Common.Tools;
using SPOC.SysSetting;
using SPOC.SysSetting.SiteSetDTO;

namespace SPOC
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class SPOCAppServiceBase : ApplicationService
    {
        protected SPOCAppServiceBase()
        {
            LocalizationSourceName = SPOCConsts.LocalizationSourceName;
        }

        public SiteSetDto BaseSiteSetDto
        {
            get
            {
                if (CacheStrategy.Get("SiteSet") != null)
                {
                    return (SiteSetDto)CacheStrategy.Get("SiteSet") ?? BaseSiteSetDto;
                }
                //从容器获取实现业务类
                var lpset = IocManager.Instance.Resolve<ISiteSetService>();
                var site = lpset.GetAllSiteSet();
                CacheStrategy.Remove("SiteSet");
                CacheStrategy.Insert("SiteSet", site, 3600);
                return site;
            }
        }
    }
}