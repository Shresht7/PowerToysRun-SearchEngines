// Library
using System.IO;
using System.Text.Json;
using Wox.Plugin.Logger;

namespace Community.PowerToys.Run.Plugin.SearchEngines
{

    /// <summary>
    /// The class responsible for loading and saving the search engines configuration
    /// </summary>
    public static class Config
    {

        /// <summary>
        /// The path to the json configuration file that holds information about the search engines
        /// </summary>
        public static readonly string FilePath = Path.Combine(
            Main.PluginDirectory,
            "..", // Plugins
            "..", // PowerToys Run
            "Settings",
            "Plugins",
            "Community.PowerToys.Run.Plugin.SearchEngines",
            "SearchEngines.json"
        );

        /// <summary>
        /// A default collection of search engines
        /// </summary>
        public static readonly List<SearchEngine> PredefinedSearchEngines =
                [
            new SearchEngine { Name = "Google", Url = "https://www.google.com/search?q=%s", Shortcut = "google" },
            new SearchEngine { Name = "Bing", Url = "https://www.bing.com/search?q=%s", Shortcut = "bing" },
            new SearchEngine { Name = "GitHub", Url = "https://github.com/search?q=%s&ref=opensearch", Shortcut = "github" },
            new SearchEngine { Name = "Mozilla Developer Network", Url = "https://developer.mozilla.org/en-US/search?q=%s", Shortcut = "mdn" },
            new SearchEngine { Name = "YouTube", Url = "https://www.youtube.com/results?search_query=%s&page={startPage?}&utm_source=opensearch", Shortcut = "yt" },
            new SearchEngine { Name = "Wikipedia", Url = "https://en.wikipedia.org/w/index.php?title=Special:Search&search=%s", Shortcut = "wikipedia" },
            new SearchEngine { Name = "Wolfram Alpha", Url = "http://www.wolframalpha.com/input/?i=%s", Shortcut = "wolfram" },
        ];

        /// <summary>
        /// Load the search engines from the configuration file
        /// </summary>
        /// <returns>A list of <see cref="SearchEngine"/>s</returns>
        public static List<SearchEngine> Load()
        {
            // Create the configuration directory if it does not exist
            string? directory = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Log.Error($"The configuration directory does not exist! {directory}", typeof(Config));
                Log.Info($"Creating the configuration directory: {directory}", typeof(Config));
                Directory.CreateDirectory(directory);
            }

            // If the file does not exist, create it and return the predefined search engines
            if (!File.Exists(FilePath))
            {
                Log.Error($"The configuration file does not exist! {FilePath}", typeof(Config));
                Log.Info("Creating a new configuration file with the predefined search engines.", typeof(Config));
                File.WriteAllText(FilePath, JsonSerializer.Serialize(PredefinedSearchEngines, jsonSerializerOptions));
                return PredefinedSearchEngines;
            }

            // Read the configuration file and parse the contents
            try
            {
                string json = File.ReadAllText(FilePath);
                if (!string.IsNullOrEmpty(json))
                {
                    return JsonSerializer.Deserialize<List<SearchEngine>>(json, jsonSerializerOptions) ?? [];
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed to load configuration file. {e.Message}", typeof(Config));
            }

            // Use the predefined search engines as a fallback
            return PredefinedSearchEngines;
        }

        /// <summary>
        /// Save the search engines configuration to disk
        /// </summary>
        public static void Save(List<SearchEngine> SearchEngines)
        {
            try
            {
                string json = JsonSerializer.Serialize(SearchEngines, jsonSerializerOptions);
                File.WriteAllText(FilePath, json);
            }
            catch (Exception e)
            {
                Log.Error($"Failed to save configuration file. {e.Message}", typeof(Config));
            }
        }

        /// <summary>
        /// JSON serializer options
        /// </summary>
        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
        };

        /// <summary>
        /// Download the favicons for the search engines that do not already have one
        /// </summary>
        public static async void DownloadFavicons(List<SearchEngine> SearchEngines)
        {
            // If the Favicon folder does not exist, create it
            string directory = Path.Combine(Main.PluginDirectory, "Images", "Favicon");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Download the favicons for the search engines that do not already have one
            List<Task<bool>> tasks = SearchEngines
                .Where(engine => !File.Exists(engine.IconPath))
                .Select(engine => engine.DownloadFavicon(directory)).ToList();

            // Wait for all the tasks to complete
            await Task.WhenAll(tasks);

            // Update the configuration file (with the IconPaths) if any of the tasks succeeded
            if (tasks.Any(task => task.Result)) { Save(SearchEngines); }
        }

    }

}
