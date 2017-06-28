using Infineon.Model;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Infineon.UI
{
    /// <summary>
    /// Interaction logic for Inf4Controls.xaml
    /// </summary>
    public partial class Inf4Controls : UserControl, IUIControls
    {
        private Inf4Data data;

        public event Action<string> ShowHelp;

        public Inf4Controls(IData data)
        {
            InitializeComponent();

            this.data = data as Inf4Data;
            InitControls();
        }

        public FrameworkElement FrameworkElement => this;

        public void UpdateLanguage()
        {
            var l = Localization.Instance;

            slBatteryCurrent.SetCaption( l.GetString( 13 ) );//( "Battery current"
            slPhaseCurrent.SetCaption( l.GetString( 14 ) );//( "Phase current"

            slSpeed1.SetCaption( l.GetString( 15 ) );//( "Speed 1 percent"
            slSpeed1Current.SetCaption( l.GetString( 33 ) );

            slSpeed2.SetCaption( l.GetString( 16 ) );//( "Speed 2 percent"
            slSpeed2Current.SetCaption( l.GetString( 34 ) );//( "Speed 1 percent"

            slSpeed3.SetCaption( l.GetString( 17 ) );//( "Speed 3 percent"
            slSpeed3Current.SetCaption( l.GetString( 35 ) );//( "Speed 1 percent"

            slSpeed4.SetCaption( l.GetString( 18 ) );//( "Speed 4 percent"
            slSpeed4Current.SetCaption( l.GetString( 36 ) );//( "Speed 1 percent"

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

            slBatteryCurrent.Setup( l.GetString( 13 ), 0, 255, data.Desc.BatteryCurrentMultiplier, 0, data.BatteryCurrent, v => data.BatteryCurrent = v );
            slPhaseCurrent.Setup( l.GetString( 14 ), 0, 255, data.Desc.PhaseCurrentMultiplier, 0, data.PhaseCurrent, v => data.PhaseCurrent = v );

            slSpeed1.Setup( l.GetString( 15 ), 0, 104, data.Desc.SpeedMultiplier, 0, data.Speed1Percentage, v => data.Speed1Percentage = v );
            slSpeed1Current.Setup( l.GetString( 33 ), 0, 141, data.Desc.CurrentMultiplierPercent, 0, data.Speed1CurrentPercentage, v => data.Speed1CurrentPercentage = v );
            slSpeed2.Setup( l.GetString( 16 ), 0, 104, data.Desc.SpeedMultiplier, 0, data.Speed2Percentage, v => data.Speed2Percentage = v );
            slSpeed2Current.Setup( l.GetString( 34 ), 0, 141, data.Desc.CurrentMultiplierPercent, 0, data.Speed2CurrentPercentage, v => data.Speed2CurrentPercentage = v );
            slSpeed3.Setup( l.GetString( 17 ), 0, 104, data.Desc.SpeedMultiplier, 0, data.Speed3Percentage, v => data.Speed3Percentage = v );
            slSpeed3Current.Setup( l.GetString( 35 ), 0, 141, data.Desc.CurrentMultiplierPercent, 0, data.Speed3CurrentPercentage, v => data.Speed3CurrentPercentage = v );
            slSpeed4.Setup( l.GetString( 18 ), 24, 95, data.Desc.SpeedMultiplier, 0, data.Speed4Percentage, v => data.Speed4Percentage = v );
            slSpeed4Current.Setup( l.GetString( 36 ), 0, 141, data.Desc.CurrentMultiplierPercent, 0, data.Speed4CurrentPercentage, v => data.Speed4CurrentPercentage = v );

            slMinVoltage.Setup( l.GetString( 19 ), data.Desc.MinVoltageRangeLimitMin, data.Desc.VoltageRangeLimitMax, data.Desc.LVCMultiplier, 0, data.MinVoltage, v => data.MinVoltage = v );
            slMinVoltageTolerance.Setup( l.GetString( 20 ), 0, data.Desc.VoltageRangeLimitMax, data.Desc.LVCMultiplier, 0, data.MinVoltageTolerance, v => data.MinVoltageTolerance = v );

            cbRegenEnabled.Setup( l.GetString( 21 ), data.RegenEnabled, v => data.RegenEnabled = v );
            slRegenStr.Setup( l.GetString( 22 ), 0, 200, 1, 0, data.RegenStrength, v => data.RegenStrength = v );
            slMaxVoltage.Setup( l.GetString( 23 ), data.Desc.VoltageRangeLimitMin, data.Desc.VoltageRangeLimitMax, data.Desc.LVCMaxMultiplier, data.Desc.LVCMaxOffset, data.RegenMaxVoltage, v => data.RegenMaxVoltage = v );

            slPasMaxSpeed.Setup( l.GetString( 24 ), 0, 128, data.Desc.PASSpeedMultiplier, 0, data.PASMaxSpeed, v => data.PASMaxSpeed = v );
            slPasPulses.Setup( l.GetString( 25 ), 1, 15, 1, 0, data.PASPulsesToSkip, v => data.PASPulsesToSkip = v );

            sl3PosMode.Setup( l.GetString( 26 ), 1, 4, 1, 0, data.ThreePosMode, v => data.ThreePosMode = v );
            slReverseSpeed.Setup( l.GetString( 27 ), 0, 128, data.Desc.ReverseSpeedMultiplier, 0, data.ReverseSpeed, v => data.ReverseSpeed = v );
            cbOnePedalMode.Setup( l.GetString( 28 ), data.OnePedalMode, v => data.OnePedalMode = v );
            cbThrotteProtection.Setup( l.GetString( 29 ), data.ThrottleProtection, v => data.ThrottleProtection = v );
            cbHallsAngle.Setup( l.GetString( 30 ), data.HallsAngle, v => data.HallsAngle = v );

            BindHelpToControl( slBatteryCurrent, "BatteryCurrent" );
            BindHelpToControl( slPhaseCurrent, "PhaseCurrent" );

            BindHelpToControl( slSpeed1, "Speed1" );
            BindHelpToControl( slSpeed2, "Speed2" );
            BindHelpToControl( slSpeed3, "Speed3" );
            BindHelpToControl( slSpeed4, "Speed4" );

            BindHelpToControl( slSpeed1Current, "Speed1Current" );
            BindHelpToControl( slSpeed2Current, "Speed2Current" );
            BindHelpToControl( slSpeed3Current, "Speed3Current" );
            BindHelpToControl( slSpeed4Current, "Speed4Current" );

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
