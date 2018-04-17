using System;
using System.Collections.Generic;
using Abp.Application.Services;
using SPOC.SysSetting.SiteSetDTO;
using SPOC.SystemSet;
using SPOC.User;

namespace SPOC.SysSetting
{
    public interface ISiteSetService : IApplicationService
    {
        SiteSetDto GetAllSiteSet();
        UserBase GetUserSessionId(Guid userId);
        void ModifySiteSet(List<SiteSetInputDto> input);

        //添加站点配置信息 
        void InsertSiteSet(SiteSetInputDto input);

      
    }
}
