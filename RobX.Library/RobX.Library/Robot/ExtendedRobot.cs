using System;
using System.Threading;
using RobX.Library.Communication;

namespace RobX.Library.Robot
{
    /// <summary>
    /// A class that extends the simple robot class with additional functions and capabilities.
    /// </summary>
    public class ExtendedRobot : Robot
    {
        # region RobotStatusChanged Event Handler and Function

        /// <summary>
        /// RobotStatusChanged event (will be invoked after anything new happens to the robot in the controller).
        /// </summary>
        public event CommunicationStatusEventHandler RobotStatusChanged;

        protected internal void ChangeRobotStatus(string status)
        {
            if (RobotStatusChanged != null)
                RobotStatusChanged(this, new CommunicationStatusEventArgs(status));
        }

        # endregion

        # region Private Fields

        private double _simulationSpeed = 1.0F;

        # endregion

        # region Public Fields

        /// <summary>
        /// Type of the robot (real or simulation).
        /// </summary>
        public readonly RobotType Type;

        /// <summary>
        /// String containing the error detected in the robot at the last health checking. Empty string indicates no error.
        /// </summary>
        public string ErrorDescription { get; private set; }

        # endregion

        # region Constructor

        /// <summary>
        /// Constructor for the robot class.
        /// </summary>
        /// <param name="type">Specifies robot type (Simulation vs. Real).</param>
        public ExtendedRobot(RobotType type)
        {
            ErrorDescription = String.Empty;
            Type = type;
        }

        # endregion

        # region Additional Simulator Commands

        /// <summary>
        /// Set x position of the robot in the environment in millimeters (works only in simulation mode).
        /// </summary>
        /// <param name="x">X position of the robot in the environment in millimeters.</param>
        public void SetX(short x)
        {
            if (Type == RobotType.Real) return;
            byte[] buffer = { 0x00, 0x41, (byte)(x >> 8), (byte)(x & 0xFF) };
            RobotClient.SendData(buffer);
        }

        /// <summary>
        /// Set y position of the robot in the environment in millimeters (works only in simulation mode)
        /// </summary>
        /// <param name="y">Y position of the robot in the environment in millimeters.</param>
        public void SetY(short y)
        {
            if (Type == RobotType.Real) return;
            byte[] buffer = { 0x00, 0x42, (byte)(y >> 8), (byte)(y & 0xFF) };
            RobotClient.SendData(buffer);
        }

        /// <summary>
        /// Set angle of the robot in the environment in degrees (works only in simulation mode).
        /// </summary>
        /// <param name="dblAngle">The angle of the robot in the environment in degrees. Will be rounded 
        /// to the nearest tenth value (i.e. 0.1).</param>
        public void SetAngle(double dblAngle)
        {
            if (Type == RobotType.Real) return;
            var angle = (short)Math.Round(dblAngle * 10);
            byte[] buffer = { 0x00, 0x43, (byte)(angle >> 8), (byte)(angle & 0xFF) };
            RobotClient.SendData(buffer);
        }

        /// <summary>
        /// Set x and y positions of the robot in the environment in millimeters (works only in simulation mode).
        /// </summary>
        /// <param name="x">X position of the robot in the environment in millimeters.</param>
        /// <param name="y">Y position of the robot in the environment in millimeters.</param>
        public void SetPosition(short x, short y)
        {
            if (Type == RobotType.Real) return;
            byte[] buffer =
            {
                0x00, 0x41, (byte) (x >> 8), (byte) (x & 0xFF),
                0x00, 0x42, (byte) (y >> 8), (byte) (y & 0xFF)
            };
            RobotClient.SendData(buffer);
        }

        /// <summary>
        /// Set angle (in degrees), x and y positions (in millimeters) of the robot in the environment 
        /// (works only in simulation mode).
        /// </summary>
        /// <param name="x">X position of the robot in the environment in millimeters.</param>
        /// <param name="y">Y position of the robot in the environment in millimeters.</param>
        /// <param name="dblAngle">The angle of the robot in the environment in degrees. Will be rounded 
        /// to the nearest tenth value (i.e. 0.1).</param>
        public void SetPose(short x, short y, double dblAngle)
        {
            if (Type == RobotType.Real) return;
            var angle = (short)Math.Round(dblAngle * 10);
            byte[] buffer =
            {
                0x00, 0x41, (byte) (x >> 8), (byte) (x & 0xFF),
                0x00, 0x42, (byte) (y >> 8), (byte) (y & 0xFF),
                0x00, 0x43, (byte) (angle >> 8), (byte) (angle & 0xFF)
            };
            RobotClient.SendData(buffer);
        }

