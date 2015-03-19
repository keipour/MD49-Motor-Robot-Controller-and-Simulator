#region Includes

using System;
using System.Collections.Generic;
using System.Threading;
using RobX.Communication;
using RobX.Communication.TCP;

# endregion

namespace RobX.Controller
{
    /// <summary>
    /// Class to control robot at a higher level
    /// </summary>
    public class Controller
    {
        // ------------------------------------------- Public Variables ---------------------------------------- //

        /// <summary>
        /// Low-level interface for robot control
        /// </summary>
        public Robot Robot;

        /// <summary>
        /// Type of robot: Real robot / Simulation
        /// </summary>
        public Commons.Robot.RobotType RobotType;

        public bool IsConnected = false;

        // ------------------------------------------- Private Variables --------------------------------------- //

        /// <summary>
        /// String containing the error detected in the robot at the last health checking
        /// </summary>
        private string ErrorString = "";

        /// <summary>
        /// Contains controller commands in queue for processing
        /// </summary>
        private LinkedList<Command> Commands = new LinkedList<Command>();

        private object CommandsLock = new object();

        # region Event Handlers
        /// <summary>
        /// Event handler for ReceivedData event (will be invoked after data is received)
        /// </summary>
        public event CommunicationEventHandler ReceivedData;

        /// <summary>
        /// Event handler for SentData event (will be invoked after data is sent)
        /// </summary>
        public event CommunicationEventHandler SentData;

        /// <summary>
        /// Event handler for BeforeSendingData event (will be invoked when server is ready to send data)
        /// </summary>
        public event CommunicationEventHandler BeforeSendingData;

        /// <summary>
        /// Event handler for CommunicationStatusChanged event (will be invoked after anything new happens in the communications)
        /// </summary>
        public event CommunicationStatusEventHandler CommunicationStatusChanged;

        /// <summary>
        /// Event handler for RobotStatusChanged event (will be invoked after anything new happens to the robot)
        /// </summary>
        public event CommunicationStatusEventHandler RobotStatusChanged;

        public event EventHandler ErrorOccured;

        # endregion

        // ------------------------------------------- Constructors and Events --------------------------------- //

        /// <summary>
        /// Constructor for the Controller class
        /// </summary>
        /// <param name="robotType"></param>
        public Controller(Commons.Robot.RobotType robotType)
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

        # region Controller Events

        /// <summary>
        /// Invokes the ReceivedData event when data is received from the robot (over the network)
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">Event arguments including the received data</param>
        private void RobotReceivedData(object sender, CommunicationEventArgs e)
        {
            if (ReceivedData != null)
                ReceivedData(this, e);
        }

        /// <summary>
        /// Invokes the SentData event when data is sent to the robot (over the network)
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">Event arguments including the sent data</param>
        private void RobotSentData(object sender, CommunicationEventArgs e)
        {
            if (SentData != null)
                SentData(this, e);
        }

        /// <summary>
        /// Invokes the StatusChanged event when the robot connection state is changed
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">Event arguments including the status change message</param>
        private void RobotCommunicationStatusChanged(object sender, CommunicationStatusEventArgs e)
        {
            if (CommunicationStatusChanged != null)
                CommunicationStatusChanged(this, e);
        }

        /// <summary>
        /// Invokes the BeforeSendingData event when data is ready to be sent to the robot (over the network)
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">Event arguments including the sent data</param>
        private void RobotBeforeSendingData(object sender, CommunicationEventArgs e)
        {
            if (BeforeSendingData != null)
                BeforeSendingData(this, e);
        }

        private void RobotErrorOccured(object sender, EventArgs e)
        {
            if (ErrorOccured != null)
                ErrorOccured(this, e);
            
            // Stop execution when error occures
            StopExecution();
        }

        # endregion

        // ------------------------------------------- Public Functions ---------------------------------------- //

        /// <summary>
        /// Connect to robot or simulator over the network
        /// </summary>
        /// <param name="IPAddress">IP address of the robot (or simulator)</param>
        /// <param name="Port">Port number of the robot (or simulator)</param>
        /// <returns>Returns true if successfully connected to the robot</returns>
        public bool Connect(String IPAddress, int Port)
        {
            IsConnected = Robot.Connect(IPAddress, Port);
            return IsConnected;
        }

        public void AddCommandToQueue(Command cmd)
        {
            lock (CommandsLock)
                Commands.AddLast(cmd);
        }

