using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using TeachingToolbox.Model;

namespace TeachingToolbox.ViewModel
{
    public class HomeViewModel : ViewModelBase
    {
        public HomeViewModel( IDataService aDataService, IAppService aAppService, INavigationService aNavService )
        {
            mDataService = aDataService;
            mAppService = aAppService;
            mNavigationService = aNavService;

            GoToSettingsCommand = new RelayCommand ( GoToSettings );
            EnterSectionCommand = new RelayCommand<Section> ( EnterSection );

            Sections = mDataService.GetSections();
        }

        private void GoToSettings()
        {
            mNavigationService.NavigateTo( new SettingsPage () );
        }

        private void EnterSection( Section aSection )
        {
            if( aSection != null )
            {
                mAppService.AppState.SelectedSection = aSection;
                mNavigationService.NavigateTo( new LiveSectionPage() );
            }  
        }

        public IEnumerable<Section> Sections
        {
            get
            {
                return mSections;
            }

            set
            {
                Set( SectionsPropertyName, ref mSections, value );
            }
        }
        private IEnumerable<Section> mSections = null;
        public const string SectionsPropertyName = "Sections";

        public RelayCommand GoToSettingsCommand { get; private set; }
        public RelayCommand<Section> EnterSectionCommand { get; private set; }

        private readonly IDataService mDataService;
        private readonly IAppService mAppService;
        private readonly INavigationService mNavigationService;
    }
}
