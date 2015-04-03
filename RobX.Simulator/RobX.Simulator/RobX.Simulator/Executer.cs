# region Includes

using System;
using System.Collections.Generic;
using System.Drawing;

# endregion

namespace RobX.Simulator
{
    /// <summary>
    /// This class executes a command for a robot and updates the robot state
    /// </summary>
    public class Executer
    {
        //------------------------------------------ Private Variables ----------------------------------//

        /// <summary>
        /// The time of the last executed command
        /// </summary>
        private DateTime LastCommandTime = DateTime.Now;

        //------------------------------------------ Public Functions -----------------------------------//

        /// <summary>
        /// Executes the first valid command in the queue of commands
        /// </summary>
        /// <param name="Commands">List of received commands</param>
        /// <param name="SendBytes">List of bytes that should be sent to the robot. 
        /// Responces to the received commands will be added to this list.</param>
        /// <param name="Robot">Robot that should execute the commands</param>
        /// <param name="Environment">Environment of the robot</param>
        /// <param name="time">Current time</param>
        /// <param name="lasttime">Time of the last executed command</param>
        /// <param name="UpdateState">Indicates whether the state of the robot should get updated</param>
        /// <returns>Boolean indicating if any new commands are executed</returns>
        public bool ExecuteNextStep(ref LinkedList<Command> Commands, ref LinkedList<byte> SendBytes,
            ref Robot Robot, ref Environment Environment, DateTime time, ref DateTime lasttime, bool UpdateState = true)
        {
            while (Commands.Count > 0 && Commands.First.Value.Timestamp <= time)
            {
                // Read first command
                var command = Commands.First.Value;

                // All valid commands start with 0x00
                if (command.Code != 0x00)
                {
                    Commands.RemoveFirst();
                    continue;
                }

                // All valid commands have at least 2 codes
                if (Commands.Count < 2)
                {
                    if (UpdateState)
                    {
                        UpdateRobotState(ref Robot, ref Environment, lasttime, time);
                        lasttime = time;
                    }
                    return false;
                }

                // Read second command
                command = Commands.First.Next.Value;

                // Check if command time is before current time
                if (command.Timestamp > time)
                {
                    if (UpdateState)
                    {
                        UpdateRobotState(ref Robot, ref Environment, lasttime, time);
                        lasttime = time;
                    }
                    return false;
                }

                // Execute command
                switch (command.Code)
                {
                    case 0x21: // Get Speed 1
                        // Use lock to avoid multithreading issues
                        lock (Simulator.SendByteLock)
                        {
                            SendBytes.AddLast(Robot.Speed1);
                        }
                        break;

                    case 0x22: // Get Speed 2
                        // Use lock to avoid multithreading issues
                        lock (Simulator.SendByteLock)
                        {
                            SendBytes.AddLast(Robot.Speed2);
                        }
                        break;

                    case 0x23: // Get Encoder 1
                        // Use lock to avoid multithreading issues
                        lock (Simulator.SendByteLock)
                        {
                            SendBytes.AddLast((byte)((Robot.Encoder1 >> 24) & 0xFF));
                            SendBytes.AddLast((byte)((Robot.Encoder1 >> 16) & 0xFF));
                            SendBytes.AddLast((byte)((Robot.Encoder1 >> 8) & 0xFF));
                            SendBytes.AddLast((byte)(Robot.Encoder1 & 0xFF));
                        }
                        break;

                    case 0x24: // Get Encoder 2
                        // Use lock to avoid multithreading issues
                        lock (Simulator.SendByteLock)
                        {
                            SendBytes.AddLast((byte)((Robot.Encoder2 >> 24) & 0xFF));
                            SendBytes.AddLast((byte)((Robot.Encoder2 >> 16) & 0xFF));
                            SendBytes.AddLast((byte)((Robot.Encoder2 >> 8) & 0xFF));
                            SendBytes.AddLast((byte)(Robot.Encoder2 & 0xFF));
                        }
                        break;

                    case 0x25: // Get Encoders
                        // Use lock to avoid multithreading issues
                        lock (Simulator.SendByteLock)
                        {
                            SendBytes.AddLast((byte)((Robot.Encoder1 >> 24) & 0xFF));
                            SendBytes.AddLast((byte)((Robot.Encoder1 >> 16) & 0xFF));
                            SendBytes.AddLast((byte)((Robot.Encoder1 >> 8) & 0xFF));
                            SendBytes.AddLast((byte)(Robot.Encoder1 & 0xFF));

                            SendBytes.AddLast((byte)((Robot.Encoder2 >> 24) & 0xFF));
                            SendBytes.AddLast((byte)((Robot.Encoder2 >> 16) & 0xFF));
                            SendBytes.AddLast((byte)((Robot.Encoder2 >> 8) & 0xFF));
                            SendBytes.AddLast((byte)(Robot.Encoder2 & 0xFF));
                        }
                        break;

                    case 0x26: // Get Volts
                        // Use lock to avoid multithreading issues
                        lock (Simulator.SendByteLock)
                        {
                            SendBytes.AddLast(Robot.Volts);
                        }
                        break;

                    case 0x27: // Get Current 1
                        // Use lock to avoid multithreading issues
                        lock (Simulator.SendByteLock)
                        {
                            SendBytes.AddLast(Robot.Current1);
                        }
                        break;

                    case 0x28: // Get Current 2
                        // Use lock to avoid multithreading issues
                        lock (Simulator.SendByteLock)
                        {
                            SendBytes.AddLast(Robot.Current2);
                        }
                        break;

                    case 0x29: // Get Version
                        // Use lock to avoid multithreading issues
                        lock (Simulator.SendByteLock)
                        {
                            SendBytes.AddLast(Robot.Version);
                        }
                        break;

                    case 0x2A: // Get Acceleration
                        // Use lock to avoid multithreading issues
                        lock (Simulator.SendByteLock)
                        {
                            SendBytes.AddLast(Robot.Acceleration);
                        }
                        break;

                    case 0x2B: // Get Mode
                        // Use lock to avoid multithreading issues
                        lock (Simulator.SendByteLock)
                        {
                            SendBytes.AddLast(Robot.Mode);
                        }
                        break;

                    case 0x2C: // Get VI
                        // Use lock to avoid multithreading issues
                        lock (Simulator.SendByteLock)
                        {
                            SendBytes.AddLast(Robot.Volts);
                            SendBytes.AddLast(Robot.Current1);
                            SendBytes.AddLast(Robot.Current2);
                        }
                        break;

                    case 0x2D: // Get Error
                        // Use lock to avoid multithreading issues
                        lock (Simulator.SendByteLock)
                        {
                            SendBytes.AddLast(Robot.Error);
                        }
                        break;

                    case 0x31: // Set Speed 1
                        if (Commands.Count < 3 || Commands.First.Next.Next.Value.Timestamp > time)
                        {
                            if (UpdateState)
                            {
                                UpdateRobotState(ref Robot, ref Environment, lasttime, time);
                                lasttime = time;
                            }
                            return false;
                        }

                        // Read third byte
                        var Speed1 = Commands.First.Next.Next.Value;
                        if (UpdateState)
                        {
                            UpdateRobotState(ref Robot, ref Environment, lasttime, Speed1.Timestamp);
                            lasttime = time;
                        }

                        var Mode1 = Robot.Mode;
                        Robot.Speed1 = Speed1.Code;

                        // Remove the additional byte
                        // Use lock to avoid multithreading issues
                        lock (Simulator.CommandsLock)
                        {
                            Commands.RemoveFirst();
                        }
                        break;

                    case 0x32: // Set Speed 2
                        if (Commands.Count < 3 || Commands.First.Next.Next.Value.Timestamp > time)
                        {
                            if (UpdateState)
                            {
                                UpdateRobotState(ref Robot, ref Environment, lasttime, time);
                                lasttime = time;
                            }
                            return false;
                        }

                        // Read third byte
                        var Speed2 = Commands.First.Next.Next.Value;
                        if (UpdateState)
                        {
                            UpdateRobotState(ref Robot, ref Environment, lasttime, Speed2.Timestamp);
                            lasttime = time;
                        }

                        var Mode2 = Robot.Mode;
                        Robot.Speed2 = Speed2.Code;

                        // Remove the additional byte
                        // Use lock to avoid multithreading issues
                        lock (Simulator.CommandsLock)
                        {
                            Commands.RemoveFirst();
                        }

                        break;

                    case 0x33: // Set Acceleration
                        if (Commands.Count < 3 || Commands.First.Next.Next.Value.Timestamp > time)
                        {
                            if (UpdateState)
                            {
                                UpdateRobotState(ref Robot, ref Environment, lasttime, time);
                                lasttime = time;
                            }
                            return false;
                        }

                        // Read third byte
                        var Acceleration = Commands.First.Next.Next.Value;
                        if (UpdateState)
                        {
                            UpdateRobotState(ref Robot, ref Environment, lasttime, Acceleration.Timestamp);
                            lasttime = time;
                        }

                        Robot.Acceleration = Acceleration.Code;

                        // Remove the additional byte
                        // Use lock to avoid multithreading issues
                        lock (Simulator.CommandsLock)
                        {
                            Commands.RemoveFirst();
                        }

                        break;

                    case 0x34: // Set Mode
                        if (Commands.Count < 3 || Commands.First.Next.Next.Value.Timestamp > time)
                        {
                            if (UpdateState)
                            {
                                UpdateRobotState(ref Robot, ref Environment, lasttime, time);
                                lasttime = time;
                            }
                            return false;
                        }

                        // Read third byte
                        var Mode = Commands.First.Next.Next.Value;
                        if (UpdateState)
                        {
                            UpdateRobotState(ref Robot, ref Environment, lasttime, Mode.Timestamp);
                            lasttime = time;
                        }

                        Robot.Mode = Mode.Code;

                        // Remove the additional byte
                        // Use lock to avoid multithreading issues
                        lock (Simulator.CommandsLock)
                        {
                            Commands.RemoveFirst();
                        }

                        break;

                    case 0x35: // Reset Encoders
                        Robot.Encoder1 = 0;
                        Robot.Encoder2 = 0;
                        break;

                    case 0x36: // Disable Regulator
                        Robot.Regulator = false;
                        break;

                    case 0x37: // Enable Regulator
                        Robot.Regulator = true;
                        break;

                    case 0x38: // Disable Timeout
                        Robot.Timeout = false;
                        break;

                    case 0x39: // Enable Timeout
                        Robot.Timeout = true;
                        break;

                    case 0x41: // Set robot x position
                        if (Commands.Count < 4 || Commands.First.Next.Next.Next.Value.Timestamp > time)
                        {
                            if (UpdateState)
                            {
                                UpdateRobotState(ref Robot, ref Environment, lasttime, time);
                                lasttime = time;
                            }
                            return false;
                        }

                        // Read third byte
                        var XPositionHigh = Commands.First.Next.Next.Value;
                        var XPositionLow = Commands.First.Next.Next.Next.Value;
                        if (UpdateState)
                        {
                            UpdateRobotState(ref Robot, ref Environment, lasttime, XPositionLow.Timestamp);
                            lasttime = time;
                        }

                        // Clear previous robot trace
                        Environment.Robot.Trace.Clear();

                        // Calculate the new robot X position
                        Environment.Robot.X = (XPositionHigh.Code << 8) + XPositionLow.Code;

                        break;

                    case 0x42: // Set robot y position
                        if (Commands.Count < 4 || Commands.First.Next.Next.Next.Value.Timestamp > time)
                        {
                            if (UpdateState)
                            {
                                UpdateRobotState(ref Robot, ref Environment, lasttime, time);
                                lasttime = time;
                            }
                            return false;
                        }

                        // Read third byte
                        var YPositionHigh = Commands.First.Next.Next.Value;
                        var YPositionLow = Commands.First.Next.Next.Next.Value;
                        if (UpdateState)
                        {
                            UpdateRobotState(ref Robot, ref Environment, lasttime, YPositionLow.Timestamp);
                            lasttime = time;
                        }

                        // Clear previous robot trace
                        Environment.Robot.Trace.Clear();

                        // Calculate the new robot Y position
                        Environment.Robot.Y = (YPositionHigh.Code << 8) + YPositionLow.Code;

                        break;

                    case 0x43: // Set robot angle (values -3600 to 3600 for -360.0 to 360.0 degrees)
                        if (Commands.Count < 4 || Commands.First.Next.Next.Next.Value.Timestamp > time)
                        {
                            if (UpdateState)
                            {
                                UpdateRobotState(ref Robot, ref Environment, lasttime, time);
                                lasttime = time;
                            }
                            return false;
                        }

                        // Read third byte
                        var AngleHigh = Commands.First.Next.Next.Value;
                        var AngleLow = Commands.First.Next.Next.Next.Value;
                        if (UpdateState)
                        {
                            UpdateRobotState(ref Robot, ref Environment, lasttime, AngleLow.Timestamp);
                            lasttime = time;
                        }

                        // Calculate the new robot Y position
                        Environment.Robot.Angle = ((AngleHigh.Code << 8) + AngleLow.Code) / 10.0F;

                        break;
                    case 0x51: // Set simulation speed (10 = 1.0x is real-time)
                        if (Commands.Count < 4 || Commands.First.Next.Next.Next.Value.Timestamp > time)
                        {
                            if (UpdateState)
                            {
                                UpdateRobotState(ref Robot, ref Environment, lasttime, time);
                                lasttime = time;
                            }
                            return false;
                        }

                        // Read third byte
                        var SimSpeedHigh = Commands.First.Next.Next.Value;
                        var SimSpeedLow = Commands.First.Next.Next.Next.Value;
                        if (UpdateState)
                        {
                            UpdateRobotState(ref Robot, ref Environment, lasttime, SimSpeedLow.Timestamp);
                            lasttime = time;
                        }

                        // Calculate the new robot Y position
                        Simulator.SimulationSpeed = ((SimSpeedHigh.Code << 8) + SimSpeedLow.Code) / 10.0F;

                        break;

                    case 0x52: // Get Simulation Speed (1 = 0.1x)
                        // Use lock to avoid multithreading issues
                        lock (Simulator.SendByteLock)
                        {
                            var simSpeed = (ushort)(Math.Round(Simulator.SimulationSpeed * 10));
                            SendBytes.AddLast((byte)(simSpeed >> 8));
                            SendBytes.AddLast((byte)(simSpeed & 0xFF));
                        }
                        break;
                }

                // Remove executed command
                // Use lock to avoid multithreading issues
                lock (Simulator.CommandsLock)
                {
                    Commands.RemoveFirst();
                }
                var lastcommandtime = Commands.First.Value.Timestamp;
                // Use lock to avoid multithreading issues
                lock (Simulator.CommandsLock)
                {
                    Commands.RemoveFirst();
                }

                ExecuteNextStep(ref Commands, ref SendBytes, ref Robot, ref Environment, lastcommandtime, ref lasttime, false);

                lasttime = lastcommandtime;

                LastCommandTime = lastcommandtime;
                return true;
            }


            if (UpdateState)
            {
                UpdateRobotState(ref Robot, ref Environment, lasttime, time);
                lasttime = time;
            }
            return false;
        }

