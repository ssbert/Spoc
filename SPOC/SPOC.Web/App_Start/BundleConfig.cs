using System.Web.Optimization;

namespace SPOC.Web
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();

            #region  后台管理
            
            //EasyUI
            bundles.Add(new StyleBundle("~/bundles/easyuicss").Include(
                "~/Scripts/jquery-easyui-1.5.4.4/themes/bootstrap/easyui.css",
                "~/Scripts/jquery-easyui-icon-extension/IconExtension.css",
                "~/Scripts/jquery-easyui-1.5.4.4/themes/icon.css",
                "~/Scripts/jquery-easyui-extension/icon.css",
                "~/Content/themes/base/ext-easyui.css",
                "~/Content/themes/base/default.css",
                "~/Scripts/jquery-easyui-extension/Style.css"));

            bundles.Add(new ScriptBundle("~/bundles/easyuijs").Include(
                "~/Scripts/jquery-easyui-1.5.4.4/jquery.min.js",
                "~/Scripts/jquery-easyui-1.5.4.4/jquery.easyui.min.js",
                "~/Scripts/jquery-easyui-1.5.4.4/locale/easyui-lang-zh_CN.js",
                "~/Scripts/jquery-easyui-1.5.4.4/datagrid-detailview.js",
                "~/Scripts/jquery-easyui-extension/VEasyUI.js",
                "~/Scripts/jquery-easyui-extension/datagrid-ext.js",
                "~/Scripts/jquery-easyui-extension/jquery.json-2.4.min.js"));

            //自定义样式表
            bundles.Add(new StyleBundle("~/bundles/commoncss").Include(
                "~/Content/themes/base/form.css"));
            //自定义js
            bundles.Add(new ScriptBundle("~/bundles/commonjs").Include(
                "~/js/common/common.js",
                "~/js/common/evtBus.js"
            ));

       
            //~/Bundles/GlobalJs
            bundles.Add(
                new ScriptBundle("~/Bundles/GlobalJs")
                    .Include(
                        "~/Scripts/jquery-easyui-extension/datagrid-ext.js"
                    )
            );

            #endregion

            #region 前台展示

//#if DEBUG
            //调试模式下脚本
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery.unobtrusive-ajax.js",
                "~/Scripts/layer/layer.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate.js",
                "~/Scripts/jquery.validate.messages_zh.js",
                "~/Scripts/jquery.validate.unobtrusive.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js",
                "~/Scripts/respond.js"));
            //css
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css"));
//#else
//            //发布模式下脚本
//            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
//                "~/Scripts/jquery-{version}.min.js",
//                "~/Scripts/jquery.unobtrusive-ajax.min.js",
//                "~/Scripts/layer/layer.js"));

//            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
//                "~/Scripts/jquery.validate.min.js",
//                "~/Scripts/jquery.validate.messages_zh.min.js",
//                "~/Scripts/jquery.validate.unobtrusive.min.js"));

//            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
//                "~/Scripts/bootstrap.min.js",
//                "~/Scripts/respond.min.js"));
//            //css
//            bundles.Add(new StyleBundle("~/Content/css").Include(
//                "~/Content/bootstrap.min.css"));
//#endif

            bundles.Add(new ScriptBundle("~/bundles/rsa").Include(
                "~/Scripts/rsa/BigInt.js",
                "~/Scripts/rsa/Barrett.js",
                "~/Scripts/rsa/RSA.js"));

            #endregion
        }
    }
}