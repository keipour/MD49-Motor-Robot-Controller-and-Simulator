﻿using System;

// ReSharper disable UnusedMember.Global

namespace RobX.Library.Robot
{
    /// <summary>
    /// Class that contains a command.
    /// </summary>
    public class Command
    {
        # region Public Enums

        /// <summary>
        /// Enum that specifies the type of a command.
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
            Stop,

            /// <summary>
            /// Robot will keep its current state for the specified amount of time.
            /// </summary>
            DoNothing,

            /// <summary>
            /// Set x position of the robot in the environment in millimeters (works only in simulation mode).
            /// </summary>
            SetX,

            /// <summary>
            /// Set y position of the robot in the environment in millimeters (works only in simulation mode).
            /// </summary>
            SetY,

            /// <summary>
            /// Set angle of the robot in the environment in degrees (works only in simulation mode).
            /// </summary>
            SetAngle,

            /// <summary>
            /// Set x and y positions of the robot in the environment in millimeters (works only in simulation mode).
            /// </summary>
            SetPosition,

            /// <summary>
            /// Set angle (in degrees), x and y positions (in millimeters) of the robot in the environment 
            /// (works only in simulation mode).
            /// </summary>
            SetPose,

            /// <summary>
            /// Get simulation speed of simulator (works only in simulation mode).
            /// </summary>
            GetSimulationSpeed,

            /// <summary>
            /// Set simulation speed on simulator (works only in simulation mode).
            /// </summary>
            SetSimulationSpeed
        }

        /// <summary>
        /// Enum that specifies the level of the command (low-level, high-level, simulator, etc).
        /// </summary>
        public enum Levels
        {
            /// <summary>
            /// The command is a low-level command that motor driver can understand directly.
            /// </summary>
            MotorDriver,

            /// <summary>
            /// The command is a high-level command that will be converted to other low-level commands
            /// for motor driver to understand.
            /// </summary>
            Controller,

            /// <summary>
            /// The command is not a valid robot command; it only works for simulator.
            /// </summary>
            Simulator
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
        /// <para>5. For other commands it has no effect.</para>
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
        /// <para>1. For timed controller commands it is interpreted as the time in milliseconds.</para>
        /// <para>2. For distanced controller commands it is interpreted as the distance in millimeters.</para>
        /// <para>3. For degree controller commands, SetAngle and SetPose it is interpreted as the angle in degrees.</para>
        /// <para>4. For SetSimulationSpeed command it is interpreted as the simulation speed amount.</para>
        /// <para>5. For other commands it has no effect.</para>
        /// </summary>
        public readonly double Amount;

        /// <summary>
        /// <para>Works as follows:</para>
        /// <para>1. For SetX, SetPosition and SetPose commands it is interpreted as the X position in millimeters.</para>
        /// <para>2. For SetY command it is interpreted as the Y position in millimeters.</para>
        /// <para>3. For other commands it has no effect.</para>
        /// </summary>
        public readonly short Amount1;

        /// <summary>
        /// <para>Works as follows:</para>
        /// <para>1. For SetPosition and SetPose commands it is interpreted as the Y position in millimeters.</para>
        /// <para>2. For other commands it has no effect.</para>
        /// </summary>
        public readonly short Amount2;

        /// <summary>
        /// The level (low, high, simulator-leels, etc.) of the current command.
        /// </summary>
        public Levels Level
        {
            get { return GetCommandLevel(Type); }
        }

        /// <summary>
        /// The description of parameters of the current command.
        /// </summary>
        public string[] ParameterDescriptions
        {
            get { return GetParameterDescriptions(Type); }
        }

        /// <summary>
        /// <para>The types of the parameters for the current command.</para>
        /// <para>D stands for <see cref="double"/> type.</para>
        /// <para>S stands for <see cref="sbyte"/> type.</para>
        /// <para>B stands for <see cref="byte"/> type.</para>
        /// <para>I stands for <see cref="int"/> type.</para>
        /// </summary>
        public string ParameterTypes
        {
            get { return GetParameterTypes(Type); }
        }

        /// <summary>
        /// The names of the parameters for the current command.
        /// </summary>
        public string[] ParameterNames
        {
            get { return GetParameterNames(Type); }
        }

        # endregion

        # region Constructor

