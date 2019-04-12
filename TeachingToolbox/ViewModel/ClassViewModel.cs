using GalaSoft.MvvmLight;
using System;
using System.Linq;
using TeachingToolbox.Model;

namespace TeachingToolbox.ViewModel
{
    public class ClassViewModel : ViewModelBase
    {
        public ClassViewModel( IAppService aAppService )
        {
            mAppService = aAppService;
        }

        public Section SelectedSection
        {
            get
            {
                return mAppService.AppState.SelectedSection;
            }
        }
        public const string SelectedSectionPropertyName = "SelectedSection";

        private readonly IAppService mAppService;
    }
}
