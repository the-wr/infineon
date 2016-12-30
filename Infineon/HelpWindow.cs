using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using mshtml;

namespace Infineon
{
    public class HelpWindow
    {
        private WebBrowser wb;

        public HelpWindow( WebBrowser wb )
        {
            this.wb = wb;
            wb.Navigated += delegate { ApplyStyle( Localization.Instance.Styles ); };

            int feature = FEATURE_DISABLE_NAVIGATION_SOUNDS;
            CoInternetSetFeatureEnabled( feature, SET_FEATURE_ON_PROCESS, true );
        }

        public void ShowHelp( string key )
        {
            var help = Localization.Instance.GetHelp( key );
            SetHTML( help );
        }

        private void SetHTML( string html )
        {
            if ( string.IsNullOrEmpty( html ) )
            {
                wb.NavigateToString( "<html></html>" );
                return;
            }

            wb.NavigateToString( html );
        }

        private void ApplyStyle( string style )
        {
            if ( string.IsNullOrEmpty( style ) )
                return;

            try
            {
                if ( wb.Document != null )
                {
                    var currentDocument = (IHTMLDocument2)wb.Document;

                    int length = currentDocument.styleSheets.length;
                    IHTMLStyleSheet styleSheet = currentDocument.createStyleSheet( @"", length + 1 );
                    styleSheet.cssText = style;
                }
            }
            catch ( Exception )
            {
            }
        }

        // http://stackoverflow.com/questions/393166/how-to-disable-click-sound-in-webbrowser-control

        private const int FEATURE_DISABLE_NAVIGATION_SOUNDS = 21;
        private const int SET_FEATURE_ON_PROCESS = 0x00000002;

        [DllImport( "urlmon.dll" )]
        [PreserveSig]
        [return: MarshalAs( UnmanagedType.Error )]
        static extern int CoInternetSetFeatureEnabled(
        int FeatureEntry,
        [MarshalAs( UnmanagedType.U4 )] int dwFlags,
        bool fEnable );
    }
}
