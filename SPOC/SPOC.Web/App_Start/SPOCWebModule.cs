using System;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Modules;
using Abp.Web.Mvc;
using SPOC.Web.Filters;

namespace SPOC.Web
{
    [DependsOn(
        typeof(AbpWebMvcModule),
        typeof(SPOCDataModule), 
        typeof(SPOCApplicationModule), 
        typeof(SPOCWebApiModule))]
    public class SPOCWebModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Add/remove languages for your application
            Configuration.Localization.Languages.Add(new LanguageInfo("en", "English", "famfamfam-flag-england", true));
            Configuration.Localization.Languages.Add(new LanguageInfo("tr", "Türkçe", "famfamfam-flag-tr"));
            Configuration.Localization.Languages.Add(new LanguageInfo("zh-CN", "简体中文", "famfamfam-flag-cn"));
            Configuration.Localization.Languages.Add(new LanguageInfo("ja", "日本語", "famfamfam-flag-jp"));

            //Add/remove localization sources here
            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    SPOCConsts.LocalizationSourceName,
                    new XmlFileLocalizationDictionaryProvider(
                        HttpContext.Current.Server.MapPath("~/Localization/SPOC")
                        )
                    )
                );

            //Configure navigation/menu
            Configuration.Navigation.Providers.Add<SPOCNavigationProvider>();

            Configuration.Modules.AbpWeb().AntiForgery.IsEnabled = false;

            //配置指定的Cache过期时间为30分钟
            Configuration.Caching.Configure("ControllerCache", cache =>
            {
                cache.DefaultSlidingExpireTime = TimeSpan.FromMinutes(30);
            });

          
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            GlobalConfiguration.Configuration.MapHttpAttributeRoutes(); //自定义路由
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalFilters.Filters.Add(IocManager.Resolve<CustomExceptionAttribute>());
        }
    }
}
