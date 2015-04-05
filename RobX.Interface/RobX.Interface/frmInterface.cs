# region Includes

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using RobX.Interface.Properties;
using RobX.Library.Commons;
using RobX.Library.Communication;
using RobX.Library.Communication.COM;
using RobX.Library.Communication.TCP;
using RobX.Library.Tools;

# endregion

namespace RobX.Interface
{
    /// <summary>
    /// This class defines the visual form for hardware interface of the robot.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public partial class frmInterface : Form
    {
        # region Private Fields

        private readonly Log _communicationLog = new Log();
        private readonly TCPServer _server = new TCPServer();
        private readonly ComClient _robot = new ComClient();
        private List<ComPort> _comPorts;

        # endregion

        # region Form Constructor and Events

        /// <summary>
        /// This is the constructor for the form class of the hardware interface of the robot.
        /// </summary>
        public frmInterface()
        {
            InitializeComponent();
        }

        private void frmLog_Load(object sender, EventArgs e)
        {
            // Reload last used settings
            LoadProperties();
            frmLog_Resize(sender, e);
            cboCOMPorts.Select();

            _communicationLog.ItemsAdded += lstLogAddItems;
            _communicationLog.LogCleared += lstLogClear;
            _server.ReceivedData += TcpReceivedData;
            _server.SentData += TcpSentData;
            _server.StatusChanged += TcpStatusChanged;
            _robot.ReceivedData += RobotReceivedData;
            _robot.SentData += RobotSentData;
            _robot.StatusChanged += RobotStatusChanged;
            btnRefresh_Click(sender, e);
        }

        private void frmLog_KeyDown(object sender, KeyEventArgs e)
        {
            HandleHotKeys(e);
        }

        private void frmLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveProperties();

            // Stop robot!
            _robot.SendData(new byte[] { 0x00, 0x34, 0x00, 0x00, 0x31, 128, 0x00, 0x32, 128 });
        }

        private void frmLog_Resize(object sender, EventArgs e)
        {
            const int spacing = 6;
            tabController.Height = pnlProperties.Top;

            cmdConnect.Left = pnlProperties.Width - cmdConnect.Width - spacing;
            cmdRefresh.Left = cmdConnect.Left - cmdRefresh.Width - spacing;
            cboCOMPorts.Width = cmdRefresh.Left - cboCOMPorts.Left - spacing;
            cmdStartServer.Left = cmdConnect.Left;

            const int timeColWidth = 80;
            colLogTime.Width = timeColWidth;
            colLogText.Width = lstLog.ClientSize.Width - colLogTime.Width - 5;
        }

        # endregion

        # region Communication Events 

        private void TcpReceivedData(object sender, CommunicationEventArgs e)
        {
            _communicationLog.AddBytes(e.Data, Log.LogItem.LogItemTypes.Receive);
            _robot.SendData(e.Data);
        }

        private void TcpSentData(object sender, CommunicationEventArgs e)
        {
            _communicationLog.AddBytes(e.Data, Log.LogItem.LogItemTypes.Send);
        }

        private void TcpStatusChanged(object sender, CommunicationStatusEventArgs e)
        {
            _communicationLog.AddItem(e.Status, true);
        }

        private void RobotReceivedData(object sender, CommunicationEventArgs e)
        {
            _communicationLog.AddBytes(e.Data, Log.LogItem.LogItemTypes.Receive);
            _server.SendData(e.Data);
        }

        private void RobotSentData(object sender, CommunicationEventArgs e)
        {
            _communicationLog.AddBytes(e.Data, Log.LogItem.LogItemTypes.Send);
        }

        private void RobotStatusChanged(object sender, CommunicationStatusEventArgs e)
        {
            _communicationLog.AddItem(e.Status, true);
        }

        # endregion

        # region Private Functions

        private bool CheckInputErrors(bool checkServerPort = true, bool checkComPort = true)
        {
            var serverPortValid = Methods.IsValidPort(txtServerPort.Text);
            if (checkServerPort)
                if (serverPortValid == false)
                    _communicationLog.AddItem("Input Error! Invalid TCP port number!", true);

            if (!checkComPort || cboCOMPorts.Items.Count != 0)
                return (serverPortValid || !checkServerPort);

            _communicationLog.AddItem("Error! No COM devices are connected to the system!", true);
            return false;
        }

        private bool Connect()
        {
            if (!CheckInputErrors(false)) return false;

            if (_robot.Connect(_comPorts[cboCOMPorts.SelectedIndex].Name, (int)Robot.BaudRate,
                Robot.DataBits, Robot.Parity, Robot.StopBits) == false)
            {
                _communicationLog.AddItem("Error! Selected COM port is busy right now!", true);
                return false;
            }

            return StartServer();
        }

