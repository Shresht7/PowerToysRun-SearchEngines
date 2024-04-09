// Library
using System.IO;
using System.Net.Http;
using Wox.Plugin.Logger;

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
        /// Path to the icon of the search engine
        /// </summary>
        public string? IconPath { get; set; }

        /// <summary>
        /// Check if the search engine object is valid
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Url) && !string.IsNullOrEmpty(Shortcut);
        }

        /// <summary>
        /// Downloads the favicon for the specified URL and saves it to the local file system.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result is a boolean value indicating whether the favicon was successfully downloaded and saved.
        /// </returns>
        public async Task<bool> DownloadFavicon(string directory)
        {
            // Extract the domain from the URL. This will be used to name the favicon file locally.
            // For example, the favicon for "https://www.google.com" will be saved as "google.png".
            string domain = new Uri(Url).Host.Replace("www.", "");

            // Path to the favicon file
            string path = Path.Combine(directory, $"{domain}.png");
            string iconPath = Path.Combine("Images", "Favicon", $"{domain}.png");

            // Check if the favicon already exists
            if (File.Exists(path))
            {
                IconPath = iconPath;
                return false; // We don't need to download the favicon again
            }

            // Download the favicon file
            byte[] favicon = await DownloadFaviconAsync(Url);
            if (favicon == null || favicon.Length == 0)
            {
                Log.Warn($"Failed to download favicon for {Name}", GetType());
                return false; // Failed to download the favicon
            }

            // Save the favicon to the local file system
            try
            {
                await File.WriteAllBytesAsync(path, favicon);
            }
            catch
            {
                Log.Warn($"Failed to save favicon for {Name}", GetType());
                return false; // Failed to save the favicon
            }

            // Set the IconPath to the local file path of the favicon
            IconPath = iconPath;
            return true;
        }


        /// <summary>
        /// Downloads the favicon for a given URL asynchronously
        /// </summary>
        /// <param name="url">The URL of the website</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the favicon as a byte array.
        /// </returns>
        private static async Task<byte[]> DownloadFaviconAsync(string url)
        {
            try
            {
                string faviconUrl = new Uri(url).GetLeftPart(UriPartial.Authority) + "/favicon.ico";
                using HttpClient client = new();
                HttpResponseMessage response = await client.GetAsync(faviconUrl);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed to download favicon: {e.Message}", typeof(SearchEngine));
            }

            return [];
        }

    }

}
