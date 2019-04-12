using System;
using System.Collections.Generic;
using TeachingToolbox.Model;

namespace TeachingToolbox.Design
{
    public class DesignDataService : IDataService
    {
        public void Load()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public System.Collections.Generic.IEnumerable<Section> GetSections()
        {
            var theSections = new List<Section> ();
            theSections.Add( new Section( "GER 101", "MWF", "9:20am" ) );
            theSections.Add( new Section( "GER 102", "TTh", "8:10am" ) );
            theSections.Add( new Section( "GER 201", "MWF", "10:40am" ) );
            theSections.Add( new Section( "GER 202", "MWF", "11:50am" ) );
            theSections.Add( new Section( "GER 315", "TTh", "1:25pm" ) );
            return theSections;
        }

        public void AddSection( Section aSection )
        {
            throw new NotImplementedException();
        }

        public void DeleteSection( Section aSection )
        {
            throw new NotImplementedException();
        }

        public Student GetStudent( Guid aStudentId )
        {
            throw new NotImplementedException();
        }

        public System.Collections.Generic.IEnumerable<Student> GetStudents()
        {
            throw new NotImplementedException();
        }

        public void AddStudent( Student aStudent )
        {
            throw new NotImplementedException();
        }

        public void DeleteStudent( Student aStudent )
        {
            throw new NotImplementedException();
        }
    }
}