        //------------------------------------------ Private Functions ----------------------------------//

        /// <summary>
        /// Updates robot state (x, y, angle, encoder counts)
        /// </summary>
        /// <param name="Robot">Robot variable</param>
        /// <param name="Environment">Environment variable</param>
        /// <param name="starttime">The time of the last update</param>
        /// <param name="endtime">The current time</param>
        private void UpdateRobotState(ref Robot Robot, ref Environment Environment, DateTime starttime, DateTime endtime)
        {
            // Add robot trace to traces list
            var trace = new PointF((float)Environment.Robot.X, (float)Environment.Robot.Y);
            if (Environment.Robot.Trace.Count == 0 || Environment.Robot.Trace[Environment.Robot.Trace.Count - 1].X != trace.X ||
                Environment.Robot.Trace[Environment.Robot.Trace.Count - 1].Y != trace.Y)
                Environment.Robot.Trace.Add(trace);

            // Stop if robot timeout is set to true and timeout has happened
            if ((starttime - LastCommandTime).TotalMilliseconds > Library.Commons.Robot.MotorTimeoutInMs && 
                Robot.Timeout)
            {
                if (Robot.Mode == 0 || Robot.Mode == 2)
                {
                    Robot.Speed1 = 128;
                    Robot.Speed2 = 128;
                }
                else
                {
                    Robot.Speed1 = 0;
                    Robot.Speed2 = 0;
                }
            }

            // Calculate wheel speeds in millimeters per second
            double Speed1, Speed2;
            Robot.WheelSpeeds(out Speed1, out Speed2);
            
            // Calculate elapsed simulation time
            var totaltime = (endtime - starttime).TotalMilliseconds / 1000.0F * Simulator.SimulationSpeed;

            // --------------------- Forward kinematics for differential drive robot ------------------------ //

            // Check if wheel speeds are equal
            if (Math.Abs(Speed1 - Speed2) < 0.00001) // if speed are equal
            {
                Environment.Robot.X += Speed1 * Math.Cos(Math.PI / 180 * Environment.Robot.Angle) * totaltime;
                Environment.Robot.Y += Speed1 * Math.Sin(Math.PI / 180 * Environment.Robot.Angle) * totaltime;
            }
            else // if wheel speeds are not equal
            {
                double L = 2 * Library.Commons.Robot.Radius;                                // Distance between centers of two wheels
                var R = L / 2 * (Speed1 + Speed2) / (Speed2 - Speed1);   // Radius of rotation
                var W = (Speed2 - Speed1) / L;                           // Angular velocity of rotation
                
                // Calculate center of rotation
                var ICCx = Environment.Robot.X - R * Math.Sin(Math.PI / 180 * Environment.Robot.Angle);
                var ICCy = Environment.Robot.Y + R * Math.Cos(Math.PI / 180 * Environment.Robot.Angle);

                // Calculate new x, y and angle of robot
                var WT = W * totaltime;
                var newX = Math.Cos(WT) * (Environment.Robot.X - ICCx) - Math.Sin(WT) * (Environment.Robot.Y - ICCy) + ICCx;
                var newY = Math.Sin(WT) * (Environment.Robot.X - ICCx) + Math.Cos(WT) * (Environment.Robot.Y - ICCy) + ICCy;
                var newAngle = (Environment.Robot.Angle + WT * 180 / Math.PI) % 360;

                // Replace old position values
                Environment.Robot.X = newX;
                Environment.Robot.Y = newY;
                Environment.Robot.Angle = newAngle;
            }

            // ----------------------------- Calculate Wheel Encoder Counts --------------------------------
            // Calculate wheel movement in millimeters
            var Distance1 = Speed1 * totaltime;
            var Distance2 = Speed2 * totaltime;

            // Calculate encoder counts
            var Encoder1 = (int)(Distance1 / Library.Commons.Robot.EncoderCount2mM);
            var Encoder2 = (int)(Distance2 / Library.Commons.Robot.EncoderCount2mM);

            Robot.Encoder1 = Robot.Encoder1 + Encoder1;
            Robot.Encoder2 = Robot.Encoder2 + Encoder2;
        }
    }
}
