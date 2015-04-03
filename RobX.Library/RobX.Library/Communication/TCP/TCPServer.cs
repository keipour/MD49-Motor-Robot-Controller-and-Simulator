# region Includes

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

# endregion

namespace RobX.Library.Communication.TCP
{
    /// <summary>
    /// A TCP Server class that runs on all IPs (v4 and v6) of current host.
    /// This class allows only one client to be connected to the server at any instance of time. 
    /// When a new client connects to the server, previous connection (if any) is closed.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class TCPServer
    {
        #region Private Variables

        private TcpListener _tcpListener;     // Server listener variable
        private Thread _listenThread;         // Server listening thread
        private TcpClient _tcpClient;         // The most recent client that is connected to the server
        private TcpClient _tcpClientTemp;     // A temporary client variable
        private bool _startedListening;       // Indicates if tcpListener successfully started listening

        /// <summary>
        /// TCP Server class initializer.
        /// </summary>
        public TCPServer()
        {
            Port = -1;
        }

        # endregion

        # region Public Events

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

        # endregion

        # region Public Fields

        /// <summary>
        /// Port of the current TCP server. If TCP server is not running, this field is set to -1.
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// All IPv4 and IPv6 IP addresses of the current TCP server.
        /// </summary>
        public IPAddress[] IpAddresses { get; private set; }

        # endregion

        # region Public Functions: Server Status

        /// <summary>
        /// Starts listening for TCP connections on the specified port on a new thread. 
        /// Stops listening to the current port if it is already running.
        /// </summary>
        /// <param name="port">Port on which the server should listen.</param>
        /// <returns>Returns true if server starts successfully, otherwise returns false.</returns>
        public bool StartServer(int port)
        {
            try
            {
                // Stop if a server is currently running
                if (Port != -1)
                    StopServer();

                Port = port;

                // Define new TCP listener
                _tcpListener = new TcpListener(IPAddress.Any, port);

                // Obtain IP addresses for the current host
                IpAddresses = GetHostIpAddresses();

                // Invoke StatusChange event for each new server
                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var ipAddress in IpAddresses)
                    if (ipAddress.AddressFamily == AddressFamily.InterNetwork) // Show only IPv4 addresses
                        if (StatusChanged != null)
                            StatusChanged(this, new CommunicationStatusEventArgs("Started TCP server on " +
                                                                                 ipAddress + " (port " + port + ")."));

                // Start the TCP server on a new thread
                _listenThread = new Thread(ListenForClients) {IsBackground = true};
                _listenThread.Start();
                
                // Determine if the tcpListener started successfully.
                _startedListening = false;
                while (_startedListening == false && Port != -1) { }
                return IsRunning();
            }
            catch (Exception e)
            {
                Port = -1;

                // Invoke StatusChange event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Could not start TCP server on port " + port + 
                        ". " + e.Message + "."));

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
                _tcpListener.Stop();
                _listenThread.Abort();
            }
            catch
            {
                // ignored
            }

            Port = -1;

            // Invoke StatusChange event
            if (StatusChanged != null)
                StatusChanged(this, new CommunicationStatusEventArgs("Stopped TCP Server!"));
        }

        /// <summary>
        /// Determines if the server is currently actively listens for connections from clients.
        /// </summary>
        /// <returns>Indicates whether the current server is running.</returns>
        public bool IsRunning()
        {
            return (Port != -1);
        }

        # endregion

        #region Public Functions: Send/Receive Data

        /// <summary>
        /// Sends data over a TCP connection.
        /// </summary>
        /// <param name="data">Array of bytes to send to client.</param>
        /// <param name="timeout">Timeout for send operation (in milliseconds). 
        /// The operation fails if sending the data could not start for the specified amount of time. 
        /// Value 0 indicates a blocking operation (no timeout).</param>
        public void SendData(byte[] data, int timeout = 1000)
        {
            // Check if there is data to send
            if (data == null || data.Length == 0) return;

            try
            {
                var clientStream = _tcpClient.GetStream();

                // Set write timeout
                clientStream.WriteTimeout = timeout;

                // Send data over network
                clientStream.Write(data, 0, data.Length);
                clientStream.Flush();

                // Invoke StatusChange event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Sent data to client."));

                // Invoke SentData event
                if (SentData != null)
                    SentData(this, new CommunicationEventArgs(data));
            }
            catch (Exception e)
            {
                // Invoke StatusChange event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Error sending data to client! " + e.Message + "."));
            }
        }

        /// <summary>
        /// Receives data from a TCP connection.
        /// </summary>
        /// <param name="buffer">Buffer in which the received data will be returned.</param>
        /// <param name="timeout">Timeout for reading operation (in milliseconds). 
        /// The operation fails if reading the data could not start for the specified amount of time. 
        /// Value 0 indicates a blocking operation (no timeout).</param>
        /// <returns>The number of bytes read from the TCP connection. 
        /// Return value -1 indicates that some connection/socket error has occured.</returns>
        public int ReceiveData(ref byte[] buffer, int timeout = 1000)
        {
            var bytesRead = 0;

            try
            {
                var clientStream = _tcpClient.GetStream();

                // Don't try to read if there is nothing to read
                if (clientStream.DataAvailable)
                {
                    try
                    {
                        // Set read timeout
                        clientStream.ReadTimeout = timeout;

                        // Read the received data from network stream
                        bytesRead = clientStream.Read(buffer, 0, buffer.Length);
                        var receivedBytes = new byte[bytesRead];
                        Array.Copy(buffer, receivedBytes, bytesRead);

                        // Invoke StatusChange event
                        if (StatusChanged != null)
                            StatusChanged(this, new CommunicationStatusEventArgs("Recieved data from client."));

                        // Invoke ReceivedData event
                        if (ReceivedData != null)
                            ReceivedData(this, new CommunicationEventArgs(receivedBytes));
                    }
                    catch (Exception e) // A socket error has occured
                    {
                        // Invoke StatusChange event
                        if (StatusChanged != null)
                            StatusChanged(this, new CommunicationStatusEventArgs("Socket error occured! " + e.Message + "."));

                        return -1;
                    }

                    // Check if the client has disconnected from the server
                    if (bytesRead == 0)
                    {
                        // Invoke StatusChange event
                        if (StatusChanged != null)
                            StatusChanged(this, new CommunicationStatusEventArgs("Error! Client has disconnected from the server!"));

                        return -1;
                    }
                }
            }
            catch (Exception e)
            {
                bytesRead = -1;
                
                // Invoke StatusChange event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Error! " + e.Message + "."));
            }

            return bytesRead;
        }

        # endregion

        # region Public Static Function (Obtaining Host IP Addresses)

        /// <summary>
        /// Obtains all IPv4 and IPv6 addresses of the current host.
        /// </summary>
        /// <returns>Array of all IPv4 and IPv6 addresses of the current host.</returns>
        public static IPAddress[] GetHostIpAddresses()
        {
            var host = Dns.GetHostName();
            var ip = Dns.GetHostEntry(host);
            return ip.AddressList;
        }

        # endregion

        # region Private Functions

        /// <summary>
        /// Listens for new TCP client connection requests.
        /// </summary>
        private void ListenForClients()
        {
            try
            {
                // Start server
                _tcpListener.Start();
            }
            catch (Exception e)
            {
                var tempport = Port;
                Port = -1;

                // Invoke StatusChange event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Could not start listening on port " + tempport + 
                        ". " + e.Message + "."));
                return;
            }

            _startedListening = true;

            while (true)
            {
                // Blocks until a client has connected to the server
                try
                {
                    _tcpClientTemp = _tcpListener.AcceptTcpClient();
                }
                catch
                {
                    // ignored
                }

                _tcpClient = _tcpClientTemp;

                // Invoke StatusChange event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("A client connected from " +
                        ((IPEndPoint)_tcpClient.Client.RemoteEndPoint).Address +
                        " (port " + ((IPEndPoint)_tcpClient.Client.RemoteEndPoint).Port + ")."));

                // Create a thread to handle communication with connected client
                var clientThread = new Thread(HandleClientComm) {IsBackground = true};
                clientThread.Start();
            }
        }

        /// <summary>
        /// Handles communication with a connected TCP client.
        /// </summary>
        private void HandleClientComm()
        {
            var buffer = new byte[4096]; // Buffer to read from client

            while (true)
            {
                // Read data from network stream if available
                var bytesRead = ReceiveData(ref buffer);  // Number of bytes read from client stream

                // Break loop if there is a socket error or a connection is closed
                if (bytesRead == -1) break;

                // Invoke BeforeSendingData event
                var e = new CommunicationEventArgs(null);
                if (BeforeSendingData != null)
                    BeforeSendingData(this, e);

                // Send data if anything exists
                SendData(e.Data);
            }
            _tcpClient.Close();
        }

        #endregion
    }
}
