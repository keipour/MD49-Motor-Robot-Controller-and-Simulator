using System;

// ReSharper disable UnusedMember.Global

namespace RobX.Library.Robot
{
    /// <summary>
    /// Class that contains a controller command.
    /// </summary>
    public class Command
    {
        # region Public Enum (Types)

        /// <summary>
        /// Enum that specifies the type of controller command.
        /// </summary>
        public enum Types
        {
            /// <summary>
            /// Set speeds of each wheels for a specified amout of time in milliseconds.
            /// </summary>
            SetSpeedForTime,

            /// <summary>
            /// Set speeds of each wheels for a specified amout of distance in millimeters (computed for the center of robot).
            /// </summary>
            SetSpeedForDistance,

            /// <summary>
            /// Set speeds of each wheels for a specified amout of rotation in degrees.
            /// </summary>
            SetSpeedForDegrees,

            /// <summary>
            /// Move robot in forward direction for a specified amount of time in milliseconds.
            /// </summary>
            MoveForwardForTime,

            /// <summary>
            /// Move robot in forward direction for a specified amount of distance in millimeters.
            /// </summary>
            MoveForwardForDistance,

            /// <summary>
            /// Move robot in backward direction for a specified amount of time in milliseconds.
            /// </summary>
            MoveBackwardForTime,

            /// <summary>
            /// Move robot in backward direction for a specified amount of distance in millimeters.
            /// </summary>
            MoveBackwardForDistance,

            /// <summary>
            /// Rotate robot in counter-clockwise direction for a specified amount of time in milliseconds.
            /// </summary>
            RotateLeftForTime,

            /// <summary>
            /// Rotate robot in counter-clockwise direction for a specified degrees.
            /// </summary>
            RotateLeftForDegrees,

            /// <summary>
            /// Rotate robot in clockwise direction for a specified amount of time in milliseconds.
            /// </summary>
            RotateRightForTime,

            /// <summary>
            /// Rotate robot in clockwise direction for a specified amount of degrees.
            /// </summary>
            RotateRightForDegrees,

            /// <summary>
            /// Stop the robot.
            /// </summary>
            Stop
        }

        # endregion

        # region Public Fields

        /// <summary>
        /// Type of controller command.
        /// </summary>
        public readonly Types Type;

        /// <summary>
        /// <para>Works as follows (range: -127 to +127):</para>
        /// <para>1. For forward and backward movement is the speed of both wheels in the forward/backward direction.</para>
        /// <para>2. For SetSpeed commands is the speed of the left wheel.</para>
        /// <para>3. For right (clockwise) rotation the speed of left wheel will be Speed1 and the speed of right wheel will be -Speed1.</para>
        /// <para>4. For left (counter-clockwise) rotation the speed of left wheel will be -Speed1 and the speed of right wheel will be Speed1.</para>
        /// <para>5. For stop command it has no effect.</para>
        /// </summary>
        public readonly sbyte Speed1;

        /// <summary>
        /// <para>Works as follows (range: -127 to +127):</para>
        /// <para>1. For SetSpeed commands is the speed of the right wheel.</para>
        /// <para>2. For other commands it has no effect.</para>
        /// </summary>
        public readonly sbyte Speed2;

        /// <summary>
        /// <para>Works as follows:</para>
        /// <para>1. For timed commands it is interpreted as the time in milliseconds.</para>
        /// <para>2. For distanced commands it is interpreted as the distance in millimeters.</para>
        /// <para>3. For degree commands it is interpreted as the amount of degrees.</para>
        /// <para>4. For stop command it has no effect.</para>
        /// </summary>
        public readonly double Amount;

        # endregion

        # region Constructor

