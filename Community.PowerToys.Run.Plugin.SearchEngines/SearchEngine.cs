namespace Community.PowerToys.Run.Plugin.SearchEngines
{
    /// <summary>
    /// Class to represent a Search Engine
    /// </summary>
    public class SearchEngine {

        /// <summary>
        /// Display Name of the Search Engine
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// URL to perform the search
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// Shortcut keywords to trigger the search
        /// </summary>
        public string? Shortcut { get; set; }

    }
}
