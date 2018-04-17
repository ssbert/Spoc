using Abp.Web.Mvc.Views;

namespace SPOC.Web.Views
{
    public abstract class SPOCWebViewPageBase : SPOCWebViewPageBase<dynamic>
    {

    }

    public abstract class SPOCWebViewPageBase<TModel> : AbpWebViewPage<TModel>
    {
        protected SPOCWebViewPageBase()
        {
            LocalizationSourceName = SPOCConsts.LocalizationSourceName;
        }
    }
}