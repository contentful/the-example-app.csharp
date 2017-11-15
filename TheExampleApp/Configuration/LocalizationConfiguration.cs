using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Localization;
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

        public JsonViewLocalizer(IHostingEnvironment hostingEnvironment)
        {
            var webRoot = hostingEnvironment.WebRootPath;

            if (string.IsNullOrEmpty(webRoot))
            {
                webRoot = $"{hostingEnvironment.ContentRootPath}/theexampleapp/wwwroot";
            }

            foreach (string file in Directory.EnumerateFiles($"{webRoot}/locales/", "*.json"))
            {
                var info = new FileInfo(file);

                var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(file));
                _items.Add(Path.GetFileNameWithoutExtension(info.Name), dictionary);
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
