using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Infineon
{
    public class Localization
    {
        private static Localization instance;
        public static Localization Instance => instance ?? ( instance = new Localization() );

        private List<string> languages = new List<string>();

        private List<string> strings = new List<string>();

        private Localization()
        {
            var dirs = Directory.GetDirectories( "Languages" );
            foreach ( var dir in dirs )
            {
                if ( File.Exists( $"{dir}\\Strings.txt" ) )
                    languages.Add( dir.Split( '\\' ).Last() );
            }
        }

        public IEnumerable<string> Languages => languages;

        public void SetLanguage( string lang )
        {
            strings.Clear();
            strings.AddRange( File.ReadAllLines( $"Languages\\{lang}\\Strings.txt" ) );
        }

        public string GetString( int index )
        {
            if ( index >= strings.Count )
                return string.Empty;

            return strings[index];
        }
    }
}
