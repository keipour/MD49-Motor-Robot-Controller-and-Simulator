# region Includes

using System;
using System.Windows.Forms;
using System.Threading;
using RobX.Tools;
using RobX.Commons;
using RobX.Communication;

# endregion

namespace RobX.Controller
{
    /// <summary>
    /// This class defines the visual form for controller of the robot.
    /// </summary>
    public partial class frmController : Form
    {
        private Log CommunicationLog = new Log();
        private Log MessageLog = new Log();
        private Controller Controller = new Controller(Commons.Robot.RobotType.Simulation);
        private Thread ExecutionThread = null; 

        /// <summary>
        /// This is the constructor for the form class of the controller of the robot.
        /// </summary>
        public frmController()
        {
            InitializeComponent();
        }

        // ---------------------------------------- Form Events ---------------------------------------- //

        private void frmLog_Load(object sender, EventArgs e)
        {
            // Reload last used settings
            LoadProperties();
            frmLog_Resize(sender, e);
            txtIPAddress.Select();

            // Add events
            CommunicationLog.ItemsAdded += new LogEventHandler(txtLogUpdate);
            CommunicationLog.LogCleared += new LogEventHandler(txtLogUpdate);
            MessageLog.ItemsAdded += new LogEventHandler(txtMessageUpdate);
            MessageLog.LogCleared += new LogEventHandler(txtMessageUpdate);
            Controller.ReceivedData += RobotReceivedData;
            Controller.SentData += RobotSentData;
            Controller.CommunicationStatusChanged += RobotCommunicationStatusChanged;
            Controller.RobotStatusChanged += RobotStatusChanged;
        }

        private void frmLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveProperties();
        }

        private void HandleHotKeys(KeyEventArgs e)
        {
            if (e.KeyCode == Properties.Settings.Default.StartKey)
                cmdStart_Click(this, e);
            else if (e.KeyCode == Properties.Settings.Default.ToggleKeyboardControl)
                chkKeyboardControl.Checked = !chkKeyboardControl.Checked;
            else if (e.KeyCode == Keys.Escape)
                Application.Exit();

            if (chkKeyboardControl.Checked == true || e.KeyCode == Properties.Settings.Default.GlobalStopKey)
            {
                if (e.KeyCode == Properties.Settings.Default.ForwardKey || e.KeyCode == Properties.Settings.Default.BackwardKey ||
                    e.KeyCode == Properties.Settings.Default.RotateClockwiseKey || e.KeyCode == Properties.Settings.Default.StopKey ||
                    e.KeyCode == Properties.Settings.Default.RotateCounterClockwiseKey)
                {
                    // Connect if is not connected yet
                    if (Controller.IsConnected == false) return;
                    
                    // Stop current execution
                    Controller.StopExecution();
                }


                if (e.KeyCode == Properties.Settings.Default.ForwardKey)
                    Controller.MoveForwardMilliseconds(500, 20);
                else if (e.KeyCode == Properties.Settings.Default.BackwardKey)
                    Controller.MoveBackwardMilliseconds(500, 20);
                else if (e.KeyCode == Properties.Settings.Default.RotateClockwiseKey)
                    Controller.SetSpeedMilliseconds(500, 20, -20);
                else if (e.KeyCode == Properties.Settings.Default.RotateCounterClockwiseKey)
                    Controller.SetSpeedMilliseconds(500, -20, 20);
                else if (e.KeyCode == Properties.Settings.Default.StopKey || e.KeyCode == Properties.Settings.Default.GlobalStopKey)
                    Controller.StopRobot();

                e.SuppressKeyPress = true;
            }
        }

        private void frmLog_KeyDown(object sender, KeyEventArgs e)
        {
            HandleHotKeys(e);
        }

        // ---------------------------------------- Communication Events ------------------------------- //

        private void RobotReceivedData(object sender, CommunicationEventArgs e)
        {
            CommunicationLog.AddBytes(e.Data);
        }

        private void RobotSentData(object sender, CommunicationEventArgs e)
        {
            CommunicationLog.AddBytes(e.Data);
        }

        private void RobotCommunicationStatusChanged(object sender, CommunicationStatusEventArgs e)
        {
            CommunicationLog.AddItem(e.Status, true);
        }

        private void RobotStatusChanged(object sender, CommunicationStatusEventArgs e)
        {
            MessageLog.AddItem(e.Status, true);
            MessageLog.AddItem();
        }

        // ----------------------------------------------- Log Events ---------------------------------- //

