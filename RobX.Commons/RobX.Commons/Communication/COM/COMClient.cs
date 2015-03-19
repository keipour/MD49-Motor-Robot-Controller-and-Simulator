# region Includes

using System;
using System.IO.Ports;

# endregion

namespace RobX.Communication.COM
{
    /// <summary>
    /// A class that can connect to a COM serial port and transmit/read data to/from a COM device.
    /// </summary>
    public class COMClient
    {
        # region Private Fields

        private int dataBits = 8;
        private Parity parity = Parity.None;
        private StopBits stopBits = StopBits.One;
        private int baudRate = 9600;
        private string portName = "";

        # endregion

        # region Public Fields

        /// <summary>
        /// COM serial port interface.
        /// </summary>
        public SerialPort SerialPort;

        /// <summary>
        /// Number of data bits for Rx/Tx connection with COM port.
        /// </summary>
        public int DataBits { get { return dataBits; } }

        /// <summary>
        /// Parity for Rx/Tx connection with COM port.
        /// </summary>
        public Parity Parity { get { return parity; } }

        /// <summary>
        /// Number of stop bits for Rx/Tx connection with COM port.
        /// </summary>
        public StopBits StopBits { get { return stopBits; } }

        /// <summary>
        /// Baud rate for connection with COM port.
        /// </summary>
        public int BaudRate { get { return baudRate; } }

        /// <summary>
        /// Name of the COM port.
        /// </summary>
        public string PortName { get { return portName; } }
       
        # endregion

        # region Event Handlers

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

        # endregion

        # region Public Methods

        /// <summary>
        /// Default constructor for COMClient class.
        /// </summary>
        public COMClient()
        {
            // Initialize serial port
            SerialPort = new SerialPort();
            SerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        }

        /// <summary>
        /// Connects to the COM port using specified parameters.
        /// </summary>
        /// <param name="PortName">Name of the COM port (i.e. COM1, COMA, etc.)</param>
        /// <param name="BaudRate">Baud rate for connection with COM port.</param>
        /// <param name="DataBits">Number of data bits for Rx/Tx connection with COM port.</param>
        /// <param name="Parity">Parity for Rx/Tx connection with COM port.</param>
        /// <param name="StopBits">Number of stop bits for Rx/Tx connection with COM port.</param>
        /// <returns>Returns true if successfully connected to COM port.</returns>
        public bool Connect(String PortName, int BaudRate = 9600, int DataBits = 8, 
            Parity Parity = Parity.None, StopBits StopBits = StopBits.One)
        {
            try
            {
                // Invoke StatusChange event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Connecting to " + PortName + 
                        " with baud rate " + BaudRate.ToString() + "..."));

                // Assign class fields
                baudRate = BaudRate;
                dataBits = DataBits;
                parity = Parity;
                stopBits = StopBits;
                portName = PortName;

                // Set input parameters
                SerialPort.BaudRate = (int)BaudRate;
                SerialPort.PortName = PortName;
                SerialPort.DataBits = DataBits;
                SerialPort.Parity = Parity;
                SerialPort.StopBits = StopBits;

                // Connect to port
                SerialPort.Handshake = Handshake.RequestToSendXOnXOff;
                SerialPort.ReadTimeout = 300; // in milliseconds
                SerialPort.Open();

                // Invoke StatusChange event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Successfully connected to " + PortName + " with baud rate " + 
                        BaudRate.ToString() + ".")); 
                
                return true;
            }
            catch (Exception e)
            {
                // Invoke StatusChange event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Connection Error! " + e.Message + ".")); 
                
                return false;
            }
        }

        /// <summary>
        /// Read data from the COM port.
        /// </summary>
        /// <param name="NumOfBytes">Number of bytes to read</param>
        /// <param name="ReadBuffer">Buffer to put read data</param>
        /// <param name="CheckDataAvailable">Checks if data is available before trying to read data. 
        /// Setting this parameter to true results in non-blocking operation.</param>
        /// <param name="Timeout">Timeout for reading operation (in milliseconds). 
        /// The operation fails if reading the data could not start for the specified amount of time. 
        /// Value 0 indicates a blocking operation (no timeout).</param>
        /// <returns>Returns true if successfully read any data; false indicates socket error.</returns>
        public bool ReceiveData(byte NumOfBytes, ref byte[] ReadBuffer, bool CheckDataAvailable = false, int Timeout = 500)
        {
            try
            {
                if (CheckDataAvailable == true)
                    if (SerialPort.BytesToRead == 0) 
                        return false;
            }
            catch (Exception e)
            {
                // Invoke StatusChanged event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Error reading data from " + PortName + " port! " 
                        + e.Message + "."));
                return false;
            }

            // Initialize Read Buffer
            ReadBuffer = new byte[NumOfBytes];

            // Read all bytes from serial port
            for (int i = 0; i < NumOfBytes; i++)
            {
                try
                {
                    // Set read timeouts
                    SerialPort.ReadTimeout = Timeout;

                    SerialPort.Read(ReadBuffer, i, 1);
                    
                    // Invoke StatusChange event
                    if (StatusChanged != null)
                        StatusChanged(this, new CommunicationStatusEventArgs("Recieved bytes from " + PortName + " port."));

                    // Invoke ReceivedData event
                    if (ReceivedData != null)
                        ReceivedData(this, new CommunicationEventArgs(ReadBuffer));
                }
                catch (Exception e)
                {
                    // Invoke StatusChange event
                    if (StatusChanged != null)
                        StatusChanged(this, new CommunicationStatusEventArgs("Error reading data from " + PortName + " port! " 
                            + e.Message + "."));
                    
                    return false;
                }
            }
            return true;
        }
        
        /// <summary>
        /// Send data to the COM port.
        /// </summary>
        /// <param name="Data">Array of bytes to send.</param>
        /// <param name="Timeout">Timeout for send operation (in milliseconds). 
        /// The operation fails if sending the data could not start for the specified amount of time. 
        /// Value 0 indicates a blocking operation (no timeout).</param>
        /// <returns>Returns true if successfully sent data; false indicates socket error.</returns>
        public bool SendData(byte[] Data, int Timeout = 500)
        {
            // Write all bytes to serial port
            try
            {
                // Invoke BeforeSendingData event
                CommunicationEventArgs e = new CommunicationEventArgs(Data);
                if (BeforeSendingData != null)
                    BeforeSendingData(this, e);

                // Set timeout for writing
                SerialPort.WriteTimeout = Timeout;

                // Send data to COM port
                SerialPort.Write(Data, 0, Data.Length);

                // Invoke StatusChange event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Sent bytes to " + PortName + " port."));

                // Invoke SentData event
                if (SentData != null)
                    SentData(this, new CommunicationEventArgs(Data));

                return true;
            }
            catch (Exception e)
            {
                // Invoke StatusChange event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Error writing data to " + PortName + " port! " 
                        + e.Message + "."));

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
            SerialPort sp = (SerialPort)sender;
            int NumOfBytes = sp.BytesToRead;

            byte[] ReadBuffer = new byte[NumOfBytes];
            ReceiveData((byte)NumOfBytes, ref ReadBuffer);
        }

        # endregion
    }
}