        /// <summary>
        /// Constructor for Command class.
        /// </summary>
        /// <param name="type">Type of controller command.</param>
        /// <param name="doubleAmount"><para>1. For timed commands it is interpreted as the time in milliseconds.</para>
        /// <para>2. For distanced commands it is interpreted as the distance in millimeters.</para>
        /// <para>3. For degree commands it is interpreted as the amount of degrees.</para>
        /// <para>4. For stop command it has no effect.</para></param>
        /// <param name="sbyteSpeed1"><para>Works as follows (range: -127 to +127):</para>
        /// <para>1. For forward and backward movement is the speed of both wheels in the forward/backward direction.</para>
        /// <para>2. For SetSpeed commands is the speed of the left wheel.</para>
        /// <para>3. For right (clockwise) rotation the speed of left wheel will be Speed1 and the speed of right wheel will be -Speed1.</para>
        /// <para>4. For left (counter-clockwise) rotation the speed of left wheel will be -Speed1 and the speed of right wheel will be Speed1.</para>
        /// <para>5. For stop command it has no effect.</para></param>
        /// <param name="sbyteSpeed2"><para>Works as follows (range: -127 to +127):</para>
        /// <para>1. For SetSpeed commands is the speed of the right wheel.</para>
        /// <para>2. For other commands it has no effect.</para></param>
        public Command(Types type, object doubleAmount = null, object sbyteSpeed1 = null, object sbyteSpeed2 = null)
        {
            Type = type;
            if (doubleAmount != null) Amount = (double) doubleAmount;
            if (sbyteSpeed1 != null) Speed1 = (sbyte) sbyteSpeed1;
            if (sbyteSpeed2 != null) Speed2 = (sbyte) sbyteSpeed2;
        }

        # endregion

        # region Public and Static Functions

        /// <summary>
        /// Get types of parameters for a given command.
        /// </summary>
        /// <param name="type">The input command type.</param>
        /// <returns><para>Returns the parameter types of the given command.</para>
        /// <para>D stands for <see cref="double"/> type.</para>
        /// <para>S stands for <see cref="sbyte"/> type.</para>
        /// <para>B stands for <see cref="byte"/> type.</para>
        /// <para>I stands for <see cref="int"/> type.</para>
        /// </returns>
        public static string GetParameterTypes(Types type)
        {
            switch (type)
            {
                case Types.SetSpeedForTime:
                case Types.SetSpeedForDistance:
                case Types.SetSpeedForDegrees:
                    return "DSS";
                case Types.MoveForwardForTime:
                case Types.MoveForwardForDistance:
                case Types.MoveBackwardForTime:
                case Types.MoveBackwardForDistance:
                case Types.RotateLeftForTime:
                case Types.RotateLeftForDegrees:
                case Types.RotateRightForTime:
                case Types.RotateRightForDegrees:
                    return "DS";
                case Types.Stop:
                    return "";
                default:
                    throw new Exception("Parameter number for the command is not assigned yet!");
            }
        }

        /// <summary>
        /// Gets types of parameters for the current command.
        /// </summary>
        /// <returns><para>Returns the types of parameters of the current command.</para>
        /// <para>D stands for <see cref="double"/> type.</para>
        /// <para>S stands for <see cref="sbyte"/> type.</para>
        /// <para>B stands for <see cref="byte"/> type.</para>
        /// <para>I stands for <see cref="int"/> type.</para>
        /// </returns>
        public string GetParameterTypes()
        {
            return GetParameterTypes(Type);
        }

        /// <summary>
        /// Get names of parameters for a given command.
        /// </summary>
        /// <param name="type">The input command type.</param>
        /// <returns>Returns the parameter names of the given command.</returns>
        public static string[] GetParameterNames(Types type)
        {
            const string strSpeed = "BothWheelSpeeds";
            const string strSpeed1 = "LeftWheelSpeed";
            const string strSpeed2 = "RightWheelSpeed";
            const string strTime = "Time";
            const string strDistance = "Distance";
            const string strDegrees = "Degrees";

            switch (type)
            {
                case Types.SetSpeedForTime:
                    return new[] { strTime, strSpeed1, strSpeed2 };
                case Types.SetSpeedForDistance:
                    return new[] { strDistance, strSpeed1, strSpeed2 };
                case Types.SetSpeedForDegrees:
                    return new[] { strDegrees, strSpeed1, strSpeed2 };
                case Types.MoveForwardForTime:
                case Types.MoveBackwardForTime:
                    return new[] { strTime, strSpeed };
                case Types.MoveForwardForDistance:
                case Types.MoveBackwardForDistance:
                    return new[] { strDistance, strSpeed };
                case Types.RotateLeftForTime:
                case Types.RotateRightForTime:
                    return new[] { strTime, strSpeed };
                case Types.RotateLeftForDegrees:
                case Types.RotateRightForDegrees:
                    return new[] { strDegrees, strSpeed };
                case Types.Stop:
                    return new string[0];
                default:
                    throw new Exception("Parameter number for the command is not assigned yet!");
            }
        }

