using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Threading;
using TeachingToolbox.Model;

namespace TeachingToolbox.ViewModel
{
    public class RandomizerViewModel : ViewModelBase
    {
        public RandomizerViewModel( IAppService aAppService )
        {
            mAppService = aAppService;

            GetStudentCommand = new RelayCommand( GetStudent, () => !IsBusy && StudentsLeft > 0 );
            ResetCommand = new RelayCommand( Reset, () => !IsBusy && StudentsLeft != NumberOfStudents );
            GetGroupsCommand = new RelayCommand ( GetGroups, () => NumberOfStudents > 2 );

            IncreaseNumberOfGroupsCommand = new RelayCommand ( IncreaseNumberOfGroups, () => NumberOfGroups < NumberOfStudents );
            DecreaseNumberOfGroupsCommand = new RelayCommand( DecreaseNumberOfGroups, () => NumberOfGroups > 1 );

            IncreaseGroupSizeCommand = new RelayCommand ( IncreaseGroupSize, () => GroupSize < NumberOfStudents );
            DecreaseGroupSizeCommand = new RelayCommand( DecreaseGroupSize, () => GroupSize > 1 );

            mTimer = new DispatcherTimer ();
            mTimer.Interval = TimeSpan.FromMilliseconds( 50 );
            mTimer.Tick += HandleTimer_Tick;

            CalculateSizes();

            foreach( var theAttendingStudent in AttendingStudents )
            {
                theAttendingStudent.PropertyChanged += HandleStudentPropertyChanged;
            }

            mAppService.AppState.PropertyChanged += AppState_PropertyChanged;
            mAppService.AppState.PropertyChanging += AppState_PropertyChanging;
        }

        void AppState_PropertyChanging( object sender, System.ComponentModel.PropertyChangingEventArgs e )
        {
            if( e.PropertyName == AppState.SelectedSectionPropertyName )
            {
                if( mAppService.AppState.SelectedSection != null )
                {
                    foreach( var theAttendingStudent in AttendingStudents )
                    {
                        theAttendingStudent.PropertyChanged -= HandleStudentPropertyChanged;
                    }

                    mAppService.AppState.SelectedSection.AttendingStudents.CollectionChanged -= AttendingStudents_CollectionChanged;
                }
            }
        }

        private void AppState_PropertyChanged( object sender, System.ComponentModel.PropertyChangedEventArgs e )
        {
            if( e.PropertyName == AppState.SelectedSectionPropertyName )
            {
                if( mAppService.AppState.SelectedSection != null )
                {
                    LuckyStudent = null;
                    CalculateSizes();

                    mAppService.AppState.SelectedSection.AttendingStudents.CollectionChanged += AttendingStudents_CollectionChanged;

                    foreach( var theAttendingStudent in AttendingStudents )
                    {
                        theAttendingStudent.PropertyChanged += HandleStudentPropertyChanged;
                    }

                    GetGroups();
                }
            }
        }

        void HandleStudentPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            if( e.PropertyName == AttendingStudent.IsAttendingPropertyName )
            {
                CalculateSizes();
                GetGroups();
            }
        }

        private void AttendingStudents_CollectionChanged( object aSender, System.Collections.Specialized.NotifyCollectionChangedEventArgs aArgs )
        {
            if( aArgs.OldItems != null )
            {
                foreach( var theStudent in aArgs.OldItems )
                {
                    ((AttendingStudent)theStudent).PropertyChanged -= HandleStudentPropertyChanged;
                }
            }

            if( aArgs.NewItems != null )
            {
                foreach( var theStudent in aArgs.NewItems )
                {
                    ( ( AttendingStudent ) theStudent ).PropertyChanged += HandleStudentPropertyChanged;
                }
            }

            CalculateSizes();
            GetGroups();
        }

