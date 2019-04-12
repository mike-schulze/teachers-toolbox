using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TeachingToolbox.Model;

namespace TeachingToolbox.ViewModel
{
    public class AttendanceViewModel : ViewModelBase
    {
        public AttendanceViewModel( IDataService aDataService, IAppService aAppService )
        {
            mDataService = aDataService;
            mAppService = aAppService;

            if( mAppService.AppState.SelectedSection != null )
            {
                SetAttendance( mAppService.AppState.SelectedSection.StudentIds );
                mAppService.AppState.SelectedSection.StudentIds.CollectionChanged += StudentIds_CollectionChanged;
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
                    mAppService.AppState.SelectedSection.StudentIds.CollectionChanged -= StudentIds_CollectionChanged;
                }
            }
        }

        private void AppState_PropertyChanged( object sender, System.ComponentModel.PropertyChangedEventArgs e )
        {
            if( e.PropertyName == AppState.SelectedSectionPropertyName )
            {
                if( mAppService.AppState.SelectedSection != null )
                {
                    SetAttendance( mAppService.AppState.SelectedSection.StudentIds );
                    mAppService.AppState.SelectedSection.StudentIds.CollectionChanged += StudentIds_CollectionChanged;
                }
            }
        }

        private void StudentIds_CollectionChanged( object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e )
        {
            if( e.OldItems != null )
            {
                var theStudentsToRemove = new List<AttendingStudent>();
                foreach( Guid theGuid in e.OldItems )
                {
                    foreach( var theStudent in AttendingStudents )
                    {
                        if( theStudent.Id == theGuid )
                        {
                            theStudentsToRemove.Add( theStudent );                            
                        }
                    }
                }

                foreach( var theStudent in theStudentsToRemove )
                {
                    AttendingStudents.Remove( theStudent );
                }
            }
            
            if( e.NewItems != null )
            {
                foreach( Guid theGuid in e.NewItems )
                {
                    var theStudent = mDataService.GetStudent( theGuid );
                    if( theStudent != null )
                    {
                        AttendingStudents.Add( new AttendingStudent() { Id = theGuid, Name = theStudent.ToString(), IsAttending = true } );
                    }
                }                
            }
        }

        private void SetAttendance( IEnumerable<Guid> aStudentIds )
        {
            if( mAppService.AppState.SelectedSection.AttendingStudents != null )
            {
                return;
            }

            mAppService.AppState.SelectedSection.AttendingStudents = new ObservableCollection<AttendingStudent>();

            foreach( Guid theGuid in aStudentIds )
            {
                var theStudent = mDataService.GetStudent( theGuid );
                if( theStudent != null )
                {
                    AttendingStudents.Add( new AttendingStudent () { Id = theGuid, Name = theStudent.ToString(), IsAttending = true } );
                }
            }
        }

        public ObservableCollection<AttendingStudent> AttendingStudents
        {
            get
            {
                return mAppService.AppState.SelectedSection.AttendingStudents;
            }
        }
        public const string AttendingStudentsPropertyName = "AttendingStudents";

        private readonly IDataService mDataService;
        private readonly IAppService mAppService;
    }
}