        /// <summary>
        /// Get names of parameters for the current command.
        /// </summary>
        /// <returns>Returns the parameter names of the current command.</returns>
        public string[] GetParameterNames()
        {
            return GetParameterNames(Type);
        }

        /// <summary>
        /// Get description of parameters for a given command.
        /// </summary>
        /// <param name="type">The input command type.</param>
        /// <returns>Returns the parameter descriptions of the given command.</returns>
        public static string[] GetParameterDescriptions(Types type)
        {
            const string strSpeed = "Speed of the both wheels; should be an integer in the range -127 to +127.";
            const string strSpeed1 = "Speed of the left wheel; should be an integer in the range -127 to +127.";
            const string strSpeed2 = "Speed of the right wheel; should be an integer in the range -127 to +127.";
            const string strTime = "Time to move in milliseconds; should be a non-negative double number.";
            const string strDistance = "Distance to move in millimeters; should be a non-negative double number.";
            const string strDegrees = "Amount of degrees to turn; should be a double number.";

            switch (type)
            {
                case Types.SetSpeedForTime:
                    return new[] { strTime, strSpeed1, strSpeed2 };
                case Types.SetSpeedForDistance:
                    return new[] { strDistance, strSpeed1, strSpeed2 };
                case Types.SetSpeedForDegrees:
                    return new[] { strDegrees, strSpeed1, strSpeed2 };
                case Types.MoveForwardForTime:
                case Types.MoveBackwardForTime:
                    return new[] { strTime, strSpeed };
                case Types.MoveForwardForDistance:
                case Types.MoveBackwardForDistance:
                    return new[] { strDistance, strSpeed };
                case Types.RotateLeftForTime:
                case Types.RotateRightForTime:
                    return new[] { strTime, strSpeed };
                case Types.RotateLeftForDegrees:
                case Types.RotateRightForDegrees:
                    return new[] { strDegrees, strSpeed };
                case Types.Stop:
                    return new string[0];
                default:
                    throw new Exception("Parameter number for the command is not assigned yet!");
            }
        }

        /// <summary>
        /// Get description of parameters for the current command.
        /// </summary>
        /// <returns>Returns the parameter descriptions of the current command.</returns>
        public string[] GetParameterDescriptions()
        {
            return GetParameterDescriptions(Type);
        }

        # endregion

        # region Parse and TryParse Functions

        /// <summary>
        /// Tries to parse the input string as a command.
        /// </summary>
        /// <param name="parseString">The input string that should be parsed.</param>
        /// <param name="command">Output command that is the result of parsing the input string.
        /// This parameter will be null if parsing fails.</param>
        /// <returns>Returns true if the input string is parsed successfully; otherwise returns false.</returns>
        public static bool TryParse(string parseString, out Command command)
        {
            try
            {
                command = Parse(parseString);
            }
            catch
            {
                command = null;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Parses the input string as a command.
        /// </summary>
        /// <param name="parseString">The input string that should be parsed.</param>
        /// <returns>Returns the command instance that is the result of parsing the input string.</returns>
        /// <exception cref="FormatException">This exception is thrown when the number of arguments for the 
        /// command specified in the input string is not the number required by the specified command.</exception>
        public static Command Parse(string parseString)
        {
            var tokens = parseString.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

            // Parse the command type
            var type = (Types)Enum.Parse(typeof(Types), tokens[0], true);

            // Get the number of parameters for the read command type
            var parameterTypes = GetParameterTypes(type);

            if (tokens.Length != parameterTypes.Length + 1)
                throw new FormatException("The " + type + " command requires exactly " + parameterTypes.Length + " arguments!");

            var parameters = new object[3];

            for (var i = 0; i < parameterTypes.Length; ++i)
            {
                switch (parameterTypes[i])
                {
                    case 'D':
                        parameters[i] = Double.Parse(tokens[i + 1]);
                        break;
                    case 'S':
                        parameters[i] = SByte.Parse(tokens[i + 1]);
                        break;
                    case 'B':
                        parameters[i] = Byte.Parse(tokens[i + 1]);
                        break;
                    case 'I':
                        parameters[i] = Int32.Parse(tokens[i + 1]);
                        break;
                    default:
                        throw new NotImplementedException("The method for '" + parameterTypes[i] +
                            "' identifier type is not implemented!");
                }
            }

            return new Command(type, parameters[0], parameters[1], parameters[2]);
        }

        # endregion
    }
}
