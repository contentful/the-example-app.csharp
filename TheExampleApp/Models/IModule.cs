using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheExampleApp.Models
{
    public interface IModule
    {
    }

    public interface ILessonModule : IModule
    {
    }

    public interface ILayoutModule : IModule
    {
    }
}
