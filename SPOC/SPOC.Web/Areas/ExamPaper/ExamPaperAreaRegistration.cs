using System.Web.Mvc;

namespace SPOC.Web.Areas.ExamPaper
{
    public class ExamPaperAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ExamPaper";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ExamPaper_default",
                "ExamPaper/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}