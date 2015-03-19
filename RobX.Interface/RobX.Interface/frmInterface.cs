# region Includes

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using RobX.Communication;
using RobX.Communication.TCP;
using RobX.Communication.COM;
using RobX.Tools;
using RobX.Commons;

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
        private COMClient Robot = new COMClient();

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

            CommunicationLog.ItemsAdded += new LogEventHandler(UpdateTextBox);
            CommunicationLog.LogCleared += new LogEventHandler(UpdateTextBox);
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
                SetTextCallback d = new SetTextCallback(UpdateTextBox);
                this.Invoke(d, new object[] { LogText });
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
            bool ServerPortValid = true;
            bool COMPortValid = true;

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
            if (CheckInputErrors() == true)
            {
                if (Robot.Connect(COMPorts[cboCOMPorts.SelectedIndex].Name, (int)Commons.Robot.BaudRate,
                    Commons.Robot.DataBits, Commons.Robot.Parity, Commons.Robot.StopBits) == false)
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

        private List<COMPort> COMPorts;
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            COMPorts = COMPort.GetCOMPorts();
            cboCOMPorts.Items.Clear();

            int LastCOMPort = COMPorts.Count - 1;
            foreach (COMPort comPort in COMPorts)
            {
                cboCOMPorts.Items.Add(string.Format("{0} – {1}", comPort.Name, comPort.Description));
                if (comPort.Name == Properties.Settings.Default.COMPort)
                    LastCOMPort = cboCOMPorts.Items.Count - 1;
            }
            cboCOMPorts.SelectedIndex = LastCOMPort;
            
            if (cboCOMPorts.Items.Count > 0)
                Properties.Settings.Default.COMPort = COMPorts[cboCOMPorts.SelectedIndex].Name;
        }

        private void frmLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveProperties();

            // Stop robot!
            Robot.SendData(new byte[] { 0x00, 0x34, 0x00, 0x00, 0x31, 128, 0x00, 0x32, 128 });
        }

        private void frmLog_Resize(object sender, EventArgs e)
        {
            int Spacing = 6;
            txtMessage.Height = this.ClientSize.Height - pnlProperties.Height - tabController.Height;
            tabController.Top = txtMessage.Height + Spacing;
            tabController.Width = this.ClientSize.Width;

            cmdConnect.Left = pnlProperties.Width - cmdConnect.Width - Spacing;
            cmdRefresh.Left = cmdConnect.Left - cmdRefresh.Width - Spacing;
            cboCOMPorts.Width = cmdRefresh.Left - cboCOMPorts.Left - Spacing;
        }

        private void SaveProperties()
        {
            if (cboCOMPorts.Items.Count > 0)
                Properties.Settings.Default.COMPort = COMPorts[cboCOMPorts.SelectedIndex].Name;
            Properties.Settings.Default.ServerPort = txtServerPort.Text;
            Properties.Settings.Default.FormPosition = this.Location;
            Properties.Settings.Default.FormSize = this.Size;
            Properties.Settings.Default.Save();
        }

        private void LoadProperties()
        {
            this.Size = Properties.Settings.Default.FormSize;
            this.Location = Properties.Settings.Default.FormPosition;
            txtServerPort.Text = Properties.Settings.Default.ServerPort.ToString();
        }

        private void txtServerPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            (sender as TextBox).ValidateInput_TCPPort(e);
        }

        private void HandleHotKeys(KeyEventArgs e)
        {
            if (e.KeyCode == Properties.Settings.Default.ToggleKeyboardControl)
                chkKeyboardControl.Checked = !chkKeyboardControl.Checked;
            else if (e.KeyCode == Keys.Escape)
                Application.Exit();

            if (chkKeyboardControl.Checked == true || e.KeyCode == Properties.Settings.Default.GlobalStopKey)
            {
                if (e.KeyCode == Properties.Settings.Default.ForwardKey || e.KeyCode == Properties.Settings.Default.BackwardKey ||
                    e.KeyCode == Properties.Settings.Default.RotateClockwiseKey || e.KeyCode == Properties.Settings.Default.StopKey ||
                    e.KeyCode == Properties.Settings.Default.RotateCounterClockwiseKey)
                {
                    // Set robot mode to 0 and enable timeout
                    Robot.SendData(new byte[] { 0x00, 0x34, 0x00, 0x00, 0x39 });
                }

                if (e.KeyCode == Properties.Settings.Default.ForwardKey)
                    Robot.SendData(new byte[] { 0x00, 0x31, 148, 0x00, 0x32, 148 });
                else if (e.KeyCode == Properties.Settings.Default.BackwardKey)
                    Robot.SendData(new byte[] { 0x00, 0x31, 108, 0x00, 0x32, 108 });
                else if (e.KeyCode == Properties.Settings.Default.RotateClockwiseKey)
                    Robot.SendData(new byte[] { 0x00, 0x31, 148, 0x00, 0x32, 108 });
                else if (e.KeyCode == Properties.Settings.Default.RotateCounterClockwiseKey)
                    Robot.SendData(new byte[] { 0x00, 0x31, 108, 0x00, 0x32, 148 });
                else if (e.KeyCode == Properties.Settings.Default.StopKey || e.KeyCode == Properties.Settings.Default.GlobalStopKey)
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
