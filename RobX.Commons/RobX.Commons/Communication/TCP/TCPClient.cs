# region Includes

using System;
using System.Net;
using System.Net.Sockets;

# endregion

namespace RobX.Communication.TCP
{
    /// <summary>
    /// TCP client class that can connect to a TCP server and send/receive data over network.
    /// </summary>
    public class TCPClient
    {

        # region Private Fields

        private TcpClient tcpClient = new TcpClient();
        private NetworkStream clientStream;
        private int clientPort = -1;
        private int remoteClientPort = -1;
        private IPAddress remoteClientIPAddress = null;
        private int remoteServerPort = -1;
        private IPAddress remoteServerIPAddress = null;

        # endregion

        # region Public Fields

        /// <summary>
        /// Port of the current TCP client.
        /// </summary>
        public int ClientPort { get { return clientPort; } }

        /// <summary>
        /// Port of the remote connected TCP client.
        /// </summary>
        public int RemoteClientPort { get { return remoteClientPort; } }

        /// <summary>
        /// IP address of remote connected TCP client.
        /// </summary>
        public IPAddress RemoteClientIPAddress { get { return remoteClientIPAddress; } }

        /// <summary>
        /// Port of the remote connected TCP server.
        /// </summary>
        public int RemoteServerPort { get { return remoteServerPort; } }

        /// <summary>
        /// IP address of remote connected TCP server.
        /// </summary>
        public IPAddress RemoteServerIPAddress { get { return remoteServerIPAddress; } }

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
                        " on port " + port.ToString() + "..."));

                tcpClient = new TcpClient();

                // Assign ip and port variables of the remote server
                remoteServerIPAddress = IPAddress.Parse(ip);
                remoteServerPort = port;
                IPEndPoint serverEndPoint = new IPEndPoint(RemoteServerIPAddress, port);

                // Connect to the server
                tcpClient.Connect(serverEndPoint);
                clientStream = tcpClient.GetStream();

                // Assign ip and port variables of the connected remote server's client
                remoteClientIPAddress = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address;
                remoteClientPort = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Port;

                // Assign local client's port
                clientPort = ((IPEndPoint)tcpClient.Client.LocalEndPoint).Port;

                // Invoke StatusChange event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Connected to server " +
                        RemoteServerIPAddress.ToString() + " (port " + RemoteServerPort.ToString() + ")." +
                        Environment.NewLine + "The client connected from port " + ClientPort.ToString() +
                        " to " + RemoteClientIPAddress.ToString() + " (port " + RemoteClientPort.ToString() + ")."));

                return true;
            }
            catch (Exception e)
            {
                remoteServerIPAddress = null;
                remoteServerPort = -1;
                remoteClientIPAddress = null;
                remoteClientPort = -1;
                clientPort = -1;

                // Invoke StatusChange event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Connection error! Could not connect to " +
                        ip.ToString() + " (port " + port.ToString() + "). " + e.Message + "."));

                return false;
            }
        }

        /// <summary>
        /// Send data to the remote server.
        /// </summary>
        /// <param name="Data">Array of bytes that should be sent to the remote server.</param>
        /// <param name="Timeout">Timeout for send operation (in milliseconds). 
        /// The operation fails if sending the data could not start for the specified amount of time. 
        /// Value 0 indicates a blocking operation (no timeout).</param>
        public void SendData(byte[] Data, int Timeout = 1000)
        {
            try
            {
                // Invoke BeforeSendingData event
                CommunicationEventArgs e = new CommunicationEventArgs(null);
                if (BeforeSendingData != null)
                    BeforeSendingData(this, e);

                // Set timeout of write operation
                clientStream.WriteTimeout = Timeout;

                // Send data to the remote server
                clientStream.Write(Data, 0, Data.Length);
                clientStream.Flush();

                // Invoke StatusChanged event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Sent data to server " +
                        RemoteServerIPAddress.ToString() + " (port " + RemoteServerPort.ToString() + ")."));

                // Invoke SentData event
                if (SentData != null)
                    SentData(this, new CommunicationEventArgs(Data));
            }
            catch (Exception e)
            {
                // Invoke StatusChanged event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Error sending data to server " +
                        RemoteServerIPAddress.ToString() + " (port " + RemoteServerPort.ToString() + ")! " + e.Message + "."));

                // Invoke ErrorOccured event
                if (ErrorOccured != null)
                    ErrorOccured(this, new EventArgs());
            }
        }

        /// <summary>
        /// Receive data from the remote server.
        /// </summary>
        /// <param name="Blocking">Block code execution until some data is available.</param>
        /// <param name="buffersize">Maximum number of bytes to read from network.</param>
        /// <param name="Timeout">Timeout for reading operation (in milliseconds). 
        /// The operation fails if reading the data could not start for the specified amount of time. 
        /// Value 0 indicates a blocking operation (no timeout).</param>
        /// <returns>Array of bytes received from the remote server.</returns>
        public byte[] ReceiveData(bool Blocking = false, int buffersize = 4096, int Timeout = 1000)
        {
            // Check if there is data to receive
            try
            {
                if (Blocking == false && clientStream.DataAvailable == false)
                    return null;
            }
            catch (Exception e)
            {
                // Invoke StatusChange event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Socket Error! Error receiving data from server " +
                        RemoteServerIPAddress.ToString() + " (port " + RemoteServerPort.ToString() + ")! " + e.Message + "."));

                // Invoke ErrorOccured event
                if (ErrorOccured != null)
                    ErrorOccured(this, new EventArgs());

                return null;
            }

            byte[] message = new byte[buffersize];
            int bytesRead = 0;

            try
            {
                // Set timeout of read operation
                clientStream.ReadTimeout = Timeout;

                // Receive available data from the remote server
                bytesRead = clientStream.Read(message, 0, message.Length);

                // Check if connection is closed
                if (bytesRead == 0)
                {
                    // Invoke StatusChanged event
                    if (StatusChanged != null)
                        StatusChanged(this, new CommunicationStatusEventArgs("Connection to " +
                        RemoteServerIPAddress.ToString() + " (port " + RemoteServerPort.ToString() + ") is closed by the server."));

                    // Invoke ErrorOccured event
                    if (ErrorOccured != null)
                        ErrorOccured(this, new EventArgs());
                    
                    return null;
                }

                byte[] result = new byte[bytesRead];
                Array.Copy(message, result, bytesRead);

                // Invoke StatusChange event
                if (StatusChanged != null)
                    StatusChanged(this, new CommunicationStatusEventArgs("Received data from server " +
                        RemoteServerIPAddress.ToString() + " (port " + RemoteServerPort.ToString() + ")."));

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
                        RemoteServerIPAddress.ToString() + " (port " + RemoteServerPort.ToString() + ")! " + e.Message + "."));

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
                clientStream.Close();
                tcpClient.Close();
            }
            catch { }
        }

        # endregion
    }
}
