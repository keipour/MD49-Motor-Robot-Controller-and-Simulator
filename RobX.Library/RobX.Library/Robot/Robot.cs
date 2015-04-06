# region Includes

using System;
using System.IO.Ports;
using RobX.Library.Commons;

# endregion

namespace RobX.Library.Robot
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
        public const double EncoderCount2mM = WheelDiameter * Math.PI / EncoderCountPerTurn;

        # endregion

        # region Hardware Interface (COM) Properties

        /// <summary>
        /// Number of data bits for connection with COM port of the MD49 motor used in RobX robot.
        /// </summary>
        public const int DataBits = 8;

        /// <summary>
        /// Parity for connection with COM port of the MD49 motor used in RobX robot.
        /// </summary>
        public const Parity Parity = System.IO.Ports.Parity.None;

        /// <summary>
        /// Number of stop bits for connection with COM port of the MD49 motor used in RobX robot.
        /// </summary>
        public const StopBits StopBits = System.IO.Ports.StopBits.Two;

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

        # region Public Static Functions

        /// <summary>
        /// Calculates real wheel speeds from motor speeds (in millimeters per second).
        /// </summary>
        /// <param name="mode">Current mode of the robot motor driver.</param>
        /// <param name="motorSpeed1">Speed1 value of robot motor.</param>
        /// <param name="motorSpeed2">Speed2 value of robot motor.</param>
        /// <param name="wheelSpeed1">Real speed of left wheel in millimeters per second.</param>
        /// <param name="wheelSpeed2">Real speed of right wheel in millimeters per second.</param>
        public static void MotorSpeedToRealSpeed(int mode, int motorSpeed1, int motorSpeed2, 
            out double wheelSpeed1, out double wheelSpeed2)
        {
            wheelSpeed1 = 0;
            wheelSpeed2 = 0;

            switch (mode)
            {
                case 0:
                    wheelSpeed1 = motorSpeed1 - 128;
                    wheelSpeed2 = motorSpeed2 - 128;
                    break;
                case 1:
                    wheelSpeed1 = Methods.ConvertUnsignedByteToSigned(motorSpeed1);
                    wheelSpeed2 = Methods.ConvertUnsignedByteToSigned(motorSpeed2);
                    break;
                case 2:
                    if (motorSpeed1 >= 128)                  // if forward direction
                    {
                        wheelSpeed1 = motorSpeed1 + motorSpeed2 - 256;
                        if (wheelSpeed1 > 127) wheelSpeed1 = 127;

                        wheelSpeed2 = motorSpeed1 - motorSpeed2;
                        if (wheelSpeed2 > 127) wheelSpeed2 = 127;
                    }
                    else                              // if backward direction
                    {
                        wheelSpeed1 = motorSpeed1 - motorSpeed2;
                        if (wheelSpeed1 < -128) wheelSpeed1 = -128;

                        wheelSpeed2 = motorSpeed1 + motorSpeed2 - 256;
                        if (wheelSpeed2 < -128) wheelSpeed2 = -128;
                    }
                    break;
                case 3:
                    int speed1Signed = Methods.ConvertUnsignedByteToSigned(motorSpeed1);
                    int speed2Signed = Methods.ConvertUnsignedByteToSigned(motorSpeed2);

                    if (speed1Signed >= 0)                  // if forward direction
                    {
                        wheelSpeed1 = speed1Signed + speed2Signed;
                        if (wheelSpeed1 > 127) wheelSpeed1 = 127;

                        wheelSpeed2 = speed1Signed - speed2Signed;
                        if (wheelSpeed2 > 127) wheelSpeed2 = 127;
                    }
                    else                              // if backward direction
                    {
                        wheelSpeed1 = speed1Signed - speed2Signed;
                        if (wheelSpeed1 < -128) wheelSpeed1 = -128;

                        wheelSpeed2 = speed1Signed + speed2Signed;
                        if (wheelSpeed2 < -128) wheelSpeed2 = -128;
                    }
                    break;
            }
            wheelSpeed1 *= RobotSpeedToMmpS;
            wheelSpeed2 *= RobotSpeedToMmpS;
        }

        /// <summary>
        /// Calculates new state of the robot based on the previous state and an update time slice. Uses 
        /// forward kinematics formula for a differential drive robot.
        /// </summary>
        /// <param name="time">Update time in milliseconds.</param>
        /// <param name="speed1">Speed of left wheel of the robot in millimeters per second.</param>
        /// <param name="speed2">Speed of right wheel of the robot in millimeters per second.</param>
        /// <param name="robotX">X position of the robot in millimeters. Also used as output of the new X position.</param>
        /// <param name="robotY">Y position of the robot in millimeters. Also used as output of the new Y position.</param>
        /// <param name="robotAngle">Angle of the robot with respect to X axis (counter-clockwise) in degrees.
        ///  Also used as output of the new angle.</param>
        public static void ForwardKinematics(double time, double speed1, double speed2,
            ref double robotX, ref double robotY, ref double robotAngle)
        {
            // Check if wheel speeds are equal
            if (Math.Abs(speed1 - speed2) < 0.00001) // if speed are equal
            {
                robotX += speed1*Math.Cos(Math.PI/180*robotAngle)*time;
                robotY += speed1*Math.Sin(Math.PI/180*robotAngle)*time;
                return;
            }

            // if wheel speeds are not equal:

            const double l = 2*Radius; // Distance between centers of two wheels
            var r = l/2*(speed1 + speed2)/(speed2 - speed1); // Radius of rotation
            var w = (speed2 - speed1)/l; // Angular velocity of rotation

            // Calculate center of rotation
            var icCx = robotX - r*Math.Sin(Math.PI/180*robotAngle);
            var icCy = robotY + r*Math.Cos(Math.PI/180*robotAngle);

            // Calculate new x, y and angle of robot
            var wt = w*time;
            var newX = Math.Cos(wt)*(robotX - icCx) - Math.Sin(wt)*(robotY - icCy) + icCx;
            var newY = Math.Sin(wt)*(robotX - icCx) + Math.Cos(wt)*(robotY - icCy) + icCy;
            var newAngle = (robotAngle + wt*180/Math.PI)%360;

            // Replace old position values
            robotX = newX;
            robotY = newY;
            robotAngle = newAngle;

        }

        /// <summary>
        /// Calculates encoder count for the distance traversed by a robot wheel.
        /// </summary>
        /// <param name="distance">Distance traversed by a robot wheel in millimeters.</param>
        /// <returns>Encoder count for the given distance.</returns>
        public static int DistanceToEncoderCount(double distance)
        {
            return (int)(distance / EncoderCount2mM);
        }

        /// <summary>
        /// Converts real speed (millimeters per second) to robot wheel speed.
        /// </summary>
        /// <param name="wheelSpeed">Real speed in millimeters per second.</param>
        /// <returns>Robot wheel speed (in range -127 to 127).</returns>
        public static int RealWheelSpeedToMotorSpeed(double wheelSpeed)
        {
            return (int)(wheelSpeed / RobotSpeedToMmpS);
        }

        # endregion
    }
}
