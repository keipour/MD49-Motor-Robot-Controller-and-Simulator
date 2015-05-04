using System;

// ReSharper disable UnusedMemberInSuper.Global
namespace RobX.Library.Communication
{
    /// <summary>
    /// Defines an interface for communication (network, hardware, etc.) server classes.
    /// </summary>
    public interface IServerInterface
    {
        # region Event Handlers

        /// <summary>
        /// Event for ReceivedData event (is invoked after data is received from a client).
        /// </summary>
        event CommunicationEventHandler ReceivedData;

        /// <summary>
        /// Event for SentData event (is invoked after data is sent to a client).
        /// </summary>
        event CommunicationEventHandler SentData;

        /// <summary>
        /// Event for BeforeSendingData event (is invoked when server is ready to send data to a client).
        /// </summary>
        event CommunicationEventHandler BeforeSendingData;

        /// <summary>
        /// Event for StatusChanged event (is invoked when something happens in the server).
        /// </summary>
        event CommunicationStatusEventHandler StatusChanged;

        /// <summary>
        /// This event is invoked when an error occures in the server's workflow.
        /// </summary>
        event EventHandler ErrorOccured;

        # endregion

        # region Public Methods

        /// <summary>
        /// Forces server to stop listening for connections from clients.
        /// </summary>
        void StopServer();

        /// <summary>
        /// Determines if the server is currently actively listens for connections from clients.
        /// </summary>
        /// <returns>Indicates whether the current server is running.</returns>
        bool IsRunning();

        /// <summary>
        /// Sends data over the connection.
        /// </summary>
        /// <param name="data">Array of bytes to send to client.</param>
        /// <param name="timeout">Timeout for send operation (in milliseconds). 
        /// The operation fails if sending the data could not start for the specified amount of time. 
        /// Value 0 indicates a blocking operation (no timeout).</param>
        /// <returns>Returns true if data successfully sent; otherwise returns false.</returns>
        bool SendData(byte[] data, int timeout = 1000);

        /// <summary>
        /// Receives data from the connection.
        /// </summary>
        /// <param name="buffer">Buffer in which the received data will be returned. The buffer size 
        /// determines the maximum number of bytes to read.</param>
        /// <param name="timeout">Timeout for reading operation (in milliseconds). 
        /// The operation fails if reading the data could not start for the specified amount of time. 
        /// Value 0 indicates a blocking operation (no timeout).</param>
        /// <returns>The number of bytes read from the connection. 
        /// Return value -1 indicates that some connection/socket error has occured.</returns>
        int ReceiveData(ref byte[] buffer, int timeout = 1000);

        # endregion
    }
}