        private delegate void SetTextCallback(string str);
        private void txtLogUpdate(string LogText)
        {
            try
            {
                if (txtLog.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(txtLogUpdate);
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
            catch { }
        }

        private void txtLogUpdate(object sender, LogEventArgs e)
        {
            txtLogUpdate(CommunicationLog.Text);
        }

        private void txtMessageUpdate(string LogText)
        {
            try
            {
                if (txtMessage.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(txtMessageUpdate);
                    this.Invoke(d, new object[] { LogText });
                }
                else
                {
                    if (txtMessage.Text != LogText)
                    {
                        txtMessage.Text = LogText;
                        txtMessage.Select(txtMessage.Text.Length, 0);
                        txtMessage.ScrollToCaret();
                    }
                }
            }
            catch { }
        }

        private void txtMessageUpdate(object sender, LogEventArgs e)
        {
            txtMessageUpdate(MessageLog.Text);
        }

        // ------------------------------------------ Private Functions -------------------------------- //

        private void StartExecution()
        {
            // Return if still is not connected
            if (Controller.IsConnected == false) return;

            //Controller.Robot.ResetEncoders();

            //txtMessage.AddLine("Encoder 1: " + Controller.Robot.GetEncoder1());
            //txtMessage.AddLine("Encoder 2: " + Controller.Robot.GetEncoder2());

            //Controller.Robot.SetSpeeds(148, 148);

            //Thread.Sleep(4000);

            //int enc1 = Controller.Robot.GetVolts();
            //int enc2 = Controller.Robot.GetVolts();

            //txtMessage.AddLine("Encoder 1: " + enc1.ToString());
            //txtMessage.AddLine("Encoder 2: " + enc2.ToString());

            UserCommands.AddCommands(ref Controller);

            Controller.ExecuteCommandQueue();
        }

        private void cmdStart_Click(object sender, EventArgs e)
        {
            SaveProperties();

            if (Controller.IsConnected == false)
                cmdConnect_Click(sender, e);

            // Define execution thread
            if (ExecutionThread != null)
            {
                Controller.StopExecution();
                ExecutionThread.Abort();
            }

            ExecutionThread = new Thread(new ThreadStart(StartExecution));
            ExecutionThread.IsBackground = true;
            ushort SimSpeed = (ushort)(Math.Round(Properties.Settings.Default.SimulationSpeed * 10));
            if (SimSpeed != 10)
                Controller.SetSimulationSpeed(SimSpeed);
            ExecutionThread.Start();
        }

        private void cmdConnect_Click(object sender, EventArgs e)
        {
            if (Connect() == false) return;
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
                Properties.Settings.Default.RealIP = txtIPAddress.Text;
                Properties.Settings.Default.RealPort = txtPort.Text;
                txtIPAddress.Text = Properties.Settings.Default.SimulatorIP;
                txtPort.Text = Properties.Settings.Default.SimulatorPort;
            }
            else
            {
                Properties.Settings.Default.SimulatorIP = txtIPAddress.Text;
                Properties.Settings.Default.SimulatorPort = txtPort.Text;
                txtIPAddress.Text = Properties.Settings.Default.RealIP;
                txtPort.Text = Properties.Settings.Default.RealPort;
            }
            Properties.Settings.Default.RobotType = cboRobotType.SelectedIndex;
        }

        private bool CheckInputErrors()
        {
            bool ipValid = true;
            bool portValid = true;

            System.Net.IPAddress ip;
            if (System.Net.IPAddress.TryParse(txtIPAddress.Text, out ip) == false)
            {
                txtMessage.AddLine("Invalid IP address format!");
                ipValid = false;
            }

            ushort port;
            if (ushort.TryParse(txtPort.Text, out port) == false || port < 2)
            {
                txtMessage.AddLine("Invalid port number!");
                portValid = false;
            }

            return ipValid && portValid;
        }

        private bool Connect()
        {
            SaveProperties();
            if (CheckInputErrors() == true)
                return Controller.Connect(txtIPAddress.Text, Int32.Parse(txtPort.Text));
            return false;
        }

        private void LoadProperties()
        {
            this.Size = Properties.Settings.Default.FormSize;
            this.Location = Properties.Settings.Default.FormPosition;
            txtSimSpeed.Text = Properties.Settings.Default.SimulationSpeed.ToString("0.0");

            if (Properties.Settings.Default.RobotType == 1)
            {
                txtIPAddress.Text = Properties.Settings.Default.SimulatorIP;
                txtPort.Text = Properties.Settings.Default.SimulatorPort;
            }
            else
            {
                txtIPAddress.Text = Properties.Settings.Default.RealIP;
                txtPort.Text = Properties.Settings.Default.RealPort;
            }
            cboRobotType.SelectedIndex = Properties.Settings.Default.RobotType;
        }

        private void SaveProperties()
        {
            if (cboRobotType.SelectedIndex == 0) // if simulation
            {
                Properties.Settings.Default.SimulatorIP = txtIPAddress.Text;
                Properties.Settings.Default.SimulatorPort = txtPort.Text;
            }
            else
            {
                Properties.Settings.Default.RealIP = txtIPAddress.Text;
                Properties.Settings.Default.RealPort = txtPort.Text;
            }
            Properties.Settings.Default.SimulationSpeed = double.Parse(txtSimSpeed.Text);
            Properties.Settings.Default.RobotType = cboRobotType.SelectedIndex;
            Properties.Settings.Default.FormPosition = this.Location;
            Properties.Settings.Default.FormSize = this.Size;
            Properties.Settings.Default.Save();
        }

        private void frmLog_Resize(object sender, EventArgs e)
        {
            int Spacing = 6;
            txtMessage.Height = this.ClientSize.Height - pnlController.Height - tabController.Height;
            tabController.Top = txtMessage.Height + Spacing;
            tabController.Width = this.ClientSize.Width;

            cmdStart.Left = pnlController.Width - cmdStart.Width - Spacing;
            cmdConnect.Left = cmdStart.Left;
            cboRobotType.Width = cmdConnect.Left - cboRobotType.Left - Spacing;
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

        private void SaveLogTextBox(object sender, KeyEventArgs e)
        {
            (sender as TextBox).SaveTextBox_CtrlS(e);
        }
    }
}
