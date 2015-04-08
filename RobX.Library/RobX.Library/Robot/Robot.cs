# region Includes

using System;
using System.IO.Ports;
using System.Linq;
using RobX.Library.Commons;
using RobX.Library.Communication;

# endregion

namespace RobX.Library.Robot
{
    /// <summary>
    /// Class that contains all commands and properties of MD49 motor driver and RobX robot.
    /// </summary>
    public class Robot
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

        # region Public Functions

        /// <summary>
        /// Connect to robot or simulator over a communication socket (network, COM, etc.).
        /// </summary>
        /// <param name="robotClient">Client (TCPClient, ComClient, etc.) instance intended to be used in 
        /// the communication with the robot.</param>
        /// <param name="connectFunction">Function used for connecting to robot. The return value of the function
        /// must be bool and there should be no input parameters for the function. In order to provides parameters
        /// for this function, you may use '() => funcName(parameters)' as the input, instead of simply using 
        /// 'funcName'.</param>
        /// <returns>Returns true if successfully connected to the robot.</returns>
        public bool Connect(IClientInterface robotClient, Func<bool> connectFunction)
        {
            _robotClient = robotClient;

            // Add event handlers
            _robotClient.ReceivedData += RobotReceivedData;
            _robotClient.SentData += RobotSentData;
            _robotClient.StatusChanged += RobotStatusChanged;
            _robotClient.BeforeSendingData += RobotBeforeSendingData;
            _robotClient.ErrorOccured += RobotErrorOccured;

            return connectFunction();
        }

        /// <summary>
        /// Get voltage of the robot batteries.
        /// </summary>
        /// <returns>The voltage of the robot batteries (in volts).</returns>
        public byte GetVolts()
        {
            byte[] buffer = { 0x00, 0x26 };
            _robotClient.SendData(buffer);

            return ReadBytes(1)[0];
        }

        /// <summary>
        /// Get current drawn by the motor 1 of the robot (1 Amperes = 10).
        /// </summary>
        /// <returns>Ten times the amperage of the robot motor 1.</returns>
        public byte GetCurrent1()
        {
            byte[] buffer = { 0x00, 0x27 };
            _robotClient.SendData(buffer);

            return ReadBytes(1)[0];
        }

        /// <summary>
        /// Get current drawn by the motor 2 of the robot (1 Amperes = 10).
        /// </summary>
        /// <returns>Ten times the amperage of the robot motor 2.</returns>
        public byte GetCurrent2()
        {
            byte[] buffer = { 0x00, 0x28 };
            _robotClient.SendData(buffer);

            return ReadBytes(1)[0];
        }

        /// <summary>
        /// Get the voltage of the batteries and the current drawn by the robot motors.
        /// </summary>
        /// <param name="volts">Voltage of the robot batteries.</param>
        /// <param name="current1">Ten times the amperage of the robot motor 1.</param>
        /// <param name="current2">Ten times the amperage of the robot motor 2.</param>
        public void GetVi(out int volts, out int current1, out int current2)
        {
            byte[] buffer = { 0x00, 0x2C };
            _robotClient.SendData(buffer);

            buffer = ReadBytes(3);

            volts = buffer[0];
            current1 = buffer[1];
            current2 = buffer[2];
        }

        /// <summary>
        /// Get the version of the robot motor firmware.
        /// </summary>
        /// <returns>The version of the robot motor firmware.</returns>
        public byte GetVersion()
        {
            byte[] buffer = { 0x00, 0x29 };
            _robotClient.SendData(buffer);

            return ReadBytes(1)[0];
        }

        /// <summary>
        /// Get the Speed1 value of the robot. 
        /// In mode 0 or 1 gets the speed and direction of wheel 1. 
        /// In mode 2 or 3 gets the speed and direction of both wheels (subject to effect of turn register).
        /// </summary>
        /// <returns>Speed1 value of the robot.</returns>
        public short GetSpeed1()
        {
            byte[] buffer = { 0x00, 0x21 };
            _robotClient.SendData(buffer);

            return ReadBytes(1)[0];
        }

