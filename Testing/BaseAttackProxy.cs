using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficServer;
using TrafficViewerSDK;
using TrafficViewerSDK.Http;

namespace Testing
{

    public class BaseAttackProxy : AdvancedExploreProxy
    {
        public static readonly string TEST_FILE_PATH = "TestFilePath";

        private Object _lock = new Object();


        protected Dictionary<int, Queue<TestJob>> _workList;

        protected int _currentTestReqIdx;
        /// <summary>
        /// Gets which request is currently tested
        /// </summary>
        public int CurrentTestReqIdx
        {
            get { return _currentTestReqIdx; }

        }
        protected int _currentReqIdx = -1;
        /// <summary>
        /// Gets the current request
        /// </summary>
        public int CurrentReqIdx
        {
            get { return _currentReqIdx; }
        }

        protected ITestController _testController;
        protected CustomTestsFile _testFile;
        protected Tester _tester;
        private string _patternOfRequestExclusion;


        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="testController"></param>
        /// <param name="testFile"></param>
        /// <param name="dataStore"></param>
        /// <param name="patternOfTestRequest"></param>
        /// <param name="host">Host by default localhost</param>
        /// <param name="port">Port to use by default 9998</param>
        public BaseAttackProxy(ITestController testController, CustomTestsFile testFile, ITrafficDataAccessor dataStore, string host = "127.0.0.1", int port = 9998)
            : base(host, port, dataStore)
        {
            
            //it doesn't support these options because the http client is controlled by the tester
            //the tester could be either appscan or blackops
            ExtraOptions.Remove(HTTP_CLIENT_PROXY_HOST);
            ExtraOptions.Remove(HTTP_CLIENT_PROXY_PORT);
            _testController = testController;
            if (testFile != null)
            {
                _testFile = testFile;
            }
            else
            {
                ExtraOptions.Add(TEST_FILE_PATH, "CustomTests.xml");
            }


        }


        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="testController"></param>
        /// <param name="testFile"></param>
        /// <param name="dataStore"></param>
        /// <param name="patternOfTestRequest"></param>
        /// <param name="host">Host by default localhost</param>
        /// <param name="port">Port to use by default 9998</param>
        public BaseAttackProxy(ITestController testController, ITrafficDataAccessor dataStore, string host = "127.0.0.1", int port = 9998)
            : this(testController, null, dataStore, host, port)
        {
        }

        public override void Start()
        {
            _currentTestReqIdx = -1;
            _currentReqIdx = -1;
            string filePath = "";

            if (ExtraOptions.ContainsKey(TEST_FILE_PATH))
            {
                filePath = ExtraOptions[TEST_FILE_PATH];
                if (!String.IsNullOrWhiteSpace(filePath))
                {
                    try
                    {
                        CustomTestsFile testFile = new CustomTestsFile();
                        testFile.Load(filePath);
                        _testFile = testFile;
                    }
                    catch
                    {
                        HttpServerConsole.Instance.WriteLine(LogMessageType.Error,
                    "Could not load custom tests file located at: '{0}'.", filePath);
                    }

                }
            }

            _tester = new Tester(_testController, _testFile);
            _workList = new Dictionary<int, Queue<TestJob>>();
            _patternOfRequestExclusion = _testFile.PatternRequestExclusion;
            base.Start();
        }

        protected bool ShouldBeTested(string rawRequest)
        {
            bool shouldBeTested = false;

            var attackTargetList = _testFile.GetAttackTargetList();
            foreach (var target in attackTargetList.Values)
            {
                if (target.Status == AttackTargetStatus.Enabled && Utils.IsMatch(rawRequest, target.RequestPattern))
                {
                    shouldBeTested = true;
                    break;
                }
            }

            shouldBeTested = shouldBeTested && !Utils.IsMatch(rawRequest, _patternOfRequestExclusion);
            
            if (!shouldBeTested)
            {
                HttpServerConsole.Instance.WriteLine(LogMessageType.Warning, "Request will not be tested '{0}'.",
                    HttpRequestInfo.GetRequestLine(rawRequest));
            }

            return shouldBeTested;
        }

        /// <summary>
        /// Whether the test is complete
        /// </summary>
        public int TestCount
        {
            get
            {
                int sum = 0;
                lock (_lock)
                {
                    foreach (var queue in _workList.Values)
                    {
                        sum += queue.Count;
                    }
                }
                return sum;
            }
        }



        protected string GetUpdatedParameterValue(HttpRequestInfo curWorkingReqInfo, TestJob testJob)
        {
            string val = null;
            HttpVariables variables = null;
            HTTPHeaders headers = null;
            switch (testJob.RequestLocation)
            {
                case RequestLocation.Body: variables = curWorkingReqInfo.BodyVariables; break;
                case RequestLocation.Path: variables = curWorkingReqInfo.PathVariables; break;
                case RequestLocation.Query: variables = curWorkingReqInfo.QueryVariables; break;
                case RequestLocation.Cookies: variables = curWorkingReqInfo.Cookies; break;
                case RequestLocation.Headers: headers = curWorkingReqInfo.Headers; break;
            }

            if (variables != null && variables.ContainsKey(testJob.ParameterName))
            {
                val = variables[testJob.ParameterName];
            }
            else if (headers != null)
            {
                val = headers[testJob.ParameterName];
            }

            return val;
        }

        protected void GenerateEntities(int requestId, HttpRequestInfo workingReqInfo)
        {
            Queue<TestJob> workQueue = new Queue<TestJob>();
            var customTests = _testFile.GetCustomTests().Values;
            string entityExclusion = _testFile.PatternEntityExclusion;
            foreach (var test in customTests)
            {
                //iterate through parameters, cookies and headers
                foreach (var pathParameter in workingReqInfo.PathVariables)
                {
                    if (!Utils.IsMatch(pathParameter.Key, entityExclusion))
                    {
                        TestJob testJob = new TestJob(pathParameter.Key, pathParameter.Value, RequestLocation.Path, test);
                        workQueue.Enqueue(testJob);
                    }
                }

                foreach (var queryParameter in workingReqInfo.QueryVariables)
                {
                    if (!Utils.IsMatch(queryParameter.Key, entityExclusion))
                    {
                        TestJob testJob = new TestJob(queryParameter.Key, queryParameter.Value, RequestLocation.Query, test);
                        workQueue.Enqueue(testJob);
                    }
                }

                foreach (var bodyParameter in workingReqInfo.BodyVariables)
                {
                    if (!Utils.IsMatch(bodyParameter.Key, entityExclusion))
                    {
                        TestJob testJob = new TestJob(bodyParameter.Key, bodyParameter.Value, RequestLocation.Body, test);
                        workQueue.Enqueue(testJob);
                    }
                }

                if (!_testFile.TestOnlyParameters)
                {
                    foreach (var header in workingReqInfo.Headers)
                    {
                        if (!Utils.IsMatch(header.Name, entityExclusion))
                        {
                            if (!header.Name.Equals("Host"))
                            {
                                TestJob testJob = new TestJob(header.Name, header.Value, RequestLocation.Headers, test);
                                workQueue.Enqueue(testJob);
                            }
                        }
                    }

                    foreach (var cookie in workingReqInfo.Cookies)
                    {
                        if (!Utils.IsMatch(cookie.Key, entityExclusion))
                        {
                            TestJob testJob = new TestJob(cookie.Key, cookie.Value, RequestLocation.Cookies, test);
                            workQueue.Enqueue(testJob);
                        }
                    }
                }
            }

            _workList.Add(requestId, workQueue);

        }

    }
}
