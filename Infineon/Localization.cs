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

        private readonly List<string> languages = new List<string>();

        private readonly List<string> strings = new List<string>();
        private readonly Dictionary<string, string> help = new Dictionary<string, string>();

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

        public string Styles { get; private set; }

        public void SetLanguage( string lang )
        {
            strings.Clear();
            help.Clear();

            Styles = File.ReadAllText( $"Languages\\{lang}\\Styles.css" );
            strings.AddRange( File.ReadAllLines( $"Languages\\{lang}\\Strings.txt" ) );

            foreach ( var file in Directory.GetFiles( $"Languages\\{lang}", "*.html" ) )
            {
                var name = file.Replace( ".html", "" ).Split( '\\' ).Last();
                var html = File.ReadAllText( file, Encoding.UTF8 );
                html = html.Replace( "file://", $"file://{Environment.CurrentDirectory}/Languages/{lang}/" );
                help.Add( name, html );
            }
        }

        public string GetString( int index )
        {
            if ( index >= strings.Count )
                return string.Empty;

            return strings[index];
        }

        public string GetHelp( string key )
        {
            if ( help.ContainsKey( key ) )
                return help[key];

            return string.Empty;
        }
    }
}
