using System;
using System.Linq;

namespace TeachingToolbox.Model
{
    public class AppService : IAppService
    {
        public AppService()
        {
            AppState = new AppState ();
        }

        public AppState AppState { get; set; }
    }
}
