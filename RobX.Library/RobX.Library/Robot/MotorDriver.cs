﻿# region Includes

using System;
using System.IO.Ports;
using System.Linq;
using RobX.Library.Communication;

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global

# endregion

namespace RobX.Library.Robot
{
    /// <summary>
    /// A class for MD49 motor driver commands and properties.
    /// </summary>
    public class MotorDriver
    {
        # region Public Variables

        /// <summary>
        /// Indicates whether the controller is connected to the robot (via network) or not.
        /// </summary>
        public bool IsConnected;

        # endregion

        # region Protected Variables

        /// <summary>
        /// Remote client instance (TCPClient, ComClient, etc.) that is used to communicate with the robot.
        /// </summary>
        protected IClientInterface RobotClient;

        # endregion

        # region Motor Properties

        /// <summary>
        /// Motor encoder count per turn for EMG49 motors used in RobX robot.
        /// </summary>
        public const int EncoderCountPerTurn = 980;

        /// <summary>
        /// Motor timeout time in milliseconds for MD49 motor driver used in RobX robot.
        /// </summary>
        public const int MotorTimeoutInMs = 2000;

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

        # region Events

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
            RobotClient = robotClient;

            // Add event handlers
            RobotClient.ReceivedData += RobotReceivedData;
            RobotClient.SentData += RobotSentData;
            RobotClient.StatusChanged += RobotStatusChanged;
            RobotClient.BeforeSendingData += RobotBeforeSendingData;
            RobotClient.ErrorOccured += RobotErrorOccured;

            IsConnected = connectFunction();
            return IsConnected;
        }

        /// <summary>
        /// Get voltage of the robot batteries.
        /// </summary>
        /// <returns>The voltage of the robot batteries (in volts).</returns>
        public byte GetVolts()
        {
            byte[] buffer = { 0x00, 0x26 };
            RobotClient.SendData(buffer);

            return ReadBytes(1)[0];
        }

        /// <summary>
        /// Get current drawn by the left motor of the robot (1 Amperes = 10).
        /// </summary>
        /// <returns>Ten times the amperage of the robot left motor.</returns>
        public byte GetCurrent1()
        {
            byte[] buffer = { 0x00, 0x27 };
            RobotClient.SendData(buffer);

            return ReadBytes(1)[0];
        }

        /// <summary>
        /// Get current drawn by the right motor of the robot (1 Amperes = 10).
        /// </summary>
        /// <returns>Ten times the amperage of the robot right motor.</returns>
        public byte GetCurrent2()
        {
            byte[] buffer = { 0x00, 0x28 };
            RobotClient.SendData(buffer);

            return ReadBytes(1)[0];
        }

        /// <summary>
        /// Get the voltage of the batteries and the current drawn by the robot motors.
        /// </summary>
        /// <param name="volts">Voltage of the robot batteries.</param>
        /// <param name="current1">Ten times the amperage of the robot left motor.</param>
        /// <param name="current2">Ten times the amperage of the robot right motor.</param>
        // ReSharper disable once InconsistentNaming
        public void GetVI(out int volts, out int current1, out int current2)
        {
            var buffer = GetVI();

            volts = buffer[0];
            current1 = buffer[1];
            current2 = buffer[2];
        }

        
        /// <summary>
        /// Get the voltage of the batteries and the current drawn by the robot motors.
        /// </summary>
        /// <returns>Returns an array of 3 bytes as follows:
        /// <para>Byte 0: Voltage of the robot batteries.</para>
        /// <para>Byte 1: Ten times the amperage of the robot left motor.</para>
        /// <para>Byte 2: Ten times the amperage of the robot right motor.</para>
        /// </returns>
        // ReSharper disable once InconsistentNaming
        public byte[] GetVI()
        {
            byte[] buffer = { 0x00, 0x2C };
            RobotClient.SendData(buffer);

            return ReadBytes(3);
        }

        /// <summary>
        /// Get the version of the robot motor firmware.
        /// </summary>
        /// <returns>The version of the robot motor firmware.</returns>
        public byte GetVersion()
        {
            byte[] buffer = { 0x00, 0x29 };
            RobotClient.SendData(buffer);

            return ReadBytes(1)[0];
        }

