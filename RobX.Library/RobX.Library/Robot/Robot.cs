using System;
using RobX.Library.Commons;

// ReSharper disable UnusedMember.Global
namespace RobX.Library.Robot
{
    /// <summary>
    /// Class that contains all commands and properties of MD49 motor driver and RobX robot.
    /// </summary>
    public class Robot : MotorDriver
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

        # region Robot Properties

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
        public const double EncoderCountToDistance = WheelDiameter * Math.PI / EncoderCountPerTurn;

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
            return (int)(distance / EncoderCountToDistance);
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

        # region CalculateDelay Function

        /// <summary>
        /// Estimates the delay of communication with robot by sending a single command and calculating the responce time.
        /// </summary>
        /// <returns>Estimated delay of communication with robot in milliseconds.</returns>
        // ReSharper disable once UnusedMember.Global
        public TimeSpan CalculateDelay()
        {
            var now = DateTime.Now;
            GetVersion();
            return DateTime.Now - now;
        }

        # endregion
    }
}
