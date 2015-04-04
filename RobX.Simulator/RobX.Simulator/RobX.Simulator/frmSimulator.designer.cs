using System.ComponentModel;
using System.Windows.Forms;

namespace RobX.Simulator
{
    partial class frmSimulator
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSimulator));
            this.picSimulation = new System.Windows.Forms.PictureBox();
            this.tabSimulator = new System.Windows.Forms.TabControl();
            this.tabServerLog = new System.Windows.Forms.TabPage();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.pnlSettings = new System.Windows.Forms.Panel();
            this.cmdStartServer = new System.Windows.Forms.Button();
            this.chkKeyboardControl = new System.Windows.Forms.CheckBox();
            this.lblServerPort = new System.Windows.Forms.Label();
            this.txtServerPort = new System.Windows.Forms.TextBox();
            this.tabHelp = new System.Windows.Forms.TabPage();
            this.txtHelp2 = new System.Windows.Forms.TextBox();
            this.txtHelp1 = new System.Windows.Forms.TextBox();
            this.tabAbout = new System.Windows.Forms.TabPage();
            this.txtAbout = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.picSimulation)).BeginInit();
            this.tabSimulator.SuspendLayout();
            this.tabServerLog.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.pnlSettings.SuspendLayout();
            this.tabHelp.SuspendLayout();
            this.tabAbout.SuspendLayout();
            this.SuspendLayout();
            // 
            // picSimulation
            // 
            this.picSimulation.Dock = System.Windows.Forms.DockStyle.Top;
            this.picSimulation.Location = new System.Drawing.Point(0, 0);
            this.picSimulation.Name = "picSimulation";
            this.picSimulation.Size = new System.Drawing.Size(483, 365);
            this.picSimulation.TabIndex = 0;
            this.picSimulation.TabStop = false;
            // 
            // tabSimulator
            // 
            this.tabSimulator.Controls.Add(this.tabServerLog);
            this.tabSimulator.Controls.Add(this.tabSettings);
            this.tabSimulator.Controls.Add(this.tabHelp);
            this.tabSimulator.Controls.Add(this.tabAbout);
            this.tabSimulator.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabSimulator.Location = new System.Drawing.Point(0, 371);
            this.tabSimulator.Name = "tabSimulator";
            this.tabSimulator.SelectedIndex = 0;
            this.tabSimulator.Size = new System.Drawing.Size(483, 159);
            this.tabSimulator.TabIndex = 0;
            this.tabSimulator.Resize += new System.EventHandler(this.tabSimulator_Resize);
            // 
            // tabServerLog
            // 
            this.tabServerLog.Controls.Add(this.txtLog);
            this.tabServerLog.Location = new System.Drawing.Point(4, 22);
            this.tabServerLog.Name = "tabServerLog";
            this.tabServerLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabServerLog.Size = new System.Drawing.Size(475, 133);
            this.tabServerLog.TabIndex = 0;
            this.tabServerLog.Text = "Server Log";
            this.tabServerLog.UseVisualStyleBackColor = true;
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.SystemColors.Window;
            this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Location = new System.Drawing.Point(3, 3);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(469, 127);
            this.txtLog.TabIndex = 1;
            this.txtLog.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SaveLogTextBox);
            // 
            // tabSettings
            // 
            this.tabSettings.BackColor = System.Drawing.SystemColors.Control;
            this.tabSettings.Controls.Add(this.pnlSettings);
            this.tabSettings.Location = new System.Drawing.Point(4, 22);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Size = new System.Drawing.Size(475, 133);
            this.tabSettings.TabIndex = 4;
            this.tabSettings.Text = "Settings";
            // 
            // pnlSettings
            // 
            this.pnlSettings.BackColor = System.Drawing.SystemColors.Control;
            this.pnlSettings.Controls.Add(this.cmdStartServer);
            this.pnlSettings.Controls.Add(this.chkKeyboardControl);
            this.pnlSettings.Controls.Add(this.lblServerPort);
            this.pnlSettings.Controls.Add(this.txtServerPort);
            this.pnlSettings.Location = new System.Drawing.Point(118, 9);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Size = new System.Drawing.Size(238, 121);
            this.pnlSettings.TabIndex = 0;
            this.pnlSettings.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlSettings_Paint);
            // 
            // cmdStartServer
            // 
            this.cmdStartServer.Location = new System.Drawing.Point(148, 25);
            this.cmdStartServer.Name = "cmdStartServer";
            this.cmdStartServer.Size = new System.Drawing.Size(75, 23);
            this.cmdStartServer.TabIndex = 4;
            this.cmdStartServer.Text = "&Start Server";
            this.cmdStartServer.UseVisualStyleBackColor = true;
            this.cmdStartServer.Click += new System.EventHandler(this.cmdStartServer_Click);
            // 
            // chkKeyboardControl
            // 
            this.chkKeyboardControl.AutoSize = true;
            this.chkKeyboardControl.Location = new System.Drawing.Point(19, 75);
            this.chkKeyboardControl.Name = "chkKeyboardControl";
            this.chkKeyboardControl.Size = new System.Drawing.Size(188, 17);
            this.chkKeyboardControl.TabIndex = 5;
            this.chkKeyboardControl.Text = "Control robot using &keyboard (F12)";
            this.chkKeyboardControl.UseVisualStyleBackColor = true;
            // 
            // lblServerPort
            // 
            this.lblServerPort.AutoSize = true;
            this.lblServerPort.Location = new System.Drawing.Point(16, 30);
            this.lblServerPort.Name = "lblServerPort";
            this.lblServerPort.Size = new System.Drawing.Size(63, 13);
            this.lblServerPort.TabIndex = 2;
            this.lblServerPort.Text = "Server &Port:";
            // 
            // txtServerPort
            // 
            this.txtServerPort.Location = new System.Drawing.Point(82, 27);
            this.txtServerPort.MaxLength = 5;
            this.txtServerPort.Name = "txtServerPort";
            this.txtServerPort.Size = new System.Drawing.Size(60, 20);
            this.txtServerPort.TabIndex = 3;
            this.txtServerPort.Text = "1371";
            this.txtServerPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtServerPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtServerPort_KeyPress);
            // 
            // tabHelp
            // 
            this.tabHelp.Controls.Add(this.txtHelp2);
            this.tabHelp.Controls.Add(this.txtHelp1);
            this.tabHelp.Location = new System.Drawing.Point(4, 22);
            this.tabHelp.Name = "tabHelp";
            this.tabHelp.Size = new System.Drawing.Size(475, 133);
            this.tabHelp.TabIndex = 2;
            this.tabHelp.Text = "Help (F1)";
            this.tabHelp.UseVisualStyleBackColor = true;
            // 
            // txtHelp2
            // 
            this.txtHelp2.BackColor = System.Drawing.SystemColors.Window;
            this.txtHelp2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtHelp2.Dock = System.Windows.Forms.DockStyle.Right;
            this.txtHelp2.Enabled = false;
            this.txtHelp2.Location = new System.Drawing.Point(293, 0);
            this.txtHelp2.Multiline = true;
            this.txtHelp2.Name = "txtHelp2";
            this.txtHelp2.ReadOnly = true;
            this.txtHelp2.Size = new System.Drawing.Size(182, 133);
            this.txtHelp2.TabIndex = 1;
            this.txtHelp2.TabStop = false;
            this.txtHelp2.Text = resources.GetString("txtHelp2.Text");
            // 
            // txtHelp1
            // 
            this.txtHelp1.BackColor = System.Drawing.SystemColors.Window;
            this.txtHelp1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtHelp1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtHelp1.Enabled = false;
            this.txtHelp1.Location = new System.Drawing.Point(0, 0);
            this.txtHelp1.Multiline = true;
            this.txtHelp1.Name = "txtHelp1";
            this.txtHelp1.ReadOnly = true;
            this.txtHelp1.Size = new System.Drawing.Size(475, 133);
            this.txtHelp1.TabIndex = 0;
            this.txtHelp1.TabStop = false;
            this.txtHelp1.Text = resources.GetString("txtHelp1.Text");
            // 
            // tabAbout
            // 
            this.tabAbout.Controls.Add(this.txtAbout);
            this.tabAbout.Location = new System.Drawing.Point(4, 22);
            this.tabAbout.Name = "tabAbout";
            this.tabAbout.Size = new System.Drawing.Size(475, 133);
            this.tabAbout.TabIndex = 3;
            this.tabAbout.Text = "About";
            this.tabAbout.UseVisualStyleBackColor = true;
            // 
            // txtAbout
            // 
            this.txtAbout.BackColor = System.Drawing.SystemColors.Window;
            this.txtAbout.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtAbout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAbout.ForeColor = System.Drawing.Color.Blue;
            this.txtAbout.Location = new System.Drawing.Point(0, 0);
            this.txtAbout.Multiline = true;
            this.txtAbout.Name = "txtAbout";
            this.txtAbout.ReadOnly = true;
            this.txtAbout.Size = new System.Drawing.Size(475, 133);
            this.txtAbout.TabIndex = 6;
            this.txtAbout.TabStop = false;
            this.txtAbout.Text = resources.GetString("txtAbout.Text");
            // 
            // frmSimulator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 530);
            this.Controls.Add(this.tabSimulator);
            this.Controls.Add(this.picSimulation);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(499, 568);
            this.Name = "frmSimulator";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RobX Robot Simulator Ver 2.0";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSimulator_FormClosing);
            this.Load += new System.EventHandler(this.frmSimulator_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmSimulator_KeyDown);
            this.Resize += new System.EventHandler(this.frmSimulator_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.picSimulation)).EndInit();
            this.tabSimulator.ResumeLayout(false);
            this.tabServerLog.ResumeLayout(false);
            this.tabServerLog.PerformLayout();
            this.tabSettings.ResumeLayout(false);
            this.pnlSettings.ResumeLayout(false);
            this.pnlSettings.PerformLayout();
            this.tabHelp.ResumeLayout(false);
            this.tabHelp.PerformLayout();
            this.tabAbout.ResumeLayout(false);
            this.tabAbout.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private TabControl tabSimulator;
        private TabPage tabServerLog;
        private TabPage tabHelp;
        private TabPage tabAbout;
        private TextBox txtLog;
        private TextBox txtHelp1;
        private TextBox txtHelp2;
        private TextBox txtAbout;
        internal PictureBox picSimulation;
        private TabPage tabSettings;
        private Panel pnlSettings;
        private CheckBox chkKeyboardControl;
        private Label lblServerPort;
        private TextBox txtServerPort;
        private Button cmdStartServer;

    }
}

