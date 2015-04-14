# region Includes

using System;

# endregion

namespace RobX.Simulator
{
    /// <summary>
    /// This class provides a structure for the commands received by the simulated robot.
    /// </summary>
    public class Command
    {
        # region Public Fields

        /// <summary>
        /// Code of the command received by the robot.
        /// </summary>
        public readonly byte Code;

        /// <summary>
        /// Time at which the command is received by the robot.
        /// </summary>
        public DateTime Timestamp;

        # endregion

        # region Constructor

        /// <summary>
        /// Constructor of the Command class. Initializes command parameters.
        /// </summary>
        /// <param name="code">Code of the received command.</param>
        /// <param name="timestamp">Time of the received command.</param>
        public Command(byte code, DateTime timestamp)
        {
            Code = code;
            Timestamp = timestamp;
        }

        # endregion
    }
}
