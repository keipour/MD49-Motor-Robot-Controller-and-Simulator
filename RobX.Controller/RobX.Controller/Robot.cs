# region Includes

using System;
using RobX.Communication;
using RobX.Communication.TCP;

# endregion

namespace RobX.Controller
{
    /// <summary>
    /// Class that contains robot hardware-level commands and general robot specifications
    /// </summary>
    public class Robot
    {
        // -------------------------------------- Public Constants ------------------------------ //

        /// <summary>
        /// Motor encoder count per shaft turn
        /// </summary>
        public const int EncoderCountPerTurn = 980;

        /// <summary>
        /// Converts robot speed to millimeters per second
        /// </summary>
        public const double RobotSpeedToMMpS = 6.25F;

        /// <summary>
        /// Converts encoder count to millimeters
        /// </summary>
        public const double EncoderCount2mM = WheelDiameter * Math.PI / EncoderCountPerTurn;

        /// <summary>
        /// Diameter of robot wheels in millimeters
        /// </summary>
        public const double WheelDiameter = 125.0F;

        /// <summary>
        /// The robot radius in millimeters
        /// </summary>
        public const int Radius = 250;

        // ------------------------------------------- Private Variables --------------------------------------- //

        private TCPClient RobotClient;
        private Commons.Robot.RobotType RobotType;

        // ------------------------------------------- Event Handlers ------------------------------------------ //

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
        /// Event handler for StatusChange event (will be invoked after anything new happens in the communication)
        /// </summary>
        public event CommunicationStatusEventHandler StatusChanged;

        public event EventHandler ErrorOccured;

        // ------------------------------------------- Constructors and Events --------------------------------- //

        /// <summary>
        /// Constructor for the robot class
        /// </summary>
        /// <param name="robotType">Specifies robot type (Simulation vs. Real)</param>
        public Robot(Commons.Robot.RobotType robotType)
        {
            RobotType = robotType;

            // Initialize robot's network interface
            RobotClient = new TCPClient();

            // Add event handlers
            RobotClient.ReceivedData += RobotReceivedData;
            RobotClient.SentData += RobotSentData;
            RobotClient.StatusChanged += RobotStatusChanged;
            RobotClient.BeforeSendingData += RobotBeforeSendingData;
            RobotClient.ErrorOccured += RobotErrorOccured;
        }

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
        private void RobotStatusChanged(object sender, CommunicationStatusEventArgs e)
        {
            if (StatusChanged != null)
                StatusChanged(this, e);
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
        }

        // ------------------------------------------- Public Functions ---------------------------------------- //

        /// <summary>
        /// Connect to robot or simulator over the network
        /// </summary>
        /// <param name="IPAddress">IP address of the robot (or simulator)</param>
        /// <param name="Port">Port number of the robot (or simulator)</param>
        /// <returns>Returns true if successfully connected to the robot</returns>
        public Boolean Connect(String IPAddress, int Port)
        {
            return RobotClient.Connect(IPAddress, Port);
        }

        /// <summary>
        /// Get voltage of the robot batteries
        /// </summary>
        /// <returns>The voltage of the robot batteries (in volts)</returns>
        public byte GetVolts()
        {
            byte[] Buffer = { 0x00, 0x26 };
            RobotClient.SendData(Buffer);
            Buffer = RobotClient.ReceiveData(true, 1);
            return Buffer[0];
        }

        /// <summary>
        /// Get current drawn by the motor 1 of the robot (1 Amperes = 10)
        /// </summary>
        /// <returns>Ten times the amperage of the robot motor 1</returns>
        public byte GetCurrent1()
        {
            byte[] Buffer = { 0x00, 0x27 };
            RobotClient.SendData(Buffer);
            Buffer = RobotClient.ReceiveData(true, 1);
            return Buffer[0];
        }

        /// <summary>
        /// Get current drawn by the motor 2 of the robot (1 Amperes = 10)
        /// </summary>
        /// <returns>Ten times the amperage of the robot motor 2</returns>
        public byte GetCurrent2()
        {
            byte[] Buffer = { 0x00, 0x28 };
            RobotClient.SendData(Buffer);
            Buffer = RobotClient.ReceiveData(true, 1);
            return Buffer[0];
        }

        /// <summary>
        /// Get the voltage of the batteries and the current drawn by the robot motors
        /// </summary>
        /// <param name="Volts">Voltage of the robot batteries</param>
        /// <param name="Current1">Ten times the amperage of the robot motor 1</param>
        /// <param name="Current2">Ten times the amperage of the robot motor 2</param>
        public void GetVI(out int Volts, out int Current1, out int Current2)
        {
            byte[] Buffer = { 0x00, 0x2C };
            RobotClient.SendData(Buffer);
            Buffer = RobotClient.ReceiveData(true, 3);
            Volts = Buffer[0];
            Current1 = Buffer[1];
            Current2 = Buffer[2];
        }

