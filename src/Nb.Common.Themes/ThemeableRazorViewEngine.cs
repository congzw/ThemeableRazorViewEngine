using System;
using System.Linq;
using System.Web.Mvc;

namespace Nb.Common.Themes
{
    public class ThemeableRazorViewEngine : RazorViewEngine
    {
        #region IThemePath

        private readonly string _themeFolderVirutalPath;
        private static readonly string _themePlaceholder = "#THEME#";
        private string ProcessThemePath(string virtualPath, string themeName)
        {
            return virtualPath.Replace(_themePlaceholder, themeName);
        }

        #endregion

        private readonly IThemeManager _themeManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ThemeableRazorViewEngine" /> class.
        /// </summary>
        public ThemeableRazorViewEngine()
            : this("~/Themes", ThemeManager.Resolve(), new[] { "cshtml" })
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ThemeableRazorViewEngine" /> class.
        /// </summary>
        public ThemeableRazorViewEngine(string themeFolderVirutalPath, IThemeManager themeManager, params string[] fileExtensions)
        {
            if (string.IsNullOrWhiteSpace(themeFolderVirutalPath))
            {
                throw new ArgumentNullException("themeFolderVirutalPath");
            }
            if (!themeFolderVirutalPath.StartsWith("~") || themeFolderVirutalPath.EndsWith("/"))
            {
                throw new ArgumentException("themeFolderVirutalPath must start with '~' and not end with '/'");
            }
            _themeFolderVirutalPath = themeFolderVirutalPath;

            if (themeManager == null)
            {
                throw new ArgumentNullException("themeManager");
            }
            _themeManager = themeManager;
            
            if (fileExtensions == null || fileExtensions.Length == 0)
            {
                throw new ArgumentException("themeFolderVirutalPath must start with '~' and not end with '/'");
            }
            FileExtensions = fileExtensions;

            string[] areaLocationFormats = CreateLocationFormats(LocationType.Area);
            AreaViewLocationFormats = areaLocationFormats;
            AreaMasterLocationFormats = areaLocationFormats;
            AreaPartialViewLocationFormats = areaLocationFormats;

            string[] defaultLocationFormats = CreateLocationFormats(LocationType.Default);
            ViewLocationFormats = defaultLocationFormats;
            MasterLocationFormats = defaultLocationFormats;
            PartialViewLocationFormats = defaultLocationFormats;
        }

        #region private members

        private string[] CreateLocationFormats(LocationType locationType)
        {
            string prefix = GetPrefixByLocationType(locationType);
            var prefixWithTheme = AppendPrefixWithTheme(_themeFolderVirutalPath, prefix);
            return FileExtensions
                .SelectMany(extension => new[]
                {
                    string.Format("{0}/Views/{{1}}/{{0}}.{1}", prefixWithTheme, extension),
                    string.Format("{0}/Views/Shared/{{0}}.{1}", prefixWithTheme, extension),
                    string.Format("~{0}/Views/{{1}}/{{0}}.{1}", prefix, extension),
                    string.Format("~{0}/Views/Shared/{{0}}.{1}", prefix, extension)
                })
                .ToArray();
        }

        private static string AppendPrefixWithTheme(string themeFolderVirtualPath, string prefix)
        {
            //[~/Themes/#THEME#]/Areas/{2}/Views/{1}/{0}.cshtml
            //[~/Themes/#THEME#]/Areas/{2}/Views/Shared/{0}.cshtml
            //[~/Themes/#THEME#]/Views/{1}/{0}.cshtml
            //[~/Themes/#THEME#]/Views/Shared/{0}.cshtml

            //append [~/Themes/#THEME#]
            var safeThemeVirtualPath = themeFolderVirtualPath + '/' + _themePlaceholder;
            return safeThemeVirtualPath + prefix;
        }

        private static string GetPrefixByLocationType(LocationType locationType)
        {
            //=> /Areas/{2}/Views/{1}/{0}.cshtml
            //=> /Areas/{2}/Views/Shared/{0}.cshtml
            //=> /Views/{1}/{0}.cshtml
            //=> /Views/Shared/{0}.cshtml

            string prefix = null;
            switch (locationType)
            {
                case LocationType.Area:
                    prefix = "/Areas/{2}";
                    break;
                case LocationType.Default:
                    prefix = "";
                    break;
            }
            return prefix;
        }

        private enum LocationType
        {
            Default,
            Area
        }

        /// <summary>
        ///     Creates a partial view using the specified controller context and partial path, including the Theme name.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="partialPath">The partial path.</param>
        /// <param name="themeName">Name of the theme.</param>
        /// <returns></returns>
        private IView CreateThemePartialView(ControllerContext controllerContext, string partialPath, string themeName)
        {
            string themePath = ProcessThemePath(partialPath, themeName);
            return base.CreatePartialView(controllerContext, themePath);
        }

        /// <summary>
        ///     Creates a view by using the specified controller context and the paths of the view and master view, including the
        ///     Theme name.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="viewPath">The view path.</param>
        /// <param name="masterPath">The master path.</param>
        /// <param name="themeName">Name of the theme.</param>
        /// <returns></returns>
        private IView CreateThemeView(ControllerContext controllerContext, string viewPath, string masterPath, string themeName)
        {
            string themeViewPath = ProcessThemePath(viewPath, themeName);
            
            //todo process layout path
            string themeMasterPath = ProcessThemePath(masterPath, themeName);
            
            return base.CreateView(controllerContext, themeViewPath, themeMasterPath);
        }

        private string GetSafeThemeName()
        {
            var theme = _themeManager.GetTheme();
            if (string.IsNullOrWhiteSpace(theme))
            {
                return string.Empty;
            }
            return theme;
        }

        #endregion

        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            var safeThemeName = GetSafeThemeName();
            if (!string.IsNullOrWhiteSpace(safeThemeName))
            {
                var processThemePath = ProcessThemePath(virtualPath, safeThemeName);
                var fileExists = base.FileExists(controllerContext, processThemePath);
                Log(string.Format("FileExist: {0} => {1}", processThemePath, fileExists));
                return fileExists;
            }
            return base.FileExists(controllerContext, virtualPath);
        }

        /// <summary>
        ///     Creates a partial view using the specified controller context and partial path.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="partialPath">The path to the partial view.</param>
        /// <returns>
        ///     The partial view.
        /// </returns>
        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            var safeThemeName = GetSafeThemeName();
            if (!string.IsNullOrWhiteSpace(safeThemeName))
            {
                return CreateThemePartialView(controllerContext, partialPath, safeThemeName);
            }

            return base.CreatePartialView(controllerContext, partialPath);
        }

        /// <summary>
        ///     Creates a view by using the specified controller context and the paths of the view and master view.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="viewPath">The path to the view.</param>
        /// <param name="masterPath">The path to the master view.</param>
        /// <returns>
        ///     The view.
        /// </returns>
        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            var safeThemeName = GetSafeThemeName();
            if (!string.IsNullOrWhiteSpace(safeThemeName))
            {
                return CreateThemeView(controllerContext, viewPath, masterPath, safeThemeName);
            }

            return base.CreateView(controllerContext, viewPath, masterPath);
        }

        private static void Log(object message)
        {
            ThemeLogger.Log(message);
        }
    }
}