        /// <summary>
        /// Constructor for Command class.
        /// </summary>
        /// <param name="type">Type of controller command.</param>
        /// <param name="param1">
        /// <para>1. For timed commands it is interpreted as the time in milliseconds (non-negative 
        /// <see cref="double"/>).</para>
        /// <para>2. For distanced commands it is interpreted as the distance in millimeters (non-negative <see cref="double"/>).</para>
        /// <para>3. For controller degree commands and SetAngle command it is interpreted as the amount of degrees (<see cref="double"/>).</para>
        /// <para>4. For SetSimulationSpeed command it is interpreted as the simulation speed amount (positive <see cref="double"/>).</para>
        /// <para>5. For SetX, SetPosition and SetPose commands it is interpreted as the X position in millimeters (<see cref="short"/>).</para>
        /// <para>6. For SetY command it is interpreted as the Y position in millimeters (<see cref="short"/>).</para>
        /// <para>7. For other commands it is null.</para>
        /// </param>
        /// <param name="param2">
        /// <para>Works as follows:</para>
        /// <para>1. For forward and backward movement is the speed of both wheels in the forward/backward 
        /// direction (<see cref="sbyte"/> with range: -127 to +127).</para>
        /// <para>2. For SetSpeed commands is the speed of the left wheel (<see cref="sbyte"/> with range: -127 to +127).</para>
        /// <para>3. For right (clockwise) rotation the speed of left wheel will be param2 and the speed 
        /// of right wheel will be -param2 (<see cref="sbyte"/> with range: -127 to +127).</para>
        /// <para>4. For left (counter-clockwise) rotation the speed of left wheel will be -param2 and 
        /// the speed of right wheel will be param2 (<see cref="sbyte"/> with range: -127 to +127).</para>
        /// <para>5. For SetPosition and SetPose commands it is interpreted as the Y position in millimeters (<see cref="short"/>).</para>
        /// <para>6. For other commands it is null.</para>
        /// </param>
        /// <param name="param3">
        /// <para>Works as follows:</para>
        /// <para>1. For SetSpeed commands is the speed of the right wheel (<see cref="sbyte"/> with range: -127 to +127).</para>
        /// <para>2. For SetPose command is the angle in degrees (<see cref="double"/>).</para>
        /// <para>3. For other commands it is null.</para>
        /// </param>
        public Command(Types type, object param1 = null, object param2 = null, object param3 = null)
        {
            Type = type;
            switch (type)
            {
                // Controller commands
                case Types.SetSpeedForTime:
                case Types.SetSpeedForDistance:
                case Types.SetSpeedForDegrees:
                case Types.MoveForwardForTime:
                case Types.MoveForwardForDistance:
                case Types.MoveBackwardForTime:
                case Types.MoveBackwardForDistance:
                case Types.RotateLeftForTime:
                case Types.RotateLeftForDegrees:
                case Types.RotateRightForTime:
                case Types.RotateRightForDegrees:
                case Types.DoNothing:
                case Types.Stop:
                case Types.SetAngle:
                case Types.SetSimulationSpeed:
                case Types.GetSimulationSpeed:
                    if (param1 != null) Amount = (double)param1;
                    if (param2 != null) Speed1 = (sbyte) param2;
                    if (param3 != null) Speed2 = (sbyte) param3;
                    break;

                // Simulator commands
                case Types.SetX:
                case Types.SetY:
                case Types.SetPosition:
                case Types.SetPose:
                    if (param1 != null) Amount1 = (short)param1;
                    if (param2 != null) Amount2 = (short)param2;
                    if (param3 != null) Amount = (double) param3;
                    break;

                default:
                    throw new NotImplementedException("The constructor for " + type + " command is not implemented!");
            }
        }

        # endregion

        # region Static Functions

        /// <summary>
        /// Get types of parameters for a given command.
        /// </summary>
        /// <param name="type">The input command type.</param>
        /// <returns><para>Returns the parameter types of the given command.</para>
        /// <para>D stands for <see cref="double"/> type.</para>
        /// <para>S stands for <see cref="sbyte"/> type.</para>
        /// <para>B stands for <see cref="byte"/> type.</para>
        /// <para>I stands for <see cref="int"/> type.</para>
        /// <para>U stands for <see cref="uint"/> type.</para>
        /// <para>s stands for <see cref="short"/> type.</para>
        /// <para>u stands for <see cref="ushort"/> type.</para>
        /// </returns>
        public static string GetParameterTypes(Types type)
        {
            switch (type)
            {
                // Controller commands
                case Types.SetSpeedForTime:
                case Types.SetSpeedForDistance:
                case Types.SetSpeedForDegrees:
                    return "DSS";
                case Types.MoveForwardForTime:
                case Types.MoveForwardForDistance:
                case Types.MoveBackwardForTime:
                case Types.MoveBackwardForDistance:
                case Types.RotateLeftForTime:
                case Types.RotateLeftForDegrees:
                case Types.RotateRightForTime:
                case Types.RotateRightForDegrees:
                    return "DS";
                case Types.DoNothing:
                    return "D";
                case Types.Stop:
                    return "";

                // Simulator commands
                case Types.SetX:
                case Types.SetY:
                    return "s";
                case Types.SetAngle:
                    return "D";
                case Types.SetPosition:
                    return "ss";
                case Types.SetPose:
                    return "ssD";
                case Types.SetSimulationSpeed:
                    return "D";
                case Types.GetSimulationSpeed:
                    return "";

                default:
                    throw new Exception("Parameter types for the " + type + " command is not assigned yet!");
            }
        }

