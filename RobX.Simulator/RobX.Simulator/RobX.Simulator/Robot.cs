# region Includes

using System;
using Microsoft.Xna.Framework.Graphics;

# endregion

namespace RobX.Simulator
{
    /// <summary>
    /// Class that contains all robot properties.
    /// </summary>
    public class Robot
    {
        # region Private Fields

        private byte _previousmode;
        private byte _mode;
        private byte _speed1 = 128;
        private byte _speed2 = 128;

        # endregion

        # region Public Fields 

        /// <summary>
        /// Array of robot images in all 360 directions.
        /// </summary>
        public Texture2D[] Image;

        /// <summary>
        /// The version of the motor controller's software.
        /// </summary>
        public readonly byte Version = 1;

        /// <summary>
        /// The voltage of robot batteries.
        /// </summary>
        public readonly byte Volts = 24;

        /// <summary>
        /// The current driven by motor 1 (1 = 100 mA).
        /// </summary>
        public readonly byte Current1 = 10;

        /// <summary>
        /// The current driven by motor 2 (1 = 100 mA).
        /// </summary>
        public readonly byte Current2 = 10;

        /// <summary>
        /// An error occured in motor (assumed 0 which means no error occured).
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
            get { return _mode; }
            set
            {
                _previousmode = _mode;
                _mode = value;
            }
        }

        /// <summary>
        /// Turn on/off speed regulation: If the required speed is not being achieved, increase power to the motors 
        /// until it reaches the desired rate (or the motors reach the maximum output).
        /// </summary>
        // ReSharper disable once NotAccessedField.Global
        public bool Regulator;

        /// <summary>
        /// Turn on/off motor timeout: Robot will automatically stop if there is no serial communications within 2 seconds.
        /// </summary>
        public bool Timeout = true;

        /// <summary>
        /// A 32-bit signed integer counter for motor 1 encoder.
        /// </summary>
        public int Encoder1;

        /// <summary>
        /// A 32-bit signed integer counter for motor 2 encoder.
        /// </summary>
        public int Encoder2;

        /// <summary>
        /// In mode 0 or 1 sets the speed and direction of wheel 1. 
        /// In mode 2 or 3 controls the speed and direction of both wheels (subject to effect of turn register).
        /// </summary>
        public byte Speed1
        {
            get { return _speed1; }
            set
            {
                _previousmode = _mode;
                _speed1 = value;
            }
        }

        /// <summary>
        /// In mode 0 or 1 sets the speed and direction of wheel 2. 
        /// In mode 2 or 3 becomes a Turn value, and is combined with Speed 1 to steer the device. 
        /// </summary>
        public byte Speed2
        {
            get { return _speed2; }
            set
            {
                _previousmode = _mode;
                _speed2 = value;
            }
        }

        /// <summary>
        /// Acceleration level (1 - 10) for robot motors.
        /// </summary>
        public byte Acceleration = 5;

        # endregion

        # region Public Functions (MotorSpeedToRealSpeed)

        /// <summary>
        /// Calculates normalized individual wheel speeds (in millimeters per second).
        /// </summary>
        /// <param name="leftWheelSpeed">Real speed of left wheel.</param>
        /// <param name="rightWheelSpeed">Real speed of right wheel.</param>
        public void MotorSpeedToRealSpeed(out double leftWheelSpeed, out double rightWheelSpeed)
        {
            Library.Robot.Robot.MotorSpeedToRealSpeed(_previousmode, Speed1, Speed2, out leftWheelSpeed, out rightWheelSpeed);
        }

        # endregion

        # region Constructor

        /// <summary>
        /// Constructor for the Robot class.
        /// </summary>
        public Robot()
        {
            // Randomize initial values for encoders 1 and 2.
            var rand = new Random();
            Encoder1 = rand.Next(Int32.MinValue, Int32.MaxValue);
            Encoder2 = rand.Next(Int32.MinValue, Int32.MaxValue);
        }

        # endregion
    }
}