        private void CalculateSizes()
        {
            NumberOfStudents = AttendingStudents.Count( theStudent => theStudent.IsAttending );
            StudentsLeft = AttendingStudents.Count( theStudent => !theStudent.AlreadyPicked && theStudent.IsAttending );

            mIsSettingSizes = true;
            GroupSize = 0;
            NumberOfGroups = 0;
            mIsSettingSizes = false;

            if (NumberOfStudents > 2)
            {
                GroupSize = 2;
            }
            else
            {
                GroupSize = NumberOfStudents;
            }
        }

        void HandleTimer_Tick( object sender, EventArgs e )
        {
            LuckyStudent = AttendingStudents.Where( theStudent => !theStudent.AlreadyPicked && theStudent.IsAttending )
                                            .OrderBy( theStudent => Guid.NewGuid() )
                                            .FirstOrDefault();


            if( mCounter > 25 || StudentsLeft == 1 )
            {
                if( LuckyStudent != null )
                {
                    LuckyStudent.AlreadyPicked = true;
                }
                
                CalculateSizes();
                mCounter = 0;
                mTimer.Stop();
                IsBusy = false;
            }
            else
            {
                mCounter++;    
            }            
        }

        private void GetStudent()
        {
            CalculateSizes();

            IsBusy = true;

            mCounter = 0;
            mTimer.Start();            
        }

        private void Reset()
        {
            foreach( var theStudent in AttendingStudents )
            {
                theStudent.AlreadyPicked = false;
            }

            LuckyStudent = null;
            CalculateSizes();
        }

        private void GetGroups()
        {
            var theSortedStudents = AttendingStudents.Where( theStudent => theStudent.IsAttending )
                                                     .OrderBy( theStudent => Guid.NewGuid() );

            int theCurrentGroup = 1;
            foreach( var theStudent in theSortedStudents )
            {
                theStudent.Group = theCurrentGroup;

                ++theCurrentGroup;
                if( theCurrentGroup > NumberOfGroups )
                {
                    theCurrentGroup = 1;
                }
            }

            RefreshStudents();
        }

        private void RefreshStudents()
        {
            GroupedStudentsView = ( CollectionView ) CollectionViewSource.GetDefaultView( AttendingStudents );
            if( GroupedStudentsView.CanGroup )
            {
                GroupedStudentsView.GroupDescriptions.Clear();
                GroupedStudentsView.SortDescriptions.Clear();

                PropertyGroupDescription theGroupDescription = new PropertyGroupDescription( "Group" );
                GroupedStudentsView.GroupDescriptions.Add( theGroupDescription );

                GroupedStudentsView.SortDescriptions.Add( new SortDescription( "Group", ListSortDirection.Ascending ) );
                GroupedStudentsView.SortDescriptions.Add( new SortDescription( "Name", ListSortDirection.Ascending ) );

                GroupedStudentsView.Filter = e => ( ( AttendingStudent ) e ).IsAttending;
            }
        }

        private void IncreaseNumberOfGroups()
        {
            ++NumberOfGroups;
        }

        private void DecreaseNumberOfGroups()
        {
            --NumberOfGroups;
        }

        private void IncreaseGroupSize()
        {
            ++GroupSize;
        }

        private void DecreaseGroupSize()
        {
            --GroupSize;
        }

        public ObservableCollection<AttendingStudent> AttendingStudents
        {
            get
            {
                return mAppService.AppState.SelectedSection.AttendingStudents;
            }
        }
        public const string AttendingStudentsPropertyName = "AttendingStudents";

        public bool IsBusy
        {
            get
            {
                return mIsBusy;
            }

            set
            {
                if( Set( IsBusyPropertyName, ref mIsBusy, value ) )
                {
                    GetStudentCommand.RaiseCanExecuteChanged();
                    ResetCommand.RaiseCanExecuteChanged();
                }
            }
        }
        private bool mIsBusy = false;
        public const string IsBusyPropertyName = "IsBusy";

        public AttendingStudent LuckyStudent
        {
            get
            {
                return mLuckyStudent;
            }

            set
            {
                if( Set( LuckyStudentPropertyName, ref mLuckyStudent, value ) )
                {
                    GetStudentCommand.RaiseCanExecuteChanged();
                }
            }
        }
        private AttendingStudent mLuckyStudent = null;
        public const string LuckyStudentPropertyName = "LuckyStudent";

