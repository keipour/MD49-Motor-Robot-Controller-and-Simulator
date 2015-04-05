# region Includes

using System;
using System.Threading;
using System.Windows.Forms;
using RobX.Controller.Properties;
using RobX.Library.Commons;
using RobX.Library.Communication;
using RobX.Library.Tools;

# endregion

namespace RobX.Controller
{
    /// <summary>
    /// This class defines the visual form for controller of the robot.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public partial class frmController : Form
    {
        # region Private Fields

        private readonly Log _communicationLog = new Log();
        private readonly Log _messageLog = new Log();
        private Controller _controller = new Controller(Library.Commons.Robot.RobotType.Simulation);
        private Thread _executionThread;

        # endregion

        # region Form Methods and Events

        /// <summary>
        /// This is the constructor for the form class of the controller of the robot.
        /// </summary>
        public frmController()
        {
            InitializeComponent();
        }

        private void frmLog_Load(object sender, EventArgs e)
        {
            // Reload last used settings
            LoadProperties();
            frmLog_Resize(sender, e);
            txtIPAddress.Select();

            // Add events
            _communicationLog.ItemsAdded += lstLogAddItems;
            _communicationLog.LogCleared += lstLogClear;
            _messageLog.ItemsAdded += lstMessageAddItems;
            _messageLog.LogCleared += lstMessageClear;
            _controller.ReceivedData += RobotReceivedData;
            _controller.SentData += RobotSentData;
            _controller.CommunicationStatusChanged += RobotCommunicationStatusChanged;
            _controller.RobotStatusChanged += RobotStatusChanged;
        }

        private void frmLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveProperties();
        }

        private void frmLog_KeyDown(object sender, KeyEventArgs e)
        {
            HandleHotKeys(e);
        }

        private void frmLog_Resize(object sender, EventArgs e)
        {
            const int spacing = 6;
            lstMessage.Height = ClientSize.Height - pnlController.Height - tabController.Height;
            tabController.Top = lstMessage.Height + spacing;
            tabController.Width = ClientSize.Width;

            cmdStart.Left = pnlController.Width - cmdStart.Width - spacing;
            cmdConnect.Left = cmdStart.Left;
            cboRobotType.Width = cmdConnect.Left - cboRobotType.Left - spacing;

            const int timeColWidth = 80;
            colMessageTime.Width = timeColWidth;
            colMessageText.Width = lstMessage.ClientSize.Width - colMessageTime.Width - 5;
            colLogTime.Width = timeColWidth;
            colLogText.Width = lstLog.ClientSize.Width - colLogTime.Width - 5;
        }

        # endregion

        # region Private Methods

        private void HandleHotKeys(KeyEventArgs e)
        {
            if (e.KeyCode == Settings.Default.StartKey)
                cmdStart_Click(this, e);
            else if (e.KeyCode == Settings.Default.ToggleKeyboardControl)
                chkKeyboardControl.Checked = !chkKeyboardControl.Checked;
            else if (e.KeyCode == Keys.Escape)
                Application.Exit();

            if (!chkKeyboardControl.Checked && e.KeyCode != Settings.Default.GlobalStopKey) return;

            if (e.KeyCode == Settings.Default.ForwardKey || e.KeyCode == Settings.Default.BackwardKey ||
                e.KeyCode == Settings.Default.RotateClockwiseKey || e.KeyCode == Settings.Default.StopKey ||
                e.KeyCode == Settings.Default.RotateCounterClockwiseKey)
            {
                // Connect if is not connected yet
                if (_controller.IsConnected == false) return;

                // Stop current execution
                _controller.StopExecution();
            }


            if (e.KeyCode == Settings.Default.ForwardKey)
                _controller.MoveForwardMilliseconds(500, 20);
            else if (e.KeyCode == Settings.Default.BackwardKey)
                _controller.MoveBackwardMilliseconds(500, 20);
            else if (e.KeyCode == Settings.Default.RotateClockwiseKey)
                _controller.SetSpeedMilliseconds(500, 20, -20);
            else if (e.KeyCode == Settings.Default.RotateCounterClockwiseKey)
                _controller.SetSpeedMilliseconds(500, -20, 20);
            else if (e.KeyCode == Settings.Default.StopKey || e.KeyCode == Settings.Default.GlobalStopKey)
                _controller.StopRobot();

            e.SuppressKeyPress = true;
        }

        private void StartExecution()
        {
            // Return if still is not connected
            if (_controller.IsConnected == false) return;

            //Controller.Robot.ResetEncoders();

            //txtMessage.AddLine("Encoder 1: " + Controller.Robot.GetEncoder1());
            //txtMessage.AddLine("Encoder 2: " + Controller.Robot.GetEncoder2());

            //Controller.Robot.SetSpeeds(148, 148);

            //Thread.Sleep(4000);

            //int enc1 = Controller.Robot.GetVolts();
            //int enc2 = Controller.Robot.GetVolts();

            //txtMessage.AddLine("Encoder 1: " + enc1.ToString());
            //txtMessage.AddLine("Encoder 2: " + enc2.ToString());

            UserCommands.AddCommands(ref _controller);

            _controller.ExecuteCommandQueue();
        }

