# region Includes

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

# endregion

namespace RobX.Library.Commons
{
    /// <summary>
    /// This class is used to add extension functions to the globally-used classes in the project.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class Extensions
    {
        # region TextBox Add Line Extension

        private delegate void SetTextCallback(TextBoxBase textBox, string line);
        private static void AddLinePrivate(TextBoxBase textBox, string line)
        {
            try
            {
                if (textBox.InvokeRequired)
                {
                    var d = new SetTextCallback(AddLinePrivate);
                    textBox.Invoke(d, textBox, line);
                }
                else
                {
                    if (textBox.Text == line) return;

                    textBox.Text += line + Environment.NewLine;
                    textBox.Select(textBox.Text.Length, 0);
                    textBox.ScrollToCaret();
                }
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Adds a line of text to the end of the text of a TextBox control.
        /// </summary>
        /// <param name="textBox">The textbox instance to which the line should be added.</param>
        /// <param name="line">The string that should be added as a line to the end of the TextBox text.</param>
        public static void AddLine(this TextBoxBase textBox, string line)
        {
            AddLinePrivate(textBox, line);
        }

        # endregion

        # region TextBox Update Text Extension

        private static void UpdateTextPrivate(TextBoxBase textBox, string text)
        {
            try
            {
                if (textBox.InvokeRequired)
                {
                    var d = new SetTextCallback(AddLinePrivate);
                    textBox.Invoke(d, textBox, text);
                }
                else
                {
                    if (textBox.Text == text) return;

                    textBox.Text = text;
                    textBox.Select(textBox.Text.Length, 0);
                    textBox.ScrollToCaret();
                }
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Updates text of a TextBox control.
        /// </summary>
        /// <param name="textBox">The textbox instance which should be updated.</param>
        /// <param name="text">The text that should be put in the TextBox.</param>
        public static void UpdateText(this TextBoxBase textBox, string text)
        {
            UpdateTextPrivate(textBox, text);
        }

        # endregion

        # region TextBox KeyPress Validation Extensions

        /// <summary>
        /// Validates the input key pressed by user so that the text is always valid as part of a TCP port number.
        /// </summary>
        /// <param name="textBox">The textbox instance which should be validated.</param>
        /// <param name="e">Key press event argument (contains information about the pressed key).</param>
        // ReSharper disable once UnusedParameter.Global
        public static void ValidateInput_TCPPort(this TextBox textBox, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar)) return;
            if (char.IsControl(e.KeyChar)) return;
            e.Handled = true;
        }

        /// <summary>
        /// Validates the input key pressed by user so that the text is always valid as part of a IP address.
        /// </summary>
        /// <param name="textBox">The textbox instance which should be validated.</param>
        /// <param name="e">Key press event argument (contains information about the pressed key).</param>
        public static void ValidateInput_IPAddress(this TextBox textBox, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;

            var str = textBox.Text + e.KeyChar;
            var dotCount = str.Count(c => c == '.');

            if (e.KeyChar == '.')
                str += "0";

            for (var i = dotCount; i < 3; ++i)
                str += ".0";

            IPAddress ip;
            if (IPAddress.TryParse(str, out ip) == false)
                e.Handled = true;
        }

        /// <summary>
        /// Validates the input key pressed by user so that the text is always valid as part of a double number.
        /// </summary>
        /// <param name="textBox">The textbox instance which should be validated.</param>
        /// <param name="e">Key press event argument (contains information about the pressed key).</param>
        public static void ValidateInput_Double(this TextBox textBox, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar)) return;
            if (char.IsControl(e.KeyChar)) return;
            if ((e.KeyChar == '.') && (textBox.Text.Contains(".") == false)) return;
            if ((e.KeyChar == '.') && (textBox.SelectionLength == textBox.TextLength)) return;
            e.Handled = true;
        }

        # endregion

        # region TextBox and ListView Save Extensions

        /// <summary>
        /// Saves the text of a textbox into file.
        /// </summary>
        /// <param name="textBox">The textbox instance which text should be saved.</param>
        /// <param name="e">Argument containing keys pressed by the user.</param>
        public static void SaveTextBox_CtrlS(this TextBox textBox, KeyEventArgs e)
        {
            if (!e.Control || e.KeyCode != Keys.S) return;
            
            var sfdSaveLog = new SaveFileDialog
            {
                Filter = @"Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                DefaultExt = "txt",
                SupportMultiDottedExtensions = true
            };
            
            if (sfdSaveLog.ShowDialog() == DialogResult.OK)
                File.WriteAllText(sfdSaveLog.FileName, textBox.Text);
            
            sfdSaveLog.Dispose();
        }

        /// <summary>
        /// Saves the text of a ListView into file.
        /// </summary>
        /// <param name="listView">The ListView instance which text should be saved.</param>
        /// <param name="e">Argument containing keys pressed by the user.</param>
        public static void SaveListView_CtrlS(this ListView listView, KeyEventArgs e)
        {
            if (!e.Control || e.KeyCode != Keys.S) return;

            var sfdSaveLog = new SaveFileDialog
            {
                Filter = @"Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                DefaultExt = "txt",
                SupportMultiDottedExtensions = true
            };

            if (sfdSaveLog.ShowDialog() == DialogResult.OK)
            {
                var text = "";
                foreach (ListViewItem item in listView.Items)
                {
                    for (var i = 0; i < item.SubItems.Count; ++i)
                    {
                        if (i > 0) text += "\t";
                        text += item.SubItems[i].Text;
                    }
                    text += Environment.NewLine;
                }

                File.WriteAllText(sfdSaveLog.FileName, text);
            }

            sfdSaveLog.Dispose();
        }

        # endregion

        # region ListView Extensions

        private delegate void SetListCallback(ListView listView, ListViewItem item);

        internal static void ListAddItemPrivate(ListView listView, ListViewItem item)
        {
            try
            {
                if (listView.InvokeRequired)
                {
                    var d = new SetListCallback(ListAddItemPrivate);
                    listView.Invoke(d, listView, item);
                }
                else
                {
                    listView.Items.Add(item);
                    listView.TopItem = listView.Items[listView.Items.Count - 1];
                }
            }
            catch
            {
                // ignored
            }
        }

        private delegate void SetListLogClearCallback(ListView listView);
        private static void ClearItemsPrivate(ListView listView)
        {
            try
            {
                if (listView.InvokeRequired)
                {
                    var d = new SetListLogClearCallback(ClearItemsPrivate);
                    listView.Invoke(d, listView);
                }
                else
                    listView.Items.Clear();
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Clears all items in the ListView control.
        /// </summary>
        /// <param name="listView">The ListView instance which should be cleared.</param>
        public static void ClearItems(this ListView listView)
        {
            ClearItemsPrivate(listView);
        }

        # endregion
    }
}
