# region Includes

using System;
using System.Collections.Generic;
using System.Drawing;

// ReSharper disable PossibleNullReferenceException

# endregion

namespace RobX.Simulator
{
    /// <summary>
    /// This class executes a command for a robot and updates the robot state.
    /// </summary>
    public class Executer
    {
        # region Private Variables

        /// <summary>
        /// The time of the last executed command.
        /// </summary>
        private DateTime _lastCommandTime = DateTime.Now;

        # endregion

        # region Public Functions

        /// <summary>
        /// Executes the first valid command in the queue of commands.
        /// </summary>
        /// <param name="commands">List of received commands.</param>
        /// <param name="sendBytes">List of bytes that should be sent to the robot. 
        /// Responces to the received commands will be added to this list.</param>
        /// <param name="robot">Robot that should execute the commands.</param>
        /// <param name="environment">Environment of the robot.</param>
        /// <param name="time">Current time.</param>
        /// <param name="lasttime">Time of the last executed command.</param>
        /// <param name="updateState">Indicates whether the state of the robot should get updated.</param>
        /// <returns>Boolean indicating if any new commands are executed.</returns>
        public bool ExecuteNextStep(ref LinkedList<Command> commands, ref LinkedList<byte> sendBytes,
            ref Robot robot, ref Environment environment, DateTime time, ref DateTime lasttime, bool updateState = true)
        {
            while (commands.Count > 0 && commands.First.Value.Timestamp <= time)
            {
                // Read first command
                var command = commands.First.Value;

                // All valid commands start with 0x00
                if (command.Code != 0x00)
                {
                    commands.RemoveFirst();
                    continue;
                }

                // All valid commands have at least 2 codes
                if (commands.Count < 2)
                {
                    if (!updateState) return false;
                    
                    UpdateRobotState(ref robot, ref environment, lasttime, time);
                    lasttime = time;
                    return false;
                }

                // Read second command
                if (commands.First.Next != null)
                {
                    command = commands.First.Next.Value;

                    // Check if command time is before current time
                    if (command.Timestamp > time)
                    {
                        if (!updateState) return false;
                        
                        UpdateRobotState(ref robot, ref environment, lasttime, time);
                        lasttime = time;
                        return false;
                    }

                    // Execute command
                    switch (command.Code)
                    {
                        case 0x21: // Get Speed 1
                            // Use lock to avoid multithreading issues
                            lock (Simulator.SendByteLock)
                            {
                                sendBytes.AddLast(robot.Speed1);
                            }
                            break;

                        case 0x22: // Get Speed 2
                            // Use lock to avoid multithreading issues
                            lock (Simulator.SendByteLock)
                            {
                                sendBytes.AddLast(robot.Speed2);
                            }
                            break;

                        case 0x23: // Get Encoder 1
                            // Use lock to avoid multithreading issues
                            lock (Simulator.SendByteLock)
                            {
                                sendBytes.AddLast((byte)((robot.Encoder1 >> 24) & 0xFF));
                                sendBytes.AddLast((byte)((robot.Encoder1 >> 16) & 0xFF));
                                sendBytes.AddLast((byte)((robot.Encoder1 >> 8) & 0xFF));
                                sendBytes.AddLast((byte)(robot.Encoder1 & 0xFF));
                            }
                            break;

                        case 0x24: // Get Encoder 2
                            // Use lock to avoid multithreading issues
                            lock (Simulator.SendByteLock)
                            {
                                sendBytes.AddLast((byte)((robot.Encoder2 >> 24) & 0xFF));
                                sendBytes.AddLast((byte)((robot.Encoder2 >> 16) & 0xFF));
                                sendBytes.AddLast((byte)((robot.Encoder2 >> 8) & 0xFF));
                                sendBytes.AddLast((byte)(robot.Encoder2 & 0xFF));
                            }
                            break;

                        case 0x25: // Get Encoders
                            // Use lock to avoid multithreading issues
                            lock (Simulator.SendByteLock)
                            {
                                sendBytes.AddLast((byte)((robot.Encoder1 >> 24) & 0xFF));
                                sendBytes.AddLast((byte)((robot.Encoder1 >> 16) & 0xFF));
                                sendBytes.AddLast((byte)((robot.Encoder1 >> 8) & 0xFF));
                                sendBytes.AddLast((byte)(robot.Encoder1 & 0xFF));

                                sendBytes.AddLast((byte)((robot.Encoder2 >> 24) & 0xFF));
                                sendBytes.AddLast((byte)((robot.Encoder2 >> 16) & 0xFF));
                                sendBytes.AddLast((byte)((robot.Encoder2 >> 8) & 0xFF));
                                sendBytes.AddLast((byte)(robot.Encoder2 & 0xFF));
                            }
                            break;

                        case 0x26: // Get Volts
                            // Use lock to avoid multithreading issues
                            lock (Simulator.SendByteLock)
                            {
                                sendBytes.AddLast(robot.Volts);
                            }
                            break;

                        case 0x27: // Get Current 1
                            // Use lock to avoid multithreading issues
                            lock (Simulator.SendByteLock)
                            {
                                sendBytes.AddLast(robot.Current1);
                            }
                            break;

                        case 0x28: // Get Current 2
                            // Use lock to avoid multithreading issues
                            lock (Simulator.SendByteLock)
                            {
                                sendBytes.AddLast(robot.Current2);
                            }
                            break;

                        case 0x29: // Get Version
                            // Use lock to avoid multithreading issues
                            lock (Simulator.SendByteLock)
                            {
                                sendBytes.AddLast(robot.Version);
                            }
                            break;

                        case 0x2A: // Get Acceleration
                            // Use lock to avoid multithreading issues
                            lock (Simulator.SendByteLock)
                            {
                                sendBytes.AddLast(robot.Acceleration);
                            }
                            break;

                        case 0x2B: // Get Mode
                            // Use lock to avoid multithreading issues
                            lock (Simulator.SendByteLock)
                            {
                                sendBytes.AddLast(robot.Mode);
                            }
                            break;

                        case 0x2C: // Get VI
                            // Use lock to avoid multithreading issues
                            lock (Simulator.SendByteLock)
                            {
                                sendBytes.AddLast(robot.Volts);
                                sendBytes.AddLast(robot.Current1);
                                sendBytes.AddLast(robot.Current2);
                            }
                            break;

                        case 0x2D: // Get Error
                            // Use lock to avoid multithreading issues
                            lock (Simulator.SendByteLock)
                            {
                                sendBytes.AddLast(robot.Error);
                            }
                            break;

                        case 0x31: // Set Speed 1
                            if (commands.Count < 3 || commands.First.Next.Next.Value.Timestamp > time)
                            {
                                if (!updateState) return false;

                                UpdateRobotState(ref robot, ref environment, lasttime, time);
                                lasttime = time;
                                return false;
                            }

                            // Read third byte
                            var speed1 = commands.First.Next.Next.Value;
                            if (updateState)
                            {
                                UpdateRobotState(ref robot, ref environment, lasttime, speed1.Timestamp);
                                lasttime = time;
                            }

                            robot.Speed1 = speed1.Code;

                            // Remove the additional byte
                            // Use lock to avoid multithreading issues
                            lock (Simulator.CommandsLock)
                            {
                                commands.RemoveFirst();
                            }
                            break;

                        case 0x32: // Set Speed 2
                            if (commands.Count < 3 || commands.First.Next.Next.Value.Timestamp > time)
                            {
                                if (!updateState) return false;
                                
                                UpdateRobotState(ref robot, ref environment, lasttime, time);
                                lasttime = time;
                                return false;
                            }

                            // Read third byte
                            var speed2 = commands.First.Next.Next.Value;
                            if (updateState)
                            {
                                UpdateRobotState(ref robot, ref environment, lasttime, speed2.Timestamp);
                                lasttime = time;
                            }

                            robot.Speed2 = speed2.Code;

                            // Remove the additional byte
                            // Use lock to avoid multithreading issues
                            lock (Simulator.CommandsLock)
                            {
                                commands.RemoveFirst();
                            }

                            break;

                        case 0x33: // Set Acceleration
                            if (commands.Count < 3 || commands.First.Next.Next.Value.Timestamp > time)
                            {
                                if (!updateState) return false;

                                UpdateRobotState(ref robot, ref environment, lasttime, time);
                                lasttime = time;
                                return false;
                            }

                            // Read third byte
                            var acceleration = commands.First.Next.Next.Value;
                            if (updateState)
                            {
                                UpdateRobotState(ref robot, ref environment, lasttime, acceleration.Timestamp);
                                lasttime = time;
                            }

                            robot.Acceleration = acceleration.Code;

                            // Remove the additional byte
                            // Use lock to avoid multithreading issues
                            lock (Simulator.CommandsLock)
                            {
                                commands.RemoveFirst();
                            }

                            break;

                        case 0x34: // Set Mode
                            if (commands.Count < 3 || commands.First.Next.Next.Value.Timestamp > time)
                            {
                                if (!updateState) return false;

                                UpdateRobotState(ref robot, ref environment, lasttime, time);
                                lasttime = time;
                                return false;
                            }

                            // Read third byte
                            var mode = commands.First.Next.Next.Value;
                            if (updateState)
                            {
                                UpdateRobotState(ref robot, ref environment, lasttime, mode.Timestamp);
                                lasttime = time;
                            }

                            robot.Mode = mode.Code;

                            // Remove the additional byte
                            // Use lock to avoid multithreading issues
                            lock (Simulator.CommandsLock)
                            {
                                commands.RemoveFirst();
                            }

                            break;

                        case 0x35: // Reset Encoders
                            robot.Encoder1 = 0;
                            robot.Encoder2 = 0;
                            break;

                        case 0x36: // Disable Regulator
                            robot.Regulator = false;
                            break;

                        case 0x37: // Enable Regulator
                            robot.Regulator = true;
                            break;

                        case 0x38: // Disable Timeout
                            robot.Timeout = false;
                            break;

                        case 0x39: // Enable Timeout
                            robot.Timeout = true;
                            break;

                        case 0x41: // Set robot x position
                            if (commands.Count < 4 || commands.First.Next.Next.Next.Value.Timestamp > time)
                            {
                                if (!updateState) return false;

                                UpdateRobotState(ref robot, ref environment, lasttime, time);
                                lasttime = time;
                                return false;
                            }

                            // Read third byte
                            var xPositionHigh = commands.First.Next.Next.Value;
                            var xPositionLow = commands.First.Next.Next.Next.Value;
                            if (updateState)
                            {
                                UpdateRobotState(ref robot, ref environment, lasttime, xPositionLow.Timestamp);
                                lasttime = time;
                            }

                            // Clear previous robot trace
                            environment.Robot.Trace.Clear();

                            // Calculate the new robot X position
                            environment.Robot.X = (xPositionHigh.Code << 8) + xPositionLow.Code;

                            break;

                        case 0x42: // Set robot y position
                            if (commands.Count < 4 || commands.First.Next.Next.Next.Value.Timestamp > time)
                            {
                                if (!updateState) return false;

                                UpdateRobotState(ref robot, ref environment, lasttime, time);
                                lasttime = time;
                                return false;
                            }

                            // Read third byte
                            var yPositionHigh = commands.First.Next.Next.Value;
                            var yPositionLow = commands.First.Next.Next.Next.Value;
                            if (updateState)
                            {
                                UpdateRobotState(ref robot, ref environment, lasttime, yPositionLow.Timestamp);
                                lasttime = time;
                            }

                            // Clear previous robot trace
                            environment.Robot.Trace.Clear();

                            // Calculate the new robot Y position
                            environment.Robot.Y = (yPositionHigh.Code << 8) + yPositionLow.Code;

                            break;

                        case 0x43: // Set robot angle (values -3600 to 3600 for -360.0 to 360.0 degrees)
                            if (commands.Count < 4 || commands.First.Next.Next.Next.Value.Timestamp > time)
                            {
                                if (!updateState) return false;

                                UpdateRobotState(ref robot, ref environment, lasttime, time);
                                lasttime = time;
                                return false;
                            }

                            // Read third byte
                            var angleHigh = commands.First.Next.Next.Value;
                            var angleLow = commands.First.Next.Next.Next.Value;
                            if (updateState)
                            {
                                UpdateRobotState(ref robot, ref environment, lasttime, angleLow.Timestamp);
                                lasttime = time;
                            }

                            // Calculate the new robot Y position
                            environment.Robot.Angle = ((angleHigh.Code << 8) + angleLow.Code) / 10.0F;

                            break;
                        case 0x51: // Set simulation speed (10 = 1.0x is real-time)
                            if (commands.Count < 4 || commands.First.Next.Next.Next.Value.Timestamp > time)
                            {
                                if (!updateState) return false;

                                UpdateRobotState(ref robot, ref environment, lasttime, time);
                                lasttime = time;
                                return false;
                            }

                            // Read third byte
                            var simSpeedHigh = commands.First.Next.Next.Value;
                            var simSpeedLow = commands.First.Next.Next.Next.Value;
                            if (updateState)
                            {
                                UpdateRobotState(ref robot, ref environment, lasttime, simSpeedLow.Timestamp);
                                lasttime = time;
                            }

                            // Calculate the new robot Y position
                            Simulator.SimulationSpeed = ((simSpeedHigh.Code << 8) + simSpeedLow.Code) / 10.0F;

                            break;

                        case 0x52: // Get Simulation Speed (1 = 0.1x)
                            // Use lock to avoid multithreading issues
                            lock (Simulator.SendByteLock)
                            {
                                var simSpeed = (ushort)(Math.Round(Simulator.SimulationSpeed * 10));
                                sendBytes.AddLast((byte)(simSpeed >> 8));
                                sendBytes.AddLast((byte)(simSpeed & 0xFF));
                            }
                            break;
                    }
                }

                // Remove executed command
                // Use lock to avoid multithreading issues
                lock (Simulator.CommandsLock)
                {
                    commands.RemoveFirst();
                }
                var lastcommandtime = commands.First.Value.Timestamp;
                // Use lock to avoid multithreading issues
                lock (Simulator.CommandsLock)
                {
                    commands.RemoveFirst();
                }

                ExecuteNextStep(ref commands, ref sendBytes, ref robot, ref environment, lastcommandtime, ref lasttime, false);

                lasttime = lastcommandtime;

                _lastCommandTime = lastcommandtime;
                return true;
            }


            if (!updateState) return false;

            UpdateRobotState(ref robot, ref environment, lasttime, time);
            lasttime = time;
            return false;
        }

