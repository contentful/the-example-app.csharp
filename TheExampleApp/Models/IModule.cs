using Contentful.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheExampleApp.Models
{
    /// <summary>
    /// Interface to mark which classes can be used as modules.
    /// </summary>
    public interface IModule
    {
        SystemProperties Sys { get; set; }
    }

    /// <summary>
    /// Interface to mark lesson modules.
    /// </summary>
    public interface ILessonModule : IModule
    {
    }

    /// <summary>
    /// Interface to mark layout modules.
    /// </summary>
    public interface ILayoutModule : IModule
    {
    }
}
