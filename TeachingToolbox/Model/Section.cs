using GalaSoft.MvvmLight;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace TeachingToolbox.Model
{
    public class Section : ObservableObject, IEquatable<Section>
    {
        public Section()
        {
        }

        public Section( string aName, string aDays, string aTime )
        {
            Id = Guid.NewGuid();

            Name = aName;
            Days = aDays;
            Time = aTime;
            
            StudentIds = new ObservableCollection<Guid> ();
        }

        public Section( Section aSection )
        {
            Id = aSection.Id;
            Name = aSection.Name;
            Days = aSection.Days;
            Time = aSection.Time;
            StudentIds = aSection.StudentIds;
            AttendingStudents = aSection.AttendingStudents;
        }

        public override string ToString()
        {
            return Name + " " + Days + " " + Time;
        }

        public void Update( Section aSection )
        {
            Name = aSection.Name;
            Days = aSection.Days;
            Time = aSection.Time;
        }

        public Guid Id { get; set; }

        public string Name 
        {
            get
            {
                return mName;
            }

            set
            {
                Set( mNamePropertyName, ref mName, value );
            }            
        }
        private readonly string mNamePropertyName = "Name";
        private string mName;

        public string Days
        {
            get
            {
                return mDays;
            }

            set
            {
                Set( mDaysPropertyName, ref mDays, value );
            }
        }
        private readonly string mDaysPropertyName = "Days";
        private string mDays;

        public string Time
        {
            get
            {
                return mTime;
            }

            set
            {
                Set( mTimePropertyName, ref mTime, value );
            }
        }
        private readonly string mTimePropertyName = "Time";
        private string mTime;

        public ObservableCollection<Guid> StudentIds { get; set; }

        // Equality checks

        public override int GetHashCode()
        {
            int theHash = 23;
            theHash = theHash * 17 + Name != null ? Name.GetHashCode() : 0;
            theHash = theHash * 17 + Days != null ? Days.GetHashCode() : 0;
            theHash = theHash * 17 + Time != null ? Time.GetHashCode() : 0;
            return theHash;
        }

        public override bool Equals( object obj )
        {
            return this.Equals( obj as Section );
        }

        public bool Equals( Section aSection )
        {
            if( this == aSection )
            {
                return true;
            }

            if( aSection == null )
            {
                return false;
            }

            if( aSection.GetType() != GetType() )
            {
                return false;
            }            

            // Return true if the fields match. 
            return( aSection.Name == Name && aSection.Days == Days && aSection.Time == Time );
        }

        public bool IsValid()
        {
            if( string.IsNullOrEmpty( Name ) )
            {
                return false;
            }

            if( string.IsNullOrEmpty( Days ) )
            {
                return false;
            }

            if( string.IsNullOrEmpty( Time ) )
            {
                return false;
            }

            return true;
        }

        [XmlIgnore]
        public bool IsDirty { get; private set; }

        [XmlIgnore]
        public ObservableCollection<AttendingStudent> AttendingStudents { get; set; }
    }
}
