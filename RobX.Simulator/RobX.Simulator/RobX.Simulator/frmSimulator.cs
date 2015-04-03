# region Includes

using System;
using System.Windows.Forms;
using RobX.Library.Commons;
using RobX.Library.Communication;
using RobX.Library.Communication.TCP;
using RobX.Library.Tools;

# endregion

namespace RobX.Simulator
{
    public partial class frmSimulator : Form
    {
        // ---------------------------- Private Variables ---------------------------- //

        public Simulator Simulator = new Simulator();
        private TCPServer RobotServer =  new TCPServer();
        private Log ServerLog = new Log();

        // ---------------------------- Public Variables ----------------------------- //

        public SimController SimulationController;

        // ------------------------------ Form Events -------------------------------- //

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

            ServerLog.ItemsAdded += UpdateTextBox;
            ServerLog.LogCleared += UpdateTextBox;

            // Start Server
            StartServer();

            // Start simulation
            StartSimulation();
        }

        private bool CheckInputErrors()
        {
            var ServerPortValid = true;

            ushort port;
            if (ushort.TryParse(txtServerPort.Text, out port) == false || port < 2)
            {
                ServerLog.AddItem("Error! Invalid server port number!");
                ServerPortValid = false;
            }

            return ServerPortValid;
        }

        private void StartServer()
        {
            if (CheckInputErrors() == false) return;

            if (RobotServer.IsRunning() == false)
            {
                RobotServer.ReceivedData += TCPReceivedData;
                RobotServer.SentData += TCPSentData;
                RobotServer.StatusChanged += TCPStatusChanged;
                RobotServer.BeforeSendingData += TCPBeforeSendingData;
            }
            RobotServer.StartServer(ushort.Parse(Properties.Settings.Default.ServerPort));
        }

        private void frmSimulator_KeyDown(object sender, KeyEventArgs e)
        {
            HandleHotKeys(e);
        }

