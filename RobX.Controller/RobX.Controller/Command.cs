namespace RobX.Controller
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
        /// <para>3. For degree commands it is interpreted as the degree.</para>
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
        /// <para>3. For degree commands it is interpreted as ten times the degree (i.e. 1 = 0.1 degrees).</para>
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
        public Command(Types type, int amount = 100, sbyte speed1 = 0, sbyte speed2 = 0)
        {
            Type = type;
            Amount = amount;
            Speed1 = speed1;
            Speed2 = speed2;
        }

        # endregion
    }
}