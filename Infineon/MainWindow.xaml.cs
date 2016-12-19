using System;
using System.IO;
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

        private bool muted;

        public MainWindow()
        {
            InitializeComponent();

            tbCont6.Checked += OnTbControllerTypeChecked;
            tbCont12.Checked += OnTbControllerTypeChecked;
            tbCont18.Checked += OnTbControllerTypeChecked;

            tbCont6.Unchecked += OnTbControllerTypeChecked;
            tbCont12.Unchecked += OnTbControllerTypeChecked;
            tbCont18.Unchecked += OnTbControllerTypeChecked;

            Init();

            btnSave.Click += OnSaveClicked;
            btnSaveAs.Click += OnSaveAsClicked;

            btnUpload.Click += OnBtnUploadClicked;

            Closing += delegate { SaveConfig(); };
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
                tbUploadMessage.Text = "Success!";
            };
            uploader.OnError += delegate(string error)
            {
                imgError.Visibility = Visibility.Visible;
                imgOk.Visibility = Visibility.Collapsed;
                tbUploadMessage.Text = "Error: " + error;
            };
            uploader.OnWaitingForButton += delegate
            {
                imgError.Visibility = Visibility.Collapsed;
                imgOk.Visibility = Visibility.Collapsed;
                tbUploadMessage.Text = "Press the button...";
            };
            uploader.OnWaitingForReply += delegate
            {
                imgError.Visibility = Visibility.Collapsed;
                imgOk.Visibility = Visibility.Collapsed;
                tbUploadMessage.Text = "Waiting for reply...";
            };
        }

        private Data LoadDefaultData( InfineonDesc desc )
        {
            return new Data { Desc = desc, BatteryCurrent = 100, Type = desc.Type };
        }

        private void UpdateControllerTypeButtons()
        {
            tbCont6.IsChecked = data.Desc == InfineonDesc.F6;
            tbCont12.IsChecked = data.Desc == InfineonDesc.F12;
            tbCont18.IsChecked = data.Desc == InfineonDesc.F18;
        }

        private void UpdateControls()
        {
            slBatteryCurrent.Setup( "Battery current", 0, 255, data.Desc.BatteryCurrentMultiplier, data.BatteryCurrent, v => data.BatteryCurrent = v );
            slPhaseCurrent.Setup( "Phase current", 0, 255, data.Desc.PhaseCurrentMultiplier, data.PhaseCurrent, v => data.PhaseCurrent = v );

            slSpeed1.Setup( "Speed 1 Percent", 0, 104, data.Desc.SpeedMultiplier, data.Speed1Precentage, v => data.Speed1Precentage = v );
            slSpeed2.Setup( "Speed 2 Percent", 0, 104, data.Desc.SpeedMultiplier, data.Speed2Precentage, v => data.Speed2Precentage = v );
            slSpeed3.Setup( "Speed 3 Percent", 0, 104, data.Desc.SpeedMultiplier, data.Speed3Precentage, v => data.Speed3Precentage = v );
            slSpeed4.Setup( "Speed 4 Percent", 0, 104, data.Desc.SpeedMultiplier, data.Speed4Precentage, v => data.Speed4Precentage = v );

            slMinVoltage.Setup( "Min Voltage", 0, 255, data.Desc.LVCMultiplier, data.MinVoltage, v => data.MinVoltage = v );
            slMinVoltageTolerance.Setup( "Tolerance", 0, 255, data.Desc.LVCMultiplier, data.MinVoltageTolerance, v => data.MinVoltageTolerance = v );

            slRegenStr.Setup( "Regen Strength", 0, 200, 1, data.RegenStrength, v => data.RegenStrength = v );
            slRegenVoltage.Setup( "Regen Max Voltage", 0, 255, data.Desc.LVCMultiplier, data.RegenMaxVoltage, v => data.RegenMaxVoltage = v );
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

            MessageBox.Show( $"Preset {config.LastPreset} saved." );
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
                MessageBox.Show( "Can't save preset:\r\n\r\n" + ex.Message, "Sum Ting Wong" );
            }

            presetsPicker.Setup( config.LastControllerType, config.LastPreset );

            MessageBox.Show( $"Preset {config.LastPreset} saved." );
        }

        private void OnBtnUploadClicked( object sender, RoutedEventArgs e )
        {
            imgOk.Visibility = Visibility.Collapsed;
            imgError.Visibility = Visibility.Collapsed;
            tbUploadMessage.Text = string.Empty;

            uploader.Upload( cbPort.Text, FirmwareBuilder.BuildFirmware( data ) );
        }
    }
}
