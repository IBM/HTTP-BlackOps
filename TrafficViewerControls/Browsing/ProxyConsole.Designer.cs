namespace TrafficViewerControls.Browsing
{
	partial class ProxyConsole
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
        private System.ComponentModel.IContainer components = null;
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



		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProxyConsole));
            this._splitContainer = new System.Windows.Forms.SplitContainer();
            this.button1 = new System.Windows.Forms.Button();
            this._checkTrapResp = new System.Windows.Forms.CheckBox();
            this._checkTrapReq = new System.Windows.Forms.CheckBox();
            this._buttonReload = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this._buttonRelease = new System.Windows.Forms.Button();
            this._buttonClear = new System.Windows.Forms.Button();
            this._settingsButton = new System.Windows.Forms.Button();
            this._availableProxies = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this._labelStatistics = new System.Windows.Forms.Label();
            this._boxConsole = new System.Windows.Forms.DataGridView();
            this._messages = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._buttonStart = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this._notificationLabel = new System.Windows.Forms.Label();
            this._displayTimer = new System.Windows.Forms.Timer(this.components);
            this._buttonTooltip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this._splitContainer)).BeginInit();
            this._splitContainer.Panel1.SuspendLayout();
            this._splitContainer.Panel2.SuspendLayout();
            this._splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._boxConsole)).BeginInit();
            this.SuspendLayout();
            // 
            // _splitContainer
            // 
            resources.ApplyResources(this._splitContainer, "_splitContainer");
            this._splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this._splitContainer.Name = "_splitContainer";
            // 
            // _splitContainer.Panel1
            // 
            this._splitContainer.Panel1.Controls.Add(this.button1);
            this._splitContainer.Panel1.Controls.Add(this._checkTrapResp);
            this._splitContainer.Panel1.Controls.Add(this._checkTrapReq);
            this._splitContainer.Panel1.Controls.Add(this._buttonReload);
            this._splitContainer.Panel1.Controls.Add(this.button2);
            this._splitContainer.Panel1.Controls.Add(this._buttonRelease);
            this._splitContainer.Panel1.Controls.Add(this._buttonClear);
            this._splitContainer.Panel1.Controls.Add(this._settingsButton);
            this._splitContainer.Panel1.Controls.Add(this._availableProxies);
            this._splitContainer.Panel1.Controls.Add(this.label1);
            this._splitContainer.Panel1.Controls.Add(this._labelStatistics);
            this._splitContainer.Panel1.Controls.Add(this._boxConsole);
            this._splitContainer.Panel1.Controls.Add(this._buttonStart);
            // 
            // _splitContainer.Panel2
            // 
            this._splitContainer.Panel2.BackColor = System.Drawing.Color.LemonChiffon;
            this._splitContainer.Panel2.Controls.Add(this.button3);
            this._splitContainer.Panel2.Controls.Add(this._notificationLabel);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.ReleaseAllClick);
            // 
            // _checkTrapResp
            // 
            resources.ApplyResources(this._checkTrapResp, "_checkTrapResp");
            this._checkTrapResp.Name = "_checkTrapResp";
            this._checkTrapResp.UseVisualStyleBackColor = true;
            this._checkTrapResp.CheckedChanged += new System.EventHandler(this.ResponseTrapCheckedChanged);
            // 
            // _checkTrapReq
            // 
            resources.ApplyResources(this._checkTrapReq, "_checkTrapReq");
            this._checkTrapReq.Name = "_checkTrapReq";
            this._checkTrapReq.UseVisualStyleBackColor = true;
            this._checkTrapReq.CheckedChanged += new System.EventHandler(this.RequestTrapCheckedChanged);
            // 
            // _buttonReload
            // 
            this._buttonReload.BackColor = System.Drawing.Color.Transparent;
            this._buttonReload.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this._buttonReload.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            resources.ApplyResources(this._buttonReload, "_buttonReload");
            this._buttonReload.ForeColor = System.Drawing.Color.White;
            this._buttonReload.Image = global::TrafficViewerControls.Properties.Resources.reload;
            this._buttonReload.Name = "_buttonReload";
            this._buttonTooltip.SetToolTip(this._buttonReload, resources.GetString("_buttonReload.ToolTip"));
            this._buttonReload.UseVisualStyleBackColor = false;
            this._buttonReload.Click += new System.EventHandler(this.ReloadProxyClick);
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.button2.BackColor = System.Drawing.Color.Transparent;
            this.button2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Name = "button2";
            this._buttonTooltip.SetToolTip(this.button2, resources.GetString("button2.ToolTip"));
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.SetupTrapsClick);
            // 
            // _buttonRelease
            // 
            resources.ApplyResources(this._buttonRelease, "_buttonRelease");
            this._buttonRelease.BackColor = System.Drawing.Color.Transparent;
            this._buttonRelease.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this._buttonRelease.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this._buttonRelease.ForeColor = System.Drawing.Color.White;
            this._buttonRelease.Name = "_buttonRelease";
            this._buttonRelease.UseVisualStyleBackColor = false;
            this._buttonRelease.Click += new System.EventHandler(this.ReleaseClick);
            // 
            // _buttonClear
            // 
            resources.ApplyResources(this._buttonClear, "_buttonClear");
            this._buttonClear.BackColor = System.Drawing.Color.Transparent;
            this._buttonClear.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this._buttonClear.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this._buttonClear.ForeColor = System.Drawing.Color.White;
            this._buttonClear.Name = "_buttonClear";
            this._buttonTooltip.SetToolTip(this._buttonClear, resources.GetString("_buttonClear.ToolTip"));
            this._buttonClear.UseVisualStyleBackColor = false;
            this._buttonClear.Click += new System.EventHandler(this.ClearClick);
            // 
            // _settingsButton
            // 
            this._settingsButton.BackColor = System.Drawing.Color.Transparent;
            this._settingsButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this._settingsButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            resources.ApplyResources(this._settingsButton, "_settingsButton");
            this._settingsButton.ForeColor = System.Drawing.Color.White;
            this._settingsButton.Name = "_settingsButton";
            this._buttonTooltip.SetToolTip(this._settingsButton, resources.GetString("_settingsButton.ToolTip"));
            this._settingsButton.UseVisualStyleBackColor = false;
            this._settingsButton.Click += new System.EventHandler(this.SettingsClick);
            // 
            // _availableProxies
            // 
            this._availableProxies.BackColor = System.Drawing.Color.Black;
            resources.ApplyResources(this._availableProxies, "_availableProxies");
            this._availableProxies.ForeColor = System.Drawing.Color.White;
            this._availableProxies.FormattingEnabled = true;
            this._availableProxies.Name = "_availableProxies";
            this._availableProxies.SelectedIndexChanged += new System.EventHandler(this.AvailableProxiesSelectedIndexChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // _labelStatistics
            // 
            resources.ApplyResources(this._labelStatistics, "_labelStatistics");
            this._labelStatistics.Name = "_labelStatistics";
            // 
            // _boxConsole
            // 
            this._boxConsole.AllowUserToAddRows = false;
            this._boxConsole.AllowUserToDeleteRows = false;
            this._boxConsole.AllowUserToResizeRows = false;
            resources.ApplyResources(this._boxConsole, "_boxConsole");
            this._boxConsole.BackgroundColor = System.Drawing.Color.Black;
            this._boxConsole.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._boxConsole.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._boxConsole.ColumnHeadersVisible = false;
            this._boxConsole.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._messages});
            this._boxConsole.Name = "_boxConsole";
            this._boxConsole.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this._boxConsole.RowHeadersVisible = false;
            this._boxConsole.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.Black;
            this._boxConsole.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._boxConsole.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.Lime;
            this._boxConsole.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.Lime;
            this._boxConsole.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
            this._boxConsole.RowTemplate.Height = 18;
            this._boxConsole.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // _messages
            // 
            this._messages.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this._messages, "_messages");
            this._messages.Name = "_messages";
            // 
            // _buttonStart
            // 
            this._buttonStart.BackColor = System.Drawing.Color.Transparent;
            this._buttonStart.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this._buttonStart.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            resources.ApplyResources(this._buttonStart, "_buttonStart");
            this._buttonStart.ForeColor = System.Drawing.Color.White;
            this._buttonStart.Name = "_buttonStart";
            this._buttonTooltip.SetToolTip(this._buttonStart, resources.GetString("_buttonStart.ToolTip"));
            this._buttonStart.UseVisualStyleBackColor = false;
            this._buttonStart.Click += new System.EventHandler(this.StartStopClick);
            // 
            // button3
            // 
            resources.ApplyResources(this.button3, "button3");
            this.button3.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button3.Name = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.CloseNotification);
            // 
            // _notificationLabel
            // 
            resources.ApplyResources(this._notificationLabel, "_notificationLabel");
            this._notificationLabel.ForeColor = System.Drawing.Color.Black;
            this._notificationLabel.Name = "_notificationLabel";
            // 
            // _displayTimer
            // 
            this._displayTimer.Interval = 50;
            this._displayTimer.Tick += new System.EventHandler(this.DisplayTimerTick);
            // 
            // ProxyConsole
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this._splitContainer);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "ProxyConsole";
            this.Load += new System.EventHandler(this.ProxyConsoleLoad);
            this._splitContainer.Panel1.ResumeLayout(false);
            this._splitContainer.Panel1.PerformLayout();
            this._splitContainer.Panel2.ResumeLayout(false);
            this._splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._splitContainer)).EndInit();
            this._splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._boxConsole)).EndInit();
            this.ResumeLayout(false);

        }



        private System.Windows.Forms.Timer _displayTimer;
        private System.Windows.Forms.ToolTip _buttonTooltip;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox _checkTrapResp;
        private System.Windows.Forms.CheckBox _checkTrapReq;
        private System.Windows.Forms.Button _buttonReload;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button _buttonRelease;
        private System.Windows.Forms.Button _buttonClear;
        private System.Windows.Forms.Button _settingsButton;
        private System.Windows.Forms.ComboBox _availableProxies;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label _labelStatistics;
        private System.Windows.Forms.DataGridView _boxConsole;
        private System.Windows.Forms.DataGridViewTextBoxColumn _messages;
        private System.Windows.Forms.Button _buttonStart;
        private System.Windows.Forms.Label _notificationLabel;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.SplitContainer _splitContainer;
	}
}