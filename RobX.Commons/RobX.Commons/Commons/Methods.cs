# region Includes

using System.Windows.Forms;

# endregion

namespace RobX.Commons
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
        /// <param name="Number">An 8-bit unsigned integer.</param>
        /// <returns>Returns signed integer</returns>
        public static sbyte ConvertUnsignedByteToSigned(int Number)
        {
            Number = Number % 256;
            if (Number <= 127)
                return (sbyte)Number;
            else
                return (sbyte)(Number - 256);
        }

        /// <summary>
        /// Reads a signed 8-bit integer as an unsigned 8-bit integer.
        /// </summary>
        /// <param name="Number">An 8-bit signed integer.</param>
        /// <returns>Returns unsigned integer (in range 0-255).</returns>
        public static byte ConvertSignedByteToUnsigned(int Number)
        {
            return (byte)(((Number % 256) + 256) % 256);
        }

        # endregion
    }
}
