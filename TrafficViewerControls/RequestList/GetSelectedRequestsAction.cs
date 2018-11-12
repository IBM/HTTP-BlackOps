using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrafficViewerSDK;

namespace TrafficViewerControls.RequestList
{
	public class GetSelectedRequestsAction : BaseSelectedRequestEditorAction
	{

		private List<TVRequestInfo> _selectedRequests = new List<TVRequestInfo>();

		public List<TVRequestInfo> SelectedRequests
		{
			get { return _selectedRequests; }
			
		}

		public GetSelectedRequestsAction(TVRequestsList requestList, DataGridView dataGrid)
			: base(requestList, dataGrid)
		{ }

		

		protected override void ActionFunction(TVRequestInfo reqInfo)
		{
			_selectedRequests.Add(reqInfo);
		}
	}
}
