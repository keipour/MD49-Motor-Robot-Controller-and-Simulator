using System.ComponentModel;
using System.Windows.Forms;

namespace RobX.Interface
{
    partial class frmInterface
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmInterface));
            this.cmdConnect = new System.Windows.Forms.Button();
            this.lblCOMPort = new System.Windows.Forms.Label();
            this.cboCOMPorts = new System.Windows.Forms.ComboBox();
            this.cmdRefresh = new System.Windows.Forms.Button();
            this.pnlProperties = new System.Windows.Forms.Panel();
            this.cmdStartServer = new System.Windows.Forms.Button();
            this.chkKeyboardControl = new System.Windows.Forms.CheckBox();
            this.lblServerPort = new System.Windows.Forms.Label();
            this.txtServerPort = new System.Windows.Forms.TextBox();
            this.tabController = new System.Windows.Forms.TabControl();
            this.tabLog = new System.Windows.Forms.TabPage();
            this.lstLog = new System.Windows.Forms.ListView();
            this.colLogTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colLogText = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabHelp = new System.Windows.Forms.TabPage();
            this.txtHelp = new System.Windows.Forms.TextBox();
            this.tabAbout = new System.Windows.Forms.TabPage();
            this.txtAbout = new System.Windows.Forms.TextBox();
            this.pnlProperties.SuspendLayout();
            this.tabController.SuspendLayout();
            this.tabLog.SuspendLayout();
            this.tabHelp.SuspendLayout();
            this.tabAbout.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdConnect
            // 
            this.cmdConnect.Location = new System.Drawing.Point(364, 10);
            this.cmdConnect.Name = "cmdConnect";
            this.cmdConnect.Size = new System.Drawing.Size(90, 23);
            this.cmdConnect.TabIndex = 6;
            this.cmdConnect.Text = "&Connect";
            this.cmdConnect.UseVisualStyleBackColor = true;
            this.cmdConnect.Click += new System.EventHandler(this.cmdConnect_Click);
            // 
            // lblCOMPort
            // 
            this.lblCOMPort.AutoSize = true;
            this.lblCOMPort.Location = new System.Drawing.Point(11, 15);
            this.lblCOMPort.Name = "lblCOMPort";
            this.lblCOMPort.Size = new System.Drawing.Size(56, 13);
            this.lblCOMPort.TabIndex = 0;
            this.lblCOMPort.Text = "COM &Port:";
            // 
            // cboCOMPorts
            // 
            this.cboCOMPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCOMPorts.FormattingEnabled = true;
            this.cboCOMPorts.Location = new System.Drawing.Point(77, 11);
            this.cboCOMPorts.Name = "cboCOMPorts";
            this.cboCOMPorts.Size = new System.Drawing.Size(255, 21);
            this.cboCOMPorts.TabIndex = 1;
            // 
            // cmdRefresh
            // 
            this.cmdRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdRefresh.Image = ((System.Drawing.Image)(resources.GetObject("cmdRefresh.Image")));
            this.cmdRefresh.Location = new System.Drawing.Point(338, 10);
            this.cmdRefresh.Name = "cmdRefresh";
            this.cmdRefresh.Size = new System.Drawing.Size(21, 23);
            this.cmdRefresh.TabIndex = 2;
            this.cmdRefresh.TabStop = false;
            this.cmdRefresh.UseVisualStyleBackColor = true;
            this.cmdRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // pnlProperties
            // 
            this.pnlProperties.Controls.Add(this.cmdStartServer);
            this.pnlProperties.Controls.Add(this.chkKeyboardControl);
            this.pnlProperties.Controls.Add(this.lblServerPort);
            this.pnlProperties.Controls.Add(this.txtServerPort);
            this.pnlProperties.Controls.Add(this.cboCOMPorts);
            this.pnlProperties.Controls.Add(this.cmdRefresh);
            this.pnlProperties.Controls.Add(this.cmdConnect);
            this.pnlProperties.Controls.Add(this.lblCOMPort);
            this.pnlProperties.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlProperties.Location = new System.Drawing.Point(0, 305);
            this.pnlProperties.Name = "pnlProperties";
            this.pnlProperties.Size = new System.Drawing.Size(460, 75);
            this.pnlProperties.TabIndex = 11;
            // 
            // cmdStartServer
            // 
            this.cmdStartServer.Location = new System.Drawing.Point(364, 41);
            this.cmdStartServer.Name = "cmdStartServer";
            this.cmdStartServer.Size = new System.Drawing.Size(90, 23);
            this.cmdStartServer.TabIndex = 12;
            this.cmdStartServer.Text = "&Restart Server";
            this.cmdStartServer.UseVisualStyleBackColor = true;
            this.cmdStartServer.Visible = false;
            this.cmdStartServer.Click += new System.EventHandler(this.cmdStartServer_Click);
            // 
            // chkKeyboardControl
            // 
            this.chkKeyboardControl.AutoSize = true;
            this.chkKeyboardControl.Location = new System.Drawing.Point(158, 45);
            this.chkKeyboardControl.Name = "chkKeyboardControl";
            this.chkKeyboardControl.Size = new System.Drawing.Size(188, 17);
            this.chkKeyboardControl.TabIndex = 5;
            this.chkKeyboardControl.Text = "Control robot using &keyboard (F12)";
            this.chkKeyboardControl.UseVisualStyleBackColor = true;
            // 
            // lblServerPort
            // 
            this.lblServerPort.AutoSize = true;
            this.lblServerPort.Location = new System.Drawing.Point(11, 46);
            this.lblServerPort.Name = "lblServerPort";
            this.lblServerPort.Size = new System.Drawing.Size(63, 13);
            this.lblServerPort.TabIndex = 3;
            this.lblServerPort.Text = "&Server Port:";
            // 
            // txtServerPort
            // 
            this.txtServerPort.Location = new System.Drawing.Point(77, 43);
            this.txtServerPort.MaxLength = 5;
            this.txtServerPort.Name = "txtServerPort";
            this.txtServerPort.Size = new System.Drawing.Size(60, 20);
            this.txtServerPort.TabIndex = 4;
            this.txtServerPort.Text = "1370";
            this.txtServerPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtServerPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtServerPort_KeyPress);
            // 
            // tabController
            // 
            this.tabController.Controls.Add(this.tabLog);
            this.tabController.Controls.Add(this.tabHelp);
            this.tabController.Controls.Add(this.tabAbout);
            this.tabController.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabController.Location = new System.Drawing.Point(0, 0);
            this.tabController.Name = "tabController";
            this.tabController.SelectedIndex = 0;
            this.tabController.Size = new System.Drawing.Size(460, 303);
            this.tabController.TabIndex = 8;
            // 
            // tabLog
            // 
            this.tabLog.BackColor = System.Drawing.SystemColors.Control;
            this.tabLog.Controls.Add(this.lstLog);
            this.tabLog.Location = new System.Drawing.Point(4, 22);
            this.tabLog.Name = "tabLog";
            this.tabLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabLog.Size = new System.Drawing.Size(452, 277);
            this.tabLog.TabIndex = 0;
            this.tabLog.Text = "Log";
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
            this.lstLog.Size = new System.Drawing.Size(446, 271);
            this.lstLog.TabIndex = 9;
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
            this.tabHelp.Size = new System.Drawing.Size(456, 416);
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
            this.txtHelp.Size = new System.Drawing.Size(456, 416);
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
            this.tabAbout.Size = new System.Drawing.Size(456, 416);
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
            this.txtAbout.Size = new System.Drawing.Size(450, 410);
            this.txtAbout.TabIndex = 7;
            this.txtAbout.TabStop = false;
            this.txtAbout.Text = resources.GetString("txtAbout.Text");
            // 
            // frmInterface
            // 
            this.AcceptButton = this.cmdConnect;
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.PageTabList;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 380);
            this.Controls.Add(this.tabController);
            this.Controls.Add(this.pnlProperties);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(476, 418);
            this.Name = "frmInterface";
            this.Text = "RobX Hardware Interface Ver 1.1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmLog_FormClosing);
            this.Load += new System.EventHandler(this.frmLog_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmLog_KeyDown);
            this.Resize += new System.EventHandler(this.frmLog_Resize);
            this.pnlProperties.ResumeLayout(false);
            this.pnlProperties.PerformLayout();
            this.tabController.ResumeLayout(false);
            this.tabLog.ResumeLayout(false);
            this.tabHelp.ResumeLayout(false);
            this.tabHelp.PerformLayout();
            this.tabAbout.ResumeLayout(false);
            this.tabAbout.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Button cmdConnect;
        private Label lblCOMPort;
        private ComboBox cboCOMPorts;
        private Button cmdRefresh;
        private Panel pnlProperties;
        private Label lblServerPort;
        private TextBox txtServerPort;
        private TabControl tabController;
        private TabPage tabLog;
        private TabPage tabHelp;
        private TextBox txtHelp;
        private TabPage tabAbout;
        private TextBox txtAbout;
        private CheckBox chkKeyboardControl;
        private ListView lstLog;
        private ColumnHeader colLogTime;
        private ColumnHeader colLogText;
        private Button cmdStartServer;

    }
}