        /// <summary>
        /// Get the version of the robot motor firmware
        /// </summary>
        /// <returns>The version of the robot motor firmware</returns>
        public byte GetVersion()
        {
            byte[] Buffer = { 0x00, 0x29 };
            RobotClient.SendData(Buffer);
            Buffer = RobotClient.ReceiveData(true, 1);
            return Buffer[0];
        }

        /// <summary>
        /// Get the Speed1 value of the robot. 
        /// In mode 0 or 1 gets the speed and direction of wheel 1. 
        /// In mode 2 or 3 gets the speed and direction of both wheels (subject to effect of turn register).
        /// </summary>
        /// <returns>Speed1 value of the robot</returns>
        public short GetSpeed1()
        {
            byte[] Buffer = { 0x00, 0x21 };
            RobotClient.SendData(Buffer);
            Buffer = RobotClient.ReceiveData(true, 1);
            return Buffer[0];
        }

        /// <summary>
        /// Get the Speed2 value of the robot. 
        /// In mode 0 or 1 gets the speed and direction of wheel 2. 
        /// In mode 2 or 3 becomes a Turn value, and is combined with Speed 1 to steer the device. 
        /// </summary>
        /// <returns>Speed2 value of the robot</returns>
        public short GetSpeed2()
        {
            byte[] Buffer = { 0x00, 0x22 };
            RobotClient.SendData(Buffer);
            Buffer = RobotClient.ReceiveData(true, 1);
            return Buffer[0];
        }

        /// <summary>
        /// Get the encoder value of wheel 1 of the robot
        /// </summary>
        /// <returns>Encoder value of wheel 1 of the robot</returns>
        public int GetEncoder1()
        {
            byte[] Buffer = { 0x00, 0x23 };
            RobotClient.SendData(Buffer);
            Buffer = RobotClient.ReceiveData(true, 4);
            int result = Buffer[3];
            result += Buffer[2] << 8;
            result += Buffer[1] << 16;
            result += Buffer[0] << 24;
            return result;
        }

        /// <summary>
        /// Get the encoder value of wheel 2 of the robot
        /// </summary>
        /// <returns>Encoder value of wheel 2 of the robot</returns>
        public int GetEncoder2()
        {
            byte[] Buffer = { 0x00, 0x24 };
            RobotClient.SendData(Buffer);
            Buffer = RobotClient.ReceiveData(true, 4);
            int result = Buffer[3];
            result += Buffer[2] << 8;
            result += Buffer[1] << 16;
            result += Buffer[0] << 24;
            return result;
        }

        /// <summary>
        /// Get the encoder values of both wheels of the robot
        /// </summary>
        /// <param name="Encoder1">Encoder value of wheel 1 of the robot</param>
        /// <param name="Encoder2">Encoder value of wheel 2 of the robot</param>
        public void GetEncoders(out int Encoder1, out int Encoder2)
        {
            byte[] Buffer = { 0x00, 0x25 };
            RobotClient.SendData(Buffer);
            Buffer = RobotClient.ReceiveData(true, 8);
            Encoder1 = Buffer[3];
            Encoder1 += Buffer[2] << 8;
            Encoder1 += Buffer[1] << 16;
            Encoder1 += Buffer[0] << 24;
            Encoder2 = Buffer[7];
            Encoder2 += Buffer[6] << 8;
            Encoder2 += Buffer[5] << 16;
            Encoder2 += Buffer[4] << 24;
        }

