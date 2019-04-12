using System;
using System.Linq;

namespace TeachingToolbox.Model
{
    public interface IAppService
    {
        AppState AppState { get; set; }
    }
}
