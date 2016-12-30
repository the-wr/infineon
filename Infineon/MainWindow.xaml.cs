using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;
using Infineon.Model;

namespace Infineon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow: Window
    {
        private Config config;
        private Data data;

        private PresetsPicker presetsPicker;
        private Uploader uploader;
        private HelpWindow helpWindow;

        private bool muted;
        private string lastHelpKey = "Index";

        public MainWindow()
        {
            InitializeComponent();

            helpWindow = new HelpWindow( wb );

            tbCont6.Checked += OnTbControllerTypeChecked;
            tbCont12.Checked += OnTbControllerTypeChecked;
            tbCont18.Checked += OnTbControllerTypeChecked;

            tbCont6.Unchecked += OnTbControllerTypeChecked;
            tbCont12.Unchecked += OnTbControllerTypeChecked;
            tbCont18.Unchecked += OnTbControllerTypeChecked;

            Init();

            btnSave.Click += OnSaveClicked;
            btnSaveAs.Click += OnSaveAsClicked;
            btnDefault.Click += OnDefaultClicked;

            btnUpload.Click += OnBtnUploadClicked;
            cbPort.DropDownOpened += OnPortDropDownOpened;

            Closing += delegate { SaveConfig(); uploader.Dispose(); };

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

            if ( config.LastControllerType == "F12" )
                data = LoadDefaultData( InfineonDesc.F12 );
            else if ( config.LastControllerType == "F18" )
                data = LoadDefaultData( InfineonDesc.F18 );
            else
                data = LoadDefaultData( InfineonDesc.F6 );

            cbPort.Text = config.LastPort;

            foreach ( var lang in Localization.Instance.Languages )
                cbLanguages.Items.Add( lang );

            cbLanguages.SelectionChanged += OnLanguageChanged;
            cbLanguages.SelectedItem = Localization.Instance.Languages.Contains( config.Language ) ? config.Language : "English";


            presetsPicker = new PresetsPicker();
            gridPresets.Children.Add( presetsPicker );
            presetsPicker.PresetSelected += OnPresetSelected;
            presetsPicker.Setup( config.LastControllerType, config.LastPreset );

            muted = true;
            UpdateControllerTypeButtons();
            muted = false;

            uploader = new Uploader();
            uploader.OnSuccess += delegate
            {
                imgError.Visibility = Visibility.Collapsed;
                imgOk.Visibility = Visibility.Visible;
                tbUploadMessage.Text = Localization.Instance.GetString(9);
            };
            uploader.OnError += delegate(string error)
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

            BindHelpToControl( slBatteryCurrent, "BatteryCurrent" );
            BindHelpToControl( slPhaseCurrent, "PhaseCurrent" );
        }

        private Data LoadDefaultData( InfineonDesc desc )
        {
            return new Data
            {
                Type = desc.Type,
                Desc = desc,
                BatteryCurrent = 128,
                PhaseCurrent = 128,

                OnePedalMode = false,
                HallsAngle = true,
                MinVoltageTolerance = 2,
                ThreePosMode = 1,
                ThrottleProtection = true,
                PASMaxSpeed = 27,
                PASPulsesToSkip = 5,
                RegenEnabled = true,
                MinVoltage = 84,
                Speed1Precentage = 24,
                Speed2Precentage = 80,
                Speed3Precentage = 104,
                Speed4Precentage = 24,
                RegenMaxVoltage = 168,
                RegenStrength = 50,
                ReverseSpeed = 16
            };
        }

        private void UpdateControllerTypeButtons()
        {
            tbCont6.IsChecked = data.Desc == InfineonDesc.F6;
            tbCont12.IsChecked = data.Desc == InfineonDesc.F12;
            tbCont18.IsChecked = data.Desc == InfineonDesc.F18;
        }

        private void UpdateControls()
        {
            var l = Localization.Instance;

            slBatteryCurrent.Setup( l.GetString( 13 ), 0, 255, data.Desc.BatteryCurrentMultiplier, data.BatteryCurrent, v => data.BatteryCurrent = v );
            slPhaseCurrent.Setup( l.GetString( 14 ), 0, 255, data.Desc.PhaseCurrentMultiplier, data.PhaseCurrent, v => data.PhaseCurrent = v );

            slSpeed1.Setup( l.GetString( 15 ), 0, 104, data.Desc.SpeedMultiplier, data.Speed1Precentage, v => data.Speed1Precentage = v );
            slSpeed2.Setup( l.GetString( 16 ), 0, 104, data.Desc.SpeedMultiplier, data.Speed2Precentage, v => data.Speed2Precentage = v );
            slSpeed3.Setup( l.GetString( 17 ), 0, 104, data.Desc.SpeedMultiplier, data.Speed3Precentage, v => data.Speed3Precentage = v );
            slSpeed4.Setup( l.GetString( 18 ), 24, 95, data.Desc.SpeedMultiplier, data.Speed4Precentage, v => data.Speed4Precentage = v );

            slMinVoltage.Setup( l.GetString( 19 ), 0, 255, data.Desc.LVCMultiplier, data.MinVoltage, v => data.MinVoltage = v );
            slMinVoltageTolerance.Setup( l.GetString( 20 ), 0, 255, data.Desc.LVCMultiplier, data.MinVoltageTolerance, v => data.MinVoltageTolerance = v );

            cbRegenEnabled.Setup( l.GetString( 21 ), data.RegenEnabled, v => data.RegenEnabled = v );
            slRegenStr.Setup( l.GetString( 22 ), 0, 200, 1, data.RegenStrength, v => data.RegenStrength = v );
            slRegenVoltage.Setup( l.GetString( 23 ), 0, 255, data.Desc.LVCMultiplier, data.RegenMaxVoltage, v => data.RegenMaxVoltage = v );

            slPasMaxSpeed.Setup( l.GetString( 24 ), 0, 128, data.Desc.PASSpeedMultiplier, data.PASMaxSpeed, v => data.PASMaxSpeed = v );
            slPasPulses.Setup( l.GetString( 25 ), 1, 15, 1, data.PASPulsesToSkip, v => data.PASPulsesToSkip = v );

            sl3PosMode.Setup( l.GetString( 26 ), 1, 4, 1, data.ThreePosMode, v => data.ThreePosMode = v );
            slReverseSpeed.Setup( l.GetString( 27 ), 0, 128, data.Desc.ReverseSpeedMultiplier, data.ReverseSpeed, v => data.ReverseSpeed = v );
            cbOnePedalMode.Setup( l.GetString( 28 ), data.OnePedalMode, v => data.OnePedalMode = v );
            cbThrotteProtection.Setup( l.GetString( 29 ), data.ThrottleProtection, v => data.ThrottleProtection = v );
            cbHallsAngle.Setup( l.GetString( 30 ), data.HallsAngle, v => data.HallsAngle = v );
        }

        private void SaveConfig()
        {
            config.LastControllerType = data.Desc.Type;

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
                        data = new XmlSerializer( typeof (Data) ).Deserialize( reader ) as Data;
                        if ( data == null )
                            data = LoadDefaultData( InfineonDesc.F6 ); // todo

                        if ( data.Type == "F6" )
                            data.Desc = InfineonDesc.F6;
                        if ( data.Type == "F12" )
                            data.Desc = InfineonDesc.F12;
                        if ( data.Type == "F18" )
                            data.Desc = InfineonDesc.F18;
                    }

                }
                catch ( Exception ex )
                {
                    MessageBox.Show( "Can't read preset file: " + shortName, "Sum Ting Wong" );
                }
            }
            else
            {
                data = LoadDefaultData( data.Desc );
            }

            config.LastControllerType = data.Desc.Type;
            config.LastPreset = shortName;

            UpdateControls();
        }

        // -----

        private void OnTbControllerTypeChecked( object sender, RoutedEventArgs e )
        {
            if ( muted )
                return;

            if ( ReferenceEquals( sender, tbCont6 ) )
                data.Desc = InfineonDesc.F6;
            if ( ReferenceEquals( sender, tbCont12 ) )
                data.Desc = InfineonDesc.F12;
            if ( ReferenceEquals( sender, tbCont18 ) )
                data.Desc = InfineonDesc.F18;

            presetsPicker.Setup( data.Desc.Type, string.Empty );

            muted = true;
            UpdateControllerTypeButtons();
            muted = false;

            UpdateControls();
        }

        private void OnSaveClicked( object sender, RoutedEventArgs e )
        {
            if ( string.IsNullOrEmpty( config.LastPreset ) )
            {
                OnSaveAsClicked( sender, e );
                return;
            }

            using ( var writer = new StreamWriter( "Presets\\" + config.LastPreset + ".xml" ) )
                new XmlSerializer( typeof( Data ) ).Serialize( writer, data );

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
                    // Save document
                    string filename = dlg.FileName;

                    using ( var writer = new StreamWriter( filename ) )
                        new XmlSerializer( typeof (Data) ).Serialize( writer, data );

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
            data = LoadDefaultData( data.Desc );
            UpdateControls();
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

        private void OnLanguageChanged( object sender, System.Windows.Controls.SelectionChangedEventArgs e )
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

            slBatteryCurrent.SetCaption( l.GetString( 13 ) );//( "Battery current"
            slPhaseCurrent.SetCaption( l.GetString( 14 ) );//( "Phase current"

            slSpeed1.SetCaption( l.GetString( 15 ) );//( "Speed 1 percent"
            slSpeed2.SetCaption( l.GetString( 16 ) );//( "Speed 2 percent"
            slSpeed3.SetCaption( l.GetString( 17 ) );//( "Speed 3 percent"
            slSpeed4.SetCaption( l.GetString( 18 ) );//( "Speed 4 percent"

            slMinVoltage.SetCaption( l.GetString( 19 ) );//( "Min voltage"
            slMinVoltageTolerance.SetCaption( l.GetString( 20 ) );//( "Tolerance"

            cbRegenEnabled.SetCaption( l.GetString( 21 ) );//( "Regen enabled"
            slRegenStr.SetCaption( l.GetString( 22 ) );//( "Regen strength"
            slRegenVoltage.SetCaption( l.GetString( 23 ) );//( "Regen max voltage"

            slPasMaxSpeed.SetCaption( l.GetString( 24 ) );//( "PAS max speed"
            slPasPulses.SetCaption( l.GetString( 25 ) );//( "Pas pulses to skip"

            sl3PosMode.SetCaption( l.GetString( 26 ) );//( "Speed switch mode"
            slReverseSpeed.SetCaption( l.GetString( 27 ) );//( "Reverse speed"
            cbOnePedalMode.SetCaption( l.GetString( 28 ) );//( "One-pedal mode"
            cbThrotteProtection.SetCaption( l.GetString( 29 ) );//( "Throttle protection"
            cbHallsAngle.SetCaption( l.GetString( 30 ) );//( "Halls angle"

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
