﻿using System;
using System.IO;
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

            UpdateControllerTypeButtons();
        }

        private Data LoadDefaultData( InfineonDesc desc )
        {
            return new Data { Desc = desc, BatteryCurrent = 100 };
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
            if ( data.Desc == InfineonDesc.F6 )
                config.LastControllerType = "F6";
            if ( data.Desc == InfineonDesc.F12 )
                config.LastControllerType = "F12";
            if ( data.Desc == InfineonDesc.F18 )
                config.LastControllerType = "F18";

            try
            {
                using ( var writer = new StreamWriter( "Config.xml" ) )
                    new XmlSerializer( typeof( Config ) ).Serialize( writer, config );
            }
            catch ( Exception ) { }
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

            data = LoadDefaultData( data.Desc );

            muted = true;
            UpdateControllerTypeButtons();
            muted = false;

            UpdateControls();
        }

    }
}
