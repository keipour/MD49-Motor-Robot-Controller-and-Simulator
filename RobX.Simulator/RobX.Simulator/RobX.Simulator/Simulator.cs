# region Includes

using System;
using System.Collections.Generic;
using RobX.Library.Commons;
using RobX.Simulator.Properties;
// ReSharper disable InconsistentlySynchronizedField

# endregion

namespace RobX.Simulator
{
    /// <summary>
    /// Class that simulates RobX differential drive robot.
    /// </summary>
    public class Simulator
    {
        # region Locks

        /// <summary>
        /// Exclusive lock for Simulator.SendByte variable (to avoid multithreading issues).
        /// </summary>
        internal static readonly object SendByteLock = new object();

        /// <summary>
        /// Exclusive lock for Simulator.Commands variable (to avoid multithreading issues).
        /// </summary>
        internal static readonly object CommandsLock = new object();

        # endregion

        # region Public Fields

        /// <summary>
        /// Sets simulation speed (1.0 = realtime).
        /// </summary>
        public static double SimulationSpeed = Settings.Default.DefaultSimulationSpeed;

        /// <summary>
        /// Environment specification of simulation (Ground size, robot position, etc.).
        /// </summary>
        public Environment Environment = new Environment();

        /// <summary>
        /// Virtual robot. Works (almost) exactly like the real robot.
        /// </summary>
        public Robot Robot = new Robot();

        /// <summary>
        /// Contains render options of the simulation.
        /// </summary>
        public readonly RenderOptions Render = new RenderOptions();

        /// <summary>
        /// Indicates whether the simulation is running.
        /// </summary>
        public bool IsRunning;

        /// <summary>
        /// Instance of the Drawer class that draws the frames of the simulation.
        /// </summary>
        public readonly Drawer Drawer = new Drawer();

        # endregion

        # region Private Fields

        /// <summary>
        /// The list of all commands received from the client that have not been executed yet.
        /// </summary>
        private LinkedList<Command> _commands = new LinkedList<Command>();

        /// <summary>
        /// The list of all responces from the simulator to the client that have not been sent yet.
        /// </summary>
        private LinkedList<byte> _sendBytes = new LinkedList<byte>();

        private DateTime _lasttime = DateTime.Now;               // Time of the last simulation step
        private DateTime _currenttime = DateTime.Now;            // Current simulation time
        private readonly Executer _executer = new Executer();    // Executer of robot simulation steps

        # endregion

        # region Private Methods

        /// <summary>
        /// Performs one step of the simulation. This function is the timer.tick() event.
        /// </summary>
        public void StepSimulation()
        {
            // Check if simulation is currently not stopped
            if (IsRunning == false) return;

            // Preserve current time
            _currenttime = DateTime.Now;

            // Execute all commands received before the current time
            while (_executer.ExecuteNextStep(ref _commands, ref _sendBytes, ref Robot, ref Environment, _currenttime, ref _lasttime)) { }

            // Preserve the time of the last simulation step
            _lasttime = _currenttime;
        }

        # endregion

        # region Public Methods

        /// <summary>
        /// Returns all robot reply bytes to send. Also removes them from the sending list.
        /// </summary>
        /// <returns>Array of bytes that should be sent. Returns null if there is nothing to send.</returns>
        public byte[] GetSentBytes()
        {
            byte[] result;
            lock (SendByteLock)
            {
                // Return null if there is nothing to send
                if (_sendBytes.Count == 0) return null;

                // Get number of bytes that should be sent
                var count = _sendBytes.Count;

                // Copy bytes from sending list to return variable and remove them from the sending list
                result = new byte[count];
                for (var i = 0; i < count; ++i)
                {
                    result[i] = _sendBytes.First.Value;
                    _sendBytes.RemoveFirst();
                }
            }
            return result;
        }

        /// <summary>
        /// Adds an array of commands to the received queue for further processing.
        /// </summary>
        /// <param name="code">The received 8-bit byte array.</param>
        public void AddCommands(IEnumerable<byte> code)
        {
            // Use lock to avoid multithreading issues
            lock (CommandsLock)
            {
                foreach (var c in code)
                    _commands.AddLast(new Command(c, DateTime.Now));
            }
        }

        /// <summary>
        /// Start simulation.
        /// </summary>
        /// <param name="renderType">Render type for simulation frames.</param>
        public void RunSimulation(RenderOptions.RenderType renderType = RenderOptions.RenderType.StaticAxisZeroCentered)
        {
            Render.Type = renderType;
            _lasttime = DateTime.Now;
            IsRunning = true;
        }

        /// <summary>
        /// Stop current simulation.
        /// </summary>
        public void StopSimulation()
        {
            IsRunning = false;
        }

        /// <summary>
        /// Continue previously stopped simulation.
        /// </summary>
        public void ContinueSimulation()
        {
            IsRunning = true;
        }

        /// <summary>
        /// Adds an obstacle to the simulation environment.
        /// </summary>
        /// <param name="obstacle">Obstacle object.</param>
        public void AddObstacle(Obstacle obstacle)
        {
            Environment.Obstacles.Add(obstacle);
        }

        # endregion

        # region Public Class (RenderOptions)

        /// <summary>
        /// Class for simulation rendering options.
        /// </summary>
        public class RenderOptions
        {
            /// <summary>
            /// Different render types for simulation frame.
            /// </summary>
            public enum RenderType
            {
                /// <summary>
                /// The screen is fixed and centered on (0, 0).
                /// </summary>
                StaticAxisZeroCentered = 0,

                /// <summary>
                /// The screen is fixed and (0, 0) is the lower-left corner.
                /// </summary>
                StaticAxisZeroCornered = 1
            }

            /// <summary>
            /// Indicates the type of the rendering.
            /// </summary>
            public RenderType Type = RenderType.StaticAxisZeroCentered;
        }

        # endregion
    }
}