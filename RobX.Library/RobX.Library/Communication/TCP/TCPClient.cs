# region Includes

using System;
using System.Net;
using System.Net.Sockets;

# endregion

namespace RobX.Library.Communication.TCP
{
    /// <summary>
    /// TCP client class that can connect to a TCP server and send/receive data over network.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class TCPClient : IClientInterface
    {
        # region Private Fields

        private TcpClient _tcpClient = new TcpClient();
        private NetworkStream _clientStream;

        # endregion

        # region Public Fields

        /// <summary>
        /// Port of the current TCP client.
        /// </summary>
        public int ClientPort { get; private set; }

        /// <summary>
        /// Port of the remote connected TCP client.
        /// </summary>
        public int RemoteClientPort { get; private set; }

        /// <summary>
        /// IP address of remote connected TCP client.
        /// </summary>
        public IPAddress RemoteClientIpAddress { get; private set; }

        /// <summary>
        /// Port of the remote connected TCP server.
        /// </summary>
        public int RemoteServerPort { get; private set; }

        /// <summary>
        /// IP address of remote connected TCP server.
        /// </summary>
        public IPAddress RemoteServerIpAddress { get; private set; }

        # endregion

        # region Constructor

        /// <summary>
        /// Constructor for the TCPClient Class
        /// </summary>
        public TCPClient()
        {
            RemoteServerPort = -1;
            RemoteClientPort = -1;
            ClientPort = -1;
        }

        # endregion

        # region Public Events

        /// <summary>
        /// This event is invoked after data is received from the server.
        /// </summary>
        public event CommunicationEventHandler ReceivedData;

        /// <summary>
        /// This event is invoked after data was sent to the server.
        /// </summary>
        public event CommunicationEventHandler SentData;

        /// <summary>
        /// This event is invoked when client is ready to send data to the server.
        /// </summary>
        public event CommunicationEventHandler BeforeSendingData;

        /// <summary>
        /// This event is invoked when anything new happens in the client.
        /// </summary>
        public event CommunicationStatusEventHandler StatusChanged;

        /// <summary>
        /// This event is invoked when an error occures in the connection with the server.
        /// </summary>
        public event EventHandler ErrorOccured;

        # endregion

        # region Public Methods

        /// <summary>
        /// Connects TCPClient instance to a running server.
        /// </summary>
        /// <param name="ip">IP address of the remote server.</param>
        /// <param name="port">Port of the remote server.</param>
        /// <returns>Returns true if the client is successfully connected to the server.</returns>
        public bool Connect(string ip, int port)
        {
            try
            {
                // Invoke StatusChange event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Connecting to " + ip +
                        " on port " + port + "..."));

                _tcpClient = new TcpClient();

                // Assign ip and port variables of the remote server
                RemoteServerIpAddress = IPAddress.Parse(ip);
                RemoteServerPort = port;
                var serverEndPoint = new IPEndPoint(RemoteServerIpAddress, port);

                // Connect to the server
                _tcpClient.Connect(serverEndPoint);
                _clientStream = _tcpClient.GetStream();

                // Assign ip and port variables of the connected remote server's client
                RemoteClientIpAddress = ((IPEndPoint)_tcpClient.Client.RemoteEndPoint).Address;
                RemoteClientPort = ((IPEndPoint)_tcpClient.Client.RemoteEndPoint).Port;

                // Assign local client's port
                ClientPort = ((IPEndPoint)_tcpClient.Client.LocalEndPoint).Port;

                // Invoke StatusChange event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Connected to server " +
                        RemoteServerIpAddress + " (port " + RemoteServerPort + ")." +
                        Environment.NewLine + "The client connected from port " + ClientPort +
                        " to " + RemoteClientIpAddress + " (port " + RemoteClientPort + ")."));

                return true;
            }
            catch (Exception e)
            {
                RemoteServerIpAddress = null;
                RemoteServerPort = -1;
                RemoteClientIpAddress = null;
                RemoteClientPort = -1;
                ClientPort = -1;

                // Invoke StatusChange event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Connection error! Could not connect to " +
                        ip + " (port " + port + "). " + e.Message + "."));

                // Invoke ErrorOccured event
                if (ErrorOccured != null)
                    ErrorOccured(this, new EventArgs());

                return false;
            }
        }

        /// <summary>
        /// Send data to the remote server.
        /// </summary>
        /// <param name="data">Array of bytes that should be sent to the remote server.</param>
        /// <param name="timeout">Timeout for send operation (in milliseconds). 
        /// The operation fails if sending the data could not start for the specified amount of time. 
        /// Value 0 indicates a blocking operation (no timeout).</param>
        public bool SendData(byte[] data, int timeout = 1000)
        {
            try
            {
                // Invoke BeforeSendingData event
                var e = new CommunicationEventArgs(null);
                if (BeforeSendingData != null)
                    BeforeSendingData(this, e);

                // Set timeout of write operation
                _clientStream.WriteTimeout = timeout;

                // Send data to the remote server
                _clientStream.Write(data, 0, data.Length);
                _clientStream.Flush();

                // Invoke StatusChanged event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Sent data to server " +
                        RemoteServerIpAddress + " (port " + RemoteServerPort + ")."));

                // Invoke SentData event
                if (SentData != null)
                    SentData(this, new CommunicationEventArgs(data));

                return true;
            }
            catch (Exception e)
            {
                // Invoke StatusChanged event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Error sending data to server " +
                        RemoteServerIpAddress + " (port " + RemoteServerPort + ")! " + e.Message + "."));

                // Invoke ErrorOccured event
                if (ErrorOccured != null)
                    ErrorOccured(this, new EventArgs());
                return false;
            }
        }

        /// <summary>
        /// Receive data from the remote server.
        /// </summary>
        /// <param name="numOfBytes">Number of bytes to read.</param>
        /// <param name="readBuffer">Buffer to put read data.</param>
        /// <param name="checkAvailableData"><para>Check if data is available before trying to read data.</para>
        /// <para>Warning: Setting this parameter to true results in non-blocking operation.</para></param>
        /// <param name="timeout">Timeout for reading operation (in milliseconds). 
        /// The operation fails if reading the data could not start for the specified amount of time. 
        /// Value 0 indicates a blocking operation (no timeout).</param>
        /// <returns>Returns true if successfully read any data; otherwise returns false.</returns>
        public bool ReceiveData(int numOfBytes, out byte[] readBuffer, bool checkAvailableData = false, int timeout = 1000)
        {
            readBuffer = null;

            // Check if there is data to receive
            try
            {
                if (checkAvailableData && _clientStream.DataAvailable == false)
                {
                    // Invoke StatusChanged event
                    if (StatusChanged != null)
                        StatusChanged(this, new CommunicationStatusEventArgs("Warning! There is no data to read from " +
                        RemoteServerIpAddress + " (port " + RemoteServerPort + ") server."));

                    return false;
                }
            }
            catch (Exception e)
            {
                // Invoke StatusChange event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Socket Error! Error receiving data from server " +
                        RemoteServerIpAddress + " (port " + RemoteServerPort + ")! " + e.Message + "."));

                // Invoke ErrorOccured event
                if (ErrorOccured != null)
                    ErrorOccured(this, new EventArgs());

                return false;
            }

            var message = new byte[numOfBytes];

            try
            {
                // Set timeout of read operation
                _clientStream.ReadTimeout = timeout;

                // Receive available data from the remote server
                var bytesRead = _clientStream.Read(message, 0, message.Length);

                // Check if connection is closed
                if (bytesRead == 0)
                {
                    // Invoke StatusChanged event
                    if (StatusChanged != null)
                        StatusChanged(this, new CommunicationStatusEventArgs("Error! Probably the connection to " +
                        RemoteServerIpAddress + " (port " + RemoteServerPort + ") is closed by the server."));

                    // Invoke ErrorOccured event
                    if (ErrorOccured != null)
                        ErrorOccured(this, new EventArgs());

                    return false;
                }

                readBuffer = new byte[bytesRead];
                Array.Copy(message, readBuffer, bytesRead);

                if (bytesRead < numOfBytes)
                {
                    // Invoke StatusChanged event
                    if (StatusChanged != null)
                        StatusChanged(this, new CommunicationStatusEventArgs("Warning! Not enough bytes read from " +
                        RemoteServerIpAddress + " (port " + RemoteServerPort + ")."));

                    // Invoke ErrorOccured event
                    if (ErrorOccured != null)
                        ErrorOccured(this, new EventArgs());

                    return true;
                }

                // Invoke StatusChange event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Received data from server " +
                        RemoteServerIpAddress + " (port " + RemoteServerPort + ")."));

                // Invoke ReceivedData event
                if (ReceivedData != null)
                    ReceivedData(this, new CommunicationEventArgs(readBuffer));

                return true;
            }
            catch (Exception e)
            {
                // Invoke StatusChange event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Error receiving data from server " +
                        RemoteServerIpAddress + " (port " + RemoteServerPort + ")! " + e.Message + "."));

                // Invoke ErrorOccured event
                if (ErrorOccured != null)
                    ErrorOccured(this, new EventArgs());

                readBuffer = null;
                return false;
            }
        }

        /// <summary>
        /// Close connection to the server.
        /// </summary>
        public void Close()
        {
            try
            {
                _clientStream.Close();
                _tcpClient.Close();
            }
            catch
            {
                // ignored
            }
        }

        # endregion
    }
}