        /// <summary>
        /// Get names of parameters for a given command.
        /// </summary>
        /// <param name="type">The input command type.</param>
        /// <returns>Returns the parameter names of the given command.</returns>
        public static string[] GetParameterNames(Types type)
        {
            // Controller parameters
            const string strSpeed = "BothWheelSpeeds";
            const string strSpeed1 = "LeftWheelSpeed";
            const string strSpeed2 = "RightWheelSpeed";
            const string strTime = "Time";
            const string strDistance = "Distance";
            const string strDegrees = "Degrees";

            // Simulator parameters
            const string strX = "X";
            const string strY = "Y";
            const string strAngle = "Angle";
            const string strSimSpeed = "SimulationSpeed";

            switch (type)
            {
                case Types.SetSpeedForTime:
                    return new[] { strTime, strSpeed1, strSpeed2 };
                case Types.SetSpeedForDistance:
                    return new[] { strDistance, strSpeed1, strSpeed2 };
                case Types.SetSpeedForDegrees:
                    return new[] { strDegrees, strSpeed1, strSpeed2 };
                case Types.MoveForwardForTime:
                case Types.MoveBackwardForTime:
                    return new[] { strTime, strSpeed };
                case Types.MoveForwardForDistance:
                case Types.MoveBackwardForDistance:
                    return new[] { strDistance, strSpeed };
                case Types.RotateLeftForTime:
                case Types.RotateRightForTime:
                    return new[] { strTime, strSpeed };
                case Types.RotateLeftForDegrees:
                case Types.RotateRightForDegrees:
                    return new[] { strDegrees, strSpeed };
                case Types.DoNothing:
                    return new[] { strTime };
                case Types.Stop:
                    return new string[0];

                // Simulator commands
                case Types.SetX:
                    return new[] { strX };
                case Types.SetY:
                    return new[] { strY };
                case Types.SetAngle:
                    return new[] { strAngle };
                case Types.SetPosition:
                    return new[] { strX, strY };
                case Types.SetPose:
                    return new[] { strX, strY, strAngle };
                case Types.SetSimulationSpeed:
                    return new[] { strSimSpeed };
                case Types.GetSimulationSpeed:
                    return new string[0];

                default:
                    throw new Exception("Parameter names for the " + type + " command is not assigned yet!");
            }
        }

        /// <summary>
        /// Get description of parameters for a given command.
        /// </summary>
        /// <param name="type">The input command type.</param>
        /// <returns>Returns the parameter descriptions of the given command.</returns>
        public static string[] GetParameterDescriptions(Types type)
        {
            // Controller parameters
            const string strSpeed = "Speed of the both wheels; should be an integer in the range -127 to +127.";
            const string strSpeed1 = "Speed of the left wheel; should be an integer in the range -127 to +127.";
            const string strSpeed2 = "Speed of the right wheel; should be an integer in the range -127 to +127.";
            const string strTime = "Time to move in milliseconds; should be a non-negative double number.";
            const string strDistance = "Distance to move in millimeters; should be a non-negative double number.";
            const string strDegrees = "Amount of degrees to turn; should be a double number.";
            // Simulator parameters
            const string strX = "X position of the robot in the environment in millimeters.";
            const string strY = "Y position of the robot in the environment in millimeters.";
            const string strAngle = "Ten times the angle of the robot in the environment in degrees (1 = 0.1 degrees).";
            const string strSimSpeed = "Ten times the new simulation speed (1 = 0.1x).";

            switch (type)
            {
                // Controller commands
                case Types.SetSpeedForTime:
                    return new[] { strTime, strSpeed1, strSpeed2 };
                case Types.SetSpeedForDistance:
                    return new[] { strDistance, strSpeed1, strSpeed2 };
                case Types.SetSpeedForDegrees:
                    return new[] { strDegrees, strSpeed1, strSpeed2 };
                case Types.MoveForwardForTime:
                case Types.MoveBackwardForTime:
                    return new[] { strTime, strSpeed };
                case Types.MoveForwardForDistance:
                case Types.MoveBackwardForDistance:
                    return new[] { strDistance, strSpeed };
                case Types.RotateLeftForTime:
                case Types.RotateRightForTime:
                    return new[] { strTime, strSpeed };
                case Types.RotateLeftForDegrees:
                case Types.RotateRightForDegrees:
                    return new[] { strDegrees, strSpeed };
                case Types.DoNothing:
                    return new[] { strTime };
                case Types.Stop:
                    return new string[0];

                // Simulator commands
                case Types.SetX:
                    return new[] { strX };
                case Types.SetY:
                    return new[] { strY };
                case Types.SetAngle:
                    return new[] { strAngle };
                case Types.SetPosition:
                    return new[] { strX, strY };
                case Types.SetPose:
                    return new[] { strX, strY, strAngle };
                case Types.SetSimulationSpeed:
                    return new[] { strSimSpeed };
                case Types.GetSimulationSpeed:
                    return new string[0];

                default:
                    throw new Exception("Parameter descriptions for the " + type + " command is not assigned yet!");
            }
        }

