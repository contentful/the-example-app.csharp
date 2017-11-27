using Contentful.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheExampleApp.Models;

namespace TheExampleApp.Configuration
{
    /// <summary>
    /// Resolves a strong type from a content type id. Instructing the serialization engine how to deserialize items in a collection.
    /// </summary>
    public class ModulesResolver : IContentTypeResolver
    {
        private Dictionary<string, Type> _types = new Dictionary<string, Type>()
        {
            { "layoutCopy", typeof(LayoutCopy) },
            { "layoutHeroImage", typeof(LayoutHeroImage) },
            { "layoutHighlightedCourse", typeof(LayoutHighlightedCourse) },
            { "lessonCodeSnippets", typeof(LessonCodeSnippets) },
            { "lessonCopy", typeof(LessonCopy) },
            { "lessonImage", typeof(LessonImage) },
        };

        /// <summary>
        /// Method to get a type based on the specified content type id.
        /// </summary>
        /// <param name="contentTypeId">The content type id to resolve to a type.</param>
        /// <returns>The type for the content type id or null if none is found.</returns>
        public Type Resolve(string contentTypeId)
        {
            return _types.TryGetValue(contentTypeId, out var type) ? type : null;
        }
    }
}
