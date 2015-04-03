﻿# region Includes

using System;
using System.Collections.Generic;
using System.Linq;

# endregion

namespace RobX.Library.Tools
{
    /// <summary>
    /// Class to maintain logs.
    /// </summary>
    public class Log
    {
        # region Private Fields

        string _newText = "";
        readonly List<LogItem> _newItems = new List<LogItem>();

        # endregion

        # region Public Events

        /// <summary>
        /// This event is invoked when a new item is added to the log.
        /// </summary>
        public event LogEventHandler ItemsAdded;

        /// <summary>
        /// Is invoked when the entire log is cleared.
        /// </summary>
        public event LogEventHandler LogCleared;

        # endregion

        # region Public Fields

        /// <summary>
        /// Complete text of the log.
        /// </summary>
        public string Text = "";

        /// <summary>
        /// List of items in the log.
        /// </summary>
        public List<LogItem> Items = new List<LogItem>();

        # endregion

        # region Private Methods

        /// <summary>
        /// Invokes ItemsAdded event (is used when a new item is added to the log).
        /// </summary>
        private void CallItemsAddedEvent()
        {
            if (ItemsAdded != null)
                ItemsAdded(this, new LogEventArgs(_newText, _newItems));
            
            _newText = "";
            _newItems.Clear();
        }

        # endregion

        # region Public Methods

        /// <summary>
        /// Adds an item to the end of current log.
        /// </summary>
        /// <param name="itemText">String of the item that should be added to the log.</param>
        /// <param name="addTime">If true, adds current time to the beginning of the new line.</param>
        public void AddItem(string itemText = "", bool addTime = false)
        {
            if (addTime)
            {
                Text += "[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] ";
                _newText += "[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] ";
            }

            Text += itemText + Environment.NewLine;
            _newText += itemText + Environment.NewLine;

            _newItems.Add(new LogItem(itemText, addTime));
            CallItemsAddedEvent();
        }

        /// <summary>
        /// Clears the entire log.
        /// </summary>
        public void Clear()
        {
            Text = "";
            CallItemsAddedEvent();

            _newText = "";
            _newItems.Clear();

            if (LogCleared != null)
                LogCleared(this, new LogEventArgs(null, null));
        }

        /// <summary>
        /// Adds an item containing an array of bytes (hex and decimal representations) to the end of current log.
        /// </summary>
        /// <param name="bytes">The array that should be added to the log.</param>
        public void AddBytes(byte[] bytes)
        {
            var byteText = bytes.Aggregate("", (current, t) => current + ("0x" + t.ToString("X2") + "(" + t + ") "));
            AddItem(byteText);
        }

        # endregion

        /// <summary>
        /// Class to maintain a log item.
        /// </summary>
        public class LogItem
        {
            # region Public Fields

            /// <summary>
            /// Time that the log item created.
            /// </summary>
            public DateTime Time;

            /// <summary>
            /// If true, shows item's creation time when printing item.
            /// </summary>
            public bool ShowTime;

            /// <summary>
            /// Text of the log item.
            /// </summary>
            public string Text;

            # endregion

            # region Contructor

            /// <summary>
            /// Constructor for the LogItem class.
            /// </summary>
            /// <param name="text">Text of the log item.</param>
            /// <param name="showTime">If true, shows item's creation time when printing item.</param>
            public LogItem(string text, bool showTime = false)
            {
                ShowTime = showTime;
                Text = text;
                Time = DateTime.Now;
            }

            # endregion
        }
    }
}