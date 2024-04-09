namespace Community.PowerToys.Run.Plugin.SearchEngines
{
    /// <summary>
    /// Models a search engine
    /// </summary>
    public class SearchEngine
    {

        /// <summary>
        /// Display name of the search engine
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

        /// <summary>
        /// Check if the search engine object is valid
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Url) && !string.IsNullOrEmpty(Shortcut);
        }

    }

}
