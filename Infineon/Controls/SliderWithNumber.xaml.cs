using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Infineon.Controls
{
    /// <summary>
    /// Interaction logic for SliderWithNumber.xaml
    /// </summary>
    public partial class SliderWithNumber: UserControl
    {
        private double multiplier = 1;
        private bool muted;

        public SliderWithNumber()
        {
            InitializeComponent();

            MouseEnter += delegate { rectHighlight.Fill = Globals.HighlightBrush; };
            MouseLeave += delegate { rectHighlight.Fill = Brushes.Transparent; };

            slValue.ValueChanged += OnSliderValueChanged;
            tbValue.TextChanged += OnTextChanged;
            tbValue.LostFocus += OnLostFocus;

            Setup( "Unnamed", 0, 10, 1 );
        }

        public void Setup( string name, int min, int max, double multiplier )
        {
            tbName.Text = name;
            slValue.Minimum = min;
            slValue.Maximum = max;
            slValue.IsSnapToTickEnabled = true;

            this.multiplier = multiplier;
        }

        private void OnSliderValueChanged( object sender, RoutedPropertyChangedEventArgs<double> routedPropertyChangedEventArgs )
        {
            if ( muted )
                return;

            muted = true;

            tbValue.Text = $"{slValue.Value * multiplier:0.0}";

            muted = false;
        }

        private void OnTextChanged( object sender, TextChangedEventArgs e )
        {
            if ( muted )
                return;

            muted = true;

            double d;
            if ( double.TryParse( tbValue.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out d ) )
                slValue.Value = (int)Math.Round( d / multiplier );

            muted = false;
        }

        private void OnLostFocus( object sender, RoutedEventArgs e )
        {
            double d;
            if ( double.TryParse( tbValue.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out d ) )
            {
                slValue.Value = (int) Math.Round( d / multiplier );
                tbValue.Text = $"{slValue.Value * multiplier:0.0}";
            }
        }
    }
}