        /// <summary>
        /// Get the Speed2 value of the robot. 
        /// In mode 0 or 1 gets the speed and direction of wheel 2. 
        /// In mode 2 or 3 becomes a Turn value, and is combined with Speed 1 to steer the device. 
        /// </summary>
        /// <returns>Speed2 value of the robot.</returns>
        public short GetSpeed2()
        {
            byte[] buffer = { 0x00, 0x22 };
            _robotClient.SendData(buffer);

            return ReadBytes(1)[0];
        }

        /// <summary>
        /// Get the encoder value of wheel 1 of the robot.
        /// </summary>
        /// <returns>Encoder value of wheel 1 of the robot.</returns>
        public int GetEncoder1()
        {
            byte[] buffer = { 0x00, 0x23 };
            _robotClient.SendData(buffer);

            buffer = ReadBytes(4);

            int result = buffer[3];
            result += buffer[2] << 8;
            result += buffer[1] << 16;
            result += buffer[0] << 24;
            return result;
        }

        /// <summary>
        /// Get the encoder value of wheel 2 of the robot.
        /// </summary>
        /// <returns>Encoder value of wheel 2 of the robot.</returns>
        public int GetEncoder2()
        {
            byte[] buffer = { 0x00, 0x24 };
            _robotClient.SendData(buffer);

            buffer = ReadBytes(4);

            int result = buffer[3];
            result += buffer[2] << 8;
            result += buffer[1] << 16;
            result += buffer[0] << 24;
            return result;
        }

        /// <summary>
        /// Get the encoder values of both wheels of the robot.
        /// </summary>
        /// <param name="encoder1">Encoder value of wheel 1 of the robot.</param>
        /// <param name="encoder2">Encoder value of wheel 2 of the robot.</param>
        public void GetEncoders(out int encoder1, out int encoder2)
        {
            byte[] buffer = { 0x00, 0x25 };
            _robotClient.SendData(buffer);

            buffer = ReadBytes(8);

            encoder1 = buffer[3];
            encoder1 += buffer[2] << 8;
            encoder1 += buffer[1] << 16;
            encoder1 += buffer[0] << 24;

            encoder2 = buffer[7];
            encoder2 += buffer[6] << 8;
            encoder2 += buffer[5] << 16;
            encoder2 += buffer[4] << 24;
        }

        /// <summary>
        /// Get acceleration level (1 - 10) for robot motors.
        /// </summary>
        /// <returns>Acceleration level for robot motors.</returns>
        public byte GetAcceleration()
        {
            byte[] buffer = { 0x00, 0x2A };
            _robotClient.SendData(buffer);

            return ReadBytes(1)[0];
        }

        /// <summary>
        /// <para>Get mode of the motor:</para>
        /// <para>0 (Default) : The speeds of wheels are in the range of 0 (Full Reverse) 128 (Stop) 255 (Full Forward).</para>
        /// <para>1 : The speeds of wheels are in the range of -128 (Full Reverse) 0 (Stop) 127 (Full Forward).</para>
        /// <para>2 :	Uses SPEED 1 for both motors, and SPEED 2 for turn value. 
        /// Data is in the range of 0 (Full Reverse) 128 (Stop) 255 (Full Forward).</para>
        /// <para>3 :	Uses SPEED 1 for both motors, and SPEED 2 for turn value. 
        /// Data is in the range of -128 (Full Reverse) 0 (Stop) 127 (Full Forward).</para>
        /// </summary>
        /// <returns>Mode of the motor (0 - 3).</returns>
        public byte GetMode()
        {
            byte[] buffer = { 0x00, 0x2B };
            _robotClient.SendData(buffer);

            return ReadBytes(1)[0];
        }