        # endregion

        # region Private Functions

        /// <summary>
        /// Updates robot state (x, y, angle, encoder counts).
        /// </summary>
        /// <param name="robot">Robot variable.</param>
        /// <param name="environment">Environment variable.</param>
        /// <param name="starttime">The time of the last update.</param>
        /// <param name="endtime">The current time.</param>
        private void UpdateRobotState(ref Robot robot, ref Environment environment, DateTime starttime, DateTime endtime)
        {
            // Add robot trace to traces list
            var trace = new PointF((float)environment.Robot.X, (float)environment.Robot.Y);
            if (environment.Robot.Trace.Count == 0 || Math.Abs(environment.Robot.Trace[environment.Robot.Trace.Count - 1].X - trace.X) > 0.1F ||
                Math.Abs(environment.Robot.Trace[environment.Robot.Trace.Count - 1].Y - trace.Y) > 0.1)
                environment.Robot.Trace.Add(trace);

            // Stop if robot timeout is set to true and timeout has happened
            if ((starttime - _lastCommandTime).TotalMilliseconds > Library.Robot.Robot.MotorTimeoutInMs && 
                robot.Timeout)
            {
                if (robot.Mode == 0 || robot.Mode == 2)
                {
                    robot.Speed1 = 128;
                    robot.Speed2 = 128;
                }
                else
                {
                    robot.Speed1 = 0;
                    robot.Speed2 = 0;
                }
            }

            // Calculate wheel speeds in millimeters per second
            double speed1, speed2;
            robot.MotorSpeedToRealSpeed(out speed1, out speed2);
            
            // Calculate elapsed simulation time
            var totaltime = (endtime - starttime).TotalMilliseconds / 1000.0F * Simulator.SimulationSpeed;

            // Forward kinematics for differential drive robot
            Library.Robot.Robot.ForwardKinematics(totaltime, speed1, speed2, ref environment.Robot.X, 
                ref environment.Robot.Y, ref environment.Robot.Angle);

            // Calculate wheel encoder counts
            robot.Encoder1 = robot.Encoder1 + Library.Robot.Robot.DistanceToEncoderCount(speed1 * totaltime);
            robot.Encoder2 = robot.Encoder2 + Library.Robot.Robot.DistanceToEncoderCount(speed2 * totaltime);
        }

        # endregion
   }
}
