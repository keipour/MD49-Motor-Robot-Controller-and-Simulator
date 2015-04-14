# region Includes

using System;
using System.IO.Ports;

# endregion

namespace RobX.Library.Communication.COM
{
    /// <summary>
    /// A class that can connect to a COM serial port and transmit/read data to/from a COM device.
    /// </summary>
    public class ComClient : IClientInterface
    {
        # region Public Fields

        /// <summary>
        /// COM serial port interface.
        /// </summary>
        public readonly SerialPort SerialPort;

        /// <summary>
        /// Number of data bits for Rx/Tx connection with COM port.
        /// </summary>
        public int DataBits { get; private set; }

        /// <summary>
        /// Parity for Rx/Tx connection with COM port.
        /// </summary>
        public Parity Parity { get; private set; }

        /// <summary>
        /// Number of stop bits for Rx/Tx connection with COM port.
        /// </summary>
        public StopBits StopBits { get; private set; }

        /// <summary>
        /// Baud rate for connection with COM port.
        /// </summary>
        public int BaudRate { get; private set; }

        /// <summary>
        /// Name of the COM port.
        /// </summary>
        public string PortName { get; private set; }

        /// <summary>
        /// Specifies if the client should listen for data received from COM port.
        /// </summary>
        public bool IsListeningForData;

        # endregion

        # region Events

        /// <summary>
        /// Event handler for ReceivedData event (will be invoked after data is received).
        /// </summary>
        public event CommunicationEventHandler ReceivedData;

        /// <summary>
        /// Event handler for SentData event (will be invoked after data is sent).
        /// </summary>
        public event CommunicationEventHandler SentData;

        /// <summary>
        /// Event handler for BeforeSendingData event (will be invoked when server is ready to send data).
        /// </summary>
        public event CommunicationEventHandler BeforeSendingData;

        /// <summary>
        /// Event handler for StatusChanged event (will be invoked after anything new happens in the communication).
        /// </summary>
        public event CommunicationStatusEventHandler StatusChanged;

        /// <summary>
        /// This event is invoked when an error occures in the connection with the communication.
        /// </summary>
        public event EventHandler ErrorOccured;

        # endregion

        # region Public Methods

        /// <summary>
        /// Default constructor for COMClient class.
        /// </summary>
        public ComClient()
        {
            DataBits = 8;
            Parity = Parity.None;
            StopBits = StopBits.One;
            BaudRate = 9600;
            PortName = "";
            IsListeningForData = false;
            // Initialize serial port
            SerialPort = new SerialPort();
            SerialPort.DataReceived += DataReceivedHandler;
        }

        /// <summary>
        /// Connects to the COM port using specified parameters.
        /// </summary>
        /// <param name="portName">Name of the COM port (i.e. COM1, COMA, etc.)</param>
        /// <param name="baudRate">Baud rate for connection with COM port.</param>
        /// <param name="dataBits">Number of data bits for Rx/Tx connection with COM port.</param>
        /// <param name="parity">Parity for Rx/Tx connection with COM port.</param>
        /// <param name="stopBits">Number of stop bits for Rx/Tx connection with COM port.</param>
        /// <returns>Returns true if successfully connected to COM port.</returns>
        public bool Connect(String portName, int baudRate = 9600, int dataBits = 8, 
            Parity parity = Parity.None, StopBits stopBits = StopBits.One)
        {
            try
            {
                // Invoke StatusChange event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Connecting to " + portName + 
                        " with baud rate " + baudRate + "..."));

                // Assign class fields
                BaudRate = baudRate;
                DataBits = dataBits;
                Parity = parity;
                StopBits = stopBits;
                PortName = portName;

                // Set input parameters
                SerialPort.BaudRate = baudRate;
                SerialPort.PortName = portName;
                SerialPort.DataBits = dataBits;
                SerialPort.Parity = parity;
                SerialPort.StopBits = stopBits;

                // Connect to port
                SerialPort.Handshake = Handshake.RequestToSendXOnXOff;
                SerialPort.ReadTimeout = 300; // in milliseconds
                SerialPort.Open();

