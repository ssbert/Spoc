using System.Web.Mvc;

namespace SPOC.Web.Areas.QuestionBank
{
    public class QuestionBankAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "QuestionBank";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "QuestionBank_default",
                "QuestionBank/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}