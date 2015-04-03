# region Includes

using System;
using RobX.Library.Commons;

# endregion

namespace RobX.Simulator
{
    /// <summary>
    /// Class that contains all robot properties.
    /// </summary>
    public class Robot
    {
        // -------------------------------------- Public Variables ------------------------------ //

        public Microsoft.Xna.Framework.Graphics.Texture2D[] Image;

        /// <summary>
        /// The version of the motor controller's software
        /// </summary>
        public readonly byte Version = 1;

        /// <summary>
        /// The voltage of robot batteries
        /// </summary>
        public readonly byte Volts = 24;

        /// <summary>
        /// The current driven by motor 1 (1 = 100 mA)
        /// </summary>
        public readonly byte Current1 = 10;

        /// <summary>
        /// The current driven by motor 2 (1 = 100 mA)
        /// </summary>
        public readonly byte Current2 = 10;

        /// <summary>
        /// An error occured in motor (assumed 0 which means no error occured)
        /// </summary>
        public readonly byte Error = 0;

        /// <summary>
        /// Mode of the motor:
        /// 0 (Default) : The speeds of wheels are in the range of 0 (Full Reverse) 128 (Stop) 255 (Full Forward).
        /// 1 : The speeds of wheels are in the range of -128 (Full Reverse) 0 (Stop) 127 (Full Forward).
        /// 2 :	Uses SPEED 1 for both motors, and SPEED 2 for turn value. Data is in the range of 0 (Full Reverse) 128 (Stop) 255 (Full Forward).
        /// 3 :	Uses SPEED 1 for both motors, and SPEED 2 for turn value. Data is in the range of -128 (Full Reverse) 0 (Stop) 127 (Full Forward).
        /// </summary>
        public byte Mode
        {
            get { return mode; }
            set
            {
                previousmode = mode;
                mode = value;
            }
        }

        /// <summary>
        /// Turn on/off speed regulation: If the required speed is not being achieved, increase power to the motors 
        /// until it reaches the desired rate (or the motors reach the maximum output)
        /// </summary>
        public bool Regulator = false;

        /// <summary>
        /// Turn on/off motor timeout: Robot will automatically stop if there is no serial communications within 2 seconds.
        /// </summary>
        public bool Timeout = true;

        /// <summary>
        /// A 32-bit signed integer counter for motor 1 encoder
        /// </summary>
        public int Encoder1;

        /// <summary>
        /// A 32-bit signed integer counter for motor 2 encoder
        /// </summary>
        public int Encoder2;

        /// <summary>
        /// In mode 0 or 1 sets the speed and direction of wheel 1. 
        /// In mode 2 or 3 controls the speed and direction of both wheels (subject to effect of turn register).
        /// </summary>
        public byte Speed1
        {
            get { return speed1; }
            set
            {
                previousmode = mode;
                speed1 = value;
            }
        }

        /// <summary>
        /// In mode 0 or 1 sets the speed and direction of wheel 2. 
        /// In mode 2 or 3 becomes a Turn value, and is combined with Speed 1 to steer the device. 
        /// </summary>
        public byte Speed2
        {
            get { return speed2; }
            set
            {
                previousmode = mode;
                speed2 = value;
            }
        }

        /// <summary>
        /// Acceleration level (1 - 10) for robot motors.
        /// </summary>
        public byte Acceleration = 5;

        // -------------------------------------- Public Functions ------------------------------ //

        /// <summary>
        /// Calculates normalized individual wheel speeds (in millimeters per second)
        /// </summary>
        /// <param name="WheelSpeed1">Speed of wheel 1</param>
        /// <param name="WheelSpeed2">Speed of wheel 2</param>
        public void WheelSpeeds(out double WheelSpeed1, out double WheelSpeed2)
        {
            WheelSpeed1 = 0;
            WheelSpeed2 = 0;

            switch (previousmode)
            {
                case 0:
                    WheelSpeed1 = Speed1 - 128;
                    WheelSpeed2 = Speed2 - 128;
                    break;
                case 1:
                    WheelSpeed1 = Methods.ConvertUnsignedByteToSigned(Speed1);
                    WheelSpeed2 = Methods.ConvertUnsignedByteToSigned(Speed2);
                    break;
                case 2:
                    if (Speed1 >= 128)                  // if forward direction
                    {
                        WheelSpeed1 = Speed1 + Speed2 - 256;
                        if (WheelSpeed1 > 127) WheelSpeed1 = 127;

                        WheelSpeed2 = Speed1 - Speed2;
                        if (WheelSpeed2 > 127) WheelSpeed2 = 127;
                    }
                    else                              // if backward direction
                    {
                        WheelSpeed1 = Speed1 - Speed2;
                        if (WheelSpeed1 < -128) WheelSpeed1 = -128;

                        WheelSpeed2 = Speed1 + Speed2 - 256;
                        if (WheelSpeed2 < -128) WheelSpeed2 = -128;
                    }
                    break;
                case 3:
                    int speed1 = Methods.ConvertUnsignedByteToSigned(Speed1);
                    int speed2 = Methods.ConvertUnsignedByteToSigned(Speed2);

                    if (speed1 >= 0)                  // if forward direction
                    {
                        WheelSpeed1 = speed1 + speed2;
                        if (WheelSpeed1 > 127) WheelSpeed1 = 127;

                        WheelSpeed2 = speed1 - speed2;
                        if (WheelSpeed2 > 127) WheelSpeed2 = 127;
                    }
                    else                              // if backward direction
                    {
                        WheelSpeed1 = speed1 - speed2;
                        if (WheelSpeed1 < -128) WheelSpeed1 = -128;

                        WheelSpeed2 = speed1 + speed2;
                        if (WheelSpeed2 < -128) WheelSpeed2 = -128;
                    }
                    break;
            }
            WheelSpeed1 *= Library.Commons.Robot.RobotSpeedToMmpS;
            WheelSpeed2 *= Library.Commons.Robot.RobotSpeedToMmpS;
        }

        // ------------------------------------------ Constructor ------------------------------- //

        /// <summary>
        /// Constructor for the Robot class
        /// </summary>
        public Robot()
        {
            // Randomize initial values for encoders 1 and 2
            var rand = new Random();
            Encoder1 = rand.Next(Int32.MinValue, Int32.MaxValue);
            Encoder2 = rand.Next(Int32.MinValue, Int32.MaxValue);
        }

        // ------------------------------------------ Constructor ------------------------------- //

        private byte previousmode = 0;
        private byte mode = 0;
        private byte speed1 = 128;
        private byte speed2 = 128;
    }
}
