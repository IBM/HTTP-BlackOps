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
	public class SelectFlowSelectedRequestsAction : BaseSelectedRequestEditorAction
	{


        public SelectFlowSelectedRequestsAction(TVRequestsList requestList, DataGridView dataGrid)
			: base(requestList, dataGrid)
		{
					
		}




		protected override void ActionFunction(TVRequestInfo curReqInfo)
		{
			
            while (curReqInfo!=null && curReqInfo.RefererId != -1)
            {
                _requestList.Select(curReqInfo.RefererId,false);
                curReqInfo = _dataSource.GetRequestInfo(curReqInfo.RefererId);
            }

		}
	}
}
