using System.IO;
using System.Web.Mvc;

namespace Nb.Common.Themes
{
    public abstract class WebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {
        public override void ExecutePageHierarchy()
        {
            var showDebugInfo = this.Request.QueryString["showRazorPath"] != null;
            if (showDebugInfo)
            {
                this.WriteLiteral("<!--Begin-ViewPath:" + this.VirtualPath + "-->");
            }
            base.ExecutePageHierarchy();
            if (showDebugInfo)
            {
                this.WriteLiteral("\r\n<!--End-ViewPath:" + this.VirtualPath + "-->");
            }
        }

        private string _themableLayoutValue = null;
        public override string Layout
        {
            get
            {
                if (!WebViewPageConfig.ThemeEnabled)
                {
                    return base.Layout;
                }

                var layout = base.Layout;
                if (string.IsNullOrWhiteSpace(layout))
                {
                    return layout;
                }

                //already processed
                if (_themableLayoutValue != null)
                {
                    return _themableLayoutValue;
                }

                _themableLayoutValue = ProcessLayout(layout);
                return _themableLayoutValue;
            }
            set
            {
                base.Layout = value;
            }
        }

        public override string Href(string path, params object[] pathParts)
        {
            if (!WebViewPageConfig.CdnEnabled)
            {
                return base.Href(path, pathParts);
            }

            //todo cdn process
            //todo theme Href path
            return base.Href(path, pathParts);
            //#region path and pathParts

            ////https://msdn.microsoft.com/zh-cn/library/system.web.webpages.helperpage.href(v=vs.111).aspx
            ////The Href(String, Object[]) method combines the initial path
            ////(such as the application root that is provided in the path parameter) with a list of folders, subfolders, and file names 
            ////that are provided in the pathParts parameter. It returns a fully-qualified URL.
            ////For example, 
            ////if path parameter contains the string "~/Music" and the pathParts array contains the strings "Jazz" and "Saxophone.wav", 
            ////the Href(String, Object[]) method returns the URL http://localhost/MyApp/Music/Jazz/Saxophone.wav.

            //#endregion

            //string cdnPath;
            //var tryResolveCdnPath = CdnServer.Current.TryResolveCDNPath(path, out cdnPath);
            //if (tryResolveCdnPath)
            //{
            //    return cdnPath;
            //}
            //return base.Href(path, pathParts);
        }

        private string ProcessLayout(string layoutPath)
        {
            var layoutName = GuessLayoutName(layoutPath);
            var viewResult = ViewEngines.Engines.FindPartialView(ViewContext.Controller.ControllerContext, layoutName);

            var view = viewResult.View as RazorView;
            if (view == null)
            {
                ThemeLogger.Log(string.Format("Process Layout: {0} => {1}", layoutPath, "EMPTY!"));
                return string.Empty;
            }

            ThemeLogger.Log(string.Format("Process Layout: {0} => {1}", layoutPath, view.ViewPath));
            return view.ViewPath;
        }

        private string GuessLayoutName(string layoutPath)
        {
            if (string.IsNullOrWhiteSpace(layoutPath))
            {
                return string.Empty;
            }
            //fix bugs: "~/Views/Shared/_Unify/_Layout.cshtml" from "_Layout" to "_Unify/_Layout"
            //fix bugs: "~/Areas/Admin/Views/Shared/_Layout.cshtml" from "~/areas/admin/views/shared/_layout" to "_Layout"
            var lowerLayoutPath = layoutPath.ToLower();
            var layoutName = Path.GetFileNameWithoutExtension(lowerLayoutPath);
            var fileName = Path.GetFileName(lowerLayoutPath);
            //~/areas/admin/views/shared/
            var areaLower = "";
            var area = TryGetAreaName(this.ViewContext);
            if (!string.IsNullOrWhiteSpace(area))
            {
                areaLower = area.ToLower();
            }
            var directoryName = lowerLayoutPath
                .Replace(@"~/views/shared/", "")
                .Replace(string.Format(@"~/areas/{0}/views/shared/", areaLower), "")
                .Replace(fileName, "");
            return directoryName + layoutName;
        }

        private static string TryGetAreaName(ViewContext viewContext)
        {
            //HttpContext.Current.Request.RequestContext.RouteData.DataTokens["area"]
            object area;
            if (viewContext.RouteData.DataTokens.TryGetValue("area", out area))
            {
                return area as string;
            }
            return null;
        }
    }

    public abstract class WebViewPage : WebViewPage<dynamic>
    {
    }

    public class WebViewPageConfig
    {
        static WebViewPageConfig()
        {
            ThemeEnabled = true;
            CdnEnabled = false;
        }
        public static bool ThemeEnabled { get; set; }
        public static bool CdnEnabled { get; set; }
    }
}