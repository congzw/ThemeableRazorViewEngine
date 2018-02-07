using System.Web.Mvc;
using Nb.Common.Themes;

namespace DemoMvc {
    public static class ThemeConfig
    {
        public static void RegisterViewEngines(ViewEngineCollection engines)
        {
            //WebViewPageConfig.CdnEnabled = false;
            //WebViewPageConfig.ThemeEnabled = true;
            //ThemeLogger.Enabled = true;
            
            //replace
            engines.Clear();
            engines.Add(new ThemeableRazorViewEngine());

            //not replace
            //engines.Insert(0, new ThemeableRazorViewEngine());
        }
    }
}
