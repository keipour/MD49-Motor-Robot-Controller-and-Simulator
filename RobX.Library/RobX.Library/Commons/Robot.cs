namespace RobX.Library.Commons
{
    /// <summary>
    /// This class keeps all common properties of RobX robot.
    /// </summary>
    public static class Robot
    {
        # region Appearance

        /// <summary>
        /// Diameter of wheels in millimeters for EMG45 motor set used in RobX robot.
        /// </summary>
        public const double WheelDiameter = 125.0F;

        /// <summary>
        /// The radius in millimeters for RobX robot.
        /// </summary>
        public const int Radius = 250;

        # endregion

        # region Robot/Motor Properties

        /// <summary>
        /// Motor encoder count per turn for EMG49 motors used in RobX robot.
        /// </summary>
        public const int EncoderCountPerTurn = 980;

        /// <summary>
        /// Motor timeout time in milliseconds for MD49 motor driver used in RobX robot.
        /// </summary>
        public const int MotorTimeoutInMs = 2000;

        /// <summary>
        /// <para>Converts robot speed to millimeters per second. Calculated empirically.</para>
        /// <para>Warning: It is supposed that the robot speed can be converted to real-world speed linearly. 
        /// This assumption works fine for most applications. To get more accurate results, the robot speed vs 
        /// real-world speed relation should be measured (which is anticipated to be non-linear).</para>
        /// </summary>
        public const double RobotSpeedToMmpS = 6.25F;

        /// <summary>
        /// Converts encoder count to distance in millimeters for EMG49 motor set used in RobX robot.
        /// </summary>
        public const double EncoderCount2mM = WheelDiameter * System.Math.PI / EncoderCountPerTurn;

        # endregion

        # region Hardware Interface (COM) Properties

        /// <summary>
        /// Number of data bits for connection with COM port of the MD49 motor used in RobX robot.
        /// </summary>
        public const int DataBits = 8;

        /// <summary>
        /// Parity for connection with COM port of the MD49 motor used in RobX robot.
        /// </summary>
        public const System.IO.Ports.Parity Parity = System.IO.Ports.Parity.None;

        /// <summary>
        /// Number of stop bits for connection with COM port of the MD49 motor used in RobX robot.
        /// </summary>
        public const System.IO.Ports.StopBits StopBits = System.IO.Ports.StopBits.Two;

        /// <summary>
        /// Baud rate for connection with COM port of the MD49 motor used in RobX robot.
        /// </summary>
        public const BaudRates BaudRate = BaudRates.BaudRate9600;

        # endregion

        # region Public Enums

        /// <summary>
        /// Serial port baud rates supported by MD49 robot controller used in RobX robot.
        /// </summary>
        public enum BaudRates
        {
            /// <summary>
            /// Baud rate of 9600 symbols per second.
            /// </summary>
            BaudRate9600 = 9600,

            /// <summary>
            /// Baud rate of 38400 symbols per second.
            /// </summary>
            BaudRate38400 = 38400
        }

        /// <summary>
        /// Speed modes for MD49 motor driver used in RobX robot.
        /// </summary>
        public enum SpeedModes
        {
            /// <summary>
            /// (Default) The speeds of wheels are in the range of 0 (Full Reverse) 128 (Stop) 255 (Full Forward).
            /// </summary>
            Mode0 = 0,

            /// <summary>
            /// The speeds of wheels are in the range of -128 (Full Reverse) 0 (Stop) 127 (Full Forward).
            /// </summary>
            Mode1 = 1,

            /// <summary>
            /// Uses SPEED 1 for both motors, and SPEED 2 for turn value. 
            /// Data is in the range of 0 (Full Reverse) 128 (Stop) 255 (Full Forward).
            /// </summary>
            Mode2 = 2,

            /// <summary>
            /// Uses SPEED 1 for both motors, and SPEED 2 for turn value. 
            /// Data is in the range of -128 (Full Reverse) 0 (Stop) 127 (Full Forward).
            /// </summary>
            Mode3 = 3
        }

        /// <summary>
        /// Robot types: Real Robot / Simulation.
        /// </summary>
        public enum RobotType
        {
            /// <summary>
            /// Real robot type.
            /// </summary>
            Real = 0,

            /// <summary>
            /// Simulated robot type.
            /// </summary>
            Simulation = 1
        }

        # endregion
    }
}
