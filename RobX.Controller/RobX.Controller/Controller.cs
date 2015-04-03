#region Includes

using System;
using System.Collections.Generic;
using System.Threading;
using RobX.Controller.Properties;
using RobX.Library.Communication;

# endregion

namespace RobX.Controller
{
    /// <summary>
    /// Class to control robot at high level.
    /// </summary>
    public class Controller
    {
        # region Public Fields

        /// <summary>
        /// Low-level interface for robot control.
        /// </summary>
        public Robot Robot;

        /// <summary>
        /// Type of robot: Real robot / Simulation.
        /// </summary>
        public Library.Commons.Robot.RobotType RobotType;

        /// <summary>
        /// Indicates whether the controller is connected to the robot (via network) or not.
        /// </summary>
        public bool IsConnected;

        # endregion

        # region Private Fields

        /// <summary>
        /// String containing the error detected in the robot at the last health checking.
        /// </summary>
        private string _errorString = "";

        /// <summary>
        /// Contains controller commands in queue for processing.
        /// </summary>
        private readonly LinkedList<Command> _commands = new LinkedList<Command>();

        private readonly object _commandsLock = new object();

        # endregion

        # region Events

        /// <summary>
        /// ReceivedData event (will be invoked after data is received).
        /// </summary>
        public event CommunicationEventHandler ReceivedData;

        /// <summary>
        /// SentData event (will be invoked after data is sent).
        /// </summary>
        public event CommunicationEventHandler SentData;

        /// <summary>
        /// BeforeSendingData event (will be invoked when server is ready to send data).
        /// </summary>
        public event CommunicationEventHandler BeforeSendingData;

        /// <summary>
        /// CommunicationStatusChanged event (will be invoked after anything new happens in the communications).
        /// </summary>
        public event CommunicationStatusEventHandler CommunicationStatusChanged;

        /// <summary>
        /// RobotStatusChanged event (will be invoked after anything new happens to the robot).
        /// </summary>
        public event CommunicationStatusEventHandler RobotStatusChanged;

        /// <summary>
        /// Event that is invoked when an error is occured.
        /// </summary>
        public event EventHandler ErrorOccured;

        # endregion

        # region Constructor

        /// <summary>
        /// Constructor for the Controller class.
        /// </summary>
        /// <param name="robotType"></param>
        public Controller(Library.Commons.Robot.RobotType robotType)
        {
            RobotType = robotType;
            Robot = new Robot(robotType);

            // Add event handlers
            Robot.ReceivedData += RobotReceivedData;
            Robot.SentData += RobotSentData;
            Robot.StatusChanged += RobotCommunicationStatusChanged;
            Robot.BeforeSendingData += RobotBeforeSendingData;
            Robot.ErrorOccured += RobotErrorOccured;
        }

        # endregion

