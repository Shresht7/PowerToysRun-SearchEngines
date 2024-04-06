// Library
using System.IO;
using System.Reflection;
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
    public class Main : IPlugin, IDisposable
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

        /// <summary>
        /// The Directory where the Plugin resides
        /// </summary>
        public static string PluginDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

        /// <summary>
        /// Whether the Plugin has been disposed
        /// </summary>
        private bool _disposed;

        #endregion

        #region Query

        /// <summary>
        /// The collection of Search Engines
        /// </summary>
        private List<SearchEngine> SearchEngines = SearchEngineCollection.PredefinedSearchEngines;

        /// <summary>
        /// Returns a filtered list of results based on the given query
        /// </summary>
        /// <param name="query">The input query provided by the user</param>
        /// <returns>A filtered list of results. Can be empty if nothing is found.</returns>
        public List<Result> Query(Query query)
        {
            // Ensure that the query is not null
            ArgumentNullException.ThrowIfNull(query);

            // Initialize the list of results
            List<Result> results = [];

            // string FirstSearch = query.FirstSearch;
            // string SecondToEndSearch = query.SecondToEndSearch;
            // query.FirstSearch and query.SecondToEndSearch do not behave as expected, so we use the following code instead

            // Parse the search query
            string FirstSearch = query.Terms[0];
            string SecondToEndSearch = query.Search[FirstSearch.Length..].Trim();
            string searchQuery = query.Search; // We create a new variable so that we can modify it later to remove the search engine shortcut
            string encodedSearchQuery = System.Net.WebUtility.UrlEncode(searchQuery); // Encode the search query to be used in the URL

            // Show a result for each search engine
            foreach (var SearchEngine in SearchEngines)
            {
                // Ensure that engine.URL and engine.Shortcut is not null
                if (string.IsNullOrEmpty(SearchEngine.Url) || string.IsNullOrEmpty(SearchEngine.Shortcut))
                {
                    continue; // Skip this search engine if the URL is invalid
                }

                // Perform a fuzzy search to determine if the query starts with a search engine shortcut
                MatchResult matchResults = StringMatcher.FuzzySearch(FirstSearch, SearchEngine.Shortcut);

                // If the match was successful...
                if (matchResults.Success)
                {
                    // ... remove the search engine shortcut from the search query and encode it
                    searchQuery = SecondToEndSearch;
                    encodedSearchQuery = System.Net.WebUtility.UrlEncode(searchQuery);
                }
                else
                {
                    // ...otherwise, skip this search engine
                    continue;
                }

                // Generate Results for this Search Engine
                results.Add(new Result
                {
                    QueryTextDisplay = query.Search,
                    Title = string.IsNullOrEmpty(searchQuery) ? SearchEngine.Name : searchQuery,
                    SubTitle = $"Search {SearchEngine.Name}",
                    IcoPath = IconPath,
                    Score = matchResults.Score,
                    Action = e =>
                    {
                        // Replace the search query in the URL
                        string url = SearchEngine.Url.Replace("%s", encodedSearchQuery);

                        // Ensure that search URL is valid
                        if (string.IsNullOrEmpty(url) && !Uri.IsWellFormedUriString(url, UriKind.Absolute))
                        {
                            Log.Error($"Plugin: {Name}\nInvalid URL: {url}", GetType());
                            Context?.API.ShowMsg($"Plugin: {Name}", $"Invalid URL: {url}");
                            return false;
                        }

                        // Open the search engine in the default browser
                        return OpenInBrowser(url);
                    }
                });
            }

            // Return the list of results
            return results;
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

            // Load the Search Engines from the Configuration File
            SearchEngines = SearchEngineCollection.Load();
        }

        /// <summary>
        /// OnThemeChanged event handler. Called when the theme is changed.
        /// </summary>
        /// <param name="theme">The new theme</param>
        private void OnThemeChanged(Theme oldTheme, Theme newTheme)
        {
            UpdateIconPath(newTheme);
        }

        /// <summary>
        /// Dispose the Plugin and release any resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the Plugin and release any resources
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called
            if (!_disposed && disposing)
            {
                // Unsubscribe from the OnThemeChanged event
                if (Context != null && Context.API != null)
                {
                    Context.API.ThemeChanged -= OnThemeChanged;
                }
                // Set the disposed flag to true
                _disposed = true;
            }
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
                Log.Error($"Plugin: {Name}\nCannot open {BrowserInfo.Path} with arguments {BrowserInfo.ArgumentsPattern} {url}", GetType());
                Context?.API.ShowMsg($"Plugin: {Name}", "Open default browser failed.");
                return false;
            }
            return true;
        }

        #endregion

    }
}
