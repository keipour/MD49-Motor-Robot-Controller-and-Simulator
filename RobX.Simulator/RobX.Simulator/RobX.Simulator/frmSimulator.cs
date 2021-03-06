﻿# region Includes

using System;
using System.Drawing;
using System.IO;
using System.Linq;
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
        private readonly Color _logBackColor = Color.Linen;
        private readonly Color _userLogBackColor = Color.Khaki;

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
            // Read help and about files
            var settingsCollection = Methods.GetApplicationSettings(Application.ExecutablePath,
                "applicationSettings/RobX_Simulator_v2.Settings");

            var helpText = Methods.ReadFormattedFile(@"Content\SimulatorHelp.txt", "%%", "%%", settingsCollection)
                .Split(new[] { "%%pagebreak%%" }, StringSplitOptions.RemoveEmptyEntries);

            txtHelp1.Text = helpText[0].TrimEnd();
            txtHelp2.Text = helpText[1].TrimStart();

            txtAbout.Text = File.ReadAllText(@"Content\SimulatorAbout.txt").Replace(@"%%version%%", ProductVersion);
            Text = Text.Replace(@"%%version%%", ProductVersion);

            LoadSettings();
            frmSimulator_Resize(sender, e);

            _serverLog.ItemsAdded += lstLogAddItems;
            _serverLog.LogCleared += lstLogClear;

            _robotServer.ReceivedData += TcpReceivedData;
            _robotServer.SentData += TcpSentData;
            _robotServer.StatusChanged += TcpStatusChanged;
            _robotServer.BeforeSendingData += TcpBeforeSendingData;
            
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
            if (Methods.IsValidPort(txtServerPort.Text)) return true;
            
            _serverLog.AddItem("Error! Invalid server port number!", Log.LogItem.LogItemTypes.Error, _userLogBackColor);
            return false;
        }

        private void StartServer()
        {
            if (CheckInputErrors() == false) return;
            _robotServer.StartServer(ushort.Parse(Settings.Default.ServerPort));
        }

        private void StartSimulation()
        {
            Simulator.RunSimulation(Simulator.RenderOptions.RenderType.StaticAxisZeroCornered);
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
            _serverLog.AddBytes(e.Data, Log.LogItem.LogItemTypes.Receive, _logBackColor);
        }

        private void TcpSentData(object sender, CommunicationEventArgs e)
        {
            _serverLog.AddBytes(e.Data, Log.LogItem.LogItemTypes.Send, _logBackColor);
        }

        private void TcpStatusChanged(object sender, CommunicationStatusEventArgs e)
        {
            _serverLog.AddItem(e.Status, true, _logBackColor);
        }

        private void TcpBeforeSendingData(object sender, CommunicationEventArgs e)
        {
            e.Data = Simulator.GetSentBytes();
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

            const int timeColWidth = 80;
            colLogTime.Width = timeColWidth;
            colLogText.Width = lstLog.ClientSize.Width - colLogTime.Width - 5;
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

        private void SaveLog(object sender, KeyEventArgs e)
        {
            (sender as ListView).SaveListView_CtrlS(e);
        }

        // ReSharper disable once InconsistentNaming
        private void lstLogAddItems(object sender, LogEventArgs e)
        {
            foreach (var item in e.Items.ToList())
                lstLog.AddLogItem(item);
        }

        // ReSharper disable once InconsistentNaming
        private void lstLogClear(object sender, LogEventArgs e)
        {
            lstLog.ClearItems();
        }

        # endregion
    }
}