        # region Controller Event Handlers

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
        private void RobotCommunicationStatusChanged(object sender, CommunicationStatusEventArgs e)
        {
            if (CommunicationStatusChanged != null)
                CommunicationStatusChanged(this, e);
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

        /// <summary>
        /// Invokes the ErrorOccured event when any error occures in the robot or connection.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">Event argument.</param>
        private void RobotErrorOccured(object sender, EventArgs e)
        {
            if (ErrorOccured != null)
                ErrorOccured(this, e);

            // Stop execution when error occures
            StopExecution();
        }

        # endregion

        # region Public Functions

        /// <summary>
        /// Connect to robot or simulator over the network.
        /// </summary>
        /// <param name="ipAddress">IP address of the robot (or simulator).</param>
        /// <param name="port">Port number of the robot (or simulator).</param>
        /// <returns>Returns true if successfully connected to the robot.</returns>
        public bool Connect(String ipAddress, int port)
        {
            IsConnected = Robot.Connect(ipAddress, port);
            return IsConnected;
        }

        /// <summary>
        /// Adds a command to the command processing queue.
        /// </summary>
        /// <param name="cmd">Command to be added.</param>
        public void AddCommandToQueue(Command cmd)
        {
            lock (_commandsLock)
                _commands.AddLast(cmd);
        }

        /// <summary>
        /// Executes sequentially all the high-level commands in the queue.
        /// </summary>
        public void ExecuteCommandQueue()
        {
            if (RobotStatusChanged != null)
                RobotStatusChanged(this, new CommunicationStatusEventArgs("Starting execution of command queue..."));

            while (true)
            {
                Command cmd;
                lock (_commandsLock)
                {
                    if (_commands.Count == 0) return;
                    cmd = _commands.First.Value;
                }

                switch (cmd.Type)
                {
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
                    case Command.Types.Stop:
                        StopRobot();
                        break;
                }

                // Delete the first command from queue
                lock (_commandsLock)
                {
                    if (_commands.Count > 0)
                        _commands.RemoveFirst();
                }
            }
        }

        /// <summary>
        /// Stops execution of commands in the processing queue immediately. Clears the processing queue.
        /// </summary>
        public void StopExecution()
        {
            lock (_commandsLock)
            {
                if (_commands.Count <= 0) return;

                if (RobotStatusChanged != null)
                    RobotStatusChanged(this, new CommunicationStatusEventArgs("Stopped execution of command queue."));
                _commands.Clear();
            }
        }

        /// <summary>
        /// Prepares robot controller for execution of commands.
        /// </summary>
        public void PrepareForExecution()
        {
            Robot.DisableTimeout();
            Robot.SetMode(Library.Commons.Robot.SpeedModes.Mode0);
        }

        /// <summary>
        /// Sets robot wheel speeds for a specified amount of time.
        /// </summary>
        /// <param name="milliseconds">Execution time in milliseconds.</param>
        /// <param name="wheel1Speed">Speed of wheel 1 (in the range -127 to 127).</param>
        /// <param name="wheel2Speed">Speed of wheel 2 (in the range -127 to 127).</param>
        /// <param name="showMessage">If true, invoke RobotStatusChanged event with a message.</param>
        public void SetSpeedMilliseconds(double milliseconds, sbyte wheel1Speed, sbyte wheel2Speed,
            bool showMessage = true)
        {
            if (milliseconds < 0) throw new Exception("Error! Time can't be negative!");

            if (showMessage && RobotStatusChanged != null)
                RobotStatusChanged(this, new CommunicationStatusEventArgs(
                    "Setting the speed of the left wheel to " + wheel1Speed +
                    " (" + (wheel1Speed*Robot.RobotSpeedToMmpS).ToString("0.0") + " mm/s) and the right wheel to " +
                    wheel2Speed + " (" + (wheel2Speed*Robot.RobotSpeedToMmpS).ToString("0.0") +
                    " mm/s) for " + milliseconds.ToString("0.00") + " milliseconds."));

            Robot.SetSpeeds((byte) (wheel1Speed + 128), (byte) (wheel2Speed + 128));

            if (milliseconds > 0)
                Thread.Sleep(TimeSpan.FromMilliseconds(milliseconds/Settings.Default.SimulationSpeed));
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
            if (showMessage && RobotStatusChanged != null)
                RobotStatusChanged(this, new CommunicationStatusEventArgs(
                    "Turning robot " + ((wheel1Speed < wheel2Speed) ? "counter-" : "") + "clockwise " +
                    degrees.ToString("0.0") + " degrees with the speed of the left wheel set to " +
                    wheel1Speed + " (" + (wheel1Speed*Robot.RobotSpeedToMmpS).ToString("0.0") +
                    " mm/s) and the right wheel set to " + wheel2Speed + " (" +
                    (wheel2Speed*Robot.RobotSpeedToMmpS).ToString("0.0") + " mm/s)."));

            var deltaV = (wheel1Speed - wheel2Speed)*Robot.RobotSpeedToMmpS/1000F;
            var radians = degrees*Math.PI/180F;
            var milliseconds = Math.Abs(2*Robot.Radius*radians/deltaV);

            Robot.SetSpeeds((byte) (wheel1Speed + 128), (byte) (wheel2Speed + 128));
            SetSpeedMilliseconds(milliseconds, wheel1Speed, wheel2Speed, false);
        }

        /// <summary>
        /// Sets robot wheel speeds and turns for a specified distance.
        /// </summary>
        /// <param name="distance">Distance in millimeters.</param>
        /// <param name="wheel1Speed">Speed of wheel 1 (in the range -127 to 127).</param>
        /// <param name="wheel2Speed">Speed of wheel 2 (in the range -127 to 127).</param>
        /// <param name="showMessage">If true, invoke RobotStatusChanged event with a message.</param>
        public void SetSpeedMillimeters(double distance, sbyte wheel1Speed, sbyte wheel2Speed, bool showMessage = true)
        {
            if (distance < 0) throw new Exception("Error! Distance can't be negative!");

            if (RobotStatusChanged != null)
                RobotStatusChanged(this, new CommunicationStatusEventArgs(
                    "Setting the speed of the left wheel to " + wheel1Speed +
                    " (" + (wheel1Speed*Robot.RobotSpeedToMmpS).ToString("0.0") + " mm/s) and the right wheel to " +
                    wheel2Speed + " (" + (wheel2Speed*Robot.RobotSpeedToMmpS).ToString("0.0") +
                    " mm/s) for " + distance.ToString("0.0") + " millimeters."));

            throw new NotImplementedException("Error! This function is not implemented yet!");
        }

        /// <summary>
        /// Stops robot. 
        /// </summary>
        public void StopRobot()
        {
            if (RobotStatusChanged != null)
                RobotStatusChanged(this, new CommunicationStatusEventArgs("Stopping robot."));
            Robot.SetMode(Library.Commons.Robot.SpeedModes.Mode0);
            SetSpeedMilliseconds(0, 0, 0, false);
        }

        /// <summary>
        /// Moves robot in forward direction for a specified amount of time in milliseconds.
        /// </summary>
        /// <param name="milliseconds">Amount of time in milliseconds.</param>
        /// <param name="speed">Speed of the robot (in the range -127 to 127).</param>
        public void MoveForwardMilliseconds(double milliseconds, sbyte speed)
        {
            if (milliseconds < 0) throw new Exception("Error! Time can't be negative!");

            if (RobotStatusChanged != null)
                RobotStatusChanged(this, new CommunicationStatusEventArgs(
                    "Moving straight forward with the speed of both wheels set to " +
                    speed + " (" + (speed*Robot.RobotSpeedToMmpS).ToString("0.0") +
                    " mm/s) for " + milliseconds.ToString("0.00") + " milliseconds."));

            SetSpeedMilliseconds(milliseconds, speed, speed, false);
        }

        /// <summary>
        /// Moves robot in backward direction for a specified amount of time in milliseconds.
        /// </summary>
        /// <param name="milliseconds">Amount of time in milliseconds.</param>
        /// <param name="speed">Speed of the robot (in the range -127 to 127).</param>
        public void MoveBackwardMilliseconds(double milliseconds, sbyte speed)
        {
            if (milliseconds < 0) throw new Exception("Error! Time can't be negative!");

            if (RobotStatusChanged != null)
                RobotStatusChanged(this, new CommunicationStatusEventArgs(
                    "Moving straight backward with the speed of both wheels set to " +
                    speed + " (" + (speed*Robot.RobotSpeedToMmpS).ToString("0.0") +
                    " mm/s) for " + milliseconds.ToString("0.00") + " milliseconds."));

            SetSpeedMilliseconds(milliseconds, (sbyte) (-speed), (sbyte) (-speed), false);
        }

        /// <summary>
        /// Moves robot in forward direction with specified speed for specified distance in millimeters.
        /// </summary>
        /// <param name="distance">Distance to go in millimeters.</param>
        /// <param name="speed">Speed of the robot (in the range of -127 to 127).</param>
        public void MoveForwardMillimeters(double distance, sbyte speed)
        {
            if (distance < 0) throw new Exception("Error! Distance can't be negative!");

            if (RobotStatusChanged != null)
                RobotStatusChanged(this, new CommunicationStatusEventArgs(
                    "Moving straight forward with the speed of both wheels set to " +
                    speed + " (" + (speed*Robot.RobotSpeedToMmpS).ToString("0.0") +
                    " mm/s) for " + distance.ToString("0.0") + " millimeters."));

            var timeout = distance/(Robot.RobotSpeedToMmpS*Math.Abs(speed))*1000F;
            SetSpeedMilliseconds(timeout, speed, speed, false);
        }

        /// <summary>
        /// Moves robot in backward direction with specified speed for specified distance in millimeters.
        /// </summary>
        /// <param name="distance">Distance to go in millimeters.</param>
        /// <param name="speed">Speed of the robot (in the range -127 to 127).</param>
        public void MoveBackwardMillimeters(double distance, sbyte speed)
        {
            if (distance < 0) throw new Exception("Error! Distance can't be negative!");

            if (RobotStatusChanged != null)
                RobotStatusChanged(this, new CommunicationStatusEventArgs(
                    "Moving straight backward with the speed of both wheels set to " +
                    speed + " (" + (speed*Robot.RobotSpeedToMmpS).ToString("0.0") +
                    " mm/s) for " + distance.ToString("0.0") + " millimeters."));

            var timeout = distance/(Robot.RobotSpeedToMmpS*Math.Abs(speed))*1000F;
            SetSpeedMilliseconds(timeout, (sbyte) (-speed), (sbyte) (-speed), false);
        }

        /// <summary>
        /// Rotates robot in counter-clockwise direction for a specified degrees.
        /// </summary>
        /// <param name="degrees">Amount of degrees to turn.</param>
        /// <param name="speed">Speed of the robot (in the range -127 to 127).</param>
        public void RotateLeftDegrees(double degrees, sbyte speed)
        {
            if (RobotStatusChanged != null)
                RobotStatusChanged(this, new CommunicationStatusEventArgs(
                    "Turning robot counter-clockwise around center of robot " +
                    degrees.ToString("0.0") + " degrees with the speed set to " + speed +
                    " (" + (speed*Robot.RobotSpeedToMmpS).ToString("0.0") + " mm/s)."));

            SetSpeedDegrees(degrees, (sbyte) (-speed), speed, false);
        }

        /// <summary>
        /// Rotates robot in clockwise direction for a specified degrees.
        /// </summary>
        /// <param name="degrees">Amount of degrees to turn.</param>
        /// <param name="speed">Speed of the robot (in the range -127 to 127).</param>
        public void RotateRightDegrees(double degrees, sbyte speed)
        {
            if (RobotStatusChanged != null)
                RobotStatusChanged(this, new CommunicationStatusEventArgs(
                    "Turning robot clockwise around center of robot " +
                    degrees.ToString("0.0") + " degrees with the speed set to " + speed +
                    " (" + (speed*Robot.RobotSpeedToMmpS).ToString("0.0") + " mm/s)."));

            SetSpeedDegrees(degrees, speed, (sbyte) (-speed), false);
        }

        /// <summary>
        /// Rotates robot in counter-clockwise direction for a specified distance.
        /// </summary>
        /// <param name="milliseconds">Distance to turn in millimeters.</param>
        /// <param name="speed">Speed of the robot (in the range -127 to 127).</param>
        public void RotateLeftMilliseconds(double milliseconds, sbyte speed)
        {
            if (milliseconds < 0) throw new Exception("Error! Time can't be negative!");

            if (RobotStatusChanged != null)
                RobotStatusChanged(this, new CommunicationStatusEventArgs(
                    "Turning robot counter-clockwise around center of robot with the speed set to " +
                    speed + " (" + (speed*Robot.RobotSpeedToMmpS).ToString("0.0") +
                    " mm/s) + for " + milliseconds.ToString("0.00") + " milliseconds."));

            SetSpeedMilliseconds(milliseconds, (sbyte) (-speed), speed, false);
        }

        /// <summary>
        /// Rotates robot in clockwise direction for a specified distance.
        /// </summary>
        /// <param name="milliseconds">Distance to turn in millimeters.</param>
        /// <param name="speed">Speed of the robot (in the range -127 to 127).</param>
        public void RotateRightMilliseconds(double milliseconds, sbyte speed)
        {
            if (milliseconds < 0) throw new Exception("Error! Time can't be negative!");

            if (RobotStatusChanged != null)
                RobotStatusChanged(this, new CommunicationStatusEventArgs(
                    "Turning robot clockwise around center of robot with the speed set to " +
                    speed + " (" + (speed*Robot.RobotSpeedToMmpS).ToString("0.0") +
                    " mm/s) + for " + milliseconds.ToString("0.00") + " milliseconds."));

            SetSpeedMilliseconds(milliseconds, speed, (sbyte) (-speed), false);
        }

        /// <summary>
        /// Check health state of the robot.
        /// </summary>
        /// <returns>Returns true if there are no errors. If there is an error, it is possible to 
        /// get the error string using <see cref="GetErrorDescription"/> function.</returns>
        // ReSharper disable once FunctionComplexityOverflow
        public bool CheckHealth()
        {
            bool voltsUnder16, voltsOver30, motor1Trip;
            bool motor2Trip, motor1Short, motor2Short;
            _errorString = "";

            if (Robot.GetError(out voltsUnder16, out voltsOver30, out motor1Trip,
                out motor2Trip, out motor1Short, out motor2Short)) return true;

            if (voltsOver30)
                _errorString = "Voltage is over 30 Volts! ";
            if (voltsUnder16)
                _errorString += "Voltage is under 16 Volts! ";
            if (motor1Trip)
                _errorString += "Motor 1 tripped! ";
            if (motor2Trip)
                _errorString += "Motor 2 tripped! ";
            if (motor1Short)
                _errorString += "Motor 1 is short-circuited! ";
            if (motor2Short)
                _errorString += "Motor 2 is short-circuited! ";
            return false;
        }

        /// <summary>
        /// Returns a string containing the errors occured in the recent <see cref="CheckHealth"/> function call.
        /// </summary>
        /// <returns>The string describing the error occured.</returns>
        public string GetErrorDescription()
        {
            return _errorString;
        }

        /// <summary>
        /// Converts real speed (millimeters per second) to robot wheel speed.
        /// </summary>
        /// <param name="millimetersPerSecond">Real speed in millimeters per second.</param>
        /// <returns>Robot wheel speed (in range -127 to 127).</returns>
        public static int MMpsToWheelSpeed(double millimetersPerSecond)
        {
            return (int) (millimetersPerSecond/Robot.RobotSpeedToMmpS);
        }

        /// <summary>
        /// Estimates the delay of communication with robot by sending a single command and calculating the responce time.
        /// </summary>
        /// <returns>Estimated delay of communication with robot in milliseconds.</returns>
        public TimeSpan CalculateDelay()
        {
            var now = DateTime.Now;
            Robot.GetVersion();
            return DateTime.Now - now;
        }

        /// <summary>
        /// Sets the simulator's simulation speed. Works only when connected to simulator.
        /// </summary>
        /// <param name="speed">Simulation speed (Real-time simulation speed = 1.0).</param>
        public void SetSimulationSpeed(ushort speed)
        {
            Settings.Default.SimulationSpeed = speed/10F;
            Robot.SetSimulationSpeed(speed);
        }

        # endregion
    }
}