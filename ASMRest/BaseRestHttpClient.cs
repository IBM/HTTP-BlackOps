using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using TrafficViewerInstance;
using TrafficViewerSDK;
using TrafficViewerSDK.Http;

namespace ASMRest
{
    public class BaseRestHttpClient
    {
        class LoginInfo
        {
            public string userId;
            public string password;
            public string featureKey = "AppScanEnterpriseUser";
        }

        private bool _inSession = false;

        private ASMRestSettings _asmRestSettings = null;
        /// <summary>
        /// Gets/sets the settings object
        /// </summary>
        protected ASMRestSettings ASMRestSettings
        {
            get
            {
                return _asmRestSettings;
            }
            set { _asmRestSettings = value; }
        }


        public bool InSession
        {
            get { return _inSession; }
        }

        public BaseRestHttpClient(ASMRestSettings settings)
        {
            _asmRestSettings = settings;
        }

        private static IHttpClient GetHttpClient()
        {
            SdkSettings.Instance.HttpRequestTimeout = 60;

            WebRequestClient client = new WebRequestClient();

            DefaultNetworkSettings networkSettings = new DefaultNetworkSettings();
            networkSettings.CertificateValidationCallback = new RemoteCertificateValidationCallback(CertificateValidator.ValidateRemoteCertificate);
            networkSettings.WebProxy = HttpWebRequest.GetSystemWebProxy();


            client.SetNetworkSettings(networkSettings);

            return client;
        }

        public bool Login()
        {
            bool success = false;
            LoginInfo lInfo = new LoginInfo();
            lInfo.userId = _asmRestSettings.UserName;
            lInfo.password = _asmRestSettings.Password;

            JavaScriptSerializer ser = new JavaScriptSerializer();
            ser.MaxJsonLength = 1024 * 1024 * 1024;
            string postData = ser.Serialize(lInfo);
            HttpResponseInfo resp = SendRequest("POST", LOGIN_PATH, null, postData);

            if (resp.Status == 200)
            {
                string responseHeaders = resp.Headers.ToString();
                string responseBody = resp.ResponseBody.ToString();
                _asmRestSettings.AscSessionId = Utils.RegexFirstGroupValue(responseHeaders, @"asc_session_id=([^;\r\n]+)");
                _asmRestSettings.AscSsoToken = Utils.RegexFirstGroupValue(responseHeaders, @"asc_sso_token=([^;\r\n]+)");
                string ver = Utils.RegexFirstGroupValue(responseBody, "version\":\"([^\"]+)");
                _asmRestSettings.ServerVersion = new Version(ver);
                success = true;

                if (_asmRestSettings.FullName.Equals("Unspecified"))
                {
                    //get the full user name
                    resp = SendRequest("GET", "/ase/api/currentuser");
                    if (resp.Status == 200)
                    {
                        _asmRestSettings.FullName = Utils.RegexFirstGroupValue(resp.ResponseBody.ToString(), "userName\":\"([^\"]+)");

                    }
                }

            }

            

            if (success)
            {
                return success;
            }
            else
            {
                throw new Exception(String.Format("Failed to login to '{0}' with user '{1}'",
                            _asmRestSettings.HostAndPort, _asmRestSettings.UserName));
            }


        }

        public HttpResponseInfo SendRequest(string method, string path,
            Dictionary<string, string> queryParams = null, string postData = null, string range = null)
        {
            HttpRequestInfo request;



            request = new HttpRequestInfo(Encoding.UTF8.GetBytes(String.Format("{0} {1} HTTP/1.1\r\n\r\n", method, path)));

            if (!String.IsNullOrWhiteSpace(range))
            {
                request.Headers["Range"] = range;
            }

            if (!String.IsNullOrWhiteSpace(postData))
            {

                request.ContentData = Encoding.UTF8.GetBytes(postData);
                request.Headers["Content-Type"] = "application/json";
            }

            if (queryParams != null)
            {
                foreach (string paramName in queryParams.Keys)
                {
                    request.QueryVariables.Add(paramName, queryParams[paramName]);

                }
            }



            return SendRequest(request);
        }

        public HttpResponseInfo SendRequest(HttpRequestInfo request)
        {


            if (_skipHtmlEncoding)
            {
                request.Headers["Accept"] = "application/json;opt=no-html-encoding";
            }

            bool isLogin = request.Path.Equals(LOGIN_PATH);
            if (String.IsNullOrWhiteSpace(_asmRestSettings.AscSessionId) && !isLogin)
            {
                Login();
            }

            if (!isLogin)
            {
                SetSessionIds(request);
            }
            request.HostAndPort = _asmRestSettings.HostAndPort;
            request.IsSecure = true;
            IHttpClient client = GetHttpClient();

            HttpResponseInfo respInfo = client.SendRequest(request);
            if (respInfo.Status == 401 && !isLogin) //session may have expired, relogin
            {
                Login();
                SetSessionIds(request);
                //try again
                client = GetHttpClient();
                respInfo = client.SendRequest(request);
            }


            return respInfo;

        }

        private void SetSessionIds(HttpRequestInfo request)
        {
            request.SetCookie("asc_session_id", _asmRestSettings.AscSessionId);
            request.SetCookie("asc_sso_token", _asmRestSettings.AscSsoToken);
            request.Headers["Asc_xsrf_token"] = _asmRestSettings.AscSessionId;
        }


        private const string LOGIN_PATH = "/ase/api/login";
        private bool _skipHtmlEncoding = false;
        /// <summary>
        /// Option that prevents html encoding the values
        /// </summary>
        public bool SkipHtmlEncoding
        {
            get { return _skipHtmlEncoding; }
            set { _skipHtmlEncoding = value; }
        }
    }
}
