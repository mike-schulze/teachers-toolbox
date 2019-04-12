using GalaSoft.MvvmLight;
using System;
using System.Linq;

namespace TeachingToolbox.Model
{
    public class AttendingStudent : ObservableObject
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public bool AlreadyPicked { get; set; }

        public bool IsAttending
        {
            get
            {
                return mIsAttending;
            }

            set
            {
                Set( IsAttendingPropertyName, ref mIsAttending, value );
            }
        }
        private bool mIsAttending;
        public const string IsAttendingPropertyName = "IsAttending";

        public int Group
        {
            get
            {
                return mGroup;
            }

            set
            {
                Set( GroupPropertyName, ref mGroup, value );
            }
        }
        private int mGroup;
        public const string GroupPropertyName = "Group";

        public override string ToString()
        {
            return Name;
        }
    }
}
