using GalaSoft.MvvmLight;
using System;
using System.Linq;
using System.Xml.Serialization;

namespace TeachingToolbox.Model
{
    public class Student : ObservableObject
    {
        public Student()
        {
        }

        public Student( string aFirstName, string aLastName )
        {
            Id = Guid.NewGuid();

            FirstName = aFirstName;
            LastName = aLastName;
        }

        public Guid Id { get; set; }

        public string FirstName
        {
            get
            {
                return mFirstName;
            }

            set
            {
                Set( mFirstNamePropertyName, ref mFirstName, value );
            }
        }
        private readonly string mFirstNamePropertyName = "FirstName";
        private string mFirstName;

        public string LastName
        {
            get
            {
                return mLastName;
            }

            set
            {
                Set( mLastNamePropertyName, ref mLastName, value );
            }
        }
        private readonly string mLastNamePropertyName = "LastName";
        private string mLastName;

        [XmlIgnore]
        public bool IsDirty { get; private set; }

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }
    }
}
