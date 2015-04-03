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
    public class TCPClient
    {

        # region Private Fields

        private TcpClient _tcpClient = new TcpClient();
        private NetworkStream _clientStream;
        private int _clientPort = -1;
        private int _remoteClientPort = -1;
        private int _remoteServerPort = -1;

        # endregion

        # region Public Fields

        /// <summary>
        /// Port of the current TCP client.
        /// </summary>
        public int ClientPort { get { return _clientPort; } }

        /// <summary>
        /// Port of the remote connected TCP client.
        /// </summary>
        public int RemoteClientPort { get { return _remoteClientPort; } }

        /// <summary>
        /// IP address of remote connected TCP client.
        /// </summary>
        public IPAddress RemoteClientIpAddress { get; private set; }

        /// <summary>
        /// Port of the remote connected TCP server.
        /// </summary>
        public int RemoteServerPort { get { return _remoteServerPort; } }

        /// <summary>
        /// IP address of remote connected TCP server.
        /// </summary>
        public IPAddress RemoteServerIpAddress { get; private set; }

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
                _remoteServerPort = port;
                var serverEndPoint = new IPEndPoint(RemoteServerIpAddress, port);

                // Connect to the server
                _tcpClient.Connect(serverEndPoint);
                _clientStream = _tcpClient.GetStream();

                // Assign ip and port variables of the connected remote server's client
                RemoteClientIpAddress = ((IPEndPoint)_tcpClient.Client.RemoteEndPoint).Address;
                _remoteClientPort = ((IPEndPoint)_tcpClient.Client.RemoteEndPoint).Port;

                // Assign local client's port
                _clientPort = ((IPEndPoint)_tcpClient.Client.LocalEndPoint).Port;

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
                _remoteServerPort = -1;
                RemoteClientIpAddress = null;
                _remoteClientPort = -1;
                _clientPort = -1;

                // Invoke StatusChange event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Connection error! Could not connect to " +
                        ip + " (port " + port + "). " + e.Message + "."));

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
        public void SendData(byte[] data, int timeout = 1000)
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
            }
        }

        /// <summary>
        /// Receive data from the remote server.
        /// </summary>
        /// <param name="blocking">Block code execution until some data is available.</param>
        /// <param name="buffersize">Maximum number of bytes to read from network.</param>
        /// <param name="timeout">Timeout for reading operation (in milliseconds). 
        /// The operation fails if reading the data could not start for the specified amount of time. 
        /// Value 0 indicates a blocking operation (no timeout).</param>
        /// <returns>Array of bytes received from the remote server.</returns>
        public byte[] ReceiveData(bool blocking = false, int buffersize = 4096, int timeout = 1000)
        {
            // Check if there is data to receive
            try
            {
                if (blocking == false && _clientStream.DataAvailable == false)
                    return null;
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

                return null;
            }

            var message = new byte[buffersize];

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
                        StatusChanged(this, new CommunicationStatusEventArgs("Connection to " +
                        RemoteServerIpAddress + " (port " + RemoteServerPort + ") is closed by the server."));

                    // Invoke ErrorOccured event
                    if (ErrorOccured != null)
                        ErrorOccured(this, new EventArgs());
                    
                    return null;
                }

                var result = new byte[bytesRead];
                Array.Copy(message, result, bytesRead);

                // Invoke StatusChange event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Received data from server " +
                        RemoteServerIpAddress + " (port " + RemoteServerPort + ")."));

                // Invoke ReceivedData event
                if (ReceivedData != null)
                    ReceivedData(this, new CommunicationEventArgs(result));

                return result;
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

                return null;
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
