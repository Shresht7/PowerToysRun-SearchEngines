# `SearchEngines`

A PowerToys Run Plugin that allows you to perform a search using search-engines.

![Demonstration](./screenshot.png)

## 📖 Usage

> ?! GitHub PowerToys

1. Open PowerToys Run (<kbd>Alt</kbd> + <kbd>Space</kbd>)
2. Use the `ActionKeyword` (`?!`) followed by the keyword and the search query (e.g. `?! yt PowerToys`)
3. Select the desired search-engine from the list of suggestions. (The keyword allows you to filter the search-engines)
4. Press <kbd>Enter</kbd> to open the search results using the default browser

### 📃 Examples

- `?! google Search Query`
- `?! bing Search Query`
- `?! mdn Search Query`
- `?! wolfram Search Query`
- `?! yt Search Query`
- `?! gh Search Query`

## 📦 Installation

1. Close PowerToys
2. Download the latest release from the [releases page][releases]
3. Extract the zip archive
4. Move the extracted folder (`SearchEngines`) to the PowerToys Run Plugins directory (`%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins`)
5. Start PowerToys

## 🛠️ Configuration

The plugin can be configured by editing the `Configuration\SearchEngines.json` file located in the plugin directory (`%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins\SearchEngines\`).

```json
[
    {
      "name": "Google",
      "url": "https://www.google.com/search?q=%s",
      "shortcut": "google"
    },
    {
      "name": "Bing",
      "url": "https://www.bing.com/search?q=%s",
      "shortcut": "bing"
    },
    {
      "name": "Mozilla Developer Network",
      "url": "https://developer.mozilla.org/en-US/search?q=%s",
      "shortcut": "mdn",
    },
    {
      "name": "Wolfram Alpha",
      "url": "https://www.wolframalpha.com/input/?i=%s",
      "shortcut": "wolfram",
    }
]
```

> [!TIP]
>
> `url` doesn't always have to be a search-engine. It can be any URL. The `%s` will be replaced with the search query. For example:
>
> ```json
> {
>   "name": "Learn X in Y Minutes",
>   "url": "https://learnxinyminutes.com/docs/%s",
>   "shortcut": "lxym"
> }  
> ```

## 🔗 Related

- [Microsoft PowerToys](https://github.com/Microsoft/PowerToys)

---

## 📄 License

This project is licensed under the [MIT License](./LICENSE).

[releases]: https://github.com/Shresht7/PowerToysRun-SearchEngines/releases
