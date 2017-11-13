using Contentful.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheExampleApp.Models;

namespace TheExampleApp.Configuration
{
    public class ModulesResolver : IContentTypeResolver
    {
        public Dictionary<string, Type> _types = new Dictionary<string, Type>()
        {
            { "layoutCopy", typeof(LayoutCopy) },
            { "layoutHeroImage", typeof(LayoutHeroImage) },
            { "layoutHighlightedCourse", typeof(LayoutHighlightedCourse) },
            { "lessonCodeSnippets", typeof(LessonCodeSnippets) },
            { "lessonCopy", typeof(LessonCopy) },
            { "lessonImage", typeof(LessonImage) },
        };

        public Type Resolve(string contentTypeId)
        {
            return _types.TryGetValue(contentTypeId, out var type) ? type : null;
        }
    }
}
