using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficViewerSDK;
using TrafficViewerSDK.Http;

namespace ASMRest
{
	public class ImportCall:BaseRestHttpClient
	{
		private string _appId;
		private string _scannerId;
		private const string BOUNDARY_ID = "----ASMIssueEditor";
		public ImportCall(string appId, string scannerId, ASMRestSettings settings):base(settings)
		{
			_appId = appId;
			_scannerId = scannerId;
		}

		/// <summary>
		/// Imports a csv
		/// </summary>
		/// <param name="csv"></param>
		/// <param name="scanName"></param>
		/// <returns></returns>
		public bool Import(string csv,string scanName)
		{
			string requestLine;

			bool is9011OrGreater = this.ASMRestSettings.ServerVersion.CompareTo(new Version(9, 0, 1, 1)) >= 0;

			if (is9011OrGreater)
			{
				requestLine = String.Format("POST /ase/api/issueimport/{0}/{1} HTTP/1.1\r\n", _appId, _scannerId);
			}
			else
			{
				requestLine = String.Format("POST /ase/api/issueimport/{0}/{1}?scanName={2} HTTP/1.1\r\n", _appId, _scannerId, Utils.UrlEncode(scanName));
			}

			HttpRequestInfo reqInfo = new HttpRequestInfo(requestLine);
			reqInfo.Headers["Content-Type"] = "multipart/form-data; boundary=" + BOUNDARY_ID;

			StringBuilder sb = new StringBuilder();

			string boundary = "--" + BOUNDARY_ID;

			sb.AppendLine(boundary);
			sb.AppendLine("Content-Disposition: form-data; name=\"asc_xsrf_token\"");
			sb.AppendLine();
			sb.AppendLine(this.ASMRestSettings.AscSessionId);

			if (is9011OrGreater)
			{
				sb.AppendLine(boundary);
				sb.AppendLine("Content-Disposition: form-data; name=\"scanName\"");
				sb.AppendLine();
				sb.AppendLine(scanName);
			}

			sb.AppendLine(boundary);
			sb.AppendLine("Content-Disposition: form-data; name=\"uploadedfile\"; filename=\"Manual Test.csv\"");
			sb.AppendLine("Content-Type: application/octet-stream");
			sb.AppendLine();
			sb.AppendLine(csv);
			sb.Append(boundary);
			sb.Append("--");
			reqInfo.ContentData = Encoding.UTF8.GetBytes(sb.ToString());
			var respInfo = SendRequest(reqInfo);
			return respInfo.Status == 200;

		}
	}
}
