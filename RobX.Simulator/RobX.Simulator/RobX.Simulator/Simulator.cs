# region Includes

using System;
using System.Collections.Generic;
using RobX.Library.Commons;

# endregion

namespace RobX.Simulator
{
    /// <summary>
    /// Class that simulates RobX differential drive robot
    /// </summary>
    public class Simulator
    {
        // ------------------------------------------ Public Constants ----------------------------- //

        /// <summary>
        /// Exclusive lock for Simulator.SendByte variable (to avoid multithreading issues)
        /// </summary>
        internal static object SendByteLock = new object();

        /// <summary>
        /// Exclusive lock for Simulator.Commands variable (to avoid multithreading issues)
        /// </summary>
        internal static object CommandsLock = new object();


        /// <summary>
        /// Sets simulation speed (1.0 = realtime)
        /// </summary>
        public static double SimulationSpeed = Properties.Settings.Default.DefaultSimulationSpeed;

        // ------------------------------------------ Public Variables ----------------------------- //

        /// <summary>
        /// Environment specification of simulation (Ground size, robot position, etc.)
        /// </summary>
        public Environment Environment = new Environment();

        /// <summary>
        /// Virtual robot. Works almost exactly like the real robot.
        /// </summary>
        public Robot Robot = new Robot();

        /// <summary>
        /// Contains render options of the simulation.
        /// </summary>
        public RenderOptions Render = new RenderOptions();

        public bool IsRunning = false;               // Indicates whether the simulation is running

        // ------------------------------------------ Private Variables ---------------------------- //

        /// <summary>
        /// The list of all commands received from the client that have not been executed yet.
        /// </summary>
        private LinkedList<Command> Commands = new LinkedList<Command>();

        /// <summary>
        /// The list of all responces from the simulator to the client that have not been sent yet.
        /// </summary>
        private LinkedList<byte> SendBytes = new LinkedList<byte>();

        public DateTime lasttime = DateTime.Now;               // Time of the last simulation step
        public DateTime starttime = DateTime.Now;              // Start time of the simulation
        public DateTime currenttime = DateTime.Now;            // Current simulation time
        public Drawer Drawer = new Drawer();                  // Draws the frames of the simulation
        private Executer Executer = new Executer();             // Executer of robot simulation steps

        // ------------------------------------------ Private Functions ---------------------------- //

        /// <summary>
        /// Performs one step of the simulation. This function is the timer.tick() event.
        /// </summary>
        /// <param name="sender">The timer object invoking call to the function.</param>
        /// <param name="e">Event arguments of the timer.tick() event.</param>
        public void StepSimulation()
        {
            // Check if simulation is currently not stopped
            if (IsRunning == false) return;

            // Preserve current time
            currenttime = DateTime.Now;

            // Execute all commands received before the current time
            while (Executer.ExecuteNextStep(ref Commands, ref SendBytes, ref Robot, ref Environment, currenttime, ref lasttime)) ;

            // Render current frame
            //Drawing.Render(ref control, ref Environment, Robot, System.Drawing.Color.LightSkyBlue,
            //    starttime, currenttime, Render.Type, Render.Grid, Render.Obstacles, Render.Statistics, Render.RobotTrace);

            // Preserve the time of the last simulation step
            lasttime = currenttime;
        }

        // ------------------------------------------ Public Functions ----------------------------- //

        /// <summary>
        /// Returns all robot reply bytes to send. Also removes them from the sending list.
        /// </summary>
        /// <returns>Array of bytes that should be sent. Returns null if there is nothing to send.</returns>
        public byte[] GetSentBytes()
        {
            byte[] result = null;
            lock (SendByteLock)
            {
                // Return null if there is nothing to send
                if (SendBytes.Count != 0)
                {
                    // Get number of bytes that should be sent
                    var Count = SendBytes.Count;

                    // Copy bytes from sending list to return variable and remove them from the sending list
                    result = new byte[Count];
                    for (var i = 0; i < Count; ++i)
                    {
                        result[i] = SendBytes.First.Value;
                        SendBytes.RemoveFirst();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Adds an array of commands to the received queue for further processing
        /// </summary>
        /// <param name="Code">The received 8-bit byte array</param>
        public void AddCommands(byte[] Code)
        {
            // Use lock to avoid multithreading issues
            lock (CommandsLock)
            {
                for (var i = 0; i < Code.Length; ++i)
                    Commands.AddLast(new Command(Code[i], DateTime.Now));
            }
        }

        /// <summary>
        /// Start simulation
        /// </summary>
        /// <param name="control">Control to draw simulation that has Graphics (Form, PictureBox, etc.)</param>
        /// <param name="interval">Minimum interval (in milliseconds) between two consecutive simulation frames</param>
        /// <param name="renderType">Render type for simulation frames</param>
        public void RunSimulation(System.Windows.Forms.Control control, int interval,
            RenderOptions.RenderType renderType = RenderOptions.RenderType.StaticAxis_ZeroCentered)
        {
            Render.Type = renderType;
            lasttime = DateTime.Now;
            starttime = DateTime.Now;
            IsRunning = true;
        }

        /// <summary>
        /// Stop current simulation
        /// </summary>
        public void StopSimulation()
        {
            IsRunning = false;
        }

        /// <summary>
        /// Continue previously stopped simulation
        /// </summary>
        public void ContinueSimulation()
        {
            IsRunning = true;
        }

        /// <summary>
        /// Adds an obstacle to the simulation environment.
        /// </summary>
        /// <param name="obstacle">Obstacle object</param>
        public void AddObstacle(Obstacle obstacle)
        {
            Environment.Obstacles.Add(obstacle);
        }

        // ------------------------------------------ Public Classes ------------------------------- //

        /// <summary>
        /// Class for simulation rendering options
        /// </summary>
        public class RenderOptions
        {
            /// <summary>
            /// Different render types for simulation frame
            /// </summary>
            public enum RenderType
            {
                StaticAxis_ZeroCentered = 0,    // The screen is fixed and centered on (0, 0)
                StaticAxis_ZeroCornered = 1,    // The screen is fixed and (0, 0) is the lower-left corner
            }

            /// <summary>
            /// Indicates the type of the rendering
            /// </summary>
            public RenderType Type = RenderType.StaticAxis_ZeroCentered;
        }
    }
}