        private bool CheckInputErrors()
        {
            var ipValid = Methods.IsValidIpAddress(txtIPAddress.Text);

            if (ipValid == false)
                _messageLog.AddItem("Invalid IP address format!", Log.LogItem.LogItemTypes.Error);

            if (Methods.IsValidPort(txtPort.Text)) return ipValid;

            _messageLog.AddItem("Invalid TCP port number!", Log.LogItem.LogItemTypes.Error);
            return false;
        }

        private void Connect()
        {
            SaveProperties();
            if (!CheckInputErrors()) return;
            _controller.Connect(txtIPAddress.Text, Int32.Parse(txtPort.Text));
        }

        private void LoadProperties()
        {
            Size = Settings.Default.FormSize;
            Location = Settings.Default.FormPosition;
            txtSimSpeed.Text = Settings.Default.SimulationSpeed.ToString("0.0");

            if (Settings.Default.RobotType == 1)
            {
                txtIPAddress.Text = Settings.Default.SimulatorIP;
                txtPort.Text = Settings.Default.SimulatorPort;
            }
            else
            {
                txtIPAddress.Text = Settings.Default.RealIP;
                txtPort.Text = Settings.Default.RealPort;
            }
            cboRobotType.SelectedIndex = Settings.Default.RobotType;
        }

        private void SaveProperties()
        {
            if (cboRobotType.SelectedIndex == 0) // if simulation
            {
                Settings.Default.SimulatorIP = txtIPAddress.Text;
                Settings.Default.SimulatorPort = txtPort.Text;
            }
            else
            {
                Settings.Default.RealIP = txtIPAddress.Text;
                Settings.Default.RealPort = txtPort.Text;
            }
            Settings.Default.SimulationSpeed = double.Parse(txtSimSpeed.Text);
            Settings.Default.RobotType = cboRobotType.SelectedIndex;
            Settings.Default.FormPosition = Location;
            Settings.Default.FormSize = Size;
            Settings.Default.Save();
        }

        # endregion

        #region Communication Events

        private void RobotReceivedData(object sender, CommunicationEventArgs e)
        {
            _communicationLog.AddBytes(e.Data, Log.LogItem.LogItemTypes.Receive);
        }

        private void RobotSentData(object sender, CommunicationEventArgs e)
        {
            _communicationLog.AddBytes(e.Data, Log.LogItem.LogItemTypes.Send);
        }

        private void RobotCommunicationStatusChanged(object sender, CommunicationStatusEventArgs e)
        {
            _communicationLog.AddItem(e.Status, true);
        }

        private void RobotStatusChanged(object sender, CommunicationStatusEventArgs e)
        {
            _messageLog.AddItem(e.Status, true);
        }

        # endregion

        # region Form Component Events

        private void cmdStart_Click(object sender, EventArgs e)
        {
            SaveProperties();

            if (_controller.IsConnected == false)
                cmdConnect_Click(sender, e);

            // Define execution thread
            if (_executionThread != null)
            {
                _controller.StopExecution();
                _executionThread.Abort();
            }

            _executionThread = new Thread(StartExecution) {IsBackground = true};

            var simSpeed = (ushort) (Math.Round(Settings.Default.SimulationSpeed*10));
            if (simSpeed != 10)
                _controller.SetSimulationSpeed(simSpeed);
            _executionThread.Start();
        }

        private void cmdConnect_Click(object sender, EventArgs e)
        {
            Connect();

            //TimeSpan TotalDelay = TimeSpan.Zero;
            //int NumOfTestPackets = 5;
            //for (int i = 0; i < NumOfTestPackets; ++i)
            //{
            //    TimeSpan Delay = Controller.CalculateDelay();
            //    TotalDelay += Delay;
            //    txtMessage.AddLine("Delay for packet " + i.ToString() + ": " + Delay.TotalMilliseconds.ToString("0") + " ms");
            //}
            //txtMessage.AddLine("Average delay : " + 
            //    TimeSpan.FromTicks(TotalDelay.Ticks / NumOfTestPackets).TotalMilliseconds.ToString("0.0") + " ms");
        }

        private void cboRobotType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboRobotType.SelectedIndex == 0) // if simulation
            {
                Settings.Default.RealIP = txtIPAddress.Text;
                Settings.Default.RealPort = txtPort.Text;
                txtIPAddress.Text = Settings.Default.SimulatorIP;
                txtPort.Text = Settings.Default.SimulatorPort;
            }
            else
            {
                Settings.Default.SimulatorIP = txtIPAddress.Text;
                Settings.Default.SimulatorPort = txtPort.Text;
                txtIPAddress.Text = Settings.Default.RealIP;
                txtPort.Text = Settings.Default.RealPort;
            }
            Settings.Default.RobotType = cboRobotType.SelectedIndex;
        }

        private void txtPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            (sender as TextBox).ValidateInput_TCPPort(e);
        }

        private void txtSimSpeed_KeyPress(object sender, KeyPressEventArgs e)
        {
            (sender as TextBox).ValidateInput_Double(e);
        }

        private void txtIPAddress_KeyPress(object sender, KeyPressEventArgs e)
        {
            (sender as TextBox).ValidateInput_IPAddress(e);
        }

        // ReSharper disable once InconsistentNaming
        private void lstMessageAddItems(object sender, LogEventArgs e)
        {
            foreach (var item in e.Items)
                lstMessage.AddLogItem(item);
        }

        // ReSharper disable once InconsistentNaming
        private void lstMessageClear(object sender, LogEventArgs e)
        {
            lstMessage.ClearItems();
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

        # endregion
    }
}