using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;
using Infineon.Model;
using System.Collections.Generic;
using System.Windows.Controls;
using Infineon.UI;

namespace Infineon
{
    public partial class MainWindow : Window
    {
        private Config config;

        private IControllerDesc currentDesc;
        private IData data;
        private IUIControls controls;

        private PresetsPicker presetsPicker;
        private Uploader uploader;
        private HelpWindow helpWindow;

        private string lastHelpKey = "Index";
        private bool mutePresetPicker;

        private List<IControllerDesc> controllerDescs = new List<IControllerDesc>();

        public MainWindow()
        {
            InitializeComponent();

            helpWindow = new HelpWindow( wb );

            cbControllerType.SelectionChanged += OnCbControllerTypeSelectionChanged;

            btnSave.Click += OnSaveClicked;
            btnSaveAs.Click += OnSaveAsClicked;
            btnDefault.Click += OnDefaultClicked;

            btnUpload.Click += OnBtnUploadClicked;
            cbPort.DropDownOpened += OnPortDropDownOpened;

            Closing += delegate { SaveConfig(); uploader.Dispose(); };
            /*
            controllerDescs.Add( new ControllerDesc( "Inf3_F6", "Infineon 3 (6-FET)",
                data => new Inf3Controls( data ),
                () => new InfData( InfDesc.Inf3_F6 ),
                data => { ( data as InfData ).Desc = InfDesc.Inf3_F6; } ) );
                */
            controllerDescs.Add( new ControllerDesc( "Inf3_F12", "Infineon 3 (12-FET)",
                data => new Inf3Controls( data ),
                () => new InfData( InfDesc.Inf3_F12 ),
                data => { ( data as InfData ).Desc = InfDesc.Inf3_F12; } ) );
            /*
            controllerDescs.Add( new ControllerDesc( "Inf3_F18", "Infineon 3 (18-FET)",
                data => new Inf3Controls( data ),
                () => new InfData( InfDesc.Inf3_F18 ),
                data => { ( data as InfData ).Desc = InfDesc.Inf3_F18; } ) );
                */
            controllerDescs.Add( new ControllerDesc( "F6", "Infineon 4 (6-FET)",
                data => new Inf4Controls( data ),
                () => new InfData( InfDesc.Inf4_F6 ),
                data => { ( data as InfData ).Desc = InfDesc.Inf4_F6; } ) );

            controllerDescs.Add( new ControllerDesc( "F12", "Infineon 4 (12-FET)",
                data => new Inf4Controls( data ),
                () => new InfData( InfDesc.Inf4_F12 ),
                data => { ( data as InfData ).Desc = InfDesc.Inf4_F12; } ) );

            controllerDescs.Add( new ControllerDesc( "F18", "Infineon 4 (18-FET)",
                data => new Inf4Controls( data ),
                () => new InfData( InfDesc.Inf4_F18 ),
                data => { ( data as InfData ).Desc = InfDesc.Inf4_F6; } ) );

            Init();
        }

        private void Init()
        {
            if ( !Directory.Exists( "Presets" ) )
                Directory.CreateDirectory( "Presets" );

            try
            {
                using ( var reader = new StreamReader( "Config.xml" ) )
                    config = new XmlSerializer( typeof( Config ) ).Deserialize( reader ) as Config;
            }
            catch ( Exception ) { }

            if ( config == null )
                config = new Config() { LastControllerType = "F6" };

            cbPort.Text = config.LastPort;

            foreach ( var lang in Localization.Instance.Languages )
                cbLanguages.Items.Add( lang );

            cbLanguages.SelectionChanged += OnLanguageChanged;
            cbLanguages.SelectedItem = Localization.Instance.Languages.Contains( config.Language ) ? config.Language : "English";

            foreach ( var desc in controllerDescs )
            {
                var item = new ComboBoxItem() { Content = desc.Name, Tag = desc };
                cbControllerType.Items.Add( item );

                if ( config.LastControllerType == desc.Id )
                    cbControllerType.SelectedItem = item;
            }

            if ( cbControllerType.SelectedItem == null && cbControllerType.Items.Count > 0 )
                cbControllerType.SelectedItem = cbControllerType.Items[0];

            presetsPicker = new PresetsPicker();
            gridPresets.Children.Add( presetsPicker );
            presetsPicker.PresetSelected += OnPresetSelected;
            presetsPicker.Setup( config.LastControllerType, config.LastPreset );

            uploader = new Uploader();
            uploader.OnSuccess += delegate
            {
                imgError.Visibility = Visibility.Collapsed;
                imgOk.Visibility = Visibility.Visible;
                tbUploadMessage.Text = Localization.Instance.GetString( 9 );
            };
            uploader.OnError += delegate ( string error )
            {
                imgError.Visibility = Visibility.Visible;
                imgOk.Visibility = Visibility.Collapsed;
                tbUploadMessage.Text = Localization.Instance.GetString( 10 ) + error;
            };
            uploader.OnWaitingForButton += delegate
            {
                imgError.Visibility = Visibility.Collapsed;
                imgOk.Visibility = Visibility.Collapsed;
                tbUploadMessage.Text = Localization.Instance.GetString( 11 );
            };
            uploader.OnWaitingForReply += delegate
            {
                imgError.Visibility = Visibility.Collapsed;
                imgOk.Visibility = Visibility.Collapsed;
                tbUploadMessage.Text = Localization.Instance.GetString( 12 );
            };

            imgLogo.MouseDown += delegate { Process.Start( Localization.Instance.GetString( 0 ) ); };

            BindHelpToControl( btnDefault, "ResetToDefault" );
            BindHelpToControl( btnSave, "SavePreset" );
            BindHelpToControl( btnSaveAs, "SavePreset" );
        }

