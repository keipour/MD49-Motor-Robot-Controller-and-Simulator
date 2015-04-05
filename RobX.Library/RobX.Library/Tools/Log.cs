﻿# region Includes

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

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
        /// Complete type of the log.
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
        /// Adds an item to the end of current log with a specified type.
        /// </summary>
        /// <param name="itemText">String of the item that should be added to the log.</param>
        /// <param name="type">Color of the new item.</param>
        /// <param name="addTime">If true, adds current time to the beginning of the new line.</param>
        public void AddItem(string itemText, Color type, bool addTime = false)
        {
            if (addTime)
            {
                Text += "[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] ";
                _newText += "[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] ";
            }

            Text += itemText + Environment.NewLine;
            _newText += itemText + Environment.NewLine;

            _newItems.Add(new LogItem(itemText, type, addTime));
            CallItemsAddedEvent();
        }

        /// <summary>
        /// Adds an item to the end of current log with a specified item type.
        /// </summary>
        /// <param name="itemText">String of the item that should be added to the log.</param>
        /// <param name="type">Type of the new item.</param>
        /// <param name="addTime">If true, adds current time to the beginning of the new line.</param>
        public void AddItem(string itemText, LogItem.LogItemTypes type, bool addTime = false)
        {
            AddItem(itemText, LogItem.DetermineTypeColor(type), addTime);
        }

        /// <summary>
        /// Adds an item to the end of current log with auto-coloring.
        /// </summary>
        /// <param name="itemText">String of the item that should be added to the log.</param>
        /// <param name="addTime">If true, adds current time to the beginning of the new line.</param>
        public void AddItem(string itemText = "", bool addTime = false)
        {
            AddItem(itemText, LogItem.DetermineTypeColor(LogItem.DetermineItemType(itemText)), addTime);
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
        /// Adds an item containing an array of bytes (hex and decimal representations) to the end of current log 
        /// with a specified color.
        /// </summary>
        /// <param name="bytes">The array that should be added to the log.</param>
        /// <param name="color">Color of the new log item.</param>
        public void AddBytes(byte[] bytes, Color color)
        {
            var byteText = bytes.Aggregate("", (current, t) => current + ("0x" + t.ToString("X2") + "(" + t + ") "));
            AddItem(byteText, color);
        }

        /// <summary>
        /// Adds an item containing an array of bytes (hex and decimal representations) to the end of current log 
        /// with a specified log item type.
        /// </summary>
        /// <param name="bytes">The array that should be added to the log.</param>
        /// <param name="type">Type of the new log item.</param>
        public void AddBytes(byte[] bytes, LogItem.LogItemTypes type)
        {
            AddBytes(bytes, LogItem.DetermineTypeColor(type));
        }

        /// <summary>
        /// Adds an item containing an array of bytes (hex and decimal representations) to the end of current log 
        /// with auto-coloring.
        /// </summary>
        /// <param name="bytes">The array that should be added to the log.</param>
        public void AddBytes(byte[] bytes)
        {
            AddBytes(bytes, LogItem.DetermineTypeColor(LogItem.LogItemTypes.Default));
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

            /// <summary>
            /// Color of the item.
            /// </summary>
            public Color Color;

            # endregion

            # region Private Methods

            /// <summary>
            /// Assigns the inputs to the public fields of the LogItem instance.
            /// </summary>
            /// <param name="text">Text of the log item.</param>
            /// <param name="color">Color of the log item.</param>
            /// <param name="showTime">If true, shows item's creation time when printing item.</param>
            private void CreateLogItem(string text, Color color, bool showTime)
            {
                ShowTime = showTime;
                Text = text;
                Time = DateTime.Now;
                Color = color;
            }

            # endregion

            # region Contructors

            /// <summary>
            /// Constructor for the LogItem class with a specified type.
            /// </summary>
            /// <param name="text">Text of the log item.</param>
            /// <param name="color">Color of the log item.</param>
            /// <param name="showTime">If true, shows item's creation time when printing item.</param>
            public LogItem(string text, Color color, bool showTime = false)
            {
                CreateLogItem(text, color, showTime);
            }

            /// <summary>
            /// Constructor for the LogItem class with a specified type.
            /// </summary>
            /// <param name="text">Text of the log item.</param>
            /// <param name="type">Type of the log item.</param>
            /// <param name="showTime">If true, shows item's creation time when printing item.</param>
            public LogItem(string text, LogItemTypes type, bool showTime = false)
            {
                CreateLogItem(text, DetermineTypeColor(type), showTime);
            }

            /// <summary>
            /// Constructor for the LogItem class with auto-coloring.
            /// </summary>
            /// <param name="text">Text of the log item.</param>
            /// <param name="showTime">If true, shows item's creation time when printing item.</param>
            public LogItem(string text = "", bool showTime = false)
            {
                CreateLogItem(text, DetermineTypeColor(DetermineItemType(text)), showTime);
            }

            # endregion
            
            # region Public Static Methods

            /// <summary>
            /// Determines the type of a string (used for auto-coloring).
            /// </summary>
            /// <param name="text">The input string.</param>
            /// <returns>The type of the input string.</returns>
            public static LogItemTypes DetermineItemType(string text)
            {
                text = text.ToLower();

                if (text.Contains("error"))
                    return LogItemTypes.Error;
                
                if (text.Contains("receive"))
                    return LogItemTypes.Receive;
                
                if (text.Contains("send") || text.Contains("sent"))
                    return LogItemTypes.Send;
                
                return LogItemTypes.Default;
            }

            /// <summary>
            /// Returns the type associated with a log item type.
            /// </summary>
            /// <param name="type">The input log item type.</param>
            /// <returns>The type associated with the input type.</returns>
            public static Color DetermineTypeColor(LogItemTypes type)
            {
                switch (type)
                {
                    case LogItemTypes.Error:
                        return Properties.Settings.Default.ErrorLogItemColor;
                    case LogItemTypes.Receive:
                        return Properties.Settings.Default.ReceiveLogItemColor;
                    case LogItemTypes.Send:
                        return Properties.Settings.Default.SendLogItemColor;
                }

                return Properties.Settings.Default.DefaultLogItemColor;
            }

            # endregion

            # region Enum (LogItemTypes)

            /// <summary>
            /// Types of log item (used for auto-coloring).
            /// </summary>
            public enum LogItemTypes
            {
                /// <summary>
                /// Default type of an item.
                /// </summary>
                Default,

                /// <summary>
                /// Color of error items.
                /// </summary>
                Error, 

                /// <summary>
                /// Color of items indicating receiving data.
                /// </summary>
                Receive,

                /// <summary>
                /// Color of items indicating sending data.
                /// </summary>
                Send
            }

            #endregion
        }
    }

    /// <summary>
    /// This class adds extension function for adding Log items in a ListView control.
    /// </summary>
    public static class LogExtension
    {
        # region ListView Extension For Log

        /// <summary>
        /// Adds a new item to the end of the ListView control.
        /// </summary>
        /// <param name="listView">The ListView instance to which the new item should be added.</param>
        /// <param name="item">The new item that should be added.</param>
        public static void AddLogItem(this ListView listView, Log.LogItem item)
        {
            var timestring = item.ShowTime ? "[" + item.Time.ToString("HH:mm:ss.fff") + "]" : String.Empty;
            var lines = item.Text.Replace("\r", String.Empty).Split('\n');

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var line in lines)
            {
                var listItem = new ListViewItem(new[] {timestring, line}) {ForeColor = item.Color};
                Commons.Extensions.ListAddItemPrivate(listView, listItem);
                timestring = String.Empty;
            }
        }

        # endregion
    }
}
