using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace TrafficViewerControls.AnalysisModules
{
	/// <summary>
	/// First page of the Analysis module wizard
	/// </summary>
	public partial class AnalysisWelcomePage : UserControl
	{
		/// <summary>
		/// Occurs when the start button was clicked
		/// </summary>
		public event EventHandler StartClicked
		{
			add
			{
				_buttonStart.Click += value;
			}
			remove
			{
				_buttonStart.Click -= value;
			}
		}

		/// <summary>
		/// Gets/sets the title of the module
		/// </summary>
		public string Title
		{
			get
			{
				return _title.Text;
			}
			set
			{
				_title.Text = value;
			}
		}

		/// <summary>
		/// Gets/sets the description of the module
		/// </summary>
		public string Description
		{
			get
			{
				return _description.Text;
			}
			set
			{
				_description.Text = value;
			}
		}

		public AnalysisWelcomePage()
		{
			InitializeComponent();
		}
	}
}