        /// <summary>
        /// Get level of a given command.
        /// </summary>
        /// <param name="type">The input command type.</param>
        /// <returns>Returns the level of the given command.</returns>
        public static Levels GetCommandLevel(Types type)
        {
            switch (type)
            {
                case Types.SetSpeedForTime:
                case Types.SetSpeedForDistance:
                case Types.SetSpeedForDegrees:
                case Types.MoveForwardForTime:
                case Types.MoveBackwardForTime:
                case Types.MoveForwardForDistance:
                case Types.MoveBackwardForDistance:
                case Types.RotateLeftForTime:
                case Types.RotateRightForTime:
                case Types.RotateLeftForDegrees:
                case Types.RotateRightForDegrees:
                case Types.Stop:
                case Types.DoNothing:
                    return Levels.Controller;

                case Types.SetX:
                case Types.SetY:
                case Types.SetAngle:
                case Types.SetPosition:
                case Types.SetPose:
                case Types.SetSimulationSpeed:
                case Types.GetSimulationSpeed:
                    return Levels.Simulator;

                default:
                    throw new Exception("Level for the " + type + " command is not assigned yet!");
            }
        }

        # endregion

        # region Parse and TryParse Functions

        /// <summary>
        /// Tries to parse the input string as a command.
        /// </summary>
        /// <param name="parseString">The input string that should be parsed.</param>
        /// <param name="command">Output command that is the result of parsing the input string.
        /// This parameter will be null if parsing fails.</param>
        /// <returns>Returns true if the input string is parsed successfully; otherwise returns false.</returns>
        public static bool TryParse(string parseString, out Command command)
        {
            try
            {
                command = Parse(parseString);
            }
            catch
            {
                command = null;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Parses the input string as a command.
        /// </summary>
        /// <param name="parseString">The input string that should be parsed.</param>
        /// <returns>Returns the command instance that is the result of parsing the input string.</returns>
        /// <exception cref="FormatException">This exception is thrown when the number of arguments for the 
        /// command specified in the input string is not the number required by the specified command.</exception>
        public static Command Parse(string parseString)
        {
            var tokens = parseString.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

            // Parse the command type
            var type = (Types)Enum.Parse(typeof(Types), tokens[0], true);

            // Get the number of parameters for the read command type
            var parameterTypes = GetParameterTypes(type);

            if (tokens.Length != parameterTypes.Length + 1)
                throw new FormatException("The " + type + " command requires exactly " + parameterTypes.Length + " arguments!");

            var parameters = new object[3];

            for (var i = 0; i < parameterTypes.Length; ++i)
            {
                switch (parameterTypes[i])
                {
                    case 'D':
                        parameters[i] = Double.Parse(tokens[i + 1]);
                        break;
                    case 'S':
                        parameters[i] = SByte.Parse(tokens[i + 1]);
                        break;
                    case 'B':
                        parameters[i] = Byte.Parse(tokens[i + 1]);
                        break;
                    case 'I':
                        parameters[i] = Int32.Parse(tokens[i + 1]);
                        break;
                    case 'U':
                        parameters[i] = UInt32.Parse(tokens[i + 1]);
                        break;
                    case 's':
                        parameters[i] = Int16.Parse(tokens[i + 1]);
                        break;
                    case 'u':
                        parameters[i] = UInt16.Parse(tokens[i + 1]);
                        break;
                    default:
                        throw new NotImplementedException("The method for '" + parameterTypes[i] +
                            "' identifier type is not implemented!");
                }
            }

            return new Command(type, parameters[0], parameters[1], parameters[2]);
        }

        # endregion
    }
}
