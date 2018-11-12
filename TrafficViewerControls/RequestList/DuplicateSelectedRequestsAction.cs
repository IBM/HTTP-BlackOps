using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrafficViewerSDK;

namespace TrafficViewerControls.RequestList
{
	public class DuplicateSelectedRequestsAction : BaseSelectedRequestEditorAction
	{
		public DuplicateSelectedRequestsAction(TVRequestsList requestList, DataGridView dataGrid)
			: base(requestList, dataGrid)
		{ }

		protected virtual void ModifyRequestResponse(byte [] request, byte[] response, out byte[] modifiedReq, out byte[] modifiedResp)
		{
			modifiedReq = request;
			modifiedResp = response;
		}

		protected override void ActionFunction(TVRequestInfo reqInfo)
		{
			byte[] request = _dataSource.LoadRequestData(reqInfo.Id);
			byte[] response = _dataSource.LoadResponseData(reqInfo.Id);
			TVRequestInfo duplicate = null;
			duplicate = new TVRequestInfo();
			duplicate.RequestLine = reqInfo.RequestLine;
			duplicate.IsHttps = reqInfo.IsHttps;
			duplicate.ThreadId = reqInfo.ThreadId;
			duplicate.Description = reqInfo.Description;
			duplicate.DomUniquenessId = reqInfo.DomUniquenessId;
			duplicate.ResponseStatus = reqInfo.ResponseStatus;

			ModifyRequestResponse(request, response, out request, out response);

			_dataSource.AddRequestInfo(duplicate);
			_dataSource.SaveRequestResponse(duplicate.Id, request, response);
			if (duplicate != null)
			{
				_requestList.Select(duplicate.Id);
			}
		}
	}
}
