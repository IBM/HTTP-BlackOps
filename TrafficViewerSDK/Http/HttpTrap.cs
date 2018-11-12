using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;


namespace TrafficViewerSDK.Http
{
    /// <summary>
    /// Singleton used to stop requests from being sent
    /// </summary>
    public class HttpTrap
    {
        private static object _creationLock = new object();
        private static object _trapLock = new object();
        private static HttpTrap _instance = null;

        private List<HttpTrapDef> _trapDefs = new List<HttpTrapDef>();
        /// <summary>
        /// Configures the trap definitions
        /// </summary>
        public List<HttpTrapDef> TrapDefs
        {
            get { return _trapDefs; }
            set { _trapDefs = value; }
        }




        private bool _trapRequests;
        /// <summary>
        /// Enables or disables the request trap
        /// </summary>
        public bool TrapRequests
        {
            get { return _trapRequests; }
            set
            {
                _trapRequests = value;
                if (_trapRequests)
                {
                    HttpServerConsole.Instance.WriteLine(LogMessageType.Information, "Request trap enabled.");
                    //check if trap defs are enabled
                    foreach (HttpTrapDef def in _trapDefs)
                    {
                        if (def.Location == HttpTrapLocation.Request && def.Enabled) return;
                    }
                    HttpServerConsole.Instance.WriteLine(LogMessageType.Warning, "WARNING: no request traps are enabled");
                }
                else
                {
                    HttpServerConsole.Instance.WriteLine(LogMessageType.Information, "Request trap disabled.");
                }
            }
        }


        private bool _trapResponses;
        /// <summary>
        /// Enables or disables the response trap
        /// </summary>
        public bool TrapResponses
        {
            get { return _trapResponses; }
            set
            {
                _trapResponses = value;
                if (_trapResponses)
                {
                    HttpServerConsole.Instance.WriteLine(LogMessageType.Information, "Response trap enabled.");
                    //check if trap defs are enabled
                    foreach (HttpTrapDef def in _trapDefs)
                    {
                        if (def.Location == HttpTrapLocation.Response && def.Enabled) return;
                    }
                    HttpServerConsole.Instance.WriteLine(LogMessageType.Warning, "WARNING: no response traps are enabled");
                }
                else
                {
                    HttpServerConsole.Instance.WriteLine(LogMessageType.Information, "Resposne trap disabled.");
                }
            }
        }

        private event EventHandler _trapOn;
        /// <summary>
        /// Occurs when any kind of trap was on
        /// </summary>
        public event EventHandler TrapOn
        {
            add
            {
                _trapOn += value;
            }
            remove { _trapOn -= value; }
        }

        private event EventHandler _trapOff;
        /// <summary>
        /// Occurs when any kind of trap is released
        /// </summary>
        public event EventHandler TrapOff
        {
            add
            {
                _trapOff += value;
            }
            remove { _trapOff -= value; }
        }


        private event RequestTrapEvent _requestTrapped = null;
        /// <summary>
        /// Event triggered in a separate thread when a request is trapped
        /// </summary>
        public event RequestTrapEvent RequestTrapped
        {
            add
            {
                _requestTrapped += value;
            }
            remove { _requestTrapped -= value; }
        }

        private event RequestTrapEvent _responseTrapped = null;
        /// <summary>
        /// Event triggered in a separate thread when a request is trapped
        /// </summary>
        public event RequestTrapEvent ResponseTrapped
        {
            add { _responseTrapped += value; }
            remove { _responseTrapped -= value; }
        }

        /// <summary>
        /// Gets the request trap instance
        /// </summary>
        public static HttpTrap Instance
        {
            get
            {
                lock (_creationLock)
                {
                    if (_instance == null)
                    {
                        _instance = new HttpTrap();
                    }
                }
                return _instance;
            }
        }


        /// <summary>
        /// Checks if the request matches the traps
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private bool MatchesTrapDefs(HttpRequestInfo info)
        {
            string rawRequest = info.ToString();
            foreach (HttpTrapDef def in _trapDefs)
            {
                if (def.Location == HttpTrapLocation.Request && def.IsMatch(rawRequest))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the response matches the traps
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private bool MatchesTrapDefs(HttpResponseInfo info)
        {
            string rawResponse = info.ToString();

            foreach (HttpTrapDef def in _trapDefs)
            {
                if (def.Location == HttpTrapLocation.Response && def.IsMatch(rawResponse))
                {
                    return true;
                }
            }
            return false;
        }



        /// <summary>
        /// Traps a request and triggers the event for the current thread
        /// </summary>
        /// <param name="httpReqInfo"></param>
        /// <param name="tvReqInfo"></param>
        /// <returns>True of False if the request was trapped or not</returns>
        public bool TrapRequest(TVRequestInfo tvReqInfo, HttpRequestInfo httpReqInfo)
        {
            if (_trapRequests)
            {

                //trigger the event, 
                if (_requestTrapped != null)
                {

                    if (MatchesTrapDefs(httpReqInfo))
                    {

                        ManualResetEvent reqLock = new ManualResetEvent(false);
                        _trapOn.BeginInvoke(this, new EventArgs(), null, null);
                        _requestTrapped.BeginInvoke(new RequestTrapEventEventArgs(tvReqInfo, httpReqInfo, reqLock), null, null);

                        //wait for the event to finish
                        reqLock.WaitOne();

                        _trapOff.BeginInvoke(this, new EventArgs(), null, null);

                        //the request was trapped return true
                        return true;
                    }
                }

            }
            return false;
        }

        /// <summary>
        /// Traps the response and triggers the event for the current thread
        /// </summary>
        /// <param name="tvReqInfo"></param>
        /// <param name="httpRespInfo"></param>
        /// <returns>True of False if the request was trapped or not</returns>
        public bool TrapResponse(TVRequestInfo tvReqInfo, HttpResponseInfo httpRespInfo)
        {
            if (_trapResponses)
            {


                //trigger the event, 
                if (_responseTrapped != null)
                {
                    string rawResponse = httpRespInfo.ToString();
                    if (MatchesTrapDefs(httpRespInfo))
                    {

                        ManualResetEvent reqLock = new ManualResetEvent(false);
                        _trapOn.BeginInvoke(this, new EventArgs(), null, null);
                        _responseTrapped.BeginInvoke(new RequestTrapEventEventArgs(tvReqInfo, httpRespInfo, reqLock), null, null);

                        //wait for the event to finish
                        reqLock.WaitOne();
                        _trapOff.BeginInvoke(this, new EventArgs(), null, null);

                        //the request was trapped return true
                        return true;
                    }
                }

            }
            return false;
        }


        private HttpTrap()
        {
            _trapRequests = false;
            _trapResponses = false;
        }

    }
}