        /// <summary>
        /// Get the error state of the robot.
        /// </summary>
        /// <param name="voltsUnder16">If true, voltage of the robot batteries is under 16 volts.</param>
        /// <param name="voltsOver30">If true, voltage of the robot batteries is over 30 volts.</param>
        /// <param name="motor1Trip">If true, wheel 1 of the robot is tripping.</param>
        /// <param name="motor2Trip">If true, wheel 2 of the robot is tripping.</param>
        /// <param name="motor1Short">If true, motor 1 of the robot is short-circuited (draws large current).</param>
        /// <param name="motor2Short">If true, motor 2 of the robot is short-circuited (draws large current).</param>
        /// <returns>Returns false if no errors occured; otherwise returns true.</returns>
        public bool GetError(out bool voltsUnder16, out bool voltsOver30, out bool motor1Trip,
            out bool motor2Trip, out bool motor1Short, out bool motor2Short)
        {
            byte[] buffer = { 0x00, 0x2D };
            _robotClient.SendData(buffer);

            buffer = ReadBytes(1);

            voltsUnder16 = (buffer[0] & (1 << 7)) == 1;
            voltsOver30 = (buffer[0] & (1 << 6)) == 1;
            motor1Trip = (buffer[0] & (1 << 2)) == 1;
            motor2Trip = (buffer[0] & (1 << 3)) == 1;
            motor1Short = (buffer[0] & (1 << 4)) == 1;
            motor2Short = (buffer[0] & (1 << 5)) == 1;
            return (voltsOver30 && voltsUnder16 && motor1Short && motor1Trip && motor2Short && motor2Trip);
        }

        /// <summary>
        /// Set the Speed1 value of the robot. 
        /// In mode 0 or 1 sets the speed and direction of wheel 1. 
        /// In mode 2 or 3 sets the speed and direction of both wheels (subject to effect of turn register).
        /// </summary>
        /// <param name="speed">Speed1 value of the robot.</param>
        public void SetSpeed1(short speed)
        {
            byte[] buffer = { 0x00, 0x31, (byte)speed };
            _robotClient.SendData(buffer);
        }

        /// <summary>
        /// Set the Speed2 value of the robot. 
        /// In mode 0 or 1 sets the speed and direction of wheel 2. 
        /// In mode 2 or 3 becomes a turn value, and is combined with Speed 1 to steer the device. 
        /// </summary>
        /// <param name="speed">Speed2 value of the robot.</param>
        public void SetSpeed2(short speed)
        {
            byte[] buffer = { 0x00, 0x32, (byte)speed };
            _robotClient.SendData(buffer);
        }

        /// <summary>
        /// Set both Speed1 and Speed2 simultaneously for the robot. 
        /// In mode 0 or 1 Speed1 sets the speed and direction of wheel 1 and Speed2 sets the speed and direction of wheel 2. 
        /// In mode 2 or 3 Speed1 sets the speed and direction of both wheels (subject to effect of Spees2 register) and Speed 2
        /// becomes a turn value, and is combined with Speed1 to steer the device.
        /// </summary>
        /// <param name="speed1">Speed1 value of the robot.</param>
        /// <param name="speed2">Speed2 value of the robot.</param>
        public void SetSpeeds(short speed1, short speed2)
        {
            byte[] buffer = { 0x00, 0x31, (byte)speed1, 0x00, 0x32, (byte)speed2 };
            _robotClient.SendData(buffer);
        }

        /// <summary>
        /// Set acceleration level (1 - 10) for robot motors.
        /// </summary>
        public void SetAcceleration(byte acceleration)
        {
            byte[] buffer = { 0x00, 0x33, acceleration };
            _robotClient.SendData(buffer);
        }

        /// <summary>
        /// Get mode of the motor:
        /// 0 (Default) : The speeds of wheels are in the range of 0 (Full Reverse) 128 (Stop) 255 (Full Forward).
        /// 1 : The speeds of wheels are in the range of -128 (Full Reverse) 0 (Stop) 127 (Full Forward).
        /// 2 :	Uses SPEED 1 for both motors, and SPEED 2 for turn value. Data is in the range of 0 (Full Reverse) 128 (Stop) 255 (Full Forward).
        /// 3 :	Uses SPEED 1 for both motors, and SPEED 2 for turn value. Data is in the range of -128 (Full Reverse) 0 (Stop) 127 (Full Forward).
        /// </summary>
        /// <returns>Mode of the motor (0 - 3).</returns>
        public void SetMode(SpeedModes mode)
        {
            byte[] buffer = { 0x00, 0x34, (byte)mode };
            _robotClient.SendData(buffer);
        }

