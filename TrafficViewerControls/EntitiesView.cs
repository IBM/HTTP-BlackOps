using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrafficViewerSDK.Http;
using TrafficViewerSDK;
using CommonControls;

namespace TrafficViewerControls
{
    public partial class EntitiesView : UserControl
    {
        private int _curReqId;
        private ITrafficDataAccessor _curAccessor;
        private HttpRequestInfo _curReqInfo;
        private bool _curIsBodyEncoded;
        
        public EntitiesView()
        {
            InitializeComponent();
        }


        internal void LoadRequest(int reqId, ITrafficDataAccessor accessor,string requestText)
        {
            _curReqId = reqId;
            _curAccessor = accessor;
            if (String.IsNullOrWhiteSpace(requestText))
            {
                return;
            }

            _gridParameters.Clear();
            _curReqInfo = new HttpRequestInfo(requestText,true);
            string contentType = _curReqInfo.Headers["Content-Type"];
            _curIsBodyEncoded = !String.IsNullOrWhiteSpace(contentType) && contentType.Contains("encoded");
            
            foreach (var variableName in _curReqInfo.PathVariables.Keys)
            {
                _gridParameters.AddRow(RequestLocation.Path.ToString(), variableName, _curReqInfo.PathVariables[variableName]);
            }
            foreach (var variableName in _curReqInfo.QueryVariables.Keys)
            {
                _gridParameters.AddRow(RequestLocation.Query.ToString(), variableName, Utils.UrlDecode(_curReqInfo.QueryVariables[variableName]));
            }
            foreach (var variableName in _curReqInfo.BodyVariables.Keys)
            {
                string val = _curReqInfo.BodyVariables[variableName];
                if (_curIsBodyEncoded)
                {
                    val = Utils.UrlDecode(val);
                }
                _gridParameters.AddRow(RequestLocation.Body.ToString(), variableName, val);
            }
            foreach (var variableName in _curReqInfo.Cookies.Keys)
            {
                _gridParameters.AddRow(RequestLocation.Cookies.ToString(), variableName, Utils.UrlDecode(_curReqInfo.Cookies[variableName]));
            }

            foreach (var header in _curReqInfo.Headers)
            {
                _gridParameters.AddRow(RequestLocation.Headers.ToString(), header.Name, header.Value);
            }
            
        }

        private void UpdateRequest(object sender, EventArgs e)
        {
            try
            {
                _curReqInfo.PathVariables.Clear();
                _curReqInfo.QueryVariables.Clear();
                _curReqInfo.BodyVariables.Clear();
                _curReqInfo.Cookies.Clear();
                _curReqInfo.Headers = new HTTPHeaders();
                
                var entities = _gridParameters.GetValues();
                foreach (var entity in entities)
                {
                    string[] values = entity.Split(Constants.VALUES_SEPARATOR.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (values.Length == 3)
                    {
                        if (values[0].Equals(RequestLocation.Path.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            _curReqInfo.PathVariables[values[1]] = Utils.UrlEncode(values[2]);
                        }
                        else if (values[0].Equals(RequestLocation.Query.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            _curReqInfo.QueryVariables[values[1]] = Utils.UrlEncode(values[2]);
                        }
                        else if (values[0].Equals(RequestLocation.Body.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            if (_curIsBodyEncoded)
                            {
                                values[2] = Utils.UrlEncode(values[2]);
                            }
                            _curReqInfo.BodyVariables[values[1]] = values[2];
                        }
                        else if (values[0].Equals(RequestLocation.Cookies.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            _curReqInfo.SetCookie(values[1], Utils.UrlEncode(values[2]));
                        }
                        else if (values[0].Equals(RequestLocation.Headers.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            _curReqInfo.Headers[values[1]] = values[2];
                        }
                    }
                }

                _curAccessor.SaveRequest(_curReqId, _curReqInfo.ToArray());
            }
            catch (Exception ex)
            {
                ErrorBox.ShowDialog(ex.Message);
            }

        }
    }
}
