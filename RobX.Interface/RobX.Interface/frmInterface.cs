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
    public partial class frmInterface : Form
    {
        private Log CommunicationLog = new Log();
        private TCPServer Server = new TCPServer();
        private ComClient Robot = new ComClient();

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

            CommunicationLog.ItemsAdded += UpdateTextBox;
            CommunicationLog.LogCleared += UpdateTextBox;
            Server.ReceivedData += TCPReceivedData;
            Server.SentData += TCPSentData;
            Server.StatusChanged += TCPStatusChanged;
            Robot.ReceivedData += RobotReceivedData;
            Robot.SentData += RobotSentData;
            Robot.StatusChanged += RobotStatusChanged;
            btnRefresh_Click(sender, e);
        }

        // ---------------------------------------- Communication Events ------------------------------- //

        private void TCPReceivedData(object sender, CommunicationEventArgs e)
        {
            CommunicationLog.AddBytes(e.Data);
            Robot.SendData(e.Data);
        }

        private void TCPSentData(object sender, CommunicationEventArgs e)
        {
            CommunicationLog.AddBytes(e.Data);
        }

        private void TCPStatusChanged(object sender, CommunicationStatusEventArgs e)
        {
            CommunicationLog.AddItem(e.Status, true);
        }

        private void RobotReceivedData(object sender, CommunicationEventArgs e)
        {
            CommunicationLog.AddBytes(e.Data);
            Server.SendData(e.Data);
        }

        private void RobotSentData(object sender, CommunicationEventArgs e)
        {
            CommunicationLog.AddBytes(e.Data);
        }

        private void RobotStatusChanged(object sender, CommunicationStatusEventArgs e)
        {
            CommunicationLog.AddItem(e.Status, true);
        }

        // ----------------------------------------------- Log Events ---------------------------------- //

        private delegate void SetTextCallback(string str);
        private void UpdateTextBox(string LogText)
        {
            if (txtLog.InvokeRequired)
            {
                var d = new SetTextCallback(UpdateTextBox);
                Invoke(d, LogText);
            }
            else
            {
                if (txtLog.Text != LogText)
                {
                    txtLog.Text = LogText;
                    txtLog.Select(txtLog.Text.Length, 0);
                    txtLog.ScrollToCaret();
                }
            }
        }

        private void UpdateTextBox(object sender, LogEventArgs e)
        {
            UpdateTextBox(CommunicationLog.Text);
        }

        // ------------------------------------------ Private Functions -------------------------------- //

        private void frmLog_KeyDown(object sender, KeyEventArgs e)
        {
            HandleHotKeys(e);
        }

        private bool CheckInputErrors()
        {
            var ServerPortValid = true;
            var COMPortValid = true;

            ushort port;
            if (ushort.TryParse(txtServerPort.Text, out port) == false || port < 2)
            {
                txtMessage.AddLine("Error! Invalid server port number!");
                ServerPortValid = false;
            }

            if (cboCOMPorts.Items.Count == 0)
            {
                txtMessage.AddLine("Error! No COM devices are connected to the system!");
                COMPortValid = false;
            }

            return ServerPortValid && COMPortValid;
        }

        private void cmdConnect_Click(object sender, EventArgs e)
        {
            SaveProperties();
            Connect();
        }

        private bool Connect()
        {
            if (CheckInputErrors())
            {
                if (Robot.Connect(COMPorts[cboCOMPorts.SelectedIndex].Name, (int)Library.Commons.Robot.BaudRate,
                    Library.Commons.Robot.DataBits, Library.Commons.Robot.Parity, Library.Commons.Robot.StopBits) == false)
                {
                    txtMessage.AddLine("Error! No COM devices are connected to the system!");
                    return false;
                }
                if (Server.StartServer(Int32.Parse(txtServerPort.Text)) == false)
                {
                    txtMessage.AddLine("Error! Could not start TCP Server on port " + txtServerPort.Text + "!");
                    return false;
                }
                return true;
            }
            return false;
        }

        private List<ComPort> COMPorts;
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            COMPorts = ComPort.GetComPorts();
            cboCOMPorts.Items.Clear();

            var LastCOMPort = COMPorts.Count - 1;
            foreach (var comPort in COMPorts)
            {
                cboCOMPorts.Items.Add(string.Format("{0} – {1}", comPort.Name, comPort.Description));
                if (comPort.Name == Settings.Default.COMPort)
                    LastCOMPort = cboCOMPorts.Items.Count - 1;
            }
            cboCOMPorts.SelectedIndex = LastCOMPort;
            
            if (cboCOMPorts.Items.Count > 0)
                Settings.Default.COMPort = COMPorts[cboCOMPorts.SelectedIndex].Name;
        }

        private void frmLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveProperties();

            // Stop robot!
            Robot.SendData(new byte[] { 0x00, 0x34, 0x00, 0x00, 0x31, 128, 0x00, 0x32, 128 });
        }

        private void frmLog_Resize(object sender, EventArgs e)
        {
            var Spacing = 6;
            txtMessage.Height = ClientSize.Height - pnlProperties.Height - tabController.Height;
            tabController.Top = txtMessage.Height + Spacing;
            tabController.Width = ClientSize.Width;

            cmdConnect.Left = pnlProperties.Width - cmdConnect.Width - Spacing;
            cmdRefresh.Left = cmdConnect.Left - cmdRefresh.Width - Spacing;
            cboCOMPorts.Width = cmdRefresh.Left - cboCOMPorts.Left - Spacing;
        }

        private void SaveProperties()
        {
            if (cboCOMPorts.Items.Count > 0)
                Settings.Default.COMPort = COMPorts[cboCOMPorts.SelectedIndex].Name;
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

        private void txtServerPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            (sender as TextBox).ValidateInput_TCPPort(e);
        }

        private void HandleHotKeys(KeyEventArgs e)
        {
            if (e.KeyCode == Settings.Default.ToggleKeyboardControl)
                chkKeyboardControl.Checked = !chkKeyboardControl.Checked;
            else if (e.KeyCode == Keys.Escape)
                Application.Exit();

            if (chkKeyboardControl.Checked || e.KeyCode == Settings.Default.GlobalStopKey)
            {
                if (e.KeyCode == Settings.Default.ForwardKey || e.KeyCode == Settings.Default.BackwardKey ||
                    e.KeyCode == Settings.Default.RotateClockwiseKey || e.KeyCode == Settings.Default.StopKey ||
                    e.KeyCode == Settings.Default.RotateCounterClockwiseKey)
                {
                    // Set robot mode to 0 and enable timeout
                    Robot.SendData(new byte[] { 0x00, 0x34, 0x00, 0x00, 0x39 });
                }

                if (e.KeyCode == Settings.Default.ForwardKey)
                    Robot.SendData(new byte[] { 0x00, 0x31, 148, 0x00, 0x32, 148 });
                else if (e.KeyCode == Settings.Default.BackwardKey)
                    Robot.SendData(new byte[] { 0x00, 0x31, 108, 0x00, 0x32, 108 });
                else if (e.KeyCode == Settings.Default.RotateClockwiseKey)
                    Robot.SendData(new byte[] { 0x00, 0x31, 148, 0x00, 0x32, 108 });
                else if (e.KeyCode == Settings.Default.RotateCounterClockwiseKey)
                    Robot.SendData(new byte[] { 0x00, 0x31, 108, 0x00, 0x32, 148 });
                else if (e.KeyCode == Settings.Default.StopKey || e.KeyCode == Settings.Default.GlobalStopKey)
                    Robot.SendData(new byte[] { 0x00, 0x31, 128, 0x00, 0x32, 128 });

                e.SuppressKeyPress = true;
            }
        }

        private void SaveLogTextBox(object sender, KeyEventArgs e)
        {
            (sender as TextBox).SaveTextBox_CtrlS(e);
        }
    }
}