        /// <summary>
        /// Reset (set to zero) the values of robot wheel encoders.
        /// </summary>
        public void ResetEncoders()
        {
            byte[] buffer = { 0x00, 0x35 };
            _robotClient.SendData(buffer);
        }

        /// <summary>
        /// Turn off speed regulation: If the required speed is not being achieved, increase power to the motors 
        /// until it reaches the desired rate (or the motors reach the maximum output).
        /// </summary>
        public void DisableRegulator()
        {
            byte[] buffer = { 0x00, 0x36 };
            _robotClient.SendData(buffer);
        }

        /// <summary>
        /// Turn on speed regulation: If the required speed is not being achieved, increase power to the motors 
        /// until it reaches the desired rate (or the motors reach the maximum output).
        /// </summary>
        public void EnableRegulator()
        {
            byte[] buffer = { 0x00, 0x37 };
            _robotClient.SendData(buffer);
        }

        /// <summary>
        /// Turn off motor timeout: Robot will automatically stop if there is no serial communications within 2 seconds.
        /// </summary>
        public void DisableTimeout()
        {
            byte[] buffer = { 0x00, 0x38 };
            _robotClient.SendData(buffer);
        }

        /// <summary>
        /// Turn on motor timeout: Robot will automatically stop if there is no serial communications within 2 seconds.
        /// </summary>
        public void EnableTimeout()
        {
            byte[] buffer = { 0x00, 0x39 };
            _robotClient.SendData(buffer);
        }

        /// <summary>
        /// Set x position of the robot in the environment in millimeters (works only in simulation mode).
        /// </summary>
        /// <param name="x">X position of the robot in the environment in millimeters.</param>
        public void SetX(short x)
        {
            if (_robotType == RobotType.Real) return;
            byte[] buffer = { 0x00, 0x41, (byte)(x >> 8), (byte)(x & 0xFF) };
            _robotClient.SendData(buffer);
        }

        /// <summary>
        /// Set y position of the robot in the environment in millimeters (works only in simulation mode)
        /// </summary>
        /// <param name="y">Y position of the robot in the environment in millimeters.</param>
        public void SetY(short y)
        {
            if (_robotType == RobotType.Real) return;
            byte[] buffer = { 0x00, 0x42, (byte)(y >> 8), (byte)(y & 0xFF) };
            _robotClient.SendData(buffer);
        }

        /// <summary>
        /// Set angle of the robot in the environment in degrees (works only in simulation mode).
        /// </summary>
        /// <param name="angle">Ten times the angle of the robot in the environment in degrees (1 = 0.1 degrees).</param>
        public void SetAngle(short angle)
        {
            if (_robotType == RobotType.Real) return;
            byte[] buffer = { 0x00, 0x43, (byte)(angle >> 8), (byte)(angle & 0xFF) };
            _robotClient.SendData(buffer);
        }

        /// <summary>
        /// Set x and y positions of the robot in the environment in millimeters (works only in simulation mode)
        /// </summary>
        /// <param name="x">X position of the robot in the environment in millimeters.</param>
        /// <param name="y">Y position of the robot in the environment in millimeters.</param>
        public void SetXy(short x, short y)
        {
            if (_robotType == RobotType.Real) return;
            byte[] buffer =
            {
                0x00, 0x41, (byte) (x >> 8), (byte) (x & 0xFF),
                0x00, 0x42, (byte) (y >> 8), (byte) (y & 0xFF)
            };
            _robotClient.SendData(buffer);
        }

        /// <summary>
        /// Set angle (in degrees), x and y positions (in millimeters) of the robot in the environment (works only in simulation mode).
        /// </summary>
        /// <param name="x">X position of the robot in the environment in millimeters.</param>
        /// <param name="y">Y position of the robot in the environment in millimeters.</param>
        /// <param name="angle">Ten times the angle of the robot in the environment in degrees (1 = 0.1 degrees).</param>
        public void SetXyAngle(short x, short y, short angle)
        {
            if (_robotType == RobotType.Real) return;
            byte[] buffer =
            {
                0x00, 0x41, (byte) (x >> 8), (byte) (x & 0xFF),
                0x00, 0x42, (byte) (y >> 8), (byte) (y & 0xFF),
                0x00, 0x43, (byte) (angle >> 8), (byte) (angle & 0xFF)
            };
            _robotClient.SendData(buffer);
        }

