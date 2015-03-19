# region Includes

using System;
using System.Collections.Generic;

# endregion

namespace RobX.Tools
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
        private string mText = "";
        private List<Log.LogItem> mItems = new List<Log.LogItem>();

        /// <summary>
        /// Text added to the log.
        /// </summary>
        public string Text { get { return mText; } }

        /// <summary>
        /// LogItems added to the log.
        /// </summary>
        public List<Log.LogItem> Items { get { return mItems; } }

        /// <summary>
        /// Constructor for LogEventArgs event argument class.
        /// </summary>
        /// <param name="text">New text added to the log.</param>
        /// <param name="items">New items added to the log.</param>
        public LogEventArgs(string text, List<Log.LogItem> items)
        {
            mText = text;
            mItems = items;
        }
    }

    # endregion
}
