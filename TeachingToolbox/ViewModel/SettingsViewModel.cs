using FolderPickerLib;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using TeachingToolbox.Model;

namespace TeachingToolbox.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {        
        public SettingsViewModel( IDataService aDataService )
        {
            mDataService = aDataService;

            AddSectionCommand = new RelayCommand ( AddSection, () => !HasSelection && CanAddSection );
            UpdateSectionCommand = new RelayCommand( UpdateSection, () => HasSelection && CanUpdateSection );
            DeleteSectionCommand = new RelayCommand ( DeleteSection, () => HasSelection );
            ClearSectionCommand = new RelayCommand( ClearSection );

            AddStudentCommand = new RelayCommand ( AddStudent, () => CanAddStudent );
            EnrollStudentCommand = new RelayCommand( EnrollStudent, () => SelectedNonEnrolledStudent != null );
            DropStudentCommand = new RelayCommand( DropStudent, () => SelectedEnrolledStudent != null );

            BrowseForDatabasePathCommand = new RelayCommand ( BrowseForDatabasePath );

            Sections = new ObservableCollection<Section>( mDataService.GetSections() );
            if( Sections.Count > 0 )
            {
                SelectedSection = Sections[ 0 ];
            }
            else
            {
                CurrentlyEditedSection = new Section( string.Empty, string.Empty, string.Empty );
            }

            NewStudent = new Student ( string.Empty, string.Empty );

            Languages = new List<string> () { "English", "Deutsch" };
            string theSelectedLanguage = Properties.Settings.Default.Language;
            if( theSelectedLanguage == "en" )
            {
                mSelectedLanguage = "English";
            }
            else if( theSelectedLanguage == "de" )
            {
                mSelectedLanguage = "Deutsch";
            }

            mDatabasePath = Properties.Settings.Default.DatabasePath;
            if( String.IsNullOrEmpty( mDatabasePath ) )
            {
                DatabasePath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData ), "Teaching Toolbox" );
            }
        }

        private void AddSection()
        {
            mDataService.AddSection( CurrentlyEditedSection );
            Sections.Add( CurrentlyEditedSection );
            SelectedSection = CurrentlyEditedSection;
        }

        private void UpdateSection()
        {
            SelectedSection.Update( CurrentlyEditedSection );
            CanUpdateSection = false;
            mDataService.Save();
        }

        private void DeleteSection()
        {
            mDataService.DeleteSection( SelectedSection );
            Sections.Remove( SelectedSection );
            ClearSection();
        }

        private void ClearSection()
        {
            SelectedSection = null;
            CurrentlyEditedSection = new Section ( string.Empty, string.Empty, string.Empty );
        }

        private void HandleCurrentlySelectedSectionUpdated( object aSender, PropertyChangedEventArgs aArgs )
        {
            if( HasSelection )
            {
                CanUpdateSection = !SelectedSection.Equals( CurrentlyEditedSection );
            }
            else
            {
                CanAddSection = CurrentlyEditedSection.IsValid();
            }
        }

        private void HandleNewStudentUpdated( object aSender, PropertyChangedEventArgs aArgs )
        {
            if( !string.IsNullOrEmpty( NewStudent.FirstName ) && !string.IsNullOrEmpty( NewStudent.LastName ) )
            {
                CanAddStudent = true;
            }
            else
            {
                CanAddStudent = false;
            }
        }

        private void AddStudent()
        {
            mDataService.AddStudent( NewStudent );
            EnrolledStudents.Add( NewStudent );
            SelectedSection.StudentIds.Add( NewStudent.Id );
            mDataService.Save();

            SelectedEnrolledStudent = NewStudent;

            NewStudent = new Student( string.Empty, string.Empty );
        }

        private void DropStudent()
        {
            var theStudent = SelectedEnrolledStudent;

            SelectedSection.StudentIds.Remove( theStudent.Id );
            mDataService.Save();

            NonEnrolledStudents.Add( theStudent );
            EnrolledStudents.Remove( theStudent );

            SelectedNonEnrolledStudent = theStudent;            
        }

        private void EnrollStudent()
        {
            var theStudent = SelectedNonEnrolledStudent;

            SelectedSection.StudentIds.Add( theStudent.Id );
            mDataService.Save();

            EnrolledStudents.Add( theStudent );
            NonEnrolledStudents.Remove( theStudent );

            SelectedEnrolledStudent = theStudent;
        }

        private void BrowseForDatabasePath()
        {
            var theDialog = new FolderPickerDialog ();
            theDialog.InitialPath = DatabasePath;

            if( theDialog.ShowDialog() == true )
            {
                DatabasePath = theDialog.SelectedPath;
            }
        }

        public ObservableCollection<Section> Sections
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
        public const string SectionsPropertyName = "Sections";
        private ObservableCollection<Section> mSections = null;

        public bool HasSelection
        {
            get
            {
                return mHasSelection;
            }

            set
            {
                if( Set( HasSelectionPropertyName, ref mHasSelection, value ) )
                {
                    AddSectionCommand.RaiseCanExecuteChanged();
                    UpdateSectionCommand.RaiseCanExecuteChanged();
                    DeleteSectionCommand.RaiseCanExecuteChanged();
                }
            }
        }
        public const string HasSelectionPropertyName = "HasSelection";
        private bool mHasSelection = false;

        public bool CanAddSection
        {
            get
            {
                return mCanAddSection;
            }

            set
            {
                if( Set( CanAddSectionPropertyName, ref mCanAddSection, value ) )
                {
                    AddSectionCommand.RaiseCanExecuteChanged();
                }
            }
        }
        public const string CanAddSectionPropertyName = "CanAddSection";
        private bool mCanAddSection = false;

        public bool CanUpdateSection
        {
            get
            {
                return mCanUpdateSection;
            }

            set
            {
                if( Set( CanUpdateSectionPropertyName, ref mCanUpdateSection, value ) )
                {
                    UpdateSectionCommand.RaiseCanExecuteChanged();
                }
            }
        }
        public const string CanUpdateSectionPropertyName = "CanUpdateSection";
        private bool mCanUpdateSection = false;

        public bool CanAddStudent
        {
            get
            {
                return mCanAddStudent;
            }

            set
            {
                if( Set( CanAddStudentPropertyName, ref mCanAddStudent, value ) )
                {
                    AddStudentCommand.RaiseCanExecuteChanged();
                }
            }
        }
        public const string CanAddStudentPropertyName = "CanAddStudent";
        private bool mCanAddStudent = false;

        public Section SelectedSection
        {
            get
            {
                return mSelectedSection;
            }

            set
            {
                if( Set( SelectedSectionPropertyName, ref mSelectedSection, value ) )
                {
                    HasSelection = value != null;

                    if( value != null )
                    {
                        CurrentlyEditedSection = new Section( value );

                        StudentsInSectionTitle = string.Format( Properties.Resources.students_in_section_title, value.Name + " (" + value.Days + ")" );

                        var theStudents = mDataService.GetStudents();
                        EnrolledStudents = new ObservableCollection<Student> ();
                        NonEnrolledStudents = new ObservableCollection<Student> ();
                        foreach( var theStudent in theStudents )
                        {
                            if( SelectedSection.StudentIds.Any( ( theId ) => theId == theStudent.Id ) )
                            {
                                EnrolledStudents.Add( theStudent );
                            }
                            else
                            {
                                NonEnrolledStudents.Add( theStudent );
                            }
                        }
                    }
                }
            }
        }
        public const string SelectedSectionPropertyName = "SelectedSection";
        private Section mSelectedSection = null;

        public string StudentsInSectionTitle
        {
            get
            {
                return mStudentsInSectionTitle;
            }

            set
            {
                Set( StudentsInSectionTitlePropertyName, ref mStudentsInSectionTitle, value );
            }
        }
        public const string StudentsInSectionTitlePropertyName = "StudentsInSectionTitle";
        private string mStudentsInSectionTitle = null;

        public Section CurrentlyEditedSection
        {
            get
            {
                return mCurrentlyEditedSection;
            }

            set
            {
                if( value != null )
                {
                    value.PropertyChanged -= HandleCurrentlySelectedSectionUpdated;
                }

                Set( CurrentlyEditedSectionPropertyName, ref mCurrentlyEditedSection, value );

                if( value != null )
                {
                    CanAddSection = false;
                    CanUpdateSection = false;
                    value.PropertyChanged += HandleCurrentlySelectedSectionUpdated;
                }
            }
        }
        public const string CurrentlyEditedSectionPropertyName = "CurrentlyEditedSection";
        private Section mCurrentlyEditedSection = null;

        public List<string> Languages
        {
            get
            {
                return mLanguages;
            }

            private set
            {
                Set( LanguagesPropertyName, ref mLanguages, value );
            }
        }
        public const string LanguagesPropertyName = "Languages";
        private List<string> mLanguages = null;

        public string SelectedLanguage
        {
            get
            {
                return mSelectedLanguage;
            }

            private set
            {
                if( Set( SelectedLanguagePropertyName, ref mSelectedLanguage, value ) )
                {
                    if( value.Equals( "English" ) )
                    {
                        Properties.Settings.Default.Language = "en";
                        Properties.Settings.Default.Save();

                        App.ChangeCulture( new CultureInfo( "en" ) );
                    }
                    else if( value.Equals( "Deutsch" ) )
                    {
                        Properties.Settings.Default.Language = "de";
                        Properties.Settings.Default.Save();

                        App.ChangeCulture( new CultureInfo( "de" ) );
                    }
                }
            }
        }
        public const string SelectedLanguagePropertyName = "SelectedLanguage";
        private string mSelectedLanguage = null;

        public ObservableCollection<Student> EnrolledStudents
        {
            get
            {
                return mEnrolledStudents;
            }

            private set
            {
                Set( EnrolledStudentsPropertyName, ref mEnrolledStudents, value );
            }
        }
        public const string EnrolledStudentsPropertyName = "EnrolledStudents";
        private ObservableCollection<Student> mEnrolledStudents = null;

        public ObservableCollection<Student> NonEnrolledStudents
        {
            get
            {
                return mNonEnrolledStudents;
            }

            private set
            {
                Set( NonEnrolledStudentsPropertyName, ref mNonEnrolledStudents, value );
            }
        }
        public const string NonEnrolledStudentsPropertyName = "NonEnrolledStudents";
        private ObservableCollection<Student> mNonEnrolledStudents = null;

        public Student SelectedEnrolledStudent
        {
            get
            {
                return mSelectedEnrolledStudent;
            }

            private set
            {
                if( Set( SelectedEnrolledStudentPropertyName, ref mSelectedEnrolledStudent, value ) )
                {
                    if( value != null )
                    {
                        SelectedNonEnrolledStudent = null;
                    }
                }
            }
        }
        public const string SelectedEnrolledStudentPropertyName = "SelectedEnrolledStudent";
        private Student mSelectedEnrolledStudent = null;

        public Student SelectedNonEnrolledStudent
        {
            get
            {
                return mSelectedNonEnrolledStudent;
            }

            private set
            {
                if( Set( SelectedNonEnrolledStudentPropertyName, ref mSelectedNonEnrolledStudent, value ) )
                {
                    if( value != null )
                    {
                        SelectedEnrolledStudent = null;
                    }
                }
            }
        }
        public const string SelectedNonEnrolledStudentPropertyName = "SelectedNonEnrolledStudent";
        private Student mSelectedNonEnrolledStudent = null;

        public Student NewStudent
        {
            get
            {
                return mNewStudent;
            }

            private set
            {
                if( value != null )
                {
                    value.PropertyChanged -= HandleNewStudentUpdated;
                }

                Set( NewStudentPropertyName, ref mNewStudent, value );

                if( value != null )
                {
                    CanAddStudent = false;
                    value.PropertyChanged += HandleNewStudentUpdated;
                }
            }
        }
        public const string NewStudentPropertyName = "NewStudent";
        private Student mNewStudent = null;

        public string DatabasePath
        {
            get
            {
                return mDatabasePath;
            }

            set
            {
                if( Set( DatabasePathPropertyName, ref mDatabasePath, value ) )
                {
                    Properties.Settings.Default.DatabasePath = value;
                    Properties.Settings.Default.Save();
                }
            }
        }
        private string mDatabasePath = null;
        public const string DatabasePathPropertyName = "DatabasePath";

        public RelayCommand AddSectionCommand { get; private set; }
        public RelayCommand UpdateSectionCommand { get; private set; }
        public RelayCommand DeleteSectionCommand { get; private set; }
        public RelayCommand ClearSectionCommand { get; private set; }

        public RelayCommand AddStudentCommand { get; private set; }
        public RelayCommand EnrollStudentCommand { get; private set; }
        public RelayCommand DropStudentCommand { get; private set; }

        public RelayCommand BrowseForDatabasePathCommand { get; private set; }

        private readonly IDataService mDataService;
    }
}