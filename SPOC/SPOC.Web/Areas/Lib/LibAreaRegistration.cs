using System.Web.Mvc;

namespace SPOC.Web.Areas.Lib
{
    public class LibAreaRegistration : AreaRegistration 
    {
        public override string AreaName => "Lib";

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Lib_default",
                "Lib/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}