        /// <summary>
        /// Set simulation speed on simulator (works only in simulation mode).
        /// </summary>
        /// <param name="speed">Ten times the new simulation speed (1 = 0.1x).</param>
        public void SetSimulationSpeed(ushort speed)
        {
            if (_robotType == RobotType.Real) return;
            byte[] buffer = { 0x00, 0x51, (byte)(speed >> 8), (byte)(speed & 0xFF) };
            _robotClient.SendData(buffer);
        }

        /// <summary>
        /// Get simulation speed of simulator (works only in simulation mode).
        /// </summary>
        /// <returns>Ten time the simulation speed of the simulator (1 = 0.1x).</returns>
        public ushort GetSimulationSpeed()
        {
            if (_robotType == RobotType.Real) return 0;
            byte[] buffer = { 0x00, 0x52 };
            _robotClient.SendData(buffer);

            buffer = ReadBytes(2);
            ushort result = buffer[1];
            result += (ushort)(buffer[0] << 8);
            return result;
        }

        # endregion

        # region Event Handlers

        /// <summary>
        /// Event handler for ReceivedData event (will be invoked after data is received).
        /// </summary>
        public event CommunicationEventHandler ReceivedData;

        /// <summary>
        /// Event handler for SentData event (will be invoked after data is sent).
        /// </summary>
        public event CommunicationEventHandler SentData;

        /// <summary>
        /// Event handler for BeforeSendingData event (will be invoked when server is ready to send data).
        /// </summary>
        public event CommunicationEventHandler BeforeSendingData;

        /// <summary>
        /// Event handler for StatusChange event (will be invoked after anything new happens in the communication).
        /// </summary>
        public event CommunicationStatusEventHandler StatusChanged;

        /// <summary>
        /// Event that is invoked when an error is occured.
        /// </summary>
        public event EventHandler ErrorOccured;

        # endregion

        # region Private Variables

        private IClientInterface _robotClient;
        private readonly RobotType _robotType;

        # endregion

        # region Constructors and Events

        /// <summary>
        /// Constructor for the robot class.
        /// </summary>
        /// <param name="robotType">Specifies robot type (Simulation vs. Real).</param>
        public Robot(RobotType robotType)
        {
            _robotType = robotType;
        }

        /// <summary>
        /// Invokes the ReceivedData event when data is received from the robot (over the network).
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">Event arguments including the received data.</param>
        private void RobotReceivedData(object sender, CommunicationEventArgs e)
        {
            if (ReceivedData != null)
                ReceivedData(this, e);
        }

        /// <summary>
        /// Invokes the SentData event when data is sent to the robot (over the network).
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">Event arguments including the sent data.</param>
        private void RobotSentData(object sender, CommunicationEventArgs e)
        {
            if (SentData != null)
                SentData(this, e);
        }

        /// <summary>
        /// Invokes the StatusChanged event when the robot connection state is changed.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">Event arguments including the status change message.</param>
        private void RobotStatusChanged(object sender, CommunicationStatusEventArgs e)
        {
            if (StatusChanged != null)
                StatusChanged(this, e);
        }


        /// <summary>
        /// Invokes the BeforeSendingData event when data is ready to be sent to the robot (over the network).
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">Event arguments including the sent data.</param>
        private void RobotBeforeSendingData(object sender, CommunicationEventArgs e)
        {
            if (BeforeSendingData != null)
                BeforeSendingData(this, e);
        }

        private void RobotErrorOccured(object sender, EventArgs e)
        {
            if (ErrorOccured != null)
                ErrorOccured(this, e);
        }

        # endregion

        # region Private Functions

        // This function reads bytes from remote client. Fills return array with zero if anythig goes wrong.
        private byte[] ReadBytes(int bytesToRead)
        {
            byte[] buffer;

            if (!_robotClient.ReceiveData(bytesToRead, out buffer) || buffer.Length < bytesToRead)
                return Enumerable.Repeat<byte>(0, bytesToRead).ToArray();

            return buffer;
        }

        # endregion
    }
}
