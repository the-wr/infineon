using Infineon.Model;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Infineon.UI
{
    /// <summary>
    /// Interaction logic for Inf4Controls.xaml
    /// </summary>
    public partial class Inf3Controls : UserControl, IUIControls
    {
        private InfData data;

        public event Action<string> ShowHelp;

        public Inf3Controls(IData data)
        {
            InitializeComponent();

            this.data = data as InfData;
            InitControls();
        }

        public FrameworkElement FrameworkElement => this;

        public void UpdateLanguage()
        {
            var l = Localization.Instance;

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
            slMaxVoltage.SetCaption( l.GetString( 23 ) );//( "Regen max voltage"

            slPasMaxSpeed.SetCaption( l.GetString( 24 ) );//( "PAS max speed"
            slPasPulses.SetCaption( l.GetString( 25 ) );//( "Pas pulses to skip"

            sl3PosMode.SetCaption( l.GetString( 26 ) );//( "Speed switch mode"
            slReverseSpeed.SetCaption( l.GetString( 27 ) );//( "Reverse speed"
            cbOnePedalMode.SetCaption( l.GetString( 28 ) );//( "One-pedal mode"
            cbThrotteProtection.SetCaption( l.GetString( 29 ) );//( "Throttle protection"
            cbHallsAngle.SetCaption( l.GetString( 30 ) );//( "Halls angle"
        }

        void InitControls()
        {
            var l = Localization.Instance;

            slBatteryCurrent.Setup( l.GetString( 13 ), 0, data.Desc.BatteryCurrentLimit, data.Desc.BatteryCurrentMultiplier, 0, data.Desc.BatteryCurrentOffset, data.BatteryCurrent, v => data.BatteryCurrent = v );
            slPhaseCurrent.Setup( l.GetString( 14 ), 0, 255, data.Desc.PhaseCurrentMultiplier, 0, data.Desc.PhaseCurrentOffset, data.PhaseCurrent, v => data.PhaseCurrent = v );

            slSpeed1.Setup( l.GetString( 15 ), 24, 95, data.Desc.SpeedMultiplier, 0, 0, data.Speed1Percentage, v => data.Speed1Percentage = v );
            slSpeed2.Setup( l.GetString( 16 ), 24, 95, data.Desc.SpeedMultiplier, 0, 0, data.Speed2Percentage, v => data.Speed2Percentage = v );
            slSpeed3.Setup( l.GetString( 17 ), 24, 95, data.Desc.SpeedMultiplier, 0, 0, data.Speed3Percentage, v => data.Speed3Percentage = v );
            slSpeed4.Setup( l.GetString( 18 ), 24, 95, data.Desc.SpeedMultiplier, 0, 0, data.Speed4Percentage, v => data.Speed4Percentage = v );

            slMinVoltage.Setup( l.GetString( 19 ), data.Desc.MinVoltageRangeLimitMin, data.Desc.VoltageRangeLimitMax, data.Desc.LVCMultiplier, 0, 0, data.MinVoltage, v => data.MinVoltage = v );
            slMinVoltageTolerance.Setup( l.GetString( 20 ), 0, data.Desc.VoltageRangeLimitMax, data.Desc.LVCMultiplier, 0, 0, data.MinVoltageTolerance, v => data.MinVoltageTolerance = v );

            cbRegenEnabled.Setup( l.GetString( 21 ), data.RegenEnabled, v => data.RegenEnabled = v );
            slRegenStr.Setup( l.GetString( 22 ), 1, 3, 1, 0, 0, data.RegenStrength, v => data.RegenStrength = v );
            slMaxVoltage.Setup( l.GetString( 23 ), data.Desc.VoltageRangeLimitMin, data.Desc.VoltageRangeLimitMax, data.Desc.LVCMaxMultiplier, data.Desc.LVCMaxOffset, 0, data.RegenMaxVoltage, v => data.RegenMaxVoltage = v );

            slPasMaxSpeed.Setup( l.GetString( 24 ), 0, 191, data.Desc.PASSpeedMultiplier, 0, 0, data.PASMaxSpeed, v => data.PASMaxSpeed = v );
            slPasPulses.Setup( l.GetString( 25 ), 1, 15, 1, 0, 0, data.PASPulsesToSkip, v => data.PASPulsesToSkip = v );

            sl3PosMode.Setup( l.GetString( 26 ), 1, 4, 1, 0, 0, data.ThreePosMode, v => data.ThreePosMode = v );
            slReverseSpeed.Setup( l.GetString( 27 ), 0, 128, data.Desc.ReverseSpeedMultiplier, 0, 0, data.ReverseSpeed, v => data.ReverseSpeed = v );
            cbOnePedalMode.Setup( l.GetString( 28 ), data.OnePedalMode, v => data.OnePedalMode = v );
            cbThrotteProtection.Setup( l.GetString( 29 ), data.ThrottleProtection, v => data.ThrottleProtection = v );
            cbHallsAngle.Setup( l.GetString( 30 ), data.HallsAngle, v => data.HallsAngle = v );

            BindHelpToControl( slBatteryCurrent, "BatteryCurrent" );
            BindHelpToControl( slPhaseCurrent, "PhaseCurrent" );

            BindHelpToControl( slSpeed1, "Speed1" );
            BindHelpToControl( slSpeed2, "Speed2" );
            BindHelpToControl( slSpeed3, "Speed3" );
            BindHelpToControl( slSpeed4, "Speed4" );

            BindHelpToControl( slMinVoltage, "MinVoltage" );
            BindHelpToControl( slMinVoltageTolerance, "VoltageTolerance" );

            BindHelpToControl( cbRegenEnabled, "RegenEnabled" );
            BindHelpToControl( slRegenStr, "RegenStrength" );
            BindHelpToControl( slMaxVoltage, "MaxVoltage" );

            BindHelpToControl( slPasMaxSpeed, "PasMaxSpeed" );
            BindHelpToControl( slPasPulses, "PasPulses" );

            BindHelpToControl( sl3PosMode, "3PosMode" );
            BindHelpToControl( slReverseSpeed, "ReverseSpeed" );
            BindHelpToControl( cbOnePedalMode, "OnePedalMode" );
            BindHelpToControl( cbThrotteProtection, "ThrottleProtection" );
            BindHelpToControl( cbHallsAngle, "HallsAngle" );
        }

        private void BindHelpToControl( FrameworkElement element, string helpKey )
        {
            element.MouseEnter += delegate
            {
                ShowHelp?.Invoke( helpKey );
            };
        }
    }
}
