using GalaSoft.MvvmLight;
using System;
using System.Linq;

namespace TeachingToolbox.Model
{
    public class AppState : ObservableObject
    {
        public Section SelectedSection
        {
            get
            {
                return mSelectedSection;
            }

            set
            {
                Set( SelectedSectionPropertyName, ref mSelectedSection, value );
            }
        }
        private Section mSelectedSection = null;
        public const string SelectedSectionPropertyName = "SelectedSection";
    }
}
