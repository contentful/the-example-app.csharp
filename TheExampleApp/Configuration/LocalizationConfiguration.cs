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
    public class JsonViewLocalizer : IViewLocalizer
    {
        private Dictionary<string, Dictionary<string,string>> _items = new Dictionary<string, Dictionary<string, string>>();

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

        public LocalizedHtmlString this[string name] => new LocalizedHtmlString(name, _items[CultureInfo.CurrentCulture.ToString()][name]);

        public LocalizedHtmlString this[string name, params object[] arguments] => throw new NotImplementedException();

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return _items[CultureInfo.CurrentCulture.ToString()].Select(c => new LocalizedString(c.Key, c.Value));
        }

        public LocalizedString GetString(string name)
        {
            return GetString(name, null);
        }

        public LocalizedString GetString(string name, params object[] arguments)
        {
            return new LocalizedString(name, _items[CultureInfo.CurrentCulture.ToString()][name]);
        }

        public IHtmlLocalizer WithCulture(CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
