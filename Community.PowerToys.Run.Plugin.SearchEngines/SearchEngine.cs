namespace Community.PowerToys.Run.Plugin.SearchEngines
{
    /// <summary>
    /// Class to represent a Search Engine
    /// </summary>
    public class SearchEngine
    {

        /// <summary>
        /// Display Name of the Search Engine
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// URL to perform the search
        /// </summary>
        public required string Url { get; set; }

        /// <summary>
        /// Shortcut keywords to trigger the search
        /// </summary>
        public required string Shortcut { get; set; }

    }

    /// <summary>
    /// The class to contain the collection of Search Engines
    /// </summary>
    public static class SearchEngineCollection
    {
        /// <summary>
        /// The Collection of Search Engines
        /// </summary>
        public static readonly List<SearchEngine> SearchEngines =
                [
            new SearchEngine { Name = "Google", Url = "https://www.google.com/search?q=%s", Shortcut = "google" },
            new SearchEngine { Name = "Bing", Url = "https://www.bing.com/search?q=%s", Shortcut = "bing" },
            new SearchEngine { Name = "GitHub", Url = "https://github.com/search?q=%s&ref=opensearch", Shortcut = "github" },
            new SearchEngine { Name = "Mozilla Developer Network", Url = "https://developer.mozilla.org/en-US/search?q=%s", Shortcut = "mdn" },
            new SearchEngine { Name = "GitHub Docs", Url = "https://docs.github.com/en/search?query=%s", Shortcut = "ghdocs" },
        ];
    }
}
