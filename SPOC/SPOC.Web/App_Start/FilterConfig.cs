using System.Web.Mvc;
using Abp.Dependency;
using SPOC.Web.Filters;

namespace SPOC.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CustomExceptionAttribute());
            filters.Add(new HandleErrorAttribute());
          
        }
    }
}