        /// <summary>
        /// Set simulation speed on simulator (works only in simulation mode).
        /// </summary>
        /// <param name="dblSpeed">New simulation speed (1 = 0.1x). Must be a positive decimal. Will be rounded to 
        /// the nearest tenth value (i.e. 0.1).</param>
        public void SetSimulationSpeed(double dblSpeed)
        {
            _simulationSpeed = dblSpeed;
            if (Type == RobotType.Real) return;
            var speed = (ushort)Math.Round(dblSpeed * 10);
            byte[] buffer = { 0x00, 0x51, (byte)(speed >> 8), (byte)(speed & 0xFF) };
            RobotClient.SendData(buffer);
        }

        /// <summary>
        /// Get simulation speed of simulator (works only in simulation mode).
        /// </summary>
        /// <returns>The simulation speed of the simulator.</returns>
        public double GetSimulationSpeed()
        {
            if (Type == RobotType.Real) return 0;
            byte[] buffer = { 0x00, 0x52 };
            RobotClient.SendData(buffer);

            buffer = ReadBytes(2);
            ushort result = buffer[1];
            result += (ushort)(buffer[0] << 8);
            return result / 10F;
        }

        # endregion

        # region Additional High-level Commands

        /// <summary>
        /// Robot will keep its current state for the specified amount of time.
        /// </summary>
        /// <param name="time">Execution time in milliseconds.</param>
        /// <param name="showMessage">If true, invoke RobotStatusChanged event with a message.</param>
        /// <exception cref="ArgumentOutOfRangeException">This exception is thrown when the input time is negative.</exception>
        public void DoNothing(double time = 0, bool showMessage = true)
        {
            if (time < 0) throw new ArgumentOutOfRangeException("time", @"Error! Time can't be negative!");

            if (showMessage)
                ChangeRobotStatus("Robot will keep the current state for " + time.ToString("0.00") + " milliseconds.");

            if (time > 0)
                Thread.Sleep(TimeSpan.FromMilliseconds(time / _simulationSpeed));
        }

        /// <summary>
        /// Sets robot wheel speeds for a specified amount of time.
        /// </summary>
        /// <param name="time">Execution time in milliseconds.</param>
        /// <param name="wheel1Speed">Speed of wheel 1 (in the range -127 to 127).</param>
        /// <param name="wheel2Speed">Speed of wheel 2 (in the range -127 to 127).</param>
        /// <param name="showMessage">If true, invoke RobotStatusChanged event with a message.</param>
        /// <exception cref="ArgumentOutOfRangeException">This exception is thrown when the input time is negative.</exception>
        public void SetSpeedMilliseconds(double time, sbyte wheel1Speed, sbyte wheel2Speed,
            bool showMessage = true)
        {
            if (time < 0) throw new ArgumentOutOfRangeException("time", @"Error! Time can't be negative!");

            if (showMessage)
                ChangeRobotStatus("Setting the speed of the left wheel to " + wheel1Speed +
                    " (" + (wheel1Speed * RobotSpeedToMmpS).ToString("0.0") + " mm/s) and the right wheel to " +
                    wheel2Speed + " (" + (wheel2Speed * RobotSpeedToMmpS).ToString("0.0") +
                    " mm/s) for " + time.ToString("0.00") + " milliseconds.");

            SetSpeeds((byte)(wheel1Speed + 128), (byte)(wheel2Speed + 128));

            DoNothing(time, false);
        }

        /// <summary>
        /// Sets robot wheel speeds and turns for a specified amount of degrees.
        /// </summary>
        /// <param name="degrees">Turn amount in degrees.</param>
        /// <param name="wheel1Speed">Speed of wheel 1 (in the range -127 to 127).</param>
        /// <param name="wheel2Speed">Speed of wheel 2 (in the range -127 to 127).</param>
        /// <param name="showMessage">If true, invoke RobotStatusChanged event with a message.</param>
        public void SetSpeedDegrees(double degrees, sbyte wheel1Speed, sbyte wheel2Speed, bool showMessage = true)
        {
            if (showMessage)
                ChangeRobotStatus("Turning robot " + ((wheel1Speed < wheel2Speed) ? "counter-" : "") + "clockwise " +
                                  degrees.ToString("0.0") + " degrees with the speed of the left wheel set to " +
                                  wheel1Speed + " (" + (wheel1Speed * RobotSpeedToMmpS).ToString("0.0") +
                                  " mm/s) and the right wheel set to " + wheel2Speed + " (" +
                                  (wheel2Speed * RobotSpeedToMmpS).ToString("0.0") + " mm/s).");

            var deltaV = (wheel1Speed - wheel2Speed) * RobotSpeedToMmpS / 1000F;
            var radians = degrees * Math.PI / 180F;
            var milliseconds = Math.Abs(2 * Radius * radians / deltaV);

            SetSpeeds((byte)(wheel1Speed + 128), (byte)(wheel2Speed + 128));
            SetSpeedMilliseconds(milliseconds, wheel1Speed, wheel2Speed, false);
        }

