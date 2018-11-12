
namespace TrafficViewerControls.Browsing
{
	partial class TrafficPlaybackBrowser
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrafficPlaybackBrowser));
            this.ButtonPlay = new System.Windows.Forms.Button();
            this.ButtonStop = new System.Windows.Forms.Button();
            this.WebBrowser = new TrafficViewerControls.Browsing.WebBrowserEx();
            this.ButtonPause = new System.Windows.Forms.Button();
            this._playbackTicker = new System.Windows.Forms.Timer(this.components);
            this._textAddress = new System.Windows.Forms.TextBox();
            this._buttonGo = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Slider = new System.Windows.Forms.TrackBar();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.CheckTrap = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.Slider)).BeginInit();
            this.SuspendLayout();
            // 
            // ButtonPlay
            // 
            resources.ApplyResources(this.ButtonPlay, "ButtonPlay");
            this.ButtonPlay.Image = global::TrafficViewerControls.Properties.Resources.tail;
            this.ButtonPlay.Name = "ButtonPlay";
            this.ButtonPlay.UseVisualStyleBackColor = true;
            this.ButtonPlay.Click += new System.EventHandler(this.PlayClick);
            // 
            // ButtonStop
            // 
            resources.ApplyResources(this.ButtonStop, "ButtonStop");
            this.ButtonStop.Image = global::TrafficViewerControls.Properties.Resources.untail;
            this.ButtonStop.Name = "ButtonStop";
            this.ButtonStop.UseVisualStyleBackColor = true;
            this.ButtonStop.Click += new System.EventHandler(this.StopClick);
            // 
            // WebBrowser
            // 
            resources.ApplyResources(this.WebBrowser, "WebBrowser");
            this.WebBrowser.Name = "WebBrowser";
            this.WebBrowser.SetUIHandler = false;
            // 
            // ButtonPause
            // 
            resources.ApplyResources(this.ButtonPause, "ButtonPause");
            this.ButtonPause.Name = "ButtonPause";
            this.ButtonPause.UseVisualStyleBackColor = true;
            this.ButtonPause.Click += new System.EventHandler(this.PauseClick);
            // 
            // _playbackTicker
            // 
            this._playbackTicker.Interval = 3000;
            this._playbackTicker.Tick += new System.EventHandler(this.PlaybackTickerTick);
            // 
            // _textAddress
            // 
            resources.ApplyResources(this._textAddress, "_textAddress");
            this._textAddress.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this._textAddress.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
            this._textAddress.Name = "_textAddress";
            this._textAddress.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AddressKeyDown);
            // 
            // _buttonGo
            // 
            resources.ApplyResources(this._buttonGo, "_buttonGo");
            this._buttonGo.Name = "_buttonGo";
            this._buttonGo.UseVisualStyleBackColor = true;
            this._buttonGo.Click += new System.EventHandler(this.GoClick);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // Slider
            // 
            resources.ApplyResources(this.Slider, "Slider");
            this.Slider.Maximum = 4;
            this.Slider.Name = "Slider";
            this.Slider.Value = 2;
            this.Slider.Scroll += new System.EventHandler(this.SliderScroll);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.BackClick);
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.FwdClick);
            // 
            // CheckTrap
            // 
            resources.ApplyResources(this.CheckTrap, "CheckTrap");
            this.CheckTrap.Name = "CheckTrap";
            this.CheckTrap.UseVisualStyleBackColor = true;
            // 
            // TrafficPlaybackBrowser
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.CheckTrap);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._buttonGo);
            this.Controls.Add(this._textAddress);
            this.Controls.Add(this.ButtonPause);
            this.Controls.Add(this.ButtonStop);
            this.Controls.Add(this.ButtonPlay);
            this.Controls.Add(this.WebBrowser);
            this.Controls.Add(this.Slider);
            this.Name = "TrafficPlaybackBrowser";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TrafficPlaybackFormFormClosing);
            this.Load += new System.EventHandler(this.TrafficPlaybackBrowserOnLoad);
            ((System.ComponentModel.ISupportInitialize)(this.Slider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		protected System.Windows.Forms.Button ButtonPlay;
		protected System.Windows.Forms.Button ButtonStop;
		protected WebBrowserEx WebBrowser;
		protected System.Windows.Forms.Button ButtonPause;
		private System.Windows.Forms.Timer _playbackTicker;
		private System.Windows.Forms.TextBox _textAddress;
		private System.Windows.Forms.Button _buttonGo;
		private System.Windows.Forms.Label label1;
		protected System.Windows.Forms.TrackBar Slider;
		protected System.Windows.Forms.Button button1;
		protected System.Windows.Forms.Button button2;
        protected System.Windows.Forms.CheckBox CheckTrap;
	}
}