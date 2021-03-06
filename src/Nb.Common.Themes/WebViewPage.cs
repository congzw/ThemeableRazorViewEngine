﻿namespace Nb.Common.Themes
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
                _themableLayoutValue = ThemeLayoutHelper.Factory().GetLayoutPath(layout, this.ViewContext);
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