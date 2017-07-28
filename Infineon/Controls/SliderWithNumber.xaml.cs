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
        private double offset = 0;
        private double preMultOffset = 0;
        private bool muted;

        private Action<int> valueChanged;

        public SliderWithNumber()
        {
            InitializeComponent();

            MouseEnter += delegate { rectHighlight.Fill = Globals.HighlightBrush; };
            MouseLeave += delegate { rectHighlight.Fill = Brushes.Transparent; };

            slValue.ValueChanged += OnSliderValueChanged;
            tbValue.TextChanged += OnTextChanged;
            tbValue.LostFocus += OnLostFocus;

            Setup( "Unnamed", 0, 10, 1, 0, 0, 0,null );
        }

        public void Setup( string name, int min, int max, double multiplier, double offset, double preMultOffset, int value, Action<int> setValueCb )
        {
            tbName.Text = name;
            slValue.Minimum = min;
            slValue.Maximum = max;
            slValue.Value = value;
            slValue.IsSnapToTickEnabled = true;

            this.multiplier = multiplier;
            this.offset = offset;
            this.preMultOffset = preMultOffset;
            valueChanged = setValueCb;

            // Update text
            OnSliderValueChanged( null, null );
        }

        public int Value
        {
            get { return (int)slValue.Value; }
            set { slValue.Value = value; }
        }

        public void SetCaption( string text )
        {
            tbName.Text = text;
        }

        // -----

        private void OnSliderValueChanged( object sender, RoutedPropertyChangedEventArgs<double> routedPropertyChangedEventArgs )
        {
            if ( muted )
                return;

            muted = true;

            tbValue.Text = $"{( slValue.Value - preMultOffset ) * multiplier + offset:0.0}";
            valueChanged?.Invoke( (int)slValue.Value );

            muted = false;
        }

        private void OnTextChanged( object sender, TextChangedEventArgs e )
        {
            if ( muted )
                return;

            muted = true;

            double d;
            if ( double.TryParse( tbValue.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out d ) )
            {
                slValue.Value = (int)Math.Round( ( d - offset ) / multiplier + preMultOffset );
                valueChanged?.Invoke( (int)slValue.Value );
            }

            muted = false;
        }

        private void OnLostFocus( object sender, RoutedEventArgs e )
        {
            double d;
            if ( double.TryParse( tbValue.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out d ) )
            {
                slValue.Value = (int)Math.Round( ( d - offset ) / multiplier + preMultOffset );
                tbValue.Text = $"{( slValue.Value - preMultOffset ) * multiplier + offset:0.0}";
                valueChanged?.Invoke( (int)slValue.Value );
            }
        }
    }
}
