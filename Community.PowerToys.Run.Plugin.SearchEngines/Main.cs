// Library
using ManagedCommon;
using Wox.Infrastructure;
using Wox.Plugin;
using Wox.Plugin.Logger;
using BrowserInfo = Wox.Plugin.Common.DefaultBrowserInfo;

namespace Community.PowerToys.Run.Plugin.SearchEngines
{
    /// <summary>
    /// Main class for the Search Engines Plugin. Implements the <see cref="IPlugin"/> interface.
    /// </summary>
    public class Main : IPlugin
    {

        #region Metadata

        /// <summary>
        /// ID of the Plugin
        /// </summary>
        public static string PluginID => "FF7C33A486FC4C619AE002CECA7571F8";

        /// <summary>
        /// Name of the Plugin
        /// </summary>
        public string Name => "Search Engines";

        /// <summary>
        /// Description of the Plugin
        /// </summary>
        public string Description => "Perform a search using various search engines";

        /// <summary>
        /// Path to the Icon for the Plugin
        /// </summary>
        private string? IconPath { get; set; }

        /// <summary>
        /// Update the IconPath based on the current theme of the application.
        /// </summary>
        /// <param name="theme">The current theme of the application</param>
        private void UpdateIconPath(Theme theme)
        {
            IconPath = theme switch
            {
                Theme.Light => "Images\\icon.light.png",
                Theme.HighContrastWhite => "Images\\icon.light.png",
                Theme.Dark => "Images\\icon.dark.png",
                Theme.HighContrastBlack => "Images\\icon.dark.png",
                Theme.HighContrastOne => "Images\\icon.dark.png",
                Theme.HighContrastTwo => "Images\\icon.dark.png",
                _ => "Images\\icon.dark.png",
            };
        }

        #endregion

        #region Query

        /// <summary>
        /// The collection of Search Engines
        /// </summary>
        private readonly List<SearchEngine> SearchEngines =
        [
            new SearchEngine { Name = "Google", Url = "https://www.google.com/search?q=", Shortcut = "google" },
            new SearchEngine { Name = "Bing", Url = "https://www.bing.com/search?q=", Shortcut = "bing" },
        ];

        /// <summary>
        /// Returns a filtered list of results based on the given query
        /// </summary>
        /// <param name="query">The input query provided by the user</param>
        /// <returns>A filtered list of results. Can be empty if nothing is found.</returns>
        public List<Result> Query(Query query)
        {
            // Show a result for each search engine
            return SearchEngines
                .Select(engine => new Result
                {
                    Title = engine.Name,
                    SubTitle = $"Search {engine.Name} for '{query.Search}'",
                    IcoPath = IconPath,
                    Action = e =>
                    {
                        // Open the search engine in the default browser
                        OpenInBrowser(engine.Url + query.Search);
                        return true;
                    }
                })
                .ToList();
        }

        #endregion

        #region Plugin Lifecycle

        /// <summary>
        /// Plugin Initialization Context
        /// </summary>
        private PluginInitContext? Context { get; set; }

        /// <summary>
        /// Initialize the plugin with the given <see cref="PluginInitContext"/>
        /// </summary>
        /// <param name="context">The <see cref="PluginInitContext"/> for this plugin.</param>
        public void Init(PluginInitContext context)
        {
            // Save the plugin initialization context for the future
            Context = context ?? throw new ArgumentNullException(nameof(context));
            // Subscribe to the OnThemeChanged event
            context.API.ThemeChanged += OnThemeChanged;
            // Update the icon path based on the current theme
            UpdateIconPath(context.API.GetCurrentTheme());
        }

        /// <summary>
        /// OnThemeChanged event handler. Called when the theme is changed.
        /// </summary>
        /// <param name="theme">The new theme</param>
        private void OnThemeChanged(Theme oldTheme, Theme newTheme)
        {
            UpdateIconPath(newTheme);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Open the given URL in the default browser
        /// </summary>
        /// <param name="url">The URL to open</param>
        /// <returns>Whether the operation was successful</returns>
        private bool OpenInBrowser(string url)
        {
            if (!Helper.OpenCommandInShell(BrowserInfo.Path, BrowserInfo.ArgumentsPattern, url))
            {
                Log.Error($"Plugin: {Name}\nCannot open {BrowserInfo.Path} with arguments {BrowserInfo.ArgumentsPattern} {url}", typeof(SearchEngine));
                return false;
            }
            return true;
        }

        #endregion

    }
}
