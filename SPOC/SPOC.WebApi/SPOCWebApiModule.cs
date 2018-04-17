using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using Abp.Application.Services;
using Abp.Configuration.Startup;
using Abp.Json;
using Abp.Modules;
using Abp.Timing;
using Abp.WebApi;
using Newtonsoft.Json.Converters;
using Swashbuckle.Application;

namespace SPOC
{
    [DependsOn(typeof(AbpWebApiModule), typeof(SPOCApplicationModule))]
    public class SPOCWebApiModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
          
            //动态API过滤后缀
            ApplicationService.CommonPostfixes = new []{ "Service" };
            Configuration.Modules.AbpWebApi().DynamicApiControllerBuilder
                .ForAll<IApplicationService>(typeof(SPOCApplicationModule).Assembly, "app")
                .Build();
            //配置输出json时不使用骆驼式命名法，按对象属性原名输出  已在AbpWebApiModule修改为默认格式此处不需设置
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();

            //Json时间格式化
            
            //Clock.Provider = ClockProviders.Local;
            //Configuration.Modules.AbpWebApi().HttpConfiguration.Formatters.JsonFormatter.SerializerSettings.Converters
            //    .Insert(0,new AbpDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
            //var converters = Configuration.Modules.AbpWebApi().HttpConfiguration.Formatters.JsonFormatter.SerializerSettings.Converters;
            //foreach (var converter in converters)
            //{
            //    if (converter is AbpDateTimeConverter)
            //    {
            //        var tmpConverter = converter as AbpDateTimeConverter;
            //        tmpConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            //    }
            //}
            //GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            ConfigureSwaggerUi();

        }
        /// <summary>
        /// 配置SwaggerUi
        /// </summary>
        private void ConfigureSwaggerUi()
        {
            Configuration.Modules.AbpWebApi().HttpConfiguration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "SPOC_API文档");
                    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                    var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                    var commentsFileName = "Bin//SPOC.Application.xml";
                    var commentsFile = Path.Combine(baseDirectory, commentsFileName);
                    //将注释的XML文档添加到SwaggerUI中
                    c.IncludeXmlComments(commentsFile);
                })
                .EnableSwaggerUi();
        }
    }
}
