using System.Data.Entity;
using System.Reflection;
using Abp.EntityFramework;
using Abp.Modules;
using SPOC.EntityFramework;

namespace SPOC
{
    [DependsOn(typeof(AbpEntityFrameworkModule), typeof(SPOCCoreModule))]
    public class SPOCDataModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = "Default";
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            //Database.SetInitializer<SPOCDbContext>(null);
            Database.SetInitializer<SPOCDbContext>(new CreateDatabaseIfNotExists<SPOCDbContext>());
        }
    }
}
