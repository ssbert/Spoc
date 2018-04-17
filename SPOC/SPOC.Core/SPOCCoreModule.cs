using System.Reflection;
using Abp.Modules;

namespace SPOC
{
    public class SPOCCoreModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
