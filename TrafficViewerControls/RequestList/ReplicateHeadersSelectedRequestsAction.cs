using CommonControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrafficViewerControls.Properties;
using TrafficViewerSDK;
using TrafficViewerSDK.Http;

namespace TrafficViewerControls.RequestList
{
	public class ReplicateHeadersSelectedRequestsAction : BaseSelectedRequestEditorAction
	{

		
		public ReplicateHeadersSelectedRequestsAction(TVRequestsList requestList, DataGridView dataGrid)
			: base(requestList, dataGrid)
		{
					
		}




		protected override void ActionFunction(TVRequestInfo curReqInfo)
		{
			int index = -1;
			TVRequestInfo tvInfo;
			var curReqData = _dataSource.LoadRequestData(curReqInfo.Id);
			if (curReqData == null) ErrorBox.ShowDialog("No request data for selected request");

			HttpRequestInfo curHttpReqInfo = new HttpRequestInfo(curReqData);


			while((tvInfo = _dataSource.GetNext(ref index))!=null)
			{
				if (tvInfo.Id != curReqInfo.Id)
				{ 
					//replicate the headers
					byte[] reqData = _dataSource.LoadRequestData(tvInfo.Id);
					HttpRequestInfo reqInfo = new HttpRequestInfo(reqData);
					reqInfo.Headers = new HTTPHeaders();
					reqInfo.Cookies.Clear();
					foreach (var header in curHttpReqInfo.Headers)
					{
						reqInfo.Headers.Add(header.Name, header.Values.ToArray());
					}
					_dataSource.SaveRequest(tvInfo.Id, reqInfo.ToArray(false));
					
				}
			}
		}
	}
}