        /// <summary>
        /// Sets robot wheel speeds and turns for a specified distance.
        /// </summary>
        /// <param name="distance">Distance in millimeters.</param>
        /// <param name="wheel1Speed">Speed of wheel 1 (in the range -127 to 127).</param>
        /// <param name="wheel2Speed">Speed of wheel 2 (in the range -127 to 127).</param>
        /// <param name="showMessage">If true, invoke RobotStatusChanged event with a message.</param>
        /// <exception cref="ArgumentOutOfRangeException">This exception is thrown when the input distance is negative.</exception>
        public void SetSpeedMillimeters(double distance, sbyte wheel1Speed, sbyte wheel2Speed, bool showMessage = true)
        {
            if (distance < 0) throw new ArgumentOutOfRangeException("distance", @"Error! Distance can't be negative!");

            if (showMessage)
                ChangeRobotStatus("Setting the speed of the left wheel to " + wheel1Speed +
                    " (" + (wheel1Speed * RobotSpeedToMmpS).ToString("0.0") + " mm/s) and the right wheel to " +
                    wheel2Speed + " (" + (wheel2Speed * RobotSpeedToMmpS).ToString("0.0") +
                    " mm/s) for " + distance.ToString("0.0") + " millimeters.");

            throw new NotImplementedException("Error! This function is not implemented yet!");
        }

        /// <summary>
        /// Stops robot. 
        /// </summary>
        public void StopRobot()
        {
            ChangeRobotStatus("Stopping robot.");
            SetMode(SpeedModes.Mode0);
            SetSpeedMilliseconds(0, 0, 0, false);
        }

        /// <summary>
        /// Moves robot in forward direction for a specified amount of time in milliseconds.
        /// </summary>
        /// <param name="time">Amount of time in time in milliseconds.</param>
        /// <param name="speed">Speed of the robot (in the range -127 to 127).</param>
        /// <exception cref="ArgumentOutOfRangeException">This exception is thrown when the input time is negative.</exception>
        public void MoveForwardMilliseconds(double time, sbyte speed)
        {
            if (time < 0) throw new ArgumentOutOfRangeException("time", @"Error! Time can't be negative!");

            ChangeRobotStatus("Moving straight forward with the speed of both wheels set to " +
                    speed + " (" + (speed * RobotSpeedToMmpS).ToString("0.0") +
                    " mm/s) for " + time.ToString("0.00") + " milliseconds.");

            SetSpeedMilliseconds(time, speed, speed, false);
        }

        /// <summary>
        /// Moves robot in backward direction for a specified amount of time in milliseconds.
        /// </summary>
        /// <param name="time">Amount of time in time in milliseconds.</param>
        /// <param name="speed">Speed of the robot (in the range -127 to 127).</param>
        /// <exception cref="ArgumentOutOfRangeException">This exception is thrown when the input time is negative.</exception>
        public void MoveBackwardMilliseconds(double time, sbyte speed)
        {
            if (time < 0) throw new ArgumentOutOfRangeException("time", @"Error! Time can't be negative!");

            ChangeRobotStatus("Moving straight backward with the speed of both wheels set to " +
                    speed + " (" + (speed * RobotSpeedToMmpS).ToString("0.0") +
                    " mm/s) for " + time.ToString("0.00") + " milliseconds.");

            SetSpeedMilliseconds(time, (sbyte)(-speed), (sbyte)(-speed), false);
        }