        public void ExecuteCommandQueue()
        {
            if (RobotStatusChanged != null)
                RobotStatusChanged(this, new CommunicationStatusEventArgs("Starting execution of command queue..."));

            while (true)
            {
                Command cmd;
                lock (CommandsLock)
                {
                    if (Commands.Count == 0) return;
                    cmd = Commands.First.Value;
                }

                if (cmd.Type == Command.Types.SetSpeedForTime)
                    SetSpeedMilliseconds(cmd.Amount, cmd.Speed1, cmd.Speed2);
                if (cmd.Type == Command.Types.SetSpeedForDistance)
                    SetSpeedMillimeters(cmd.Amount, cmd.Speed1, cmd.Speed2);
                else if (cmd.Type == Command.Types.SetSpeedForDegrees)
                    SetSpeedDegrees(cmd.Amount, cmd.Speed1, cmd.Speed2);
                else if (cmd.Type == Command.Types.MoveForwardForTime)
                    MoveForwardMilliseconds(cmd.Amount, cmd.Speed1);
                else if (cmd.Type == Command.Types.MoveForwardForDistance)
                    MoveForwardMillimeters(cmd.Amount, cmd.Speed1);
                else if (cmd.Type == Command.Types.MoveBackwardForTime)
                    MoveBackwardMilliseconds(cmd.Amount, cmd.Speed1);
                else if (cmd.Type == Command.Types.MoveBackwardForDistance)
                    MoveBackwardMillimeters(cmd.Amount, cmd.Speed1);
                else if (cmd.Type == Command.Types.RotateLeftForTime)
                    RotateLeftMilliseconds(cmd.Amount, cmd.Speed1);
                else if (cmd.Type == Command.Types.RotateLeftForDegrees)
                    RotateLeftDegrees(cmd.Amount, cmd.Speed1);
                else if (cmd.Type == Command.Types.RotateRightForTime)
                    RotateRightMilliseconds(cmd.Amount, cmd.Speed1);
                else if (cmd.Type == Command.Types.RotateRightForDegrees)
                    RotateRightDegrees(cmd.Amount, cmd.Speed1);
                else if (cmd.Type == Command.Types.Stop)
                    StopRobot();

                // Delete the first command from queue
                lock (CommandsLock)
                {
                    if (Commands.Count > 0)
                        Commands.RemoveFirst();
                }
            }
        }

        public void StopExecution()
        {
            lock (CommandsLock)
            {
                if (Commands.Count > 0)
                {
                    if (RobotStatusChanged != null)
                        RobotStatusChanged(this, new CommunicationStatusEventArgs("Stopped execution of command queue."));
                    Commands.Clear();
                }
            }
        }

        /// <summary>
        /// Prepares robot controller for execution of commands
        /// </summary>
        public void PrepareForExecution()
        {
            Robot.DisableTimeout();
            Robot.SetMode(Commons.Robot.SpeedModes.Mode_0);
        }

        /// <summary>
        /// Set robot wheel speeds (can block execution for a specified amount of time)
        /// </summary>
        /// <param name="Milliseconds">Execution blocking time (in milliseconds)</param>
        /// <param name="Wheel1_Speed">Speed of wheel 1 (in the range -127 to 127)</param>
        /// <param name="Wheel2_Speed">Speed of wheel 2 (in the range -127 to 127)</param>
        public void SetSpeedMilliseconds(double Milliseconds, sbyte Wheel1_Speed, sbyte Wheel2_Speed, bool ShowMessage = true)
        {
            if (Milliseconds < 0) throw new Exception("Error! Time can't be negative!");

            if (ShowMessage == true && RobotStatusChanged != null)
                RobotStatusChanged(this, new CommunicationStatusEventArgs(
                    "Setting the speed of the left wheel to " + Wheel1_Speed.ToString() +
                    " (" + (Wheel1_Speed * Robot.RobotSpeedToMMpS).ToString("0.0") + " mm/s) and the right wheel to " + 
                    Wheel2_Speed.ToString() + " (" + (Wheel2_Speed * Robot.RobotSpeedToMMpS).ToString("0.0") + 
                    " mm/s) for " + Milliseconds.ToString("0.00") + " milliseconds."));

            Robot.SetSpeeds((byte)(Wheel1_Speed + 128), (byte)(Wheel2_Speed + 128));

            if (Milliseconds > 0)
                Thread.Sleep(TimeSpan.FromMilliseconds(Milliseconds / Properties.Settings.Default.SimulationSpeed));
        }

