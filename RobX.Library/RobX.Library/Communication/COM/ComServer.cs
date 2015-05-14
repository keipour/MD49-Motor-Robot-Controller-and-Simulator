using System;
using System.IO.Ports;
using System.Threading;

// ReSharper disable UnusedMember.Global

namespace RobX.Library.Communication.COM
{
    /// <summary>
    /// <para>A COM Server class that runs on a COM port of the current host.</para>
    /// <para>This class allows only one client to be connected to the server at any instance of time. 
    /// There is no way for a client to connect to the current server when there is an active
    /// connection. To make a new connection, the previous connection should be disconnected.</para>
    /// <para>This module is a <see cref="SerialPort"/> wraper which implements 
    /// <see cref="IServerInterface"/> interface.</para>
    /// </summary>
    public class ComServer : IServerInterface
    {
        #region Private Variables

        private bool _startedListening;       // Indicates if tcpListener successfully started listening

        # endregion

        # region Event Handlers

        /// <summary>
        /// Event for ReceivedData event (is invoked after data is received from a client).
        /// </summary>
        public event CommunicationEventHandler ReceivedData;

        /// <summary>
        /// Event for SentData event (is invoked after data is sent to a client).
        /// </summary>
        public event CommunicationEventHandler SentData;

        /// <summary>
        /// Event for BeforeSendingData event (is invoked when server is ready to send data to a client).
        /// </summary>
        public event CommunicationEventHandler BeforeSendingData;

        /// <summary>
        /// Event for StatusChanged event (is invoked when something happens in the server).
        /// </summary>
        public event CommunicationStatusEventHandler StatusChanged;

        private void ChangeStatus(string status)
        {
            if (StatusChanged != null)
                StatusChanged(this, new CommunicationStatusEventArgs(status));
        }

        /// <summary>
        /// This event is invoked when an error occures in the server's workflow.
        /// </summary>
        public event EventHandler ErrorOccured;

        # endregion

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

        # endregion

        # region Constructor

        /// <summary>
        /// COM Server class constructor.
        /// </summary>
        public ComServer()
        {
            DataBits = 8;
            Parity = Parity.None;
            StopBits = StopBits.One;
            BaudRate = 9600;
            PortName = String.Empty;

            // Initialize serial port
            SerialPort = new SerialPort();
            SerialPort.DataReceived += DataReceivedHandler;
        }

        # endregion

        # region Public Functions: Server Status

        /// <summary>
        /// Starts listening for COM connections on the specified port on a new thread. 
        /// </summary>
        /// <param name="portName">Name of the COM port (i.e. COM1, COMA, etc.)</param>
        /// <param name="baudRate">Baud rate for connection with COM port.</param>
        /// <param name="dataBits">Number of data bits for Rx/Tx connection with COM port.</param>
        /// <param name="parity">Parity for Rx/Tx connection with COM port.</param>
        /// <param name="stopBits">Number of stop bits for Rx/Tx connection with COM port.</param>
        /// <param name="timeout">Timeout for starting the server (in milliseconds).
        /// The operation fails if the server could not start for the specified amount of time.</param>
        /// <returns>Returns true if server starts successfully, otherwise returns false.</returns>
        public bool StartServer(String portName, int baudRate = 9600, int dataBits = 8,
            Parity parity = Parity.None, StopBits stopBits = StopBits.One, int timeout = 1000)
        {
            try
            {
                // Don't proceed if the server is running on the same port
                if (portName == PortName && !SerialPort.IsOpen)
                {
                    ChangeStatus("Warning! COM server on port " + portName + " is already running.");
                    return true;
                }

                // Stop if a server is currently running
                if (PortName != String.Empty)
                {
                    StopServer();
                    Thread.Sleep(250);
                }

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

                // Invoke StatusChange event for the new server
                ChangeStatus("Starting COM server on port " + portName + "...");

                // Connect to port
                SerialPort.Handshake = Handshake.RequestToSendXOnXOff;
                SerialPort.ReadTimeout = timeout;  // in milliseconds
                SerialPort.WriteTimeout = timeout; // in milliseconds
                SerialPort.Open();

                // Determine if the server started successfully in timeout milliseconds
                _startedListening = !SerialPort.IsOpen;
                var now = DateTime.Now;
                while (_startedListening == false && PortName != String.Empty)
                {
                    if (!((DateTime.Now - now).TotalMilliseconds > timeout)) continue;

                    StopServer();

                    // Invoke StatusChange event
                    ChangeStatus("Error starting COM server on port "
                                 + portName + ". The server could not start in " + timeout + " milliseconds.");
                    break;
                }

                if (!IsRunning()) return false;

                // Invoke StatusChange event for the new server
                ChangeStatus("COM server on port " + portName +
                    " with baud rate " + baudRate + " started successfully.");
                return true;
            }
            catch (Exception e)
            {
                PortName = String.Empty;

                // Invoke StatusChange event
                ChangeStatus("Error starting COM server on port "
                             + portName + ". " + e.Message.Replace("PortName", portName) + ".");

                return false;
            }
        }

