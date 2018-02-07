using System;
using System.Configuration;
using System.Web;

namespace Nb.Common.Themes
{
    public interface IThemeManager
    {
        /// <summary>
        /// Get Current Theme
        /// </summary>
        /// <returns></returns>
        string GetTheme();
    }

    public class ThemeManager : IThemeManager
    {
        public string GetTheme()
        {
            //todo use multi IThemeSelector impls
            //e.g.：Request => Site => Config(Sg) => Default
            var theme = GetConfigTheme();
            if (HttpContext.Current != null)
            {
                var httpContextWrapper = new HttpContextWrapper(HttpContext.Current);
                var requestTheme = GetRequestTheme(httpContextWrapper);
                if (!string.IsNullOrWhiteSpace(requestTheme))
                {
                    theme = requestTheme;
                }
            }
            return theme;
        }

        #region for di extensions

        private static readonly Lazy<IThemeManager> Lazy = new Lazy<IThemeManager>(() => new ThemeManager());
        private static Func<IThemeManager> _resolve = () => Lazy.Value;
        public static Func<IThemeManager> Resolve
        {
            get { return _resolve; }
            set { _resolve = value; }
        }

        #endregion

        #region helpers
        public const string DefaultThemeParamNameOrConfigKey = "theme";
        private static string _themeNameInConfig = null;
        private static string GetRequestTheme(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                return null;
            }
            var themeName = httpContext.Request.QueryString[DefaultThemeParamNameOrConfigKey];
            return themeName;
        }
        private static string GetConfigTheme()
        {
            return _themeNameInConfig ?? (_themeNameInConfig = ConfigurationManager.AppSettings[DefaultThemeParamNameOrConfigKey]);
        }
        #endregion
    }
}