        /// <summary>
        /// Get the Speed1 value of the robot. 
        /// In mode 0 or 1 gets the speed and direction of left wheel. 
        /// In mode 2 or 3 gets the speed and direction of both wheels (subject to effect of turn register).
        /// </summary>
        /// <returns>Speed1 value of the robot.</returns>
        public short GetSpeed1()
        {
            byte[] buffer = { 0x00, 0x21 };
            RobotClient.SendData(buffer);

            return ReadBytes(1)[0];
        }

        /// <summary>
        /// Get the Speed2 value of the robot. 
        /// In mode 0 or 1 gets the speed and direction of right wheel. 
        /// In mode 2 or 3 becomes a Turn value, and is combined with Speed 1 to steer the device. 
        /// </summary>
        /// <returns>Speed2 value of the robot.</returns>
        public short GetSpeed2()
        {
            byte[] buffer = { 0x00, 0x22 };
            RobotClient.SendData(buffer);

            return ReadBytes(1)[0];
        }

        /// <summary>
        /// Get the encoder value of left wheel of the robot.
        /// </summary>
        /// <returns>Encoder value of left wheel of the robot.</returns>
        public int GetEncoder1()
        {
            byte[] buffer = { 0x00, 0x23 };
            RobotClient.SendData(buffer);

            buffer = ReadBytes(4);

            int result = buffer[3];
            result += buffer[2] << 8;
            result += buffer[1] << 16;
            result += buffer[0] << 24;
            return result;
        }

