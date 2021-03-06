﻿# region Includes

using System;
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

        # region Public Variables

        /// <summary>
        /// This property returns the number of commands in the command processing queue.
        /// </summary>
        public int Count
        {
            get { return _commands.Count; }
        }

        # endregion

        # region Public Queue Functions

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

        # region Public Functions

        /// <summary>
        /// Adds commands to the queue by parsing a string containing a list of commands.
        /// Each command should be in a separate line.
        /// </summary>
        /// <param name="commandList">Command list.</param>
        /// <param name="skipErrors">If true, skips command lines that contain errors; otherwise throws an exception 
        /// indicating the kind of error occured during the parsing of the commands.</param>
        public void AddCommandsFromString(string commandList, bool skipErrors = true)
        {
            var lines = commandList.Split(new[] {"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                if (skipErrors)
                {
                    Command cmd;
                    if (Command.TryParse(line, out cmd))
                        Enqueue(cmd);
                }
                else
                    Enqueue(Command.Parse(line));
            }
        }

        # endregion
    }
}
