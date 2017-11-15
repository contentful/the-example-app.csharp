using Contentful.Core.Configuration;

namespace TheExampleApp.Configuration
{
    public interface IContentfulOptionsManager
    {
        ContentfulOptions Options
        {
            get;
        }
    }
}