        /// <summary>
        /// Get the encoder value of right wheel of the robot.
        /// </summary>
        /// <returns>Encoder value of right wheel of the robot.</returns>
        public int GetEncoder2()
        {
            byte[] buffer = { 0x00, 0x24 };
            RobotClient.SendData(buffer);

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
        /// <param name="encoder1">Encoder value of the left wheel of the robot.</param>
        /// <param name="encoder2">Encoder value of the right wheel of the robot.</param>
        public void GetEncoders(out int encoder1, out int encoder2)
        {
            var encoders = GetEncoders();
            encoder1 = encoders[0];
            encoder2 = encoders[1];
        }

        /// <summary>
        /// Get the encoder values of both wheels of the robot.
        /// </summary>
        /// <returns><para>Returns an array of 2 integers as below:</para>
        /// <para>Int 0: Encoder value of the left wheel of the robot.</para>
        /// <para>Int 1: Encoder value of the right wheel of the robot.</para>
        /// </returns>
        public int[] GetEncoders()
        {
            byte[] buffer = { 0x00, 0x25 };
            RobotClient.SendData(buffer);

            buffer = ReadBytes(8);

            int encoder1 = buffer[3];
            encoder1 += buffer[2] << 8;
            encoder1 += buffer[1] << 16;
            encoder1 += buffer[0] << 24;

            int encoder2 = buffer[7];
            encoder2 += buffer[6] << 8;
            encoder2 += buffer[5] << 16;
            encoder2 += buffer[4] << 24;

            return new[] {encoder1, encoder2};
        }

        /// <summary>
        /// Get acceleration level (1 - 10) for robot motors.
        /// </summary>
        /// <returns>Acceleration level for robot motors.</returns>
        public byte GetAcceleration()
        {
            byte[] buffer = { 0x00, 0x2A };
            RobotClient.SendData(buffer);

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
            RobotClient.SendData(buffer);

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
            var errorByte = GetError();
            voltsUnder16 = (errorByte & (1 << 7)) == 1;
            voltsOver30 = (errorByte & (1 << 6)) == 1;
            motor1Trip = (errorByte & (1 << 2)) == 1;
            motor2Trip = (errorByte & (1 << 3)) == 1;
            motor1Short = (errorByte & (1 << 4)) == 1;
            motor2Short = (errorByte & (1 << 5)) == 1;
            return (voltsOver30 && voltsUnder16 && motor1Short && motor1Trip && motor2Short && motor2Trip);
        }

        /// <summary>
        /// Get the error state of the robot.
        /// </summary>
        /// <returns>
        /// <para>Returns 0 if no errors occured; otherwise returns an 8-bit number as below:</para>
        /// <para>Bit 2: If 1, left wheel of the robot is tripping.</para>
        /// <para>Bit 3: If 1, right wheel of the robot is tripping.</para>
        /// <para>Bit 4: If 1, left motor of the robot is short-circuited (draws large current).</para>
        /// <para>Bit 5: If 1, right motor of the robot is short-circuited (draws large current).</para>
        /// <para>Bit 6: If 1, voltage of the robot batteries is over 30 volts.</para>
        /// <para>Bit 7: If 1, voltage of the robot batteries is under 16 volts.</para>
        /// </returns>
        public byte GetError()
        {
            byte[] buffer = { 0x00, 0x2D };
            RobotClient.SendData(buffer);
            return ReadBytes(1)[0];
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
            RobotClient.SendData(buffer);
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
            RobotClient.SendData(buffer);
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
            RobotClient.SendData(buffer);
        }

        /// <summary>
        /// Set acceleration level (1 - 10) for robot motors.
        /// </summary>
        public void SetAcceleration(byte acceleration)
        {
            byte[] buffer = { 0x00, 0x33, acceleration };
            RobotClient.SendData(buffer);
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
            SetMode((byte) mode);
        }

        /// <summary>
        /// Get mode of the motor:
        /// 0 (Default) : The speeds of wheels are in the range of 0 (Full Reverse) 128 (Stop) 255 (Full Forward).
        /// 1 : The speeds of wheels are in the range of -128 (Full Reverse) 0 (Stop) 127 (Full Forward).
        /// 2 :	Uses SPEED 1 for both motors, and SPEED 2 for turn value. Data is in the range of 0 (Full Reverse) 128 (Stop) 255 (Full Forward).
        /// 3 :	Uses SPEED 1 for both motors, and SPEED 2 for turn value. Data is in the range of -128 (Full Reverse) 0 (Stop) 127 (Full Forward).
        /// </summary>
        /// <returns>Mode of the motor (0 - 3).</returns>
        public void SetMode(byte mode)
        {
            byte[] buffer = { 0x00, 0x34, mode };
            RobotClient.SendData(buffer);
        }

        /// <summary>
        /// Reset (set to zero) the values of robot wheel encoders.
        /// </summary>
        public void ResetEncoders()
        {
            byte[] buffer = { 0x00, 0x35 };
            RobotClient.SendData(buffer);
        }

        /// <summary>
        /// Turn off speed regulation: If the required speed is not being achieved, increase power to the motors 
        /// until it reaches the desired rate (or the motors reach the maximum output).
        /// </summary>
        public void DisableRegulator()
        {
            byte[] buffer = { 0x00, 0x36 };
            RobotClient.SendData(buffer);
        }

        /// <summary>
        /// Turn on speed regulation: If the required speed is not being achieved, increase power to the motors 
        /// until it reaches the desired rate (or the motors reach the maximum output).
        /// </summary>
        public void EnableRegulator()
        {
            byte[] buffer = { 0x00, 0x37 };
            RobotClient.SendData(buffer);
        }

        /// <summary>
        /// Turn off motor timeout: Robot will automatically stop if there is no serial communications within 2 seconds.
        /// </summary>
        public void DisableTimeout()
        {
            byte[] buffer = { 0x00, 0x38 };
            RobotClient.SendData(buffer);
        }

        /// <summary>
        /// Turn on motor timeout: Robot will automatically stop if there is no serial communications within 2 seconds.
        /// </summary>
        public void EnableTimeout()
        {
            byte[] buffer = { 0x00, 0x39 };
            RobotClient.SendData(buffer);
        }

        # endregion

        # region Protected Functions

        /// <summary>
        /// Reads bytes from remote client. Fills return array with zero if anythig goes wrong.
        /// </summary>
        /// <param name="bytesToRead">Number of bytes to read from the remote client.</param>
        /// <returns>An array with size of bytesToRead filled with read data from the remote client. 
        /// Return an array of zero values with size bytesToRead if anythig goes wrong.</returns>
        protected byte[] ReadBytes(int bytesToRead)
        {
            byte[] buffer;

            if (!RobotClient.ReceiveData(bytesToRead, out buffer) || buffer.Length < bytesToRead)
                return Enumerable.Repeat<byte>(0, bytesToRead).ToArray();

            return buffer;
        }

        # endregion
    }
}
