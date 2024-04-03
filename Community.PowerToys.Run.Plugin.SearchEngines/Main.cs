// Library
using Wox.Plugin;

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

        #endregion

        /// <summary>
        /// Returns a filtered list of results based on the given query
        /// </summary>
        /// <param name="query">The input query provided by the user</param>
        /// <returns>A filtered list of results. Can be empty if nothing is found.</returns>
        public List<Result> Query(Query query)
        {
            return
            [
                new Result
                {
                    Title = "Hello World!",
                    SubTitle = "Says hello world!",
                    IcoPath = "Images\\icon.png",
                    Action = e =>
                    {
                        Context?.API.ShowNotification("Hello World!");
                        return true;
                    }
                }
            ];
        }

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
        }

    }
}
