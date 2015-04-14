# region Includes

using System.Collections.Generic;

# endregion

// ReSharper disable UnusedMember.Global
namespace RobX.Library.Robot
{
    /// <summary>
    /// Class that implements a queue for maintaining robot commands.
    /// </summary>
    public class CommandQueue
    {
        # region Private Variables

        /// <summary>
        /// Contains commands in queue for processing.
        /// </summary>
        private readonly LinkedList<Command> _commands = new LinkedList<Command>();

        private readonly object _commandsLock = new object();

        # endregion

        # region Public Functions

        /// <summary>
        /// Adds a command to the end of command processing queue.
        /// </summary>
        /// <param name="cmd">Command to be added.</param>
        public void Enqueue(Command cmd)
        {
            lock (_commandsLock)
                _commands.AddLast(cmd);
        }

        /// <summary>
        /// Removes all commands from the command processing queue.
        /// </summary>
        public void Clear()
        {
            lock (_commandsLock)
            {
                if (_commands.Count == 0) return;
                _commands.Clear();
            }
        }

        /// <summary>
        /// Returns the front command in the queue.
        /// </summary>
        /// <returns>The front command in the queue. Returns null if the queue is empty.</returns>
        public Command Front()
        {
            lock (_commandsLock)
            {
                // ReSharper disable once ConvertIfStatementToReturnStatement
                if (_commands.Count == 0) return null;
                return _commands.First.Value;
            }
        }

        /// <summary>
        /// Returns the front command in the queue and removes it from the queue.
        /// </summary>
        /// <returns>The front command in the queue before removal. Returns null if the queue is empty.</returns>
        public Command Dequeue()
        {
            lock (_commandsLock)
            {
                if (_commands.Count == 0) return null;
                var cmd = _commands.First.Value;
                _commands.RemoveFirst();
                return cmd;
            }
        }

        # endregion
    }
}
