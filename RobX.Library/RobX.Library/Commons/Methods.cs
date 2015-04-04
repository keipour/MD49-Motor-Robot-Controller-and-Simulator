# region Includes

using System.Net;

# endregion

namespace RobX.Library.Commons
{
    /// <summary>
    /// This class contains public static methods used in the project.
    /// </summary>
    public static class Methods
    {
        # region Number Conversion Methods

        /// <summary>
        /// Reads an unsigned 8-bit integer as a signed 8-bit integer.
        /// </summary>
        /// <param name="number">An 8-bit unsigned integer.</param>
        /// <returns>Returns signed integer</returns>
        public static sbyte ConvertUnsignedByteToSigned(int number)
        {
            number = number % 256;
            if (number <= 127)
                return (sbyte)number;
            
            return (sbyte)(number - 256);
        }

        /// <summary>
        /// Reads a signed 8-bit integer as an unsigned 8-bit integer.
        /// </summary>
        /// <param name="number">An 8-bit signed integer.</param>
        /// <returns>Returns unsigned integer (in range 0-255).</returns>
        public static byte ConvertSignedByteToUnsigned(int number)
        {
            return (byte)(((number % 256) + 256) % 256);
        }

        # endregion

        # region String Validation Methods

        /// <summary>
        /// This method checks if the input string is a valid TCP port number.
        /// </summary>
        /// <returns>Returns true if the input string is a valid TCP port number; otherwise returns false.</returns>
        public static bool IsValidPort(string str)
        {
            ushort port;
            return ushort.TryParse(str, out port) && port >= 2;
        }

        /// <summary>
        /// This method checks if the input string is a valid IP address.
        /// </summary>
        /// <returns>Returns true if the input string is a valid IP address; otherwise returns false.</returns>
        public static bool IsValidIpAddress(string str)
        {
            IPAddress ip;
            return IPAddress.TryParse(str, out ip);
        }

        # endregion
    }
}
