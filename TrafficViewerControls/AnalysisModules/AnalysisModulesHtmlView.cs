using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TrafficViewerSDK;

namespace TrafficViewerControls.AnalysisModules
{
	public partial class AnalysisModulesHtmlView : Form
	{
		string _content;
		string _extension;

		public AnalysisModulesHtmlView(string browserContent, string contentExtension)
		{
			InitializeComponent();
			_content = browserContent;
			_extension = contentExtension;
		}

		private void AnalysisModulesHtmlViewLoad(object sender, EventArgs e)
		{
			//create a temp file
			TempFile temp = new TempFile(_extension);
			temp.Write(_content);
			_browser.Navigate(temp.Url);
		}


	}
}