        private void frmSimulator_Resize(object sender, EventArgs e)
        {
            var width = ClientSize.Width;
            var height = tabSimulator.Top;

            if (Properties.Settings.Default.KeepAspectRatio)
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

        private void tabSimulator_Resize(object sender, EventArgs e)
        {
            var txtHelpMinSize = 272;
            var txtHelp1Width = tabHelp.ClientSize.Width / 2;
            if (txtHelp1Width < txtHelpMinSize)
                txtHelp1Width = txtHelpMinSize;
            txtHelp2.Width = tabHelp.ClientSize.Width - txtHelp1Width;
            pnlSettings.Left = (tabSettings.ClientSize.Width - pnlSettings.Width) / 2;
        }

        // ---------------------------------------- TCP Server Events ---------------------------------- //

        private void TCPReceivedData(object sender, CommunicationEventArgs e)
        {
            Simulator.AddCommands(e.Data);
            ServerLog.AddBytes(e.Data);
        }

        private void TCPSentData(object sender, CommunicationEventArgs e)
        {
            ServerLog.AddBytes(e.Data);
        }

        private void TCPStatusChanged(object sender, CommunicationStatusEventArgs e)
        {
            ServerLog.AddItem(e.Status, true);
        }

        private void TCPBeforeSendingData(object sender, CommunicationEventArgs e)
        {
            e.Data = Simulator.GetSentBytes();
        }

        // ----------------------------------------------- Log Events ---------------------------------- //

        private delegate void SetTextCallback(string str);
        private void UpdateTextBox(string ServerLogText)
        {
            if (txtLog.InvokeRequired)
            {
                var d = new SetTextCallback(UpdateTextBox);
                Invoke(d, new object[] { ServerLogText });
            }
            else
            {
                if (txtLog.Text != ServerLogText)
                {
                    txtLog.Text = ServerLogText;
                    txtLog.Select(txtLog.Text.Length, 0);
                    txtLog.ScrollToCaret();
                }
            }
        }

        private void UpdateTextBox(object sender, LogEventArgs e)
        {
            UpdateTextBox(ServerLog.Text);
        }

        // ---------------------------------------- Private Functions ---------------------------------- //

        private void StartSimulation()
        {
            Simulator.RunSimulation(picSimulation, 10, Simulator.RenderOptions.RenderType.StaticAxis_ZeroCornered);
        }

        private void HandleHotKeys(KeyEventArgs e)
        {
            var txtServerPortActive = txtServerPort.Focused;

            if (txtServerPortActive == false)
            {
                if (e.KeyCode == Keys.D1 || e.KeyCode == Keys.NumPad1)
                    Simulator.Render.Type = Simulator.RenderOptions.RenderType.StaticAxis_ZeroCentered;
                else if (e.KeyCode == Keys.D2 || e.KeyCode == Keys.NumPad2)
                    Simulator.Render.Type = Simulator.RenderOptions.RenderType.StaticAxis_ZeroCornered;
                else if (e.KeyCode == Keys.F1)
                    tabSimulator.SelectedTab = tabHelp;
                else if (e.KeyCode == Keys.G)
                    Properties.Settings.Default.DrawGrids = !Properties.Settings.Default.DrawGrids;
                else if (e.KeyCode == Keys.S)
                    Properties.Settings.Default.DrawStatistics = !Properties.Settings.Default.DrawStatistics;
                else if (e.KeyCode == Keys.O)
                    Properties.Settings.Default.DrawObstacles = !Properties.Settings.Default.DrawObstacles;
                else if (e.KeyCode == Keys.T)
                    Properties.Settings.Default.DrawRobotTrace = !Properties.Settings.Default.DrawRobotTrace;
                else if (e.KeyCode == Keys.A)
                {
                    Properties.Settings.Default.KeepAspectRatio = !Properties.Settings.Default.KeepAspectRatio;
                    frmSimulator_Resize(this, new EventArgs());
                }
            }
            
            if (e.KeyCode == Keys.F5)
                StartSimulation();
            else if (e.KeyCode == Keys.F6)
                Simulator.ContinueSimulation();
            else if (e.KeyCode == Keys.F7)
                Simulator.StopSimulation();
            else if (e.KeyCode == Keys.Escape)
                Application.Exit();
            else if (e.KeyCode == Properties.Settings.Default.ToggleKeyboardControl)
                chkKeyboardControl.Checked = !chkKeyboardControl.Checked;

            if (chkKeyboardControl.Checked || e.KeyCode == Properties.Settings.Default.GlobalStopKey)
            {
                if (e.KeyCode == Properties.Settings.Default.ForwardKey || e.KeyCode == Properties.Settings.Default.BackwardKey ||
                    e.KeyCode == Properties.Settings.Default.RotateClockwiseKey || e.KeyCode == Properties.Settings.Default.StopKey ||
                    e.KeyCode == Properties.Settings.Default.RotateCounterClockwiseKey)
                {
                    // Set robot mode to 0
                    Simulator.AddCommands(new byte[] { 0x00, 0x34, 0x00 });

                    // Enable timeout
                    Simulator.AddCommands(new byte[] { 0x00, 0x39 });
                }

                if (e.KeyCode == Properties.Settings.Default.ForwardKey)
                {
                    Simulator.AddCommands(new byte[] { 0x00, 0x31, 148 });
                    Simulator.AddCommands(new byte[] { 0x00, 0x32, 148 });
                }
                else if (e.KeyCode == Properties.Settings.Default.BackwardKey)
                {
                    Simulator.AddCommands(new byte[] { 0x00, 0x31, 108 });
                    Simulator.AddCommands(new byte[] { 0x00, 0x32, 108 });
                }
                else if (e.KeyCode == Properties.Settings.Default.RotateClockwiseKey)
                {
                    Simulator.AddCommands(new byte[] { 0x00, 0x31, 148 });
                    Simulator.AddCommands(new byte[] { 0x00, 0x32, 108 });
                }
                else if (e.KeyCode == Properties.Settings.Default.RotateCounterClockwiseKey)
                {
                    Simulator.AddCommands(new byte[] { 0x00, 0x31, 108 });
                    Simulator.AddCommands(new byte[] { 0x00, 0x32, 148 });
                }
                else if (e.KeyCode == Properties.Settings.Default.StopKey || e.KeyCode == Properties.Settings.Default.GlobalStopKey)
                {
                    Simulator.AddCommands(new byte[] { 0x00, 0x31, 128 });
                    Simulator.AddCommands(new byte[] { 0x00, 0x32, 128 });
                }

                e.SuppressKeyPress = true;
            }
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

        private void SaveSettings()
        {
            Properties.Settings.Default.ServerPort = txtServerPort.Text;
            Properties.Settings.Default.FormPosition = Location;
            Properties.Settings.Default.FormSize = Size;
            Properties.Settings.Default.Save();
        }

        private void LoadSettings()
        {
            UserDefinitions.DefineSimulation(ref Simulator);
            Size = Properties.Settings.Default.FormSize;
            Location = Properties.Settings.Default.FormPosition;
            txtServerPort.Text = Properties.Settings.Default.ServerPort;
        }

        private void txtServerPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            (sender as TextBox).ValidateInput_TCPPort(e);
        }

        private void SaveLogTextBox(object sender, KeyEventArgs e)
        {
            (sender as TextBox).SaveTextBox_CtrlS(e);
        }
    }
}
