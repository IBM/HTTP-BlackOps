using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrafficViewerSDK.Http
{
    /// <summary>
    /// Defines the location of the trap
    /// </summary>
    public enum HttpTrapLocation
    {
        Request,
        Response
    }
    /// <summary>
    /// Stores Http Trapn Definitions
    /// </summary>
    public class HttpTrapDef
    {
        private bool _enabled;
        /// <summary>
        /// Gets sets whether the trap is enabled
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        private HttpTrapLocation _location;
        /// <summary>
        /// Gets sets the trap location
        /// </summary>
        public HttpTrapLocation Location
        {
            get { return _location; }
            set { _location = value; }
        }


        private string _regex;
        /// <summary>
        /// Gets/sets the trap regex
        /// </summary>
        public string Regex
        {
            get { return _regex; }
            set { _regex = value; }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="location"></param>
        /// <param name="regex"></param>
        public HttpTrapDef(string enabled, string location, string regex)
        {
            // TODO: Complete member initialization
            _enabled = enabled.Equals("Enabled");
            _location = location.Equals("Request") ? HttpTrapLocation.Request: HttpTrapLocation.Response;
            _regex = regex;
        }

        /// <summary>
        /// Ovewrriten tab separated values of the definition
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(_enabled ? "Enabled" : "Disabled");
            sb.Append(Constants.VALUES_SEPARATOR);
            sb.Append(_location == HttpTrapLocation.Request ? "Request" : "Response");
            sb.Append(Constants.VALUES_SEPARATOR);
            sb.Append(_regex);
            return sb.ToString();
        }

        /// <summary>
        /// Checks if the request matches
        /// </summary>
        /// <param name="reqInfo"></param>
        /// <returns></returns>
        public bool IsMatch(string data)
        {
            if (!_enabled || String.IsNullOrWhiteSpace(_regex))
            {
                return false;
            }

            return Utils.IsMatch(data, _regex);
            
        }

               

    }
}
