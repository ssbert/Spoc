using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using Abp.Castle.Logging.Log4Net;
using Abp.Dependency;
using Abp.Events.Bus;
using Abp.Events.Bus.Exceptions;
using Abp.Logging;
using Abp.Web.Models;
using Castle.Core.Logging;

namespace SPOC.Web.Filters
{
    public class CustomExceptionAttribute : HandleErrorAttribute, ITransientDependency
    {
        public ILogger Logger { get; set; }
        //public IEventBus EventBus { get; set; }
        public CustomExceptionAttribute()
        {
            Logger = NullLogger.Instance;
           // EventBus = NullEventBus.Instance;
        }
   
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled == false)
            {

                try
                {
                   
                    LogHelper.LogException(Logger, filterContext.Exception);
                    //EventBus.Trigger(this, new AbpHandledExceptionData(filterContext.Exception));
                }
                catch
                {
                    // ignored
                }
            }
            filterContext.RequestContext.HttpContext.Response.Redirect("~/home/error");
            filterContext.ExceptionHandled = true;
        }
    }
}