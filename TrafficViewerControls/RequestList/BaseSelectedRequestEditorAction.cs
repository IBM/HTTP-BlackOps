using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrafficViewerSDK;

namespace TrafficViewerControls.RequestList
{
	/// <summary>
	/// Iterates over the selected requests and executes an action for them
	/// </summary>
	public abstract class BaseSelectedRequestEditorAction
	{
		protected int _requestEventsQueueSize;
		protected DataGridView _dataGrid;
		protected ITrafficDataAccessor _dataSource;
		protected TVRequestsList _requestList;

		public BaseSelectedRequestEditorAction(TVRequestsList requestList, DataGridView dataGrid)
		{
			_requestList = requestList;
			_dataGrid = dataGrid;
			_dataSource = requestList.DataSource;
			_requestEventsQueueSize = requestList.RequestEventsQueueSize;
		}

		public void Execute()
		{
			int temp = _requestEventsQueueSize;
			//temporarily change the events queue size to allow the new request to be automatically added
			_requestEventsQueueSize = 0;
			
			int i, count = _dataGrid.SelectedRows.Count;
			for (i = count - 1; i > -1; i--)
			{
				DataGridViewRow row = _dataGrid.SelectedRows[i];
				//copy each selected row at the end of the list
				//get the id
				string stringId = (string)row.Cells["_id"].Value;
				int id = Convert.ToInt32(stringId);
				TVRequestInfo reqInfo = _dataSource.GetRequestInfo(id);
				ActionFunction(reqInfo);
			}

			_requestEventsQueueSize = temp;
			
		
		}


		protected abstract void ActionFunction(TVRequestInfo curReqInfo);
	}
}