        public int StudentsLeft
        {
            get
            {
                return mStudentsLeft;
            }

            set
            {
                if( Set( StudentsLeftPropertyName, ref mStudentsLeft, value ) )
                {
                    GetStudentCommand.RaiseCanExecuteChanged();
                    ResetCommand.RaiseCanExecuteChanged();
                }
            }
        }
        private int mStudentsLeft = 0;
        public const string StudentsLeftPropertyName = "StudentsLeft";

        public int NumberOfStudents
        {
            get
            {
                return mNumberOfStudents;
            }

            set
            {
                if( Set( NumberOfStudentsPropertyName, ref mNumberOfStudents, value ) )
                {
                    ResetCommand.RaiseCanExecuteChanged();
                    GetGroupsCommand.RaiseCanExecuteChanged();
                }
            }
        }
        private int mNumberOfStudents = 0;
        public const string NumberOfStudentsPropertyName = "NumberOfStudents";

        public List<AttendingStudent> Groups
        {
            get
            {
                return mGroups;
            }

            set
            {
                Set( GroupsPropertyName, ref mGroups, value );
            }
        }
        private List<AttendingStudent> mGroups = null;
        public const string GroupsPropertyName = "Groups";

        public CollectionView GroupedStudentsView
        {
            get
            {
                return mGroupedStudentsView;
            }

            set
            {
                Set( GroupedStudentsViewPropertyName, ref mGroupedStudentsView, value );
            }
        }
        private CollectionView mGroupedStudentsView = null;
        public const string GroupedStudentsViewPropertyName = "GroupedStudentsView";

        public int GroupSize
        {
            get
            {
                return mGroupSize;
            }

            set
            {
                if( Set( GroupSizePropertyName, ref mGroupSize, value ) )
                {
                    IncreaseGroupSizeCommand.RaiseCanExecuteChanged();
                    DecreaseGroupSizeCommand.RaiseCanExecuteChanged();

                    if( !mIsSettingSizes )
                    {
                        mIsSettingSizes = true;
                        NumberOfGroups = ( int ) Math.Ceiling( ( double ) NumberOfStudents / GroupSize );
                        GetGroups();
                    }
                    mIsSettingSizes = false;                    
                }
            }
        }
        private int mGroupSize = 0;
        public const string GroupSizePropertyName = "GroupSize";

        public int NumberOfGroups
        {
            get
            {
                return mNumberOfGroups;
            }

            set
            {
                if( Set( NumberOfGroupsPropertyName, ref mNumberOfGroups, value ) )
                {
                    IncreaseNumberOfGroupsCommand.RaiseCanExecuteChanged();
                    DecreaseNumberOfGroupsCommand.RaiseCanExecuteChanged();

                    if( !mIsSettingSizes )
                    {
                        mIsSettingSizes = true;
                        GroupSize = ( int ) Math.Ceiling( ( double ) NumberOfStudents / mNumberOfGroups );
                        GetGroups();
                    }
                    mIsSettingSizes = false;
                }
            }
        }
        private int mNumberOfGroups = 0;
        public const string NumberOfGroupsPropertyName = "NumberOfGroups";

        public RelayCommand GetStudentCommand { get; private set; }
        public RelayCommand ResetCommand { get; private set; }
        public RelayCommand GetGroupsCommand { get; private set; }

        public RelayCommand IncreaseNumberOfGroupsCommand { get; private set; }
        public RelayCommand DecreaseNumberOfGroupsCommand { get; private set; }
        public RelayCommand IncreaseGroupSizeCommand { get; private set; }
        public RelayCommand DecreaseGroupSizeCommand { get; private set; }

        private readonly IAppService mAppService;

        private readonly DispatcherTimer mTimer;
        private int mCounter;

        private bool mIsSettingSizes;
    }
}
