using System;
using System.IO.Ports;
using System.Threading;
using System.Windows.Threading;

namespace Infineon
{
    public class Uploader: IDisposable
    {
        private Thread thread;
        private Dispatcher dispatcher;

        public event Action OnWaitingForButton;
        public event Action OnWaitingForReply;
        public event Action OnSuccess;
        public event Action<string> OnError;

        public void Upload( string portName, byte[] buffer )
        {
            thread?.Abort();

            dispatcher = Dispatcher.CurrentDispatcher;

            if ( buffer == null )
                return;

            thread = new Thread( () => DoUpload( portName, buffer ) );
            thread.Start();
        }

        public void Dispose()
        {
            thread?.Abort();
            thread = null;
        }

        private void DoUpload( string portName, byte[] buffer )
        {
            try
            {
                var port = new SerialPort
                {
                    PortName = portName,
                    BaudRate = 38400,
                    DataBits = 8,
                    Parity = Parity.None,
                    StopBits = StopBits.Two,
                    NewLine = "\r\n"
                };
                port.Open();

                dispatcher.Invoke( new Action( () => OnWaitingForButton?.Invoke() ) );

                // 1. Send '8', wait for 'U'
                while ( true )
                {
                    if ( port.BytesToRead > 0 )
                    {
                        var reply = new byte[1];
                        port.Read( reply, 0, 1 );

                        if ( reply[0] == 'U' )
                        {
                            port.DiscardInBuffer();
                            break;
                        }
                    }

                    port.DiscardInBuffer();
                    port.Write( "8" );
                    Thread.Sleep( 100 );
                }

                // 2. Send data, wait for "QR"
                port.Write( buffer, 0, buffer.Length );

                dispatcher.Invoke( new Action( () => OnWaitingForReply?.Invoke() ) );

                while ( true )
                {
                    if ( port.BytesToRead >= 2 )
                    {
                        var reply = new byte[2];
                        port.Read( reply, 0, 2 );

                        if ( reply[0] == 'Q' && reply[1] == 'R' )
                            dispatcher.Invoke( new Action( () => OnSuccess?.Invoke() ) );
                        else
                            dispatcher.Invoke( new Action( () => OnError?.Invoke( "Incorrect data" ) ) );

                        break;
                    }

                    Thread.Sleep( 100 );
                }
            }
            catch ( Exception ex )
            {
                dispatcher.Invoke( new Action( () => OnError?.Invoke( ex.Message ) ) );
            }

            thread = null;
        }
    }
}