        /// <summary>
        /// Forces server to stop listening for connections from clients on the current port.
        /// </summary>
        public void StopServer()
        {
            try
            {
                SerialPort.Close();
            }
            catch
            {
                // ignored
            }

            var port = PortName;
            PortName = String.Empty;

            // Invoke StatusChange event
            ChangeStatus("COM server on port " + port + " stopped.");
        }

        /// <summary>
        /// Determines if the server is currently actively listens for connections from clients.
        /// </summary>
        /// <returns>Indicates whether the current server is running.</returns>
        public bool IsRunning()
        {
            return (PortName != String.Empty);
        }

        # endregion

        #region Public Functions: Send/Receive Data

        /// <summary>
        /// Sends data over a COM connection.
        /// </summary>
        /// <param name="data">Array of bytes to send to client.</param>
        /// <param name="timeout">Timeout for send operation (in milliseconds). 
        /// The operation fails if sending the data could not start for the specified amount of time. 
        /// Value 0 indicates a blocking operation (no timeout).</param>
        public bool SendData(byte[] data, int timeout = 500)
        {
            // Check if there is data to send
            if (data == null || data.Length == 0) return true;

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
                ChangeStatus("COM server on " + PortName + " port successfully sent data to client.");

                // Invoke SentData event
                if (SentData != null)
                    SentData(this, new CommunicationEventArgs(data));

                return true;
            }
            catch (Exception e)
            {
                // Invoke StatusChange event
                ChangeStatus("COM server on " + PortName + " port encountered an error sending data. " +
                    e.Message.Replace("PortName", PortName) + ".");

                // Invoke ErrorOccured event
                if (ErrorOccured != null)
                    ErrorOccured(this, new EventArgs());

                return false;
            }
        }

        /// <summary>
        /// Receives data from a COM connection.
        /// </summary>
        /// <param name="buffer">Buffer in which the received data will be returned. The buffer size 
        /// determines the maximum number of bytes to read.</param>
        /// <param name="timeout">Timeout for reading operation (in milliseconds). 
        /// The operation fails if reading the data could not start for the specified amount of time. 
        /// Value 0 indicates a blocking operation (no timeout).</param>
        /// <param name="suppressWarning">If set to true, suppresses all warnings, otherwise will invoke StatusChanged event 
        /// for warnings.</param>
        /// <returns>The number of bytes read from the COM connection. 
        /// Return value -1 indicates that some connection/socket error has occured.</returns>
        public int ReceiveData(ref byte[] buffer, int timeout = 500, bool suppressWarning = false)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", "The input should not be null for receiving data by COM server on " 
                    + PortName + " port.");

            if (buffer.Length == 0)
            {
                // Invoke StatusChanged event
                if (suppressWarning == false)
                    ChangeStatus("Warning! Buffer length is zero for reading data by COM server on " + PortName +
                             " port. No data can be read by the server with this buffer.");
                return 0;
            }
            
            var numOfBytes = Math.Min(SerialPort.BytesToRead, buffer.Length);

            try
            {
                if (numOfBytes == 0)
                {
                    // Invoke StatusChanged event
                    if (suppressWarning == false)
                        ChangeStatus("Warning! No data is available to read for COM server on " + PortName + " port!");
                    return 0;
                }
            }
            catch (Exception e)
            {
                // Invoke StatusChanged event
                ChangeStatus("COM server on " + PortName + " port encountered an error receiving data. " + 
                    e.Message.Replace("PortName", PortName) + ".");

                // Invoke ErrorOccured event
                if (ErrorOccured != null)
                    ErrorOccured(this, new EventArgs());

                return -1;
            }

            try
            {
                // Set read timeout
                SerialPort.ReadTimeout = timeout;

                var bytesRead = SerialPort.Read(buffer, 0, numOfBytes);

                var receivedBytes = new byte[bytesRead];
                Array.Copy(buffer, receivedBytes, bytesRead);

                // Invoke StatusChange event
                ChangeStatus("COM server on " + PortName + " port recieved " + bytesRead + " bytes.");

                // Invoke ReceivedData event
                if (ReceivedData != null)
                    ReceivedData(this, new CommunicationEventArgs(receivedBytes));

                return bytesRead;
            }
            catch (Exception e)
            {
                // Invoke StatusChange event
                ChangeStatus("COM server on " + PortName + " port encountered an error receiving data. " + 
                    e.Message.Replace("PortName", PortName) + ".");

                // Invoke ErrorOccured event
                if (ErrorOccured != null)
                    ErrorOccured(this, new EventArgs());

                return -1;
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
            var readBuffer = new byte[4096];
            ReceiveData(ref readBuffer, suppressWarning:true);
        }

        # endregion
    }
}
