using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Xml.Serialization;
using Infineon.Model;

namespace Infineon
{
    /// <summary>
    /// Interaction logic for PresetsPicker.xaml
    /// </summary>
    public partial class PresetsPicker: UserControl
    {
        public event Action<string> PresetSelected;
        private List<ToggleButton> buttons = new List<ToggleButton>();
        private bool muted;

        public PresetsPicker()
        {
            InitializeComponent();
        }

        public void Setup( string controllerType, string selectedPreset )
        {
            spPresets.Children.Clear();
            buttons.Clear();

            string presetToUse = string.Empty;
            try
            {
                var files = Directory.GetFileSystemEntries( "Presets", "*.xml" );
                foreach ( var file in files )
                {
                    using ( var reader = new StreamReader( file ) )
                    {
                        var data = new XmlSerializer( typeof( Data ) ).Deserialize( reader ) as Data;
                        if ( data != null && data.Type == controllerType )
                        {
                            var shortName = file.Split( '\\' ).Last().Replace( ".xml", "" );
                            var btn = new ToggleButton { Content = shortName, Tag = shortName, MinWidth = 40 };
                            spPresets.Children.Add( btn );
                            buttons.Add( btn );

                            if ( string.IsNullOrEmpty( presetToUse ) || selectedPreset == shortName )
                            {
                                foreach ( var toggleButton in buttons )
                                    toggleButton.IsChecked = false;
                                btn.IsChecked = true;

                                presetToUse = shortName;
                            }
                        }
                    }
                }
            }
            catch ( Exception ex )
            {
            }

            PresetSelected?.Invoke( presetToUse );

            foreach ( var btn in buttons )
            {
                btn.Checked += OnBtnChecked;
                btn.Unchecked += OnBtnChecked;
            }
        }

        private void OnBtnChecked( object sender, RoutedEventArgs e )
        {
            if ( muted )
                return;

            muted = true;

            foreach ( var btn in buttons )
                btn.IsChecked = btn == sender;

            PresetSelected?.Invoke( (sender as ToggleButton).Tag.ToString() );

            muted = false;
        }
    }
}