        private void SetControllerType( IControllerDesc desc, IData newData = null )
        {
            currentDesc = desc;
            data = newData;

            if ( data == null )
                data = desc.CreateData();

            controls = desc.CreateControls( data );
            
            gridControls.Children.Clear();
            gridControls.Children.Add( controls.FrameworkElement );

            controls.ShowHelp += ( helpKey ) =>
            {
                lastHelpKey = helpKey;
                helpWindow.ShowHelp( helpKey );
            };

            if ( !mutePresetPicker && presetsPicker != null )
                presetsPicker.Setup( data.Type, config.LastPreset );
        }

        private void SaveConfig()
        {
            try
            {
                using ( var writer = new StreamWriter( "Config.xml" ) )
                    new XmlSerializer( typeof( Config ) ).Serialize( writer, config );
            }
            catch ( Exception ) { }
        }

        // -----

        private void OnPresetSelected( string shortName )
        {
            if ( !string.IsNullOrEmpty( shortName ) )
            {
                try
                {
                    using ( var reader = new StreamReader( "Presets\\" + shortName + ".xml" ) )
                    {
                        var dataDs = new XmlSerializer( typeof (DataDS) ).Deserialize( reader ) as DataDS;
                        if ( dataDs == null )
                            return;

                        var desc = controllerDescs.FirstOrDefault( ( d ) => d.Id == dataDs.Data.Type );
                        if ( desc == null )
                            return;

                        desc.PostLoad( dataDs.Data );

                        mutePresetPicker = true;
                        SetControllerType( desc, dataDs.Data );
                        mutePresetPicker = false;
                    }

                }
                catch ( Exception ex )
                {
                    MessageBox.Show( "Can't read preset file: " + shortName, "Sum Ting Wong" );
                }
            }
         
            config.LastControllerType = data.Type;
            config.LastPreset = shortName;
        }

        // -----

        private void OnCbControllerTypeSelectionChanged( object sender, SelectionChangedEventArgs e )
        {
            var item = cbControllerType.SelectedItem;
            if ( item is ComboBoxItem )
                SetControllerType( ( item as ComboBoxItem ).Tag as IControllerDesc );
        }

        private void OnSaveClicked( object sender, RoutedEventArgs e )
        {
            if ( string.IsNullOrEmpty( config.LastPreset ) )
            {
                OnSaveAsClicked( sender, e );
                return;
            }

            using ( var writer = new StreamWriter( "Presets\\" + config.LastPreset + ".xml" ) )
                new XmlSerializer( typeof( DataDS ) ).Serialize( writer, new DataDS { Data = data } );

            MessageBox.Show( Localization.Instance.GetString( 32 ) + config.LastPreset );
        }

        private void OnSaveAsClicked( object sender, RoutedEventArgs e )
        {
            try
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
                {
                    InitialDirectory = "Presets",
                    FileName = "",
                    DefaultExt = ".xml",
                    Filter = "Presets (.xml)|*.xml",
                    RestoreDirectory = true
                };

                var result = dlg.ShowDialog();
                if ( result == true )
                {
                    string filename = dlg.FileName;

                    using ( var writer = new StreamWriter( filename ) )
                        new XmlSerializer( typeof( DataDS ) ).Serialize( writer, new DataDS { Data = data } );

                    config.LastPreset = filename.Split( '\\' ).Last().Replace( ".xml", "" );
                }
            }
            catch ( Exception ex )
            {
                MessageBox.Show( Localization.Instance.GetString( 31 ) + "\r\n\r\n" + ex.Message, "Sum Ting Wong" );
            }

            presetsPicker.Setup( config.LastControllerType, config.LastPreset );

            MessageBox.Show( Localization.Instance.GetString( 32 ) + config.LastPreset );
        }

        private void OnDefaultClicked( object sender, RoutedEventArgs e )
        {
            mutePresetPicker = true;
            SetControllerType( currentDesc );
            mutePresetPicker = false;
        }

        private void OnBtnUploadClicked( object sender, RoutedEventArgs e )
        {
            imgOk.Visibility = Visibility.Collapsed;
            imgError.Visibility = Visibility.Collapsed;
            tbUploadMessage.Text = string.Empty;

            uploader.Upload( cbPort.Text, FirmwareBuilder.BuildFirmware( data ) );
            config.LastPort = cbPort.Text;
        }

        private void OnPortDropDownOpened( object sender, EventArgs e )
        {
            cbPort.Items.Clear();

            var ports = SerialPort.GetPortNames().ToList();
            ports.Sort();

            foreach ( var port in ports )
                cbPort.Items.Add( port );
        }

        private void OnLanguageChanged( object sender, SelectionChangedEventArgs e )
        {
            var l = Localization.Instance;
            l.SetLanguage( cbLanguages.SelectedItem.ToString() );

            config.Language = cbLanguages.SelectedItem.ToString();
            SaveConfig();

            tbCaption.Text = l.GetString( 1 );
            tbController.Text = l.GetString( 2 );
            tbPreset.Text = l.GetString( 3 );
            btnDefault.Content = l.GetString( 4 );
            btnSave.Content = l.GetString( 5 );
            btnSaveAs.Content = l.GetString( 6 );
            tbPort.Text = l.GetString( 7 );
            btnUpload.Content = l.GetString( 8 );

            if ( controls != null )
                controls.UpdateLanguage();

            helpWindow.ShowHelp( lastHelpKey );
        }

        private void BindHelpToControl( FrameworkElement element, string helpKey )
        {
            element.MouseEnter += delegate
            {
                lastHelpKey = helpKey;
                helpWindow.ShowHelp( helpKey );
            };
        }
    }
}
