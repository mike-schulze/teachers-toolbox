using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace TeachingToolbox.Model
{
    public class DataService : IDataService
    {
        public DataService()
        {
            mDataPath = Properties.Settings.Default.DatabasePath;

            mSections = new List<Section> ();
            mStudents = new List<Student> ();

            if( !Directory.Exists( mDataPath ) )
            {
                Directory.CreateDirectory( mDataPath );
            }

            if ( !Directory.Exists( SectionsPath() ) )
            {
                Directory.CreateDirectory( SectionsPath() );
            }

            if( !Directory.Exists( StudentsPath() ) )
            {
                Directory.CreateDirectory( StudentsPath() );
            }

            Load();
        }

        public void Load()
        {
            var theStudentSerializer = new XmlSerializer( typeof( Student ) );
            foreach( var theFile in Directory.GetFiles( StudentsPath(), "*.xml" ) )
            {
                using( var theReader = new StreamReader( theFile ) )
                {
                    var theStudent = ( Student ) theStudentSerializer.Deserialize( theReader );
                    mStudents.Add( theStudent );                    
                }
            }

            var theSectionSerializer = new XmlSerializer( typeof( Section ) );
            foreach( var theFile in Directory.GetFiles( SectionsPath(), "*.xml" ) )
            {
                using( var theReader = new StreamReader ( theFile ) )
                {
                    var theSection = ( Section ) theSectionSerializer.Deserialize( theReader );
                    mSections.Add( theSection );                    
                }
            }
        }

        public void Save()
        {
            var theStudentSerializer = new XmlSerializer( typeof( Student ) );
            foreach( var theStudent in mStudents )
            {
                using( var theWriter = new StreamWriter( StudentsPath( theStudent ) ) )
                {
                    theStudentSerializer.Serialize( theWriter, theStudent );    
                }                
            }

            var theSectionSerializer = new XmlSerializer( typeof( Section ) );
            foreach( var theSection in mSections )
            {
                using( var theWriter = new StreamWriter( SectionsPath( theSection ) ) )
                {
                    theSectionSerializer.Serialize( theWriter, theSection );    
                }                
            }
        }

        public IEnumerable<Section> GetSections()
        {
            return mSections;
        }

        public void AddSection( Section aSection )
        {
            mSections.Add( aSection );
            Save();
        }

        public void DeleteSection( Section aSection )
        {
            mSections.Remove( aSection );
            File.Delete( SectionsPath( aSection ) );
        }

        public Student GetStudent( Guid aStudentId )
        {
            return mStudents.FirstOrDefault( aStudent => aStudent.Id == aStudentId );
        }

        public IEnumerable<Student> GetStudents()
        {
            return mStudents;       
        }

        public void AddStudent( Student aStudent )
        {
            mStudents.Add( aStudent );
            Save();
        }

        public void DeleteStudent( Student aStudent )
        {
            foreach( var theSection in mSections )
            {
                if( theSection.StudentIds.Any( ( theGuid ) => theGuid == aStudent.Id ) )
                {
                    theSection.StudentIds.Remove( aStudent.Id );
                }
            }

            mStudents.Remove( aStudent );
            Save();
        }

        private string SectionsPath ( Section aSection = null )
        {
            string theSubPath = "Sections";

            if( aSection == null )
            {
                return Path.Combine( mDataPath, theSubPath );
            }

            return Path.Combine( mDataPath, theSubPath, aSection.Id.ToString() + ".xml" );
        }

        private string StudentsPath( Student aStudent = null )
        {
            string theSubPath = "Students";

            if( aStudent == null )
            {
                return Path.Combine(mDataPath, theSubPath );
            }

            return Path.Combine(mDataPath, theSubPath, aStudent.Id.ToString() + ".xml");
        }
            
        private readonly List<Section> mSections;
        private readonly List<Student> mStudents;
        private readonly string mDataPath;
    }
}