        private bool StartServer()
        {
            if (!CheckInputErrors(true, false)) return false;
            if (_server.StartServer(Int32.Parse(txtServerPort.Text))) return true;
            _communicationLog.AddItem("Error! Could not start TCP Server on port " + txtServerPort.Text + "!", true);
            return false;
        }

        private void SaveProperties()
        {
            if (cboCOMPorts.Items.Count > 0)
                Settings.Default.COMPort = _comPorts[cboCOMPorts.SelectedIndex].Name;
            Settings.Default.ServerPort = txtServerPort.Text;
            Settings.Default.FormPosition = Location;
            Settings.Default.FormSize = Size;
            Settings.Default.Save();
        }

        private void LoadProperties()
        {
            Size = Settings.Default.FormSize;
            Location = Settings.Default.FormPosition;
            txtServerPort.Text = Settings.Default.ServerPort;
        }

        private void HandleHotKeys(KeyEventArgs e)
        {
            if (e.KeyCode == Settings.Default.ToggleKeyboardControl)
                chkKeyboardControl.Checked = !chkKeyboardControl.Checked;
            else if (e.KeyCode == Keys.Escape)
                Application.Exit();

            if (!chkKeyboardControl.Checked && e.KeyCode != Settings.Default.GlobalStopKey) return;

            if (e.KeyCode == Settings.Default.ForwardKey || e.KeyCode == Settings.Default.BackwardKey ||
                e.KeyCode == Settings.Default.RotateClockwiseKey || e.KeyCode == Settings.Default.StopKey ||
                e.KeyCode == Settings.Default.RotateCounterClockwiseKey)
            {
                // Set robot mode to 0 and enable timeout
                _robot.SendData(new byte[] { 0x00, 0x34, 0x00, 0x00, 0x39 });
            }

            if (e.KeyCode == Settings.Default.ForwardKey)
                _robot.SendData(new byte[] { 0x00, 0x31, 148, 0x00, 0x32, 148 });
            else if (e.KeyCode == Settings.Default.BackwardKey)
                _robot.SendData(new byte[] { 0x00, 0x31, 108, 0x00, 0x32, 108 });
            else if (e.KeyCode == Settings.Default.RotateClockwiseKey)
                _robot.SendData(new byte[] { 0x00, 0x31, 148, 0x00, 0x32, 108 });
            else if (e.KeyCode == Settings.Default.RotateCounterClockwiseKey)
                _robot.SendData(new byte[] { 0x00, 0x31, 108, 0x00, 0x32, 148 });
            else if (e.KeyCode == Settings.Default.StopKey || e.KeyCode == Settings.Default.GlobalStopKey)
                _robot.SendData(new byte[] { 0x00, 0x31, 128, 0x00, 0x32, 128 });

            e.SuppressKeyPress = true;
        }

        # endregion

        # region Form Component Events

        private void cmdConnect_Click(object sender, EventArgs e)
        {
            SaveProperties();
            if (!Connect()) return;

            cmdConnect.Text = @"Re&connect";
            cmdStartServer.Visible = true;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            _comPorts = ComPort.GetComPorts();
            cboCOMPorts.Items.Clear();

            var lastComPort = _comPorts.Count - 1;
            foreach (var comPort in _comPorts)
            {
                cboCOMPorts.Items.Add(string.Format("{0} – {1}", comPort.Name, comPort.Description));
                if (comPort.Name == Settings.Default.COMPort)
                    lastComPort = cboCOMPorts.Items.Count - 1;
            }
            cboCOMPorts.SelectedIndex = lastComPort;
            
            if (cboCOMPorts.Items.Count > 0)
                Settings.Default.COMPort = _comPorts[cboCOMPorts.SelectedIndex].Name;
        }

        private void txtServerPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            (sender as TextBox).ValidateInput_TCPPort(e);
        }

        // ReSharper disable once InconsistentNaming
        private void lstLogAddItems(object sender, LogEventArgs e)
        {
            foreach (var item in e.Items)
                lstLog.AddLogItem(item);
        }

        // ReSharper disable once InconsistentNaming
        private void lstLogClear(object sender, LogEventArgs e)
        {
            lstLog.ClearItems();
        }

        private void SaveLog(object sender, KeyEventArgs e)
        {
            (sender as ListView).SaveListView_CtrlS(e);
        }

        private void cmdStartServer_Click(object sender, EventArgs e)
        {
            SaveProperties();
            StartServer();
        }

        # endregion
    }
}
