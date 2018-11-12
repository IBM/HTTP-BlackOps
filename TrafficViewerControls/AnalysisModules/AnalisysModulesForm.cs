using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TrafficViewerSDK.AnalysisModules;
using TrafficViewerSDK;

namespace TrafficViewerControls.AnalysisModules
{
	public partial class AnalisysModulesForm : Form
	{
		private IAnalysisModule _module;
		private ITrafficDataAccessor _source;
		private UserControl _currentView;

		public AnalisysModulesForm(IAnalysisModule module,ITrafficDataAccessor source)
		{
			InitializeComponent();

			_module = module;
			_source = source;

			AnalysisWelcomePage firstPage = new AnalysisWelcomePage();
			firstPage.Title = _module.Caption;
			firstPage.Description = _module.Description;
			firstPage.StartClicked += new EventHandler(StartClicked);
			
			firstPage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			_split.Panel2.Controls.Add(firstPage);

			_currentView = firstPage;
		}

		void StartClicked(object sender, EventArgs e)
		{
			_split.Panel2.Controls.Remove(_currentView);

			AnalysisProcessingPage procPage = new AnalysisProcessingPage(_module,_source);
			procPage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			_split.Panel2.Controls.Add(procPage);
			_currentView = procPage;
		}
	}
}