                // Invoke StatusChange event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Successfully connected to " + portName + " with baud rate " + 
                        baudRate + ".")); 
                
                return true;
            }
            catch (Exception e)
            {
                // Invoke StatusChange event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Connection Error! " + e.Message.Replace("PortName", PortName) + "."));

                // Invoke ErrorOccured event
                if (ErrorOccured != null)
                    ErrorOccured(this, new EventArgs());

                return false;
            }
        }

        /// <summary>
        /// Read data from the COM port.
        /// </summary>
        /// <param name="numOfBytes">Number of bytes to read.</param>
        /// <param name="readBuffer">Buffer to put read data.</param>
        /// <param name="checkAvailableData"><para>Check if data is available before trying to read data.</para>
        /// <para>Warning: Setting this parameter to true results in non-blocking operation.</para></param>
        /// <param name="timeout">Timeout for reading operation (in milliseconds). 
        /// The operation fails if reading the data could not start for the specified amount of time. 
        /// Value 0 indicates a blocking operation (no timeout).</param>
        /// <returns>Returns true if successfully read any data; otherwise returns false.</returns>
        public bool ReceiveData(int numOfBytes, out byte[] readBuffer, bool checkAvailableData = false, int timeout = 500)
        {
            readBuffer = null;

            try
            {
                if (checkAvailableData && SerialPort.BytesToRead == 0)
                {
                    // Invoke StatusChanged event
                    if (StatusChanged != null)
                        StatusChanged(this, new CommunicationStatusEventArgs("Warning! No data is available to read from " 
                            + PortName + " port!"));

                    return false;
                }
            }
            catch (Exception e)
            {
                // Invoke StatusChanged event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Error reading data from " + PortName + " port! "
                        + e.Message.Replace("PortName", PortName) + "."));

                // Invoke ErrorOccured event
                if (ErrorOccured != null)
                    ErrorOccured(this, new EventArgs());

                return false;
            }

            // Initialize Read Buffer
            readBuffer = new byte[numOfBytes];

            // Read all bytes from serial port
            for (var i = 0; i < numOfBytes; i++)
            {
                try
                {
                    // Set read timeouts
                    SerialPort.ReadTimeout = timeout;

                    SerialPort.Read(readBuffer, i, 1);
                    
                    // Invoke StatusChange event
                    if (StatusChanged != null)
                        StatusChanged(this, new CommunicationStatusEventArgs("Recieved bytes from " + PortName + " port."));

                    // Invoke ReceivedData event
                    if (ReceivedData != null)
                        ReceivedData(this, new CommunicationEventArgs(readBuffer));
                }
                catch (Exception e)
                {
                    // Invoke StatusChange event
                    if (StatusChanged != null)
                        StatusChanged(this, new CommunicationStatusEventArgs("Error reading data from " + PortName + " port! " 
                            + e.Message.Replace("PortName", PortName) + "."));

                    // Invoke ErrorOccured event
                    if (ErrorOccured != null)
                        ErrorOccured(this, new EventArgs());

                    readBuffer = null;
                    return false;
                }
            }
            return true;
        }
        
        /// <summary>
        /// Send data to the COM port.
        /// </summary>
        /// <param name="data">Array of bytes to send.</param>
        /// <param name="timeout">Timeout for send operation (in milliseconds). 
        /// The operation fails if sending the data could not start for the specified amount of time. 
        /// Value 0 indicates a blocking operation (no timeout).</param>
        /// <returns>Returns true if successfully sent data; false indicates socket error.</returns>
        public bool SendData(byte[] data, int timeout = 500)
        {
            // Write all bytes to serial port
            try
            {
                // Invoke BeforeSendingData event
                var e = new CommunicationEventArgs(data);
                if (BeforeSendingData != null)
                    BeforeSendingData(this, e);

                // Set timeout for writing
                SerialPort.WriteTimeout = timeout;

                // Send data to COM port
                SerialPort.Write(data, 0, data.Length);

                // Invoke StatusChange event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Sent bytes to " + PortName + " port."));

                // Invoke SentData event
                if (SentData != null)
                    SentData(this, new CommunicationEventArgs(data));

                return true;
            }
            catch (Exception e)
            {
                // Invoke StatusChange event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Error writing data to " + PortName + " port! "
                        + e.Message.Replace("PortName", PortName) + "."));

                // Invoke ErrorOccured event
                if (ErrorOccured != null)
                    ErrorOccured(this, new EventArgs());

                return false;
            }
        }

        # endregion

        # region Private Methods

        /// <summary>
        /// Handles DataReceived event of the SerialPort variale. Invokes ReceiveData to read the available data.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event argument varialbe.</param>
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            if (IsListeningForData == false) return;

            var sp = (SerialPort)sender;
            var numOfBytes = sp.BytesToRead;

            byte[] readBuffer;
            ReceiveData(numOfBytes, out readBuffer);
        }

        # endregion
    }
}