        public void SetSpeedDegrees(double Degrees, sbyte Wheel1_Speed, sbyte Wheel2_Speed, bool ShowMessage = true)
        {
            if (ShowMessage == true && RobotStatusChanged != null)
                RobotStatusChanged(this, new CommunicationStatusEventArgs(
                    "Turning robot " + ((Wheel1_Speed < Wheel2_Speed)?"counter-":"") + "clockwise " + 
                    Degrees.ToString("0.0") + " degrees with the speed of the left wheel set to " + 
                    Wheel1_Speed.ToString() + " (" + (Wheel1_Speed * Robot.RobotSpeedToMMpS).ToString("0.0") + 
                    " mm/s) and the right wheel set to " +  Wheel2_Speed.ToString() + " (" + 
                    (Wheel2_Speed * Robot.RobotSpeedToMMpS).ToString("0.0") + " mm/s)."));

            double deltaV = (Wheel1_Speed - Wheel2_Speed) * Robot.RobotSpeedToMMpS / 1000F;
            double Radians = Degrees * Math.PI / 180F;
            double milliseconds = Math.Abs(2 * Robot.Radius * Radians / deltaV);

            Robot.SetSpeeds((byte)(Wheel1_Speed + 128), (byte)(Wheel2_Speed + 128));
            SetSpeedMilliseconds(milliseconds, Wheel1_Speed, Wheel2_Speed, false);
        }

        public void SetSpeedMillimeters(double Distance, sbyte Wheel1_Speed, sbyte Wheel2_Speed, bool ShowMessage = true)
        {
            if (Distance < 0) throw new Exception("Error! Distance can't be negative!");

            if (RobotStatusChanged != null)
                RobotStatusChanged(this, new CommunicationStatusEventArgs(
                    "Setting the speed of the left wheel to " + Wheel1_Speed.ToString() +
                    " (" + (Wheel1_Speed * Robot.RobotSpeedToMMpS).ToString("0.0") + " mm/s) and the right wheel to " +
                    Wheel2_Speed.ToString() + " (" + (Wheel2_Speed * Robot.RobotSpeedToMMpS).ToString("0.0") +
                    " mm/s) for " + Distance.ToString("0.0") + " millimeters."));

            throw new NotImplementedException("Error! This function is not implemented yet!");
        }

        public void StopRobot()
        {
            if (RobotStatusChanged != null)
                RobotStatusChanged(this, new CommunicationStatusEventArgs("Stopping robot."));
            Robot.SetMode(Commons.Robot.SpeedModes.Mode_0);
            SetSpeedMilliseconds(0, 0, 0, false);
        }

        public void MoveForwardMilliseconds(double Milliseconds, sbyte Speed)
        {
            if (Milliseconds < 0) throw new Exception("Error! Time can't be negative!");

            if (RobotStatusChanged != null)
                RobotStatusChanged(this, new CommunicationStatusEventArgs(
                    "Moving straight forward with the speed of both wheels set to " + 
                    Speed.ToString() + " (" + (Speed * Robot.RobotSpeedToMMpS).ToString("0.0") +
                    " mm/s) for " + Milliseconds.ToString("0.00") + " milliseconds."));

            SetSpeedMilliseconds(Milliseconds, Speed, Speed, false);
        }

        public void MoveBackwardMilliseconds(double Milliseconds, sbyte Speed)
        {
            if (Milliseconds < 0) throw new Exception("Error! Time can't be negative!");

            if (RobotStatusChanged != null)
                RobotStatusChanged(this, new CommunicationStatusEventArgs(
                    "Moving straight backward with the speed of both wheels set to " +
                    Speed.ToString() + " (" + (Speed * Robot.RobotSpeedToMMpS).ToString("0.0") +
                    " mm/s) for " + Milliseconds.ToString("0.00") + " milliseconds."));

            SetSpeedMilliseconds(Milliseconds, (sbyte)(-Speed), (sbyte)(-Speed), false);
        }

        /// <summary>
        /// Move robot in forward direction with specified speed (-128 to 127) for specified distance in millimeters
        /// </summary>
        /// <param name="Distance">Distance to go in millimeters</param>
        /// <param name="Speed">Speed of the robot (negative for backward, positive for forward move)</param>
        public void MoveForwardMillimeters(double Distance, sbyte Speed)
        {
            if (Distance < 0) throw new Exception("Error! Distance can't be negative!");

            if (RobotStatusChanged != null)
                RobotStatusChanged(this, new CommunicationStatusEventArgs(
                    "Moving straight forward with the speed of both wheels set to " +
                    Speed.ToString() + " (" + (Speed * Robot.RobotSpeedToMMpS).ToString("0.0") +
                    " mm/s) for " + Distance.ToString("0.0") + " millimeters."));

            double timeout = Distance / (Robot.RobotSpeedToMMpS * Math.Abs(Speed)) * 1000F;
            SetSpeedMilliseconds(timeout, Speed, Speed, false);
        }