        /// <summary>
        /// Get acceleration level (1 - 10) for robot motors
        /// </summary>
        /// <returns>Acceleration level for robot motors</returns>
        public byte GetAcceleration()
        {
            byte[] Buffer = { 0x00, 0x2A };
            RobotClient.SendData(Buffer);
            Buffer = RobotClient.ReceiveData(true, 1);
            return Buffer[0];
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
        /// <returns>Mode of the motor (0 - 3)</returns>
        public byte GetMode()
        {
            byte[] Buffer = { 0x00, 0x2B };
            RobotClient.SendData(Buffer);
            Buffer = RobotClient.ReceiveData(true, 1);
            return Buffer[0];
        }

        /// <summary>
        /// Get the error state of the robot
        /// </summary>
        /// <param name="VoltsUnder16">If true, voltage of the robot batteries is under 16 volts</param>
        /// <param name="VoltsOver30">If true, voltage of the robot batteries is over 30 volts</param>
        /// <param name="Motor1Trip">If true, wheel 1 of the robot is tripping</param>
        /// <param name="Motor2Trip">If true, wheel 2 of the robot is tripping</param>
        /// <param name="Motor1Short">If true, motor 1 of the robot is short-circuited (draws large current)</param>
        /// <param name="Motor2Short">If true, motor 2 of the robot is short-circuited (draws large current)</param>
        /// <returns>Returns false if no errors occured; otherwise returns true</returns>
        public bool GetError(out bool VoltsUnder16, out bool VoltsOver30, out bool Motor1Trip,
            out bool Motor2Trip, out bool Motor1Short, out bool Motor2Short)
        {
            byte[] Buffer = { 0x00, 0x2D };
            RobotClient.SendData(Buffer);
            Buffer = RobotClient.ReceiveData(true, 1);
            VoltsUnder16 = (Buffer[0] & (1 << 7)) == 1;
            VoltsOver30 = (Buffer[0] & (1 << 6)) == 1;
            Motor1Trip = (Buffer[0] & (1 << 2)) == 1;
            Motor2Trip = (Buffer[0] & (1 << 3)) == 1;
            Motor1Short = (Buffer[0] & (1 << 4)) == 1;
            Motor2Short = (Buffer[0] & (1 << 5)) == 1;
            return (VoltsOver30 && VoltsUnder16 && Motor1Short && Motor1Trip && Motor2Short && Motor2Trip);
        }

        /// <summary>
        /// Set the Speed1 value of the robot. 
        /// In mode 0 or 1 sets the speed and direction of wheel 1. 
        /// In mode 2 or 3 sets the speed and direction of both wheels (subject to effect of turn register).
        /// </summary>
        /// <param name="Speed">Speed1 value of the robot</param>
        public void SetSpeed1(short Speed)
        {
            byte[] Buffer = { 0x00, 0x31, (byte)Speed };
            RobotClient.SendData(Buffer);
        }

        /// <summary>
        /// Set the Speed2 value of the robot. 
        /// In mode 0 or 1 sets the speed and direction of wheel 2. 
        /// In mode 2 or 3 becomes a turn value, and is combined with Speed 1 to steer the device. 
        /// </summary>
        /// <param name="Speed">Speed2 value of the robot</param>
        public void SetSpeed2(short Speed)
        {
            byte[] Buffer = { 0x00, 0x32, (byte)Speed };
            RobotClient.SendData(Buffer);
        }

        /// <summary>
        /// Set both Speed1 and Speed2 simultaneously for the robot. 
        /// In mode 0 or 1 Speed1 sets the speed and direction of wheel 1 and Speed2 sets the speed and direction of wheel 2. 
        /// In mode 2 or 3 Speed1 sets the speed and direction of both wheels (subject to effect of Spees2 register) and Speed 2
        /// becomes a turn value, and is combined with Speed1 to steer the device.
        /// </summary>
        /// <param name="Speed1">Speed1 value of the robot</param>
        /// <param name="Speed2">Speed2 value of the robot</param>
        public void SetSpeeds(short Speed1, short Speed2)
        {
            byte[] Buffer = { 0x00, 0x31, (byte)Speed1, 0x00, 0x32, (byte)Speed2 };
            RobotClient.SendData(Buffer);
        }

        /// <summary>
        /// Set acceleration level (1 - 10) for robot motors
        /// </summary>
        public void SetAcceleration(byte Acceleration)
        {
            byte[] Buffer = { 0x00, 0x33, Acceleration };
            RobotClient.SendData(Buffer);
        }

        /// <summary>
        /// Get mode of the motor:
        /// 0 (Default) : The speeds of wheels are in the range of 0 (Full Reverse) 128 (Stop) 255 (Full Forward).
        /// 1 : The speeds of wheels are in the range of -128 (Full Reverse) 0 (Stop) 127 (Full Forward).
        /// 2 :	Uses SPEED 1 for both motors, and SPEED 2 for turn value. Data is in the range of 0 (Full Reverse) 128 (Stop) 255 (Full Forward).
        /// 3 :	Uses SPEED 1 for both motors, and SPEED 2 for turn value. Data is in the range of -128 (Full Reverse) 0 (Stop) 127 (Full Forward).
        /// </summary>
        /// <returns>Mode of the motor (0 - 3)</returns>
        public void SetMode(Commons.Robot.SpeedModes Mode)
        {
            byte[] Buffer = { 0x00, 0x34, (byte)Mode };
            RobotClient.SendData(Buffer);
        }

        /// <summary>
        /// Reset (set to zero) the values of robot wheel encoders
        /// </summary>
        public void ResetEncoders()
        {
            byte[] Buffer = { 0x00, 0x35 };
            RobotClient.SendData(Buffer);
        }

        /// <summary>
        /// Turn off speed regulation: If the required speed is not being achieved, increase power to the motors 
        /// until it reaches the desired rate (or the motors reach the maximum output)
        /// </summary>
        public void DisableRegulator()
        {
            byte[] Buffer = { 0x00, 0x36 };
            RobotClient.SendData(Buffer);
        }

        /// <summary>
        /// Turn on speed regulation: If the required speed is not being achieved, increase power to the motors 
        /// until it reaches the desired rate (or the motors reach the maximum output)
        /// </summary>
        public void EnableRegulator()
        {
            byte[] Buffer = { 0x00, 0x37 };
            RobotClient.SendData(Buffer);
        }

        /// <summary>
        /// Turn off motor timeout: Robot will automatically stop if there is no serial communications within 2 seconds.
        /// </summary>
        public void DisableTimeout()
        {
            byte[] Buffer = { 0x00, 0x38 };
            RobotClient.SendData(Buffer);
        }

        /// <summary>
        /// Turn on motor timeout: Robot will automatically stop if there is no serial communications within 2 seconds.
        /// </summary>
        public void EnableTimeout()
        {
            byte[] Buffer = { 0x00, 0x39 };
            RobotClient.SendData(Buffer);
        }

        /// <summary>
        /// Set x position of the robot in the environment in millimeters (works only in simulation mode)
        /// </summary>
        /// <param name="X">X position of the robot in the environment in millimeters</param>
        public void SetX(short X)
        {
            if (RobotType == Commons.Robot.RobotType.Real) return;
            byte[] Buffer = { 0x00, 0x41, (byte)(X >> 8), (byte)(X & 0xFF) };
            RobotClient.SendData(Buffer);
        }

        /// <summary>
        /// Set y position of the robot in the environment in millimeters (works only in simulation mode)
        /// </summary>
        /// <param name="Y">Y position of the robot in the environment in millimeters</param>
        public void SetY(short Y)
        {
            if (RobotType == Commons.Robot.RobotType.Real) return;
            byte[] Buffer = { 0x00, 0x42, (byte)(Y >> 8), (byte)(Y & 0xFF) };
            RobotClient.SendData(Buffer);
        }

        /// <summary>
        /// Set angle of the robot in the environment in degrees (works only in simulation mode)
        /// </summary>
        /// <param name="Angle">Ten times the angle of the robot in the environment in degrees (1 = 0.1 degrees)</param>
        public void SetAngle(short Angle)
        {
            if (RobotType == Commons.Robot.RobotType.Real) return;
            byte[] Buffer = { 0x00, 0x43, (byte)(Angle >> 8), (byte)(Angle & 0xFF) };
            RobotClient.SendData(Buffer);
        }

        /// <summary>
        /// Set x and y positions of the robot in the environment in millimeters (works only in simulation mode)
        /// </summary>
        /// <param name="X">X position of the robot in the environment in millimeters</param>
        /// <param name="Y">Y position of the robot in the environment in millimeters</param>
        public void SetXY(short X, short Y)
        {
            if (RobotType == Commons.Robot.RobotType.Real) return;
            byte[] Buffer = {   0x00, 0x41, (byte)(X >> 8), (byte)(X & 0xFF) ,
                                0x00, 0x42, (byte)(Y >> 8), (byte)(Y & 0xFF) };
            RobotClient.SendData(Buffer);
        }

        /// <summary>
        /// Set angle (in degrees), x and y positions (in millimeters) of the robot in the environment (works only in simulation mode)
        /// </summary>
        /// <param name="X">X position of the robot in the environment in millimeters</param>
        /// <param name="Y">Y position of the robot in the environment in millimeters</param>
        /// <param name="Angle">Ten times the angle of the robot in the environment in degrees (1 = 0.1 degrees)</param>
        public void SetXYAngle(short X, short Y, short Angle)
        {
            if (RobotType == Commons.Robot.RobotType.Real) return;
            byte[] Buffer = {   0x00, 0x41, (byte)(X >> 8), (byte)(X & 0xFF) ,
                                0x00, 0x42, (byte)(Y >> 8), (byte)(Y & 0xFF),
                                0x00, 0x43, (byte)(Angle >> 8), (byte)(Angle & 0xFF) };
            RobotClient.SendData(Buffer);
        }

        /// <summary>
        /// Set simulation speed on simulator (works only in simulation mode)
        /// </summary>
        /// <param name="Speed">Ten times the new simulation speed (1 = 0.1x)</param>
        public void SetSimulationSpeed(ushort Speed)
        {
            if (RobotType == Commons.Robot.RobotType.Real) return;
            byte[] Buffer = { 0x00, 0x51, (byte)(Speed >> 8), (byte)(Speed & 0xFF) };
            RobotClient.SendData(Buffer);
        }

        public ushort GetSimulationSpeed()
        {
            if (RobotType == Commons.Robot.RobotType.Real) return 0;
            byte[] Buffer = { 0x00, 0x52 };
            RobotClient.SendData(Buffer);
            Buffer = RobotClient.ReceiveData(true, 2);
            ushort result = Buffer[1];
            result += (ushort)(Buffer[0] << 8);
            return result;
        }
    }
}
