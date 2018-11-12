using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficViewerSDK.Options;

namespace TrafficViewerSDK.Http
{
    /// <summary>
    /// Used to update requests with patterns tracked from responses
    /// </summary>
    public class PatternTracker
    {

        private Dictionary<string, string> _currentValues;

        private Dictionary<string, TrackingPattern> _patternsToTrack;
        /// <summary>
        /// Property set by users of the pattern tracker
        /// </summary>
        public Dictionary<string, TrackingPattern> PatternsToTrack
        {
            get { return _patternsToTrack; }
            set
            {
                lock (_trackingLock)
                {
                    _currentValues = new Dictionary<string, string>();
                    _patternsToTrack = value;
                }
            }
        }


        private static object _constructLock = new object();
        private object _trackingLock = new object();

        private static PatternTracker _instance;
        
        /// <summary>
        /// The current pattern tracker instace
        /// </summary>
        public static PatternTracker Instance
        {
            get
            {
                lock (_constructLock)
                {
                    if (_instance == null)
                    {
                        _instance = new PatternTracker();
                    }
                }

                return PatternTracker._instance;
            }

        }

        /// <summary>
        /// Singleton constructor
        /// </summary>
        private PatternTracker()
        {
            _currentValues = new Dictionary<string, string>();
        }

        /// <summary>
        /// Updates patterns to be tracked with values from response info
        /// </summary>
        /// <param name="respInfo"></param>
        public void UpdatePatternValues(HttpResponseInfo respInfo)
        {
            if (respInfo != null)
            {
                UpdatePatternValues(respInfo.ToString());
            }
        }

        /// <summary>
        /// Updates patterns to be tracked with values from response info
        /// </summary>
        /// <param name="rawResponse"></param>
        public void UpdatePatternValues(string rawResponse)
        {
            lock (_trackingLock)
            {
                if (_patternsToTrack != null && !String.IsNullOrWhiteSpace(rawResponse))
                {
                    foreach (string key in _patternsToTrack.Keys)
                    {
                        TrackingPattern pattern = _patternsToTrack[key];

                        if (pattern.TrackingType == TrackingType.ResponsePattern)
                        {

                            string regex = pattern.TrackingValue;

                            string value = Utils.RegexFirstGroupValue(rawResponse, regex);

                            if (!String.IsNullOrWhiteSpace(value))
                            {
                                if (_currentValues.ContainsKey(key))
                                {
                                    _currentValues[key] = value;
                                }
                                else
                                {
                                    _currentValues.Add(key, value);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates the request with the current values
        /// </summary>
        /// <param name="requestInfo"></param>
        public HttpRequestInfo UpdateRequest(HttpRequestInfo requestInfo)
        {
            if (requestInfo != null)
            {
                bool isSecure = requestInfo.IsSecure;
                string updatedRawRequest = UpdateRequest(requestInfo.ToString());

                requestInfo = new HttpRequestInfo(updatedRawRequest);
                requestInfo.IsSecure = isSecure;
            }
            return requestInfo;

        }


        /// <summary>
        /// Updates the request with the current values
        /// </summary>
        /// <param name="rawRequest"></param>
        public string UpdateRequest(string rawRequest)
        {

            lock (_trackingLock)
            {

                if (_patternsToTrack != null && !String.IsNullOrWhiteSpace(rawRequest))
                {
                    foreach (string key in _patternsToTrack.Keys)
                    {
                        string currentValue = null;
                        TrackingPattern currentPattern = _patternsToTrack[key];

                        if (currentPattern.TrackingType == TrackingType.ResponsePattern)
                        {

                            //check if we have an updated value for this pattern

                            if (_currentValues.ContainsKey(key))
                            {
                                currentValue = _currentValues[key];
                            }


                            //update the request with this pattern
                            if (!String.IsNullOrWhiteSpace(currentValue))
                            {

                                rawRequest = Utils.ReplaceGroups(rawRequest, currentPattern.RequestPattern, currentValue);
                                HttpServerConsole.Instance.WriteLine(LogMessageType.Information, "Tracking pattern applied for: '{0}'", key);

                            }
                        }

                        if (currentPattern.TrackingType == TrackingType.Function)
                        {
                            //execute a function on the parameter value

                            currentValue = Utils.RegexFirstGroupValue(rawRequest, currentPattern.RequestPattern);

                            if (!String.IsNullOrWhiteSpace(currentValue))
                            {
                                //extract the functions to execute
                                string[] functions = currentPattern.TrackingValue.Split(',');

                                foreach (string func in functions)
                                {
                                    //apply the functions in order
                                    switch (func.ToLower())
                                    {
                                        case "base64encode": currentValue = Utils.Base64Encode(currentValue); break;
                                        case "base64decode": currentValue = Utils.Base64Decode(currentValue); break;
                                        case "urlencode": currentValue = Utils.UrlEncode(currentValue); break;
                                        case "urldecode": currentValue = Utils.UrlDecode(currentValue); break;
                                        case "ticks": currentValue = DateTime.Now.Ticks.ToString(); break;
                                        case "increment":
                                            int currentValueInt = 0;
                                            if (int.TryParse(currentValue, out currentValueInt))
                                            {
                                                currentValueInt++;
                                                currentValue = currentValueInt.ToString();
                                            }
                                            break;
                                    }
                                }

                                rawRequest = Utils.ReplaceGroups(rawRequest, currentPattern.RequestPattern, currentValue);
                                HttpServerConsole.Instance.WriteLine(LogMessageType.Information, "Function tracking pattern applied for: '{0}'", key);
                             
                            }
                        }

                    }
                }
            }
            return rawRequest;
        }

    }
}