        /// <summary>
        /// Moves robot in forward direction with specified speed for specified distance in millimeters.
        /// </summary>
        /// <param name="distance">Distance to go in millimeters.</param>
        /// <param name="speed">Speed of the robot (in the range of -127 to 127).</param>
        /// <exception cref="ArgumentOutOfRangeException">This exception is thrown when the input distance is negative.</exception>
        public void MoveForwardMillimeters(double distance, sbyte speed)
        {
            if (distance < 0) throw new ArgumentOutOfRangeException("distance", @"Error! Distance can't be negative!");

            ChangeRobotStatus("Moving straight forward with the speed of both wheels set to " +
                    speed + " (" + (speed * RobotSpeedToMmpS).ToString("0.0") +
                    " mm/s) for " + distance.ToString("0.0") + " millimeters.");

            var timeout = distance / (RobotSpeedToMmpS * Math.Abs(speed)) * 1000F;
            SetSpeedMilliseconds(timeout, speed, speed, false);
        }

        /// <summary>
        /// Moves robot in backward direction with specified speed for specified distance in millimeters.
        /// </summary>
        /// <param name="distance">Distance to go in millimeters.</param>
        /// <param name="speed">Speed of the robot (in the range -127 to 127).</param>
        /// <exception cref="ArgumentOutOfRangeException">This exception is thrown when the input distance is negative.</exception>
        public void MoveBackwardMillimeters(double distance, sbyte speed)
        {
            if (distance < 0) throw new ArgumentOutOfRangeException("distance", @"Error! Distance can't be negative!");

            ChangeRobotStatus("Moving straight backward with the speed of both wheels set to " +
                    speed + " (" + (speed * RobotSpeedToMmpS).ToString("0.0") +
                    " mm/s) for " + distance.ToString("0.0") + " millimeters.");

            var timeout = distance / (RobotSpeedToMmpS * Math.Abs(speed)) * 1000F;
            SetSpeedMilliseconds(timeout, (sbyte)(-speed), (sbyte)(-speed), false);
        }

        /// <summary>
        /// Rotates robot in counter-clockwise direction for a specified amount of degrees.
        /// </summary>
        /// <param name="degrees">Amount of degrees to turn.</param>
        /// <param name="speed">Speed of the robot (in the range -127 to 127).</param>
        public void RotateLeftDegrees(double degrees, sbyte speed)
        {
            ChangeRobotStatus("Turning robot counter-clockwise around center of robot " +
                    degrees.ToString("0.0") + " degrees with the speed set to " + speed +
                    " (" + (speed * RobotSpeedToMmpS).ToString("0.0") + " mm/s).");

            SetSpeedDegrees(degrees, (sbyte)(-speed), speed, false);
        }

        /// <summary>
        /// Rotates robot in clockwise direction for a specified amount of degrees.
        /// </summary>
        /// <param name="degrees">Amount of degrees to turn.</param>
        /// <param name="speed">Speed of the robot (in the range -127 to 127).</param>
        public void RotateRightDegrees(double degrees, sbyte speed)
        {
            ChangeRobotStatus("Turning robot clockwise around center of robot " +
                    degrees.ToString("0.0") + " degrees with the speed set to " + speed +
                    " (" + (speed * RobotSpeedToMmpS).ToString("0.0") + " mm/s).");

            SetSpeedDegrees(degrees, speed, (sbyte)(-speed), false);
        }

        /// <summary>
        /// Rotates robot in counter-clockwise direction for a specified amount of time.
        /// </summary>
        /// <param name="time">Time to turn in milliseconds.</param>
        /// <param name="speed">Speed of the robot (in the range -127 to 127).</param>
        /// <exception cref="ArgumentOutOfRangeException">This exception is thrown when the input time is negative.</exception>
        public void RotateLeftMilliseconds(double time, sbyte speed)
        {
            if (time < 0) throw new ArgumentOutOfRangeException("time", @"Error! Time can't be negative!");

            ChangeRobotStatus("Turning robot counter-clockwise around center of robot with the speed set to " +
                    speed + " (" + (speed * RobotSpeedToMmpS).ToString("0.0") +
                    " mm/s) + for " + time.ToString("0.00") + " milliseconds.");

            SetSpeedMilliseconds(time, (sbyte)(-speed), speed, false);
        }

