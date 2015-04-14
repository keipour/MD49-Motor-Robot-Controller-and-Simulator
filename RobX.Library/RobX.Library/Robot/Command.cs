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
        /// <param name="amount"><para>1. For timed commands it is interpreted as the time in milliseconds.</para>
        /// <para>2. For distanced commands it is interpreted as the distance in millimeters.</para>
        /// <para>3. For degree commands it is interpreted as the amount of degrees.</para>
        /// <para>4. For stop command it has no effect.</para></param>
        /// <param name="speed1"><para>Works as follows (range: -127 to +127):</para>
        /// <para>1. For forward and backward movement is the speed of both wheels in the forward/backward direction.</para>
        /// <para>2. For SetSpeed commands is the speed of the left wheel.</para>
        /// <para>3. For right (clockwise) rotation the speed of left wheel will be Speed1 and the speed of right wheel will be -Speed1.</para>
        /// <para>4. For left (counter-clockwise) rotation the speed of left wheel will be -Speed1 and the speed of right wheel will be Speed1.</para>
        /// <para>5. For stop command it has no effect.</para></param>
        /// <param name="speed2"><para>Works as follows (range: -127 to +127):</para>
        /// <para>1. For SetSpeed commands is the speed of the right wheel.</para>
        /// <para>2. For other commands it has no effect.</para></param>
        public Command(Types type, double amount = 100, sbyte speed1 = 0, sbyte speed2 = 0)
        {
            Type = type;
            Amount = amount;
            Speed1 = speed1;
            Speed2 = speed2;
        }

        # endregion

        # region Public and Static Functions

        /// <summary>
        /// Number of parameters for a given command type.
        /// </summary>
        /// <param name="type">The input command type.</param>
        /// <returns>Returns the number of parameters of the given command type.</returns>
        public static int NumberOfParameters(Types type)
        {
            switch (type)
            {
                case Types.SetSpeedForTime:
                case Types.SetSpeedForDistance:
                case Types.SetSpeedForDegrees:
                    return 3;
                case Types.MoveForwardForTime:
                case Types.MoveForwardForDistance:
                case Types.MoveBackwardForTime:
                case Types.MoveBackwardForDistance:
                case Types.RotateLeftForTime:
                case Types.RotateLeftForDegrees:
                case Types.RotateRightForTime:
                case Types.RotateRightForDegrees:
                    return 2;
                case Types.Stop:
                    return 0;
                default:
                    throw new Exception("Parameter number for the command is not assigned yet!");
            }
        }

        /// <summary>
        /// Number of parameters for the current command.
        /// </summary>
        /// <returns>Returns the number of parameters of the current command.</returns>
        public int NumberOfParameters()
        {
            return NumberOfParameters(Type);
        }

        /// <summary>
        /// Tries to parse the input string as a command.
        /// </summary>
        /// <param name="parseString">The input string that should be parsed.</param>
        /// <param name="command">Output command that is the result of parsing the input string.
        /// This parameter will be null if parsing fails.</param>
        /// <returns>Returns true if the input string is parsed successfully; otherwise returns false.</returns>
        public static bool TryParse(string parseString, out Command command)
        {
            command = null;
            var tokens = parseString.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

            // Parse the command type
            Types type;
            try
            {
                type = (Types)Enum.Parse(typeof(Types), tokens[0], true);
            }
            catch
            {
                return false;
            }

            // Get the number of parameters for the read command type
            var numOfParams = NumberOfParameters(type);

            // Our work is done if there are no parameters
            if (numOfParams == 0)
            {
                command = new Command(type);
                return true;
            }

            // Parse the amount (which is double)
            double amount;
            if (Double.TryParse(tokens[1], out amount) == false)
                return false;

            // Our work is done if there is only 1 parameter needed
            if (numOfParams == 1)
            {
                command = new Command(type, amount);
                return true;
            }

            // Parse speed1 (which is sbyte)
            sbyte speed1;
            if (SByte.TryParse(tokens[2], out speed1) == false)
                return false;

            // Our work is done if there are only 2 parameters for the command type
            if (numOfParams == 2)
            {
                command = new Command(type, amount, speed1);
                return true;
            }

            // Parse speed2 (which is sbyte)
            sbyte speed2;
            if (SByte.TryParse(tokens[3], out speed2) == false)
                return false;

            command = new Command(type, amount, speed1, speed2);
            return true;
        }

        /// <summary>
        /// Parses the input string as a command.
        /// </summary>
        /// <param name="parseString">The input string that should be parsed.</param>
        /// <returns>Returns the command instance that is the result of parsing the input string.</returns>
        public static Command Parse(string parseString)
        {
            var tokens = parseString.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

            // Parse the command type
            var type = (Types)Enum.Parse(typeof(Types), tokens[0], true);

            // Get the number of parameters for the read command type
            var numOfParams = NumberOfParameters(type);

            // Our work is done if there are no parameters
            if (numOfParams == 0)
                return new Command(type);

            // Parse the amount (which is double)
            var amount = Double.Parse(tokens[1]);

            // Our work is done if there is only 1 parameter needed
            if (numOfParams == 1)
                return new Command(type, amount);

            // Parse speed1 (which is sbyte)
            var speed1 = SByte.Parse(tokens[2]);

            // Our work is done if there are only 2 parameters for the command type
            if (numOfParams == 2)
                return new Command(type, amount, speed1);

            // Parse speed2 (which is sbyte)
            var speed2 = SByte.Parse(tokens[3]);

            return new Command(type, amount, speed1, speed2);
        }

        # endregion
    }
}