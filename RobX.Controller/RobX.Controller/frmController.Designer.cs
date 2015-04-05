using System.ComponentModel;
using System.Windows.Forms;

namespace RobX.Controller
{
    partial class frmController
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmController));
            this.cmdStart = new System.Windows.Forms.Button();
            this.cmdConnect = new System.Windows.Forms.Button();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.lblIPAddress = new System.Windows.Forms.Label();
            this.lblPort = new System.Windows.Forms.Label();
            this.cboRobotType = new System.Windows.Forms.ComboBox();
            this.pnlController = new System.Windows.Forms.Panel();
            this.chkKeyboardControl = new System.Windows.Forms.CheckBox();
            this.lblSimSpeed = new System.Windows.Forms.Label();
            this.txtSimSpeed = new System.Windows.Forms.TextBox();
            this.txtIPAddress = new System.Windows.Forms.TextBox();
            this.tabController = new System.Windows.Forms.TabControl();
            this.tabLog = new System.Windows.Forms.TabPage();
            this.lstLog = new System.Windows.Forms.ListView();
            this.colLogTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colLogText = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabHelp = new System.Windows.Forms.TabPage();
            this.txtHelp = new System.Windows.Forms.TextBox();
            this.tabAbout = new System.Windows.Forms.TabPage();
            this.txtAbout = new System.Windows.Forms.TextBox();
            this.lstMessage = new System.Windows.Forms.ListView();
            this.colMessageTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colMessageText = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pnlController.SuspendLayout();
            this.tabController.SuspendLayout();
            this.tabLog.SuspendLayout();
            this.tabHelp.SuspendLayout();
            this.tabAbout.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdStart
            // 
            this.cmdStart.Location = new System.Drawing.Point(383, 36);
            this.cmdStart.Name = "cmdStart";
            this.cmdStart.Size = new System.Drawing.Size(75, 23);
            this.cmdStart.TabIndex = 9;
            this.cmdStart.Text = "&Start";
            this.cmdStart.UseVisualStyleBackColor = true;
            this.cmdStart.Click += new System.EventHandler(this.cmdStart_Click);
            // 
            // cmdConnect
            // 
            this.cmdConnect.Location = new System.Drawing.Point(383, 6);
            this.cmdConnect.Name = "cmdConnect";
            this.cmdConnect.Size = new System.Drawing.Size(75, 23);
            this.cmdConnect.TabIndex = 8;
            this.cmdConnect.Text = "&Connect";
            this.cmdConnect.UseVisualStyleBackColor = true;
            this.cmdConnect.Click += new System.EventHandler(this.cmdConnect_Click);
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(197, 7);
            this.txtPort.MaxLength = 5;
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(53, 20);
            this.txtPort.TabIndex = 3;
            this.txtPort.Text = "1371";
            this.txtPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPort_KeyPress);
            // 
            // lblIPAddress
            // 
            this.lblIPAddress.AutoSize = true;
            this.lblIPAddress.Location = new System.Drawing.Point(5, 11);
            this.lblIPAddress.Name = "lblIPAddress";
            this.lblIPAddress.Size = new System.Drawing.Size(61, 13);
            this.lblIPAddress.TabIndex = 0;
            this.lblIPAddress.Text = "IP &Address:";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(165, 11);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(29, 13);
            this.lblPort.TabIndex = 2;
            this.lblPort.Text = "&Port:";
            // 
            // cboRobotType
            // 
            this.cboRobotType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRobotType.FormattingEnabled = true;
            this.cboRobotType.Items.AddRange(new object[] {
            "Simulation",
            "Robot"});
            this.cboRobotType.Location = new System.Drawing.Point(259, 7);
            this.cboRobotType.Name = "cboRobotType";
            this.cboRobotType.Size = new System.Drawing.Size(115, 21);
            this.cboRobotType.TabIndex = 4;
            this.cboRobotType.SelectedIndexChanged += new System.EventHandler(this.cboRobotType_SelectedIndexChanged);
            // 
            // pnlController
            // 
            this.pnlController.Controls.Add(this.chkKeyboardControl);
            this.pnlController.Controls.Add(this.lblSimSpeed);
            this.pnlController.Controls.Add(this.txtSimSpeed);
            this.pnlController.Controls.Add(this.txtIPAddress);
            this.pnlController.Controls.Add(this.lblIPAddress);
            this.pnlController.Controls.Add(this.cmdConnect);
            this.pnlController.Controls.Add(this.cmdStart);
            this.pnlController.Controls.Add(this.cboRobotType);
            this.pnlController.Controls.Add(this.lblPort);
            this.pnlController.Controls.Add(this.txtPort);
            this.pnlController.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlController.Location = new System.Drawing.Point(0, 455);
            this.pnlController.Name = "pnlController";
            this.pnlController.Size = new System.Drawing.Size(464, 68);
            this.pnlController.TabIndex = 10;
            // 
            // chkKeyboardControl
            // 
            this.chkKeyboardControl.AutoSize = true;
            this.chkKeyboardControl.Location = new System.Drawing.Point(168, 40);
            this.chkKeyboardControl.Name = "chkKeyboardControl";
            this.chkKeyboardControl.Size = new System.Drawing.Size(188, 17);
            this.chkKeyboardControl.TabIndex = 7;
            this.chkKeyboardControl.Text = "Control robot using &keyboard (F12)";
            this.chkKeyboardControl.UseVisualStyleBackColor = true;
            // 
            // lblSimSpeed
            // 
            this.lblSimSpeed.AutoSize = true;
            this.lblSimSpeed.Location = new System.Drawing.Point(5, 41);
            this.lblSimSpeed.Name = "lblSimSpeed";
            this.lblSimSpeed.Size = new System.Drawing.Size(92, 13);
            this.lblSimSpeed.TabIndex = 5;
            this.lblSimSpeed.Text = "Si&mulation Speed:";
            // 
            // txtSimSpeed
            // 
            this.txtSimSpeed.Location = new System.Drawing.Point(99, 37);
            this.txtSimSpeed.MaxLength = 4;
            this.txtSimSpeed.Name = "txtSimSpeed";
            this.txtSimSpeed.Size = new System.Drawing.Size(60, 20);
            this.txtSimSpeed.TabIndex = 6;
            this.txtSimSpeed.Text = "1.0";
            this.txtSimSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtSimSpeed.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSimSpeed_KeyPress);
            // 
            // txtIPAddress
            // 
            this.txtIPAddress.Location = new System.Drawing.Point(66, 8);
            this.txtIPAddress.MaxLength = 15;
            this.txtIPAddress.Name = "txtIPAddress";
            this.txtIPAddress.Size = new System.Drawing.Size(93, 20);
            this.txtIPAddress.TabIndex = 0;
            this.txtIPAddress.Text = "127.0.0.1";
            this.txtIPAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtIPAddress.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtIPAddress_KeyPress);
            // 
            // tabController
            // 
            this.tabController.Controls.Add(this.tabLog);
            this.tabController.Controls.Add(this.tabHelp);
            this.tabController.Controls.Add(this.tabAbout);
            this.tabController.Location = new System.Drawing.Point(0, 229);
            this.tabController.Name = "tabController";
            this.tabController.SelectedIndex = 0;
            this.tabController.Size = new System.Drawing.Size(464, 223);
            this.tabController.TabIndex = 11;
            this.tabController.TabStop = false;
            // 
            // tabLog
            // 
            this.tabLog.BackColor = System.Drawing.SystemColors.Control;
            this.tabLog.Controls.Add(this.lstLog);
            this.tabLog.Location = new System.Drawing.Point(4, 22);
            this.tabLog.Name = "tabLog";
            this.tabLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabLog.Size = new System.Drawing.Size(456, 197);
            this.tabLog.TabIndex = 0;
            this.tabLog.Text = "Communication Log";
            // 
            // lstLog
            // 
            this.lstLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colLogTime,
            this.colLogText});
            this.lstLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstLog.FullRowSelect = true;
            this.lstLog.GridLines = true;
            this.lstLog.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lstLog.Location = new System.Drawing.Point(3, 3);
            this.lstLog.MultiSelect = false;
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(450, 191);
            this.lstLog.TabIndex = 12;
            this.lstLog.UseCompatibleStateImageBehavior = false;
            this.lstLog.View = System.Windows.Forms.View.Details;
            this.lstLog.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SaveLog);
            // 
            // colLogTime
            // 
            this.colLogTime.Text = "Time";
            // 
            // colLogText
            // 
            this.colLogText.Text = "Log Text";
            // 
            // tabHelp
            // 
            this.tabHelp.Controls.Add(this.txtHelp);
            this.tabHelp.Location = new System.Drawing.Point(4, 22);
            this.tabHelp.Name = "tabHelp";
            this.tabHelp.Size = new System.Drawing.Size(456, 197);
            this.tabHelp.TabIndex = 2;
            this.tabHelp.Text = "Help (F1)";
            this.tabHelp.UseVisualStyleBackColor = true;
            // 
            // txtHelp
            // 
            this.txtHelp.BackColor = System.Drawing.SystemColors.Window;
            this.txtHelp.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtHelp.Enabled = false;
            this.txtHelp.ForeColor = System.Drawing.Color.Black;
            this.txtHelp.Location = new System.Drawing.Point(0, 0);
            this.txtHelp.Multiline = true;
            this.txtHelp.Name = "txtHelp";
            this.txtHelp.ReadOnly = true;
            this.txtHelp.Size = new System.Drawing.Size(456, 197);
            this.txtHelp.TabIndex = 8;
            this.txtHelp.TabStop = false;
            this.txtHelp.Text = resources.GetString("txtHelp.Text");
            // 
            // tabAbout
            // 
            this.tabAbout.Controls.Add(this.txtAbout);
            this.tabAbout.Location = new System.Drawing.Point(4, 22);
            this.tabAbout.Name = "tabAbout";
            this.tabAbout.Padding = new System.Windows.Forms.Padding(3);
            this.tabAbout.Size = new System.Drawing.Size(456, 197);
            this.tabAbout.TabIndex = 1;
            this.tabAbout.Text = "About";
            this.tabAbout.UseVisualStyleBackColor = true;
            // 
            // txtAbout
            // 
            this.txtAbout.BackColor = System.Drawing.SystemColors.Window;
            this.txtAbout.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtAbout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAbout.ForeColor = System.Drawing.Color.Blue;
            this.txtAbout.Location = new System.Drawing.Point(3, 3);
            this.txtAbout.Multiline = true;
            this.txtAbout.Name = "txtAbout";
            this.txtAbout.ReadOnly = true;
            this.txtAbout.Size = new System.Drawing.Size(450, 191);
            this.txtAbout.TabIndex = 7;
            this.txtAbout.Text = resources.GetString("txtAbout.Text");
            // 
            // lstMessage
            // 
            this.lstMessage.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colMessageTime,
            this.colMessageText});
            this.lstMessage.Dock = System.Windows.Forms.DockStyle.Top;
            this.lstMessage.FullRowSelect = true;
            this.lstMessage.GridLines = true;
            this.lstMessage.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lstMessage.Location = new System.Drawing.Point(0, 0);
            this.lstMessage.Name = "lstMessage";
            this.lstMessage.Size = new System.Drawing.Size(464, 223);
            this.lstMessage.TabIndex = 10;
            this.lstMessage.UseCompatibleStateImageBehavior = false;
            this.lstMessage.View = System.Windows.Forms.View.Details;
            this.lstMessage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SaveLog);
            // 
            // colMessageTime
            // 
            this.colMessageTime.Text = "Time";
            // 
            // colMessageText
            // 
            this.colMessageText.Text = "Message Text";
            // 
            // frmController
            // 
            this.AcceptButton = this.cmdConnect;
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.PageTabList;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 523);
            this.Controls.Add(this.lstMessage);
            this.Controls.Add(this.tabController);
            this.Controls.Add(this.pnlController);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(480, 561);
            this.Name = "frmController";
            this.Text = "RobX Controller Ver 1.1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmLog_FormClosing);
            this.Load += new System.EventHandler(this.frmLog_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmLog_KeyDown);
            this.Resize += new System.EventHandler(this.frmLog_Resize);
            this.pnlController.ResumeLayout(false);
            this.pnlController.PerformLayout();
            this.tabController.ResumeLayout(false);
            this.tabLog.ResumeLayout(false);
            this.tabHelp.ResumeLayout(false);
            this.tabHelp.PerformLayout();
            this.tabAbout.ResumeLayout(false);
            this.tabAbout.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Button cmdStart;
        private Button cmdConnect;
        private TextBox txtPort;
        private Label lblIPAddress;
        private Label lblPort;
        private ComboBox cboRobotType;
        private Panel pnlController;
        private TextBox txtIPAddress;
        private Label lblSimSpeed;
        private TextBox txtSimSpeed;
        private CheckBox chkKeyboardControl;
        private TabControl tabController;
        private TabPage tabLog;
        private TabPage tabAbout;
        private TextBox txtAbout;
        private TabPage tabHelp;
        private TextBox txtHelp;
        private ListView lstLog;
        private ListView lstMessage;
        private ColumnHeader colLogTime;
        private ColumnHeader colLogText;
        private ColumnHeader colMessageTime;
        private ColumnHeader colMessageText;

    }
}

