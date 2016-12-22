using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace Infineon.Controls
{
    /// <summary>
    /// Interaction logic for SliderWithNumber.xaml
    /// </summary>
    public partial class CheckBoxControl: UserControl
    {
        private Action<bool> valueChanged;

        public CheckBoxControl()
        {
            InitializeComponent();

            MouseEnter += delegate { rectHighlight.Fill = Globals.HighlightBrush; };
            MouseLeave += delegate { rectHighlight.Fill = Brushes.Transparent; };

            Setup( "Unnamed", false, null );

            cbValue.Checked += delegate { valueChanged?.Invoke( cbValue.IsChecked.Value ); };
            cbValue.Unchecked += delegate { valueChanged?.Invoke( cbValue.IsChecked.Value ); };
        }

        public void Setup( string name, bool value, Action<bool> setValueCb )
        {
            tbName.Text = name;
            cbValue.IsChecked = value;

            valueChanged = setValueCb;
        }

        public bool Value
        {
            get { return cbValue.IsChecked ?? false; }
            set { cbValue.IsChecked = value; }
        }
    }
}
