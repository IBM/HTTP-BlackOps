using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrafficServer;
using TrafficViewerSDK;
using TrafficViewerSDK.Http;

namespace Testing
{
    /// <summary>
    /// Executes attacks while the user is browsing
    /// </summary>
    public class DriveByAttackProxy : BaseAttackProxy
    {
        private Object _lock = new Object();
        private int _numThreads = 10;
        
        private Dictionary<int, HttpRequestInfo> _requestIndex;
        private Queue<int> _requestsToTest;
        private int MAX_REQ_THREADS = 1;
        private List<int> _testedRequestHashes = new List<int>();

        public DriveByAttackProxy(ITestController testController, CustomTestsFile testFile, ITrafficDataAccessor dataStore, string host = "127.0.0.1", int port = 9998)
            : base(testController, testFile, dataStore, host, port)
        {
            _requestsToTest = new Queue<int>();
            _requestIndex = new Dictionary<int, HttpRequestInfo>();

        }

        public DriveByAttackProxy(ITestController testController, ITrafficDataAccessor dataStore, string host = "127.0.0.1", int port = 9998)
            : this(testController, null, dataStore, host, port)
        {
            
        }

        public override void Start()
        {
            
            base.Start();
            _requestIndex.Clear();
            _requestsToTest.Clear();
            _workList.Clear();
            _testedRequestHashes.Clear();
            _numThreads = _testFile.NumberOfThreads;
            for (int count = 0; count < MAX_REQ_THREADS; count++)
            {
                var t = new Thread(new ThreadStart(RequestHandlerThread));
                t.Start();
            }
        }

        private void RequestHandlerThread()
        {
            while (IsListening)
            {
                HttpRequestInfo reqInfo;
                int thisThreadRequestIndex = -1;
                lock (_lock)
                {
                    reqInfo = null;
                    if (_requestsToTest.Count > 0)
                    {
                        thisThreadRequestIndex = _requestsToTest.Dequeue();
                        _currentTestReqIdx = thisThreadRequestIndex;
                        reqInfo = _requestIndex[thisThreadRequestIndex];

                    }
                }

                if (reqInfo != null)
                {
                    bool isSecure = reqInfo.IsSecure;
                    string rawRequest = reqInfo.ToString();
                    
                    if (ShouldBeTested(rawRequest))
                    {
                        //parse parameters
                        reqInfo = new HttpRequestInfo(rawRequest, true);
                        reqInfo.IsSecure = isSecure;
                        int hash = reqInfo.GetHashCode(TrafficServerMode.IgnoreCookies);
                        lock (_lock)
                        {
                            if (_testedRequestHashes.Contains(hash))
                            {
                                HttpServerConsole.Instance.WriteLine(LogMessageType.Warning,
                                    "Request already tested: '{0}'", reqInfo.FullUrl);
                                continue; //we tested this request before
                            }
                            else
                            {
                                _testedRequestHashes.Add(hash);
                            }
                        }
                        Uri reqUri = new Uri(reqInfo.FullUrl);
                        MultiThreadedTestExecution testExecution = new MultiThreadedTestExecution(_tester, rawRequest, reqUri, _numThreads);
                      
                        lock (_lock)
                        {
                            GenerateEntities(thisThreadRequestIndex, reqInfo);
                            testExecution.TestsQueue = _workList[thisThreadRequestIndex];
                        }
                       
                        testExecution.StartTestsAsync();

                        while (testExecution.IsRunning)
                        {
                            if (!IsListening)
                            {
                                testExecution.CancelTests();
                                break;
                            }
                            HttpServerConsole.Instance.WriteLine(LogMessageType.Notification,
                                       "Requests in queue: {0}, Tests in queue for current request: {1}, testing with {2} threads.", _requestsToTest.Count, testExecution.TestsQueue.Count, _numThreads);
                            
                            Thread.Sleep(1000);
                        }

                        HttpServerConsole.Instance.WriteLine(LogMessageType.Notification,
                                     "Test execution completed.");
                    }
                }
                

                Thread.Sleep(10);


            }
            HttpServerConsole.Instance.WriteLine(LogMessageType.Notification,
                                    "Drive by Attack Proxy stopped.");                       
        }


        /// <summary>
        /// Gets the http proxy connection
        /// </summary>
        /// <param name="clientInfo"></param>
        /// <returns></returns>
        protected override IProxyConnection GetConnection(TrafficViewerSDK.Http.TcpClientInfo clientInfo)
        {
            return new DriveByAttackProxyConnection(clientInfo.Client, clientInfo.IsSecure, NetworkSettings, this, TrafficDataStore);
        }



        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <returns></returns>
        public HttpRequestInfo HandleRequest(HttpRequestInfo requestInfo)
        {
            lock (_lock)
            {
                _currentReqIdx++;
                _requestIndex.Add(_currentReqIdx, requestInfo);
                _requestsToTest.Enqueue(_currentReqIdx);
            }
            return requestInfo;
        }



    }
}