        /// <summary>
        /// Rotates robot in clockwise direction for a specified amount of time.
        /// </summary>
        /// <param name="time">Time to turn in milliseconds.</param>
        /// <param name="speed">Speed of the robot (in the range -127 to 127).</param>
        /// <exception cref="ArgumentOutOfRangeException">This exception is thrown when the input time is negative.</exception>
        public void RotateRightMilliseconds(double time, sbyte speed)
        {
            if (time < 0) throw new ArgumentOutOfRangeException("time", @"Error! Time can't be negative!");

            ChangeRobotStatus("Turning robot clockwise around center of robot with the speed set to " +
                    speed + " (" + (speed * RobotSpeedToMmpS).ToString("0.0") +
                    " mm/s) + for " + time.ToString("0.00") + " milliseconds.");

            SetSpeedMilliseconds(time, speed, (sbyte)(-speed), false);
        }

        /// <summary>
        /// Check health state of the robot.
        /// </summary>
        /// <returns>Returns true if there are no errors. If there is an error, it is possible to 
        /// get the error string using <see cref="ErrorDescription"/> property.</returns>
        // ReSharper disable once FunctionComplexityOverflow
        // ReSharper disable once UnusedMember.Global
        public bool CheckHealth()
        {
            bool voltsUnder16, voltsOver30, motor1Trip;
            bool motor2Trip, motor1Short, motor2Short;
            ErrorDescription = String.Empty;

            if (GetError(out voltsUnder16, out voltsOver30, out motor1Trip,
                out motor2Trip, out motor1Short, out motor2Short)) return true;

            if (voltsOver30)
                ErrorDescription = "Voltage is over 30 Volts! ";
            if (voltsUnder16)
                ErrorDescription += "Voltage is under 16 Volts! ";
            if (motor1Trip)
                ErrorDescription += "Motor 1 tripped! ";
            if (motor2Trip)
                ErrorDescription += "Motor 2 tripped! ";
            if (motor1Short)
                ErrorDescription += "Motor 1 is short-circuited! ";
            if (motor2Short)
                ErrorDescription += "Motor 2 is short-circuited! ";
            return false;
        }

        # endregion

        # region Command Execution Functions

