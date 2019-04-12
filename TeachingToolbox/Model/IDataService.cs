using System;
using System.Collections.Generic;
using System.Linq;

namespace TeachingToolbox.Model
{
    public interface IDataService
    {
        void Load();
        void Save();

        IEnumerable<Section> GetSections();
        void AddSection( Section aSection );
        void DeleteSection( Section aSection );

        Student GetStudent( Guid aStudentId );
        IEnumerable<Student> GetStudents();
        void AddStudent( Student aStudent );
        void DeleteStudent( Student aStudent );
    }
}
