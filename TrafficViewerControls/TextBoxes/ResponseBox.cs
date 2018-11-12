using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK;
using System.Windows.Forms;

namespace TrafficViewerControls.TextBoxes
{
	/// <summary>
	/// Rich text response box
	/// </summary>
	public class ResponseBox : TrafficTextBox
	{
		#region Menu Items

		private ToolStripMenuItem _applyDomUniquenessHtmlOnly;
		private ToolStripMenuItem _applyDomUniquenessText;
		private ToolStripMenuItem _getDomId;
		private ToolStripSeparator _separator;

		#endregion

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResponseBox));
			this._applyDomUniquenessHtmlOnly = new System.Windows.Forms.ToolStripMenuItem();
			this._applyDomUniquenessText = new System.Windows.Forms.ToolStripMenuItem();
			this._getDomId = new System.Windows.Forms.ToolStripMenuItem();
			this._separator = new System.Windows.Forms.ToolStripSeparator();
			this.SuspendLayout();
			// 
			// _textBox
			// 
			resources.ApplyResources(this._textBox, "_textBox");
			// 
			// _separator
			// 
			this._separator.Name = "_separator";
			resources.ApplyResources(this._separator, "_separator");
			// 
			// ResponseBox
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			resources.ApplyResources(this, "$this");
			this.Name = "ResponseBox";
			this.Controls.SetChildIndex(this._textBox, 0);
			this.ResumeLayout(false);

		}




		public ResponseBox()
		{
			InitializeComponent();
			//add items to the menu separately to prevent VS from overriding
			_contextMenu.Items.AddRange(new ToolStripItem[]
			{
				_separator,
				_applyDomUniquenessHtmlOnly,
				_applyDomUniquenessText,
				_getDomId
			});

		}
	
	}
}