        /// <summary>
        /// Executes a command from <see cref="Command"/> instance. Execution of commands using this function
        /// will work as expected when <see cref="PrepareForExecution"/> function is executed first.
        /// </summary>
        public void ExecuteCommand(Command cmd)
        {
            if (cmd == null) return;

            switch (cmd.Type)
            {
                // Controller commands
                case Command.Types.SetSpeedForTime:
                    SetSpeedMilliseconds(cmd.Amount, cmd.Speed1, cmd.Speed2);
                    break;
                case Command.Types.SetSpeedForDistance:
                    SetSpeedMillimeters(cmd.Amount, cmd.Speed1, cmd.Speed2);
                    break;
                case Command.Types.SetSpeedForDegrees:
                    SetSpeedDegrees(cmd.Amount, cmd.Speed1, cmd.Speed2);
                    break;
                case Command.Types.MoveForwardForTime:
                    MoveForwardMilliseconds(cmd.Amount, cmd.Speed1);
                    break;
                case Command.Types.MoveForwardForDistance:
                    MoveForwardMillimeters(cmd.Amount, cmd.Speed1);
                    break;
                case Command.Types.MoveBackwardForTime:
                    MoveBackwardMilliseconds(cmd.Amount, cmd.Speed1);
                    break;
                case Command.Types.MoveBackwardForDistance:
                    MoveBackwardMillimeters(cmd.Amount, cmd.Speed1);
                    break;
                case Command.Types.RotateLeftForTime:
                    RotateLeftMilliseconds(cmd.Amount, cmd.Speed1);
                    break;
                case Command.Types.RotateLeftForDegrees:
                    RotateLeftDegrees(cmd.Amount, cmd.Speed1);
                    break;
                case Command.Types.RotateRightForTime:
                    RotateRightMilliseconds(cmd.Amount, cmd.Speed1);
                    break;
                case Command.Types.RotateRightForDegrees:
                    RotateRightDegrees(cmd.Amount, cmd.Speed1);
                    break;
                case Command.Types.DoNothing:
                    DoNothing(cmd.Amount);
                    break;
                case Command.Types.Stop:
                    StopRobot();
                    break;

                // Simulator commands
                case Command.Types.SetX:
                    SetX(cmd.Amount1);
                    break;
                case Command.Types.SetY:
                    SetY(cmd.Amount1);
                    break;
                case Command.Types.SetAngle:
                    SetAngle(cmd.Amount);
                    break;
                case Command.Types.SetPosition:
                    SetPosition(cmd.Amount1, cmd.Amount2);
                    break;
                case Command.Types.SetPose:
                    SetPose(cmd.Amount1, cmd.Amount2, cmd.Amount);
                    break;
                case Command.Types.SetSimulationSpeed:
                    SetSimulationSpeed(cmd.Amount);
                    break;
                case Command.Types.GetSimulationSpeed:
                    ChangeRobotStatus("Current simulation speed is " + GetSimulationSpeed().ToString("##.0") + ".");
                    break;

                // MotorDriver commands

                case Command.Types.GetVolts:
                    ChangeRobotStatus("Voltage of the motor batteries is " + GetVolts() + " volts.");
                    break;

                case Command.Types.GetCurrent1:
                    ChangeRobotStatus("Current driven by the left motor is " + (GetCurrent1()/10F).ToString("##.0") +
                                      " amperes.");
                    break;

                case Command.Types.GetCurrent2:
                    ChangeRobotStatus("Current driven by the right motor is " + (GetCurrent2()/10F).ToString("##.0") +
                                      " amperes.");
                    break;

                case Command.Types.GetVI:
                    var vi = GetVI();
                    ChangeRobotStatus("Voltage of the motor batteries is " + vi[0] + " volts. " +
                                      Environment.NewLine +
                                      "Current driven by the left motor is " + (vi[1]/10F).ToString("##.0") +
                                      " amperes. " +
                                      Environment.NewLine +
                                      "Current driven by the right motor is " + (vi[2]/10F).ToString("##.0") +
                                      " amperes.");
                    break;

                case Command.Types.GetVersion:
                    ChangeRobotStatus("Version of the MD49 firmware is " + GetVersion() + ".");
                    break;

                case Command.Types.GetSpeed1:
                    ChangeRobotStatus("Speed of the motor's Speed1 register is " + GetSpeed1() + ".");
                    break;

                case Command.Types.GetSpeed2:
                    ChangeRobotStatus("Speed of the motor's Speed2 register is " + GetSpeed2() + ".");
                    break;

                case Command.Types.GetEncoder1:
                    ChangeRobotStatus("Encoder count of the left wheel is " + GetEncoder1() + ".");
                    break;

                case Command.Types.GetEncoder2:
                    ChangeRobotStatus("Encoder count of the right wheel is " + GetEncoder2() + ".");
                    break;

                case Command.Types.GetEncoders:
                    var encoders = GetEncoders();
                    ChangeRobotStatus("Encoder count of the left wheel is " + encoders[0] + "." +
                                      Environment.NewLine + "Encoder count of the right wheel is " + encoders[1] + ".");
                    break;

                case Command.Types.GetAcceleration:
                    ChangeRobotStatus("Acceleration of the motor is " + GetAcceleration() + ".");
                    break;

                case Command.Types.GetError:
                    if (CheckHealth() == false)
                        ChangeRobotStatus("The robot works without errors.");
                    else
                        ChangeRobotStatus("An error found in robot's work. " + Environment.NewLine + ErrorDescription);
                    break;

                case Command.Types.GetMode:
                    ChangeRobotStatus("Mode of the motor driver is " + GetMode() + ".");
                    break;

                case Command.Types.SetSpeed1:
                    SetSpeed1(cmd.Amount1);
                    break;
                case Command.Types.SetSpeed2:
                    SetSpeed2(cmd.Amount1);
                    break;
                case Command.Types.SetSpeeds:
                    SetSpeeds(cmd.Amount1, cmd.Amount2);
                    break;
                case Command.Types.SetAcceleration:
                    SetAcceleration(cmd.AmountByte);
                    break;
                case Command.Types.SetMode:
                    SetMode(cmd.AmountByte);
                    break;
                case Command.Types.ResetEncoders:
                    ResetEncoders();
                    break;
                case Command.Types.DisableRegulator:
                    DisableRegulator();
                    break;
                case Command.Types.EnableRegulator:
                    EnableRegulator();
                    break;
                case Command.Types.DisableTimeout:
                    DisableTimeout();
                    break;
                case Command.Types.EnableTimeout:
                    EnableTimeout();
                    break;

                default:
                    throw new NotImplementedException("Execution of " + cmd.Type + " command is not implemented!");
            }
        }

        /// <summary>
        /// Prepares robot controller for execution of commands. Execution of commands using <see cref="ExecuteCommand"/> 
        /// will work as expected when this function is executed first.
        /// </summary>
        public void PrepareForExecution()
        {
            DisableTimeout();
            SetMode(SpeedModes.Mode0);
        }

        # endregion
    }
}
