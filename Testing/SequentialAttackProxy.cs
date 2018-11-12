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
    

    public class SequentialAttackProxy : BaseAttackProxy
    {
        private Object _lock = new Object();
       
       
        private int _firstRequestHash = 0;

        public SequentialAttackProxy(ITestController testController, CustomTestsFile testFile, ITrafficDataAccessor dataStore, string host = "127.0.0.1", int port = 9998)
            : base(testController, testFile, dataStore, host, port)
        {
        }

        public SequentialAttackProxy(ITestController testController, ITrafficDataAccessor dataStore, string host = "127.0.0.1", int port = 9998)
            : this(testController, null, dataStore, host, port)
        {

        }

        private string _curMutatedRawReq;
        private Queue<string> _curMutatedRawReqQueue = new Queue<string>();
        private string _curWorkingRawRequest;
        private HttpRequestInfo _curWorkingReqInfo;
        private TestJob _curTestJob;
        private string _curEntityId;
        private int _detectedFlowRequests = -1; //this is the number of requests in the flow based on playback 
        private List<string> _curMutatedRequestList = new List<string>();
        private Dictionary<int, List<string>> _curTestResponseCollection = new Dictionary<int, List<string>>();
        
        /// <summary> 
        /// Whether all the requests in the flow have been tested
        /// </summary>
        public bool TestComplete
        {
            get
            {
                return _currentTestReqIdx == _detectedFlowRequests;
            }
        }

        /// <summary>
        /// Gets the http proxy connection
        /// </summary>
        /// <param name="clientInfo"></param>
        /// <returns></returns>
        protected override IProxyConnection GetConnection(TrafficViewerSDK.Http.TcpClientInfo clientInfo)
        {
            return new SequentialAttackProxyConnection(clientInfo.Client, clientInfo.IsSecure, NetworkSettings, this, TrafficDataStore);
        }


        public bool ValidateResponse(HttpResponseInfo respInfo)
        {
            bool found = false;
            lock (_lock)
            {
                if (_curMutatedRequestList != null && _curTestJob != null)
                {
                    string testResponse = null;
                    if (respInfo != null)
                    {
                        testResponse = respInfo.ToString();
                    }

                    if (!_tester.IsMultiValidationRule(_curTestJob.TestDef.Validation))
                    {
                        found = _tester.ValidateSingleTest(_curWorkingRawRequest, "\r\nTesting proxy, original not available\r\n",
                            new Uri(_curWorkingReqInfo.FullUrl), _curTestJob.ParameterName, _curEntityId, _curTestJob.TestDef, _curMutatedRawReq, testResponse);
                    }
                    else if (_curMutatedRequestList.Count > 1)
                    {
                        List<string> currentRequestTestResponseSet = null;
                        if (!_curTestResponseCollection.TryGetValue(_currentReqIdx, out currentRequestTestResponseSet))
                        {
                            currentRequestTestResponseSet = new List<string>();
                            _curTestResponseCollection.Add(_currentReqIdx, currentRequestTestResponseSet);
                        }
                        currentRequestTestResponseSet.Add(testResponse);


                        if (_curTestResponseCollection[_currentReqIdx].Count == _curMutatedRequestList.Count)
                        {
                            found = _tester.ValidateTest(_curWorkingRawRequest, "\r\nTesting proxy, original not available\r\n",
                           new Uri(_curWorkingReqInfo.FullUrl), _curTestJob.ParameterName, _curEntityId, _curTestJob.TestDef, _curMutatedRequestList, _curTestResponseCollection[_currentReqIdx]);

                        }
                    }
                }
            }
            return found;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <returns></returns>
        public HttpRequestInfo HandleRequest(HttpRequestInfo requestInfo, out bool mutated)
        {
            lock (_lock)
            {
                mutated = false;
                //make a request info that we parse in order to generate a hash and entities to test
                _curWorkingRawRequest = requestInfo.ToString();

                
                if (!ShouldBeTested(_curWorkingRawRequest))
                {
                    return requestInfo;
                }

                _curWorkingReqInfo = new HttpRequestInfo(_curWorkingRawRequest, true);
                _curWorkingReqInfo.IsSecure = requestInfo.IsSecure;

                int hash = _curWorkingReqInfo.GetHashCode(TrafficServerMode.IgnoreCookies); //get a hash including only the parameter names

                if (_firstRequestHash == 0)
                {
                    _firstRequestHash = hash;
                }

                if (hash == _firstRequestHash)
                {
                    if (_currentReqIdx > _detectedFlowRequests)
                    {
                        _detectedFlowRequests = _currentReqIdx + 1;
                    }
                    //reset the counter
                    _currentReqIdx = 0;
                }
                else
                {
                    //increment the request counter
                    _currentReqIdx++;
                }


                if (_currentTestReqIdx < 0) //this is the first request 
                {
                    if (String.IsNullOrEmpty(_testFile.PatternOfFirstRequestToTest)) //if this is not the first request
                    {
                        _currentTestReqIdx = 0;
                    }
                    else
                    {
                        if (Utils.IsMatch(_curWorkingRawRequest, _testFile.PatternOfFirstRequestToTest))
                        {
                            _currentTestReqIdx = _currentReqIdx;
                        }
                    }
                }

               

                if (_currentReqIdx == _currentTestReqIdx)
                {
                    if (!_workList.ContainsKey(_currentTestReqIdx)) //no tests have been generated for this request
                    {
                        GenerateEntities(_currentReqIdx, _curWorkingReqInfo);
                    }
                    
                    //this is the test request execute the next test
                    Queue<TestJob> testQueue = _workList[_currentTestReqIdx];
                    if (testQueue.Count == 0 && _curMutatedRawReqQueue.Count == 0)
                    {
                        _currentTestReqIdx++;
                    }
                    else
                    {
                        if (_curMutatedRawReqQueue.Count == 0)
                        {
                            _curTestJob = testQueue.Dequeue();
                            //TODO: alter the request
                            Uri workingReqUri = new Uri(_curWorkingReqInfo.FullUrl);
                            _curEntityId = _tester.GetEntityId(workingReqUri, _curTestJob.ParameterName);

                            string entityString = _tester.GetEntityString(_curWorkingRawRequest, workingReqUri, _curTestJob.ParameterName, _curTestJob.ParameterValue);

                            if (entityString == null)//we could not find the parameter name /value combination
                            {
                                //try to obtain an updated value for the parameter
                                string updatedParameterValue = GetUpdatedParameterValue(_curWorkingReqInfo, _curTestJob);
                                entityString = _tester.GetEntityString(_curWorkingRawRequest, workingReqUri, _curTestJob.ParameterName, updatedParameterValue);
                            }
                            
                            _curMutatedRequestList = _tester.GenerateMutatedRequestList(_curWorkingRawRequest, _curTestJob, entityString, _curEntityId);
                            foreach (string req in _curMutatedRequestList)
                            {
                                _curMutatedRawReqQueue.Enqueue(req);
                            }
                            _curTestResponseCollection = new Dictionary<int, List<string>>(); //re-init the current test response list for multi response validation


                        }
                        //check again in case for some reason requests could not be generated (empty parameters)
                        if (_curMutatedRawReqQueue.Count > 0)
                        {
                            //if multiple mutations are generated for the test then they need to be executed with each flow iteration
                            _curMutatedRawReq = _curMutatedRawReqQueue.Dequeue();
                            //return the mutated request
                            requestInfo = new HttpRequestInfo(_curMutatedRawReq, false);
                            requestInfo.IsSecure = _curWorkingReqInfo.IsSecure;
                            mutated = true;
                        }

                    }
                }


            }
            return requestInfo;
        }

       

    }
}
