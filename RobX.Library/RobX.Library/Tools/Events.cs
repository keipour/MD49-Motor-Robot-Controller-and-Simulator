# region Includes

using System;
using System.Collections.Generic;

# endregion

namespace RobX.Library.Tools
{
    # region Event Handler Delegate

    /// <summary>
    /// Event handler delegate for Log events.
    /// </summary>
    /// <param name="sender">Current instance of Log class.</param>
    /// <param name="e">Event argument that contains the event parameters.</param>
    public delegate void LogEventHandler(object sender, LogEventArgs e);

    # endregion

    # region Event Argument

    /// <summary>
    /// Event argument for log class that can contain a text and a LogItem instances.
    /// </summary>
    public class LogEventArgs : EventArgs
    {
        /// <summary>
        /// Text added to the log.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// LogItems added to the log.
        /// </summary>
        public List<Log.LogItem> Items { get; private set; }

        /// <summary>
        /// Constructor for LogEventArgs event argument class.
        /// </summary>
        /// <param name="text">New text added to the log.</param>
        /// <param name="items">New items added to the log.</param>
        public LogEventArgs(string text, List<Log.LogItem> items)
        {
            Text = text;
            Items = items;
        }
    }

    # endregion
}
