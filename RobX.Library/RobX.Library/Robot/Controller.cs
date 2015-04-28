using System;

// ReSharper disable UnusedMember.Global
namespace RobX.Library.Robot
{
    /// <summary>
    /// Class to control robot at high level.
    /// </summary>
    public class Controller : ExtendedRobot
    {
        # region Public Fields

        /// <summary>
        /// Contains controller commands in queue for processing.
        /// </summary>
        public readonly CommandQueue Commands = new CommandQueue();

        # endregion

        # region ErrorOccured Event

        private void OnErrorOccured(object sender, EventArgs e)
        {
            StopExecution();
        }

        # endregion

        # region Constructor

        /// <summary>
        /// Constructor for the Controller class.
        /// </summary>
        /// <param name="type">Specifies robot type (Simulation vs. Real).</param>
        public Controller(RobotType type) : base(type)
        {
            ErrorOccured += OnErrorOccured;
        }

        # endregion

        # region Execution Functions

        /// <summary>
        /// Executes sequentially all the commands in the queue. Execution of command queue using this function
        /// will work as expected when <see cref="ExtendedRobot.PrepareForExecution"/> function is executed first.
        /// </summary>
        public void ExecuteCommandQueue()
        {
            ChangeRobotStatus("Starting execution of command queue...");

            while (Commands.Count > 0)
            {
                ExecuteCommand(Commands.Dequeue());
            }
        }

        /// <summary>
        /// Stops execution of commands in the processing queue immediately. Clears the processing queue.
        /// </summary>
        public void StopExecution()
        {
            Commands.Clear();
            ChangeRobotStatus("Stopped execution of command queue.");
        }

        # endregion
    }
}