using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrafficViewerControls.Properties;
using TrafficViewerSDK;
using TrafficViewerSDK.Http;

namespace TrafficViewerControls.RequestList
{
	public class SaveResponseSelectedRequestsAction : BaseSelectedRequestEditorAction
	{


        public SaveResponseSelectedRequestsAction(TVRequestsList requestList, DataGridView dataGrid)
			: base(requestList, dataGrid)
		{
					
		}




		protected override void ActionFunction(TVRequestInfo curReqInfo)
		{
            OpenFileDialog fDialog = new OpenFileDialog();
            fDialog.CheckFileExists = false;
            if (fDialog.ShowDialog() == DialogResult.OK)
            {
                byte[] responseBytes = _dataSource.LoadResponseData(curReqInfo.Id);
                if (responseBytes != null)
                {
                    HttpResponseInfo respInfo = new HttpResponseInfo(responseBytes);
                    byte[] respBody = respInfo.ResponseBody.ToArray();
                    if (respBody != null && !String.IsNullOrWhiteSpace(fDialog.FileName))
                    {
                        File.WriteAllBytes(fDialog.FileName, respBody);
                    }

                }
            }

		}
	}
}
