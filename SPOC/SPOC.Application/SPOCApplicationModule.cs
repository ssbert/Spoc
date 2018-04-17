using System.Reflection;
using Abp.AutoMapper;
using Abp.Modules;

namespace SPOC
{
    [DependsOn(typeof(SPOCCoreModule),(typeof(AbpAutoMapperModule)))]
    public class SPOCApplicationModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
