using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace TrafficViewerControls.Configuration
{
	public partial class ColorSwatch : UserControl
	{

		Color _color;
		/// <summary>
		/// Gets/sets the color of the swatch. On set the swatch changes its color
		/// </summary>
		[DefaultValue(typeof(Color),"White")]
		public Color Color
		{
			get { return _color; }
			set 
			{ 
				_color = value;
				_buttonSwatch.BackColor = value;
			}
		}

		/// <summary>
		/// Gets sets the text of the swatch
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Always)]
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Bindable(true)]
		public override string Text
		{
			get
			{
				return _labelText.Text;
			}
			set
			{
				_labelText.Text = value;
			}
		}

		public ColorSwatch()
		{
			InitializeComponent();
		}

		private void SwatchClick(object sender, EventArgs e)
		{
			if (_dialogColor.ShowDialog() == DialogResult.OK)
			{
				Color = _dialogColor.Color;
			}
		}
	}
}
