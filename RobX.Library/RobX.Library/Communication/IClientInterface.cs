using System;

namespace RobX.Library.Communication
{
    /// <summary>
    /// Defines an interface for communication (network, hardware, etc.) client classes.
    /// </summary>
    public interface IClientInterface
    {
        # region Event Handlers

        /// <summary>
        /// This event is invoked after data is received from the remote client.
        /// </summary>
        event CommunicationEventHandler ReceivedData;

        /// <summary>
        /// This event is invoked after data was sent to the server.
        /// </summary>
        event CommunicationEventHandler SentData;

        /// <summary>
        /// This event is invoked when client is ready to send data to the remote client.
        /// </summary>
        event CommunicationEventHandler BeforeSendingData;

        /// <summary>
        /// This event is invoked when anything new happens in the client.
        /// </summary>
        event CommunicationStatusEventHandler StatusChanged;

        /// <summary>
        /// This event is invoked when an error occures in the connection with the remote client.
        /// </summary>
        event EventHandler ErrorOccured;

        # endregion

        # region Public Methods

        /// <summary>
        /// Send data to the remote client.
        /// </summary>
        /// <param name="data">Array of bytes to send.</param>
        /// <param name="timeout">Timeout for send operation (in milliseconds). 
        /// The operation fails if sending the data could not start for the specified amount of time. 
        /// Value 0 indicates a blocking operation (no timeout).</param>
        /// <returns>Returns true if successfully sent data; false indicates connection error.</returns>
        bool SendData(byte[] data, int timeout = 1000);

        /// <summary>
        /// Receive data from the remote client.
        /// </summary>
        /// <param name="numOfBytes">Number of bytes to read.</param>
        /// <param name="readBuffer">Buffer to put read data.</param>
        /// <param name="checkAvailableData"><para>Check if data is available before trying to read data.</para>
        /// <para>Warning: Setting this parameter to true results in non-blocking operation.</para></param>
        /// <param name="timeout">Timeout for reading operation (in milliseconds). 
        /// The operation fails if reading the data could not start for the specified amount of time. 
        /// Value 0 indicates a blocking operation (no timeout).</param>
        /// <returns>Returns true if successfully read any data; otherwise returns false.</returns>
        bool ReceiveData(int numOfBytes, out byte[] readBuffer, bool checkAvailableData = false, int timeout = 1000);

        # endregion
    }
}
