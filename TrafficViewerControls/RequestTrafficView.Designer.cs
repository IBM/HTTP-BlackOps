namespace TrafficViewerControls
{
	partial class RequestTrafficView
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RequestTrafficView));
			this._splitContainer = new System.Windows.Forms.SplitContainer();
			this._requestBox = new TrafficViewerControls.TextBoxes.RequestBox();
			this.label1 = new System.Windows.Forms.Label();
			this._responseBox = new TrafficViewerControls.TextBoxes.ResponseBox();
			this.label2 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this._splitContainer)).BeginInit();
			this._splitContainer.Panel1.SuspendLayout();
			this._splitContainer.Panel2.SuspendLayout();
			this._splitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// _splitContainer
			// 
			this._splitContainer.BackColor = System.Drawing.Color.Black;
			resources.ApplyResources(this._splitContainer, "_splitContainer");
			this._splitContainer.Name = "_splitContainer";
			// 
			// _splitContainer.Panel1
			// 
			this._splitContainer.Panel1.BackColor = System.Drawing.Color.Black;
			this._splitContainer.Panel1.Controls.Add(this._requestBox);
			this._splitContainer.Panel1.Controls.Add(this.label1);
			resources.ApplyResources(this._splitContainer.Panel1, "_splitContainer.Panel1");
			this._splitContainer.Panel1.ForeColor = System.Drawing.Color.Lime;
			// 
			// _splitContainer.Panel2
			// 
			this._splitContainer.Panel2.BackColor = System.Drawing.Color.Black;
			this._splitContainer.Panel2.Controls.Add(this._responseBox);
			this._splitContainer.Panel2.Controls.Add(this.label2);
			resources.ApplyResources(this._splitContainer.Panel2, "_splitContainer.Panel2");
			this._splitContainer.Panel2.ForeColor = System.Drawing.Color.Lime;
			this._splitContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this._splitContainer_SplitterMoved);
			// 
			// _requestBox
			// 
			this._requestBox.AutoSearchCriteria = null;
			resources.ApplyResources(this._requestBox, "_requestBox");
			this._requestBox.LineMatch = null;
			this._requestBox.Name = "_requestBox";
			this._requestBox.Rtf = "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1033{\\fonttbl{\\f0\\fnil\\fcharset0 Courier New" +
    ";}}\r\n\\viewkind4\\uc1\\pard\\f0\\fs17\\par\r\n}\r\n";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// _responseBox
			// 
			this._responseBox.AutoSearchCriteria = null;
			resources.ApplyResources(this._responseBox, "_responseBox");
			this._responseBox.LineMatch = null;
			this._responseBox.Name = "_responseBox";
			this._responseBox.Rtf = "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1033{\\fonttbl{\\f0\\fnil\\fcharset0 Courier New" +
    ";}}\r\n\\viewkind4\\uc1\\pard\\f0\\fs17\\par\r\n}\r\n";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// RequestTrafficView
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._splitContainer);
			this.Name = "RequestTrafficView";
			this._splitContainer.Panel1.ResumeLayout(false);
			this._splitContainer.Panel1.PerformLayout();
			this._splitContainer.Panel2.ResumeLayout(false);
			this._splitContainer.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._splitContainer)).EndInit();
			this._splitContainer.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer _splitContainer;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private TrafficViewerControls.TextBoxes.RequestBox _requestBox;
		private TrafficViewerControls.TextBoxes.ResponseBox _responseBox;
	}
}