        public void MoveBackwardMillimeters(double Distance, sbyte Speed)
        {
            if (Distance < 0) throw new Exception("Error! Distance can't be negative!");

            if (RobotStatusChanged != null)
                RobotStatusChanged(this, new CommunicationStatusEventArgs(
                    "Moving straight backward with the speed of both wheels set to " +
                    Speed.ToString() + " (" + (Speed * Robot.RobotSpeedToMMpS).ToString("0.0") +
                    " mm/s) for " + Distance.ToString("0.0") + " millimeters."));

            double timeout = Distance / (Robot.RobotSpeedToMMpS * Math.Abs(Speed)) * 1000F;
            SetSpeedMilliseconds(timeout, (sbyte)(-Speed), (sbyte)(-Speed), false);
        }

        public void RotateLeftDegrees(double Degrees, sbyte Speed)
        {
            if (RobotStatusChanged != null)
                RobotStatusChanged(this, new CommunicationStatusEventArgs(
                    "Turning robot counter-clockwise around center of robot " + 
                    Degrees.ToString("0.0") + " degrees with the speed set to " + Speed.ToString() +
                    " (" + (Speed * Robot.RobotSpeedToMMpS).ToString("0.0") + " mm/s)."));

            SetSpeedDegrees(Degrees, (sbyte)(-Speed), Speed, false);
        }

        public void RotateRightDegrees(double Degrees, sbyte Speed)
        {
            if (RobotStatusChanged != null)
                RobotStatusChanged(this, new CommunicationStatusEventArgs(
                    "Turning robot clockwise around center of robot " +
                    Degrees.ToString("0.0") + " degrees with the speed set to " + Speed.ToString() +
                    " (" + (Speed * Robot.RobotSpeedToMMpS).ToString("0.0") + " mm/s)."));

            SetSpeedDegrees(Degrees, Speed, (sbyte)(-Speed), false);
        }

        public void RotateLeftMilliseconds(double Milliseconds, sbyte Speed)
        {
            if (Milliseconds < 0) throw new Exception("Error! Time can't be negative!");

            if (RobotStatusChanged != null)
                RobotStatusChanged(this, new CommunicationStatusEventArgs(
                    "Turning robot counter-clockwise around center of robot with the speed set to " + 
                    Speed.ToString() + " (" + (Speed * Robot.RobotSpeedToMMpS).ToString("0.0") +
                    " mm/s) + for " + Milliseconds.ToString("0.00") + " milliseconds."));

            SetSpeedMilliseconds(Milliseconds, (sbyte)(-Speed), Speed, false);
        }

        public void RotateRightMilliseconds(double Milliseconds, sbyte Speed)
        {
            if (Milliseconds < 0) throw new Exception("Error! Time can't be negative!");

            if (RobotStatusChanged != null)
                RobotStatusChanged(this, new CommunicationStatusEventArgs(
                    "Turning robot clockwise around center of robot with the speed set to " +
                    Speed.ToString() + " (" + (Speed * Robot.RobotSpeedToMMpS).ToString("0.0") +
                    " mm/s) + for " + Milliseconds.ToString("0.00") + " milliseconds."));

            SetSpeedMilliseconds(Milliseconds, Speed, (sbyte)(-Speed), false);
        }

        public bool CheckHealth()
        {
            bool VoltsUnder16 = false, VoltsOver30 = false, Motor1Trip = false;
            bool Motor2Trip = false, Motor1Short = false, Motor2Short = false;
            ErrorString = "";

            if (Robot.GetError(out VoltsUnder16, out VoltsOver30, out Motor1Trip,
                out Motor2Trip, out Motor1Short, out Motor2Short) == false)
            {
                if (VoltsOver30)
                    ErrorString = "Voltage is over 30 Volts! ";
                if (VoltsUnder16)
                    ErrorString += "Voltage is under 16 Volts! ";
                if (Motor1Trip)
                    ErrorString += "Motor 1 tripped! ";
                if (Motor2Trip)
                    ErrorString += "Motor 2 tripped! ";
                if (Motor1Short)
                    ErrorString += "Motor 1 is short-circuited! ";
                if (Motor2Short)
                    ErrorString += "Motor 2 is short-circuited! ";
                return false;
            }
            return true;
        }

        public string GetErrorDescription()
        {
            return ErrorString;
        }

        public static int MMpsToWheelSpeed(double MillimetersPerSecond)
        {
            return (int)(MillimetersPerSecond / Robot.RobotSpeedToMMpS);
        }

        public TimeSpan CalculateDelay()
        {
            DateTime Now = DateTime.Now;
            Robot.GetVersion();
            return DateTime.Now - Now;
        }

        public void SetSimulationSpeed(ushort Speed)
        {
            Properties.Settings.Default.SimulationSpeed = Speed / 10F;
            Robot.SetSimulationSpeed(Speed);
        }
    }
}
