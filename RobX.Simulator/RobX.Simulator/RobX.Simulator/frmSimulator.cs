# region Includes

using System;
using System.Windows.Forms;
using RobX.Library.Commons;
using RobX.Library.Communication;
using RobX.Library.Communication.TCP;
using RobX.Library.Tools;
using RobX.Simulator.Properties;

# endregion

namespace RobX.Simulator
{
    /// <summary>
    /// Window of the simulator application.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public partial class frmSimulator : Form
    {
        # region Private Variables 

        private readonly TCPServer _robotServer =  new TCPServer();
        private readonly Log _serverLog = new Log();

        # endregion

        # region Public Variables 

        /// <summary>
        /// Contains all the simulation functions and objects.
        /// </summary>
        public Simulator Simulator = new Simulator();

        /// <summary>
        /// Manages all the simulation steps (timing, ets.).
        /// </summary>
        public SimController SimulationController;

        # endregion

        # region Form Constructor and Events 

        /// <summary>
        /// Constructor for the Simulator window.
        /// </summary>
        public frmSimulator()
        {
            InitializeComponent();
        }

        private void frmSimulator_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
            Application.Exit();
        }

        private void frmSimulator_Load(object sender, EventArgs e)
        {
            LoadSettings();
            frmSimulator_Resize(sender, e);

            _serverLog.ItemsAdded += UpdateTextBox;
            _serverLog.LogCleared += UpdateTextBox;

            // Start Server
            StartServer();

            // Start simulation
            StartSimulation();
        }

        private void frmSimulator_KeyDown(object sender, KeyEventArgs e)
        {
            HandleHotKeys(e);
        }

        private void frmSimulator_Resize(object sender, EventArgs e)
        {
            var width = ClientSize.Width;
            var height = tabSimulator.Top;

            if (Settings.Default.KeepAspectRatio)
            {
                picSimulation.Left = 0;
                picSimulation.Top = 0;
                picSimulation.Width = ClientSize.Width;
                picSimulation.Height = tabSimulator.Top;
            }
            else
            {
                picSimulation.Dock = DockStyle.None;
                var screenratio = (double)height / width;
                var groundratio = (double)Simulator.Environment.Ground.Height / Simulator.Environment.Ground.Width;
                if (groundratio > screenratio)
                {
                    picSimulation.Width = (int)(height / groundratio);
                    picSimulation.Height = height;
                }
                else if (groundratio < screenratio)
                {
                    picSimulation.Width = width;
                    picSimulation.Height = (int)(width * groundratio);
                }

                picSimulation.Left = (ClientSize.Width - picSimulation.Width) / 2;
                picSimulation.Top = (tabSimulator.Top - picSimulation.Height) / 2;
            }
            tabSimulator_Resize(sender, e);
        }

        # endregion

        # region Private Methods

        private bool CheckInputErrors()
        {
            ushort port;

            if (ushort.TryParse(txtServerPort.Text, out port) && port >= 2) return true;
            
            _serverLog.AddItem("Error! Invalid server port number!");

            return false;
        }

        private void StartServer()
        {
            if (CheckInputErrors() == false) return;

            if (_robotServer.IsRunning() == false)
            {
                _robotServer.ReceivedData += TcpReceivedData;
                _robotServer.SentData += TcpSentData;
                _robotServer.StatusChanged += TcpStatusChanged;
                _robotServer.BeforeSendingData += TcpBeforeSendingData;
            }
            _robotServer.StartServer(ushort.Parse(Settings.Default.ServerPort));
        }

        private void StartSimulation()
        {
            Simulator.RunSimulation(picSimulation, 10, Simulator.RenderOptions.RenderType.StaticAxisZeroCornered);
        }

