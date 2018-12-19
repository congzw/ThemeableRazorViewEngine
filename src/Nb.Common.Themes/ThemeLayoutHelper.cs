using System;
using System.IO;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nb.Common.Themes
{
    public interface IThemeLayoutHelper
    {
        /// <summary>
        /// 获取布局页面的路径
        /// </summary>
        /// <param name="layoutPath"></param>
        /// <param name="viewContext"></param>
        /// <returns></returns>
        string GetLayoutPath(string layoutPath, ViewContext viewContext);
    }

    public class ThemeLayoutHelper : IThemeLayoutHelper
    {
        public string GetLayoutPath(string layoutPath, ViewContext viewContext)
        {
            var layoutName = GuessLayoutName(layoutPath, viewContext.RouteData);
            var viewResult = ViewEngines.Engines.FindPartialView(viewContext.Controller.ControllerContext, layoutName);

            var view = viewResult.View as RazorView;
            if (view == null)
            {
                ThemeLogger.Log(string.Format("Process Layout: {0} => {1}", layoutPath, "EMPTY!"));
                return string.Empty;
            }

            ThemeLogger.Log(string.Format("Process Layout: {0} => {1}", layoutPath, view.ViewPath));
            return view.ViewPath;
        }

        private static string GuessLayoutName(string layoutPath, RouteData routeData)
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
            var area = TryGetAreaName(routeData);
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
        private static string TryGetAreaName(RouteData routeData)
        {
            //HttpContext.Current.Request.RequestContext.RouteData.DataTokens["area"]
            object area;
            if (routeData.DataTokens.TryGetValue("area", out area))
            {
                return area as string;
            }
            return null;
        }

        #region for di extensions

        private static readonly IThemeLayoutHelper _instance = new ThemeLayoutHelper();
        public static Func<IThemeLayoutHelper> Factory = new Func<IThemeLayoutHelper>(() => _instance);

        #endregion
    }
}
