using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TheExampleApp.Configuration
{
    /// <summary>
    /// A view localizer that reads its values from a json file.
    /// </summary>
    public class JsonViewLocalizer : IViewLocalizer
    {
        private Dictionary<string, Dictionary<string,string>> _items = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// Initializes a new JsonViewLocalizer with a fileprovider.
        /// </summary>
        /// <param name="provider">The file provider used to retrieve the json files.</param>
        public JsonViewLocalizer(IFileProvider provider)
        {
            var files = provider.GetDirectoryContents("");

            foreach (IFileInfo file in files)
            {
                using ( var sr = new StreamReader(file.CreateReadStream()))
                {
                    var fileContents = sr.ReadToEnd();
                    var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileContents);
                    var fileName = Path.GetFileNameWithoutExtension(file.Name);
                    _items.Add(fileName.Substring(fileName.LastIndexOf('.') + 1), dictionary);
                }
            }
        }

        /// <summary>
        /// Gets a translated string with the provided name for the current culture.
        /// </summary>
        /// <param name="name">The name of the key in the json file to get a translation for.</param>
        /// <returns>The localized result.</returns>
        public LocalizedHtmlString this[string name] => new LocalizedHtmlString(name, _items[CultureInfo.CurrentCulture.ToString()][name]);

        /// <summary>
        /// This method is not implemented.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public LocalizedHtmlString this[string name, params object[] arguments] => throw new NotImplementedException();

        /// <summary>
        /// Gets all the available string for the current culture.
        /// </summary>
        /// <param name="includeParentCultures">Wether or not to include parent cultures. This parameter is not used in the current implementation.</param>
        /// <returns>An IEnumerable of localized string.</returns>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var culture = CultureInfo.CurrentCulture.ToString();

            if (!_items.ContainsKey(culture))
            {
                return new List<LocalizedString>();
            }

            return _items[culture].Select(c => new LocalizedString(c.Key, c.Value));
        }

        /// <summary>
        /// Gets a localized string by the provided key.
        /// </summary>
        /// <param name="name">The key to get a translation for.</param>
        /// <returns>The localized string.</returns>
        public LocalizedString GetString(string name)
        {
            return GetString(name, null);
        }

        /// <summary>
        /// Gets a localized string by the provided key.
        /// </summary>
        /// <param name="name">The key to get a translation for.</param>
        /// <param name="arguments">The additional arguments. Not used in the current implementation.</param>
        /// <returns>The localized string.</returns>
        public LocalizedString GetString(string name, params object[] arguments)
        {
            var culture = CultureInfo.CurrentCulture.ToString();

            if (!_items.ContainsKey(culture) || !_items[culture].ContainsKey(name))
            {
                return new LocalizedString(name, name, true);
            }

            return new LocalizedString(name, _items[culture][name]);
        }

        /// <summary>
        /// This method is not implemented.
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public IHtmlLocalizer WithCulture(CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