        private void HandleHotKeys(KeyEventArgs e)
        {
            var txtServerPortActive = txtServerPort.Focused;

            if (txtServerPortActive == false)
            {
                switch (e.KeyCode)
                {
                    case Keys.D1:
                    case Keys.NumPad1:
                        Simulator.Render.Type = Simulator.RenderOptions.RenderType.StaticAxisZeroCentered;
                        break;
                    case Keys.D2:
                    case Keys.NumPad2:
                        Simulator.Render.Type = Simulator.RenderOptions.RenderType.StaticAxisZeroCornered;
                        break;
                    case Keys.F1:
                        tabSimulator.SelectedTab = tabHelp;
                        break;
                    case Keys.G:
                        Settings.Default.DrawGrids = !Settings.Default.DrawGrids;
                        break;
                    case Keys.S:
                        Settings.Default.DrawStatistics = !Settings.Default.DrawStatistics;
                        break;
                    case Keys.O:
                        Settings.Default.DrawObstacles = !Settings.Default.DrawObstacles;
                        break;
                    case Keys.T:
                        Settings.Default.DrawRobotTrace = !Settings.Default.DrawRobotTrace;
                        break;
                    case Keys.A:
                        Settings.Default.KeepAspectRatio = !Settings.Default.KeepAspectRatio;
                        frmSimulator_Resize(this, new EventArgs());
                        break;
                }
            }

            switch (e.KeyCode)
            {
                case Keys.F5:
                    StartSimulation();
                    break;
                case Keys.F6:
                    Simulator.ContinueSimulation();
                    break;
                case Keys.F7:
                    Simulator.StopSimulation();
                    break;
                case Keys.Escape:
                    Application.Exit();
                    break;
                default:
                    if (e.KeyCode == Settings.Default.ToggleKeyboardControl)
                        chkKeyboardControl.Checked = !chkKeyboardControl.Checked;
                    break;
            }

            if (!chkKeyboardControl.Checked && e.KeyCode != Settings.Default.GlobalStopKey) return;

            if (e.KeyCode == Settings.Default.ForwardKey || e.KeyCode == Settings.Default.BackwardKey ||
                e.KeyCode == Settings.Default.RotateClockwiseKey || e.KeyCode == Settings.Default.StopKey ||
                e.KeyCode == Settings.Default.RotateCounterClockwiseKey)
            {
                // Set robot mode to 0
                Simulator.AddCommands(new byte[] { 0x00, 0x34, 0x00 });

                // Enable timeout
                Simulator.AddCommands(new byte[] { 0x00, 0x39 });
            }

            if (e.KeyCode == Settings.Default.ForwardKey)
            {
                Simulator.AddCommands(new byte[] { 0x00, 0x31, 148 });
                Simulator.AddCommands(new byte[] { 0x00, 0x32, 148 });
            }
            else if (e.KeyCode == Settings.Default.BackwardKey)
            {
                Simulator.AddCommands(new byte[] { 0x00, 0x31, 108 });
                Simulator.AddCommands(new byte[] { 0x00, 0x32, 108 });
            }
            else if (e.KeyCode == Settings.Default.RotateClockwiseKey)
            {
                Simulator.AddCommands(new byte[] { 0x00, 0x31, 148 });
                Simulator.AddCommands(new byte[] { 0x00, 0x32, 108 });
            }
            else if (e.KeyCode == Settings.Default.RotateCounterClockwiseKey)
            {
                Simulator.AddCommands(new byte[] { 0x00, 0x31, 108 });
                Simulator.AddCommands(new byte[] { 0x00, 0x32, 148 });
            }
            else if (e.KeyCode == Settings.Default.StopKey || e.KeyCode == Settings.Default.GlobalStopKey)
            {
                Simulator.AddCommands(new byte[] { 0x00, 0x31, 128 });
                Simulator.AddCommands(new byte[] { 0x00, 0x32, 128 });
            }

            e.SuppressKeyPress = true;
        }

        private void SaveSettings()
        {
            Settings.Default.ServerPort = txtServerPort.Text;
            Settings.Default.FormPosition = Location;
            Settings.Default.FormSize = Size;
            Settings.Default.Save();
        }

        private void LoadSettings()
        {
            UserDefinitions.DefineSimulation(ref Simulator);
            Size = Settings.Default.FormSize;
            Location = Settings.Default.FormPosition;
            txtServerPort.Text = Settings.Default.ServerPort;
        }

        # endregion

        # region Communication Events

        private void TcpReceivedData(object sender, CommunicationEventArgs e)
        {
            Simulator.AddCommands(e.Data);
            _serverLog.AddBytes(e.Data);
        }

        private void TcpSentData(object sender, CommunicationEventArgs e)
        {
            _serverLog.AddBytes(e.Data);
        }

        private void TcpStatusChanged(object sender, CommunicationStatusEventArgs e)
        {
            _serverLog.AddItem(e.Status, true);
        }

        private void TcpBeforeSendingData(object sender, CommunicationEventArgs e)
        {
            e.Data = Simulator.GetSentBytes();
        }

        # endregion

        # region Log Events

        private delegate void SetTextCallback(string str);
        private void UpdateTextBox(string serverLogText)
        {
            if (txtLog.InvokeRequired)
            {
                var d = new SetTextCallback(UpdateTextBox);
                Invoke(d, serverLogText);
            }
            else
            {
                if (txtLog.Text == serverLogText) return;

                txtLog.Text = serverLogText;
                txtLog.Select(txtLog.Text.Length, 0);
                txtLog.ScrollToCaret();
            }
        }

        private void UpdateTextBox(object sender, LogEventArgs e)
        {
            UpdateTextBox(_serverLog.Text);
        }

        # endregion

        # region Form Component Events

        private void tabSimulator_Resize(object sender, EventArgs e)
        {
            const int txtHelpMinSize = 272;
            var txtHelp1Width = tabHelp.ClientSize.Width / 2;
            if (txtHelp1Width < txtHelpMinSize)
                txtHelp1Width = txtHelpMinSize;
            txtHelp2.Width = tabHelp.ClientSize.Width - txtHelp1Width;
            pnlSettings.Left = (tabSettings.ClientSize.Width - pnlSettings.Width) / 2;
        }

        private void pnlSettings_Paint(object sender, PaintEventArgs e)
        {
            tabSimulator_Resize(sender, e);
        }

        private void cmdStartServer_Click(object sender, EventArgs e)
        {
            SaveSettings();
            StartServer();
            tabSimulator.SelectedIndex = 0;
        }

        private void txtServerPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            (sender as TextBox).ValidateInput_TCPPort(e);
        }

        private void SaveLogTextBox(object sender, KeyEventArgs e)
        {
            (sender as TextBox).SaveTextBox_CtrlS(e);
        }

        # endregion
    }
}
